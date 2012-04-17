using System;
using System.Drawing;
using System.IO;
using System.Xml.Linq;

namespace Vixen.Sys {
	static class Helper {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns>True if the directory exists after this call.</returns>
		static public bool EnsureDirectory(string path) {
			if(!Directory.Exists(path)) {
				try {
					Directory.CreateDirectory(path);
					return true;
				} catch {
					return false;
				}
			}
			return true;
		}

		static public T Load<T>(string filePath, IFileLoader<T> loader)
			where T : class {
			return File.Exists(filePath) ? loader.Load(filePath) : null;
		}

		//static public XElement LoadXml(string filePath) {
		//    if(File.Exists(filePath)) {
		//        using(FileStream fileStream = new FileStream(filePath, FileMode.Open)) {
		//            using(StreamReader reader = new StreamReader(fileStream)) {
		//                return XElement.Load(reader);
		//            }
		//        }
		//    }
		//    return null;
		//}

		//In case we ever change how times are stored again, we only have to change it in one place.
		static public TimeSpan? GetXmlTimeValue(XElement element, string attributeName) {
			return XmlHelper.GetTimeSpanAttribute(element, attributeName);
		}

		static public TimeSpan GetEffectRelativeTime(TimeSpan currentTime, EffectNode effectNode) {
			return currentTime - effectNode.StartTime;
		}

		static public TimeSpan GetIntentRelativeTime(TimeSpan effectRelativeTime, IntentNode intentNode) {
			return effectRelativeTime - intentNode.StartTime;
		}

		static public TimeSpan GetPreFilterRelativeTime(TimeSpan sequenceRelativeTime, PreFilterNode preFilterNode) {
			return sequenceRelativeTime - preFilterNode.StartTime;
		}

		static public TimeSpan GetEffectRelativeTime(TimeSpan intentRelativeTime, IntentNode intentNode) {
			return intentNode.StartTime + intentRelativeTime;
		}

		static public TimeSpan TranslateIntentRelativeTime(TimeSpan intent1RelativeTime, IntentNode intent1, IntentNode intent2) {
			TimeSpan effectRelativeTime = GetEffectRelativeTime(intent1RelativeTime, intent1);
			TimeSpan otherIntentRelativeTime = GetIntentRelativeTime(effectRelativeTime, intent2);
			return otherIntentRelativeTime;
		}

		static public Color ConvertToGrayscale(float value) {
			// Convert to grayscale, capping at 255.
			byte b = ConvertToByte(value);
			return ConvertToGrayscale(b);
		}

		static public Color ConvertToGrayscale(long value) {
			// Convert to grayscale, capping at 255.
			byte b = ConvertToByte(value);
			return ConvertToGrayscale(b);
		}

		static public Color ConvertToGrayscale(int value) {
			// Convert to grayscale, capping at 255.
			byte b = ConvertToByte(value);
			return ConvertToGrayscale(b);
		}

		static public Color ConvertToGrayscale(byte value) {
			return Color.FromArgb(value, value, value);
		}

		static public Color ConvertToGrayscale(double value) {
			// 0-1 representing 0-100%
			byte b = (byte)(value * byte.MaxValue);
			return ConvertToGrayscale(b);
		}

		static public byte ConvertToByte(float value) {
			int i = (int)value;
			i = Math.Max(byte.MinValue, i);
			i = Math.Min(byte.MaxValue, i);
			return (byte)i;
		}

		static public byte ConvertToByte(int value) {
			value = Math.Max(byte.MinValue, value);
			value = Math.Min(byte.MaxValue, value);
			return (byte)value;
		}

		static public byte ConvertToByte(double value) {
			// 0-1 representing 0-100%
			value = value * byte.MaxValue;
			return (byte)value;
		}

		static public byte ConvertToByte(long value) {
			value = Math.Max(byte.MinValue, value);
			value = Math.Min(byte.MaxValue, value);
			return (byte)value;
		}

		static public TimeSpan GetIntentNodeTimeSpan(IPreFilterNode intentNode) {
			return GetNodeTimeSpan(intentNode);
		}

		static public TimeSpan GetEffectNodeTimeSpan(IEffectNode intentNode) {
			return GetNodeTimeSpan(intentNode);
		}

		static public TimeSpan GetPreFilterNodeTimeSpan(IIntentNode intentNode) {
			return GetNodeTimeSpan(intentNode);
		}

		static public TimeSpan GetNodeTimeSpan(IDataNode node) {
			return (node != null) ? node.TimeSpan : TimeSpan.Zero;
		}
	}
}
