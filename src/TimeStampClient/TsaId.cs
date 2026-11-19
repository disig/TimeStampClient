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

using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Cms;

namespace Disig.TimeStampClient
{
    /// <summary>
    /// Stores available information about the TSA that signed the time-stamp token.
    /// </summary>
    public class TsaId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TsaId"/> class.
        /// </summary>
        /// <param name="signerID">Identifier of the signer of the time-stamp token.</param>
        /// <param name="cert">Signing certificate of the TSA, or null if not available.</param>
        internal TsaId(SignerID signerID, X509Certificate2 cert)
        {
            this.TsaCertSerialNumber = signerID.SerialNumber.ToByteArray();
            this.TsaCertSubjectKeyIdentifier = signerID.SubjectKeyIdentifier;
            this.TsaCertIssuerName = new X500DistinguishedName(signerID.Issuer.GetEncoded());

            if (null != cert)
                TsaCert = cert;
        }

        /// <summary>
        /// Gets the issuer name of the TSA certificate used to sign the time-stamp token.
        /// </summary>
        public X500DistinguishedName TsaCertIssuerName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the serial number of the TSA certificate used to sign the time-stamp token.
        /// </summary>
        public byte[] TsaCertSerialNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the subject key identifier of the TSA certificate used to sign the time-stamp token,
        /// or null if the signer is identified by issuer name and serial number.
        /// </summary>
        public byte[] TsaCertSubjectKeyIdentifier
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the signing certificate of the TSA, or null if the time-stamp token does not contain it
        /// (see <see cref="Request.CertReq"/>).
        /// </summary>
        public X509Certificate2 TsaCert
        {
            get;
            private set;
        }
    }
}
