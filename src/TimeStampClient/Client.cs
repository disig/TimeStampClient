/*
*  Copyright 2016-2026 Disig a.s.
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
using System.Text;

namespace Disig.TimeStampClient
{
    /// <summary>
    /// Provides methods for requesting time-stamp tokens from a TSA service according to RFC 3161.
    /// </summary>
    /// <remarks>
    /// The PKI status of the received time-stamp response is checked and the time-stamp token is checked to match the request
    /// (nonce, requested policy and message imprint). The signature of the time-stamp token and the TSA certificate are not validated.
    /// </remarks>
    public static class TimeStampClient
    {
        /// <summary>
        /// Default port of TCP-based time-stamp protocol (RFC 3161, section 3.3).
        /// </summary>
        private const int DefaultTcpPort = 318;

        /// <summary>
        /// Send and receive timeout of TCP connection in milliseconds (same as default timeout of HttpWebRequest).
        /// </summary>
        private const int TcpTimeout = 100000;

        /// <summary>
        /// Maximum accepted length of time-stamp response (HTTP response body or TCP message).
        /// </summary>
        private const int MaxResponseLength = 1024 * 1024;

        /// <summary>
        /// TCP message flag: message contains TimeStampReq.
        /// </summary>
        private const byte TcpFlagTsaMsg = 0x00;

        /// <summary>
        /// TCP message flag: message contains TimeStampResp.
        /// </summary>
        private const byte TcpFlagFinalMsgRep = 0x05;

        /// <summary>
        /// TCP message flag: message contains human readable error message.
        /// </summary>
        private const byte TcpFlagErrorMsgRep = 0x06;

        #region RequestTimeStamp

        /// <summary>
        /// Requests time-stamp for the specified time-stamp request.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service. Supported schemes are http, https and tcp (default port 318).</param>
        /// <param name="request">The time-stamp request.</param>
        /// <param name="credentials">User's credentials to access TSA service. Used only with http and https schemes.</param>
        /// <returns>Time-stamp token.</returns>
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
        /// Requests time-stamp for the data from stream.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service. Supported schemes are http, https and tcp (default port 318).</param>
        /// <param name="dataToTimestamp">Specifies the data to time-stamp.</param>
        /// <returns>Time-stamp token.</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, Stream dataToTimestamp)
        {
            return RequestTimeStampToken(tsaUri, dataToTimestamp, null, null);
        }

        /// <summary>
        /// Requests time-stamp for the data from stream.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service. Supported schemes are http, https and tcp (default port 318).</param>
        /// <param name="dataToTimestamp">Specifies the data to time-stamp.</param>
        /// <param name="credentials">User's credentials to access TSA service. Used only with http and https schemes.</param>
        /// <returns>Time-stamp token.</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, Stream dataToTimestamp, UserCredentials credentials)
        {
            return RequestTimeStampToken(tsaUri, dataToTimestamp, null, credentials);
        }

        /// <summary>
        /// Requests time-stamp for the data from stream.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service. Supported schemes are http, https and tcp (default port 318).</param>
        /// <param name="dataToTimestamp">Specifies the data to time-stamp.</param>
        /// <param name="digestType">Specifies the hash algorithm to be used to compute digest from data. SHA-512 is used if null.</param>
        /// <returns>Time-stamp token.</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, Stream dataToTimestamp, Oid digestType)
        {
            return RequestTimeStampToken(tsaUri, dataToTimestamp, digestType, null);
        }

        /// <summary>
        /// Requests time-stamp for the data from stream.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service. Supported schemes are http, https and tcp (default port 318).</param>
        /// <param name="dataToTimestamp">Specifies the data to time-stamp.</param>
        /// <param name="digestType">Specifies the hash algorithm to be used to compute digest from data. SHA-512 is used if null.</param>
        /// <param name="credentials">User's credentials to access TSA service. Used only with http and https schemes.</param>
        /// <returns>Time-stamp token.</returns>
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
            Request request = new Request(digest, digestType.OID, Utils.GenerateNonce());
            return RequestTST(tsaUri, request, credentials);
        }

        /// <summary>
        /// Requests time-stamp for the data stored in the byte array.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service. Supported schemes are http, https and tcp (default port 318).</param>
        /// <param name="dataToTimestamp">Specifies the data to time-stamp.</param>
        /// <returns>Time-stamp token.</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, byte[] dataToTimestamp)
        {
            return RequestTimeStampToken(tsaUri, dataToTimestamp, null, null);
        }

        /// <summary>
        /// Requests time-stamp for the data stored in the byte array.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service. Supported schemes are http, https and tcp (default port 318).</param>
        /// <param name="dataToTimestamp">Specifies the data to time-stamp.</param>
        /// <param name="digestType">Specifies the hash algorithm to be used to compute digest from data. SHA-512 is used if null.</param>
        /// <returns>Time-stamp token.</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, byte[] dataToTimestamp, Oid digestType)
        {
            return RequestTimeStampToken(tsaUri, dataToTimestamp, digestType, null);
        }

        /// <summary>
        /// Requests time-stamp for the data stored in the byte array.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service. Supported schemes are http, https and tcp (default port 318).</param>
        /// <param name="dataToTimestamp">Specifies the data to time-stamp.</param>
        /// <param name="credentials">User's credentials to access TSA service. Used only with http and https schemes.</param>
        /// <returns>Time-stamp token.</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, byte[] dataToTimestamp, UserCredentials credentials)
        {
            return RequestTimeStampToken(tsaUri, dataToTimestamp, null, credentials);
        }

        /// <summary>
        /// Requests time-stamp for the data stored in the byte array.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service. Supported schemes are http, https and tcp (default port 318).</param>
        /// <param name="dataToTimestamp">Specifies the data to time-stamp.</param>
        /// <param name="digestType">Specifies the hash algorithm to be used to compute digest from data. SHA-512 is used if null.</param>
        /// <param name="credentials">User's credentials to access TSA service. Used only with http and https schemes.</param>
        /// <returns>Time-stamp token.</returns>
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
        /// Requests time-stamp for the file specified by the path.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service. Supported schemes are http, https and tcp (default port 318).</param>
        /// <param name="pathToFile">Specifies the file to time-stamp.</param>
        /// <returns>Time-stamp token.</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, string pathToFile)
        {
            return RequestTimeStampToken(tsaUri, pathToFile, null, null);
        }

        /// <summary>
        /// Requests time-stamp for the file specified by the path.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service. Supported schemes are http, https and tcp (default port 318).</param>
        /// <param name="pathToFileToTimestamp">Specifies the file to time-stamp.</param>
        /// <param name="digestType">Specifies the hash algorithm to be used to compute digest from data. SHA-512 is used if null.</param>
        /// <param name="credentials">User's credentials to access TSA service. Used only with http and https schemes.</param>
        /// <returns>Time-stamp token.</returns>
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
        /// Requests time-stamp for the file specified by the path.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service. Supported schemes are http, https and tcp (default port 318).</param>
        /// <param name="pathToFileToTimestamp">Specifies the file to time-stamp.</param>
        /// <param name="digestType">Specifies the hash algorithm to be used to compute digest from data. SHA-512 is used if null.</param>
        /// <returns>Time-stamp token.</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, string pathToFileToTimestamp, Oid digestType)
        {
            return RequestTimeStampToken(tsaUri, pathToFileToTimestamp, digestType, null);
        }

        /// <summary>
        /// Requests time-stamp for the file specified by the path.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service. Supported schemes are http, https and tcp (default port 318).</param>
        /// <param name="pathToFileToTimestamp">Specifies the file to time-stamp.</param>
        /// <param name="credentials">User's credentials to access TSA service. Used only with http and https schemes.</param>
        /// <returns>Time-stamp token.</returns>
        public static TimeStampToken RequestTimeStampToken(string tsaUri, string pathToFileToTimestamp, UserCredentials credentials)
        {
            return RequestTimeStampToken(tsaUri, pathToFileToTimestamp, null, credentials);
        }

        #endregion

        /// <summary>
        /// Requests time-stamp from TSA service.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="request">Time-stamp request.</param>
        /// <param name="credentials">User's credentials to access TSA service.</param>
        /// <returns>Time-stamp token.</returns>
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

            Response response;
            try
            {
                response = new Response(responseBytes);
            }
            catch (Exception e)
            {
                throw new TimeStampException("Invalid TS response: unable to parse response", e);
            }

            ValidateResponse(request, response);
            return response.TST;
        }

        #region Connections

        /// <summary>
        /// Creates TCP request and processes TCP response.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="tsr">DER encoded time-stamp request.</param>
        /// <returns>DER encoded time-stamp response.</returns>
        private static byte[] GetTcpResponse(string tsaUri, byte[] tsr)
        {
            UriBuilder urib = new UriBuilder(tsaUri);
            int port = (-1 == urib.Port) ? DefaultTcpPort : urib.Port;

            // Message format: 4 bytes length of the remainder (big-endian), 1 byte flag, message
            byte[] request = new byte[5 + tsr.Length];
            WriteTcpHeader(request, tsr.Length + 1, TcpFlagTsaMsg);
            tsr.CopyTo(request, 5);

            using (TcpClient tcpClient = new TcpClient())
            {
                tcpClient.SendTimeout = TcpTimeout;
                tcpClient.ReceiveTimeout = TcpTimeout;
                tcpClient.Connect(urib.Host, port);

                using (NetworkStream networkStream = tcpClient.GetStream())
                {
                    networkStream.Write(request, 0, request.Length);

                    byte[] header = ReadExactly(networkStream, 5);
                    int respLen = (header[0] << 24) | (header[1] << 16) | (header[2] << 8) | header[3];
                    if (respLen < 1 || respLen > MaxResponseLength)
                    {
                        throw new TimeStampException(string.Format(CultureInfo.InvariantCulture, "Invalid TCP response: invalid message length {0}", respLen));
                    }

                    byte[] body = ReadExactly(networkStream, respLen - 1);
                    switch (header[4])
                    {
                        case TcpFlagFinalMsgRep:
                            return body;
                        case TcpFlagErrorMsgRep:
                            throw new TimeStampException("TSA returned error: " + Encoding.UTF8.GetString(body));
                        default:
                            throw new TimeStampException(string.Format(CultureInfo.InvariantCulture, "Invalid TCP response: unsupported message type 0x{0:X2}", header[4]));
                    }
                }
            }
        }

        /// <summary>
        /// Writes header of TCP-based time-stamp protocol message.
        /// </summary>
        /// <param name="buffer">Buffer to write header to.</param>
        /// <param name="length">Length of the remainder of the message (flag and message).</param>
        /// <param name="flag">Message type flag.</param>
        private static void WriteTcpHeader(byte[] buffer, int length, byte flag)
        {
            buffer[0] = (byte)(length >> 24);
            buffer[1] = (byte)(length >> 16);
            buffer[2] = (byte)(length >> 8);
            buffer[3] = (byte)length;
            buffer[4] = flag;
        }

        /// <summary>
        /// Reads exact number of bytes from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="count">Number of bytes to read.</param>
        /// <returns>Bytes read from stream.</returns>
        private static byte[] ReadExactly(Stream stream, int count)
        {
            byte[] buffer = new byte[count];
            int offset = 0;
            while (offset < count)
            {
                int read = stream.Read(buffer, offset, count - offset);
                if (0 == read)
                {
                    throw new TimeStampException("Invalid TCP response: connection closed before the whole message was received");
                }

                offset += read;
            }

            return buffer;
        }

        /// <summary>
        /// Creates HTTP request and processes HTTP response.
        /// </summary>
        /// <param name="tsaUri">URL of a TSA service.</param>
        /// <param name="tsr">DER encoded time-stamp request.</param>
        /// <param name="credentials">User's credentials to access TSA service.</param>
        /// <returns>DER encoded time-stamp response.</returns>
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

            using (Stream reqStream = httpReq.GetRequestStream())
            {
                reqStream.Write(tsr, 0, tsr.Length);
            }

            using (WebResponse httpResp = httpReq.GetResponse())
            {
                // Content type is not checked strictly because some TSAs do not send "application/timestamp-reply",
                // but textual content (e.g. HTML error page) is never a valid time stamp response
                string contentType = httpResp.ContentType;
                if (null != contentType && contentType.TrimStart().StartsWith("text/", StringComparison.OrdinalIgnoreCase))
                {
                    throw new TimeStampException(string.Format(CultureInfo.InvariantCulture, "Invalid TS response: unexpected content type {0}", contentType));
                }

                if (httpResp.ContentLength > MaxResponseLength)
                {
                    throw new TimeStampException(string.Format(CultureInfo.InvariantCulture, "Invalid TS response: response length {0} exceeds the limit", httpResp.ContentLength));
                }

                using (Stream respStream = httpResp.GetResponseStream())
                {
                    return ReadToEnd(respStream, MaxResponseLength);
                }
            }
        }

        /// <summary>
        /// Reads all bytes from stream up to the specified limit.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="maxLength">Maximum number of bytes to read.</param>
        /// <returns>Bytes read from stream.</returns>
        private static byte[] ReadToEnd(Stream stream, int maxLength)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (ms.Length + read > maxLength)
                    {
                        throw new TimeStampException("Invalid TS response: response length exceeds the limit");
                    }

                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        #endregion

        /// <summary>
        /// Validates time-stamp response against time-stamp request.
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

                if (!Utils.CompareNonces(response.TST.Nonce, request.Nonce))
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
