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

using System.Runtime.Serialization;

namespace Disig.TimeStampClient.Gui
{
    [DataContract(Namespace = "http://disig.sk/TimeStampClient/1.0.0")]
    public class ClientSettings
    {
        [DataMember]
        public string Nonce
        {
            get;
            set;
        }

        [DataMember]
        public string HashAlgorithm
        {
            get;
            set;
        }

        [DataMember]
        public string Policy
        {
            get;
            set;
        }

        [DataMember]
        public bool CertReq
        {
            get;
            set;
        }

        [DataMember]
        public string SourceFile
        {
            get;
            set;
        }

        [DataMember]
        public string ServerAddress
        {
            get;
            set;
        }

        [DataMember]
        public string UserCert
        {
            get;
            set;
        }

        [DataMember]
        public string UserCertPassword
        {
            get;
            set;
        }

        [DataMember]
        public string UserName
        {
            get;
            set;
        }

        [DataMember]
        public string UserPassword
        {
            get;
            set;
        }

        [DataMember]
        public SharedUtils.ResultFormat Format
        {
            get;
            set;
        }

        [DataMember]
        public string OutFile
        {
            get;
            set;
        }
    }
}
