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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;

using Dataweb.Utilities;


namespace Dataweb.NShape.Advanced {

	/// <summary>
	/// Displays a bitmap in the diagram.
	/// </summary>
	public class PictureBase : RectangleBase {

		#region [Public] Properties

		/// <ToBeCompleted></ToBeCompleted>
		[Category("Appearance")]
		[Description("The shape's image.")]
		[RequiredPermission(Permission.Present)]
		[Editor("Dataweb.NShape.WinFormsUI.NamedImageUITypeEditor, Dataweb.NShape.WinFormsUI", typeof(UITypeEditor))]
		public NamedImage Image {
			get { return image; }
			set {
				GdiHelpers.DisposeObject(ref brushImage);
				if (NamedImage.IsNullOrEmpty(value))
					image = null;
				else image = value;
				InvalidateDrawCache();
				Invalidate();
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Category("Appearance")]
		[Description("Defines the layout of the displayed image.")]
		[PropertyMappingId(PropertyIdImageLayout)]
		[RequiredPermission(Permission.Present)]
		public ImageLayoutMode ImageLayout {
			get { return imageLayout; }
			set {
				imageLayout = value;
				InvalidateDrawCache();
				Invalidate();
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Category("Appearance")]
		[Description("Displays the image as grayscale image.")]
		[PropertyMappingId(PropertyIdImageGrayScale)]
		[RequiredPermission(Permission.Present)]
		public bool GrayScale {
			get { return imageGrayScale; }
			set {
				imageGrayScale = value;
				InvalidateDrawCache();
				Invalidate();
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Category("Appearance")]
		[Description("Gamma correction value for the image.")]
		[PropertyMappingId(PropertyIdImageGamma)]
		[RequiredPermission(Permission.Present)]
		public float GammaCorrection {
			get { return imageGamma; }
			set {
				if (value <= 0) throw new ArgumentOutOfRangeException("Value has to be greater 0.");
				imageGamma = value;
				InvalidateDrawCache();
				Invalidate();
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Category("Appearance")]
		[Description("Transparency of the image in percentage.")]
		[PropertyMappingId(PropertyIdImageTransparency)]
		[RequiredPermission(Permission.Present)]
		public byte Transparency {
			get { return imageTransparency; }
			set {
				if (value < 0 || value > 100) throw new ArgumentOutOfRangeException("Value has to be between 0 and 100.");
				imageTransparency = value;
				InvalidateDrawCache();
				Invalidate();
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Category("Appearance")]
		[Description("Transparency of the image in percentage.")]
		[PropertyMappingId(PropertyIdImageTransparentColor)]
		[RequiredPermission(Permission.Present)]
		public Color TransparentColor {
			get { return transparentColor; }
			set {
				transparentColor = value;
				InvalidateDrawCache();
				Invalidate();
			}
		}

		#endregion


		#region [Public] Methods

		/// <ToBeCompleted></ToBeCompleted>
		public void FitShapeToImageSize() {
			if (image != null) {
				Width = image.Width + (Width - imageBounds.Width);
				Height = image.Height + (Height - imageBounds.Height);
			}
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new PictureBase(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override void CopyFrom(Shape source) {
			base.CopyFrom(source);
			if (source is PictureBase) {
				PictureBase src = (PictureBase)source;
				if (!NamedImage.IsNullOrEmpty(src.Image))
					Image = src.Image.Clone();
				imageGrayScale = src.GrayScale;
				imageLayout = src.ImageLayout;
				imageGamma = src.GammaCorrection;
				imageTransparency = src.Transparency;
				transparentColor = src.TransparentColor;
				compressionQuality = src.compressionQuality;
			}
		}


		/// <override></override>
		public override void MakePreview(IStyleSet styleSet) {
			base.MakePreview(styleSet);
			isPreview = true;
			GdiHelpers.DisposeObject(ref imageAttribs);
			GdiHelpers.DisposeObject(ref imageBrush);
			//if (!NamedImage.IsNullOrEmpty(image) && BrushImage != null)
			//   image.Image = BrushImage;
		}


		/// <override></override>
		public override ControlPointId HitTest(int x, int y, ControlPointCapabilities controlPointCapability, int range) {
			return base.HitTest(x, y, controlPointCapability, range);
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			//if (ImageLayout == ImageLayoutMode.Original) {
			//    if ((controlPointCapability & ControlPointCapabilities.Glue) != 0)
			//        return false;
			//    if ((controlPointCapability & ControlPointCapabilities.Connect) != 0 ) {
			//        return (controlPointId != RotateControlPointId && IsConnectionPointEnabled(controlPointId));
			//    }
			//    if ((controlPointCapability & ControlPointCapabilities.Reference) != 0) {
			//        if (controlPointId == RotateControlPointId || controlPointId == ControlPointId.Reference)
			//            return true;
			//    }
			//    if ((controlPointCapability & ControlPointCapabilities.Rotate) != 0) {
			//        if (controlPointId == RotateControlPointId)
			//            return true;
			//    }
				return base.HasControlPointCapability(controlPointId, controlPointCapability);
			//} else return base.HasControlPointCapability(controlPointId, controlPointCapability);
		}


		/// <override></override>
		public override void Draw(Graphics graphics) {
			if (graphics == null) throw new ArgumentNullException("graphics");
			UpdateDrawCache();
			Pen pen = ToolCache.GetPen(LineStyle, null, null);
			Brush brush = ToolCache.GetTransformedBrush(FillStyle, boundingRectangleUnrotated, Center, Angle);
			//Brush brush = ToolCache.GetTransformedBrush(FillStyle, GetBoundingRectangle(true), Center, Angle);
			graphics.FillPath(brush, Path);

			Debug.Assert(imageAttribs != null);
			Debug.Assert(Geometry.IsValid(imageBounds));
			if (image != null && image.Image is Metafile) {
				GdiHelpers.DrawImage(graphics, image.Image, imageAttribs, imageLayout, imageBounds, imageBounds, Geometry.TenthsOfDegreeToDegrees(Angle), Center);
			} else {
				Debug.Assert(imageBrush != null);
				graphics.FillPolygon(imageBrush, imageDrawBounds);
			}
			DrawCaption(graphics);
			graphics.DrawPath(pen, Path);
			if (Children.Count > 0) foreach (Shape s in Children) s.Draw(graphics);
		}

		#endregion


		#region IEntity Members

		/// <override></override>
		protected override void LoadFieldsCore(IRepositoryReader reader, int version) {
			base.LoadFieldsCore(reader, version);
			imageLayout = (ImageLayoutMode)reader.ReadByte();
			imageTransparency = reader.ReadByte();
			imageGamma = reader.ReadFloat();
			compressionQuality = reader.ReadByte();
			imageGrayScale = reader.ReadBool();
			string name = reader.ReadString();
			Image img = reader.ReadImage();
			if (name != null && img != null)
				image = new NamedImage(img, name);
			transparentColor = Color.FromArgb(reader.ReadInt32());
		}


		/// <override></override>
		protected override void SaveFieldsCore(IRepositoryWriter writer, int version) {
			base.SaveFieldsCore(writer, version);
			writer.WriteByte((byte)imageLayout);
			writer.WriteByte(imageTransparency);
			writer.WriteFloat(imageGamma);
			writer.WriteByte(compressionQuality);
			writer.WriteBool(imageGrayScale);
			if (NamedImage.IsNullOrEmpty(image)) {
				writer.WriteString(string.Empty);
				writer.WriteImage(null);
			} else {
				writer.WriteString(image.Name);
				object imgTag = image.Image.Tag;
				image.Image.Tag = image.Name;
				writer.WriteImage(image.Image);
				image.Image.Tag = imgTag;
			}
			writer.WriteInt32(transparentColor.ToArgb());
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.PictureBase" />.
		/// </summary>
		new public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version) {
			foreach (EntityPropertyDefinition pi in RectangleBase.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("ImageLayout", typeof(byte));
			yield return new EntityFieldDefinition("ImageTransparency", typeof(byte));
			yield return new EntityFieldDefinition("ImageGammaCorrection", typeof(float));
			yield return new EntityFieldDefinition("ImageCompressionQuality", typeof(byte));
			yield return new EntityFieldDefinition("ConvertToGrayScale", typeof(bool));
			yield return new EntityFieldDefinition("ImageFileName", typeof(string));
			yield return new EntityFieldDefinition("Image", typeof(Image));
			yield return new EntityFieldDefinition("ImageTransparentColor", typeof(int));
		}

		#endregion


		#region [Protected] Overridden Methods

		/// <ToBeCompleted></ToBeCompleted>
		protected internal PictureBase(ShapeType shapeType, Template template)
			: base(shapeType, template) {
			Construct();
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal PictureBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
			Construct();
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			Size txtSize = Size.Empty;
			txtSize.Width = Width;
			txtSize.Height = Height;
			string txt = string.IsNullOrEmpty(Text) ? "Ip" : Text;
			if (DisplayService != null)
				txtSize = TextMeasurer.MeasureText(DisplayService.InfoGraphics, txt, CharacterStyle, txtSize, ParagraphStyle);
			else txtSize = TextMeasurer.MeasureText(txt, CharacterStyle, txtSize, ParagraphStyle);

			captionBounds = Rectangle.Empty;
			captionBounds.Width = Width;
			captionBounds.Height = Math.Min(Height, txtSize.Height);
			captionBounds.X = (int)Math.Round(-(Width / 2f));
			captionBounds.Y = (int)Math.Round((Height / 2f) - captionBounds.Height);
		}


		/// <override></override>
		protected override void InvalidateDrawCache() {
			base.InvalidateDrawCache();
			GdiHelpers.DisposeObject(ref imageAttribs);
			GdiHelpers.DisposeObject(ref imageBrush);
			imageBounds = Geometry.InvalidRectangle;
		}


		/// <override></override>
		protected override void RecalcDrawCache() {
			base.RecalcDrawCache();
			imageBounds.Width = Width;
			imageBounds.Height = Height;
			imageBounds.X = (int)Math.Round(-Width / 2f);
			imageBounds.Y = (int)Math.Round(-Height / 2f);
			if (!string.IsNullOrEmpty(Text)) {
				Rectangle r;
				CalcCaptionBounds(0, out r);
				imageBounds.Height -= r.Height;
			}
			imageDrawBounds[0].X = imageBounds.Left; imageDrawBounds[0].Y = imageBounds.Top;
			imageDrawBounds[1].X = imageBounds.Right; imageDrawBounds[1].Y = imageBounds.Top;
			imageDrawBounds[2].X = imageBounds.Right; imageDrawBounds[2].Y = imageBounds.Bottom;
			imageDrawBounds[3].X = imageBounds.Left; imageDrawBounds[3].Y = imageBounds.Bottom;

			if (imageAttribs == null)
				imageAttribs = GdiHelpers.GetImageAttributes(imageLayout, imageGamma, imageTransparency, imageGrayScale, isPreview, transparentColor);

			Image bitmapImg = null;
			if (image == null || image.Image == null)
				bitmapImg = Properties.Resources.DefaultBitmapLarge;
			else if (image.Image is Bitmap)
				bitmapImg = Image.Image;
			if (bitmapImg is Bitmap && imageBrush == null) {
				if (isPreview)
					imageBrush = GdiHelpers.CreateTextureBrush(bitmapImg, Width, Height, imageAttribs);
				else imageBrush = GdiHelpers.CreateTextureBrush(bitmapImg, imageAttribs);
				// Transform texture Brush
				Point imageCenter = Point.Empty;
				imageCenter.Offset((int)Math.Round(imageBounds.Width / 2f), (int)Math.Round(imageBounds.Height / 2f));
				if (Angle != 0) imageCenter = Geometry.RotatePoint(Point.Empty, Geometry.TenthsOfDegreeToDegrees(Angle), imageCenter);
				GdiHelpers.TransformTextureBrush(imageBrush, imageLayout, imageBounds, imageCenter, Geometry.TenthsOfDegreeToDegrees(Angle));
			}
		}


		/// <override></override>
		protected override void TransformDrawCache(int deltaX, int deltaY, int deltaAngle, int rotationCenterX, int rotationCenterY) {
			base.TransformDrawCache(deltaX, deltaY, deltaAngle, rotationCenterX, rotationCenterY);
			if (Geometry.IsValid(imageBounds)) {
				imageBounds.Offset(deltaX, deltaY);
				Matrix.TransformPoints(imageDrawBounds);
				if (imageBrush != null) {
					float angleDeg = Geometry.TenthsOfDegreeToDegrees(Angle);
					Point p = Point.Empty;
					p.X = (int)Math.Round(imageBounds.X + (imageBounds.Width / 2f));
					p.Y = (int)Math.Round(imageBounds.Y + (imageBounds.Height / 2f));
					p = Geometry.RotatePoint(Center, angleDeg, p);
					GdiHelpers.TransformTextureBrush(imageBrush, imageLayout, imageBounds, p, angleDeg);
				}
			}
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);

				Rectangle shapeRect = Rectangle.Empty;
				shapeRect.Offset(left, top);
				shapeRect.Width = Width;
				shapeRect.Height = Height;

				Path.Reset();
				Path.StartFigure();
				Path.AddRectangle(shapeRect);
				Path.CloseFigure();
				return true;
			} else return false;
		}

		#endregion

		/// <override></override>
		protected override void ProcessExecModelPropertyChange(IModelMapping propertyMapping) {
			switch (propertyMapping.ShapePropertyId) {
				//case PropertyIdImage: break;
				case PropertyIdImageLayout:
					ImageLayout = (ImageLayoutMode)propertyMapping.GetInteger();
					break;
				case PropertyIdImageGrayScale:
					GrayScale = (propertyMapping.GetInteger() != 0);
					break;
				case PropertyIdImageGamma:
					//GammaCorrection = Math.Max(0.00000001f, Math.Abs(propertyMapping.GetFloat()));
					GammaCorrection = propertyMapping.GetFloat();
					break;
				case PropertyIdImageTransparency:
					checked {
						Transparency = (byte)propertyMapping.GetInteger();
					}
					break;
				case PropertyIdImageTransparentColor:
					TransparentColor = Color.FromArgb(propertyMapping.GetInteger());
					break;
				default:
					base.ProcessExecModelPropertyChange(propertyMapping);
					break;
			}
		}


		//private Image BrushImage {
		//   get {
		//      if (brushImage == null
		//         && !NamedImage.IsNullOrEmpty(image)
		//         && (image.Width >= 2 * Width || image.Height >= 2 * Height))
		//            brushImage = GdiHelpers.GetBrushImage(image.Image, Width, Height);
		//      return brushImage;
		//   }
		//}


		private void Construct() {
			// this fillStyle holds the image of the shape
			image = null;
			imageGrayScale = false;
			compressionQuality = 100;
			imageGamma = 1;
			imageLayout = ImageLayoutMode.Fit;
			imageTransparency = 0;
		}


		#region Fields

		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdImage = 9;
		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdImageLayout = 10;
		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdImageGrayScale = 11;
		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdImageGamma = 12;
		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdImageTransparency = 13;
		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdImageTransparentColor = 14;

		private const int RotateControlPointId = 9;

		private ImageAttributes imageAttribs = null;
		private TextureBrush imageBrush = null;
		private Rectangle imageBounds = Geometry.InvalidRectangle;
		private Point[] imageDrawBounds = new Point[4];
		private bool isPreview = false;
		private Size fullImageSize = Size.Empty;
		private Size currentImageSize = Size.Empty;
		private Image brushImage;

		private NamedImage image;
		private ImageLayoutMode imageLayout = ImageLayoutMode.Fit;
		private byte imageTransparency = 0;
		private float imageGamma = 1.0f;
		private bool imageGrayScale = false;
		private byte compressionQuality = 100;
		private Color transparentColor = Color.Empty;
		#endregion
	}


	/// <summary>
	/// Abstract base class for shapes that draw themselves using a bitmap or
	/// meta file.
	/// </summary>
	/// <remarks>RequiredPermissions set</remarks>
	public class ImageBasedShape : ShapeBase, IPlanarShape, ICaptionedShape {

		/// <ToBeCompleted></ToBeCompleted>
		protected internal ImageBasedShape(ShapeType shapeType, Template template,
			string resourceBaseName, Assembly resourceAssembly)
			: base(shapeType, template) {
			Construct(resourceBaseName, resourceAssembly);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal ImageBasedShape(ShapeType shapeType, IStyleSet styleSet,
			string resourceBaseName, Assembly resourceAssembly)
			: base(shapeType, styleSet) {
			Construct(resourceBaseName, resourceAssembly);
		}


		/// <override></override>
		protected internal override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			w = image.Width;
			h = image.Height;
			Fit(0, 0, 100, 100);
			charStyle = styleSet.CharacterStyles.Normal;
			paragraphStyle = styleSet.ParagraphStyles.Title;
			fillStyle = styleSet.FillStyles.Transparent;
		}


		/// <override></override>
		public override void CopyFrom(Shape source) {
			base.CopyFrom(source);

			if (source is IPlanarShape) {
				IPlanarShape src = (IPlanarShape)source;
				// Copy regular properties
				this.angle = src.Angle;
				// Copy templated properties
				this.fillStyle = (Template != null && src.FillStyle == ((IPlanarShape)Template.Shape).FillStyle) ? null : src.FillStyle;
			}
			if (source is ICaptionedShape) {
				// Copy as many captions as possible. Leave the rest untouched.
				int ownCaptionCnt = CaptionCount;
				int srcCaptionCnt = ((ICaptionedShape)source).CaptionCount;
				int cnt = Math.Min(ownCaptionCnt, srcCaptionCnt);
				for (int i = 0; i < cnt; ++i) {
					this.SetCaptionText(i, ((ICaptionedShape)source).GetCaptionText(i));
					this.SetCaptionCharacterStyle(i, ((ICaptionedShape)source).GetCaptionCharacterStyle(i));
					this.SetCaptionParagraphStyle(i, ((ICaptionedShape)source).GetCaptionParagraphStyle(i));
				}
			}
			if (source is ImageBasedShape) {
				w = ((ImageBasedShape)source).w;
				h = ((ImageBasedShape)source).h;
				if (((ImageBasedShape)source).image != null)
					image = (Image)((ImageBasedShape)source).image.Clone();
			} else {
				Rectangle r = source.GetBoundingRectangle(true);
				Fit(r.X, r.Y, r.Width, r.Height);
			}
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new ImageBasedShape(Type, (Template)null, resourceName, resourceAssembly);
			result.CopyFrom(this);
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Category("Data")]
		[Description("Text displayed inside the shape")]
		[PropertyMappingId(PropertyIdText)]
		[RequiredPermission(Permission.Data)]
		[Editor("Dataweb.NShape.WinFormsUI.TextUITypeEditor, Dataweb.NShape.WinFormsUI", typeof(UITypeEditor))]
		public string Text {
			get { return caption.Text; }
			set { caption.Text = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Category("Appearance")]
		[Description("Determines the style of the shape's text.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles.")]
		[PropertyMappingId(PropertyIdCharacterStyle)]
		[RequiredPermission(Permission.Present)]
		public ICharacterStyle CharacterStyle {
			get { return charStyle ?? ((ICaptionedShape)Template.Shape).GetCaptionCharacterStyle(0); }
			set {
				charStyle = (Template != null && value == ((ICaptionedShape)Template.Shape).GetCaptionCharacterStyle(0)) ? null : value;
				caption.InvalidatePath();
				Invalidate();
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Category("Appearance")]
		[Description("Determines the layout of the shape's text.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles.")]
		[RequiredPermission(Permission.Present)]
		[PropertyMappingId(PropertyIdParagraphStyle)]
		public IParagraphStyle ParagraphStyle {
			get { return paragraphStyle ?? ((ICaptionedShape)Template.Shape).GetCaptionParagraphStyle(0); }
			set {
				paragraphStyle = (Template != null && value == ((ICaptionedShape)Template.Shape).GetCaptionParagraphStyle(0)) ? null : value;
				caption.InvalidatePath();
				Invalidate();
			}
		}


		/// <override></override>
		public override void MakePreview(IStyleSet styleSet) {
			base.MakePreview(styleSet);
			charStyle = styleSet.GetPreviewStyle(CharacterStyle);
			paragraphStyle = styleSet.GetPreviewStyle(ParagraphStyle);
			fillStyle = styleSet.GetPreviewStyle(FillStyle);
		}


		/// <override></override>
		public override bool HasStyle(IStyle style) {
			if (IsStyleAffected(CharacterStyle, style) || IsStyleAffected(ParagraphStyle, style))
				return true;
			else return base.HasStyle(style);
		}


		/// <override></override>
		public override RelativePosition CalculateRelativePosition(int x, int y) {
			if (!Geometry.IsValid(x, y)) throw new ArgumentOutOfRangeException("x / y");
			RelativePosition result = RelativePosition.Empty;
			result.A = 0;
			result.B = 0;
			return result;
		}


		/// <override></override>
		public override Point CalculateAbsolutePosition(RelativePosition relativePosition) {
			if (relativePosition == RelativePosition.Empty) throw new ArgumentOutOfRangeException("relativePosition");
			Point result = Point.Empty;
			result.X = x;
			result.Y = y;
			return result;
		}


		/// <override></override>
		public override Point CalculateNormalVector(int x, int y) {
			if (!ContainsPoint(x, y)) throw new NShapeException("Coordinates {0} are outside {1}.", new Point(x, y), Type.FullName);
			return Geometry.CalcNormalVectorOfRectangle(x - (w / 2), y - (h / 2), x + (w / 2), y + (h / 2), x, y, 100);
		}


		/// <override></override>
		protected override Rectangle CalculateBoundingRectangle(bool tight) {
			// All controlPoints of the imagebased shape are inside the shape's bounds
			// so the tight fitting bounding rectangle equals the loose bounding rectangle
			Rectangle result = Rectangle.Empty;
			result.X = x - w / 2;
			result.Y = y - h / 2;
			result.Width = w;
			result.Height = h;
			return result;
		}


		/// <override></override>
		public override int X {
			get { return x; }
			set {
				int origValue = x;
				if (!MoveTo(value, Y)) {
					MoveTo(origValue, Y);
					throw new InvalidOperationException(string.Format("Shape cannot move to {0}", new Point(value, Y)));
				}
			}
		}


		/// <override></override>
		public override int Y {
			get { return y; }
			set {
				int origValue = y;
				if (!MoveTo(X, value)) {
					MoveTo(X, origValue);
					throw new InvalidOperationException(string.Format("Shape cannot move to {0}", new Point(X, value)));
				}
			}
		}

		
		///// <override></override>
		//public override bool MoveByCore(int deltaX, int deltaY) {
		//    return true;
		//}


		/// <override></override>
		public override void Fit(int x, int y, int width, int height) {
			if (height - ch < minH) ch = 0;
			if (width * image.Height <= (height - ch) * image.Width) {
				// Höhe ist verhältnismäßig größer
				this.h = height;
				if (this.h < ch + minH) this.w = this.h * image.Width / image.Height;
				else this.w = (this.h - ch) * image.Width / image.Height;
			} else {
				this.w = width;
				this.h = (this.w * image.Height) / image.Width + ch;
			}
			this.x = (x + width) / 2;
			this.y = (y + height) / 2;
			captionUpdated = false;
			InvalidateDrawCache();
		}


		/// <override></override>
		public override Point CalculateConnectionFoot(int fromX, int fromY) {
			Point result = Geometry.InvalidPoint;
			if (fromX != x || fromY != y)
				result = Geometry.IntersectLineWithRectangle(fromX, fromY, x, y, x - w / 2, y - h / 2, x - w / 2 + w, y - h / 2 + h);
			if (!Geometry.IsValid(result)) {
				result.X = x; result.Y = y;
			}
			return result;
		}


		/// <override></override>
		protected internal override int ControlPointCount {
			get { return 4; }
		}


		/// <override></override>
		public override Point GetControlPointPosition(ControlPointId controlPointId) {
			Point result = Point.Empty;
			switch (controlPointId) {
				case 1:
					// Links oben
					result.X = x - w / 2;
					result.Y = y - h / 2;
					break;
				case ControlPointId.Reference:
				case 2:
					// Referenzpunkt
					result.X = x;
					result.Y = y;
					break;
				case 3:
				    result.X = x;
				    result.Y = y - h / 2 + h - ch;
				    break;
				case 8:
					result.X = x;
					result.Y = y - h / 2 + h;
					break;
				default:
					Debug.Fail("Unsupported control point id");
					break;
			}
			return result;
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			bool result;
			switch (controlPointId) {
				case 1:
					result = (controlPointCapability & ControlPointCapabilities.Resize) != 0;
					break;
				case ControlPointId.Reference:
				case 2:
					result = (controlPointCapability & (ControlPointCapabilities.Reference | ControlPointCapabilities.Connect)) != 0;
					break;
				case 3:
					result = (controlPointCapability & ControlPointCapabilities.Connect) != 0 && IsConnectionPointEnabled(controlPointId);
					break;
				case 8:
					result = (controlPointCapability & ControlPointCapabilities.Resize) != 0;
					break;
				default:
					result = base.HasControlPointCapability(controlPointId, controlPointCapability);
					break;
			}
			return result;
		}


		/// <override></override>
		public override void Draw(Graphics graphics) {
			if (graphics == null) throw new ArgumentNullException("graphics");
			UpdateDrawCache();
			if (h >= ch + minH) {
				graphics.DrawImage(image, x - w / 2, Y - w / 2, w, h - ch);
				caption.Draw(graphics, CharacterStyle, ParagraphStyle);
			} else
				graphics.DrawImage(image, x - w / 2, Y - w / 2, w, h);
			base.Draw(graphics);
		}


		/// <override></override>
		public override void DrawOutline(Graphics graphics, Pen pen) {
			if (graphics == null) throw new ArgumentNullException("graphics");
			if (pen == null) throw new ArgumentNullException("pen");
			UpdateDrawCache();
			base.DrawOutline(graphics, pen);
			graphics.DrawRectangle(pen, x - w / 2, y - h / 2, w, h);
		}


		/// <override></override>
		public override void Invalidate() {
			if (DisplayService != null)
				DisplayService.Invalidate(x - w / 2, y - h / 2, w, h);
			base.Invalidate();
		}


		#region IPlanarShape Members

		/// <ToBeCompleted></ToBeCompleted>
		[Category("Layout")]
		[Description("Rotation angle of the Shape in tenths of degree.")]
		[PropertyMappingId(PropertyIdAngle)]
		[RequiredPermission(Permission.Layout)]
		public int Angle {
			get { return angle; }
			set { angle = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Category("Appearance")]
		[Description("Defines the appearence of the shape's interior.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles.")]
		[PropertyMappingId(PropertyIdFillStyle)]
		[RequiredPermission(Permission.Present)]
		public virtual IFillStyle FillStyle {
			get { return fillStyle ?? ((IPlanarShape)Template.Shape).FillStyle; }
			set {
				fillStyle = (Template != null && value == ((IPlanarShape)Template.Shape).FillStyle) ? null : value;
				//caption.InvalidatePath();
				Invalidate();
			}
		}

		#endregion


		#region ICaptionedShape Members

		/// <ToBeCompleted></ToBeCompleted>
		public int CaptionCount {
			get { return 1; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool GetCaptionTextBounds(int index, out Point topLeft, out Point topRight, out Point bottomRight, out Point bottomLeft) {
			if (index != 0) throw new IndexOutOfRangeException("index");
			Point location = Point.Empty;
			location.Offset(X, Y);
			Rectangle captionBounds = Rectangle.Empty;
			captionBounds.X = -w / 2;
			captionBounds.Y = -h / 2 + h - ch;
			captionBounds.Width = w;
			captionBounds.Height = ch;
			captionBounds = caption.CalculateTextBounds(captionBounds, CharacterStyle, ParagraphStyle, DisplayService);
			Geometry.TransformRectangle(location, Angle, captionBounds, out topLeft, out topRight, out bottomRight, out bottomLeft);
			return true;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool GetCaptionBounds(int index, out Point topLeft, out Point topRight, out Point bottomRight, out Point bottomLeft) {
			if (index != 0) throw new IndexOutOfRangeException("index");
			Point location = Point.Empty;
			location.Offset(X, Y);
			Rectangle captionBounds = Rectangle.Empty;
			captionBounds.X = -w / 2;
			captionBounds.Y = -h / 2 + h - ch;
			captionBounds.Width = w;
			captionBounds.Height = ch;
			Geometry.TransformRectangle(location, Angle, captionBounds, out topLeft, out topRight, out bottomRight, out bottomLeft);
			return true;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Rectangle GetCaptionTextBounds(int index) {
			if (index != 0) throw new IndexOutOfRangeException("index");
			Rectangle captionBounds = Rectangle.Empty;
			captionBounds.X = -w / 2;
			captionBounds.Y = -h / 2 + h - ch;
			captionBounds.Width = w;
			captionBounds.Height = ch;
			return caption.CalculateTextBounds(captionBounds, CharacterStyle, ParagraphStyle, DisplayService);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Rectangle GetCaptionBounds(int index) {
			Rectangle result = Rectangle.Empty;
			result.X = -w / 2;
			result.Y = -h / 2 + h - ch;
			result.Width = w;
			result.Height = ch;
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public string GetCaptionText(int index) {
			return caption.Text;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ICharacterStyle GetCaptionCharacterStyle(int index) {
			return CharacterStyle;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public IParagraphStyle GetCaptionParagraphStyle(int index) {
			return ParagraphStyle;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void SetCaptionText(int index, string text) {
			caption.Text = text;
			this.captionUpdated = false;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void SetCaptionCharacterStyle(int index, ICharacterStyle characterStyle) {
			CharacterStyle = characterStyle;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void SetCaptionParagraphStyle(int index, IParagraphStyle paragraphStyle) {
			ParagraphStyle = paragraphStyle;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public int FindCaptionFromPoint(int x, int y) {
			return Geometry.RectangleContainsPoint(this.x - this.w / 2, this.y - this.h / 2 + this.h - this.ch, this.w, this.ch, x, y) ? 0 : -1;
		}

		#endregion


		#region IEntity Members

		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.ImageBasedShape" />.
		/// </summary>
		new public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version) {
			foreach (EntityPropertyDefinition epd in ShapeBase.GetPropertyDefinitions(version))
				yield return epd;
			yield return new EntityFieldDefinition("FillStyle", typeof(object));
			yield return new EntityFieldDefinition("CharacterStyle", typeof(object));
			yield return new EntityFieldDefinition("ParagraphStyle", typeof(object));
			yield return new EntityFieldDefinition("Text", typeof(string));
			yield return new EntityFieldDefinition("Width", typeof(int));
			yield return new EntityFieldDefinition("Height", typeof(int));
		}


		/// <override></override>
		protected override void LoadFieldsCore(IRepositoryReader reader, int version) {
			base.LoadFieldsCore(reader, version);
			fillStyle = reader.ReadFillStyle();
			charStyle = reader.ReadCharacterStyle();
			paragraphStyle = reader.ReadParagraphStyle();

			string txt = reader.ReadString();
			if (caption == null) caption = new Caption(txt);
			else caption.Text = txt;
			w = reader.ReadInt32();
			h = reader.ReadInt32();
		}


		/// <override></override>
		protected override void SaveFieldsCore(IRepositoryWriter writer, int version) {
			base.SaveFieldsCore(writer, version);
			writer.WriteStyle(fillStyle);
			writer.WriteStyle(charStyle);
			writer.WriteStyle(ParagraphStyle);

			writer.WriteString(Text);
			writer.WriteInt32(w);
			writer.WriteInt32(h);
		}


		/// <override></override>
		protected override void DeleteCore(IRepositoryWriter writer, int version) {
			// Nothing to do
		}

		#endregion


		/// <override></override>
		protected override ControlPointId GetControlPointId(int index) {
			switch (index) {
				case 0: return 1;
				case 1: return 2;
				case 2: return 3;
				case 3: return 8;
				default: throw new NShapeException("NotSupported control point index.");
			}
		}


		/// <override></override>
		protected override bool ContainsPointCore(int x, int y) {
			return Geometry.RectangleContainsPoint(this.x - this.w / 2, this.y - this.h / 2, this.w, this.h, x, y);
		}


		/// <override></override>
		protected override bool IntersectsWithCore(int x, int y, int width, int height) {
			bool result = Geometry.RectangleIntersectsWithRectangle(this.x - this.w / 2, this.y - this.h / 2, this.w, this.h, x, y, width, height);
			return result;
		}


		/// <override></override>
		protected override bool MoveByCore(int deltaX, int deltaY) {
			//base.MoveByCore(deltaX, deltaY);
			this.x += deltaX;
			this.y += deltaY;
			transformation.Reset();
			transformation.Translate(deltaX, deltaY);
			caption.TransformPath(transformation);
			return true;
		}


		/// <override></override>
		protected override bool MovePointByCore(ControlPointId pointId, int deltaX, int deltaY, ResizeModifiers modifiers) {
			bool result;
			if (pointId == ControlPointId.Reference || pointId == 2) result = MoveByCore(deltaX, deltaY);
			else {
				int oldX = x, oldY = y, oldW = w, oldH = h;
				int newX, newY, newW, newH;
				switch (pointId) {
					case 1:
						newX = x;
						newY = y;
						newW = oldW - deltaX;
						newH = oldH - deltaY;
						if (newW <= newH) newH = int.MaxValue;
						else newW = int.MaxValue;

						if (newW < minW) { newW = minW; newH = int.MaxValue; } else if (newH < minH) { newH = minH; newW = int.MaxValue; }
						if (newW == int.MaxValue) newW = newH * image.Width / image.Height;
						if (newH == int.MaxValue) newH = newW * image.Height / image.Width + ch;

						result = false;
						break;
					case 8:
						newH = oldH + deltaY;
						if (newH < minH) newH = minH;
						newW = (newH - ch) * image.Width / image.Height;
						if (newW < minW) {
							newW = minW;
							newH = newW * image.Height / image.Width + ch;
						}
						newX = oldX;
						newY = oldY - oldH / 2 + newH / 2;
						result = false;
						break;
					default:
						newW = w; newH = h; newX = x; newY = y;
						result = true;
						break;
				}
				x = newX; y = newY; w = newW; h = newH;
				captionUpdated = false;
			}
			return result;
		}


		/// <override></override>
		protected override bool RotateCore(int angle, int x, int y) {
			if (x != X || y != Y) {
				int toX = X;
				int toY = Y;
				Geometry.RotatePoint(x, y, angle, ref toX, ref toY);
				MoveTo(toX, toY);
			}
			return false;
		}


		/// <override></override>
		protected override void InvalidateDrawCache() {
			base.InvalidateDrawCache();
			caption.InvalidatePath();
		}


		/// <override></override>
		protected override void RecalcDrawCache() {
			UpdateDrawCache();
		}


		/// <override></override>
		protected override void UpdateDrawCache() {
			if (drawCacheIsInvalid) {
				ch = CharacterStyle.Size + 4;
				caption.CalculatePath(-w / 2, -h / 2 + -ch, w, ch, CharacterStyle, ParagraphStyle);
				TransformDrawCache(x, y + h, 0, x, y);
				captionUpdated = true;
			}
		}


		/// <override></override>
		protected override void TransformDrawCache(int deltaX, int deltaY, int deltaAngle, int rotationCenterX, int rotationCenterY) {
			transformation.Reset();
			transformation.Translate(deltaX, deltaY);
			caption.TransformPath(transformation);
		}


		private void Construct(string resourceBaseName, Assembly resourceAssembly) {
			if (resourceBaseName == null) throw new ArgumentNullException("resourceBaseName");
			System.IO.Stream stream = resourceAssembly.GetManifestResourceStream(resourceBaseName);
			if (stream == null) throw new ArgumentException(string.Format("'{0}' is not a valid resource in '{1}'.", resourceBaseName, resourceAssembly), "resourceBaseName");
			image = Image.FromStream(stream);
			if (image == null) throw new ArgumentException(string.Format("'{0}' is not a valid image resource.", resourceBaseName), "resourceBaseName");
			this.resourceName = resourceBaseName;
			this.resourceAssembly = resourceAssembly;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdAngle = 2;
		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdFillStyle = 3;
		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdText = 4;
		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdCharacterStyle = 5;
		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdParagraphStyle = 6;

		/// <ToBeCompleted></ToBeCompleted>
		protected const int minW = 10;
		/// <ToBeCompleted></ToBeCompleted>
		protected const int minH = 10;

		/// <ToBeCompleted></ToBeCompleted>
		protected int x, y; // Position of reference point in the center
		/// <ToBeCompleted></ToBeCompleted>
		protected int w, h; // Size of shape
		/// <ToBeCompleted></ToBeCompleted>
		protected int angle;
		/// <ToBeCompleted></ToBeCompleted>
		protected Image image;
		//
		// these are needed for calling the constructor when cloning
		/// <ToBeCompleted></ToBeCompleted>
		protected string resourceName;
		/// <ToBeCompleted></ToBeCompleted>
		protected Assembly resourceAssembly;
		//
		// Caption fields
		/// <ToBeCompleted></ToBeCompleted>
		protected bool captionUpdated = false;
		/// <ToBeCompleted></ToBeCompleted>
		protected int ch; // Height of the caption
		/// <ToBeCompleted></ToBeCompleted>
		protected Caption caption = new Caption("Test");

		private ICharacterStyle charStyle;
		private IParagraphStyle paragraphStyle;
		private IFillStyle fillStyle;
		private Matrix transformation = new Matrix();
	}


	/// <summary>
	/// Provides a metafile whose color and line color can be customized.
	/// </summary>
	/// <remarks>RequiredPermissions set</remarks>
	public class CustomizableMetaFile : ImageBasedShape {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new CustomizableMetaFile(Type, (Template)null, resourceName, resourceAssembly);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override void CopyFrom(Shape source) {
			base.CopyFrom(source);
			if (source is CustomizableMetaFile) {
				ReplaceColor = ((CustomizableMetaFile)source).ReplaceColor;
			}
		}


		/// <override></override>
		public override void Draw(Graphics graphics) {
			if (graphics == null) throw new ArgumentNullException("graphics");
			if (!captionUpdated) UpdateDrawCache();
			replaceColorFound = false;
			Rectangle dstBounds = Rectangle.Empty;
			dstBounds.Offset((x - w / 2), (y - h / 2));
			dstBounds.Width = w;
			dstBounds.Height = h - ch;

			//// Workaround: Create a buffer image to draw
			//if (bufferImage == null) bufferImage = CreateImage();
			//MetafileHeader header = bufferImage.GetMetafileHeader();
			//graphics.DrawImage(bufferImage, dstBounds, header.Bounds.X, header.Bounds.Y, header.Bounds.Width, header.Bounds.Height, GraphicsUnit.Pixel, imageAttribs);
			if (imageAttribs == null) 
				UpdateImageAttributes();
			MetafileHeader header = ((Metafile)image).GetMetafileHeader();
			//graphics.DrawImage(((Metafile)image), dstBounds, header.Bounds.X, header.Bounds.Y, header.Bounds.Width, header.Bounds.Height, GraphicsUnit.Pixel, imageAttribs);
			GdiHelpers.DrawImage(graphics, image, imageAttribs, ImageLayoutMode.Fit, dstBounds, header.Bounds);
			
			if (caption != null) caption.Draw(graphics, CharacterStyle, ParagraphStyle);
		}


		/// <override></override>
		public override void MakePreview(IStyleSet styleSet) {
			base.MakePreview(styleSet);
			isPreview = true;
			GdiHelpers.DisposeObject(ref imageAttribs);
			UpdateImageAttributes();
		}


		/// <override></override>
		public override bool HasStyle(IStyle style) {
			if (IsStyleAffected(FillStyle, style))
				return true;
			else return base.HasStyle(style);
		}


		/// <override></override>
		public override IFillStyle FillStyle {
			get { return base.FillStyle; }
			set {
				replaceColorFound = false;
				base.FillStyle = value;
				UpdateImageAttributes();
				Invalidate();
			}
		}


		/// <override></override>
		protected override void UpdateDrawCache() {
			base.UpdateDrawCache();
			UpdateImageAttributes();
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal CustomizableMetaFile(ShapeType shapeType, Template template,
			string resourceBaseName, Assembly resourceAssembly)
			: base(shapeType, template, resourceBaseName, resourceAssembly) {
			Construct();
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal CustomizableMetaFile(ShapeType shapeType, IStyleSet styleSet,
			string resourceBaseName, Assembly resourceAssembly)
			: base(shapeType, styleSet, resourceBaseName, resourceAssembly) {
			Construct();
		}


		/// <override></override>
		protected internal override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			Debug.Assert(image is Metafile);
			metafileDataSize = 20;
			metafileData = new byte[metafileDataSize];

			LineStyle = styleSet.LineStyles.Normal;
			FillStyle = styleSet.FillStyles.Red;
		}


		private void Construct() {
			//replaceBrushCallback = new Graphics.EnumerateMetafileProc(ReplaceBrushCallbackProc);
			findReplaceColorCallback = new Graphics.EnumerateMetafileProc(FindReplaceColorCallbackProc);
			FindRemapColor();
		}


		private bool ReplaceBrushCallbackProc(EmfPlusRecordType recordType, int flags, int dataSize, IntPtr data, PlayRecordCallback callbackData) {
			try {
				if (data == IntPtr.Zero)
					((Metafile)image).PlayRecord(recordType, flags, 0, null);
				else {
					if (dataSize > metafileDataSize) {
						metafileDataSize = dataSize;
						metafileData = new byte[metafileDataSize];
					}
					// Copy the unmanaged record to a managed byte buffer that can be used by PlayRecord.
					Marshal.Copy(data, metafileData, 0, dataSize);
					// Adjust the color
					switch (recordType) {
						case EmfPlusRecordType.EmfCreateBrushIndirect:
							// This type of record only appears in WMF and 'classic' EMF files, not in EMF+ files.
							// Get color of current brush
							if (!replaceColorFound) {
								Buffers.SetByte(metafileData, 8, FillStyle.BaseColorStyle.Color.R);
								Buffers.SetByte(metafileData, 9, FillStyle.BaseColorStyle.Color.G);
								Buffers.SetByte(metafileData, 10, FillStyle.BaseColorStyle.Color.B);
								Buffers.SetByte(metafileData, 11, FillStyle.BaseColorStyle.Color.A);
								replaceColorFound = true;
							}
							break;
						default: /* nothing to do */ break;
					}
					((Metafile)image).PlayRecord(recordType, flags, dataSize, metafileData);
				}
			} catch (Exception exc) {
				Console.WriteLine("Error while playing metafile record: {0}", exc.Message);
			}
			return true;
		}


		private bool FindReplaceColorCallbackProc(EmfPlusRecordType recordType, int flags, int dataSize, IntPtr data, PlayRecordCallback callbackData) {
			switch (recordType) {
				case EmfPlusRecordType.EmfCreateBrushIndirect:
					if (!replaceColorFound) {
						// Copy the unmanaged record to a managed byte buffer that can be used by PlayRecord.
						if (dataSize > metafileDataSize) {
							metafileDataSize = dataSize;
							metafileData = new byte[metafileDataSize];
						}
						Marshal.Copy(data, metafileData, 0, dataSize);
						// Get the RGB color components of the brush
						replaceColor = Color.FromArgb(metafileData[8], metafileData[9], metafileData[10]);
						replaceColorFound = true;
					}
					break;
				
				default:
					// Nothing to do
					break;
			}
			// No need to play the record as we only search a color to replace
			//((Metafile)image).PlayRecord(recordType, flags, dataSize, dataArray);    //note that every record gets played even if the dataArray is empty
			return true;
		}


		// This is the official sample code from MSDN documentation. Does not work on x64 systems. See comment on CreateImage()
		private bool MetafileCallback(EmfPlusRecordType recordType, int flags, int dataSize, IntPtr data, PlayRecordCallback callbackData) {
			byte[] dataArray = null;
			if (data != IntPtr.Zero) {
				// Copy the unmanaged record to a managed byte buffer 
				// that can be used by PlayRecord.
				dataArray = new byte[dataSize];
				Marshal.Copy(data, dataArray, 0, dataSize);
			}
			((Metafile)image).PlayRecord(recordType, flags, dataSize, dataArray);
			return true;
		}


		// Due to a bug in the .NET wrapper of GDI+, Records using CreateBrushIndirect are not played on x64 systems.
		// On x86 systems, these records can be played if the record's buffer is enlarged by 8 bytes but we don't
		// want to rely on this.
		// So we use EnumerateMetafile for searching a color to replace and do the color replacement via the color 
		// remapping functionality of the ImageAttributes class.
		private Metafile CreateImage() {
			// Create MetaFile and graphics context
			Metafile metaFile = null;
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			using (Graphics gfx = Graphics.FromHwnd(IntPtr.Zero)) {
				IntPtr hdc = gfx.GetHdc();
				try {
					MetafileHeader header = ((Metafile)image).GetMetafileHeader();
					EmfType emfType;
					switch (header.Type) {
						case MetafileType.EmfPlusDual: emfType = EmfType.EmfPlusDual; break;
						case MetafileType.EmfPlusOnly: emfType = EmfType.EmfPlusOnly; break;
						default: emfType = EmfType.EmfOnly; break;
					}
					metaFile = new Metafile(stream, hdc, header.Bounds, MetafileFrameUnit.Pixel,
						emfType, "CustomizableMetafile Buffer Image");
				} finally {
					gfx.ReleaseHdc(hdc);
				}
			}

			UpdateImageAttributes();
			using (Graphics gfx = Graphics.FromImage(metaFile)) {
				GdiHelpers.ApplyGraphicsSettings(gfx, RenderingQuality.HighQuality);
				MetafileHeader header = ((Metafile)image).GetMetafileHeader();
				GdiHelpers.DrawImage(gfx, image, imageAttribs, ImageLayoutMode.Fit, header.Bounds, header.Bounds);
				// Deactivated, see comment above
				//gfx.EnumerateMetafile((Metafile)image, header.Bounds, header.Bounds, GraphicsUnit.Pixel, replaceBrushCallback, IntPtr.Zero, imageAttribs);
			}
			if (stream != null) {
				stream.Dispose();
				stream = null;
			}
			return metaFile;
		}


		private void FindRemapColor() {
			using (Graphics gfx = Graphics.FromHwnd(IntPtr.Zero)) {
				MetafileHeader header = ((Metafile)image).GetMetafileHeader();
				gfx.EnumerateMetafile((Metafile)image, header.Bounds, header.Bounds, GraphicsUnit.Pixel, findReplaceColorCallback, IntPtr.Zero);
			}
		}


		private void UpdateImageAttributes() {
			GdiHelpers.DisposeObject(ref imageAttribs);
			imageAttribs = GdiHelpers.GetImageAttributes(ImageLayoutMode.Fit, isPreview);
			imageAttribs.ClearRemapTable();

			// As long as we cannot set/unset the color (available in next version), we map every color...
			//if (ReplaceColor != Color.Empty) {
				colorReplaceMap.OldColor = ReplaceColor;
				colorReplaceMap.NewColor = FillStyle.BaseColorStyle.Color;
				ColorMap[] colorMap = { colorReplaceMap };
				imageAttribs.SetRemapTable(colorMap);
			//}
		}


		/// <summary>
		/// Specifies the color of the meta file to replace with the base color of the specified fill style.
		/// </summary>
		private Color ReplaceColor {
			get { return (privateReplaceColor != Color.Empty) ? privateReplaceColor : replaceColor; }
			set {
				privateReplaceColor = value;
				UpdateImageAttributes();
				Invalidate();
			}
		}


		// Buffers used for replacing solid brushes in EMF files (not used any longer, see comment on CreateImage())
		//private Graphics.EnumerateMetafileProc replaceBrushCallback;
		//private Metafile bufferImage;
		//private bool brushReplaced;

		// Buffers for replacing colors via color remap table
		private Graphics.EnumerateMetafileProc findReplaceColorCallback;
		private bool replaceColorFound;
		private Color privateReplaceColor = Color.Empty;
		private Color replaceColor = Color.Empty;
		private ColorMap colorReplaceMap = new ColorMap();

		// Buffer for metafile record's data
		private byte[] metafileData = null; // Data buffer for meta file drawing
		private int metafileDataSize = 0; // Allocated size for meta file data

		private bool isPreview = false;
		private ImageAttributes imageAttribs;
	}

}