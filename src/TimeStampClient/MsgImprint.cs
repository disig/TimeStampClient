/*
*  Copyright 2016-2019 Disig a.s.
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

namespace Disig.TimeStampClient
{
    /// <summary>
    /// Encapsulates the hashed message and the hash algorithm identifier.
    /// </summary>
    internal class MsgImprint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MsgImprint"/> class.
        /// </summary>
        /// <param name="hashedMessage">The hashed message.</param>
        /// <param name="hashAlgOid">The hash algorithm identifier.</param>
        internal MsgImprint(byte[] hashedMessage, string hashAlgOid)
        {
            this.HashedMessage = hashedMessage;
            this.HashAlgorithm = hashAlgOid;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MsgImprint"/> class.
        /// </summary>
        /// <param name="hashedMessage">The hashed message.</param>
        /// <param name="oid">The hash algorithm identifier.</param>
        internal MsgImprint(byte[] hashedMessage, Oid oid)
        {
            this.HashAlgorithm = oid.OID;
            this.HashedMessage = hashedMessage;
        }

        /// <summary>
        /// Gets the hash identifier.
        /// </summary>
        internal string HashAlgorithm
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the The hashed message.
        /// </summary>
        internal byte[] HashedMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// Compares two message imprints.
        /// </summary>
        /// <param name="a">The first message imprint.</param>
        /// <param name="b">The second message imprint.</param>
        /// <returns>True if message imprints are identical, otherwise false.</returns>
        internal static bool CompareImprints(MsgImprint a, MsgImprint b)
        {
            if ((null == a) && (null == b))
            {
                return true;
            }

            if ((null == a && null != b) || (null != a && null == b))
            {
                return false;
            }

            if (0 != string.CompareOrdinal(a.HashAlgorithm, b.HashAlgorithm))
            {
                return false;
            }

            if (false == Utils.CompareByteArray(a.HashedMessage, b.HashedMessage))
            {
                return false;
            }

            return true;
        }
    }
}
