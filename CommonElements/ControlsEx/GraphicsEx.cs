using System;
using System.Drawing;

namespace CommonElements.ControlsEx
{
	/// <summary>
	/// Zusammenfassung für GraphicsEx.
	/// </summary>
	public static class GraphicsEx
	{
		#region variables
		private const ContentAlignment
			//horizontal
			anyleft=ContentAlignment.BottomLeft|ContentAlignment.MiddleLeft|ContentAlignment.TopLeft,
			anycenter=ContentAlignment.BottomCenter|ContentAlignment.MiddleCenter|ContentAlignment.TopCenter,
			anyright=ContentAlignment.BottomRight|ContentAlignment.MiddleRight|ContentAlignment.TopRight,
			//vertical
			anytop=ContentAlignment.TopCenter|ContentAlignment.TopLeft|ContentAlignment.TopRight,
			anymiddle=ContentAlignment.MiddleCenter|ContentAlignment.MiddleLeft|ContentAlignment.MiddleRight,
			anybottom=ContentAlignment.BottomCenter|ContentAlignment.BottomLeft|ContentAlignment.BottomRight;
		#endregion
		public static RectangleF SortedRectangle(RectangleF rct)
		{
			return RectangleF.FromLTRB(
				Math.Min(rct.X, rct.Right), Math.Min(rct.Y, rct.Bottom),
				Math.Max(rct.X, rct.Right), Math.Max(rct.Y, rct.Bottom));
		}
		public static Rectangle SortedRectangle(Rectangle rct)
		{
			return Rectangle.FromLTRB(
				Math.Min(rct.X, rct.Right), Math.Min(rct.Y, rct.Bottom),
				Math.Max(rct.X, rct.Right), Math.Max(rct.Y, rct.Bottom));
		}
		/// <summary>
		/// converts two given alignment values to one content alignment
		/// </summary>
		public static ContentAlignment GetAlignmentFromStringAlignment(StringAlignment alignment,
			StringAlignment linealignment)
		{
			ContentAlignment ret;
			//horizontal
			switch(alignment)
			{
				case StringAlignment.Near:
					ret=anyleft; break;
				case StringAlignment.Center:
					ret=anycenter; break;
				default:
					ret=anyright; break;
			}
			//vertical
			switch(linealignment)
			{
				case StringAlignment.Near:
					ret&=anytop; break;
				case StringAlignment.Center:
					ret&=anymiddle; break;
				default:
					ret&=anybottom; break;
			}
			return ret;
		}
		/// <summary>
		/// converts a given content alignment value into to string alignments
		/// </summary>
		public static void GetStringAlignmentFromAlignment(ContentAlignment value,
			out StringAlignment alignment, out StringAlignment linealignment)
		{
			//horizontal
			if((value&anyleft)!=0)
				alignment=StringAlignment.Near;
			else if((value&anycenter)!=0)
				alignment=StringAlignment.Center;
			else
				alignment=StringAlignment.Far;
			//vertical
			if((value&anytop)!=0)
				linealignment=StringAlignment.Near;
			else if((value&anymiddle)!=0)
				linealignment=StringAlignment.Center;
			else
				linealignment=StringAlignment.Far;
		}
	}
}
