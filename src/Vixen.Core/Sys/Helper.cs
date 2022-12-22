﻿using System.Xml.Linq;
using Vixen.Data.Flow;

namespace Vixen.Sys
{
	internal static class Helper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns>True if the directory exists after this call.</returns>
		public static bool EnsureDirectory(string path)
		{
			if (!Directory.Exists(path)) {
				try {
					Directory.CreateDirectory(path);
					return true;
				}
				catch {
					return false;
				}
			}
			return true;
		}

		//In case we ever change how times are stored again, we only have to change it in one place.
		public static TimeSpan? GetXmlTimeValue(XElement element, string attributeName)
		{
			return XmlHelper.GetTimeSpanAttribute(element, attributeName);
		}

		public static TimeSpan GetEffectRelativeTime(TimeSpan currentTime, IEffectNode effectNode)
		{
			return currentTime - effectNode.StartTime;
		}

		public static TimeSpan GetIntentRelativeTime(TimeSpan effectRelativeTime, IIntentNode intentNode)
		{
			return effectRelativeTime - intentNode.StartTime;
		}

		public static TimeSpan GetSequenceFilterRelativeTime(TimeSpan sequenceRelativeTime,
		                                                     SequenceFilterNode sequenceFilterNode)
		{
			return sequenceRelativeTime - sequenceFilterNode.StartTime;
		}

		public static TimeSpan GetEffectRelativeTime(TimeSpan intentRelativeTime, IIntentNode intentNode)
		{
			return intentNode.StartTime + intentRelativeTime;
		}

		public static TimeSpan TranslateIntentRelativeTime(TimeSpan intent1RelativeTime, IntentNode intent1,
		                                                   IntentNode intent2)
		{
			TimeSpan effectRelativeTime = GetEffectRelativeTime(intent1RelativeTime, intent1);
			TimeSpan otherIntentRelativeTime = GetIntentRelativeTime(effectRelativeTime, intent2);
			return otherIntentRelativeTime;
		}

		/// <summary>
		/// Converts to grayscale, capping at 255.
		/// </summary>
		public static Color ConvertToGrayscale(float value)
		{
			byte b = ConvertToByte(value);
			return ConvertToGrayscale(b);
		}

		/// <summary>
		/// Converts to grayscale, capping at 255.
		/// </summary>
		public static Color ConvertToGrayscale(long value)
		{
			byte b = ConvertToByte(value);
			return ConvertToGrayscale(b);
		}

		/// <summary>
		/// Converts to grayscale, capping at 255.
		/// </summary>
		public static Color ConvertToGrayscale(int value)
		{
			byte b = ConvertToByte(value);
			return ConvertToGrayscale(b);
		}

		public static Color ConvertToGrayscale(byte value)
		{
			return Color.FromArgb(value, value, value);
		}

		/// <summary>
		/// 0-1 representing 0-100%
		/// </summary>
		public static Color ConvertToGrayscale(double value)
		{
			byte b = (byte) (value*byte.MaxValue);
			return ConvertToGrayscale(b);
		}

		public static byte ConvertToByte(float value)
		{
			int i = (int) value;
			i = Math.Max(byte.MinValue, i);
			i = Math.Min(byte.MaxValue, i);
			return (byte) i;
		}

		public static byte ConvertToByte(int value)
		{
			value = Math.Max(byte.MinValue, value);
			value = Math.Min(byte.MaxValue, value);
			return (byte) value;
		}

		/// <summary>
		/// 0-1 representing 0-100%
		/// </summary>
		public static byte ConvertToByte(double value)
		{
			value = value*byte.MaxValue;
			return (byte) value;
		}

		public static byte ConvertToByte(long value)
		{
			value = Math.Max(byte.MinValue, value);
			value = Math.Min(byte.MaxValue, value);
			return (byte) value;
		}

		public static IDataFlowData GetOutputState(IDataFlowComponent dataFlowComponent, int outputIndex)
		{
			return (outputIndex < dataFlowComponent.Outputs.Length) ? dataFlowComponent.Outputs[outputIndex].Data : null;
		}
	}
}