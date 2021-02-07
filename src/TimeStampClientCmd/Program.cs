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
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Disig.TimeStampClient.Cmd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                string fileName = null;
                string tsa = null;
                string hash = null;
                string policy = null;
                string nonce = null;
                bool cert = false;
                string outFile = null;
                string sslClientCertFile = null;
                string sslClientCertPass = null;
                string httpAuthLogin = null;
                string httpAuthPass = null;
                bool isAsics = false;

                if (args == null)
                {
                    return;
                }

                int i = 0;
                if (0 >= args.Length)
                {
                    ExitWithHelp(string.Empty);
                }

                while (i < args.Length)
                {
                    switch (args[i])
                    {
                        case "--file":
                            fileName = args[++i];
                            break;
                        case "--tsa":
                            tsa = args[++i];
                            break;
                        case "--out":
                            outFile = args[++i];
                            break;
                        case "--hash":
                            hash = args[++i];
                            break;
                        case "--policy":
                            policy = args[++i];
                            break;
                        case "--nonce":
                            nonce = args[++i];
                            break;
                        case "--cert-req":
                            cert = true;
                            break;
                        case "--ssl-client-cert-file":
                            sslClientCertFile = args[++i];
                            break;
                        case "--ssl-client-cert-pass":
                            sslClientCertPass = args[++i];
                            break;
                        case "--http-auth-login":
                            httpAuthLogin = args[++i];
                            break;
                        case "--http-auth-pass":
                            httpAuthPass = args[++i];
                            break;
                        case "--asics":
                            isAsics = true;
                            break;
                        default:
                            ExitWithHelp("Invalid argument: " + args[i]);
                            break;
                    }

                    i++;
                }

                X509Certificate2 sslCert = null;
                if (!string.IsNullOrEmpty(sslClientCertFile))
                {
                    sslCert = new X509Certificate2(sslClientCertFile, sslClientCertPass);
                }

                NetworkCredential networkCredential = null;
                if (!string.IsNullOrEmpty(httpAuthLogin) && !string.IsNullOrEmpty(httpAuthPass))
                {
                    networkCredential = new NetworkCredential(httpAuthLogin, httpAuthPass);
                }

                UserCredentials credentials = null;
                if (networkCredential != null || sslCert != null)
                {
                    credentials = new UserCredentials(sslCert, networkCredential);
                }

                TimeStampToken token = SharedUtils.RequestTimeStamp(tsa, fileName, hash, policy, nonce, cert, credentials, new LogDelegate(LogMessage), true);
                if (isAsics)
                {
                    SharedUtils.SaveToAsicSimple(fileName, token, outFile);
                }
                else
                {
                    SharedUtils.SaveResponse(outFile, token);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ExitWithHelp(null);
            }

            Console.WriteLine("Success");
        }

        public static void LogMessage(string msg)
        {
            Console.WriteLine(msg);
        }

        public static void ExitWithHelp(string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Name + " " + Assembly.GetExecutingAssembly().GetName().Version);
                Console.WriteLine(@"Copyright (c) 2016-2021 Disig a.s. <http://www.disig.sk>");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine(@"Argument error: " + error);
                Console.WriteLine();
            }

            Console.WriteLine(@"Example usage:");
            Console.WriteLine();
            Console.WriteLine(@"  Request time stamp:");
            Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Name + SharedUtils.AppVersion);
            Console.WriteLine(@"      --file ""file_to_timestamp""");
            Console.WriteLine(@"      --tsa ""tsa_service_address""");
            Console.WriteLine(@"      --out ""file_to_save_timestamp""");
            Console.WriteLine(@"      [--hash ""sha1 | sha256 | sha512 | md5""]");
            Console.WriteLine(@"      [--policy ""policy_oid""]");
            Console.WriteLine(@"      [--cert-req]");
            Console.WriteLine(@"      [--nonce ""1234567890ABCDEF""]");
            Console.WriteLine(@"      [--ssl-client-cert-file ""path_to_client_pkcs12_certificate""]");
            Console.WriteLine(@"      [--ssl-client-cert-pass ""certificate_password""]");
            Console.WriteLine(@"      [--http-auth-login ""http_authentication_login""]");
            Console.WriteLine(@"      [--http-auth-pass ""http_authentication_password""]");

            Environment.Exit(1);
        }
    }
}
