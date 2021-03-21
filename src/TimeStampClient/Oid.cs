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

using System.Collections.Generic;

namespace Disig.TimeStampClient
{
    /// <summary>
    /// Object identifiers.
    /// </summary>
    public sealed class Oid
    {
        /// <summary>
        /// Object identifier for the SHA1 algorithm.
        /// </summary>
        public static readonly Oid SHA1 = new Oid("1.3.14.3.2.26");

        /// <summary>
        /// Object identifier for the SHA512 algorithm.
        /// </summary>
        public static readonly Oid SHA512 = new Oid("2.16.840.1.101.3.4.2.3");

        /// <summary>
        /// Object identifier for the MD5 algorithm.
        /// </summary>
        public static readonly Oid MD5 = new Oid("1.2.840.113549.2.5");

        /// <summary>
        /// Object identifier for the SHA256 algorithm.
        /// </summary>
        public static readonly Oid SHA256 = new Oid("2.16.840.1.101.3.4.2.1");

        /// <summary>
        /// Initializes a new instance of the <see cref="Oid"/> class.
        /// </summary>
        /// <param name="oid">Object identifier.</param>
        private Oid(string oid)
        {
            this.OID = oid;
        }

        /// <summary>
        /// Gets object identifier.
        /// </summary>
        public string OID
        {
            get;
            private set;
        }
    }
}
