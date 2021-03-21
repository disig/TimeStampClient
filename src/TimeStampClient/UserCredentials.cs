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

using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Disig.TimeStampClient
{
    /// <summary>
    /// User credentials class.
    /// </summary>
    public class UserCredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserCredentials"/> class.
        /// </summary>
        /// <param name="httpCredentials">User's certificate to access a TSA service.</param>
        public UserCredentials(X509Certificate2 httpCredentials)
        {
            this.UserSslCert = httpCredentials;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCredentials"/> class.
        /// </summary>
        /// <param name="httpCredentials">User's network credential to access a TSA service.</param>
        public UserCredentials(NetworkCredential httpCredentials)
        {
            this.HttpCredentials = httpCredentials;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCredentials"/> class.
        /// </summary>
        /// <param name="userSslCert">User's certificate to access a TSA service.</param>
        /// <param name="httpCredentials">User's network credential to access a TSA service.</param>
        public UserCredentials(X509Certificate2 userSslCert, NetworkCredential httpCredentials)
            : this(httpCredentials)
        {
            this.UserSslCert = userSslCert;
        }

        /// <summary>
        /// Gets user's certificate.
        /// </summary>
        public X509Certificate2 UserSslCert
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets user's network credential.
        /// </summary>
        public NetworkCredential HttpCredentials
        {
            get;
            private set;
        }
    }
}
