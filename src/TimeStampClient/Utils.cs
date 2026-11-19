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

using System.Security.Cryptography;
using Org.BouncyCastle.Math;

namespace Disig.TimeStampClient
{
    /// <summary>
    /// Utility methods.
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Compares byte arrays.
        /// </summary>
        /// <param name="a">first byte array</param>
        /// <param name="b">second byte array</param>
        /// <returns>true if arrays are identical. Otherwise returns false.</returns>
        internal static bool CompareByteArray(byte[] a, byte[] b)
        {
            if (null == a && null == b)
            {
                return true;
            }

            if ((null == a && null != b) || (null != a && null == b))
            {
                return false;
            }

            if (a.Length != b.Length)
            {
                return false;
            }

            int i = 0;
            while (i < a.Length && a[i] == b[i])
            {
                i++;
            }

            if (i != a.Length)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Compares nonces as integers so that differences in their encoding (e.g. redundant leading bytes) are ignored.
        /// </summary>
        /// <param name="a">first nonce</param>
        /// <param name="b">second nonce</param>
        /// <returns>true if nonces are equal or both are missing. Otherwise returns false.</returns>
        internal static bool CompareNonces(byte[] a, byte[] b)
        {
            if (null == a && null == b)
            {
                return true;
            }

            if (null == a || null == b || 0 == a.Length || 0 == b.Length)
            {
                return false;
            }

            return new BigInteger(a).Equals(new BigInteger(b));
        }

        /// <summary>
        /// Generates random nonce.
        /// </summary>
        /// <returns>Nonce bytes representing a positive integer.</returns>
        internal static byte[] GenerateNonce()
        {
            byte[] nonce = new byte[10];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(nonce);
            }

            // Clear the sign bit so that the nonce is a positive integer
            nonce[0] &= 0x7F;

            return nonce;
        }
    }
}
