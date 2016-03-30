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

using Org.BouncyCastle.Tsp;
using System;

namespace Disig.TimeStampClient
{
    /// <summary>
    /// Time stamp response
    /// </summary>
    internal class Response
    {
        #region Timestamp reponse according to RFC3161
        /*
            TimeStampResp::= SEQUENCE  {
                status          PKIStatusInfo,
                timeStampToken  TimeStampToken OPTIONAL  }

            PKIStatusInfo ::= SEQUENCE {
                status        PKIStatus,
                statusString  PKIFreeText     OPTIONAL,
                failInfo      PKIFailureInfo  OPTIONAL  }

            PKIStatus ::= INTEGER {
                granted                (0),
                    -- when the PKIStatus contains the value zero a TimeStampToken, as
                requested, is present.
                grantedWithMods        (1),
                    -- when the PKIStatus contains the value one a TimeStampToken,
                with modifications, is present.
                rejection              (2),
                waiting                (3),
                revocationWarning      (4),
                    -- this message contains a warning that a revocation is imminent
                revocationNotification (5)
                    -- notification that a revocation has occurred  }


            PKIFailureInfo ::= BIT STRING {
                badAlg               (0),
                    -- unrecognized or unsupported Algorithm Identifier
                badRequest           (2),
                    -- transaction not permitted or supported
                badDataFormat        (5),
                    -- the data submitted has the wrong format
                timeNotAvailable    (14),
                    -- the TSA's time source is not available
                unacceptedPolicy    (15),
                    -- the requested TSA policy is not supported by the TSA
                unacceptedExtension (16),
                    -- the requested extension is not supported by the TSA
                addInfoNotAvailable (17)
                    -- the additional information requested could not be understood
                    -- or is not available
                systemFailure       (25)
                    -- the request cannot be handled due to system failure  }


            TSTInfo ::= SEQUENCE  {
               version                      INTEGER  { v1(1) },
               policy                       TSAPolicyId,
               messageImprint               MessageImprint,
                 -- MUST have the same value as the similar field in
                 -- TimeStampReq
               serialNumber                 INTEGER,
                -- Time-Stamping users MUST be ready to accommodate integers
                -- up to 160 bits.
               genTime                      GeneralizedTime,
               accuracy                     Accuracy                 OPTIONAL,
               ordering                     BOOLEAN             DEFAULT FALSE,
               nonce                        INTEGER                  OPTIONAL,
                 -- MUST be present if the similar field was present
                 -- in TimeStampReq.  In that case it MUST have the same value.
               tsa                          [0] GeneralName          OPTIONAL,
               extensions                   [1] IMPLICIT Extensions   OPTIONAL  }



            Accuracy ::= SEQUENCE {
                seconds        INTEGER              OPTIONAL,
                millis     [0] INTEGER  (1..999)    OPTIONAL,
                micros     [1] INTEGER  (1..999)    OPTIONAL  }
        */
        #endregion

        /// <summary>
        /// RFC 3161 Time Stamp Response object.
        /// </summary>
        private TimeStampResponse response;

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="response">DER encoded time stamp response.</param>
        public Response(byte[] response)
        {
            if (null == response)
            {
                throw new ArgumentNullException("response");
            }

            this.response = new TimeStampResponse(response);

            if (null != this.response.TimeStampToken)
            {
                // NOTICE: this.response.TimeStampToken.GetEncoded() returns malformed byte array; therefore, we use low level interface
                // this.TST = new TimeStampToken(this.response.TimeStampToken.GetEncoded());
                Org.BouncyCastle.Asn1.Tsp.TimeStampResp asn1Response = Org.BouncyCastle.Asn1.Tsp.TimeStampResp.GetInstance(Org.BouncyCastle.Asn1.Asn1Sequence.FromByteArray(response));
                var derTst = asn1Response.TimeStampToken.GetDerEncoded();

                this.TST = new TimeStampToken(derTst);
            }
        }

        /// <summary>
        /// Gets PKIStatus
        /// </summary>
        public PkiStatus PKIStatus
        {
            get
            {
                return (PkiStatus)this.response.Status;
            }
        }

        /// <summary>
        /// Gets additional status information.
        /// </summary>
        internal string PKIStatusString
        {
            get
            {
                return this.response.GetStatusString();
            }
        }

        /// <summary>
        /// Gets the reason why the time-stamp request was rejected.
        /// </summary>
        internal PkiFailureInfo? PKIFailureInfo
        {
            get
            {
                if (null != this.response.GetFailInfo())
                {
                    PkiFailureInfo res;
                    switch (this.response.GetFailInfo().IntValue)
                    {
                        case Org.BouncyCastle.Asn1.Cmp.PkiFailureInfo.BadAlg:
                            res = PkiFailureInfo.BadAlg;
                            break;
                        case Org.BouncyCastle.Asn1.Cmp.PkiFailureInfo.BadRequest:
                            res = PkiFailureInfo.BadRequest;
                            break;
                        case Org.BouncyCastle.Asn1.Cmp.PkiFailureInfo.BadDataFormat:
                            res = PkiFailureInfo.BadDataFormat;
                            break;
                        case Org.BouncyCastle.Asn1.Cmp.PkiFailureInfo.TimeNotAvailable:
                            res = PkiFailureInfo.TimeNotAvailable;
                            break;
                        case Org.BouncyCastle.Asn1.Cmp.PkiFailureInfo.UnacceptedPolicy:
                            res = PkiFailureInfo.UnacceptedPolicy;
                            break;
                        case Org.BouncyCastle.Asn1.Cmp.PkiFailureInfo.UnacceptedExtension:
                            res = PkiFailureInfo.UnacceptedExtension;
                            break;
                        case Org.BouncyCastle.Asn1.Cmp.PkiFailureInfo.AddInfoNotAvailable:
                            res = PkiFailureInfo.AddInfoNotAvailable;
                            break;
                        case Org.BouncyCastle.Asn1.Cmp.PkiFailureInfo.SystemFailure:
                            res = PkiFailureInfo.SystemFailure;
                            break;
                        default:
                            res = PkiFailureInfo.Unknown;
                            break;
                    }
                    return res;
                }
                return null;
            }
        }

        internal TimeStampToken TST
        {
            get;
            set;
        }
    }
}
