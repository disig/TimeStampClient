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

using System.IO;
using System.Security.Cryptography;

namespace Disig.TimeStampClient
{
    /// <summary>
    /// Utils functions.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Computes digest of the data.
        /// </summary>
        /// <param name="dataStream">Data stream to compute digest for.</param>
        /// <param name="digestType">Defines hash algorithm to be used to compute digest.</param>
        /// <returns>digest stored in a byte array</returns>
        public static byte[] ComputeDigest(Stream dataStream, Oid digestType)
        {
            HashAlgorithm digestAlg = null;
            try
            {
                if (Oid.MD5 == digestType)
                {
                    digestAlg = new MD5CryptoServiceProvider();
                }
                else if (Oid.SHA1 == digestType)
                {
                    digestAlg = new SHA1CryptoServiceProvider();
                }
                else if (Oid.SHA256 == digestType)
                {
                    digestAlg = new SHA256Managed();
                }
                else if (Oid.SHA512 == digestType)
                {
                    digestAlg = new SHA512Managed();
                }
                else
                {
                    throw new CryptographicException("Unsupported hash algorithm.");
                }

                return digestAlg.ComputeHash(dataStream);
            }
            finally
            {
                digestAlg.Dispose();
            }
        }

        /// <summary>
        /// Computes digest of the file.
        /// </summary>
        /// <param name="pathToFile">Path to file to compute digest for.</param>
        /// <param name="digestType">Defines hash algorithm to be used to compute digest.</param>
        /// <returns>digest stored in a byte array</returns>
        public static byte[] ComputeDigest(string pathToFile, Oid digestType)
        {
            using (FileStream fs = new FileStream(pathToFile, FileMode.Open, FileAccess.Read))
            {
                return ComputeDigest(fs, digestType);
            }
        }

        /// <summary>
        /// Computes digest of the data.
        /// </summary>
        /// <param name="data">Data to compute digest for.</param>
        /// <param name="digestType">Defines hash algorithm to be used to compute digest.</param>
        /// <returns>digest stored in a byte array</returns>
        public static byte[] ComputeDigest(byte[] data, Oid digestType)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return ComputeDigest(ms, digestType);
            }
        }

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
    }
}
