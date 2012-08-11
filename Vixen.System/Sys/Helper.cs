using System;
using System.Drawing;
using System.IO;
using System.Xml.Linq;
using Vixen.Data.Flow;

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

		//In case we ever change how times are stored again, we only have to change it in one place.
		static public TimeSpan? GetXmlTimeValue(XElement element, string attributeName) {
			return XmlHelper.GetTimeSpanAttribute(element, attributeName);
		}

		static public TimeSpan GetEffectRelativeTime(TimeSpan currentTime, IEffectNode effectNode) {
			return currentTime - effectNode.StartTime;
		}

		static public TimeSpan GetIntentRelativeTime(TimeSpan effectRelativeTime, IIntentNode intentNode) {
			return effectRelativeTime - intentNode.StartTime;
		}

		static public TimeSpan GetSequenceFilterRelativeTime(TimeSpan sequenceRelativeTime, SequenceFilterNode sequenceFilterNode) {
			return sequenceRelativeTime - sequenceFilterNode.StartTime;
		}

		static public TimeSpan GetEffectRelativeTime(TimeSpan intentRelativeTime, IIntentNode intentNode) {
			return intentNode.StartTime + intentRelativeTime;
		}

		static public TimeSpan TranslateIntentRelativeTime(TimeSpan intent1RelativeTime, IntentNode intent1, IntentNode intent2) {
			TimeSpan effectRelativeTime = GetEffectRelativeTime(intent1RelativeTime, intent1);
			TimeSpan otherIntentRelativeTime = GetIntentRelativeTime(effectRelativeTime, intent2);
			return otherIntentRelativeTime;
		}

		/// <summary>
		/// Converts to grayscale, capping at 255.
		/// </summary>
		static public Color ConvertToGrayscale(float value) {
			byte b = ConvertToByte(value);
			return ConvertToGrayscale(b);
		}

		/// <summary>
		/// Converts to grayscale, capping at 255.
		/// </summary>
		static public Color ConvertToGrayscale(long value) {
			byte b = ConvertToByte(value);
			return ConvertToGrayscale(b);
		}

		/// <summary>
		/// Converts to grayscale, capping at 255.
		/// </summary>
		static public Color ConvertToGrayscale(int value) {
			byte b = ConvertToByte(value);
			return ConvertToGrayscale(b);
		}

		static public Color ConvertToGrayscale(byte value) {
			return Color.FromArgb(value, value, value);
		}

		/// <summary>
		/// 0-1 representing 0-100%
		/// </summary>
		static public Color ConvertToGrayscale(double value) {
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

		/// <summary>
		/// 0-1 representing 0-100%
		/// </summary>
		static public byte ConvertToByte(double value) {
			value = value * byte.MaxValue;
			return (byte)value;
		}

		static public byte ConvertToByte(long value) {
			value = Math.Max(byte.MinValue, value);
			value = Math.Min(byte.MaxValue, value);
			return (byte)value;
		}

		static public IDataFlowData GetOutputState(IDataFlowComponent dataFlowComponent, int outputIndex) {
			return (outputIndex < dataFlowComponent.Outputs.Length) ? dataFlowComponent.Outputs[outputIndex].Data : null;
		}
	}
}
