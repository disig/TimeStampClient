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

using System;
using System.Runtime.Serialization;

namespace Disig.TimeStampClient
{
    /// <summary>
    /// Time stamp exception class.
    /// </summary>
    [Serializable]
    public class TimeStampException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeStampException"/> class.
        /// </summary>
        public TimeStampException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeStampException"/> class with the specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TimeStampException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeStampException"/> class with the specified error message and nested exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="e">A nested Exception.</param>
        public TimeStampException(string message, Exception e)
            : base(message, e)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeStampException"/> class with the specified PKI status.
        /// </summary>
        /// <param name="pkiStatus">PKI status.</param>
        public TimeStampException(PkiStatus pkiStatus)
            : base()
        {
            this.PKIStatus = pkiStatus;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeStampException"/> class with the specified error message, PKI status code, PKI status string and PKI failure info.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="pkiStatus">PKI status.</param>
        /// <param name="pkiStatusString">PKI status string.</param>
        /// <param name="pkiFailureInfo">PKI failure info.</param>
        public TimeStampException(string message, PkiStatus pkiStatus, string pkiStatusString, PkiFailureInfo? pkiFailureInfo)
            : base(message)
        {
            this.PKIStatus = pkiStatus;
            this.PKIStatusString = pkiStatusString;
            this.PKIFailureInfo = pkiFailureInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeStampException"/> class with the specified error message, nested exception and PKI status code.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="statusCode">PKI status code.</param>
        public TimeStampException(string message, PkiStatus statusCode)
            : base(message)
        {
            this.PKIStatus = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeStampException"/> class with the specified error message, nested exception and PKI status code.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="e">A nested Exception.</param>
        /// <param name="statusCode">PKI status code.</param>
        public TimeStampException(string message, Exception e, PkiStatus statusCode)
            : base(message, e)
        {
            this.PKIStatus = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeStampException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        protected TimeStampException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (null != info)
            {
                this.PKIStatus = (PkiStatus)info.GetInt32("PKIStatus");
                this.PKIStatusString = info.GetString("PKIStatusString");
                this.PKIFailureInfo = (PkiFailureInfo)info.GetInt32("PKIFailureInfo");
            }
        }

        /// <summary>
        /// Gets or sets PKIStatus.
        /// </summary>
        public PkiStatus PKIStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets PKIStatusString. 
        /// </summary>
        public string PKIStatusString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets PKIFailure info. If present, indicates the error that occurred while time-stamping data.
        /// </summary>
        public PkiFailureInfo? PKIFailureInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The SerializationInfo to populate with data.</param>
        /// <param name="context">The destination for this serialization.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (null != info)
            {
                info.AddValue("PKIStatus", this.PKIStatus);
                info.AddValue("PKIStatusString", this.PKIStatusString);
                info.AddValue("PKIFailureInfo", this.PKIFailureInfo);
            }

            base.GetObjectData(info, context);
        }
    }
}
