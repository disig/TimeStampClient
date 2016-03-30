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

namespace Disig.TimeStampClient
{
    /// <summary>
    /// PKI statuses according to RFC 3161
    /// </summary>
    public enum PkiStatus
    {
        /// <summary>
        /// When the PKIStatus contains the value zero a TimeStampToken, as requested, is present.
        /// </summary>
        Granted = 0,

        /// <summary>
        /// When the PKIStatus contains the value one a TimeStampToken, with modifications, is present.
        /// </summary>
        GrantedWithMods = 1,

        /// <summary>
        /// When the PKIStatus contains the value two a TimeStamp request was rejected.
        /// </summary>
        Rejection = 2,

        /// <summary>
        /// The request body part has not yet been processed, expect to hear more later.
        /// </summary>
        Waiting = 3,

        /// <summary>
        /// A warning that a revocation is imminent.
        /// </summary>
        RevocationWarning = 4,

        /// <summary>
        /// Revocation has occurred.
        /// </summary>
        RevocationNotification = 5
    }
}
