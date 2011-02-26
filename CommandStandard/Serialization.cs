using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Xml;
using System.Xml.Serialization;

namespace CommandStandard {
    static class Serialization {
        static private UTF8Encoding m_utfEncoding = new UTF8Encoding();

        static private string UTF8ByteArrayToString(byte[] characters) {
            return m_utfEncoding.GetString(characters);
        }

        static private byte[] StringToUTF8ByteArray(string xmlString) {
            return m_utfEncoding.GetBytes(xmlString);
        }

        static public string SerializeObject(object obj) {
            string xmlString = null;
            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.Default);

            serializer.Serialize(xmlTextWriter, obj);
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
            xmlString = UTF8ByteArrayToString(memoryStream.ToArray());

            return xmlString;
        }

        static public object DeserializeObject(string xmlString, Type type) {
            XmlSerializer serializer = new XmlSerializer(type);
            MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(xmlString));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.Default);

            return serializer.Deserialize(memoryStream);
        }
    }
}
