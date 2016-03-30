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
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Disig.TimeStampClient
{
    /// <summary>
    /// Provides interface to time-stamp data.
    /// </summary>
    public static class Client
    {
        #region RequestTimeStamp

        /// <summary>
        /// Requests time stamp for the specified time stamp request.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="request">The time stamp request.</param>
        /// <param name="credentials">User's credentials to access TSA service.</param>
        /// <returns>Time-stamp token</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, Request request, UserCredentials credentials = null)
        {
            if (null == request)
            {
                throw new ArgumentNullException("request");
            }

            if (null == tsaUri)
            {
                throw new ArgumentNullException("tsaUri");
            }

            return RequestTST(tsaUri, request, credentials);
        }

        /// <summary>
        /// Requests time stamp for the data from stream.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="dataToTimestamp">Specifies the data to time-stamp.</param>
        /// <returns>Time-stamp token</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, Stream dataToTimestamp)
        {
            return RequestTimeStampToken(tsaUri, dataToTimestamp, null, null);
        }

        /// <summary>
        /// Requests time stamp for the data from stream.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="dataToTimestamp">Specifies the data to time-stamp.</param>
        /// <param name="credentials">User's credentials to access TSA service.</param>
        /// <returns>Time-stamp token</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, Stream dataToTimestamp, UserCredentials credentials)
        {
            return RequestTimeStampToken(tsaUri, dataToTimestamp, null, credentials);
        }

        /// <summary>
        /// Requests time stamp for the data from stream.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="dataToTimestamp">Specifies the data to time-stamp.</param>
        /// <param name="digestType">Specifies the hash algorithm to be used to compute digest from data.</param>
        /// <returns>Time-stamp token</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, Stream dataToTimestamp, Oid digestType)
        {
            return RequestTimeStampToken(tsaUri, dataToTimestamp, digestType, null);
        }

        /// <summary>
        /// Requests time stamp for the data from stream.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="dataToTimestamp">Specifies the data to time-stamp.</param>
        /// <param name="digestType">Specifies the hash algorithm to be used to compute digest from data.</param>
        /// <param name="credentials">User's credentials to access TSA service.</param>
        /// <returns>Time-stamp token</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, Stream dataToTimestamp, Oid digestType, UserCredentials credentials)
        {
            if (null == tsaUri)
            {
                throw new ArgumentNullException("tsaUri");
            }

            if (null == dataToTimestamp)
            {
                throw new ArgumentNullException("dataToTimestamp");
            }

            if (null == digestType)
            {
                digestType = Oid.SHA512;
            }

            byte[] digest = DigestUtils.ComputeDigest(dataToTimestamp, digestType);
            Request request = new Request(digest, digestType.OID);
            return RequestTST(tsaUri, request, credentials);
        }

        /// <summary>
        /// Requests time stamp for the data stored in the byte array.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="dataToTimestamp">Specifies the data to time-stamp.</param>
        /// <returns>Time-stamp token</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, byte[] dataToTimestamp)
        {
            return RequestTimeStampToken(tsaUri, dataToTimestamp, null, null);
        }

        /// <summary>
        /// Requests time stamp for the data stored in the byte array.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="dataToTimestamp">Specifies the data to time-stamp.</param>
        /// <param name="digestType">Specifies the hash algorithm to be used to compute digest from data.</param>
        /// <returns>Time-stamp token</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, byte[] dataToTimestamp, Oid digestType)
        {
            return RequestTimeStampToken(tsaUri, dataToTimestamp, digestType, null);
        }

        /// <summary>
        /// Requests time stamp for the data stored in the byte array.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="dataToTimestamp">Specifies the data to time-stamp.</param>
        /// <param name="credentials">User's credentials to access TSA service.</param>
        /// <returns>Time-stamp token</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, byte[] dataToTimestamp, UserCredentials credentials)
        {
            return RequestTimeStampToken(tsaUri, dataToTimestamp, null, credentials);
        }

        /// <summary>
        /// Requests time stamp for the data stored in the byte array.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="dataToTimestamp">Specifies the data to time-stamp.</param>
        /// <param name="digestType">Specifies the hash algorithm to be used to compute digest from data.</param>
        /// <param name="credentials">User's credentials to access TSA service.</param>
        /// <returns>Time-stamp token</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, byte[] dataToTimestamp, Oid digestType, UserCredentials credentials)
        {
            if (null == dataToTimestamp)
            {
                throw new ArgumentNullException("dataToTimestamp");
            }

            using (MemoryStream ms = new MemoryStream(dataToTimestamp))
            {
                return RequestTimeStampToken(tsaUri, ms, digestType, credentials);
            }
        }

        /// <summary>
        /// Requests time stamp for the file specified by the path.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="pathToFile">Specifies the file to time-stamp.</param>
        /// <returns>Time-stamp token</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, string pathToFile)
        {
            return RequestTimeStampToken(tsaUri, pathToFile, null, null);
        }

        /// <summary>
        /// Requests time stamp for the file specified by the path.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="pathToFileToTimestamp">Specifies the file to time-stamp.</param>
        /// <param name="digestType">Specifies the hash algorithm to be used to compute digest from data.</param>
        /// <param name="credentials">User's credentials to access TSA service.</param>
        /// <returns>Time-stamp token</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, string pathToFileToTimestamp, Oid digestType, UserCredentials credentials)
        {
            if (null == pathToFileToTimestamp)
            {
                throw new ArgumentNullException("pathToFileToTimestamp");
            }

            using (FileStream fs = new FileStream(pathToFileToTimestamp, FileMode.Open, FileAccess.Read))
            {
                return RequestTimeStampToken(tsaUri, fs, digestType, credentials);
            }
        }

        /// <summary>
        /// Requests time stamp for the file specified by the path.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="pathToFileToTimestamp">Specifies the file to time-stamp.</param>
        /// <param name="digestType">Specifies the hash algorithm to be used to compute digest from data.</param>
        /// <returns>Time-stamp token</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, string pathToFileToTimestamp, Oid digestType)
        {
            return RequestTimeStampToken(tsaUri, pathToFileToTimestamp, digestType, null);
        }

        /// <summary>
        /// Requests time stamp for the file specified by the path.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="pathToFileToTimestamp">Specifies the file to time-stamp.</param>
        /// <param name="credentials">User's credentials to access TSA service.</param>
        /// <returns>Time-stamp token</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, string pathToFileToTimestamp, UserCredentials credentials)
        {
            return RequestTimeStampToken(tsaUri, pathToFileToTimestamp, null, credentials);
        }

        #endregion

        /// <summary>
        /// Requests time-stamp from TSA service
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="request">Time-stamp request.</param>
        /// <param name="credentials">User's credentials to access TSA service.</param>
        /// <returns>Time-stamp token</returns>
        private static TimeStampToken RequestTST(string tsaUri, Request request, UserCredentials credentials = null)
        {
            byte[] responseBytes = null;
            UriBuilder urib = new UriBuilder(tsaUri);

            switch (urib.Uri.Scheme)
            {
                case "http":
                case "https":
                    responseBytes = GetHttpResponse(tsaUri, request.ToByteArray(), credentials);
                    break;
                case "tcp":
                    responseBytes = GetTcpResponse(tsaUri, request.ToByteArray());
                    break;
                default:
                    throw new TimeStampException("Unknown protocol.");
            }

            Response response = new Response(responseBytes);
            ValidateResponse(request, response);
            return response.TST;
        }

        #region Connections

        /// <summary>
        /// Creates TCP request and processes TCP response.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="tsr">DER encoded time stamp request.</param>
        /// <returns>DER encoded time stamp response.</returns>
        private static byte[] GetTcpResponse(string tsaUri, byte[] tsr)
        {
            Stream returnStream = null;
            UriBuilder urib = new UriBuilder(tsaUri);

            byte[] lenByte = new byte[] { 0, 0, 0, 0, 0 };
            int reqLen = tsr.Length + 1;
            lenByte[0] = (byte)(reqLen >> 24);
            lenByte[1] = (byte)(reqLen >> 16 & 255);
            lenByte[2] = (byte)(reqLen >> 8 & 255);
            lenByte[3] = (byte)(reqLen & 255);
            byte[] request = new byte[tsr.Length + lenByte.Length];

            lenByte.CopyTo(request, 0);
            tsr.CopyTo(request, lenByte.Length);

            using (TcpClient tcpClnt = new TcpClient())
            {
                tcpClnt.Connect(urib.Host, urib.Port);
                if (tcpClnt.Connected)
                {
                    using (MemoryStream respStream = new MemoryStream())
                    {
                        using (NetworkStream stm = tcpClnt.GetStream())
                        {
                            stm.Write(request, 0, request.Length);
                            byte[] buff = new byte[1024];
                            int read;
                            int offset = 5;

                            while (0 < (read = stm.Read(buff, 0, buff.Length)))
                            {
                                respStream.Write(buff, offset, read - offset);
                                offset = 0;
                            }
                        }

                        returnStream = new BufferedStream(respStream);
                        tcpClnt.Close();
                        if (returnStream != null)
                        {
                            returnStream.Position = 0;
                        }

                        byte[] buffer = new byte[16 * 1024];
                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;
                            while ((read = returnStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, read);
                            }

                            returnStream.Close();
                            return ms.ToArray();
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Creates HTTP request and processes HTTP response.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="tsr">DER encoded time stamp request.</param>
        /// <param name="credentials">User's credentials to access TSA service.</param>
        /// <returns>DER encoded time stamp response</returns>
        private static byte[] GetHttpResponse(string tsaUri, byte[] tsr, UserCredentials credentials = null)
        {
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(tsaUri);
            httpReq.Method = "POST";
            httpReq.ContentType = "application/timestamp-query";

            if (null != credentials)
            {
                if (null != credentials.UserSslCert)
                {
                    httpReq.ClientCertificates.Add(credentials.UserSslCert);
                }

                if (null != credentials.HttpCredentials)
                {
                    httpReq.Credentials = credentials.HttpCredentials;
                }
            }

            Stream reqStream = httpReq.GetRequestStream();
            reqStream.Write(tsr, 0, tsr.Length);
            reqStream.Close();
            WebResponse httpResp = httpReq.GetResponse();
            Stream respStream = httpResp.GetResponseStream();

            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = respStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                respStream.Close();
                return ms.ToArray();
            }
        }

        #endregion

        /// <summary>
        /// Validates time stamp response against time stamp request.
        /// </summary>
        /// <param name="request">Time-stamp request.</param>
        /// <param name="response">Time-stamp response.</param>
        private static void ValidateResponse(Request request, Response response)
        {
            if (PkiStatus.Granted == response.PKIStatus || PkiStatus.GrantedWithMods == response.PKIStatus)
            {
                if (null == response.TST)
                {
                    throw new TimeStampException("Invalid TS response: missing time stamp token", response.PKIStatus);
                }

                if (!Utils.CompareByteArray(response.TST.Nonce, request.Nonce))
                {
                    throw new TimeStampException("Invalid TS response: nonce mismatch", response.PKIStatus);
                }

                if (!string.IsNullOrEmpty(request.ReqPolicy) && 0 != string.CompareOrdinal(response.TST.PolicyOid, request.ReqPolicy))
                {
                    throw new TimeStampException("Invalid TS response: policy mismatch", response.PKIStatus);
                }

                if (!MsgImprint.CompareImprints(response.TST.MessageImprint, request.MessageImprint))
                {
                    throw new TimeStampException("Invalid TS response: message imprint mismatch", response.PKIStatus);
                }
            }
            else
            {
                throw new TimeStampException(string.Format(CultureInfo.InvariantCulture, "Invalid TS response. Response status: {0}", response.PKIStatus), response.PKIStatus, response.PKIStatusString, response.PKIFailureInfo);
            }
        }
    }
}
