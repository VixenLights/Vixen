using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NLog;

namespace VixenModules.Effect.CountDown
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

		[XmlIgnore]
		public Font FontValue { get; set; }

		[XmlElement("FontValue")]
		[DataMember]
		public string SerializeFontAttribute
		{
			get
			{
				return FontXmlConverter.ConvertToString(FontValue);
			}
			set
			{
				FontValue = FontXmlConverter.ConvertToFont(value);
			}
		}

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
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		public static string ConvertToString(Font font)
		{
			try
			{
				if (font != null)
				{
					TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
					return converter.ConvertToString(font);
				}
				
				return null;
			}
			catch { Logging.Error("Unable to convert font"); }
			return null;
		}
		public static Font ConvertToFont(string fontString)
		{
			try
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
				return (Font)converter.ConvertFromString(fontString);
			}
			catch { Logging.Error("Unable to convert font"); }
			return null;
		}
	}
}
