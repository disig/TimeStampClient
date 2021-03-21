/*
*  Copyright 2016-2021 Disig a.s.
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
    /// Stores available information about TSA.
    /// </summary>
    public class TsaId
    {
        internal TsaId(SignerID signerID, X509Certificate2 cert)
        {
            this.TsaCertSerialNumber = signerID.SerialNumber.ToByteArray();
            this.TsaCertSubjectKeyIdentifier = signerID.SubjectKeyIdentifier;
            this.TsaCertIssuerName = new X500DistinguishedName(signerID.Issuer.GetEncoded());

            if (null != cert)
                TsaCert = cert;
        }

        /// <summary>
        /// 
        /// </summary>
        public X500DistinguishedName TsaCertIssuerName
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] TsaCertSerialNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] TsaCertSubjectKeyIdentifier
        {
            get;
            private set;
        }

        /// <summary>
        /// Signing certificate of a TSA.
        /// </summary>
        public X509Certificate2 TsaCert
        {
            get;
            private set;
        }
    }
}
