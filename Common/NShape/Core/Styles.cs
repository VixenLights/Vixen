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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.InteropServices;
using Dataweb.NShape.Advanced;


namespace Dataweb.NShape
{

	#region ***   Enums, Structs and Delegates   ***

	/// <summary>
	/// Specifies the category of a style.
	/// </summary>
	public enum StyleCategory
	{
		/// <summary>Specifies a line cap style.</summary>
		CapStyle,

		/// <summary>Specifies a character style.</summary>
		CharacterStyle,

		/// <summary>Specifies a color style.</summary>
		ColorStyle,

		/// <summary>Specifies a fill style.</summary>
		FillStyle,

		/// <summary>Specifies a line style.</summary>
		LineStyle,

		/// <summary>Specifies a paragraph style.</summary>
		ParagraphStyle
	}


	/// <summary>
	/// Specifies the layout of an image inside its display bounds.
	/// </summary>
	public enum ImageLayoutMode
	{
		/// <summary>The image is displayed unscaled and aligned to the upper left corner.</summary>
		Original,

		/// <summary>The image is displayed unscaled and centered.</summary>
		Center,

		/// <summary>The image is stretched to the display bounds.</summary>
		Stretch,

		/// <summary>The image is fitted into the display bounds maintaining the aspect ratio of the image.</summary>
		Fit,

		/// <summary>The image is displayed as tiles.</summary>
		Tile,

		/// <summary>The image is displayed as tiles where one tile will be centered.</summary>
		CenterTile,

		/// <summary>The image is displayed as tiles. Tiles are flipped so the tile bounds match each other.</summary>
		FlipTile
	}


	/// <summary>
	/// Specifies the fill mode of a <see cref="T:Dataweb.NShape.IFillStyle" />.
	/// </summary>
	public enum FillMode
	{
		/// <summary>The area is filled with a color.</summary>
		Solid,

		/// <summary>The area is filled with a color gradient.</summary>
		Gradient,

		/// <summary>The area is filled with a pattern.</summary>
		Pattern,

		/// <summary>The area is filled with an image.</summary>
		Image
	}


	/// <summary>
	/// Specifies the shape of a line cap.
	/// </summary>
	public enum CapShape
	{
		/// <summary>No line cap (equals 'Round').</summary>
		None,

		/// <summary>A triangle shaped arrow cap.</summary>
		ClosedArrow,

		/// <summary>A V-shaped arrow cap.</summary>
		OpenArrow,

		/// <summary>A circular line cap.</summary>
		Circle,

		/// <summary>A triangle shaped cap. The triangle's base line is located at the line's cap.</summary>
		Triangle,

		/// <summary>A rhombical line cap.</summary>
		Diamond,

		/// <summary>A quadratic line cap.</summary>
		Square,

		/// <summary>A circular line cap. The circle's center is located at the line's cap.</summary>
		CenteredCircle,

		/// <summary>A half circle shapes line cap. The circle's center is located at the line's cap.</summary>
		CenteredHalfCircle,

		/// <summary>A rounded line cap.</summary>
		Round,

		/// <summary>A flattened line cap.</summary>
		Flat,

		/// <summary>A peaked line cap</summary>
		Peak
	}


	/// <summary>
	/// Specifies the dashes of a line.
	/// </summary>
	public enum DashType
	{
		/// <summary>Specifies a solid line.</summary>
		Solid,

		/// <summary>Specifies a line consisting of dashes.</summary>
		Dash,

		/// <summary>Specifies a line consisting of dots.</summary>
		Dot,

		/// <summary>Specifies a line consisting of a repeating pattern of dash-dot.</summary>
		DashDot,

		/// <summary>Specifies a line consisting of a repeating pattern of dash-dot-dot.</summary>
		DashDotDot
	}


	/// <summary>
	/// Represents padding or margin information of a layouted text.
	/// </summary>
	[TypeConverter("Dataweb.NShape.WinFormsUI.TextPaddingTypeConverter, Dataweb.NShape.WinFormsUI")]
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct TextPadding : IEquatable<TextPadding>
	{
		/// <summary>
		/// Provides a <see cref="T:Dataweb.NShape.TextPadding" /> object with no padding.
		/// </summary>
		public static readonly TextPadding Empty;


		/// <summary>
		/// Tests whether two specified <see cref="T:Dataweb.NShape.TextPadding" /> objects are equivalent.
		/// </summary>
		public static bool operator ==(TextPadding a, TextPadding b)
		{
			return (a.Left == b.Left
			        && a.Top == b.Top
			        && a.Right == b.Right
			        && a.Bottom == b.Bottom);
		}


		/// <summary>
		/// Tests whether two specified <see cref="T:Dataweb.NShape.TextPadding" /> objects are not equivalent.
		/// </summary>
		public static bool operator !=(TextPadding a, TextPadding b)
		{
			return !(a == b);
		}


		/// <summary>
		/// Generates a hash code for the current <see cref="T:Dataweb.NShape.TextPadding" />.
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.TextPadding" />.
		/// </summary>
		public TextPadding(int all)
		{
			if (all < 0) throw new ArgumentOutOfRangeException("all");
			this.all = true;
			this.left = this.top = this.right = this.bottom = all;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.TextPadding" />.
		/// </summary>
		public TextPadding(int left, int top, int right, int bottom)
		{
			if (left < 0) throw new ArgumentOutOfRangeException("left");
			if (top < 0) throw new ArgumentOutOfRangeException("top");
			if (right < 0) throw new ArgumentOutOfRangeException("right");
			if (bottom < 0) throw new ArgumentOutOfRangeException("bottom");
			this.all = false;
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
			CheckAll();
		}


		/// <summary>
		/// Gets or sets the padding value for the left edge.
		/// </summary>
		[RefreshProperties(RefreshProperties.All)]
		public int Left
		{
			get { return left; }
			set
			{
				if (value < 0) throw new ArgumentOutOfRangeException();
				if (all || left != value) {
					left = value;
					CheckAll();
				}
			}
		}


		/// <summary>
		/// Gets or sets the padding value for the top edge.
		/// </summary>
		[RefreshProperties(RefreshProperties.All)]
		public int Top
		{
			get { return all ? left : top; }
			set
			{
				if (value < 0) throw new ArgumentOutOfRangeException();
				if (all || top != value) {
					top = value;
					CheckAll();
				}
			}
		}


		/// <summary>
		/// Gets or sets the padding value for the right edge.
		/// </summary>
		[RefreshProperties(RefreshProperties.All)]
		public int Right
		{
			get { return all ? left : right; }
			set
			{
				if (value < 0) throw new ArgumentOutOfRangeException();
				if (all || right != value) {
					right = value;
					CheckAll();
				}
			}
		}


		/// <summary>
		/// Gets or sets the padding value for the bottom edge.
		/// </summary>
		[RefreshProperties(RefreshProperties.All)]
		public int Bottom
		{
			get { return all ? left : bottom; }
			set
			{
				if (value < 0) throw new ArgumentOutOfRangeException();
				if (all || bottom != value) {
					bottom = value;
					CheckAll();
				}
			}
		}


		/// <summary>
		/// Gets or sets the padding value for all edges.
		/// </summary>
		[RefreshProperties(RefreshProperties.All)]
		public int All
		{
			get { return all ? left : -1; }
			set
			{
				if (value < 0) throw new ArgumentOutOfRangeException();
				left = top = right = bottom = value;
				CheckAll();
			}
		}


		/// <summary>
		/// Gets the combined padding for the right and left edges.
		/// </summary>
		[Browsable(false)]
		public int Horizontal
		{
			get { return left + right; }
		}


		/// <summary>
		/// Gets the combined padding for the top and bottom edges.
		/// </summary>
		[Browsable(false)]
		public int Vertical
		{
			get { return top + bottom; }
		}


		/// <override></override>
		public override bool Equals(object obj)
		{
			return (obj is TextPadding && ((TextPadding) obj) == this);
		}


		/// <override></override>
		public bool Equals(TextPadding other)
		{
			return other == this;
		}


		static TextPadding()
		{
			Empty.left =
				Empty.top =
				Empty.right =
				Empty.bottom = 0;
			Empty.all = true;
		}


		private void CheckAll()
		{
			all = (left == top && left == right && left == bottom);
		}


		private int left, top, right, bottom;
		private bool all;
	}


	/// <summary>
	/// Returns the style of the same type with the same name if there is one in the design's style collection.
	/// </summary>
	public delegate IStyle FindStyleCallback(IStyle style);


	/// <ToBeCompleted></ToBeCompleted>
	public delegate Style CreatePreviewStyleCallback(IStyle style);

	#endregion

	#region ***   Type Description Classes   ***

	/// <summary>
	/// A type description provider that calls type converters and/or ui type editors registered with the TypeDescriptorRegistrar.
	/// </summary>
	public class StyleTypeDescriptionProvider : TypeDescriptionProvider
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.StyleTypeDescriptionProvider" />.
		/// </summary>
		public StyleTypeDescriptionProvider()
			: base(TypeDescriptor.GetProvider(typeof (Style)))
		{
		}


		/// <override></override>
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			return new StyleTypeDescriptor(base.GetTypeDescriptor(objectType, instance));
		}
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class StyleTypeDescriptor : CustomTypeDescriptor
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.StyleTypeDescriptor" />.
		/// </summary>
		public StyleTypeDescriptor(ICustomTypeDescriptor parent)
			: base(parent)
		{
		}


		/// <override></override>
		public override object GetEditor(Type editorBaseType)
		{
			return TypeDescriptorRegistrar.GetRegisteredUITypeEditor(editorBaseType) ?? base.GetEditor(editorBaseType);
		}


		/// <override></override>
		public override TypeConverter GetConverter()
		{
			return TypeDescriptorRegistrar.GetRegisteredTypeConverter(typeof (IStyle)) ?? base.GetConverter();
		}
	}

	#endregion

	#region ***   Style Interfaces   ***

	/// <summary>
	/// Provides read-only access to a styles that define the appearance of shapes.
	/// </summary>
	[TypeConverter("Dataweb.NShape.WinFormsUI.StyleTypeConverter, Dataweb.NShape.WinFormsUI")]
	public interface IStyle : IEntity, IDisposable
	{
		/// <summary>Specifies the culture independent name of the style. Used for identifying it within a style collection. Has to be unique inside a style collection.</summary>
		[Browsable(false)]
		string Name { get; }

		/// <summary>Specifies the culture dependent title of the style.</summary>
		string Title { get; }

		/// <override></override>
		string ToString();
	}


	/// <summary>
	/// Provides read-only access to a style that defines the appearance of line caps.
	/// </summary>
	[Editor("Dataweb.NShape.WinFormsUI.StyleUITypeEditor, Dataweb.NShape.WinFormsUI", typeof (UITypeEditor))]
	public interface ICapStyle : IStyle
	{
		/// <summary>Specifies the shape of the line cap.</summary>
		CapShape CapShape { get; }

		/// <summary>Specifies the diameter of the cap.</summary>
		short CapSize { get; }

		/// <summary>Specifies the interior fill color of the cap.</summary>
		IColorStyle ColorStyle { get; }
	}


	/// <summary>
	/// Provides read-only access to a style that defines the appearance of text.
	/// </summary>
	[Editor("Dataweb.NShape.WinFormsUI.StyleUITypeEditor, Dataweb.NShape.WinFormsUI", typeof (UITypeEditor))]
	public interface ICharacterStyle : IStyle
	{
		/// <summary>Specifies the name of the font family.</summary>
		string FontName { get; }

		/// <summary>Specifies the <see cref="T:System.Drawing.FontFamily" />.</summary>
		FontFamily FontFamily { get; }

		/// <summary>Specifies the size of the font in points.</summary>
		float SizeInPoints { get; }

		/// <summary>Specifies the size of the font in world coordinates.</summary>
		int Size { get; }

		/// <summary>Specifies the style of the text's characters, such as bold or italic.</summary>
		FontStyle Style { get; }

		/// <summary>Specifies the text color.</summary>
		IColorStyle ColorStyle { get; }
	}


	/// <summary>
	/// Provides read-only access to a style that defines colors.
	/// </summary>
	[Editor("Dataweb.NShape.WinFormsUI.StyleUITypeEditor, Dataweb.NShape.WinFormsUI", typeof (UITypeEditor))]
	public interface IColorStyle : IStyle
	{
		/// <summary>Specifies the color value of the color.</summary>
		Color Color { get; }

		/// <summary>Specifies transparency in percentage. Valid value range is 0 to 100.</summary>
		byte Transparency { get; }

		/// <summary>Specifies if the color value should be converted to a gray scale value.</summary>
		bool ConvertToGray { get; }
	}


	/// <summary>
	/// Provides read-only access to a style that defines the filling of a shape.
	/// </summary>
	[Editor("Dataweb.NShape.WinFormsUI.StyleUITypeEditor, Dataweb.NShape.WinFormsUI", typeof (UITypeEditor))]
	public interface IFillStyle : IStyle
	{
		/// <summary>
		/// Specifies the base color of the fill style. 
		/// Depending on the FillMode this means 
		/// the fill color (<see cref="E:Dataweb.NShape.FillMode.Solid" />), 
		/// the color of a gradient's lower right (<see cref="E:Dataweb.NShape.FillMode.Gradient" />) or 
		/// a pattern's background color (<see cref="E:Dataweb.NShape.FillMode.Pattern" />).
		/// </summary>
		IColorStyle BaseColorStyle { get; }

		/// <summary>
		/// Specifies the additional color of the fill style. 
		/// Depending on the FillMode this means the color of the gradient's upper left or the fore color of a pattern.
		/// </summary>
		IColorStyle AdditionalColorStyle { get; }

		/// <summary>
		/// Specifies the fill mode.
		/// </summary>
		FillMode FillMode { get; }

		/// <summary>
		/// Specifies the fill pattern, applies only when FillMode is set to FillMode.Pattern.
		/// </summary>
		HatchStyle FillPattern { get; }

		/// <summary>
		/// The angle of the color gradient in degrees. 45 means from upper left to lower right.
		/// </summary>
		short GradientAngle { get; }

		/// <summary>
		/// Specifies if the colors or the image should be converted into grayscale.
		/// </summary>
		bool ConvertToGrayScale { get; }

		/// <summary>
		/// Defines the image of a fill style. Only applies when FillMode is set to <see cref="E:Dataweb.NShape.FillMode.Image" />.
		/// </summary>
		NamedImage Image { get; }

		/// <summary>
		/// Defines the layout of the image inside its bounds. Only applies when FillMode is set to <see cref="E:Dataweb.NShape.FillMode.Image" />.
		/// </summary>
		ImageLayoutMode ImageLayout { get; }

		/// <summary>
		/// Defines the transparency of the image. Only applies when FillMode is set to <see cref="E:Dataweb.NShape.FillMode.Image" />.
		/// </summary>
		byte ImageTransparency { get; }

		/// <summary>
		/// Defines the gamma correction of the image. Only applies when FillMode is set to <see cref="E:Dataweb.NShape.FillMode.Image" />.
		/// </summary>
		float ImageGammaCorrection { get; }
	}


	/// <summary>
	/// Provides read-only access to a style that the appearance of lines and outlines.
	/// </summary>
	[Editor("Dataweb.NShape.WinFormsUI.StyleUITypeEditor, Dataweb.NShape.WinFormsUI", typeof (UITypeEditor))]
	public interface ILineStyle : IStyle
	{
		/// <summary>Specifies the thickness of the line in display units.</summary>
		int LineWidth { get; }

		/// <summary>Specifies the color of the line and the line cap's outline.</summary>
		IColorStyle ColorStyle { get; }

		/// <summary>Specifies whether the line is dashed.</summary>
		DashType DashType { get; }

		/// <summary>Specifies the shape of the dashes' ends.</summary>
		DashCap DashCap { get; }

		/// <summary>Specifies the pattern of the dashes/dots (in display units).</summary>
		float[] DashPattern { get; }

		/// <summary>Specifies the shape of the line's corners.</summary>
		LineJoin LineJoin { get; }
	}


	/// <summary>
	/// Provides read-only access to a style that defines the layout of text.
	/// </summary>
	[Editor("Dataweb.NShape.WinFormsUI.StyleUITypeEditor, Dataweb.NShape.WinFormsUI", typeof (UITypeEditor))]
	public interface IParagraphStyle : IStyle
	{
		/// <summary>Specifies the alignment of the text inside a given area.</summary>
		ContentAlignment Alignment { get; }

		/// <summary>Specifies whether and how text should be truncated if the text's layout area is too small.</summary>
		StringTrimming Trimming { get; }

		/// <summary>Specifies the margin between the layout area's bounds and the text.</summary>
		TextPadding Padding { get; }

		/// <summary>Specifies whether text should be wrapped automatically if the text does not fit into the layout area.</summary>
		bool WordWrap { get; }
	}

	#endregion

	#region ***   StandardStyle Definitions   ***

	/// <summary>
	/// Base class for StandardStyleNames. 
	/// Implements all methods. Derived classes only have to define 
	/// public readonly string fields named like the standard style name.
	/// </summary>
	public abstract class StandardStyleNames
	{
		/// <summary>
		/// Base constructor of all derived StandardStyleNames.
		/// Initializes all public readonly string fields with their field names
		/// and creates the names string array.
		/// </summary>
		protected StandardStyleNames()
		{
			FieldInfo[] fieldInfos = this.GetType().GetFields();
			Array.Resize<string>(ref names, fieldInfos.Length);
			int idx = -1;
			for (int i = fieldInfos.Length - 1; i >= 0; --i) {
				if (fieldInfos[i].IsInitOnly && fieldInfos[i].IsPublic &&
				    fieldInfos[i].FieldType == typeof (string)) {
					names[++idx] = fieldInfos[i].Name;
					fieldInfos[i].SetValue(this, fieldInfos[i].Name);
				}
				else {
				}
			}
			if (idx + 1 < names.Length) Array.Resize<string>(ref names, idx + 1);
		}


		/// <summary>
		/// Provides index based access to the items of the collection.
		/// </summary>
		public string this[int index]
		{
			get
			{
				if (index >= names.Length) throw new IndexOutOfRangeException();
				return names[index];
			}
		}


		/// <summary>
		/// Specifies the number of items in the collection.
		/// </summary>
		public int Count
		{
			get { return names.Length; }
		}


		/// <summary>
		/// Returns true if the given name equals any of the items in the collection.
		/// </summary>
		public bool EqualsAny(string name)
		{
			Debug.Assert(names != null);
			if (name == Style.EmptyStyleName) return true;
			for (int i = names.Length - 1; i >= 0; --i)
				if (names[i].Equals(name, StringComparison.InvariantCultureIgnoreCase))
					return true;
			return false;
		}


		private string[] names;
	}


	/// <summary>
	/// Defines the standard cap style names.
	/// </summary>
	public sealed class StandardCapStyleNames : StandardStyleNames
	{
		/// <ToBeCompleted></ToBeCompleted>
		[Obsolete("This name will be removed in future versions. Use ArrowClosed instead.")] public readonly string Arrow;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string ArrowClosed;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string ArrowOpen;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string None;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Special1;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Special2;
	}


	/// <summary>
	/// Defines the standard character style names.
	/// </summary>
	public sealed class StandardCharacterStyleNames : StandardStyleNames
	{
		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Caption;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Heading1;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Heading2;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Heading3;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Normal;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Subtitle;
	}


	/// <summary>
	/// Defines the standard color style names.
	/// </summary>
	public sealed class StandardColorStyleNames : StandardStyleNames
	{
		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Background;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Black;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Blue;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Gray;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Green;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Highlight;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string HighlightText;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string LightBlue;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string LightGray;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string LightGreen;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string LightRed;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string LightYellow;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Red;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Text;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Transparent;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string White;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Yellow;
	}


	/// <summary>
	/// Defines the standard fill style names.
	/// </summary>
	public sealed class StandardFillStyleNames : StandardStyleNames
	{
		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Black;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Blue;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Green;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Red;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Transparent;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string White;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Yellow;
	}


	/// <summary>
	/// Defines the standard line style names.
	/// </summary>
	public sealed class StandardLineStyleNames : StandardStyleNames
	{
		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Blue;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Dashed;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Dotted;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Green;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Highlight;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string HighlightDashed;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string HighlightDotted;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string HighlightThick;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string None;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Normal;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Red;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Special1;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Special2;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Thick;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Yellow;
	}


	/// <summary>
	/// Defines the standard paragraph style names.
	/// </summary>
	public sealed class StandardParagraphStyleNames : StandardStyleNames
	{
		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Label;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Text;

		/// <ToBeCompleted></ToBeCompleted>
		public readonly string Title;
	}

	#endregion

	#region ***   Style Classes   ***

	/// <summary>
	/// Provides a base class for <see cref="T:Dataweb.NShape.IStyle" /> implementations.
	/// </summary>
	[TypeDescriptionProvider(typeof (TypeDescriptionProviderDg))]
	public abstract class Style : IStyle, IEquatable<IStyle>, IEquatable<Style>
	{
		/// <override></override>
		[Description(
			"The name of the style, used for identifying this style in the style set. Has to unique inside its style set.")]
		[RequiredPermission(Permission.Designs)]
		[Category("General")]
		public string Name
		{
			get { return name; }
			set
			{
				if (string.IsNullOrEmpty(value) || value == EmptyStyleName)
					throw new ArgumentException(string.Format("'{0}' is not a valid Style name.", value));
				if (!renameable) throw new InvalidOperationException("Standard styles must not be renamed.");
				else if (IsStandardName(value))
					throw new ArgumentException(
						string.Format("'{0}' is a standard style name and therefore not valid for a custom style.", value));
				name = value;
			}
		}


		/// <override></override>
		[Description("The title of the style.")]
		[RequiredPermission(Permission.Designs)]
		[Category("General")]
		public string Title
		{
			get { return string.IsNullOrEmpty(title) ? name : title; }
			set
			{
				if (value == name || string.IsNullOrEmpty(value))
					title = null;
				else title = value;
			}
		}


		/// <summary>
		/// Copies all properties from the given <see cref="T:Dataweb.NShape.IStyle" />.
		/// </summary>
		/// <param name="style">Specifies the copy source.</param>
		/// <param name="findStyleCallback">A callback method that obtains a style.</param>
		public virtual void Assign(IStyle style, FindStyleCallback findStyleCallback)
		{
			if (style == null) throw new ArgumentNullException("style");
			if (findStyleCallback == null) throw new ArgumentNullException("findStyleCallback");
			if (this.Name != style.Name) this.Name = style.Name;
			this.Title = style.Title;
		}


		/// <override></override>
		public override string ToString()
		{
			return Title;
		}


		/// <override></override>
		public bool Equals(IStyle other)
		{
			return other == this;
		}


		/// <override></override>
		public bool Equals(Style other)
		{
			return other == this;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Style" />.
		/// </summary>
		protected Style()
			: this(string.Empty)
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Style" />.
		/// </summary>
		protected Style(string name)
		{
			this.renameable = !IsStandardName(name);
			this.name = name;
			this.title = string.Empty;
		}


		/// <summary>
		/// Finalizer of <see cref="T:Dataweb.NShape.Style" />.
		/// </summary>
		~Style()
		{
			Dispose();
		}


		/// <summary>
		/// Tests if the given name is a standard style name.
		/// </summary>
		protected abstract bool IsStandardName(string name);


		/// <summary>
		/// Defines an empty style name.
		/// </summary>
		protected internal const string EmptyStyleName = "0";

		#region IDisposable Members

		/// <override></override>
		public abstract void Dispose();

		#endregion

		#region IEntity Members

		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Style" />.
		/// </summary>
		public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			yield return new EntityFieldDefinition("Name", typeof (string));
			if (version >= 3) yield return new EntityFieldDefinition("Title", typeof (string));
		}


		/// <override></override>
		[Browsable(false)]
		public virtual object Id
		{
			get { return id; }
		}


		/// <override></override>
		public virtual void AssignId(object id)
		{
			if (id == null) throw new ArgumentNullException("id");
			if (this.id != null)
				throw new InvalidOperationException("Style has already an id.");
			this.id = id;
		}


		/// <override></override>
		public virtual void LoadFields(IRepositoryReader reader, int version)
		{
			if (reader == null) throw new ArgumentNullException("reader");
			name = reader.ReadString();
			if (version >= 3) title = reader.ReadString();
			renameable = !IsStandardName(name);
		}


		/// <override></override>
		public virtual void LoadInnerObjects(string propertyName, IRepositoryReader reader, int version)
		{
			if (propertyName == null) throw new ArgumentNullException("propertyName");
			if (reader == null) throw new ArgumentNullException("reader");
			// nothing to do
		}


		/// <override></override>
		public virtual void SaveFields(IRepositoryWriter writer, int version)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			writer.WriteString(name);
			if (version >= 3) writer.WriteString(title);
		}


		/// <override></override>
		public virtual void SaveInnerObjects(string propertyName, IRepositoryWriter writer, int version)
		{
			if (propertyName == null) throw new ArgumentNullException("propertyName");
			if (writer == null) throw new ArgumentNullException("writer");
			// nothing to do
		}


		/// <override></override>
		public virtual void Delete(IRepositoryWriter writer, int version)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			foreach (EntityPropertyDefinition pi in GetPropertyDefinitions(version)) {
				if (pi is EntityInnerObjectsDefinition)
					writer.DeleteInnerObjects();
			}
		}

		#endregion

		#region Fields

		private object id = null;
		private string name = null;
		private string title = null;
		private bool renameable = true;

		#endregion
	}


	/// <summary>
	/// Provides the definition of line cap.
	/// </summary>
	public sealed class CapStyle : Style, ICapStyle, IEquatable<ICapStyle>, IEquatable<CapStyle>
	{
		/// <summary>
		/// This static read-only field represents a default and not initialized cap style.
		/// </summary>
		public static readonly CapStyle Default;


		/// <summary>
		/// Provides the <see cref="T:Dataweb.NShape.StandardCapStyleNames" />.
		/// </summary>
		public static StandardCapStyleNames StandardNames;


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.CapStyle" />.
		/// </summary>
		public CapStyle()
			: base()
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.CapStyle" />.
		/// </summary>
		public CapStyle(string name)
			: base(name)
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.CapStyle" />.
		/// </summary>
		public CapStyle(string name, CapShape capShape, IColorStyle colorStyle)
			: base(name)
		{
			this.CapShape = capShape;
			this.ColorStyle = colorStyle;
		}

		#region IDisposable Members

		/// <override></override>
		public override void Dispose()
		{
			if (colorStyle != null) {
				colorStyle.Dispose();
				colorStyle = null;
			}
		}

		#endregion

		#region IEntity Members

		/// <override></override>
		public override void LoadFields(IRepositoryReader reader, int version)
		{
			base.LoadFields(reader, version);
			CapShape = (CapShape) reader.ReadByte();
			CapSize = reader.ReadInt16();
			ColorStyle = (IColorStyle) reader.ReadColorStyle();
		}


		/// <override></override>
		public override void SaveFields(IRepositoryWriter writer, int version)
		{
			base.SaveFields(writer, version);
			writer.WriteByte((byte) capShape);
			writer.WriteInt16(capSize);
			writer.WriteStyle(colorStyle);
		}


		/// <summary>
		/// The entity type name of <see cref="T:Dataweb.NShape.CapStyle" />.
		/// </summary>
		public static string EntityTypeName
		{
			get { return "Core.CapStyle"; }
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.CapStyle" />.
		/// </summary>
		public new static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			foreach (EntityPropertyDefinition pi in Style.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("CapShape", typeof (byte));
			yield return new EntityFieldDefinition("CapSize", typeof (short));
			yield return new EntityFieldDefinition("ColorStyle", typeof (object));
		}

		#endregion

		/// <override></override>
		[Category("Appearance")]
		[Description("Shape of the line cap. If none, the cap of the line depends on the line style's LineJoin setting.")]
		[RequiredPermission(Permission.Designs)]
		public CapShape CapShape
		{
			get { return capShape; }
			set { capShape = value; }
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Size of the line cap in display units.")]
		[RequiredPermission(Permission.Designs)]
		public short CapSize
		{
			get { return capSize; }
			set
			{
				if (value < 0)
					throw new ArgumentException("Value has to be greater than 0.");
				capSize = value;
			}
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Color of the line cap's interior.")]
		[RequiredPermission(Permission.Designs)]
		public IColorStyle ColorStyle
		{
			get { return colorStyle ?? Dataweb.NShape.ColorStyle.Empty; }
			set
			{
				if (value == Dataweb.NShape.ColorStyle.Empty)
					colorStyle = null;
				else colorStyle = value;
			}
		}


		/// <override></override>
		public override void Assign(IStyle style, FindStyleCallback findStyleCallback)
		{
			if (style is CapStyle) {
				// Delete GDI+ objects based on the current style
				ToolCache.NotifyStyleChanged(this);

				base.Assign(style, findStyleCallback);
				this.CapShape = ((CapStyle) style).CapShape;
				this.CapSize = ((CapStyle) style).CapSize;
				IColorStyle colorStyle = (IColorStyle) findStyleCallback(((CapStyle) style).ColorStyle);
				if (colorStyle != null) this.ColorStyle = colorStyle;
				else this.ColorStyle = ((CapStyle) style).ColorStyle;
			}
			else throw new NShapeException("Style is not of the required type.");
		}


		/// <override></override>
		public bool Equals(ICapStyle other)
		{
			return other == this;
		}


		/// <override></override>
		public bool Equals(CapStyle other)
		{
			return other == this;
		}


		/// <override></override>
		protected override bool IsStandardName(string name)
		{
			return StandardNames.EqualsAny(name);
		}


		static CapStyle()
		{
			StandardNames = new StandardCapStyleNames();

			Default = new CapStyle(EmptyStyleName);
			Default.CapShape = CapShape.None;
			Default.CapSize = 1;
			Default.ColorStyle = Dataweb.NShape.ColorStyle.Empty;
		}


		private int GetCapShapePointCount(CapShape capShape)
		{
			switch (capShape) {
				case CapShape.OpenArrow:
				case CapShape.ClosedArrow:
				case CapShape.Triangle:
					return 3;
				case CapShape.Circle:
				case CapShape.Diamond:
				case CapShape.Square:
					return 4;
				case CapShape.None:
				case CapShape.Flat:
				case CapShape.Peak:
				case CapShape.Round:
					return 0;
				default:
					throw new NShapeUnsupportedValueException(capShape);
			}
		}

		#region Fields

		private CapShape capShape = CapShape.None;
		private short capSize = 10;
		private IColorStyle colorStyle = null;

		#endregion
	}


	/// <summary>
	/// Provides the definition of text appearance.
	/// </summary>
	public sealed class CharacterStyle : Style, ICharacterStyle, IEquatable<ICharacterStyle>, IEquatable<CharacterStyle>
	{
		/// <summary>
		/// This static read-only field represents a default and not initialized character style.
		/// </summary>
		public static readonly CharacterStyle Default;


		/// <summary>
		/// Provides the <see cref="T:Dataweb.NShape.StandardCharacterStyleNames" />.
		/// </summary>
		public static StandardCharacterStyleNames StandardNames;


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.CharacterStyle" />.
		/// </summary>
		public CharacterStyle()
			: base()
		{
			Construct();
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.CharacterStyle" />.
		/// </summary>
		public CharacterStyle(string name)
			: base(name)
		{
			Construct();
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.CharacterStyle" />.
		/// </summary>
		public CharacterStyle(string name, float sizeInPoints, IColorStyle colorStyle)
			: base(name)
		{
			this.ColorStyle = colorStyle;
			this.fontSizeInPoints = sizeInPoints;
			Construct();
		}

		#region IDisposable Members

		/// <override></override>
		public override void Dispose()
		{
			if (colorStyle != null) {
				colorStyle.Dispose();
				colorStyle = null;
			}
		}

		#endregion

		#region IEntity Members

		/// <override></override>
		public override void LoadFields(IRepositoryReader reader, int version)
		{
			base.LoadFields(reader, version);
			fontFamily = FindFontFamily(reader.ReadString());
			fontSizeInPoints = reader.ReadInt32()/100f;
			fontSize = Geometry.PointToPixel(fontSizeInPoints, dpi);
			fontStyle = (FontStyle) reader.ReadByte();
			ColorStyle = (IColorStyle) reader.ReadColorStyle(); // Set Property!
		}


		/// <override></override>
		public override void SaveFields(IRepositoryWriter writer, int version)
		{
			base.SaveFields(writer, version);
			writer.WriteString(fontFamily.Name);
			writer.WriteInt32((int) (100*fontSizeInPoints));
			writer.WriteByte((byte) fontStyle);
			writer.WriteStyle(colorStyle);
		}


		/// <summary>
		/// The type name of <see cref="T:Dataweb.NShape.CharacterStyle" />.
		/// </summary>
		public static string EntityTypeName
		{
			get { return "Core.CharacterStyle"; }
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.CharacterStyle" />.
		/// </summary>
		public new static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			foreach (EntityPropertyDefinition pi in Dataweb.NShape.Style.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("FontName", typeof (string));
			yield return new EntityFieldDefinition("Size", typeof (int));
			yield return new EntityFieldDefinition("Decoration", typeof (byte));
			yield return new EntityFieldDefinition("ColorStyle", typeof (object));
		}

		#endregion

		/// <override></override>
		[Category("Appearance")]
		[Description("The color of the style's font.")]
		[RequiredPermission(Permission.Designs)]
		public IColorStyle ColorStyle
		{
			get { return colorStyle ?? Dataweb.NShape.ColorStyle.Empty; }
			set
			{
				if (value == Dataweb.NShape.ColorStyle.Empty)
					colorStyle = null;
				else colorStyle = value;
			}
		}


		/// <override></override>
		[Browsable(false)]
		[Category("Appearance")]
		[RequiredPermission(Permission.Designs)]
		public FontFamily FontFamily
		{
			get { return fontFamily; }
			set { fontFamily = value; }
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Name of the style's font.")]
		[RequiredPermission(Permission.Designs)]
		[Editor("Dataweb.NShape.WinFormsUI.FontFamilyUITypeEditor, Dataweb.NShape.WinFormsUI", typeof (UITypeEditor))]
		public string FontName
		{
			get { return fontFamily.Name; }
			set { fontFamily = FindFontFamily(value); }
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Size of the style's font in display units.")]
		[RequiredPermission(Permission.Designs)]
		public int Size
		{
			get { return fontSize; }
			set
			{
				fontSize = value;
				fontSizeInPoints = Geometry.PixelToPoint(value, dpi);
			}
		}


		/// <summary>
		/// Font Size in Point (1/72 Inch)
		/// </summary>
		[Category("Appearance")]
		[Description("Size of the style's font in points.")]
		[RequiredPermission(Permission.Designs)]
		public float SizeInPoints
		{
			get { return fontSizeInPoints; }
			set
			{
				fontSizeInPoints = value;
				fontSize = Geometry.PointToPixel(value, dpi);
			}
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Style of the style's font.")]
		[RequiredPermission(Permission.Designs)]
		public FontStyle Style
		{
			get { return fontStyle; }
			set
			{
				if (value == FontStyle.Regular)
					fontStyle = FontStyle.Regular;
				else
					fontStyle = fontStyle ^ value;
			}
		}


		/// <override></override>
		public override void Assign(IStyle style, FindStyleCallback findStyleCallback)
		{
			if (style is CharacterStyle) {
				// Delete GDI+ objects based on the current style
				ToolCache.NotifyStyleChanged(this);

				base.Assign(style, findStyleCallback);

				IColorStyle colorStyle = (IColorStyle) findStyleCallback(((CharacterStyle) style).ColorStyle);
				if (colorStyle != null) this.ColorStyle = colorStyle;
				else this.ColorStyle = ((CharacterStyle) style).ColorStyle;

				this.FontName = ((CharacterStyle) style).FontName;
				this.SizeInPoints = ((CharacterStyle) style).SizeInPoints;
				this.Style = ((CharacterStyle) style).Style;
			}
			else throw new NShapeException("Style is not of the required type.");
		}


		/// <override></override>
		public bool Equals(ICharacterStyle other)
		{
			return other == this;
		}


		/// <override></override>
		public bool Equals(CharacterStyle other)
		{
			return other == this;
		}


		/// <override></override>
		protected override bool IsStandardName(string name)
		{
			return CharacterStyle.StandardNames.EqualsAny(name);
		}


		static CharacterStyle()
		{
			StandardNames = new StandardCharacterStyleNames();
			using (Graphics gfx = Graphics.FromHwnd(IntPtr.Zero))
				dpi = gfx.DpiY;

			Default = new CharacterStyle(EmptyStyleName);
			Default.ColorStyle = Dataweb.NShape.ColorStyle.Empty;
			Default.SizeInPoints = 10;
			Default.Style = FontStyle.Regular;
		}


		private void Construct()
		{
			// Get the system's default GenericSansSerif font family
			fontFamily = FindFontFamily(string.Empty);
			fontSize = Geometry.PointToPixel(fontSizeInPoints, dpi);
		}


		private FontFamily FindFontFamily(string fontName)
		{
			FontFamily result = null;
			if (!string.IsNullOrEmpty(fontName)) {
				FontFamily[] families = FontFamily.Families;
				foreach (FontFamily ff in families) {
					if (ff.Name.Equals(fontName, StringComparison.InvariantCultureIgnoreCase)) {
						result = ff;
						break;
					}
				}
			}
			return result ?? FontFamily.GenericSansSerif;
		}

		#region Fields

		private static readonly float dpi;

		private float fontSizeInPoints = 8.25f;
		private int fontSize;
		private FontStyle fontStyle = 0;
		private FontFamily fontFamily = null;
		private IColorStyle colorStyle = null;

		#endregion
	}


	/// <summary>
	/// Provides the definition of a color.
	/// </summary>
	public sealed class ColorStyle : Style, IColorStyle, IEquatable<IColorStyle>, IEquatable<ColorStyle>
	{
		/// <summary>
		/// This static read-only field represents a default and not initialized color style.
		/// </summary>
		public static readonly IColorStyle Empty;


		/// <summary>
		/// Provides the <see cref="T:Dataweb.NShape.StandardColorStyleNames" />.
		/// </summary>
		public static StandardColorStyleNames StandardNames;


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.ColorStyle" />.
		/// </summary>
		public ColorStyle()
			: this(string.Empty, Color.Empty)
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.ColorStyle" />.
		/// </summary>
		public ColorStyle(string name)
			: this(name, Color.Empty)
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.ColorStyle" />.
		/// </summary>
		public ColorStyle(string name, Color color)
			: base(name)
		{
			Construct(color, AlphaToTransparency(color.A));
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.ColorStyle" />.
		/// </summary>
		public ColorStyle(string name, Color color, byte transparency)
			: base(name)
		{
			Construct(color, transparency);
		}

		#region IDisposable Members

		/// <override></override>
		public override void Dispose()
		{
			// nothing to do
		}

		#endregion

		#region IEntity Members

		/// <override></override>
		public override void LoadFields(IRepositoryReader reader, int version)
		{
			base.LoadFields(reader, version);
			color = Color.FromArgb(reader.ReadInt32());
			transparency = reader.ReadByte();
			if (version >= 3) convertToGray = reader.ReadBool();
		}


		/// <override></override>
		public override void SaveFields(IRepositoryWriter writer, int version)
		{
			base.SaveFields(writer, version);
			writer.WriteInt32(color.ToArgb());
			writer.WriteByte(transparency);
			if (version >= 3) writer.WriteBool(convertToGray);
		}


		/// <summary>
		/// The entity type name of <see cref="T:Dataweb.NShape.ColorStyle" />.
		/// </summary>
		public static string EntityTypeName
		{
			get { return "Core.ColorStyle"; }
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.ColorStyle" />.
		/// </summary>
		public new static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			foreach (EntityPropertyDefinition pi in Style.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("Color", typeof (Color));
			yield return new EntityFieldDefinition("Transparency", typeof (byte));
			if (version >= 3) yield return new EntityFieldDefinition("ConvertToGray", typeof (bool));
		}

		#endregion

		/// <override></override>
		[Category("Appearance")]
		[Description("The style's color.")]
		[RequiredPermission(Permission.Designs)]
		public Color Color
		{
			get { return color; }
			set
			{
				color = value;
				transparency = AlphaToTransparency(color.A);
			}
		}


		/// <summary>
		/// Specifies if the color should be converted into a grayscale value.
		/// </summary>
		[Browsable(false)]
		[Category("Appearance")]
		[RequiredPermission(Permission.Designs)]
		public bool ConvertToGray
		{
			get { return convertToGray; }
			set { convertToGray = value; }
		}


		/// <summary>
		/// Indicates the transparency in percent (0-100).
		/// </summary>
		[Category("Appearance")]
		[Description("Transparency of the style's color in percentage. 100 is 100% transparent and therefore invisible.")]
		[RequiredPermission(Permission.Designs)]
		public byte Transparency
		{
			get { return transparency; }
			set
			{
				if (value < 0 || value > 100) throw new NShapeException("Value has to be between 0 and 100.");
				transparency = value;
				color = Color.FromArgb(TransparencyToAlpha(transparency), color);
			}
		}


		/// <override></override>
		public override void Assign(IStyle style, FindStyleCallback findStyleCallback)
		{
			if (style is ColorStyle) {
				// Delete GDI+ objects based on the current style
				ToolCache.NotifyStyleChanged(this);

				base.Assign(style, findStyleCallback);
				this.Color = ((ColorStyle) style).Color;
				this.Transparency = ((ColorStyle) style).Transparency;
				this.ConvertToGray = ((ColorStyle) style).ConvertToGray;
			}
			else throw new NShapeException("Style is not of the required type.");
		}


		/// <override></override>
		public bool Equals(IColorStyle other)
		{
			return other == this;
		}


		/// <override></override>
		public bool Equals(ColorStyle other)
		{
			return other == this;
		}


		/// <override></override>
		protected override bool IsStandardName(string name)
		{
			return StandardNames.EqualsAny(name);
		}


		static ColorStyle()
		{
			StandardNames = new StandardColorStyleNames();
			Empty = new ColorStyle(EmptyStyleName, Color.Empty);
		}


		private void Construct(Color color, byte transparency)
		{
			if (transparency < 0 || transparency > 100)
				throw new ArgumentOutOfRangeException("Argument 'transparency' has to be between 0 and 100.");
			this.transparency = transparency;
			this.color = Color.FromArgb(TransparencyToAlpha(transparency), color);
		}


		private byte AlphaToTransparency(byte alpha)
		{
			return (byte) (100 - Math.Round(alpha/2.55f));
		}


		private byte TransparencyToAlpha(byte transparency)
		{
			if (transparency < 0 || transparency > 100)
				throw new ArgumentOutOfRangeException("Value has to be between 0 and 100.");
			return Convert.ToByte(255 - (transparency*2.55f));
		}

		#region Fields

		private Color color = Color.White;
		private byte transparency = 0;
		private bool convertToGray = false;

		#endregion
	}


	/// <summary>
	/// Provides the definition of filling.
	/// </summary>
	public sealed class FillStyle : Style, IFillStyle, IEquatable<IFillStyle>, IEquatable<FillStyle>
	{
		/// <summary>
		/// This static read-only field represents a default and not initialized fill style.
		/// </summary>
		public static readonly FillStyle Empty;


		/// <summary>
		/// Provides the <see cref="T:Dataweb.NShape.StandardFillStyleNames" />.
		/// </summary>
		public static StandardFillStyleNames StandardNames;


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.FillStyle" />.
		/// </summary>
		public FillStyle()
			: this(string.Empty)
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.FillStyle" />.
		/// </summary>
		public FillStyle(string name)
			: this(name, ColorStyle.Empty, ColorStyle.Empty)
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.FillStyle" />.
		/// </summary>
		public FillStyle(string name, IColorStyle baseColorStyle, IColorStyle additionalColorStyle)
			: base(name)
		{
			Construct(baseColorStyle, additionalColorStyle);
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.FillStyle" />.
		/// </summary>
		public FillStyle(string name, NamedImage image)
			: base(name)
		{
			Construct(image);
		}

		#region IDisposable Members

		/// <override></override>
		public override void Dispose()
		{
			if (baseColorStyle != null) {
				baseColorStyle.Dispose();
				baseColorStyle = null;
			}
			if (additionalColorStyle != null) {
				additionalColorStyle.Dispose();
				additionalColorStyle = null;
			}
			if (image != null) {
				image.Dispose();
				image = null;
			}
		}

		#endregion

		#region IEntity Members

		/// <override></override>
		public override void LoadFields(IRepositoryReader reader, int version)
		{
			base.LoadFields(reader, version);
			BaseColorStyle = (IColorStyle) reader.ReadColorStyle(); // Set property!
			AdditionalColorStyle = (IColorStyle) reader.ReadColorStyle(); // Set property!
			fillMode = (FillMode) reader.ReadByte();
			fillPattern = (HatchStyle) reader.ReadByte();
			if (version >= 3) convertToGrayScale = reader.ReadBool();
			imageLayout = (ImageLayoutMode) reader.ReadByte();
			imageTransparency = reader.ReadByte();
			imageGamma = reader.ReadFloat();
			imageCompressionQuality = reader.ReadByte();
			string imgName = reader.ReadString();
			Image img = reader.ReadImage();
			if (img != null) image = new NamedImage(img, imgName);
		}


		/// <override></override>
		public override void SaveFields(IRepositoryWriter writer, int version)
		{
			base.SaveFields(writer, version);
			writer.WriteStyle(baseColorStyle);
			writer.WriteStyle(additionalColorStyle);
			writer.WriteByte((byte) fillMode);
			writer.WriteByte((byte) fillPattern);
			if (version >= 3) writer.WriteBool(convertToGrayScale);
			writer.WriteByte((byte) imageLayout);
			writer.WriteByte(imageTransparency);
			writer.WriteFloat(imageGamma);
			writer.WriteByte(imageCompressionQuality);
			if (NamedImage.IsNullOrEmpty(image)) {
				writer.WriteString(string.Empty);
				writer.WriteImage(null);
			}
			else {
				writer.WriteString(image.Name);
				object imgTag = image.Image.Tag;
				image.Image.Tag = image.Name;
				writer.WriteImage(image.Image);
				image.Image.Tag = imgTag;
			}
		}


		/// <summary>
		/// The entity type name of <see cref="T:Dataweb.NShape.FillStyle" />.
		/// </summary>
		public static string EntityTypeName
		{
			get { return "Core.FillStyle"; }
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.FillStyle" />.
		/// </summary>
		public new static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			foreach (EntityPropertyDefinition pi in Style.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("BaseColorStyle", typeof (object));
			yield return new EntityFieldDefinition("AdditionalColorStyle", typeof (object));
			yield return new EntityFieldDefinition("FillMode", typeof (byte));
			yield return new EntityFieldDefinition("FillPattern", typeof (byte));
			if (version >= 3) yield return new EntityFieldDefinition("ConvertToGrayScale", typeof (bool));
			yield return new EntityFieldDefinition("ImageLayout", typeof (byte));
			yield return new EntityFieldDefinition("ImageTransparency", typeof (byte));
			yield return new EntityFieldDefinition("ImageGammaCorrection", typeof (float));
			yield return new EntityFieldDefinition("ImageCompressionQuality", typeof (byte));
			yield return new EntityFieldDefinition("ImageFileName", typeof (string));
			yield return new EntityFieldDefinition("Image", typeof (Image));
		}

		#endregion

		/// <override></override>
		[Category("Appearance")]
		[Description(
			"The additional color of the style. Depends on the style's FillMode: For gradients, it is the color of the upper left edge. In case of a pattern, this is the foreground color. For all other fill modes it will be ignored."
			)]
		[RequiredPermission(Permission.Designs)]
		public IColorStyle AdditionalColorStyle
		{
			get { return additionalColorStyle ?? Dataweb.NShape.ColorStyle.Empty; }
			set
			{
				if (value == Dataweb.NShape.ColorStyle.Empty)
					additionalColorStyle = null;
				else additionalColorStyle = value;
			}
		}


		/// <override></override>
		[Category("Appearance")]
		[Description(
			"The base color of the style. Depends on the style's FillMode: For gradients, it is the color of the lower right edge. In case of a pattern, this is the background color. For the solid fill mode it is the fill color and otherwise it will be ignored."
			)]
		[RequiredPermission(Permission.Designs)]
		public IColorStyle BaseColorStyle
		{
			get { return baseColorStyle ?? Dataweb.NShape.ColorStyle.Empty; }
			set
			{
				if (value == Dataweb.NShape.ColorStyle.Empty)
					baseColorStyle = null;
				else baseColorStyle = value;
			}
		}


		/// <summary>
		/// If true, the Image is shown as grayscale image
		/// </summary>
		[Category("Appearance")]
		[Description(
			"Specifies if the style's image should be displayed as grayscale image. Will be ignored if the fill mode is not image."
			)]
		[RequiredPermission(Permission.Designs)]
		public bool ConvertToGrayScale
		{
			get { return convertToGrayScale; }
			set { convertToGrayScale = value; }
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Specifies the fill mode of the style.")]
		[RequiredPermission(Permission.Designs)]
		public FillMode FillMode
		{
			get { return fillMode; }
			set { fillMode = value; }
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Specifies the fill pattern of the style. Will be ignored if the fill mode is not pattern.")]
		[RequiredPermission(Permission.Designs)]
		public HatchStyle FillPattern
		{
			get { return fillPattern; }
			set { fillPattern = value; }
		}


		/// <override></override>
		[Browsable(true)]
		public short GradientAngle
		{
			get { return gradientAngle; }
		}


		/// <override></override>
		[Category("Appearance")]
		[Description(
			"The Image of the fill style. Will be ignored if the fill mode is not image. Supports both bitmap and vector images."
			)]
		[RequiredPermission(Permission.Designs)]
		[Editor("Dataweb.NShape.WinFormsUI.NamedImageUITypeEditor, Dataweb.NShape.WinFormsUI", typeof (UITypeEditor))]
		public NamedImage Image
		{
			get { return image; }
			set { image = value; }
		}


		/// <summary>
		/// Quality setting in percentage when compressing the image with a non-lossless encoder.
		/// </summary>
		[Browsable(false)]
		[Category("Appearance")]
		[Description(
			"Specifies the compression quality in percentage when saving the style's image with a lossy compression file format. Will be ignored if the fill mode is not image."
			)]
		[RequiredPermission(Permission.Designs)]
		public byte ImageCompressionQuality
		{
			get { return imageCompressionQuality; }
			set
			{
				if (value < 0 || value > 100)
					throw new NShapeException("Value has to be between 0 and 100.");
				imageCompressionQuality = value;
			}
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Gamma correction for the style's image. Will be ignored if the fill mode is not image.")]
		[RequiredPermission(Permission.Designs)]
		public float ImageGammaCorrection
		{
			get { return imageGamma; }
			set
			{
				if (value <= 0) throw new ArgumentException("Value has to be greater than 0.");
				imageGamma = value;
			}
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Layout of the style's image. Will be ignored if the fill mode is not image.")]
		[RequiredPermission(Permission.Designs)]
		public ImageLayoutMode ImageLayout
		{
			get { return imageLayout; }
			set { imageLayout = value; }
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Transparency of the style's image. Will be ignored if the fill mode is not image.")]
		[RequiredPermission(Permission.Designs)]
		public byte ImageTransparency
		{
			get { return imageTransparency; }
			set
			{
				if (value < 0 || value > 100)
					throw new ArgumentException("The value has to be between 0 and 100.");
				imageTransparency = value;
			}
		}


		/// <override></override>
		public override void Assign(IStyle style, FindStyleCallback findStyleCallback)
		{
			if (style is FillStyle) {
				// Delete GDI+ objects based on the current style
				ToolCache.NotifyStyleChanged(this);

				base.Assign(style, findStyleCallback);
				IColorStyle colorStyle;
				if (((FillStyle) style).AdditionalColorStyle != null) {
					colorStyle = (IColorStyle) findStyleCallback(((FillStyle) style).AdditionalColorStyle);
					if (colorStyle != null) this.AdditionalColorStyle = colorStyle;
					else this.AdditionalColorStyle = ((FillStyle) style).AdditionalColorStyle;
				}
				if (((FillStyle) style).BaseColorStyle != null) {
					colorStyle = (IColorStyle) findStyleCallback(((FillStyle) style).BaseColorStyle);
					if (colorStyle != null) this.BaseColorStyle = colorStyle;
					else this.BaseColorStyle = ((FillStyle) style).BaseColorStyle;
				}

				this.ConvertToGrayScale = ((FillStyle) style).ConvertToGrayScale;
				this.FillMode = ((FillStyle) style).FillMode;
				this.FillPattern = ((FillStyle) style).FillPattern;
				if (this.Image != null) this.Image.Dispose();
				this.Image = ((FillStyle) style).Image;
				this.ImageCompressionQuality = ((FillStyle) style).ImageCompressionQuality;
				this.ImageGammaCorrection = ((FillStyle) style).ImageGammaCorrection;
				this.ImageLayout = ((FillStyle) style).ImageLayout;
				this.ImageTransparency = ((FillStyle) style).ImageTransparency;
			}
			else throw new NShapeException("Style is not of the required Type.");
		}


		/// <override></override>
		public bool Equals(IFillStyle other)
		{
			return other == this;
		}


		/// <override></override>
		public bool Equals(FillStyle other)
		{
			return other == this;
		}


		/// <override></override>
		protected override bool IsStandardName(string name)
		{
			return FillStyle.StandardNames.EqualsAny(name);
		}


		private void Construct(IColorStyle baseColorStyle, IColorStyle additionalColorStyle)
		{
			if (baseColorStyle == null) throw new ArgumentNullException("baseColorStyle");
			if (additionalColorStyle == null) throw new ArgumentNullException("additionalColorStyle");
			this.BaseColorStyle = baseColorStyle;
			this.AdditionalColorStyle = additionalColorStyle;
		}


		private void Construct(NamedImage image)
		{
			if (image == null) throw new ArgumentNullException("image");
			this.image = image;
		}


		static FillStyle()
		{
			StandardNames = new StandardFillStyleNames();

			Empty = new FillStyle(EmptyStyleName);
			Empty.AdditionalColorStyle = ColorStyle.Empty;
			Empty.BaseColorStyle = ColorStyle.Empty;
			Empty.ConvertToGrayScale = false;
			Empty.FillMode = FillMode.Solid;
			Empty.FillPattern = HatchStyle.Cross;
			Empty.Image = null;
			Empty.ImageLayout = ImageLayoutMode.Original;
		}

		#region Fields

		// Color and Pattern Stuff
		private IColorStyle baseColorStyle = null;
		private IColorStyle additionalColorStyle = null;
		private FillMode fillMode = FillMode.Gradient;
		private HatchStyle fillPattern = HatchStyle.BackwardDiagonal;
		private short gradientAngle = 45;
		// Image Stuff
		private NamedImage image = null;
		private ImageLayoutMode imageLayout = ImageLayoutMode.CenterTile;
		private byte imageTransparency = 0;
		private float imageGamma = 1f;
		private byte imageCompressionQuality = 100;
		private bool convertToGrayScale = false;

		#endregion
	}


	/// <summary>
	/// Provides the definition of lines and outlines.
	/// </summary>
	public sealed class LineStyle : Style, ILineStyle, IEquatable<ILineStyle>, IEquatable<LineStyle>
	{
		/// <summary>
		/// This static read-only field represents a default and not initialized line style.
		/// </summary>
		public static readonly LineStyle Empty;


		/// <summary>
		/// Provides the <see cref="T:Dataweb.NShape.StandardLineStyleNames" />.
		/// </summary>
		public static StandardLineStyleNames StandardNames;


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.LineStyle" />.
		/// </summary>
		public LineStyle(string name, int lineWidth, IColorStyle colorStyle)
			: base(name)
		{
			this.lineWidth = lineWidth;
			this.colorStyle = colorStyle;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.LineStyle" />.
		/// </summary>
		public LineStyle(string name)
			: base(name)
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.LineStyle" />.
		/// </summary>
		public LineStyle()
			: base()
		{
		}

		#region IDisposable Members

		/// <override></override>
		public override void Dispose()
		{
			if (colorStyle != null) {
				colorStyle.Dispose();
				colorStyle = null;
			}
			if (dashPattern != null)
				dashPattern = null;
		}

		#endregion

		#region IPersistable Members

		/// <override></override>
		public override void LoadFields(IRepositoryReader reader, int version)
		{
			base.LoadFields(reader, version);
			LineWidth = reader.ReadInt32();
			DashType = (DashType) reader.ReadByte();
			DashCap = (DashCap) reader.ReadByte(); // set property instead of member var in order to create DashPattern array
			LineJoin = (LineJoin) reader.ReadByte();
			ColorStyle = (IColorStyle) reader.ReadColorStyle(); // Set property!
		}


		/// <override></override>
		public override void SaveFields(IRepositoryWriter writer, int version)
		{
			base.SaveFields(writer, version);
			writer.WriteInt32(lineWidth);
			writer.WriteByte((byte) dashStyle);
			writer.WriteByte((byte) dashCap);
			writer.WriteByte((byte) lineJoin);
			writer.WriteStyle(colorStyle);
		}


		/// <summary>
		/// The entity type name of <see cref="T:Dataweb.NShape.LineStyle" />.
		/// </summary>
		public static string EntityTypeName
		{
			get { return "Core.LineStyle"; }
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.LineStyle" />.
		/// </summary>
		public new static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			foreach (EntityPropertyDefinition pi in Style.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("LineWidth", typeof (int));
			yield return new EntityFieldDefinition("DashType", typeof (byte));
			yield return new EntityFieldDefinition("DashCap", typeof (byte));
			yield return new EntityFieldDefinition("LineJoin", typeof (byte));
			yield return new EntityFieldDefinition("ColorStyle", typeof (object));
		}

		#endregion

		/// <override></override>
		[Category("Appearance")]
		[Description("Color of the line.")]
		[RequiredPermission(Permission.Designs)]
		public IColorStyle ColorStyle
		{
			get { return colorStyle ?? Dataweb.NShape.ColorStyle.Empty; }
			set
			{
				if (value == Dataweb.NShape.ColorStyle.Empty)
					colorStyle = null;
				else colorStyle = value;
			}
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Caps of a dashed line's dashes.")]
		[RequiredPermission(Permission.Designs)]
		public DashCap DashCap
		{
			get { return dashCap; }
			set { dashCap = value; }
		}


		/// <override></override>
		[Browsable(false)]
		public float[] DashPattern
		{
			get { return dashPattern; }
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Dash type of the line.")]
		[RequiredPermission(Permission.Designs)]
		public DashType DashType
		{
			get { return dashStyle; }
			set
			{
				dashStyle = value;
				switch (dashStyle) {
					case DashType.Solid:
						dashPattern = new float[0];
						break;
					case DashType.Dash:
						dashPattern = new float[2] {lineDashLen, lineDashSpace};
						break;
					case DashType.DashDot:
						dashPattern = new float[4] {lineDashLen, lineDashSpace, lineDotLen, lineDashSpace};
						break;
					case DashType.DashDotDot:
						dashPattern = new float[6] {lineDashLen, lineDashSpace, lineDotLen, lineDotSpace, lineDotLen, lineDashSpace};
						break;
					case DashType.Dot:
						dashPattern = new float[2] {lineDotLen, lineDotSpace};
						break;
					default:
						throw new NShapeUnsupportedValueException(dashStyle);
				}
			}
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Shape of the line's edges and caps.")]
		[RequiredPermission(Permission.Designs)]
		public LineJoin LineJoin
		{
			get { return lineJoin; }
			set { lineJoin = value; }
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Thickness of the line.")]
		[RequiredPermission(Permission.Designs)]
		public int LineWidth
		{
			get { return lineWidth; }
			set
			{
				if (value <= 0)
					throw new NShapeException("Value has to be greater than 0.");
				lineWidth = value;
			}
		}


		/// <override></override>
		public override void Assign(IStyle style, FindStyleCallback findStyleCallback)
		{
			if (style is LineStyle) {
				// Delete GDI+ objects based on the current style
				ToolCache.NotifyStyleChanged(this);

				base.Assign(style, findStyleCallback);
				IColorStyle colorStyle = (IColorStyle) findStyleCallback(((LineStyle) style).ColorStyle);
				if (colorStyle != null) this.ColorStyle = colorStyle;
				else this.ColorStyle = ((LineStyle) style).ColorStyle;

				this.DashCap = ((LineStyle) style).DashCap;
				this.DashType = ((LineStyle) style).DashType;
				this.LineJoin = ((LineStyle) style).LineJoin;
				this.LineWidth = ((LineStyle) style).LineWidth;
			}
			else throw new NShapeException("Style is not of the required type.");
		}


		/// <override></override>
		public bool Equals(ILineStyle other)
		{
			return other == this;
		}


		/// <override></override>
		public bool Equals(LineStyle other)
		{
			return other == this;
		}


		/// <override></override>
		protected override bool IsStandardName(string name)
		{
			return LineStyle.StandardNames.EqualsAny(name);
		}


		static LineStyle()
		{
			StandardNames = new StandardLineStyleNames();

			Empty = new LineStyle(EmptyStyleName);
			Empty.ColorStyle = Dataweb.NShape.ColorStyle.Empty;
			Empty.DashCap = DashCap.Round;
			Empty.DashType = DashType.Solid;
			Empty.LineJoin = LineJoin.Round;
			Empty.LineWidth = 1;
		}

		#region Fields

		private int lineWidth = 1;
		private IColorStyle colorStyle = null;
		private DashType dashStyle = DashType.Solid;
		private DashCap dashCap = DashCap.Round;
		private LineJoin lineJoin = LineJoin.Round;
		private float[] dashPattern = new float[0];
		// dashpattern defs
		private const float lineDashSpace = 2f;
		private const float lineDashLen = 5f;
		private const float lineDotSpace = 1f;
		private const float lineDotLen = 1f;

		#endregion
	}


	/// <summary>
	/// Provides the definition of text layout.
	/// </summary>
	public sealed class ParagraphStyle : Style, IParagraphStyle, IEquatable<IParagraphStyle>, IEquatable<ParagraphStyle>
	{
		/// <summary>
		/// This static read-only field represents a default and not initialized paragraph style.
		/// </summary>
		public static readonly ParagraphStyle Empty;


		/// <summary>
		/// Provides the <see cref="T:Dataweb.NShape.StandardParagraphStyleNames" />.
		/// </summary>
		public static StandardParagraphStyleNames StandardNames;


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.ParagraphStyle" />.
		/// </summary>
		public ParagraphStyle(string name)
			: base(name)
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.ParagraphStyle" />.
		/// </summary>
		public ParagraphStyle()
			: base()
		{
		}

		#region IDisposable Members

		/// <override></override>
		public override void Dispose()
		{
			// nothing to do
		}

		#endregion

		#region IEntity Members

		/// <override></override>
		public override void LoadFields(IRepositoryReader reader, int version)
		{
			base.LoadFields(reader, version);
			Alignment = (ContentAlignment) reader.ReadByte();
			Trimming = (StringTrimming) reader.ReadByte();
			Padding = new TextPadding(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
			WordWrap = reader.ReadBool();
		}


		/// <override></override>
		public override void SaveFields(IRepositoryWriter writer, int version)
		{
			base.SaveFields(writer, version);
			writer.WriteByte((byte) alignment);
			writer.WriteByte((byte) trimming);
			writer.WriteInt32(padding.Left);
			writer.WriteInt32(padding.Top);
			writer.WriteInt32(padding.Right);
			writer.WriteInt32(padding.Bottom);
			writer.WriteBool(wordWrap);
		}


		/// <summary>
		/// The entity type name of <see cref="T:Dataweb.NShape.ParagraphStyle" />.
		/// </summary>
		public static string EntityTypeName
		{
			get { return "Core.ParagraphStyle"; }
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.ParagraphStyle" />.
		/// </summary>
		public new static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			foreach (EntityPropertyDefinition pi in Style.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("Alignment", typeof (byte));
			yield return new EntityFieldDefinition("Trimming", typeof (byte));
			yield return new EntityFieldDefinition("PaddingLeft", typeof (int));
			yield return new EntityFieldDefinition("PaddingTop", typeof (int));
			yield return new EntityFieldDefinition("PaddingRight", typeof (int));
			yield return new EntityFieldDefinition("PaddingBottom", typeof (int));
			yield return new EntityFieldDefinition("WordWrap", typeof (bool));
		}

		#endregion

		/// <override></override>
		[Category("Appearance")]
		[Description("Alignment of the text.")]
		[RequiredPermission(Permission.Designs)]
		public ContentAlignment Alignment
		{
			get { return alignment; }
			set { alignment = value; }
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Trimming of the text if it requires more space than available.")]
		[RequiredPermission(Permission.Designs)]
		public StringTrimming Trimming
		{
			get { return trimming; }
			set { trimming = value; }
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Distance of the text from its layout rectangle.")]
		[RequiredPermission(Permission.Designs)]
		public TextPadding Padding
		{
			get { return padding; }
			set { padding = value; }
		}


		/// <override></override>
		[Category("Appearance")]
		[Description("Specifies if automatic line breaks should be enabled.")]
		[RequiredPermission(Permission.Designs)]
		public bool WordWrap
		{
			get { return wordWrap; }
			set { wordWrap = value; }
		}


		/// <override></override>
		public override void Assign(IStyle style, FindStyleCallback findStyleCallback)
		{
			if (style is ParagraphStyle) {
				// Delete GDI+ objects based on the current style
				ToolCache.NotifyStyleChanged(this);

				base.Assign(style, findStyleCallback);
				this.Alignment = ((ParagraphStyle) style).Alignment;
				this.Padding = ((ParagraphStyle) style).Padding;
				this.Trimming = ((ParagraphStyle) style).Trimming;
				this.WordWrap = ((ParagraphStyle) style).WordWrap;
			}
			else throw new NShapeException("Style is not of the required type.");
		}


		/// <override></override>
		public bool Equals(IParagraphStyle other)
		{
			return other == this;
		}


		/// <override></override>
		public bool Equals(ParagraphStyle other)
		{
			return other == this;
		}


		/// <override></override>
		protected override bool IsStandardName(string name)
		{
			return ParagraphStyle.StandardNames.EqualsAny(name);
		}


		static ParagraphStyle()
		{
			StandardNames = new StandardParagraphStyleNames();

			Empty = new ParagraphStyle(EmptyStyleName);
			Empty.Alignment = ContentAlignment.MiddleCenter;
			Empty.Padding = TextPadding.Empty;
			Empty.Trimming = StringTrimming.None;
			Empty.WordWrap = true;
		}

		#region Fields

		private ContentAlignment alignment = ContentAlignment.MiddleCenter;
		private StringTrimming trimming = StringTrimming.None;
		private TextPadding padding = TextPadding.Empty;
		private bool wordWrap = true;

		#endregion
	}

	#endregion

	#region ***   StyleCollection Classes   ***

	/// <summary>
	/// A collection of <see cref="T:Dataweb.NShape.IStyle" /> sorted by name.
	/// </summary>
	public abstract class StyleCollection<TStyle> where TStyle : class, IStyle
	{
		/// <summary>
		/// Initialize a new instance of <see cref="T:Dataweb.NShape.StyleCollection`1" />.
		/// </summary>
		public StyleCollection()
		{
			Construct(-1);
		}


		/// <summary>
		/// Initialize a new instance of <see cref="T:Dataweb.NShape.StyleCollection`1" />.
		/// </summary>
		public StyleCollection(int capacity)
		{
			Construct(capacity);
		}


		/// <summary>
		/// Provides index based direct access to the items of the collection.
		/// </summary>
		/// <param name="index">Zero-based index.</param>
		public TStyle this[int index]
		{
			get { return internalList[internalList.Keys[index]].Style; }
		}


		/// <summary>
		/// Provides index based direct access to the items of the collection.
		/// </summary>
		/// <param name="name">The name of the style.</param>
		public TStyle this[string name]
		{
			get { return internalList[name].Style; }
		}


		/// <summary>
		/// Returns the number of items in the collection.
		/// </summary>
		public int Count
		{
			get { return internalList.Count; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public TStyle GetPreviewStyle(TStyle style)
		{
			if (style == null) throw new ArgumentNullException("style");
			return internalList[style.Name].PreviewStyle;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public TStyle GetPreviewStyle(string styleName)
		{
			if (styleName == null) throw new ArgumentNullException("styleName");
			return internalList[styleName].PreviewStyle;
		}


		/// <override></override>
		public void Add(TStyle style, TStyle previewStyle)
		{
			if (style == null) throw new ArgumentNullException("style");
			if (previewStyle == null) throw new ArgumentNullException("previewStyle");
			internalList.Add(style.Name, new StyleCollection<TStyle>.StylePair<TStyle>(style, previewStyle));
		}


		/// <override></override>
		public void Clear()
		{
			foreach (KeyValuePair<string, StylePair<TStyle>> item in internalList) {
				IStyle baseStyle = item.Value.Style;
				IStyle previewStyle = item.Value.PreviewStyle;
				if (baseStyle != null) baseStyle.Dispose();
				if (previewStyle != null) previewStyle.Dispose();
				baseStyle = previewStyle = null;
			}
			internalList.Clear();
		}


		/// <override></override>
		public bool Contains(TStyle style)
		{
			if (style == null) throw new ArgumentNullException("style");
			for (int i = internalList.Values.Count - 1; i >= 0; --i) {
				if (internalList.Values[i].Style == style
				    || internalList.Values[i].PreviewStyle == style)
					return true;
			}
			return false;
		}


		/// <override></override>
		public bool Contains(string name)
		{
			if (name == null) throw new ArgumentNullException("name");
			return internalList.ContainsKey(name);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool ContainsPreviewStyle(TStyle style)
		{
			if (style == null) throw new ArgumentNullException("style");
			for (int i = internalList.Values.Count - 1; i >= 0; --i) {
				if (internalList.Values[i].Style == style)
					return (internalList.Values[i].PreviewStyle != null);
				else if (internalList.Values[i].PreviewStyle == style)
					return true;
			}
			return false;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool ContainsPreviewStyle(string name)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (internalList.ContainsKey(name))
				return (internalList[name].PreviewStyle != null);
			else return false;
		}


		/// <override></override>
		public int IndexOf(TStyle item)
		{
			if (item == null) throw new ArgumentNullException("item");
			for (int i = internalList.Values.Count - 1; i >= 0; --i) {
				if (internalList.Values[i].Style == item
				    || internalList.Values[i].PreviewStyle == item)
					return internalList.IndexOfKey(internalList.Values[i].Style.Name);
			}
			return -1;
		}


		/// <override></override>
		public int IndexOf(string styleName)
		{
			if (styleName == null) throw new ArgumentNullException("styleName");
			return internalList.IndexOfKey(styleName);
		}


		/// <override></override>
		public bool Remove(TStyle item)
		{
			if (item == null) throw new ArgumentNullException("item");
			return internalList.Remove(item.Name);
		}


		/// <override></override>
		public bool Remove(string styleName)
		{
			return internalList.Remove(styleName);
		}


		/// <override></override>
		public void RemoveAt(int index)
		{
			string key = internalList.Keys[index];
			internalList.Remove(key);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public abstract bool IsStandardStyle(TStyle style);


		/// <ToBeCompleted></ToBeCompleted>
		public void SetPreviewStyle(string baseStyleName, TStyle value)
		{
			if (baseStyleName == null) throw new ArgumentNullException("baseStyle");
			if (value == null) throw new ArgumentNullException("value");
			internalList[baseStyleName].PreviewStyle = value;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void SetPreviewStyle(TStyle baseStyle, TStyle value)
		{
			if (baseStyle == null) throw new ArgumentNullException("baseStyle");
			if (value == null) throw new ArgumentNullException("value");
			internalList[baseStyle.Name].PreviewStyle = value;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void SetStyle(TStyle style, TStyle previewStyle)
		{
			if (style == null) throw new ArgumentNullException("style");
			if (previewStyle == null) throw new ArgumentNullException("previewStyle");
			internalList[style.Name].Style = style;
			internalList[style.Name].PreviewStyle = previewStyle;
		}


		private void Construct(int capacity)
		{
			if (capacity > 0)
				internalList = new SortedList<string, StylePair<TStyle>>(capacity);
			else internalList = new SortedList<string, StylePair<TStyle>>();
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected class StylePair<T> where T : class, IStyle
		{
			/// <ToBeCompleted></ToBeCompleted>
			public StylePair(T baseStyle, T previewStyle)
			{
				this.Style = baseStyle;
				this.PreviewStyle = previewStyle;
			}

			/// <ToBeCompleted></ToBeCompleted>
			public T Style;

			/// <ToBeCompleted></ToBeCompleted>
			public T PreviewStyle;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected SortedList<string, StylePair<TStyle>> internalList = null;
	}


	/// <summary>
	/// A collection of <see cref="T:Dataweb.NShape.CapStyle" /> sorted by name.
	/// </summary>
	public class CapStyleCollection : StyleCollection<CapStyle>, ICapStyles
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.CapStyleCollection" />.
		/// </summary>
		public CapStyleCollection()
			: base(CapStyle.StandardNames.Count)
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.CapStyleCollection" />.
		/// </summary>
		public CapStyleCollection(int capacity)
			: base(capacity)
		{
		}


		/// <override></override>
		public override bool IsStandardStyle(CapStyle style)
		{
			if (style == null) throw new ArgumentNullException("style");
			return CapStyle.StandardNames.EqualsAny(style.Name);
		}

		#region ICapStyles Members

		ICapStyle ICapStyles.this[string name]
		{
			get { return internalList[name].Style; }
		}


		/// <override></override>
		public ICapStyle None
		{
			get { return internalList[CapStyle.StandardNames.None].Style; }
		}


		/// <override></override>
		[Obsolete("This property will be removed in future versions. Use ArrowClosed instead.")]
		public ICapStyle Arrow
		{
			get { return ClosedArrow; }
		}


		/// <override></override>
		public ICapStyle ClosedArrow
		{
			get { return internalList[CapStyle.StandardNames.ArrowClosed].Style; }
		}


		/// <override></override>
		public ICapStyle OpenArrow
		{
			get { return internalList[CapStyle.StandardNames.ArrowOpen].Style; }
		}


		/// <override></override>
		public ICapStyle Special1
		{
			get { return internalList[CapStyle.StandardNames.Special1].Style; }
		}


		/// <override></override>
		public ICapStyle Special2
		{
			get { return internalList[CapStyle.StandardNames.Special2].Style; }
		}

		#endregion

		#region IEnumerable<ICapStyle> Members

		/// <override></override>
		public IEnumerator<ICapStyle> GetEnumerator()
		{
			return Enumerator.Create(this);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Enumerator.Create(this);
		}

		#endregion

		private struct Enumerator : IEnumerator<ICapStyle>
		{
			public static readonly Enumerator Empty;

			public static Enumerator Create(CapStyleCollection collection)
			{
				if (collection == null) throw new ArgumentNullException("collection");
				Enumerator result = Enumerator.Empty;
				result.collection = collection;
				result.cnt = collection.Count;
				result.idx = -1;
				return result;
			}

			public Enumerator(CapStyleCollection collection)
			{
				if (collection == null) throw new ArgumentNullException("collection");
				this.collection = collection;
				this.cnt = collection.Count;
				this.idx = -1;
			}

			#region IEnumerator<ICapStyle> Members

			public ICapStyle Current
			{
				get { return collection[idx]; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				collection = null;
			}

			#endregion

			#region IEnumerator Members

			object IEnumerator.Current
			{
				get { return collection[idx]; }
			}

			public bool MoveNext()
			{
				return (++idx < cnt);
			}

			public void Reset()
			{
				idx = -1;
			}

			#endregion

			static Enumerator()
			{
				Empty.collection = null;
				Empty.cnt = 0;
				Empty.idx = -1;
			}

			private CapStyleCollection collection;
			private int idx;
			private int cnt;
		}
	}


	/// <summary>
	/// A collection of <see cref="T:Dataweb.NShape.CharacterStyle" /> sorted by name.
	/// </summary>
	public class CharacterStyleCollection : StyleCollection<CharacterStyle>, ICharacterStyles
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.CharacterStyleCollection" />.
		/// </summary>
		public CharacterStyleCollection()
			: base(CharacterStyle.StandardNames.Count)
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.CharacterStyleCollection" />.
		/// </summary>
		public CharacterStyleCollection(int capacity)
			: base(capacity)
		{
		}


		/// <override></override>
		public override bool IsStandardStyle(CharacterStyle style)
		{
			if (style == null) throw new ArgumentNullException("style");
			return CharacterStyle.StandardNames.EqualsAny(style.Name);
		}

		#region ICharacterStyles Members

		ICharacterStyle ICharacterStyles.this[string name]
		{
			get { return this[name]; }
		}


		/// <override></override>
		public ICharacterStyle Normal
		{
			get { return internalList[CharacterStyle.StandardNames.Normal].Style; }
		}


		/// <override></override>
		public ICharacterStyle Caption
		{
			get { return internalList[CharacterStyle.StandardNames.Caption].Style; }
		}


		/// <override></override>
		public ICharacterStyle Subtitle
		{
			get { return internalList[CharacterStyle.StandardNames.Subtitle].Style; }
		}


		/// <override></override>
		public ICharacterStyle Heading3
		{
			get { return internalList[CharacterStyle.StandardNames.Heading3].Style; }
		}


		/// <override></override>
		public ICharacterStyle Heading2
		{
			get { return internalList[CharacterStyle.StandardNames.Heading2].Style; }
		}


		/// <override></override>
		public ICharacterStyle Heading1
		{
			get { return internalList[CharacterStyle.StandardNames.Heading1].Style; }
		}

		#endregion

		#region IEnumerable<ICharacterStyle> Members

		/// <override></override>
		public IEnumerator<ICharacterStyle> GetEnumerator()
		{
			return Enumerator.Create(this);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Enumerator.Create(this);
		}

		#endregion

		private struct Enumerator : IEnumerator<ICharacterStyle>
		{
			public static readonly Enumerator Empty;

			public static Enumerator Create(CharacterStyleCollection collection)
			{
				if (collection == null) throw new ArgumentNullException("collection");
				Enumerator result = Enumerator.Empty;
				result.collection = collection;
				result.cnt = collection.Count;
				result.idx = -1;
				return result;
			}

			public Enumerator(CharacterStyleCollection collection)
			{
				if (collection == null) throw new ArgumentNullException("collection");
				this.collection = collection;
				this.cnt = collection.Count;
				this.idx = -1;
			}

			#region IEnumerator<ICapStyle> Members

			public ICharacterStyle Current
			{
				get { return collection[idx]; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				collection = null;
			}

			#endregion

			#region IEnumerator Members

			object IEnumerator.Current
			{
				get { return collection[idx]; }
			}

			public bool MoveNext()
			{
				return (++idx < cnt);
			}

			public void Reset()
			{
				idx = -1;
			}

			#endregion

			static Enumerator()
			{
				Empty.collection = null;
				Empty.cnt = 0;
				Empty.idx = -1;
			}

			private CharacterStyleCollection collection;
			private int idx;
			private int cnt;
		}
	}


	/// <summary>
	/// A collection of <see cref="T:Dataweb.NShape.ColorStyle" /> sorted by name.
	/// </summary>
	public class ColorStyleCollection : StyleCollection<ColorStyle>, IColorStyles
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.ColorStyleCollection" />.
		/// </summary>
		public ColorStyleCollection()
			: base(ColorStyle.StandardNames.Count)
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.ColorStyleCollection" />.
		/// </summary>
		public ColorStyleCollection(int capacity)
			: base(capacity)
		{
		}


		/// <override></override>
		public override bool IsStandardStyle(ColorStyle style)
		{
			if (style == null) throw new ArgumentNullException("style");
			return ColorStyle.StandardNames.EqualsAny(style.Name);
		}

		#region IColorStyles Members

		IColorStyle IColorStyles.this[string name]
		{
			get { return this[name]; }
		}

		/// <override></override>
		public IColorStyle Transparent
		{
			get { return internalList[ColorStyle.StandardNames.Transparent].Style; }
		}

		/// <override></override>
		public IColorStyle Background
		{
			get { return internalList[ColorStyle.StandardNames.Background].Style; }
		}

		/// <override></override>
		public IColorStyle Highlight
		{
			get { return internalList[ColorStyle.StandardNames.Highlight].Style; }
		}

		/// <override></override>
		public IColorStyle Text
		{
			get { return internalList[ColorStyle.StandardNames.Text].Style; }
		}

		/// <override></override>
		public IColorStyle HighlightText
		{
			get { return internalList[ColorStyle.StandardNames.HighlightText].Style; }
		}

		/// <override></override>
		public IColorStyle Black
		{
			get { return internalList[ColorStyle.StandardNames.Black].Style; }
		}

		/// <override></override>
		public IColorStyle White
		{
			get { return internalList[ColorStyle.StandardNames.White].Style; }
		}

		/// <override></override>
		public IColorStyle Gray
		{
			get { return internalList[ColorStyle.StandardNames.Gray].Style; }
		}

		/// <override></override>
		public IColorStyle LightGray
		{
			get { return internalList[ColorStyle.StandardNames.LightGray].Style; }
		}

		/// <override></override>
		public IColorStyle Red
		{
			get { return internalList[ColorStyle.StandardNames.Red].Style; }
		}

		/// <override></override>
		public IColorStyle LightRed
		{
			get { return internalList[ColorStyle.StandardNames.LightRed].Style; }
		}

		/// <override></override>
		public IColorStyle Blue
		{
			get { return internalList[ColorStyle.StandardNames.Blue].Style; }
		}

		/// <override></override>
		public IColorStyle LightBlue
		{
			get { return internalList[ColorStyle.StandardNames.LightBlue].Style; }
		}

		/// <override></override>
		public IColorStyle Green
		{
			get { return internalList[ColorStyle.StandardNames.Green].Style; }
		}

		/// <override></override>
		public IColorStyle LightGreen
		{
			get { return internalList[ColorStyle.StandardNames.LightGreen].Style; }
		}

		/// <override></override>
		public IColorStyle Yellow
		{
			get { return internalList[ColorStyle.StandardNames.Yellow].Style; }
		}

		/// <override></override>
		public IColorStyle LightYellow
		{
			get { return internalList[ColorStyle.StandardNames.LightYellow].Style; }
		}

		#endregion

		#region IEnumerable<IColorStyle> Members

		/// <override></override>
		public IEnumerator<IColorStyle> GetEnumerator()
		{
			return Enumerator.Create(this);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Enumerator.Create(this);
		}

		#endregion

		private struct Enumerator : IEnumerator<IColorStyle>
		{
			public static readonly Enumerator Empty;

			public static Enumerator Create(ColorStyleCollection collection)
			{
				if (collection == null) throw new ArgumentNullException("collection");
				Enumerator result = Enumerator.Empty;
				result.collection = collection;
				result.cnt = collection.Count;
				result.idx = -1;
				return result;
			}

			public Enumerator(ColorStyleCollection collection)
			{
				if (collection == null) throw new ArgumentNullException("collection");
				this.collection = collection;
				this.cnt = collection.Count;
				this.idx = -1;
			}

			#region IEnumerator<IColorStyle> Members

			public IColorStyle Current
			{
				get { return collection[idx]; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				collection = null;
			}

			#endregion

			#region IEnumerator Members

			object IEnumerator.Current
			{
				get { return collection[idx]; }
			}

			public bool MoveNext()
			{
				return (++idx < cnt);
			}

			public void Reset()
			{
				idx = -1;
			}

			#endregion

			static Enumerator()
			{
				Empty.collection = null;
				Empty.idx = -1;
				Empty.cnt = 0;
			}

			private ColorStyleCollection collection;
			private int idx;
			private int cnt;
		}
	}


	/// <summary>
	/// A collection of <see cref="T:Dataweb.NShape.FillStyle" /> sorted by name.
	/// </summary>
	public class FillStyleCollection : StyleCollection<FillStyle>, IFillStyles
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.FillStyleCollection" />.
		/// </summary>
		public FillStyleCollection()
			: base(FillStyle.StandardNames.Count)
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.FillStyleCollection" />.
		/// </summary>
		public FillStyleCollection(int capacity)
			: base(capacity)
		{
		}


		/// <override></override>
		public override bool IsStandardStyle(FillStyle style)
		{
			if (style == null) throw new ArgumentNullException("style");
			return FillStyle.StandardNames.EqualsAny(style.Name);
		}

		#region IFillStyles Members

		IFillStyle IFillStyles.this[string name]
		{
			get { return this[name]; }
		}

		/// <override></override>
		public IFillStyle Transparent
		{
			get { return internalList[FillStyle.StandardNames.Transparent].Style; }
		}

		/// <override></override>
		public IFillStyle Black
		{
			get { return internalList[FillStyle.StandardNames.Black].Style; }
		}

		/// <override></override>
		public IFillStyle White
		{
			get { return internalList[FillStyle.StandardNames.White].Style; }
		}

		/// <override></override>
		public IFillStyle Red
		{
			get { return internalList[FillStyle.StandardNames.Red].Style; }
		}

		/// <override></override>
		public IFillStyle Blue
		{
			get { return internalList[FillStyle.StandardNames.Blue].Style; }
		}

		/// <override></override>
		public IFillStyle Green
		{
			get { return internalList[FillStyle.StandardNames.Green].Style; }
		}

		/// <override></override>
		public IFillStyle Yellow
		{
			get { return internalList[FillStyle.StandardNames.Yellow].Style; }
		}

		#endregion

		#region IEnumerable<IFillStyle> Members

		/// <override></override>
		public IEnumerator<IFillStyle> GetEnumerator()
		{
			return Enumerator.Create(this);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Enumerator.Create(this);
		}

		#endregion

		private struct Enumerator : IEnumerator<IFillStyle>
		{
			public static readonly Enumerator Empty;

			public static Enumerator Create(FillStyleCollection collection)
			{
				if (collection == null) throw new ArgumentNullException("collection");
				Enumerator result = Enumerator.Empty;
				result.collection = collection;
				result.cnt = collection.Count;
				result.idx = -1;
				return result;
			}

			public Enumerator(FillStyleCollection collection)
			{
				if (collection == null) throw new ArgumentNullException("collection");
				this.collection = collection;
				this.cnt = collection.Count;
				this.idx = -1;
			}

			#region IEnumerator<ICapStyle> Members

			public IFillStyle Current
			{
				get { return collection[idx]; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				collection = null;
			}

			#endregion

			#region IEnumerator Members

			object IEnumerator.Current
			{
				get { return collection[idx]; }
			}

			public bool MoveNext()
			{
				return (++idx < cnt);
			}

			public void Reset()
			{
				idx = -1;
			}

			#endregion

			static Enumerator()
			{
				Empty.collection = null;
				Empty.cnt = 0;
				Empty.idx = -1;
			}

			private FillStyleCollection collection;
			private int idx;
			private int cnt;
		}
	}


	/// <summary>
	/// A collection of <see cref="T:Dataweb.NShape.LineStyle" /> sorted by name.
	/// </summary>
	public class LineStyleCollection : StyleCollection<LineStyle>, ILineStyles
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.LineStyleCollection" />.
		/// </summary>
		public LineStyleCollection()
			: base(LineStyle.StandardNames.Count)
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.LineStyleCollection" />.
		/// </summary>
		public LineStyleCollection(int capacity)
			: base(capacity)
		{
		}


		/// <override></override>
		public override bool IsStandardStyle(LineStyle style)
		{
			if (style == null) throw new ArgumentNullException("style");
			return LineStyle.StandardNames.EqualsAny(style.Name);
		}

		#region ILineStyles Members

		ILineStyle ILineStyles.this[string name]
		{
			get { return this[name]; }
		}

		/// <override></override>
		public ILineStyle None
		{
			get { return internalList[LineStyle.StandardNames.None].Style; }
		}

		/// <override></override>
		public ILineStyle Normal
		{
			get { return internalList[LineStyle.StandardNames.Normal].Style; }
		}

		/// <override></override>
		public ILineStyle Thick
		{
			get { return internalList[LineStyle.StandardNames.Thick].Style; }
		}

		/// <override></override>
		public ILineStyle Dotted
		{
			get { return internalList[LineStyle.StandardNames.Dotted].Style; }
		}

		/// <override></override>
		public ILineStyle Dashed
		{
			get { return internalList[LineStyle.StandardNames.Dashed].Style; }
		}

		/// <override></override>
		public ILineStyle Highlight
		{
			get { return internalList[LineStyle.StandardNames.Highlight].Style; }
		}

		/// <override></override>
		public ILineStyle HighlightThick
		{
			get { return internalList[LineStyle.StandardNames.HighlightThick].Style; }
		}

		/// <override></override>
		public ILineStyle HighlightDotted
		{
			get { return internalList[LineStyle.StandardNames.HighlightDotted].Style; }
		}

		/// <override></override>
		public ILineStyle HighlightDashed
		{
			get { return internalList[LineStyle.StandardNames.HighlightDashed].Style; }
		}

		/// <override></override>
		public ILineStyle Red
		{
			get { return internalList[LineStyle.StandardNames.Red].Style; }
		}

		/// <override></override>
		public ILineStyle Blue
		{
			get { return internalList[LineStyle.StandardNames.Blue].Style; }
		}

		/// <override></override>
		public ILineStyle Green
		{
			get { return internalList[LineStyle.StandardNames.Green].Style; }
		}

		/// <override></override>
		public ILineStyle Yellow
		{
			get { return internalList[LineStyle.StandardNames.Yellow].Style; }
		}

		/// <override></override>
		public ILineStyle Special1
		{
			get { return internalList[LineStyle.StandardNames.Special1].Style; }
		}

		/// <override></override>
		public ILineStyle Special2
		{
			get { return internalList[LineStyle.StandardNames.Special2].Style; }
		}

		#endregion

		#region IEnumerable<ILineStyle> Members

		/// <override></override>
		public IEnumerator<ILineStyle> GetEnumerator()
		{
			return Enumerator.Create(this);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Enumerator.Create(this);
		}

		#endregion

		private struct Enumerator : IEnumerator<ILineStyle>
		{
			public static readonly Enumerator Empty;

			public static Enumerator Create(LineStyleCollection collection)
			{
				if (collection == null) throw new ArgumentNullException("collection");
				Enumerator result = Enumerator.Empty;
				result.collection = collection;
				result.cnt = collection.Count;
				result.idx = -1;
				return result;
			}

			public Enumerator(LineStyleCollection collection)
			{
				if (collection == null) throw new ArgumentNullException("collection");
				this.collection = collection;
				this.cnt = collection.Count;
				this.idx = -1;
			}

			#region IEnumerator<ICapStyle> Members

			public ILineStyle Current
			{
				get { return collection[idx]; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				collection = null;
			}

			#endregion

			#region IEnumerator Members

			object IEnumerator.Current
			{
				get { return collection[idx]; }
			}

			public bool MoveNext()
			{
				return (++idx < cnt);
			}

			public void Reset()
			{
				idx = -1;
			}

			#endregion

			static Enumerator()
			{
				Empty.collection = null;
				Empty.cnt = 0;
				Empty.idx = -1;
			}

			private LineStyleCollection collection;
			private int idx;
			private int cnt;
		}
	}


	/// <summary>
	/// A collection of <see cref="T:Dataweb.NShape.ParagraphStyle" /> sorted by name.
	/// </summary>
	public class ParagraphStyleCollection : StyleCollection<ParagraphStyle>, IParagraphStyles
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.ParagraphStyleCollection" />.
		/// </summary>
		public ParagraphStyleCollection()
			: base(ParagraphStyle.StandardNames.Count)
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.ParagraphStyleCollection" />.
		/// </summary>
		public ParagraphStyleCollection(int capacity)
			: base(capacity)
		{
		}


		/// <override></override>
		public override bool IsStandardStyle(ParagraphStyle style)
		{
			if (style == null) throw new ArgumentNullException("style");
			return ParagraphStyle.StandardNames.EqualsAny(style.Name);
		}

		#region IParagraphStyles Members

		IParagraphStyle IParagraphStyles.this[string name]
		{
			get { return this[name]; }
		}

		/// <override></override>
		public IParagraphStyle Label
		{
			get { return internalList[ParagraphStyle.StandardNames.Label].Style; }
		}

		/// <override></override>
		public IParagraphStyle Text
		{
			get { return internalList[ParagraphStyle.StandardNames.Text].Style; }
		}

		/// <override></override>
		public IParagraphStyle Title
		{
			get { return internalList[ParagraphStyle.StandardNames.Title].Style; }
		}

		#endregion

		#region IEnumerable<IParagraphStyle> Members

		/// <override></override>
		public IEnumerator<IParagraphStyle> GetEnumerator()
		{
			return Enumerator.Create(this);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Enumerator.Create(this);
		}

		#endregion

		private struct Enumerator : IEnumerator<IParagraphStyle>
		{
			public static readonly Enumerator Empty;

			public static Enumerator Create(ParagraphStyleCollection collection)
			{
				if (collection == null) throw new ArgumentNullException("collection");
				Enumerator result = Enumerator.Empty;
				result.collection = collection;
				result.cnt = collection.Count;
				result.idx = -1;
				return result;
			}

			public Enumerator(ParagraphStyleCollection collection)
			{
				if (collection == null) throw new ArgumentNullException("collection");
				this.collection = collection;
				this.cnt = collection.Count;
				this.idx = -1;
			}

			#region IEnumerator<ICapStyle> Members

			public IParagraphStyle Current
			{
				get { return collection[idx]; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				collection = null;
			}

			#endregion

			#region IEnumerator Members

			object IEnumerator.Current
			{
				get { return collection[idx]; }
			}


			public bool MoveNext()
			{
				return (++idx < cnt);
			}


			public void Reset()
			{
				idx = -1;
			}

			#endregion

			static Enumerator()
			{
				Empty.collection = null;
				Empty.cnt = 0;
				Empty.idx = -1;
			}

			private ParagraphStyleCollection collection;
			private int idx;
			private int cnt;
		}
	}

	#endregion
}