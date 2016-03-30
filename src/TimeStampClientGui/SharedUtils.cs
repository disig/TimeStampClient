/*
*  Copyright 2016 Disig a.s.
*
*  Licensed under the Apache License, Version 2.0 (the "License");
*  you may not use this file except in compliance with the License.
*  You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License.
*/

/*
*  Written by:
*  Marek KLEIN <kleinmrk@gmail.com>
*/

using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Disig.TimeStampClient
{
    public delegate void LogDelegate(string msg);

    public static class SharedUtils
    {
        private const string DateTimeFormat = "dd MMM yyyy HH':'mm':'ss 'GMT'";
        private static string appVersion = null;

        public enum ResultFormat
        {
            RawTST,
            ASIC_S
        }

        public static string AppVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                if (string.IsNullOrEmpty(appVersion))
                {
                    appVersion = assembly.GetName().Version.ToString();
                }

                return appVersion;
            }
        }

        public static TimeStampToken RequestTimeStamp(string tsaService, string fileName, string hashAlg, string policy, string nonce, bool certReq, UserCredentials credentials, LogDelegate logger, bool logExceptions)
        {
            Oid hashOid = null;
            switch (hashAlg)
            {
                case "sha1":
                    hashOid = Oid.SHA1;
                    break;
                case "sha256":
                    hashOid = Oid.SHA256;
                    break;
                case "sha512":
                    hashOid = Oid.SHA512;
                    break;
                case "md5":
                    hashOid = Oid.MD5;
                    break;
                default:
                    hashOid = Oid.SHA256;
                    break;
            }

            return RequestTimeStamp(tsaService, fileName, hashOid, policy, nonce, certReq, credentials, logger, logExceptions);
        }

        public static TimeStampToken RequestTST(string fileName, string tsaService, Oid hashAlg, string policy, string nonce, bool certReq, UserCredentials credentials)
        {
            byte[] nonce_bytes = null;
            byte[] hashedMessage = DigestUtils.ComputeDigest(fileName, hashAlg);
            if (!string.IsNullOrEmpty(nonce))
            {
                nonce_bytes = SharedUtils.HexStringToBytes(nonce);
            }

            Request request = new Request(hashedMessage, hashAlg, nonce_bytes, policy, certReq);
            return TimeStampClient.RequestTimeStampToken(tsaService, request, credentials);
        }

        public static void SaveResponse(string destinationFile, TimeStampToken timeStampToken)
        {
            if (string.IsNullOrEmpty(destinationFile))
            {
                throw new ArgumentNullException("destinationFile");
            }

            if (null == timeStampToken)
            {
                throw new ArgumentNullException("timeStampToken");
            }

            System.IO.File.WriteAllBytes(destinationFile, timeStampToken.ToByteArray());
        }

        public static string BytesToHexString(byte[] value)
        {
            return BitConverter.ToString(value).Replace("-", string.Empty);
        }

        public static byte[] HexStringToBytes(string value)
        {
            if (value == null)
            {
                return null;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"\A\b[0-9a-fA-F]+\b\Z"))
            {
                throw new ArgumentException("Nonce field can contain only hex characters");
            }

            if (0 != value.Length % 2)
            {
                value = string.Format("0{0}", value);
            }

            byte[] bytes = new byte[value.Length / 2];

            for (int i = 0; i < value.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(value.Substring(i, 2), 16);
            }

            return bytes;
        }

        public static TimeStampToken RequestTimeStamp(
            string tsaAddress,
            string fileToTimestamp,
            Oid hashAlg,
            string requestedPolicy,
            string nonce,
            bool certReq,
            UserCredentials credentials,
            LogDelegate logger,
            bool logExceptions)
        {
            try
            {
                if (logger == null)
                {
                    return null;
                }

                logger(string.Format("=== {0:" + DateTimeFormat + "} =============================================", DateTime.UtcNow));
                logger(string.Format("Requesting time stamp from {0}", tsaAddress));

                TimeStampToken token = SharedUtils.RequestTST(fileToTimestamp, tsaAddress, hashAlg, requestedPolicy, nonce, certReq, credentials);
                logger(string.Format("Time stamp successfully received:"));
                logger(string.Format("    Serial number: {0}", SharedUtils.BytesToHexString(token.SerialNumber)));

                if (null != token.Time)
                {
                    logger(string.Format("    Time: {0:dd MMM yyyy HH':'mm':'ss 'GMT'}", (token.Time).ToUniversalTime()));
                }

                logger(string.Format("TSA certificate:"));
                logger(string.Format("    Issuer: {0}", token.TsaInformation.TsaCertIssuerName.Name));
                logger(string.Format("    Serial: {0}", SharedUtils.BytesToHexString(token.TsaInformation.TsaCertSerialNumber)));

                if (null != token.TsaInformation.TsaCert)
                {
                    logger(string.Format("    Subject: {0}", token.TsaInformation.TsaCert.Subject));
                    logger(string.Format("    Valid from: {0}", token.TsaInformation.TsaCert.NotBefore));
                    logger(string.Format("    Valid to: {0}", token.TsaInformation.TsaCert.NotAfter));
                }

                return token;
            }
            catch (Exception e)
            {
                logger(string.Format("Error occurred:"));
                if (logExceptions)
                {
                    logger(e.ToString());
                }
                else
                {
                    logger(e.Message);
                }

                throw;
            }
        }

        public static void SaveInFormat(string fileName, TimeStampToken timeStampToken, ResultFormat format, string outFile)
        {
            if (format == ResultFormat.ASIC_S)
            {
                SharedUtils.SaveToAsicSimple(fileName, timeStampToken, outFile);
            }
            else
            {
                SharedUtils.SaveResponse(outFile, timeStampToken);
            }
        }

        public static byte[] GenerateNonceBytes()
        {
            byte[] nonce = new byte[10];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(nonce);
                return nonce;
            }
        }

        public static void SaveToAsicSimple(string inputFile, TimeStampToken timeStampToken, string outputFile)
        {
            using (Ionic.Zip.ZipFile zipFile = new Ionic.Zip.ZipFile(UTF8Encoding.UTF8))
            { 
                zipFile.ParallelDeflateThreshold = -1;
                zipFile.UseZip64WhenSaving = Ionic.Zip.Zip64Option.Never;
                zipFile.EmitTimesInUnixFormatWhenSaving = false;
                zipFile.EmitTimesInWindowsFormatWhenSaving = false;
                zipFile.Comment = @"mimetype=application/vnd.etsi.asic-s+zip";

                using (System.IO.FileStream inputStream = new System.IO.FileStream(inputFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                using (System.IO.FileStream outputStream = new System.IO.FileStream(outputFile, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    zipFile.AddEntry(@"mimetype", System.Text.UTF8Encoding.UTF8.GetBytes(@"application/vnd.etsi.asic-s+zip"));
                    zipFile.AddEntry(System.IO.Path.GetFileName(inputFile), inputStream);
                    zipFile.AddEntry(@"META-INF/timestamp.tst", timeStampToken.ToByteArray());
                    zipFile.Save(outputStream);
                }
            }
        }
    }
}
