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

namespace Disig.TimeStampClient
{
    /// <summary>
    /// Utils functions.
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
    }
}
