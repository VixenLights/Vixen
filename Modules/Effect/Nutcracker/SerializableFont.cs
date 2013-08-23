using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace VixenModules.Effect.Nutcracker
{
   
    [DataContract]
    public class SerializableFont
    {
        public SerializableFont()
        {
            FontValue = null;
        }

        public SerializableFont(Font font)
        {
            FontValue = font;
        }

        [DataMember]
        public Font FontValue { get { return fontValue; } set { fontValue = value; } }
        private Font fontValue;

        //[XmlElement("FontValue")]
        //[DataMember(Name="FontValue")]
        //public string SerializeFontAttribute
        //{
        //    get { return FontXmlConverter.ConvertToString(FontValue); }
        //    set { FontValue = FontXmlConverter.ConvertToFont(value); }
        //}

        public static implicit operator Font(SerializableFont serializeableFont)
        {
            if (serializeableFont == null)
                return null;
            return serializeableFont.FontValue;
        }

        public static implicit operator SerializableFont(Font font)
        {
            return new SerializableFont(font);
        }
    }

    public static class FontXmlConverter
    {
        public static string ConvertToString(Font font)
        {
            try
            {
                if (font != null)
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
                    var fonts =  converter.ConvertToString(font);
                    return fonts;
                }
                else
                    return null;
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Unable to convert");
            }
            return null;
        }

        public static Font ConvertToFont(string fontString)
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
                return (Font)converter.ConvertFromString(fontString);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Unable to convert");
            }
            return null;
        }
    }
}