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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


namespace Dataweb.NShape.Advanced
{
	/// <summary>
	/// Manages drawing tools for GDI+.
	/// </summary>
	public static class ToolCache
	{
		#region [Public] Methods

		/// <summary>
		/// Releases all resources
		/// </summary>
		public static void Clear()
		{
			// dispose and clear pens
			foreach (KeyValuePair<PenKey, Pen> item in penCache)
				item.Value.Dispose();
			penCache.Clear();

			// dispose and clear solid brushes
			foreach (KeyValuePair<IColorStyle, SolidBrush> item in solidBrushCache)
				item.Value.Dispose();
			solidBrushCache.Clear();

			// dispose and clear other brushes
			foreach (KeyValuePair<BrushKey, Brush> item in brushCache)
				item.Value.Dispose();
			brushCache.Clear();

			// dispose and clear image attributes
			foreach (KeyValuePair<BrushKey, ImageAttributes> item in imageAttribsCache)
				item.Value.Dispose();
			imageAttribsCache.Clear();

			// dispose and clear fonts
			foreach (KeyValuePair<ICharacterStyle, Font> item in fontCache)
				item.Value.Dispose();
			fontCache.Clear();

			// dispose and clear string formatters
			foreach (KeyValuePair<IParagraphStyle, StringFormat> item in stringFormatCache)
				item.Value.Dispose();
			stringFormatCache.Clear();

			// dispose and clear cap paths
			foreach (KeyValuePair<CapKey, GraphicsPath> item in capPathCache)
				item.Value.Dispose();
			capPathCache.Clear();

			// dispose and clear caps
			foreach (KeyValuePair<CapKey, CustomLineCap> item in capCache)
				item.Value.Dispose();
			capCache.Clear();
		}


		/// <summary>
		/// Releases resources used for styles of the given <see cref="T:Dataweb.NShape.IStyleSet" />.
		/// </summary>
		public static void RemoveStyleSetTools(IStyleSet styleSet)
		{
			if (styleSet == null) throw new ArgumentNullException("styleSet");

			// delete GDI+ objects created from styles
			foreach (ICapStyle style in styleSet.CapStyles)
				ToolCache.NotifyCapStyleChanged(style);
			foreach (ICharacterStyle style in styleSet.CharacterStyles)
				ToolCache.NotifyCharacterStyleChanged(style);
			foreach (IColorStyle style in styleSet.ColorStyles)
				ToolCache.NotifyColorStyleChanged(style);
			foreach (IFillStyle style in styleSet.FillStyles)
				ToolCache.NotifyFillStyleChanged(style);
			foreach (ILineStyle style in styleSet.LineStyles)
				ToolCache.NotifyLineStyleChanged(style);
			foreach (IParagraphStyle style in styleSet.ParagraphStyles)
				ToolCache.NotifyParagraphStyleChanged(style);
		}


		/// <summary>
		/// Deletes all cached instances of the given <see cref="T:Dataweb.NShape.IStyle" />.
		/// </summary>
		public static void NotifyStyleChanged(IStyle style)
		{
			if (style == null) throw new ArgumentNullException("style");
			if (style is ICapStyle) NotifyCapStyleChanged((ICapStyle) style);
			else if (style is ICharacterStyle) NotifyCharacterStyleChanged((ICharacterStyle) style);
			else if (style is IColorStyle) NotifyColorStyleChanged((IColorStyle) style);
			else if (style is IFillStyle) NotifyFillStyleChanged((IFillStyle) style);
			else if (style is ILineStyle) NotifyLineStyleChanged((ILineStyle) style);
			else if (style is IParagraphStyle) NotifyParagraphStyleChanged((IParagraphStyle) style);
			else throw new ArgumentException("style");
		}


		/// <summary>
		/// Finds and returns the <see cref="T:System.Drawing.Brush" /> for the given <see cref="T:Dataweb.NShape.IFillStyle" />. 
		/// The <see cref="T:System.Drawing.Brush" /> will be translated, scaled and rotated.
		/// </summary>
		/// <param name="fillStyle">Specifies the <see cref="T:Dataweb.NShape.IFillStyle" /> the brush belongs to.</param>
		/// <param name="unrotatedBounds">Specifies the axis aligned bounding rectangle of the unrotated shape.</param>
		/// <param name="center">Specifies the rotation center.</param>
		/// <param name="angle">Specifies the rotation angle in tenths of degrees.</param>
		public static Brush GetTransformedBrush(IFillStyle fillStyle, Rectangle unrotatedBounds, Point center, int angle)
		{
			if (fillStyle == null) throw new ArgumentNullException("fillStyle");

			Brush brush = GetBrush(fillStyle);
			float angleDeg = Geometry.TenthsOfDegreeToDegrees(angle);
			if (brush is LinearGradientBrush)
				GdiHelpers.TransformLinearGradientBrush((LinearGradientBrush) brush, fillStyle.GradientAngle, unrotatedBounds,
				                                        center, angleDeg);
			else if (brush is PathGradientBrush)
				GdiHelpers.TransformPathGradientBrush((PathGradientBrush) brush, unrotatedBounds, center, angleDeg);
			else if (brush is TextureBrush)
				GdiHelpers.TransformTextureBrush((TextureBrush) brush, fillStyle.ImageLayout, unrotatedBounds, center, angleDeg);
			return brush;
		}


		///// <summary>
		///// Finds and returns the <see cref="T:System.Drawing.Brush" /> for the given <see cref="T:Dataweb.NShape.IFillStyle" />. 
		///// The <see cref="T:System.Drawing.Brush" /> will be translated, scaled and rotated.
		///// </summary>
		///// <param name="fillStyle">Specifies the <see cref="T:Dataweb.NShape.IFillStyle" /> the brush belongs to.</param>
		///// <param name="unrotatedBounds">Specifies the x axis aligned bounding rectangle of the unrotated shape.</param>
		///// <param name="rotatedBounds">Specifies the x axis aligned bounding rectangle of the rotated shape.</param>
		///// <param name="angle">Specifies the rotation angle in tenths of degrees.</param>
		//public static Brush GetTransformedBrush(IFillStyle fillStyle, PointF center, Rectangle rotatedBounds, int angle) {
		//    if (fillStyle == null) throw new ArgumentNullException("fillStyle");

		//    Brush brush = GetBrush(fillStyle);
		//    float angleDeg = Geometry.TenthsOfDegreeToDegrees(angle);
		//    if (brush is LinearGradientBrush)
		//        GdiHelpers.TransformLinearGradientBrush((LinearGradientBrush)brush, fillStyle.GradientAngle, center, rotatedBounds, angleDeg);
		//    else if (brush is PathGradientBrush)
		//        GdiHelpers.TransformPathGradientBrush((PathGradientBrush)brush, rotatedBounds, Point.Round(center), angleDeg);
		//    else if (brush is TextureBrush) {
		//        GdiHelpers.TransformTextureBrush2((TextureBrush)brush, 45, center, rotatedBounds, angleDeg);
		//    }
		//    return brush;
		//}


		/// <summary>
		/// Returns the untransformed axis aligned bounding rectangle of the line cap defined by the given styles.
		/// </summary>
		public static Rectangle GetCapBounds(ICapStyle capStyle, ILineStyle lineStyle, float angleDeg)
		{
			if (capStyle == null) throw new ArgumentNullException("capStyle");
			if (lineStyle == null) throw new ArgumentNullException("lineStyle");
			Rectangle result = Geometry.InvalidRectangle;
			PointF[] buffer = null;
			GetCapPoints(capStyle, lineStyle, ref buffer);

			// Transform cap points
			matrix.Reset();
			matrix.RotateAt(angleDeg + 90, Point.Empty);
			// Scale GraphicsPath up for correcting the automatic scaling that is applied to
			// LineCaps by GDI+ when altering the LineWidth of the pen
			matrix.Scale(lineStyle.LineWidth, lineStyle.LineWidth);
			matrix.TransformPoints(buffer);

			Geometry.CalcBoundingRectangle(buffer, out result);
			ShapeUtils.InflateBoundingRectangle(ref result, lineStyle);

			return result;
		}


		/// <summary>
		/// Constructs a polygon from the given cap style used for drawing the line cap's interior.
		/// </summary>
		public static void GetCapPoints(ICapStyle capStyle, ILineStyle lineStyle, ref PointF[] capPoints)
		{
			if (capStyle == null) throw new ArgumentNullException("capStyle");
			if (lineStyle == null) throw new ArgumentNullException("lineStyle");
			// Copy path points to point buffer
			GraphicsPath capPath = GetCapPath(capStyle, lineStyle);
			if (capPoints == null)
				capPoints = new PointF[capPath.PointCount];
			else if (capPoints.Length != capPath.PointCount)
				Array.Resize(ref capPoints, capPath.PointCount);
			Array.Copy(capPath.PathPoints, capPoints, capPoints.Length);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static Pen GetPen(ILineStyle lineStyle, ICapStyle startCapStyle, ICapStyle endCapStyle)
		{
			if (lineStyle == null) throw new ArgumentNullException("lineStyle");

			PenKey penKey;
			penKey.LineStyle = lineStyle;
			penKey.StartCapStyle = startCapStyle;
			penKey.EndCapStyle = endCapStyle;

			Pen pen = null;
			penCache.TryGetValue(penKey, out pen);
			if (pen == null) {
				// If the corresponding pen was not found, create a new pen based on the given LineStyle
				pen = new Pen(GetColor(lineStyle.ColorStyle, lineStyle.ColorStyle.ConvertToGray), lineStyle.LineWidth);
				// "PenAlignment.Inset" does not draw exactly along the outline of the shape and
				// causes GraphicsPath.Widen(Pen pen) to produce strance results
				//pen.Alignment = PenAlignment.Inset;
				pen.LineJoin = lineStyle.LineJoin;
				pen.DashCap = lineStyle.DashCap;
				if (lineStyle.DashType == DashType.Solid)
					pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
				else {
					pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
					pen.DashPattern = lineStyle.DashPattern;
				}
				// Create LineCaps
				SetLineCap(pen, lineStyle, startCapStyle, true);
				SetLineCap(pen, lineStyle, endCapStyle, false);

				// Add created pen to the PenCache
				penCache.Add(penKey, pen);
			}
			return pen;
		}


		/// <summary>
		/// Creates a solid color brush from the given <see cref="T:Dataweb.NShape.IColorStyle"/>.
		/// </summary>
		public static Brush GetBrush(IColorStyle colorStyle)
		{
			if (colorStyle == null) throw new ArgumentNullException("colorStyle");

			SolidBrush brush = null;
			solidBrushCache.TryGetValue(colorStyle, out brush);
			if (brush == null) {
				brush = new SolidBrush(GetColor(colorStyle, colorStyle.ConvertToGray));
				// add created brush to the BrushCache
				solidBrushCache.Add(colorStyle, brush);
			}
			return brush;
		}


		/// <summary>
		/// Creates a solid color brush from the given <see cref="T:Dataweb.NShape.IColorStyle"/> or <see cref="T:Dataweb.NShape.ILineStyle"/>.
		/// If the color style is ColorStyle.Empty, the line style's color style is used.
		/// </summary>
		public static Brush GetBrush(IColorStyle colorStyle, ILineStyle lineStyle)
		{
			if (colorStyle == null) throw new ArgumentNullException("colorStyle");
			if (lineStyle == null) throw new ArgumentNullException("lineStyle");
			if (colorStyle == ColorStyle.Empty)
				return GetBrush(lineStyle.ColorStyle);
			else return GetBrush(colorStyle);
		}


		/// <summary>
		/// Creates a brush from the given <see cref="T:Dataweb.NShape.IFillStyle"/>.
		/// Depending of the fill mode of the fill style, the result can be a <see cref="T:System.Drawing.SolidBrush"/>, 
		/// a <see cref="T:System.Drawing.Drawing2D.HatchBrush"/>, a a <see cref="T:System.Drawing.Drawing2D.LinerGradientBrush"/> or
		/// a <see cref="T:System.Drawing.TextureBrush"/>.
		/// </summary>
		public static Brush GetBrush(IFillStyle fillStyle)
		{
			if (fillStyle == null) throw new ArgumentNullException("fillStyle");
			BrushKey brushKey;
			brushKey.FillStyle = fillStyle;
			brushKey.Image = null;

			Brush brush = null;
			brushCache.TryGetValue(brushKey, out brush);
			if (brush == null) {
				switch (fillStyle.FillMode) {
					case FillMode.Solid:
						brush = new SolidBrush(GetColor(fillStyle.BaseColorStyle, fillStyle.ConvertToGrayScale));
						break;
					case FillMode.Pattern:
						brush = new HatchBrush(fillStyle.FillPattern, GetColor(fillStyle.BaseColorStyle, fillStyle.ConvertToGrayScale),
						                       GetColor(fillStyle.AdditionalColorStyle, fillStyle.ConvertToGrayScale));
						break;
					case FillMode.Gradient:
						rectBuffer.X = 0;
						rectBuffer.Y = 0;
						rectBuffer.Width = 100;
						rectBuffer.Height = 100;
						brush = new LinearGradientBrush(rectBuffer, GetColor(fillStyle.AdditionalColorStyle, fillStyle.ConvertToGrayScale),
						                                GetColor(fillStyle.BaseColorStyle, fillStyle.ConvertToGrayScale),
						                                fillStyle.GradientAngle);
						break;
					case FillMode.Image:
						if (NamedImage.IsNullOrEmpty(fillStyle.Image))
							brush = new SolidBrush(Color.Transparent);
						else {
							// First, get ImageAttributes
							ImageAttributes imgAttribs = null;
							imageAttribsCache.TryGetValue(brushKey, out imgAttribs);
							if (imgAttribs == null) {
								imgAttribs = GdiHelpers.GetImageAttributes(fillStyle.ImageLayout, fillStyle.ImageGammaCorrection,
								                                           fillStyle.ImageTransparency, fillStyle.ConvertToGrayScale);
								imageAttribsCache.Add(brushKey, imgAttribs);
							}

							// Create Brush
							rectBuffer.X = 0;
							rectBuffer.Y = 0;
							rectBuffer.Width = fillStyle.Image.Width;
							rectBuffer.Height = fillStyle.Image.Height;
							brush = new TextureBrush(fillStyle.Image.Image, rectBuffer, imgAttribs);
						}
						break;
					default:
						throw new NShapeUnsupportedValueException(fillStyle.FillMode);
				}

				// add created brush to the BrushCache
				if (brushCache.ContainsKey(brushKey))
					brushCache[brushKey] = brush;
				else brushCache.Add(brushKey, brush);
			}
			return brush;
		}


		/// <summary>
		/// Returns the font specified by the given <see cref="T:Dataweb.NShape.ICharacterStyle"/>.
		/// </summary>
		/// <param name="characterStyle"></param>
		/// <returns></returns>
		public static Font GetFont(ICharacterStyle characterStyle)
		{
			if (characterStyle == null) throw new ArgumentNullException("characterStyle");

			Font font = null;
			fontCache.TryGetValue(characterStyle, out font);
			if (font == null) {
				FontFamily fontFamily = characterStyle.FontFamily;
				FontStyle style = characterStyle.Style;
				// check if the desired FontStyle is available for this particular FontFamily
				// Set an available FontStyle if not.
				if (fontFamily != null && !fontFamily.IsStyleAvailable(style)) {
					if (fontFamily.IsStyleAvailable(FontStyle.Regular)) {
						if (fontFamily.IsStyleAvailable(style | FontStyle.Regular))
							style |= FontStyle.Regular;
						else style = FontStyle.Regular;
					}
					else if (fontFamily.IsStyleAvailable(FontStyle.Bold)) {
						if (fontFamily.IsStyleAvailable(style | FontStyle.Bold))
							style |= FontStyle.Bold;
						else style = FontStyle.Bold;
					}
					else if (fontFamily.IsStyleAvailable(FontStyle.Italic)) {
						if (fontFamily.IsStyleAvailable(style | FontStyle.Italic))
							style |= FontStyle.Italic;
						else style = FontStyle.Italic;
					}
					else if (fontFamily.IsStyleAvailable(FontStyle.Strikeout)) {
						if (fontFamily.IsStyleAvailable(style | FontStyle.Strikeout))
							style |= FontStyle.Strikeout;
						else style = FontStyle.Strikeout;
					}
					else if (fontFamily.IsStyleAvailable(FontStyle.Underline)) {
						if (fontFamily.IsStyleAvailable(style | FontStyle.Underline))
							style |= FontStyle.Underline;
						else style = FontStyle.Underline;
					}
				}
				//font = new Font(fontFamily, characterStyle.PointSize, style, GraphicsUnit.Point);
				font = new Font(fontFamily, characterStyle.Size, style, GraphicsUnit.Pixel);
				// add font to the FontCache
				fontCache.Add(characterStyle, font);
			}
			return font;
		}


		/// <summary>
		/// Returns a <see cref="T:System.Drawing.StringFormat"/> from the given <see cref="T:Dataweb.NShape.IParagraphStyle"/>.
		/// </summary>
		public static StringFormat GetStringFormat(IParagraphStyle paragraphStyle)
		{
			if (paragraphStyle == null) throw new ArgumentNullException("paragraphStyle");

			StringFormat stringFormat = null;
			stringFormatCache.TryGetValue(paragraphStyle, out stringFormat);
			if (stringFormat == null) {
				stringFormat = new StringFormat();
				switch (paragraphStyle.Alignment) {
					case ContentAlignment.BottomLeft:
						stringFormat.Alignment = StringAlignment.Near;
						stringFormat.LineAlignment = StringAlignment.Far;
						break;
					case ContentAlignment.BottomCenter:
						stringFormat.Alignment = StringAlignment.Center;
						stringFormat.LineAlignment = StringAlignment.Far;
						break;
					case ContentAlignment.BottomRight:
						stringFormat.Alignment = StringAlignment.Far;
						stringFormat.LineAlignment = StringAlignment.Far;
						break;
					case ContentAlignment.MiddleLeft:
						stringFormat.Alignment = StringAlignment.Near;
						stringFormat.LineAlignment = StringAlignment.Center;
						break;
					case ContentAlignment.MiddleCenter:
						stringFormat.Alignment = StringAlignment.Center;
						stringFormat.LineAlignment = StringAlignment.Center;
						break;
					case ContentAlignment.MiddleRight:
						stringFormat.Alignment = StringAlignment.Far;
						stringFormat.LineAlignment = StringAlignment.Center;
						break;
					case ContentAlignment.TopLeft:
						stringFormat.Alignment = StringAlignment.Near;
						stringFormat.LineAlignment = StringAlignment.Near;
						break;
					case ContentAlignment.TopCenter:
						stringFormat.Alignment = StringAlignment.Center;
						stringFormat.LineAlignment = StringAlignment.Near;
						break;
					case ContentAlignment.TopRight:
						stringFormat.Alignment = StringAlignment.Far;
						stringFormat.LineAlignment = StringAlignment.Near;
						break;
					default:
						throw new Exception(string.Format("Unexpected ContentAlignment value '{0}'.", paragraphStyle.Alignment));
				}
				// LineLimit prevents the Title from being drawn outside the layout rectangle.
				// If the layoutRectangle is too small, the text will not be rendered at all.
				//stringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.FitBlackBox | StringFormatFlags.LineLimit;
				//stringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.FitBlackBox;
				stringFormat.FormatFlags = StringFormatFlags.FitBlackBox;
				if (!paragraphStyle.WordWrap)
					stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
				stringFormat.Trimming = paragraphStyle.Trimming;

				// add font to the FontCache
				stringFormatCache.Add(paragraphStyle, stringFormat);
			}
			return stringFormat;
		}

		#endregion

		#region [Private] Methods

		/// <summary>
		/// Deletes all tools based on the given CapStyle.
		/// </summary>
		private static void NotifyCapStyleChanged(ICapStyle capStyle)
		{
			Debug.Assert(capStyle != null);

			// collect affected PenKeys
			List<PenKey> penKeys = new List<PenKey>();
			foreach (KeyValuePair<PenKey, Pen> item in penCache)
				if (item.Key.StartCapStyle != null && item.Key.StartCapStyle == capStyle)
					penKeys.Add(item.Key);
				else if (item.Key.EndCapStyle != null && item.Key.EndCapStyle == capStyle)
					penKeys.Add(item.Key);
			// delete affected Pens
			foreach (PenKey penKey in penKeys) {
				Pen pen = penCache[penKey];
				penCache.Remove(penKey);
				pen.Dispose();
				pen = null;
			}
			penKeys.Clear();

			// collect affected CustomLineCaps
			List<CapKey> capKeys = new List<CapKey>();
			foreach (KeyValuePair<CapKey, CustomLineCap> item in capCache)
				if (item.Key.CapStyle == capStyle)
					capKeys.Add(item.Key);
			// delete affected CustomLineCaps
			foreach (CapKey capKey in capKeys) {
				CustomLineCap cap = capCache[capKey];
				capCache.Remove(capKey);
				cap.Dispose();
				cap = null;
			}

			// delete all GraphicsPaths
			foreach (CapKey capKey in capKeys) {
				if (capPathCache.ContainsKey(capKey)) {
					GraphicsPath path = capPathCache[capKey];
					capPathCache.Remove(capKey);
					path.Dispose();
					path = null;
				}
			}
			capKeys.Clear();
		}


		/// <summary>
		/// Deletes all tools based on the given CharacterStyle.
		/// </summary>
		private static void NotifyCharacterStyleChanged(ICharacterStyle characterStyle)
		{
			Debug.Assert(characterStyle != null);

			while (fontCache.ContainsKey(characterStyle)) {
				Font font = fontCache[characterStyle];
				fontCache.Remove(characterStyle);
				font.Dispose();
				font = null;
			}
		}


		/// <summary>
		/// Deletes all tools based on the given ColorStyle.
		/// </summary>
		private static void NotifyColorStyleChanged(IColorStyle colorStyle)
		{
			Debug.Assert(colorStyle != null);

			// dispose affected SolidBrushes
			while (solidBrushCache.ContainsKey(colorStyle)) {
				Brush brush = solidBrushCache[colorStyle];
				solidBrushCache.Remove(colorStyle);
				brush.Dispose();
				brush = null;
			}

			// collect affected Brushes
			List<IFillStyle> fillStyles = new List<IFillStyle>();
			foreach (KeyValuePair<BrushKey, Brush> item in brushCache) {
				if (item.Key.FillStyle != null
				    && item.Key.FillStyle.FillMode != FillMode.Image &&
				    (item.Key.FillStyle.AdditionalColorStyle == colorStyle || item.Key.FillStyle.BaseColorStyle == colorStyle)) {
					fillStyles.Add(item.Key.FillStyle);
				}
			}
			// delete affected Brushes and notify that FillStyles have changed
			foreach (IFillStyle fillStyle in fillStyles)
				NotifyFillStyleChanged(fillStyle);
			fillStyles.Clear();

			// collect affected Pens
			List<PenKey> penKeys = new List<PenKey>();
			foreach (KeyValuePair<PenKey, Pen> item in penCache) {
				if (item.Key.LineStyle.ColorStyle == colorStyle)
					penKeys.Add(item.Key);
				else if (item.Key.StartCapStyle != null && item.Key.StartCapStyle.ColorStyle == colorStyle)
					penKeys.Add(item.Key);
				else if (item.Key.EndCapStyle != null && item.Key.EndCapStyle.ColorStyle == colorStyle)
					penKeys.Add(item.Key);
			}
			// delete affected Pens
			foreach (PenKey penKey in penKeys)
				// notify only affected LineStyles, affected CapStyles are notified later
				NotifyLineStyleChanged(penKey.LineStyle);
			penKeys.Clear();

			// collect affected CustomLineCaps
			List<CapKey> capKeys = new List<CapKey>();
			foreach (KeyValuePair<CapKey, CustomLineCap> item in capCache)
				if (item.Key.CapStyle.ColorStyle == colorStyle || item.Key.CapStyle.ColorStyle == colorStyle)
					capKeys.Add(item.Key);
			// delete affected CustomLineCaps
			foreach (CapKey capKey in capKeys)
				NotifyCapStyleChanged(capKey.CapStyle);
			capKeys.Clear();
		}


		/// <summary>
		/// Deletes all tools based on the given FillStyle.
		/// </summary>
		private static void NotifyFillStyleChanged(IFillStyle fillStyle)
		{
			Debug.Assert(fillStyle != null);
			BrushKey brushKey;
			brushKey.FillStyle = fillStyle;
			brushKey.Image = null;
			while (brushCache.ContainsKey(brushKey)) {
				Brush brush = brushCache[brushKey];
				brushCache.Remove(brushKey);
				brush.Dispose();
				brush = null;
			}
			while (imageAttribsCache.ContainsKey(brushKey)) {
				ImageAttributes imgAttribs = imageAttribsCache[brushKey];
				imageAttribsCache.Remove(brushKey);
				imgAttribs.Dispose();
				imgAttribs = null;
			}
		}


		/// <summary>
		/// Deletes all tools based on the given LineStyle.
		/// </summary>
		private static void NotifyLineStyleChanged(ILineStyle lineStyle)
		{
			Debug.Assert(lineStyle != null);

			// collect affected PenKeys
			List<PenKey> penKeys = new List<PenKey>();
			foreach (KeyValuePair<PenKey, Pen> item in penCache)
				if (item.Key.LineStyle == lineStyle)
					penKeys.Add(item.Key);
			// delete affected Pens
			foreach (PenKey penKey in penKeys) {
				Pen pen = penCache[penKey];
				penCache.Remove(penKey);
				pen.Dispose();
				pen = null;
			}
			penKeys.Clear();

			// collect affected CustomLineCaps
			List<CapKey> capKeys = new List<CapKey>();
			foreach (KeyValuePair<CapKey, CustomLineCap> item in capCache)
				if (item.Key.LineStyle == lineStyle)
					capKeys.Add(item.Key);
			// delete affected CustomLineCaps and their GraphicsPaths
			foreach (CapKey capKey in capKeys)
				NotifyCapStyleChanged(capKey.CapStyle);
			capKeys.Clear();
		}


		/// <summary>
		/// Deletes all tools based on the given ParagraphStyle.
		/// </summary>
		private static void NotifyParagraphStyleChanged(IParagraphStyle paragraphStyle)
		{
			Debug.Assert(paragraphStyle != null);
			while (stringFormatCache.ContainsKey(paragraphStyle)) {
				StringFormat stringFormat = stringFormatCache[paragraphStyle];
				stringFormatCache.Remove(paragraphStyle);
				stringFormat.Dispose();
				stringFormat = null;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		private static TextureBrush GetBrush(Image image, ImageLayoutMode imageLayout, float gamma, byte transparency,
		                                     bool grayScale)
		{
			if (image == null) throw new ArgumentNullException("image");
			BrushKey brushKey;
			brushKey.FillStyle = null;
			brushKey.Image = image;

			Brush brush = null;
			brushCache.TryGetValue(brushKey, out brush);
			if (!(brush is TextureBrush)) {
				// First, get ImageAttributes
				ImageAttributes imgAttribs = null;
				imageAttribsCache.TryGetValue(brushKey, out imgAttribs);
				if (imgAttribs == null) {
					imgAttribs = GdiHelpers.GetImageAttributes(imageLayout, gamma, transparency, grayScale);
					imageAttribsCache.Add(brushKey, imgAttribs);
				}

				// Create Brush
				rectBuffer.X = 0;
				rectBuffer.Y = 0;
				rectBuffer.Width = image.Width;
				rectBuffer.Height = image.Height;
				brush = new TextureBrush(image, rectBuffer, imgAttribs);

				// add created brush to the BrushCache
				if (brushCache.ContainsKey(brushKey))
					brushCache[brushKey] = brush;
				else brushCache.Add(brushKey, brush);
			}
			return (TextureBrush) brush;
		}


		private static Color GetColor(IColorStyle colorStyle, bool convertToGray)
		{
			Debug.Assert(colorStyle != null);

			if (convertToGray) {
				int luminance = 0;
				luminance += (byte) Math.Round(colorStyle.Color.R*luminanceFactorRed);
				luminance += (byte) Math.Round(colorStyle.Color.G*luminanceFactorGreen);
				luminance += (byte) Math.Round(colorStyle.Color.B*luminanceFactorBlue);
				if (luminance > 255)
					luminance = 255;
				return Color.FromArgb(colorStyle.Color.A, luminance, luminance, luminance);
			}
			else return (colorStyle != null) ? colorStyle.Color : Color.Empty;
		}


		/// <summary>
		/// Finds and returns the <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> for the given <see cref="T:Dataweb.NShape.ICapStyle" /> and <see cref="T:Dataweb.NShape.ILineStyle" />.
		/// </summary>
		private static CustomLineCap GetCustomLineCap(ICapStyle capStyle, ILineStyle lineStyle)
		{
			if (capStyle == null) throw new ArgumentNullException("capStyle");
			if (lineStyle == null) throw new ArgumentNullException("lineStyle");

			// build CapKey
			CapKey capKey;
			capKey.CapStyle = capStyle;
			capKey.LineStyle = lineStyle;
			// get GraphicsPath for the CustomLineCap
			GraphicsPath capPath = GetCapPath(capStyle, lineStyle);
			// find/create CustomLineCap
			CustomLineCap customCap = null;
			capCache.TryGetValue(capKey, out customCap);
			if (customCap == null) {
				customCap = new CustomLineCap(null, capPath);
				customCap.StrokeJoin = lineStyle.LineJoin;
				// ToDo: Use WidthScale property and remove the manual scaling
				customCap.WidthScale = 1;
				//customCap.WidthScale = 1f / lineStyle.LineWidth;
				rectFBuffer = capPath.GetBounds();
				// ToDo: Write a GetBaseInset() method for this purpose
				if (capStyle.CapShape == CapShape.OpenArrow)
					customCap.BaseInset = rectFBuffer.Height/4f;
				else customCap.BaseInset = (float) (rectFBuffer.Height - (rectFBuffer.Height + rectFBuffer.Y));
				capCache.Add(capKey, customCap);
			}
			return customCap;
		}


		private static GraphicsPath GetCapPath(ICapStyle capStyle, ILineStyle lineStyle)
		{
			if (capStyle == null) throw new ArgumentNullException("capStyle");
			if (lineStyle == null) throw new ArgumentNullException("lineStyle");

			// build CapKey
			CapKey capKey;
			capKey.CapStyle = capStyle;
			capKey.LineStyle = lineStyle;
			// find/create CapPath
			GraphicsPath capPath;
			capPathCache.TryGetValue(capKey, out capPath);
			if (capPath == null) {
				CalcCapShape(ref capPath, capStyle.CapShape, capStyle.CapSize);
				// Scale GraphicsPath down for correcting the automatic scaling that is applied to
				// LineCaps by GDI+ when altering the LineWidth of the pen
				matrix.Reset();
				matrix.Scale(1f/lineStyle.LineWidth, 1f/lineStyle.LineWidth);
				capPath.Transform(matrix);
				capPathCache.Add(capKey, capPath);
			}
			return capPath;
		}


		private static void SetLineCap(Pen pen, ILineStyle lineStyle, ICapStyle capStyle, bool isStartCap)
		{
			if (pen == null) throw new ArgumentNullException("pen");
			if (lineStyle == null) throw new ArgumentNullException("lineStyle");
			LineCap cap = LineCap.Round;
			if (capStyle != null) {
				switch (capStyle.CapShape) {
					case CapShape.None:
					case CapShape.Round:
						cap = LineCap.Round;
						break;
					case CapShape.Flat:
						cap = LineCap.Square;
						break;
					case CapShape.Peak:
						cap = LineCap.Triangle;
						break;
					default:
						cap = LineCap.Custom;
						break;
				}
			}
			if (isStartCap) {
				pen.StartCap = cap;
				if (cap == LineCap.Custom)
					pen.CustomStartCap = GetCustomLineCap(capStyle, lineStyle);
			}
			else {
				pen.EndCap = cap;
				if (cap == LineCap.Custom)
					pen.CustomEndCap = GetCustomLineCap(capStyle, lineStyle);
			}
		}


		/// <summary>
		/// (re)calculates the given GraphicsPath according to the given CapShape.
		/// </summary>
		/// <param name="capPath">Reference of the GraphicsPath to (re)calculate</param>
		/// <param name="capShape">Desired shape of the LineCap</param>
		/// <param name="capSize">Desired Size of the LineCap</param>
		private static void CalcCapShape(ref GraphicsPath capPath, CapShape capShape, int capSize)
		{
			Debug.Assert(capSize >= 0);
			if (capPath == null) capPath = new GraphicsPath();

			float halfSize = capSize/2f;
			capPath.Reset();
			switch (capShape) {
				case CapShape.ClosedArrow:
					capPath.StartFigure();
					capPath.AddLine(-halfSize, -capSize, 0, 0);
					capPath.AddLine(0, 0, halfSize, -capSize);
					capPath.AddLine(halfSize, -capSize, -halfSize, -capSize);
					capPath.CloseFigure();
					break;
				case CapShape.OpenArrow:
					float quaterSize = capSize/4f;
					capPath.StartFigure();
					capPath.AddLine(-halfSize, -capSize, 0, 0);
					capPath.AddLine(0, 0, halfSize, -capSize);
					capPath.AddLine(halfSize, -capSize, 0, -quaterSize);
					capPath.AddLine(0, -quaterSize, -halfSize, -capSize);
					capPath.CloseFigure();
					break;
				case CapShape.Triangle:
					capPath.StartFigure();
					capPath.AddLine(0, -capSize, -halfSize, 0);
					capPath.AddLine(-halfSize, 0, halfSize, 0);
					capPath.AddLine(halfSize, 0, 0, -capSize);
					capPath.CloseFigure();
					break;
				case CapShape.Circle:
					capPath.StartFigure();
					capPath.AddEllipse(-halfSize, -capSize, capSize, capSize);
					capPath.CloseFigure();
					break;
				case CapShape.Square:
					rectFBuffer.X = -halfSize;
					rectFBuffer.Y = -capSize;
					rectFBuffer.Width = halfSize + halfSize;
					rectFBuffer.Height = capSize;
					capPath.StartFigure();
					capPath.AddRectangle(rectFBuffer);
					capPath.CloseFigure();
					break;
				case CapShape.Diamond:
					capPath.StartFigure();
					capPath.AddLine(0, 0, -halfSize, -halfSize);
					capPath.AddLine(-halfSize, -halfSize, 0, -capSize);
					capPath.AddLine(0, -capSize, halfSize, -halfSize);
					capPath.AddLine(halfSize, -halfSize, 0, 0);
					capPath.CloseFigure();
					break;
				case CapShape.CenteredCircle:
					capPath.StartFigure();
					capPath.AddEllipse(-halfSize, -halfSize, capSize, capSize);
					capPath.CloseFigure();
					break;
				case CapShape.CenteredHalfCircle:
					capPath.StartFigure();
					capPath.StartFigure();
					capPath.AddArc(-halfSize, -halfSize, capSize, capSize, 0f, -180f);
					capPath.AddLine(-halfSize, 0, -halfSize - 1, 0);
					capPath.AddArc(-halfSize - 1, -halfSize - 1, capSize + 2, capSize + 2, 180f, 180f);
					capPath.AddLine(halfSize + 1, 0, halfSize, 0);
					capPath.CloseFigure();
					capPath.CloseFigure();
					break;
				case CapShape.None:
				case CapShape.Round:
					return;
				default:
					throw new NShapeUnsupportedValueException(capShape);
			}
		}

		#endregion

		#region [Private] Types

		private struct PenKey : IEquatable<PenKey>
		{
			public static bool operator ==(PenKey a, PenKey b)
			{
				return (a.LineStyle == b.LineStyle
				        && a.StartCapStyle == b.StartCapStyle
				        && a.EndCapStyle == b.EndCapStyle);
			}

			public static bool operator !=(PenKey a, PenKey b)
			{
				return !(a == b);
			}

			public ILineStyle LineStyle;

			public ICapStyle StartCapStyle;

			public ICapStyle EndCapStyle;

			/// <override></override>
			public override bool Equals(object obj)
			{
				return (obj is PenKey && this == (PenKey) obj);
			}

			/// <ToBeCompleted></ToBeCompleted>
			public bool Equals(PenKey other)
			{
				return other == this;
			}

			/// <override></override>
			public override int GetHashCode()
			{
				int hashCode = 0;
				if (LineStyle != null) hashCode ^= LineStyle.GetHashCode();
				if (StartCapStyle != null) hashCode ^= StartCapStyle.GetHashCode();
				if (EndCapStyle != null) hashCode ^= EndCapStyle.GetHashCode();
				return hashCode;
			}
		}


		private struct CapKey : IEquatable<CapKey>
		{
			public static bool operator ==(CapKey a, CapKey b)
			{
				return (a.LineStyle == b.LineStyle && a.CapStyle == b.CapStyle);
			}

			public static bool operator !=(CapKey a, CapKey b)
			{
				return !(a == b);
			}

			public ICapStyle CapStyle;

			public ILineStyle LineStyle;

			/// <override></override>
			public override bool Equals(object obj)
			{
				return (obj is CapKey && this == (CapKey) obj);
			}

			/// <ToBeCompleted></ToBeCompleted>
			public bool Equals(CapKey other)
			{
				return other == this;
			}

			/// <override></override>
			public override int GetHashCode()
			{
				int hashCode = 0;
				if (LineStyle != null) hashCode ^= LineStyle.GetHashCode();
				if (CapStyle != null) hashCode ^= CapStyle.GetHashCode();
				return hashCode;
			}
		}


		private struct BrushKey : IEquatable<BrushKey>
		{
			public static bool operator ==(BrushKey a, BrushKey b)
			{
				return (a.FillStyle == b.FillStyle
				        && a.Image == b.Image);
			}

			public static bool operator !=(BrushKey a, BrushKey b)
			{
				return !(a == b);
			}

			public IFillStyle FillStyle;

			public Image Image;

			/// <override></override>
			public override bool Equals(object obj)
			{
				return (obj is BrushKey && this == (BrushKey) obj);
			}

			/// <ToBeCompleted></ToBeCompleted>
			public bool Equals(BrushKey other)
			{
				return other == this;
			}

			/// <override></override>
			public override int GetHashCode()
			{
				int hashCode = 0;
				if (FillStyle != null) {
					hashCode ^= FillStyle.GetHashCode();
					//if (FillStyle.AdditionalColorStyle!= null)
					//   hashCode ^= FillStyle.AdditionalColorStyle.GetHashCode();
					//if (FillStyle.BaseColorStyle != null)
					//   hashCode ^= FillStyle.BaseColorStyle.GetHashCode();
					//if (FillStyle.Image != null)
					//   hashCode ^= FillStyle.Image.GetHashCode();
					//hashCode ^= FillStyle.ConvertToGrayScale.GetHashCode();
					//hashCode ^= FillStyle.FillMode.GetHashCode();
					//hashCode ^= FillStyle.FillPattern.GetHashCode();
					//hashCode ^= FillStyle.GradientAngle.GetHashCode();
					//hashCode ^= FillStyle.ImageGammaCorrection.GetHashCode();
					//hashCode ^= FillStyle.ImageLayout.GetHashCode();
					//hashCode ^= FillStyle.ImageTransparency.GetHashCode();
					//hashCode ^= FillStyle.Name.GetHashCode();
				}
				if (Image != null)
					hashCode ^= Image.GetHashCode();
				return hashCode;
			}
		}

		#endregion

		#region  Fields

		private static Dictionary<PenKey, Pen> penCache = new Dictionary<PenKey, Pen>(50);
		private static Dictionary<IColorStyle, SolidBrush> solidBrushCache = new Dictionary<IColorStyle, SolidBrush>(50);
		private static Dictionary<BrushKey, Brush> brushCache = new Dictionary<BrushKey, Brush>(20);
		private static Dictionary<BrushKey, ImageAttributes> imageAttribsCache = new Dictionary<BrushKey, ImageAttributes>(5);
		private static Dictionary<ICharacterStyle, Font> fontCache = new Dictionary<ICharacterStyle, Font>(10);

		private static Dictionary<IParagraphStyle, StringFormat> stringFormatCache =
			new Dictionary<IParagraphStyle, StringFormat>(5);

		private static Dictionary<CapKey, GraphicsPath> capPathCache = new Dictionary<CapKey, GraphicsPath>(10);
		private static Dictionary<CapKey, CustomLineCap> capCache = new Dictionary<CapKey, CustomLineCap>(10);

		private static Rectangle rectBuffer = Rectangle.Empty; // Rectangle buffer 
		private static RectangleF rectFBuffer = RectangleF.Empty; // RectangleF buffer
		private static PointF[] pointFBuffer = new PointF[0];
		private static Matrix matrix = new Matrix(); // Matrix for transformations

		// constants for the color-to-greyscale conversion
		// luminance correction factor (the human eye has preferences regarding colors)
		private const float luminanceFactorRed = 0.3f;
		private const float luminanceFactorGreen = 0.59f;
		private const float luminanceFactorBlue = 0.11f;
		// Alternative values
		//private const float luminanceFactorRed = 0.3f;
		//private const float luminanceFactorGreen = 0.5f;
		//private const float luminanceFactorBlue = 0.3f;

		#endregion
	}
}