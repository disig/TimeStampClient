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
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Disig.TimeStampClient.Gui
{
    public static class XmlSerializer
    {
        public static T Deserialize<T>(Stream inputStream)
        {
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings()
            {
                CloseInput = false,
                ConformanceLevel = ConformanceLevel.Document,
                DtdProcessing = DtdProcessing.Prohibit,
                ValidationType = ValidationType.None
            };

            using (XmlReader xmlReader = XmlReader.Create(inputStream, xmlReaderSettings))
            {
                DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T));
                return (T)dataContractSerializer.ReadObject(xmlReader);
            }
        }

        public static void Serialize(object o, Stream outputStream)
        {
            if (o == null)
            {
                return;
            }
            XmlWriterSettings xmlWriterSettings = new System.Xml.XmlWriterSettings()
            {
                CloseOutput = false,
                Encoding = new UTF8Encoding(false, true),
                OmitXmlDeclaration = false,
                Indent = true
            };

            using (XmlWriter xmlWriter = XmlWriter.Create(outputStream, xmlWriterSettings))
            {
                DataContractSerializer dataContractSerializer = new DataContractSerializer(o.GetType());
                dataContractSerializer.WriteObject(xmlWriter, o);
            }
        }
    }
}
