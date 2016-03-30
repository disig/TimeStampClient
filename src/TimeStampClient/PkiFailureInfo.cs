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
    /// Reason why the time-stamp request was rejected.
    /// </summary>
    public enum PkiFailureInfo
    {
        /// <summary>
        /// Unrecognized or unsupported Algorithm Identifier.
        /// </summary>
        BadAlg = 0,

        /// <summary>
        /// Transaction not permitted or supported.
        /// </summary>
        BadRequest = 2,

        /// <summary>
        /// The data submitted has the wrong format.
        /// </summary>
        BadDataFormat = 5,

        /// <summary>
        /// The TSA's time source is not available.
        /// </summary>
        TimeNotAvailable = 14,

        /// <summary>
        /// The requested TSA policy is not supported by the TSA.
        /// </summary>
        UnacceptedPolicy = 15,

        /// <summary>
        /// The requested extension is not supported by the TSA.
        /// </summary>
        UnacceptedExtension = 16,

        /// <summary>
        /// The additional information requested could not be understood or is not available.
        /// </summary>
        AddInfoNotAvailable = 17,

        /// <summary>
        /// The request cannot be handled due to system failure.
        /// </summary>
        SystemFailure = 25,

        /// <summary>
        /// Unknown status.
        /// </summary>
        Unknown = -1
    }
}
