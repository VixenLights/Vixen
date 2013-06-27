/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Drawing;


namespace Dataweb.NShape.Advanced
{
	/// <summary>
	/// Measures the extent of strings.
	/// </summary>
	public class TextMeasurer
	{
		/// <summary>
		/// Measures the given text and returns its size.
		/// </summary>
		/// <param name="text">The text to measure</param>
		/// <param name="characterStyle">The text's character style.</param>
		/// <param name="proposedSize">The layout area of the text. Size.Empty means no fitting in area.</param>
		/// <param name="paragraphStyle">The paragraph layout of the text</param>
		/// <returns></returns>
		public static Size MeasureText(string text, ICharacterStyle characterStyle, Size proposedSize,
		                               IParagraphStyle paragraphStyle)
		{
			using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
				return MeasureText(graphics, text, ToolCache.GetFont(characterStyle), proposedSize,
				                   ToolCache.GetStringFormat(paragraphStyle));
		}


		/// <summary>
		/// Measures the given text and returns its size.
		/// </summary>
		/// <param name="text">The text to measure</param>
		/// <param name="font">The text's font</param>
		/// <param name="proposedSize">The layout area of the text. Size.Empty means no fitting in area.</param>
		/// <param name="paragraphStyle">The paragraph layout of the text</param>
		/// <returns></returns>
		public static Size MeasureText(string text, Font font, Size proposedSize, IParagraphStyle paragraphStyle)
		{
			using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
				return MeasureText(graphics, text, font, proposedSize, paragraphStyle);
		}


		/// <summary>
		/// Measures the given text and returns its size.
		/// </summary>
		/// <param name="text">The text to measure</param>
		/// <param name="font">The text's font</param>
		/// <param name="proposedSize">The layout area of the text. Size.Empty means no fitting in area.</param>
		/// <param name="format">StringFormat object defining the layout of the text</param>
		/// <returns></returns>
		public static Size MeasureText(string text, Font font, Size proposedSize, StringFormat format)
		{
			using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
				return MeasureText(graphics, text, font, proposedSize, format);
		}


		/// <summary>
		/// Measures the given text and returns its size.
		/// </summary>
		/// <param name="graphics">Graphics context used for the measuring</param>
		/// <param name="text">The text to measure</param>
		/// <param name="characterStyle">The text's character style</param>
		/// <param name="proposedSize">The layout area of the text. Size.Empty means no fitting in area.</param>
		/// <param name="paragraphStyle">The paragraph layout of the text</param>
		/// <returns></returns>
		public static Size MeasureText(Graphics graphics, string text, ICharacterStyle characterStyle, Size proposedSize,
		                               IParagraphStyle paragraphStyle)
		{
			if (paragraphStyle == null) throw new ArgumentNullException("paragraphStyle");
			return MeasureText(graphics, text, ToolCache.GetFont(characterStyle), proposedSize,
			                   ToolCache.GetStringFormat(paragraphStyle));
		}


		/// <summary>
		/// Measures the given text and returns its size.
		/// </summary>
		/// <param name="graphics">Graphics context used for the measuring</param>
		/// <param name="text">The text to measure</param>
		/// <param name="font">The text's font</param>
		/// <param name="proposedSize">The layout area of the text. Size.Empty means no fitting in area.</param>
		/// <param name="paragraphStyle">The paragraph layout of the text</param>
		/// <returns></returns>
		public static Size MeasureText(Graphics graphics, string text, Font font, Size proposedSize,
		                               IParagraphStyle paragraphStyle)
		{
			if (paragraphStyle == null) throw new ArgumentNullException("paragraphStyle");
			return MeasureText(graphics, text, font, proposedSize, ToolCache.GetStringFormat(paragraphStyle));
		}


		/// <summary>
		/// Measures the given text and returns its size.
		/// </summary>
		/// <param name="graphics">Graphics context used for the measuring</param>
		/// <param name="text">The text to measure</param>
		/// <param name="font">The text's font</param>
		/// <param name="proposedSize">The layout area of the text. Size.Empty means no fitting in area.</param>
		/// <param name="format">StringFormat object defining the layout of the text</param>
		/// <returns></returns>
		public static Size MeasureText(Graphics graphics, string text, Font font, Size proposedSize, StringFormat format)
		{
			if (graphics == null) throw new ArgumentNullException("graphics");
			if (font == null) throw new ArgumentNullException("font");
			if (text == null) throw new ArgumentNullException("text");
			if (format == null) throw new ArgumentNullException("format");
			// Add Flag "Measure Trailing Spaces" if not set
			StringFormatFlags flags = format.FormatFlags;
			bool flagRemoved = RemoveFlag(ref flags, StringFormatFlags.MeasureTrailingSpaces);
			if (flagRemoved) format.FormatFlags = flags;

			Size result = Size.Empty;
			// Workaround: Last Line break will be always be ignored when measuring the string, so add another line break
			string measuredText = text.EndsWith("\n") ? text + "\n" : text;
			if (proposedSize == Size.Empty)
				result = Size.Ceiling(graphics.MeasureString(measuredText, font, PointF.Empty, format));
			else result = Size.Ceiling(graphics.MeasureString(measuredText, font, proposedSize, format));

			// Restore StringFormatFlags
			if (flagRemoved) {
				AddFlag(ref flags, StringFormatFlags.MeasureTrailingSpaces);
				format.FormatFlags = flags;
			}
			return result;
		}


		private static bool AddFlag(ref StringFormatFlags formatFlags, StringFormatFlags flag)
		{
			if ((formatFlags & flag) == flag) return false;
			formatFlags |= flag;
			return true;
		}


		private static bool RemoveFlag(ref StringFormatFlags formatFlags, StringFormatFlags flag)
		{
			if ((formatFlags & flag) != flag) return false;
			formatFlags ^= flag;
			return true;
		}
	}
}