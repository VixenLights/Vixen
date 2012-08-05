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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;


namespace Dataweb.NShape.Advanced {

	/// <summary>
	/// Defines the rendering quality of a <see cref="T:Dataweb.NShape.Controllers.IDiagramPresenters" />.
	/// </summary>
	public enum RenderingQuality {
		/// <summary>Best rendering Quality, lowest rendering speed.</summary>
		MaximumQuality,
		/// <summary>High quality rendering with acceptable performance loss.</summary>
		HighQuality,
		/// <summary>Rendering with system default settings.</summary>
		DefaultQuality,
		/// <summary>Low Quality, high rendering speed.</summary>
		LowQuality
	}


	/// <summary>
	/// A helper class providing methods mainly for setting drawing quality of graphics context and drawing images.
	/// </summary>
	internal static class GdiHelpers {

		/// <summary>
		/// If the given object exists, it will be disposed and set to null/Nothing.
		/// </summary>
		public static void DisposeObject<T>(ref T disposableObject) where T : class, IDisposable {
			if (disposableObject != null) {
				disposableObject.Dispose();
				disposableObject = null;
			}
		}


		#region Methods for drawing geometric primitives

		/// <summary>
		/// Visualizing points - used for debugging purposes
		/// </summary>
		public static void DrawPoint(Graphics gfx, Pen pen, PointF point, int radius) {
			DrawPoint(gfx, pen, point.X, point.Y, radius);
		}


		/// <summary>
		/// Visualizing points - used for debugging purposes
		/// </summary>
		public static void DrawPoint(Graphics gfx, Pen pen, float x, float y, int radius) {
			if (gfx == null) throw new ArgumentNullException("gfx");
			if (pen == null) throw new ArgumentNullException("pen");
			gfx.DrawLine(pen, x - radius, y, x + radius, y);
			gfx.DrawLine(pen, x, y - radius, x, y + radius);
		}


		/// <summary>
		/// Visualizing angles - used for debugging purposes
		/// </summary>
		public static void DrawAngle(Graphics gfx, Brush brush, PointF center, float angle, int radius) {
			if (gfx == null) throw new ArgumentNullException("gfx");
			if (brush == null) throw new ArgumentNullException("brush");
			Rectangle rect = Rectangle.Empty;
			rect.X = (int)Math.Round(center.X - radius);
			rect.Y = (int)Math.Round(center.Y - radius);
			rect.Width = rect.Height = radius + radius;
			gfx.FillPie(brush, rect, 0, angle);
			gfx.DrawPie(Pens.White, rect, 0, angle);
		}


		/// <summary>
		/// Visualizing lines - used for debugging purposes
		/// </summary>
		public static void DrawLine(Graphics graphics, Pen pen, PointF p1, PointF p2) {
			float a, b, c;
			Geometry.CalcLine(p1.X, p1.Y, p2.X, p2.Y, out a, out b, out c);
			DrawLine(graphics, pen, a, b, c);
		}


		/// <summary>
		/// Visualizing lines - used for debugging purposes
		/// </summary>
		public static void DrawLine(Graphics graphics, Pen pen, float a, float b, float c) {
			if (graphics == null) throw new ArgumentNullException("graphics");
			if (pen == null) throw new ArgumentNullException("pen");
			// Gleichung nach y auflösen, 2 Werte für X einsetzen und dann die zugehörigen y ausrechnen
			float x1 = 0, x2 = 0, y1 = 0, y2 = 0;
			if (b != 0) {
				x1 = -10000;
				x2 = 10000;
				y1 = (-(a * x1) - c) / b;
				y2 = (-(a * x2) - c) / b;
			}
			else if (a != 0) {
				y1 = -10000;
				y2 = 10000;
				x1 = (-(b * y1) - c) / a;
				x2 = (-(b * y2) - c) / a;
			}
			if ((a != 0 || b != 0) 
				&& !(float.IsNaN(x1) || float.IsNaN(x2) || float.IsNaN(y1) || float.IsNaN(y2))
				&& !(float.IsInfinity(x1) || float.IsInfinity(x2) || float.IsInfinity(y1) || float.IsInfinity(y2)))
				graphics.DrawLine(pen, x1, y1, x2, y2);
		}


		/// <summary>
		/// Visualizing angles - used for debugging purposes
		/// </summary>
		public static void DrawAngle(Graphics gfx, Brush brush, PointF center, float startAngle, float sweepAngle, int radius) {
			if (gfx == null) throw new ArgumentNullException("gfx");
			if (brush == null) throw new ArgumentNullException("brush");
			Rectangle rect = Rectangle.Empty;
			rect.X = (int)Math.Round(center.X - radius);
			rect.Y = (int)Math.Round(center.Y - radius);
			rect.Width = rect.Height = radius + radius;
			gfx.FillPie(brush, rect, startAngle, sweepAngle);
			gfx.DrawPie(Pens.White, rect, startAngle, sweepAngle);
		}


		/// <summary>
		/// Visualizing rotated rectangles - used for debugging purposes
		/// </summary>
		public static void DrawRotatedRectangle(Graphics gfx, Pen pen, RectangleF rect, float angleDeg) {
			if (gfx == null) throw new ArgumentNullException("gfx");
			if (pen == null) throw new ArgumentNullException("pen");
			PointF[] pts = new PointF[5] {
				new PointF(rect.Left, rect.Top),
				new PointF(rect.Right, rect.Top),
				new PointF(rect.Right, rect.Bottom),
				new PointF(rect.Left, rect.Bottom),
				new PointF(rect.Left, rect.Top) };
			PointF center = PointF.Empty;
			center.X = rect.Left + (rect.Width / 2f);
			center.Y = rect.Top + (rect.Height / 2f);
			matrix.Reset();
			matrix.RotateAt(angleDeg, center);
			matrix.TransformPoints(pts);
			gfx.DrawLines(pen, pts);
		}


		/// <summary>
		/// Visualizing rotated ellipses - used for debugging purposes
		/// </summary>
		public static void DrawRotatedEllipse(Graphics gfx, Pen pen, float centerX, float centerY, float width, float height, float angleDeg) {
			if (gfx == null) throw new ArgumentNullException("gfx");
			if (pen == null) throw new ArgumentNullException("pen");
			using (GraphicsPath path = new GraphicsPath()) {
				path.StartFigure();
				path.AddEllipse(centerX - width / 2f, centerY - height / 2f, width, height);

				PointF center = new PointF(centerX, centerY);
				matrix.Reset();
				matrix.RotateAt(angleDeg, center);
				path.Transform(matrix);
				gfx.DrawPath(pen, path);
			}
		}

		#endregion


		#region Graphics settings

		/// <summary>
		/// Sets all parameters that affect rendering quality / rendering speed
		/// </summary>
		public static void ApplyGraphicsSettings(Graphics graphics, RenderingQuality renderingQuality) {
			if (graphics == null) throw new ArgumentNullException("graphics");
			switch (renderingQuality) {
				case RenderingQuality.MaximumQuality:
					graphics.SmoothingMode = SmoothingMode.HighQuality;
					//graphics.TextRenderingHint = System.Drawing.Title.TextRenderingHint.AntiAliasGridFit;	// smoothed but blurry fonts
					graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;	// sharp but slightly chunky fonts
					graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					graphics.CompositingQuality = CompositingQuality.HighQuality;
					//graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;			// produces quite blurry results
					break;

				case RenderingQuality.HighQuality:
					// antialiasing and nice font rendering
					graphics.SmoothingMode = SmoothingMode.HighQuality;
					//graphics.TextRenderingHint = System.Drawing.Title.TextRenderingHint.AntiAliasGridFit;	// smoothed but blurry fonts
					graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;	// sharp but slightly chunky fonts
					graphics.InterpolationMode = InterpolationMode.High;
					break;

				case RenderingQuality.LowQuality:
					graphics.SmoothingMode = SmoothingMode.HighSpeed;
					graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
					graphics.InterpolationMode = InterpolationMode.Low;
					graphics.CompositingQuality = CompositingQuality.HighSpeed;
					graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
					break;

				case RenderingQuality.DefaultQuality:
				default:
					graphics.SmoothingMode = SmoothingMode.Default;
					graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
					graphics.InterpolationMode = InterpolationMode.Default;
					graphics.CompositingQuality = CompositingQuality.Default;
					graphics.PixelOffsetMode = PixelOffsetMode.Default;
					break;
			}
		}


		/// <summary>
		///  Copy all parameters that affect rendering quality / rendering speed from the given infoGraphics.
		/// </summary>
		public static void ApplyGraphicsSettings(Graphics graphics, Graphics infoGraphics) {
			if (graphics == null) throw new ArgumentNullException("graphics");
			if (infoGraphics == null) throw new ArgumentNullException("infoGraphics");
			graphics.SmoothingMode = infoGraphics.SmoothingMode;
			graphics.TextRenderingHint = infoGraphics.TextRenderingHint;
			graphics.InterpolationMode = infoGraphics.InterpolationMode;
			graphics.CompositingMode = infoGraphics.CompositingMode;
			graphics.CompositingQuality = infoGraphics.CompositingQuality;
			graphics.PixelOffsetMode = infoGraphics.PixelOffsetMode;
		}

		#endregion


		#region Drawing images

		/// <summary>
		/// Copies the given image. The given oldColor will be replaced by the given newColor (used for icon rendering).
		/// </summary>
		public static Bitmap GetIconBitmap(Bitmap sourceImg, Color oldBackgroundColor, Color newBackgroundColor) {
			if (sourceImg == null) throw new ArgumentNullException("sourceImg");
			Bitmap result = new Bitmap(sourceImg.Width, sourceImg.Height, sourceImg.PixelFormat);
			result.SetResolution(sourceImg.HorizontalResolution, sourceImg.VerticalResolution);
			using (Graphics gfx = Graphics.FromImage(result)) {
				using (ImageAttributes imgAttr = new ImageAttributes()) {
					ColorMap colorMap = new ColorMap();
					colorMap.OldColor = oldBackgroundColor;
					colorMap.NewColor = newBackgroundColor;
					imgAttr.SetRemapTable(new ColorMap[] { colorMap });

					gfx.Clear(Color.Transparent);
					gfx.DrawImage(sourceImg, new Rectangle(0, 0, result.Width, result.Height), 0, 0, sourceImg.Width, sourceImg.Height, GraphicsUnit.Pixel, imgAttr);
				}
			}
			return result;
		}


		/// <summary>
		/// Get a suitable GDI+ <see cref="T:System.Drawing.Drawing2D.WrapMode" /> out of the given NShapeImageLayout.
		/// </summary>
		public static WrapMode GetWrapMode(ImageLayoutMode imageLayout) {
			switch (imageLayout) {
				case ImageLayoutMode.Center:
				case ImageLayoutMode.Fit:
				case ImageLayoutMode.Original:
				case ImageLayoutMode.Stretch:
					return WrapMode.Clamp;
				case ImageLayoutMode.CenterTile:
				case ImageLayoutMode.Tile:
					return WrapMode.Tile;
				case ImageLayoutMode.FlipTile:
					return WrapMode.TileFlipXY;
				default: throw new NShapeUnsupportedValueException(imageLayout);
			}
		}


		/// <summary>
		/// Get ImageAttributes for drawing an images or creating a TextureBrush.
		/// </summary>
		public static ImageAttributes GetImageAttributes(IFillStyle fillStyle) {
			return GetImageAttributes(fillStyle.ImageLayout, fillStyle.ImageGammaCorrection, fillStyle.ImageTransparency, fillStyle.ConvertToGrayScale, false, Color.Empty);
		}


		/// <summary>
		/// Get ImageAttributes for drawing an images or creating a TextureBrush.
		/// </summary>
		public static ImageAttributes GetImageAttributes(ImageLayoutMode imageLayout) {
			return GetImageAttributes(imageLayout, -1f, 0, false, false, Color.Empty);
		}


		/// <summary>
		/// Get ImageAttributes for drawing an images or creating a TextureBrush.
		/// </summary>
		public static ImageAttributes GetImageAttributes(ImageLayoutMode imageLayout, bool forPreview) {
			return GetImageAttributes(imageLayout, -1f, 0, false, forPreview, Color.Empty);
		}


		/// <summary>
		/// Get ImageAttributes for drawing an images or creating a TextureBrush.
		/// </summary>
		public static ImageAttributes GetImageAttributes(ImageLayoutMode imageLayout, Color transparentColor) {
			return GetImageAttributes(imageLayout, -1f, 0, false, false, transparentColor);
		}


		/// <summary>
		/// Get ImageAttributes for drawing an images or creating a TextureBrush.
		/// </summary>
		public static ImageAttributes GetImageAttributes(ImageLayoutMode imageLayout, Color transparentColor, bool forPreview) {
			return GetImageAttributes(imageLayout, -1f, 0, false, forPreview, transparentColor);
		}


		/// <summary>
		/// Get ImageAttributes for drawing an images or creating a TextureBrush.
		/// </summary>
		public static ImageAttributes GetImageAttributes(ImageLayoutMode imageLayout, float gamma, byte transparency, bool grayScale) {
			return GetImageAttributes(imageLayout, gamma, transparency, grayScale, false, Color.Empty);
		}


		/// <summary>
		/// Get ImageAttributes for drawing an images or creating a TextureBrush.
		/// </summary>
		public static ImageAttributes GetImageAttributes(ImageLayoutMode imageLayout, float gamma, byte transparency, bool grayScale, bool forPreview) {
			return GetImageAttributes(imageLayout, gamma, transparency, grayScale, forPreview, Color.Empty);
		}


		/// <summary>
		/// Get ImageAttributes for drawing an images or creating a TextureBrush.
		/// </summary>
		public static ImageAttributes GetImageAttributes(ImageLayoutMode imageLayout, float gamma, byte transparency, bool grayScale, bool forPreview, Color transparentColor) {
			if (transparency < 0 || transparency > 100) throw new ArgumentOutOfRangeException("transparency");
			ImageAttributes imageAttribs = new ImageAttributes();
			//
			// Set WrapMode
			imageAttribs.SetWrapMode(GetWrapMode(imageLayout));
			//
			// Set Gamma correction
			if (gamma > 0) imageAttribs.SetGamma(gamma);
			//
			// Reset color matrix before applying effects
			ResetColorMatrix(colorMatrix);
			// Add conversion to grayScale
			if (grayScale || (forPreview && Design.PreviewsAsGrayScale))
				ApplyGrayScale(colorMatrix);
			// Add transparency
			float transparencyFactor = forPreview ? Design.GetPreviewTransparency(transparency) / 100f : transparency / 100f;
			if (transparencyFactor != 0) ApplyTransparency(colorMatrix, transparencyFactor);
			// Apply color matrix
			imageAttribs.SetColorMatrix(colorMatrix);
			//
			// Set color remap table
			if (transparentColor != Color.Empty) {
				colorMaps[0].OldColor = transparentColor;
				colorMaps[0].NewColor = Color.Transparent;
				imageAttribs.SetRemapTable(colorMaps);
			}
			return imageAttribs;
		}


		/// <summary>
		/// Draw an image into the specified bounds
		/// </summary>
		public static void DrawImage(Graphics gfx, Image image, ImageAttributes imageAttribs, ImageLayoutMode imageLayout, Rectangle dstBounds, Rectangle srcBounds) {
			DrawImage(gfx, image, imageAttribs, imageLayout, dstBounds, srcBounds, 0);
		}


		/// <summary>
		/// Draw an image into the specified bounds
		/// </summary>
		public static void DrawImage(Graphics gfx, Image image, ImageAttributes imageAttribs, ImageLayoutMode imageLayout, Rectangle dstBounds, Rectangle srcBounds, float angle) {
			PointF center = PointF.Empty;
			center.X = dstBounds.X + (dstBounds.Width / 2f);
			center.Y = dstBounds.Y + (dstBounds.Height / 2f);
			DrawImage(gfx, image, imageAttribs, imageLayout, dstBounds, srcBounds, angle, center);
		}


		/// <summary>
		/// Draw an image into the specified bounds
		/// </summary>
		public static void DrawImage(Graphics gfx, Image image, ImageAttributes imageAttribs, ImageLayoutMode imageLayout, Rectangle dstBounds, Rectangle srcBounds, float angle, PointF rotationCenter) {
			if (gfx == null) throw new ArgumentNullException("gfx");
			if (image == null) throw new ArgumentNullException("image");
			// ToDo: Optimize this (draw only the drawArea part of the image, optimize calculations and variable use, draw bitmaps by using a TextureBrush)

			float scaleX, scaleY, aspectRatio;
			aspectRatio = CalcImageScaleAndAspect(out scaleX, out scaleY, dstBounds.Width, dstBounds.Height, image, imageLayout);

			Rectangle destinationBounds = Rectangle.Empty;
			// transform image bounds
			switch (imageLayout) {
				case ImageLayoutMode.Tile:
				case ImageLayoutMode.FlipTile:
					destinationBounds = dstBounds;
					break;
				case ImageLayoutMode.Original:
					destinationBounds.X = dstBounds.X;
					destinationBounds.Y = dstBounds.Y;
					destinationBounds.Width = Math.Min(image.Width, dstBounds.Width);
					destinationBounds.Height = Math.Min(image.Height, dstBounds.Height);
					break;
				case ImageLayoutMode.CenterTile:
				case ImageLayoutMode.Center:
					destinationBounds.X = dstBounds.X + (int)Math.Round((dstBounds.Width - image.Width) / 2f);
					destinationBounds.Y = dstBounds.Y + (int)Math.Round((dstBounds.Height - image.Height) / 2f);
					destinationBounds.Width = image.Width;
					destinationBounds.Height = image.Height;
					break;
				case ImageLayoutMode.Stretch:
				case ImageLayoutMode.Fit:
					destinationBounds.X = dstBounds.X;
					destinationBounds.Y = dstBounds.Y;
					if (imageLayout == ImageLayoutMode.Fit) {
						destinationBounds.X += (int)Math.Round((dstBounds.Width - (image.Width * scaleX)) / 2f);
						destinationBounds.Y += (int)Math.Round((dstBounds.Height - (image.Height * scaleY)) / 2f);
					}
					destinationBounds.Width = (int)Math.Round(image.Width * scaleX);
					destinationBounds.Height = (int)Math.Round(image.Height * scaleY);
					break;
				default:
					throw new NShapeException(string.Format("Unexpected {0} '{1}'.", imageLayout.GetType(), imageLayout));
			}

			if (angle != 0) {
				gfx.TranslateTransform(rotationCenter.X, rotationCenter.Y);
				gfx.RotateTransform(angle);
				gfx.TranslateTransform(-rotationCenter.X, -rotationCenter.Y);
			}

			GraphicsUnit gfxUnit = GraphicsUnit.Display;
			RectangleF imageBounds = image.GetBounds(ref gfxUnit);
			int srcX, srcY, srcWidth, srcHeight;
			switch (imageLayout) {
				case ImageLayoutMode.CenterTile:
				case ImageLayoutMode.FlipTile:
				case ImageLayoutMode.Tile:
					int startX, startY, endX, endY;
					if (imageLayout == ImageLayoutMode.CenterTile) {
						int nX = (int)Math.Ceiling(dstBounds.Width / (float)image.Width);
						int nY = (int)Math.Ceiling(dstBounds.Height / (float)image.Height);
						if (nX == 1) startX = destinationBounds.X;
						else startX = (int)Math.Round(destinationBounds.X - ((image.Width * nX) / 2f));
						if (nY == 1) startY = destinationBounds.Y;
						else startY = (int)Math.Round(destinationBounds.Y - ((image.Height * nY) / 2f));
					} else {
						startX = dstBounds.X;
						startY = dstBounds.Y;
					}
					endX = dstBounds.Right;
					endY = dstBounds.Bottom;

					Rectangle r = Rectangle.Empty;
					r.Width = image.Width;
					r.Height = image.Height;
					for (int x = startX; x < endX; x += image.Width) {
						for (int y = startY; y < endY; y += image.Height) {
							// Set source bounds location
							srcX = (x < 0) ? -startX : 0;
							srcY = (y < 0) ? -startY : 0;
							// Set destination bounds location
							r.X = Math.Max(x, dstBounds.X);
							r.Y = Math.Max(y, dstBounds.Y);
							// set source and destination bounds' size
							if (x + image.Width > endX)
								srcWidth = r.Width = endX - r.X;
							else
								srcWidth = r.Width = image.Width - srcX;

							if (y + image.Height > endY)
								srcHeight = r.Height = endY - r.Y;
							else
								srcHeight = r.Height = image.Height - srcY;

							if (imageLayout == ImageLayoutMode.FlipTile) {
								int modX = (x / image.Width) % 2;
								int modY = (y / image.Height) % 2;
								if (modX != 0) {
									srcX = image.Width - srcX;
									srcWidth = -srcWidth;
								}
								if (modY != 0) {
									srcY = image.Height - srcY;
									srcHeight = -srcHeight;
								}
							}
							gfx.DrawImage(image, r, srcX, srcY, srcWidth, srcHeight, GraphicsUnit.Pixel, imageAttribs);
						}
					}
					break;
				case ImageLayoutMode.Original:
				case ImageLayoutMode.Center:
					if (image.Width > dstBounds.Width || image.Height > dstBounds.Height) {
						srcWidth = destinationBounds.Width = Math.Min(image.Width, dstBounds.Width);
						srcHeight = destinationBounds.Height = Math.Min(image.Height, dstBounds.Height);
						destinationBounds.X = dstBounds.X;
						destinationBounds.Y = dstBounds.Y;
					} else {
						srcWidth = image.Width;
						srcHeight = image.Height;
					}
					if (imageLayout == ImageLayoutMode.Center) {
						srcX = image.Width - srcWidth;
						srcY = image.Height - srcHeight;
					} else {
						srcX = (int)imageBounds.X;
						srcY = (int)imageBounds.Y;
					}
					gfx.DrawImage(image, destinationBounds, srcX, srcY, srcWidth, srcHeight, GraphicsUnit.Pixel, imageAttribs);
					break;
				case ImageLayoutMode.Fit:
				case ImageLayoutMode.Stretch:
					gfx.DrawImage(image, destinationBounds, imageBounds.X, imageBounds.Y, imageBounds.Width, imageBounds.Height, gfxUnit, imageAttribs);
					break;
				default: throw new NShapeUnsupportedValueException(imageLayout);
			}
			if (angle != 0) {
				gfx.TranslateTransform(rotationCenter.X, rotationCenter.Y);
				gfx.RotateTransform(-angle);
				gfx.TranslateTransform(-rotationCenter.X, -rotationCenter.Y);
			}
		}

		#endregion


		#region Creating and transforming brushes

		/// <summary>
		/// Copies the given image into a new image if the desired size is at half (or less) of the image's size, the image will be shrinked to the desired size.
		/// If the desired size is more than half of the image's size, the original image will be returned.
		/// </summary>
		public static Image GetBrushImage(Image image, int desiredWidth, int desiredHeight) {
			if (image == null) throw new ArgumentNullException("image");
			if (desiredWidth <= 0) desiredWidth = 1;
			if (desiredHeight <= 0) desiredHeight = 1;
			float scaleFactor = Geometry.CalcScaleFactor(
				image.Width,
				image.Height,
				image.Width / Math.Max(1, (image.Width / desiredWidth)),
				image.Height / Math.Max(1, (image.Height / desiredHeight)));
			if (scaleFactor > 0.75)
				return image;
			else {
				int scaledWidth = (int)Math.Round(image.Width * scaleFactor);
				int scaledHeight = (int)Math.Round(image.Height * scaleFactor);
				Stopwatch sw = new Stopwatch();
				sw.Start();
				Bitmap newImage = (Bitmap)image.GetThumbnailImage(scaledWidth, scaledHeight, null, IntPtr.Zero);
				sw.Stop();
				Debug.Print("Creating Thumbnail scaled to {0:F2}%: {1}", scaleFactor * 100, sw.Elapsed);
				return newImage;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static TextureBrush CreateTextureBrush(Image image, int width, int height, ImageLayoutMode imageLayout, float gamma, byte transparency, bool grayScale) {
			if (image == null) throw new ArgumentNullException("image");
			return CreateTextureBrush(image, width, height, GetImageAttributes(imageLayout, gamma, transparency, grayScale));
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static TextureBrush CreateTextureBrush(Image image, ImageLayoutMode imageLayout, float gamma, byte transparency, bool grayScale) {
			if (image == null) throw new ArgumentNullException("image");
			return CreateTextureBrush(image, GetImageAttributes(imageLayout, gamma, transparency, grayScale));
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static TextureBrush CreateTextureBrush(Image image, ImageAttributes imageAttribs) {
			if (image == null) throw new ArgumentNullException("image");
			Rectangle brushBounds = Rectangle.Empty;
			brushBounds.X = 0;
			brushBounds.Y = 0;
			brushBounds.Width = image.Width;
			brushBounds.Height = image.Height;
			return new TextureBrush(image, brushBounds, imageAttribs);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static TextureBrush CreateTextureBrush(Image image, int desiredWidth, int desiredHeight, ImageAttributes imageAttribs) {
			if (image == null) throw new ArgumentNullException("image");
			if (!(image is Bitmap)) throw new ArgumentException(string.Format("{0} are not supported for this operation.", image.GetType().Name));
			return CreateTextureBrush(GetBrushImage(image, desiredWidth, desiredHeight), imageAttribs);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static void TransformLinearGradientBrush(LinearGradientBrush brush, float gradientAngleDeg, Rectangle unrotatedBounds, Point center, float angleDeg) {
			if (brush == null) throw new ArgumentNullException("brush");
			if (brush == null) throw new ArgumentNullException("brush");
			// move brush higher than needed  and make the brush larger than needed
			// (ensure that there are no false color pixels from antialiasing inside the gradient)
			
			PointF boundsCenter = PointF.Empty;
			boundsCenter.X = unrotatedBounds.X + (unrotatedBounds.Width / 2f);
			boundsCenter.Y = unrotatedBounds.Y + (unrotatedBounds.Height / 2f);
			float gradientSize = (int)Math.Ceiling(CalculateGradientSize(angleDeg, gradientAngleDeg, boundsCenter.X - unrotatedBounds.Left, Math.Max(boundsCenter.Y - unrotatedBounds.Top, unrotatedBounds.Bottom - boundsCenter.Y))) + 2;
			float scaleFactor = ((float)Math.Sqrt((gradientSize * gradientSize) * 2) + 1f) / ((LinearGradientBrush)brush).Rectangle.Width;

			float dx = boundsCenter.X - center.X;
			float dy = boundsCenter.Y - center.Y;
			Geometry.RotatePoint(0, 0, angleDeg, ref dx, ref dy);

			brush.ResetTransform();
			brush.TranslateTransform(center.X + dx, center.Y + dy - gradientSize);
			brush.RotateTransform(gradientAngleDeg);
			brush.ScaleTransform(scaleFactor, scaleFactor);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static void TransformLinearGradientBrush(LinearGradientBrush brush, float gradientAngleDeg, RectangleF unrotatedBounds, PointF center, float angleDeg) {
			if (brush == null) throw new ArgumentNullException("brush");
			// move brush higher than needed  and make the brush larger than needed
			// (ensure that there are no false color pixels from antialiasing inside the gradient)
			float gradientSize = (CalculateGradientSize(angleDeg, gradientAngleDeg, center.X - unrotatedBounds.Left, Math.Max(center.Y - unrotatedBounds.Top, unrotatedBounds.Bottom - center.Y))) + 2;
			float scaleFactor = ((float)Math.Sqrt((gradientSize * gradientSize) * 2) + 1f) / ((LinearGradientBrush)brush).Rectangle.Width;

			brush.ResetTransform();
			brush.TranslateTransform(center.X, center.Y - gradientSize);
			brush.RotateTransform(gradientAngleDeg);
			brush.ScaleTransform(scaleFactor, scaleFactor);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static void TransformPathGradientBrush(PathGradientBrush brush, Rectangle unrotatedBounds, Point center, float angleDeg) {
			throw new NotImplementedException();
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static void TransformPathGradientBrush(PathGradientBrush brush, Rectangle unrotatedBounds, Rectangle rotatedBounds, float angleDeg) {
			throw new NotImplementedException();
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static void TransformTextureBrush(TextureBrush brush, ImageLayoutMode imageLayout, Rectangle unrotatedBounds, Point center, float angleDeg) {
			if (brush == null) throw new ArgumentNullException("brush");
			// scale image
			float scaleX, scaleY;
			GdiHelpers.CalcImageScaleAndAspect(out scaleX, out scaleY, unrotatedBounds.Width, unrotatedBounds.Height, brush.Image, imageLayout);
			// rotate at center point
			((TextureBrush)brush).ResetTransform();
			((TextureBrush)brush).TranslateTransform(center.X, center.Y);
			((TextureBrush)brush).RotateTransform(angleDeg);
			((TextureBrush)brush).TranslateTransform(-unrotatedBounds.Width / 2f, -unrotatedBounds.Height / 2f);
			// scale image
			switch (imageLayout) {
				case ImageLayoutMode.Tile:
				case ImageLayoutMode.FlipTile:
					// nothing to do
					break;
				case ImageLayoutMode.Center:
				case ImageLayoutMode.CenterTile:
				case ImageLayoutMode.Original:
					((TextureBrush)brush).TranslateTransform((unrotatedBounds.Width - brush.Image.Width) / 2f, (unrotatedBounds.Height - brush.Image.Height) / 2f);
					break;
				case ImageLayoutMode.Stretch:
				case ImageLayoutMode.Fit:
					if (imageLayout == ImageLayoutMode.Fit)
						((TextureBrush)brush).TranslateTransform((unrotatedBounds.Width - (brush.Image.Width * scaleX)) / 2f, (unrotatedBounds.Height - (brush.Image.Height * scaleY)) / 2f);
					((TextureBrush)brush).ScaleTransform(scaleX, scaleY);
					break;
				default: throw new NShapeUnsupportedValueException(imageLayout);
			}
		}


		/// <summary>
		/// Calculates scale factors and Aspect ratio depending on the given ImageLayout.
		/// </summary>
		/// <param name="scaleFactorX">Scaling factor on X axis.</param>
		/// <param name="scaleFactorY">Scaling factor on Y axis.</param>
		/// <param name="dstWidth">The desired width of the image.</param>
		/// <param name="dstHeight">The desired height of the image.</param>
		/// <param name="image">the untransformed Image object.</param>
		/// <param name="imageLayout">ImageLayout enumeration. Deines the scaling behavior.</param>
		/// <returns>Aspect ratio of the image.</returns>
		public static float CalcImageScaleAndAspect(out float scaleFactorX, out float scaleFactorY, int dstWidth, int dstHeight, Image image, ImageLayoutMode imageLayout) {
			float aspectRatio = 1;
			scaleFactorX = 1f;
			scaleFactorY = 1f;
			if (image != null && image.Height > 0 && image.Width > 0) {
				if (image.Width == image.Height) aspectRatio = 1;
				else aspectRatio = (float)image.Width / image.Height;

				switch (imageLayout) {
					case ImageLayoutMode.Fit:
						double lowestRatio = Math.Min((double)dstWidth / (double)image.Width, (double)dstHeight / (double)image.Height);
						scaleFactorX = (float)Math.Round(lowestRatio, 6);
						scaleFactorY = (float)Math.Round(lowestRatio, 6);
						break;
					case ImageLayoutMode.Stretch:
						scaleFactorX = (float)dstWidth / image.Width;
						scaleFactorY = (float)dstHeight / image.Height;
						break;
					case ImageLayoutMode.Original:
					case ImageLayoutMode.Center:
						// nothing to do
						break;
					case ImageLayoutMode.CenterTile:
					case ImageLayoutMode.Tile:
					case ImageLayoutMode.FlipTile:
						// nothing to do
						break;
					default:
						throw new NShapeException(string.Format("Unexpected {0} '{1}'", imageLayout.GetType(), imageLayout));
				}
			}
			return aspectRatio;
		}


		// calculate gradient size dependent of the shape's rotation
		/// <ToBeCompleted></ToBeCompleted>
		private static float CalculateGradientSize(float angle, float gradientAngle, int leftToCenter, int topToCenter) {
			float result = 0f;

			double a = angle;
			int r = leftToCenter;
			int o = topToCenter;

			if (a < 0) a = 360 + a;
			a = a % 180;
			if (a / 90 >= 1 && a / 90 < 2) {
				r = topToCenter;
				o = leftToCenter;
				a = a % 90;
			}
			if (a > 45)
				a = (a - ((a % 45) * 2)) * (Math.PI / 180);
			else
				a = a * (Math.PI / 180);
			double cos = Math.Cos(a);
			double sin = Math.Sin(a);

			// calculate offset 
			double dR = (r * sin) + (r * cos);
			double dO = Math.Abs((o * cos) - (o * sin));
			result = (float)Math.Round(dR + dO, 6);

			return result;
		}


		// Calculate gradient size dependent of the shape's rotation
		/// <ToBeCompleted></ToBeCompleted>
		private static float CalculateGradientSize(float angle, float gradientAngle, float leftToCenter, float topToCenter) {
			float result = 0f;

			double a = angle;
			float r = leftToCenter;
			float o = topToCenter;

			if (a < 0) a = 360 + a;
			a = a % 180;
			if (a / 90 >= 1 && a / 90 < 2) {
				r = topToCenter;
				o = leftToCenter;
				a = a % 90;
			}
			if (a > 45)
				a = (a - ((a % 45) * 2)) * (Math.PI / 180);
			else
				a = a * (Math.PI / 180);
			double cos = Math.Cos(a);
			double sin = Math.Sin(a);

			// calculate offset 
			double dR = (r * sin) + (r * cos);
			double dO = Math.Abs((o * cos) - (o * sin));
			result = (float)Math.Round(dR + dO, 6);

			return result;
		}

		#endregion


		#region Exporting Images

		/// <ToBeCompleted></ToBeCompleted>
		public static void SaveImageToFile(Image image, string filePath, ImageFileFormat imageFormat) {
			SaveImageToFile(image, filePath, imageFormat, 85);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static void SaveImageToFile(Image image, string filePath, ImageFileFormat imageFormat, int compressionQuality) {
			if (image == null) throw new ArgumentNullException("image");
			if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");
			if (image is Metafile) {
				EmfHelper.SaveEnhMetaFile(filePath, (Metafile)image.Clone());

				#region Old version: Works but replaces all metafile records with a single DrawImage record
				//// Create a new metafile
				//Graphics gfx = Graphics.FromHwnd(IntPtr.Zero);
				//IntPtr hdc = gfx.GetHdc();
				//Metafile metaFile = new Metafile(filePath, 
				//   hdc, 
				//   Rectangle.FromLTRB(0, 0, image.Width, image.Height), 
				//   MetafileFrameUnit.Pixel, 
				//   (imageFormat == NShapeImageFormat.EmfPlus) ? EmfType.EmfPlusDual : EmfType.EmfOnly);
				//gfx.ReleaseHdc(hdc);
				//gfx.Dispose();

				//// Create graphics context for drawing
				//gfx = Graphics.FromImage(metaFile);
				//GdiHelpers.ApplyGraphicsSettings(gfx, NShapeRenderingQuality.MaximumQuality);
				//// Draw image
				//gfx.DrawImage(image, Point.Empty);
				
				//gfx.Dispose();
				//metaFile.Dispose();
				#endregion

			} else if (image is Bitmap) {
				ImageCodecInfo codecInfo = GetEncoderInfo(imageFormat);

				EncoderParameters encoderParams = new EncoderParameters(3);
				encoderParams.Param[0] = new EncoderParameter(Encoder.RenderMethod, (long)EncoderValue.RenderProgressive);
				// JPG specific encoder parameter
				encoderParams.Param[1] = new EncoderParameter(Encoder.Quality, (long)compressionQuality);
				// TIFF specific encoder parameter
				encoderParams.Param[2] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionLZW);

				image.Save(filePath, codecInfo, encoderParams);
			} else image.Save(filePath, GetGdiImageFormat(imageFormat));
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static void CreateMetafile(string filePath, ImageFileFormat imageFormat, int width, int height, DrawCallback callback) {
			if (callback == null) throw new ArgumentNullException("callback");
			if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");
			Metafile metaFile = null;
			// Create a new metafile
			using (Graphics gfx = Graphics.FromHwnd(IntPtr.Zero)) {
				IntPtr hdc = gfx.GetHdc();
				try {
					metaFile= new Metafile(filePath,
						hdc,
						Rectangle.FromLTRB(0, 0, width, height),
						MetafileFrameUnit.Pixel,
						(imageFormat == ImageFileFormat.EmfPlus) ? EmfType.EmfPlusDual : EmfType.EmfOnly);
				} finally { gfx.ReleaseHdc(hdc); }
			}
			// Create graphics context for drawing
			if (metaFile != null) {
				using (Graphics gfx = Graphics.FromImage(metaFile)) {
					GdiHelpers.ApplyGraphicsSettings(gfx, RenderingQuality.MaximumQuality);
					// Draw image
					Rectangle bounds = Rectangle.Empty;
					bounds.Width = width;
					bounds.Height = height;
					callback(gfx, bounds);
				}
				metaFile.Dispose();
				metaFile = null;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static ImageFormat GetGdiImageFormat(ImageFileFormat imageFormat) {
			ImageFormat result;
			switch (imageFormat) {
				case ImageFileFormat.Bmp: result = ImageFormat.Bmp; break;
				case ImageFileFormat.Emf:
				case ImageFileFormat.EmfPlus: result = ImageFormat.Emf; break;
				case ImageFileFormat.Gif: result = ImageFormat.Gif; break;
				case ImageFileFormat.Jpeg: result = ImageFormat.Jpeg; break;
				case ImageFileFormat.Png: result = ImageFormat.Png; break;
				case ImageFileFormat.Tiff: result = ImageFormat.Tiff; break;
				default: return result = ImageFormat.Bmp;
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static ImageCodecInfo GetEncoderInfo(ImageFileFormat imageFormat) {
			ImageFormat format = GetGdiImageFormat(imageFormat);
			ImageCodecInfo result = null;
			ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
			foreach (ImageCodecInfo ici in encoders)
				if (ici.FormatID.Equals(format.Guid)) {
					result = ici;
					break;
				}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public delegate void DrawCallback(Graphics graphics, Rectangle bounds);


		#endregion


		#region [Private] Methods

		private static void ResetColorMatrix(ColorMatrix colorMatrix) {
			// Reset color matrix to these values:
			//		1	0	0	0	0
			//		0	1	0	0	0
			//		0	0	1	0	0
			//		0	0	0	1	0
			//		0	0	0	0	1
			colorMatrix.Matrix00 =
			colorMatrix.Matrix11 =
			colorMatrix.Matrix22 =
			colorMatrix.Matrix33 =
			colorMatrix.Matrix44 = 1;
			colorMatrix.Matrix01 = colorMatrix.Matrix02 = colorMatrix.Matrix03 = colorMatrix.Matrix04 =
			colorMatrix.Matrix10 = colorMatrix.Matrix12 = colorMatrix.Matrix13 = colorMatrix.Matrix14 =
			colorMatrix.Matrix20 = colorMatrix.Matrix21 = colorMatrix.Matrix23 = colorMatrix.Matrix24 =
			colorMatrix.Matrix30 = colorMatrix.Matrix31 = colorMatrix.Matrix32 = colorMatrix.Matrix34 =
			colorMatrix.Matrix40 = colorMatrix.Matrix41 = colorMatrix.Matrix42 = colorMatrix.Matrix43 = 0;
		}


		private static void ApplyGrayScale(ColorMatrix colorMatrix) {
			// Grayscale == no color saturation
			ApplySaturation(colorMatrix, 0);
		}


		private static void ApplyContrast(ColorMatrix colorMatrix, float contrastFactor) {
			ApplyContrast(colorMatrix, contrastFactor, contrastFactor, contrastFactor);
		}


		private static void ApplyContrast(ColorMatrix colorMatrix, float contrastFactorR, float contrastFactorG, float contrastFactorB) {
			// Contrast correction matrix:
			//		c	0	0	0	0
			//		0	c	0	0	0
			//		0	0	c	0	0
			//		0	0	0	1	0
			//		0	0	0	0	1
			// Note: 
			// Due to a overflow bug in GDI+, the RGB color channel values will flip to 0 as soon as 
			// the original value multiplied with the contrast factor exceeds 255.
			// This bug was corrected with Windows 7.
			
			// Workaround for the GDI+ bug (see above)
			colorMatrix.Matrix40 = 
			colorMatrix.Matrix41 = 
			colorMatrix.Matrix42 = 0.000001f;

			// Set contrast color shear matrix
			colorMatrix.Matrix00 *= contrastFactorR;
			colorMatrix.Matrix11 *= contrastFactorG;
			colorMatrix.Matrix22 *= contrastFactorB;
		}


		private static void ApplyInvertation(ColorMatrix colorMatrix) {
			ApplyInvertation(colorMatrix, 1);
		}


		private static void ApplyInvertation(ColorMatrix colorMatrix, float invertationFactor) {
			ApplyInvertation(colorMatrix, invertationFactor, invertationFactor, invertationFactor);
		}


		private static void ApplyInvertation(ColorMatrix colorMatrix, float invertFactorR, float invertFactorG, float invertFactorB) {
			// Matrix layout:
			//		-r		0		0		0		0
			//		0		-g		0		0		0
			//		0		0		-b		0		0
			//		0		0		0		1		0
			//		1		1		1		0		1
			// Set invertation color shear matrix
			colorMatrix.Matrix00 *= -invertFactorR;
			colorMatrix.Matrix11 *= -invertFactorG;
			colorMatrix.Matrix22 *= -invertFactorB;
			colorMatrix.Matrix40 = colorMatrix.Matrix41 = colorMatrix.Matrix42 = 1f;
		}


		private static void ApplySaturation(ColorMatrix colorMatrix, float saturationFactor) {
			ApplySaturation(colorMatrix, saturationFactor, saturationFactor, saturationFactor);
		}


		private static void ApplySaturation(ColorMatrix colorMatrix, float saturationFactorR, float saturationFactorG, float saturationFactorB) {
			float complementR = luminanceFactorR * (colorMatrix.Matrix00 - saturationFactorR);
			float complementG = luminanceFactorG * (colorMatrix.Matrix11 - saturationFactorG);
			float complementB = luminanceFactorB * (colorMatrix.Matrix22 - saturationFactorB);
			//float complementR = 0.3086f * (colorMatrix.Matrix00 - saturationFactorR);
			//float complementG = 0.6094f * (colorMatrix.Matrix11 - saturationFactorG);
			//float complementB = 0.0820f * (colorMatrix.Matrix22 - saturationFactorB);

			colorMatrix.Matrix00 = complementR + saturationFactorR;
			colorMatrix.Matrix01 = complementR;
			colorMatrix.Matrix02 = complementR;

			colorMatrix.Matrix10 = complementG;
			colorMatrix.Matrix11 = complementG + saturationFactorG;
			colorMatrix.Matrix12 = complementG;

			colorMatrix.Matrix20 = complementB;
			colorMatrix.Matrix21 = complementB;
			colorMatrix.Matrix22 = complementB + saturationFactorB;
		}


		private static void ApplyTransparency(ColorMatrix colorMatrix, float transparencyFactor) {
			colorMatrix.Matrix33 *= 1f - transparencyFactor;
		}
		
		#endregion


		#region Fields

		private static ImageAttributes imageAttribs = new ImageAttributes();
		private static ColorMap[] colorMaps = new ColorMap[] { new ColorMap() };
		private static ColorMatrix colorMatrix = new ColorMatrix();
		private static Matrix matrix = new Matrix();

		// Constants for the color-to-greyscale conversion
		// Luminance correction factor (the human eye has preferences regarding colors)
		private const float luminanceFactorR = 0.3f;
		private const float luminanceFactorG = 0.59f;
		private const float luminanceFactorB = 0.11f;
		// Alternative values
		//private const float luminanceFactorRed = 0.3f;
		//private const float luminanceFactorGreen = 0.5f;
		//private const float luminanceFactorBlue = 0.3f;

		#endregion
	}


	/// <summary>
	/// This class combines GDI+ images (bitmaps or metafiles) with a name (typically the filename without path and extension).
	/// </summary>
	[TypeConverter("Dataweb.NShape.WinFormsUI.NamedImageTypeConverter, Dataweb.NShape.WinFormsUI")]
	public class NamedImage : IDisposable {

		/// <summary>
		/// Loads an image from a file.
		/// </summary>
		public static NamedImage FromFile(string fileName) {
			return new NamedImage(fileName);
		}


		/// Indicates whether the specified <see cref="T:Dataweb.NShape.Advanced.NamedImage" /> is null or has neither an <see cref="T:System.Drawing.Imaging.Image" /> nor an existing file to load from.
		public static bool IsNullOrEmpty(NamedImage namedImage) {
			if (namedImage != null) {
				if (namedImage.image != null)
					return false;
				else if (File.Exists(namedImage.FilePath))
					return false;
			}
			return true;
		}


		/// <override></override>
		public override string ToString() {
			return this.name;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.NamedImage" />.
		/// </summary>
		public NamedImage() {
			image = null;
			name = string.Empty;
			filePath = string.Empty;
			//canLoadFromFile = false;
			//imageType = typeof(Image);
			imageSize = Size.Empty;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.NamedImage" />.
		/// </summary>
		public NamedImage(string fileName)
			: this() {
			if (fileName == null) throw new ArgumentNullException("fileName");
			if (fileName == string.Empty) throw new ArgumentException("fileName");
			Load(fileName);
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.NamedImage" />.
		/// </summary>
		/// <remarks>The given image will be cloned.</remarks>
		public NamedImage(Image image, string name)
			: this() {
			if (image == null) throw new ArgumentNullException("image");
			if (name == null) throw new ArgumentNullException("name");
			Image = (Image)image.Clone();
			Name = name;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.NamedImage" />.
		/// </summary>
		/// <remarks>The given image will be cloned.</remarks>
		public NamedImage(Image image, string fileName, string name)
			: this() {
			if (image == null) throw new ArgumentNullException("image");
			if (fileName == null) throw new ArgumentNullException("fileName");
			if (name == null) throw new ArgumentNullException("name");
			Name = name;
			FilePath = fileName;
			Image = (Image)image.Clone();
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.NamedImage" />.
		/// </summary>
		public NamedImage(NamedImage source) {
			if (source == null) throw new ArgumentNullException("source");
			Name = source.Name;
			FilePath = source.FilePath;
			//this.canLoadFromFile = source.canLoadFromFile;
			//if (source.canLoadFromFile) {
			//   image = null;	// Load on next usage
			//   imageSize = source.imageSize;
			//   imageType = source.imageType;
			//} else {
				if (source.Image == null)
					Image = null;
				else Image = (Image)source.Image.Clone();
			//}
		}


		/// <summary>
		/// Finalizer of <see cref="T:Dataweb.NShape.Advanced.NamedImage" />.
		/// </summary>
		~NamedImage() {
			Dispose();
		}


		#region IDisposable Members

		/// <summary>
		/// Releases all resources.
		/// </summary>
		public void Dispose() {
			if (image != null) image.Dispose();
			image = null;
		}

		#endregion


		/// <summary>
		/// Creates a copy of this <see cref="T:Dataweb.NShape.Advanced.NamedImage" />.
		/// </summary>
		public NamedImage Clone() {
			return new NamedImage(this);
		}


		/// <summary>
		/// The image.
		/// </summary>
		public Image Image {
			get {
				if (image == null && File.Exists(filePath))
					Load(filePath);
				return image;
			}
			set {
				//canLoadFromFile = false;
				//imageType = typeof(Image);
				imageSize = Size.Empty;
				image = value;
				if (image != null) {
					imageSize = image.Size;
					//imageType = image.GetType();
				}
			}
		}


		/// <summary>
		/// Name of the image (typically the filename without path and extension)
		/// </summary>
		public string Name {
			get {
				if (!string.IsNullOrEmpty(name))
					return name;
				else if (!string.IsNullOrEmpty(filePath))
					return Path.GetFileNameWithoutExtension(filePath);
				else return string.Empty;
			}
			set {
				if (string.IsNullOrEmpty(value))
					name = string.Empty;
				else name = value;
			}
		}


		/// <summary>
		/// Path to the image file
		/// </summary>
		public string FilePath {
			get { return filePath; }
			set {
				if (string.IsNullOrEmpty(value))
					filePath = string.Empty;
				else filePath = value;
			}
		}


		/// <summary>
		/// Width of the image. 0 if no image is set.
		/// </summary>
		public int Width {
			get { return imageSize.Width; }
		}


		/// <summary>
		/// Height of the image. 0 if no image is set.
		/// </summary>
		public int Height {
			get { return imageSize.Height; }
		}


		/// <summary>
		/// Size of the image. 0 if no image is set.
		/// </summary>
		public Size Size {
			get { return imageSize; }
		}


		/// <summary>
		/// Saves this NamedImage to an image file.
		/// </summary>
		/// <param name="directoryName"></param>
		/// <param name="format"></param>
		public void Save(string directoryName, ImageFormat format) {
			if (!Directory.Exists(directoryName))
				throw new FileNotFoundException("Directory does not exist.", directoryName);
			string fileName = Name;
			if (string.IsNullOrEmpty(fileName)) fileName = Guid.NewGuid().ToString();
			fileName += GetImageFormatExtension(format);
			image.Save(Path.Combine(directoryName, fileName), format);

			if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(filePath)) {
				filePath = fileName;
				//canLoadFromFile = true;
			}
		}


		/// <summary>
		/// Loads the imagefile into this NamedImage.
		/// </summary>
		public void Load(string fileName) {
			if (!File.Exists(fileName)) throw new FileNotFoundException("File not found or access denied.", fileName);
			// GDI+ only reads the file header and keeps the image file locked for loading the 
			// image data later on demand. 
			// So we have to read the entire image to a buffer and create the image from a MemoryStream
			//
			// Read the image data into a byte array
            byte[] buffer = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
                // Copy image data to memory buffer
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
            }
			// Create the image from the read byte buffer (does not copy the buffer)
			image = Image.FromStream(new MemoryStream(buffer), true, true);
			imageSize = image.Size;
			imageType = image.GetType();
			if (!canLoadFromFile) canLoadFromFile = true;
			if (filePath != fileName) filePath = fileName;
		}


		/// <summary>
		/// Gets the appropriate file extension for the given ImageFormat.
		/// </summary>
		private string GetImageFormatExtension(ImageFormat imageFormat) {
			string result = string.Empty;
			if (image.RawFormat.Guid == ImageFormat.Bmp.Guid) result = ".bmp";
			else if (image.RawFormat.Guid == ImageFormat.Emf.Guid) result = ".emf";
			else if (image.RawFormat.Guid == ImageFormat.Exif.Guid) result = ".exif";
			else if (image.RawFormat.Guid == ImageFormat.Gif.Guid) result = ".gif";
			else if (image.RawFormat.Guid == ImageFormat.Icon.Guid) result = ".ico";
			else if (image.RawFormat.Guid == ImageFormat.Jpeg.Guid) result = ".jpeg";
			else if (image.RawFormat.Guid == ImageFormat.MemoryBmp.Guid) result = ".bmp";
			else if (image.RawFormat.Guid == ImageFormat.Png.Guid) result = ".png";
			else if (image.RawFormat.Guid == ImageFormat.Tiff.Guid) result = ".tiff";
			else if (image.RawFormat.Guid == ImageFormat.Wmf.Guid) result = ".wmf";
			else Debug.Fail("Unsupported image format.");
			return result;
		}

		#region Fields
		
		private Image image = null;
		private string name = string.Empty;
		private string filePath = string.Empty;
		private Size imageSize = Size.Empty;
		private bool canLoadFromFile = false;
		private Type imageType = typeof(Image);
		private System.Text.ASCIIEncoding asciiEncoder = new System.Text.ASCIIEncoding();
		
		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	public enum ExifPropertyTags {
		/// <ToBeCompleted></ToBeCompleted>
		ImageWidth = 0x100,
		/// <ToBeCompleted></ToBeCompleted>
		ImageLength = 0x101,
		/// <ToBeCompleted></ToBeCompleted>
		BitsPerSample = 0x102,
		/// <ToBeCompleted></ToBeCompleted>
		Compression = 0x103,
		/// <ToBeCompleted></ToBeCompleted>
		PhotometricInterpretation = 0x106,
		/// <ToBeCompleted></ToBeCompleted>
		FillOrder = 0x10A,
		/// <ToBeCompleted></ToBeCompleted>
		DocumentName = 0x10D,
		/// <ToBeCompleted></ToBeCompleted>
		ImageDescription = 0x10E,
		/// <ToBeCompleted></ToBeCompleted>
		Make = 0x10F,
		/// <ToBeCompleted></ToBeCompleted>
		Model = 0x110,
		/// <ToBeCompleted></ToBeCompleted>
		StripOffsets = 0x111,
		/// <ToBeCompleted></ToBeCompleted>
		Orientation = 0x112,
		/// <ToBeCompleted></ToBeCompleted>
		SamplesPerPixel = 0x115,
		/// <ToBeCompleted></ToBeCompleted>
		RowsPerStrip = 0x116,
		/// <ToBeCompleted></ToBeCompleted>
		StripByteCounts = 0x117,
		/// <ToBeCompleted></ToBeCompleted>
		XResolution = 0x11A,
		/// <ToBeCompleted></ToBeCompleted>
		YResolution = 0x11B,
		/// <ToBeCompleted></ToBeCompleted>
		PlanarConfiguration = 0x11C,
		/// <ToBeCompleted></ToBeCompleted>
		ResolutionUnit = 0x128,
		/// <ToBeCompleted></ToBeCompleted>
		TransferFunction = 0x12D,
		/// <ToBeCompleted></ToBeCompleted>
		Software = 0x131,
		/// <ToBeCompleted></ToBeCompleted>
		DateTime = 0x132,
		/// <ToBeCompleted></ToBeCompleted>
		Artist = 0x13B,
		/// <ToBeCompleted></ToBeCompleted>
		WhitePoint = 0x13E,
		/// <ToBeCompleted></ToBeCompleted>
		PrimaryChromaticities = 0x13F,
		/// <ToBeCompleted></ToBeCompleted>
		TransferRange = 0x156,
		/// <ToBeCompleted></ToBeCompleted>
		JPEGProc = 0x200,
		/// <ToBeCompleted></ToBeCompleted>
		JPEGInterchangeFormat = 0x201,
		/// <ToBeCompleted></ToBeCompleted>
		JPEGInterchangeFormatLength = 0x202,
		/// <ToBeCompleted></ToBeCompleted>
		YCbCrCoefficients = 0x211,
		/// <ToBeCompleted></ToBeCompleted>
		YCbCrSubSampling = 0x212,
		/// <ToBeCompleted></ToBeCompleted>
		YCbCrPositioning = 0x213,
		/// <ToBeCompleted></ToBeCompleted>
		ReferenceBlackWhite = 0x214,
		/// <ToBeCompleted></ToBeCompleted>
		BatteryLevel = 0x828F,
		/// <ToBeCompleted></ToBeCompleted>
		Copyright = 0x8298,
		/// <ToBeCompleted></ToBeCompleted>
		ExposureTime = 0x829A,
		/// <ToBeCompleted></ToBeCompleted>
		FNumber = 0x829D,
		/// <ToBeCompleted></ToBeCompleted>
		IPTC_NAA = 0x83BB,
		/// <ToBeCompleted></ToBeCompleted>
		ExifIFDPointer = 0x8769,
		/// <ToBeCompleted></ToBeCompleted>
		InterColorProfile = 0x8773,
		/// <ToBeCompleted></ToBeCompleted>
		ExposureProgram = 0x8822,
		/// <ToBeCompleted></ToBeCompleted>
		SpectralSensitivity = 0x8824,
		/// <ToBeCompleted></ToBeCompleted>
		GPSInfoIFDPointer = 0x8825,
		/// <ToBeCompleted></ToBeCompleted>
		ISOSpeedRatings = 0x8827,
		/// <ToBeCompleted></ToBeCompleted>
		OECF = 0x8828,
		/// <ToBeCompleted></ToBeCompleted>
		ExifVersion = 0x9000,
		/// <ToBeCompleted></ToBeCompleted>
		DateTimeOriginal = 0x9003,
		/// <ToBeCompleted></ToBeCompleted>
		DateTimeDigitized = 0x9004,
		/// <ToBeCompleted></ToBeCompleted>
		ComponentsConfiguration = 0x9101,
		/// <ToBeCompleted></ToBeCompleted>
		CompressedBitsPerPixel = 0x9102,
		/// <ToBeCompleted></ToBeCompleted>
		ShutterSpeedValue = 0x9201,
		/// <ToBeCompleted></ToBeCompleted>
		ApertureValue = 0x9202,
		/// <ToBeCompleted></ToBeCompleted>
		BrightnessValue = 0x9203,
		/// <ToBeCompleted></ToBeCompleted>
		ExposureBiasValue = 0x9204,
		/// <ToBeCompleted></ToBeCompleted>
		MaxApertureValue = 0x9205,
		/// <ToBeCompleted></ToBeCompleted>
		SubjectDistance = 0x9206,
		/// <ToBeCompleted></ToBeCompleted>
		MeteringMode = 0x9207,
		/// <ToBeCompleted></ToBeCompleted>
		LightSource = 0x9208,
		/// <ToBeCompleted></ToBeCompleted>
		Flash = 0x9209,
		/// <ToBeCompleted></ToBeCompleted>
		FocalLength = 0x920A,
		/// <ToBeCompleted></ToBeCompleted>
		SubjectArea = 0x9214,
		/// <ToBeCompleted></ToBeCompleted>
		MakerNote = 0x927C,
		/// <ToBeCompleted></ToBeCompleted>
		UserComment = 0x9286,
		/// <ToBeCompleted></ToBeCompleted>
		SubSecTime = 0x9290,
		/// <ToBeCompleted></ToBeCompleted>
		SubSecTimeOriginal = 0x9291,
		/// <ToBeCompleted></ToBeCompleted>
		SubSecTimeDigitized = 0x9292,
		/// <ToBeCompleted></ToBeCompleted>
		FlashPixVersion = 0xA000,
		/// <ToBeCompleted></ToBeCompleted>
		ColorSpace = 0xA001,
		/// <ToBeCompleted></ToBeCompleted>
		PixelXDimension = 0xA002,
		/// <ToBeCompleted></ToBeCompleted>
		PixelYDimension = 0xA003,
		/// <ToBeCompleted></ToBeCompleted>
		RelatedSoundFile = 0xA004,
		/// <ToBeCompleted></ToBeCompleted>
		InteroperabilityIFDPointer = 0xA005,
		/// <ToBeCompleted></ToBeCompleted>
		FlashEnergy = 0xA20B,
		/// <ToBeCompleted></ToBeCompleted>
		FlashEnergy_TIFF = 0x920B,
		/// <ToBeCompleted></ToBeCompleted>
		SpatialFrequencyResponse = 0xA20C,
		/// <ToBeCompleted></ToBeCompleted>
		SpatialFrequencyResponse_TIFF = 0x920C,
		/// <ToBeCompleted></ToBeCompleted>
		FocalPlaneXResolution = 0xA20E,
		/// <ToBeCompleted></ToBeCompleted>
		FocalPlaneXResolution_TIFF = 0x920E,
		/// <ToBeCompleted></ToBeCompleted>
		FocalPlaneYResolution = 0xA20F,
		/// <ToBeCompleted></ToBeCompleted>
		FocalPlaneYResolution_TIFF = 0x920F,
		/// <ToBeCompleted></ToBeCompleted>
		FocalPlaneResolutionUnit = 0xA210,
		/// <ToBeCompleted></ToBeCompleted>
		FocalPlaneResolutionUnit_TIFF = 0x9210,
		/// <ToBeCompleted></ToBeCompleted>
		SubjectLocation = 0xA214,
		/// <ToBeCompleted></ToBeCompleted>
		SubjectLocation_TIFF = 0x9214,
		/// <ToBeCompleted></ToBeCompleted>
		ExposureIndex = 0xA215,
		/// <ToBeCompleted></ToBeCompleted>
		ExposureIndex_TIFF = 0x9215,
		/// <ToBeCompleted></ToBeCompleted>
		SensingMethod = 0xA217,
		/// <ToBeCompleted></ToBeCompleted>
		SensingMethod_TIFF = 0x9217,
		/// <ToBeCompleted></ToBeCompleted>
		FileSource = 0xA300,
		/// <ToBeCompleted></ToBeCompleted>
		SceneType = 0xA301,
		/// <ToBeCompleted></ToBeCompleted>
		CFAPattern = 0xA302,
		/// <ToBeCompleted></ToBeCompleted>
		CFAPattern_TIFF = 0x828E,
		/// <ToBeCompleted></ToBeCompleted>
		CustomRendered = 0xA401,
		/// <ToBeCompleted></ToBeCompleted>
		ExposureMode = 0xA402,
		/// <ToBeCompleted></ToBeCompleted>
		WhiteBalance = 0xA403,
		/// <ToBeCompleted></ToBeCompleted>
		DigitalZoomRatio = 0xA404,
		/// <ToBeCompleted></ToBeCompleted>
		FocalLengthIn35mmFilm = 0xA405,
		/// <ToBeCompleted></ToBeCompleted>
		SceneCaptureType = 0xA406,
		/// <ToBeCompleted></ToBeCompleted>
		GainControl = 0xA407,
		/// <ToBeCompleted></ToBeCompleted>
		Contrast = 0xA408,
		/// <ToBeCompleted></ToBeCompleted>
		Saturation = 0xA409,
		/// <ToBeCompleted></ToBeCompleted>
		Sharpness = 0xA40A,
		/// <ToBeCompleted></ToBeCompleted>
		DeviceSettingDescription = 0xA40B,
		/// <ToBeCompleted></ToBeCompleted>
		SubjectDistanceRange = 0xA40C,
		/// <ToBeCompleted></ToBeCompleted>
		ImageUniqueID = 0xA420
	}


	/// <ToBeCompleted></ToBeCompleted>
	public enum ExifPropertyTypes {
		/// <ToBeCompleted></ToBeCompleted>
		Byte = 0x1,
		/// <ToBeCompleted></ToBeCompleted>
		ASCII = 0x2,
		/// <ToBeCompleted></ToBeCompleted>
		Short = 0x3,
		/// <ToBeCompleted></ToBeCompleted>
		Long = 0x4,
		/// <ToBeCompleted></ToBeCompleted>
		Rational = 0x5,
		/// <ToBeCompleted></ToBeCompleted>
		Undefined = 0x7,
		/// <ToBeCompleted></ToBeCompleted>
		SLONG = 0x9,
		/// <ToBeCompleted></ToBeCompleted>
		SRational = 0xA
	}


	/// <ToBeCompleted></ToBeCompleted>
	public enum ExifIntrTags {
		/// <ToBeCompleted></ToBeCompleted>
		InteroperabilityIndex = 0x0001,
		/// <ToBeCompleted></ToBeCompleted>
		InteroperabilityVersion = 0x0002,
		/// <ToBeCompleted></ToBeCompleted>
		RelatedImageFileFormat = 0x1000,
		/// <ToBeCompleted></ToBeCompleted>
		RelatedImageWidth = 0x1001,
		/// <ToBeCompleted></ToBeCompleted>
		RelatedImageLength = 0x1002,
	}


	/// <ToBeCompleted></ToBeCompleted>
	public enum ExifGpsTags {
		/// <ToBeCompleted></ToBeCompleted>
		GPSVersionID = 0x0,
		/// <ToBeCompleted></ToBeCompleted>
		GPSLatitudeRef = 0x1,
		/// <ToBeCompleted></ToBeCompleted>
		GPSLatitude = 0x2,
		/// <ToBeCompleted></ToBeCompleted>
		GPSLongitudeRef = 0x3,
		/// <ToBeCompleted></ToBeCompleted>
		GPSLongitude = 0x4,
		/// <ToBeCompleted></ToBeCompleted>
		GPSAltitudeRef = 0x5,
		/// <ToBeCompleted></ToBeCompleted>
		GPSAltitude = 0x6,
		/// <ToBeCompleted></ToBeCompleted>
		GPSTimeStamp = 0x7,
		/// <ToBeCompleted></ToBeCompleted>
		GPSSatellites = 0x8,
		/// <ToBeCompleted></ToBeCompleted>
		GPSStatus = 0x9,
		/// <ToBeCompleted></ToBeCompleted>
		GPSMeasureMode = 0xA,
		/// <ToBeCompleted></ToBeCompleted>
		GPSDOP = 0xB,
		/// <ToBeCompleted></ToBeCompleted>
		GPSSpeedRef = 0xC,
		/// <ToBeCompleted></ToBeCompleted>
		GPSSpeed = 0xD,
		/// <ToBeCompleted></ToBeCompleted>
		GPSTrackRef = 0xE,
		/// <ToBeCompleted></ToBeCompleted>
		GPSTrack = 0xF,
		/// <ToBeCompleted></ToBeCompleted>
		GPSImgDirectionRef = 0x10,
		/// <ToBeCompleted></ToBeCompleted>
		GPSImgDirection = 0x11,
		/// <ToBeCompleted></ToBeCompleted>
		GPSMapDatum = 0x12,
		/// <ToBeCompleted></ToBeCompleted>
		GPSDestLatitudeRef = 0x13,
		/// <ToBeCompleted></ToBeCompleted>
		GPSDestLatitude = 0x14,
		/// <ToBeCompleted></ToBeCompleted>
		GPSDestLongitudeRef = 0x15,
		/// <ToBeCompleted></ToBeCompleted>
		GPSDestLongitude = 0x16,
		/// <ToBeCompleted></ToBeCompleted>
		GPSDestBearingRef = 0x17,
		/// <ToBeCompleted></ToBeCompleted>
		GPSDestBearing = 0x18,
		/// <ToBeCompleted></ToBeCompleted>
		GPSDestDistanceRef = 0x19,
		/// <ToBeCompleted></ToBeCompleted>
		GPSDestDistance = 0x1A,
		/// <ToBeCompleted></ToBeCompleted>
		GPSProcessingMethod = 0x1B,
		/// <ToBeCompleted></ToBeCompleted>
		GPSAreaInformation = 0x1C,
		/// <ToBeCompleted></ToBeCompleted>
		GPSDateStamp = 0x1D,
		/// <ToBeCompleted></ToBeCompleted>
		GPSDifferential = 0x1E
	}

}
