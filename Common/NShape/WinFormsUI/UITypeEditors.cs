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
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Dataweb.NShape.Advanced;
using System.IO;


namespace Dataweb.NShape.WinFormsUI {

	#region TypeConverters

	/// <summary>
	/// Converts all types of System.String and collections of System.String to System.String and collections of System.String.
	/// </summary>
	public class TextTypeConverter : TypeConverter {

		/// <override></override>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string))
				return true;
			if (destinationType == typeof(string[]))
				return true;
			if (destinationType == typeof(IEnumerable<string>))
				return true;
			if (destinationType == typeof(IList<string>))
				return true;
			if (destinationType == typeof(IReadOnlyCollection<string>))
				return true;
			if (destinationType == typeof(ICollection<string>))
				return true;
			return base.CanConvertTo(context, destinationType);
		}


		/// <override></override>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string)) {
				if (value != null) {
					if (culture == null) culture = CultureInfo.CurrentCulture;
					string separator = culture.TextInfo.ListSeparator + " ";

					string result = string.Empty;
					foreach (string line in ((IEnumerable<string>)value)) {
						if (result.Length > 0) result += separator;
						result += line;
					}
					return result;
				}
			} else if (destinationType == typeof(string[]))
				return value as IEnumerable<string>;
			else if (destinationType == typeof(IEnumerable<string>))
				return value as IEnumerable<string>;
			else if (destinationType == typeof(IList<string>))
				return value as IEnumerable<string>;
			else if (destinationType == typeof(ICollection<string>))
				return value as IEnumerable<string>;
			return base.ConvertTo(context, culture, value, destinationType);
		}


		/// <override></override>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string))
				return true;
			if (sourceType == typeof(string[]))
				return true;
			if (sourceType == typeof(IEnumerable<string>))
				return true;
			if (sourceType == typeof(IList<string>))
				return true;
			if (sourceType == typeof(IReadOnlyCollection<string>))
				return true;
			if (sourceType == typeof(ICollection<string>))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}


		/// <override></override>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if (value == null) return null;
			List<string> lines = new List<string>();
			if (value is string) {
				if (culture == null) culture = CultureInfo.CurrentCulture;
				string separator = culture.TextInfo.ListSeparator + " ";
				lines.AddRange(((string)value).Split(new string[] { separator }, StringSplitOptions.None));
			} else if (value is string[]
				|| value is IEnumerable<string>)
				lines.AddRange((IEnumerable<string>)value);

			if (context.Instance is string)
				return ConvertTo(context, culture, lines, typeof(string));
			else if (context.Instance is string[])
				return lines.ToArray();
			else return lines;
		}

	}


	/// <summary>
	/// Converts a Dataweb.NShape.Advanced.NamedImage to a System.Drawing.Image type or a System.String.
	/// </summary>
	public class NamedImageTypeConverter : TypeConverter {

		/// <override></override>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string)) return true;
			if (destinationType == typeof(Image)) return true;
			if (destinationType == typeof(Bitmap)) return true;
			if (destinationType == typeof(Metafile)) return true;
			return base.CanConvertTo(context, destinationType);
		}


		/// <override></override>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (value != null && value is NamedImage) {
				NamedImage val = (NamedImage)value;
				if (destinationType == typeof(string))
					return val.Name;
				else if (destinationType == typeof(Bitmap))
					return (Bitmap)val.Image;
				else if (destinationType == typeof(Metafile))
					return (Metafile)val.Image;
				else if (destinationType == typeof(Image))
					return val.Image;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

	}


	/// <summary>
	/// Converts a Dataweb.NShape.IStyle type to a System.String type.
	/// </summary>
	public class StyleTypeConverter : TypeConverter {

		/// <override></override>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string)) return true;
			return base.CanConvertTo(context, destinationType);
		}


		/// <override></override>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string) && value is IStyle) return ((IStyle)value).Title;
			return base.ConvertTo(context, culture, value, destinationType);
		}

	}


	/// <summary>
	/// Converts a System.Drawing.FontFamily type to a System.String type.
	/// </summary>
	public class FontFamilyTypeConverter : TypeConverter {

		/// <override></override>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string)) return true;
			return base.CanConvertTo(context, destinationType);
		}


		/// <override></override>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string) && value != null)
				return ((FontFamily)value).Name;
			return base.ConvertTo(context, culture, value, destinationType);
		}

	}


	/// <summary>
	/// Converts a Dataweb.NShape.TextPadding type to a System.Windows.Forms.Padding or a System.String type and vice versa.
	/// </summary>
	public class TextPaddingTypeConverter : TypeConverter {

		/// <override></override>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return ((sourceType == typeof(string))
					|| (sourceType == typeof(Padding))
					|| base.CanConvertFrom(context, sourceType));
		}


		/// <override></override>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string)
				|| destinationType == typeof(Padding)
				|| destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
				return true;
			else return base.CanConvertTo(context, destinationType);
		}


		/// <override></override>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			TextPadding result = TextPadding.Empty;
			if (value is string) {
				string valueStr = value as string;
				if (valueStr == null) return base.ConvertFrom(context, culture, value);

				valueStr = valueStr.Trim();
				if (valueStr.Length == 0) return null;

				if (culture == null) culture = CultureInfo.CurrentCulture;
				char ch = culture.TextInfo.ListSeparator[0];
				string[] strArray = valueStr.Split(new char[] { ch });
				int[] numArray = new int[strArray.Length];
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
				for (int i = 0; i < numArray.Length; i++)
					numArray[i] = (int)converter.ConvertFromString(context, culture, strArray[i]);

				if (numArray.Length == 1)
					result.All = numArray[0];
				else if (numArray.Length == 4) {
					result.Left = numArray[0];
					result.Top = numArray[1];
					result.Right = numArray[2];
					result.Bottom = numArray[3];
				} else throw new ArgumentException();
			} else if (value is Padding) {
				Padding padding = (Padding)value;
				result.Left = padding.Left;
				result.Top = padding.Top;
				result.Right = padding.Right;
				result.Bottom = padding.Bottom;
			} else return base.ConvertFrom(context, culture, value);
			return result;
		}


		/// <override></override>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == null) throw new ArgumentNullException("destinationType");
			if (value is TextPadding) {
				if (destinationType == typeof(string)) {
					TextPadding txtPadding = (TextPadding)value;
					if (destinationType == typeof(string)) {
						if (culture == null) culture = CultureInfo.CurrentCulture;

						string separator = culture.TextInfo.ListSeparator + " ";
						TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
						string[] strArray = new string[4];
						strArray[0] = converter.ConvertToString(context, culture, txtPadding.Left);
						strArray[1] = converter.ConvertToString(context, culture, txtPadding.Top);
						strArray[2] = converter.ConvertToString(context, culture, txtPadding.Right);
						strArray[3] = converter.ConvertToString(context, culture, txtPadding.Bottom);
						return string.Join(separator, strArray);
					}
					if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)) {
						if (txtPadding.All < 0) {
							return new System.ComponentModel.Design.Serialization.InstanceDescriptor(
								typeof(TextPadding).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }),
									new object[] { txtPadding.Left, txtPadding.Top, txtPadding.Right, txtPadding.Bottom });
						} else {
							return new System.ComponentModel.Design.Serialization.InstanceDescriptor(
								typeof(TextPadding).GetConstructor(new Type[] { typeof(int) }), new object[] { txtPadding.All }
							);
						}
					}
				} else if (destinationType == typeof(Padding)) {
					Padding paddingResult = Padding.Empty;
					if (value != null) {
						TextPadding val = (TextPadding)value;
						paddingResult.Left = val.Left;
						paddingResult.Top = val.Top;
						paddingResult.Right = val.Right;
						paddingResult.Bottom = val.Bottom;
					}
					return paddingResult;
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}


		/// <override></override>
		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues) {
			if (context == null) throw new ArgumentNullException("context");
			if (propertyValues == null) throw new ArgumentNullException("propertyValues");

			TextPadding txtPadding = (TextPadding)context.PropertyDescriptor.GetValue(context.Instance);
			TextPadding result = TextPadding.Empty;
			int all = (int)propertyValues["All"];
			if (txtPadding.All != all) result.All = all;
			else {
				result.Left = (int)propertyValues["Left"];
				result.Top = (int)propertyValues["Top"];
				result.Right = (int)propertyValues["Right"];
				result.Bottom = (int)propertyValues["Bottom"];
			}
			return result;
		}


		/// <override></override>
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return true;
		}


		/// <override></override>
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return TypeDescriptor.GetProperties(typeof(TextPadding), attributes).Sort(
				new string[] { "All", "Left", "Top", "Right", "Bottom" }
			);
		}


		/// <override></override>
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}

	}

	#endregion


	#region UITypeEditors

	/// <summary>
	/// NShape UI type editor for choosing a character style's font from a drop down list.
	/// </summary>
	public class FontFamilyUITypeEditor : UITypeEditor {

		/// <override></override>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (context != null && context.Instance != null && provider != null) {
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) {
					using (FontFamilyListBox listBox = new FontFamilyListBox(edSvc)) {
						listBox.BorderStyle = BorderStyle.None;
						listBox.IntegralHeight = false;
						listBox.Items.Clear();

						FontFamily[] families = FontFamily.Families;
						int cnt = families.Length;
						for (int i = 0; i < cnt; ++i) {
							listBox.Items.Add(families[i].Name);
							if (families[i] == value)
								listBox.SelectedIndex = listBox.Items.Count - 1;
						}
						edSvc.DropDownControl(listBox);
						if (listBox.SelectedItem != null)
							value = listBox.SelectedItem.ToString();
					}
				}
			}
			return value;
		}


		/// <override></override>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null)
				return UITypeEditorEditStyle.DropDown;
			return base.GetEditStyle(context);
		}


		/// <override></override>
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return false;
		}


		/// <override></override>
		public override void PaintValue(PaintValueEventArgs e) {
			if (e != null && e.Value != null) {
				if (formatter.Alignment != StringAlignment.Center) formatter.Alignment = StringAlignment.Center;
				if (formatter.LineAlignment != StringAlignment.Near) formatter.LineAlignment = StringAlignment.Near;

				GdiHelpers.ApplyGraphicsSettings(e.Graphics, RenderingQuality.HighQuality);
				using (Font font = new Font(e.Value.ToString(), e.Bounds.Height, FontStyle.Regular, GraphicsUnit.Pixel))
					e.Graphics.DrawString(e.Value.ToString(), font, Brushes.Black, (RectangleF)e.Bounds, formatter);
				GdiHelpers.ApplyGraphicsSettings(e.Graphics, RenderingQuality.DefaultQuality);
				
				base.PaintValue(e);
			}
		}


		/// <override></override>
		public override bool IsDropDownResizable {
			get { return true; }
		}

		StringFormat formatter = new StringFormat();
	}


	/// <summary>
	/// NShape UI type editor for adding/removing shapes to/from layers.
	/// </summary>
	public class LayerUITypeEditor : UITypeEditor {

		/// <override></override>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (context != null && context.Instance != null && provider != null) {
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null && context.Instance is Shape) {
					Shape shape = (Shape)context.Instance;
					using (CheckedListBox listBox = new CheckedListBox()) {
						listBox.BorderStyle = BorderStyle.None;
						listBox.IntegralHeight = false;
						listBox.Items.Clear();

						// Add existing layers and check shape's layers
						foreach (Layer l in shape.Diagram.Layers) {
							int idx = listBox.Items.Count;
							listBox.Items.Insert(idx, l);
							listBox.SetItemChecked(idx, ((shape.Layers & l.Id) != 0));
						}

						edSvc.DropDownControl(listBox);
						LayerIds shapeLayers = LayerIds.None;
						for (int i = listBox.Items.Count - 1; i >= 0; --i) {
							if (listBox.GetItemChecked(i))
								shapeLayers |= ((Layer)listBox.Items[i]).Id;
						}
						shape.Diagram.RemoveShapeFromLayers(shape, LayerIds.All);
						shape.Diagram.AddShapeToLayers(shape, shapeLayers);

						value = shapeLayers;
					}
				}
			}
			return value;
		}


		/// <override></override>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null)
				return UITypeEditorEditStyle.DropDown;
			return base.GetEditStyle(context);
		}


		/// <override></override>
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return false;
		}


		///// <override></override>
		//public override void PaintValue(PaintValueEventArgs e) {
		//    if (e != null && e.Value != null) {
		//        // store original values;
		//        SmoothingMode origSmoothingMode = e.Graphics.SmoothingMode;
		//        CompositingQuality origCompositingQuality = e.Graphics.CompositingQuality;
		//        InterpolationMode origInterpolationmode = e.Graphics.InterpolationMode;
		//        System.Drawing.Text.TextRenderingHint origTextRenderingHint = e.Graphics.TextRenderingHint;
		//        Matrix origTransform = e.Graphics.Transform;

		//        // set new GraphicsModes
		//        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
		//        e.Graphics.CompositingQuality = CompositingQuality.HighQuality;	// CAUTION: Slows down older machines!!
		//        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

		//        StringFormat formatter = new StringFormat();
		//        formatter.Alignment = StringAlignment.Center;
		//        formatter.LineAlignment = StringAlignment.Near;

		//        Font font = new Font(e.Value.ToString(), e.Bounds.Height, FontStyle.Regular, GraphicsUnit.Pixel);
		//        e.Graphics.DrawString(e.Value.ToString(), font, Brushes.Black, (RectangleF)e.Bounds, formatter);

		//        font.Dispose();
		//        formatter.Dispose();

		//        // restore original values
		//        e.Graphics.Transform = origTransform;
		//        e.Graphics.SmoothingMode = origSmoothingMode;
		//        e.Graphics.CompositingQuality = origCompositingQuality;
		//        e.Graphics.InterpolationMode = origInterpolationmode;
		//        e.Graphics.TextRenderingHint = origTextRenderingHint;

		//        base.PaintValue(e);
		//    }
		//}


		/// <override></override>
		public override bool IsDropDownResizable {
			get { return true; }
		}

	}


	/// <summary>
	/// NShape UI type editor for editing properties of type System.String or a System.String collection type.
	/// </summary>
	public class TextUITypeEditor : UITypeEditor {

		/// <override></override>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (context != null && context.Instance != null && provider != null) {
				if (context.PropertyDescriptor.PropertyType == typeof(string)) {
					string valueStr = string.IsNullOrEmpty((string)value) ? string.Empty : (string)value;
					using (TextEditorDialog stringEditor = new TextEditorDialog(valueStr)) {
						if (stringEditor.ShowDialog() == DialogResult.OK)
							value = stringEditor.ResultText;
					}
				} else if (context.PropertyDescriptor.PropertyType == typeof(string[])) {
					string[] valueArr = (value != null) ? (string[])value : new string[0];
					using (TextEditorDialog stringEditor = new TextEditorDialog(valueArr)) {
						if (stringEditor.ShowDialog() == DialogResult.OK)
							value = stringEditor.ResultText;
					}
				} else if (context.PropertyDescriptor.PropertyType == typeof(IEnumerable<string>)
					|| context.PropertyDescriptor.PropertyType == typeof(IList<string>)
					|| context.PropertyDescriptor.PropertyType == typeof(ICollection<string>)
					|| context.PropertyDescriptor.PropertyType == typeof(List<string>)) {
					IEnumerable<string> values = (value != null) ? (IEnumerable<string>)value : new string[0];
					using (TextEditorDialog stringEditor = new TextEditorDialog(values)) {
						if (stringEditor.ShowDialog() == DialogResult.OK)
							value = new List<string>(stringEditor.Lines);
					}
				}
			}
			return value;
		}


		/// <override></override>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null)
				return UITypeEditorEditStyle.Modal;
			else return base.GetEditStyle(context);
		}

	}


	/// <summary>
	/// NShape UI type editor for choosing an image and assinging a name.
	/// </summary>
	public class NamedImageUITypeEditor : UITypeEditor {

		/// <override></override>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (context != null && context.Instance != null && provider != null) {
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) {
					ImageEditor imageEditor = null;
					try {
						if (value is NamedImage && value != null)
							imageEditor = new ImageEditor((NamedImage)value);
						else imageEditor = new ImageEditor();

						if (imageEditor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
							value = imageEditor.Result;
					} finally {
						if (imageEditor != null) imageEditor.Dispose();
						imageEditor = null;
					}
				}
			}
			return value;
		}


		/// <override></override>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null)
				return UITypeEditorEditStyle.Modal;
			return base.GetEditStyle(context);
		}


		/// <override></override>
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return true;
		}


		/// <override></override>
		public override void PaintValue(PaintValueEventArgs e) {
			base.PaintValue(e);
			if (e != null && e.Value is NamedImage) {
				NamedImage img = (NamedImage)e.Value;
				if (img.Image != null) {
					Rectangle srcRect = Rectangle.Empty;
					srcRect.X = 0;
					srcRect.Y = 0;
					srcRect.Width = img.Width;
					srcRect.Height = img.Height;

					float lowestRatio = (float)Math.Round(Math.Min((double)(e.Bounds.Width - (e.Bounds.X + e.Bounds.X)) / (double)img.Width, (double)(e.Bounds.Height - (e.Bounds.Y + e.Bounds.Y)) / (double)img.Height), 6);
					Rectangle dstRect = Rectangle.Empty;
					dstRect.Width = (int)Math.Round(srcRect.Width * lowestRatio);
					dstRect.Height = (int)Math.Round(srcRect.Height * lowestRatio);
					dstRect.X = e.Bounds.X + (int)Math.Round((float)(e.Bounds.Width - dstRect.Width) / 2);
					dstRect.Y = e.Bounds.Y + (int)Math.Round((float)(e.Bounds.Height - dstRect.Height) / 2);

					// Apply HighQuality rendering settings to avoid false-color images when using
					// certain image formats (e.g. JPG with 24 bits color depth) on x64 OSes
					// Revert to default settings afterwards in order to avoid other graphical glitches
					GdiHelpers.ApplyGraphicsSettings(e.Graphics, RenderingQuality.HighQuality);
					e.Graphics.DrawImage(img.Image, dstRect, srcRect, GraphicsUnit.Pixel);
					GdiHelpers.ApplyGraphicsSettings(e.Graphics, RenderingQuality.DefaultQuality);
				}
			}
		}

	}


	/// <summary>
	/// NShape UI type editor for choosing a directory.
	/// </summary>
	public class DirectoryUITypeEditor : UITypeEditor {

		/// <override></override>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (context != null && context.Instance != null && provider != null) {
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) {
					using (FolderBrowserDialog dlg = new FolderBrowserDialog()) {
						if (value is DirectoryInfo)
							dlg.SelectedPath = ((DirectoryInfo)value).FullName;
						else if (value is string)
							dlg.SelectedPath = (string)value;
						dlg.ShowNewFolderButton = true;

						if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
							value = dlg.SelectedPath;
					}
				}
			}
			return value;
		}


		/// <override></override>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null)
				return UITypeEditorEditStyle.Modal;
			return base.GetEditStyle(context);
		}


		/// <override></override>
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return false;
		}

	}


	/// <summary>
	/// NShape UI type editor for choosing a style from a drop down list.
	/// </summary>
	public class StyleUITypeEditor : UITypeEditor {

		/// <summary>
		/// The <see cref="T:Dataweb.NShape.Project" /> providing the <see cref="T:Dataweb.NShape.Design" /> for the <see cref="T:Dataweb.NShape.WinFormsUI.StyleUITypeEditor" />.
		/// </summary>
		public static Project Project {
			set {
				if (projectBuffer != value) {
					projectBuffer = value;
					designBuffer = projectBuffer.Design;
				} else {
					bool replaceDesign = (designBuffer == null);
					if (!replaceDesign) {
						replaceDesign = true;
						// Ensure that the current design is part in the repository:
						foreach (Design design in projectBuffer.Repository.GetDesigns()) {
							if (design == designBuffer) {
								replaceDesign = false;
								break;
							}
						}
					}
					if (replaceDesign) designBuffer = projectBuffer.Design;
				}
			}
		}


		/// <summary>
		/// The design providing styles for the <see cref="T:Dataweb.NShape.WinFormsUI.StyleUITypeEditor" />.
		/// </summary>
		public static Design Design {
			set { designBuffer = value; }
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.StyleUITypeEditor" />.
		/// </summary>
		public StyleUITypeEditor() {
			if (designBuffer == null) {
				string msg = string.Format("{0} is not set. Set the static property {1}.Design to a reference of the current design before creating the UI type editor.", typeof(Design).Name, GetType().Name);
				throw new NShapeInternalException(msg);
			}
			this.project = projectBuffer;
			this.design = designBuffer;
		}


		/// <override></override>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (provider != null) {
#if DEBUG
				if (!(context.PropertyDescriptor is PropertyDescriptorDg))
					System.Diagnostics.Debug.Print("### The given PropertyDescriptor for {0} is not of type {1}.", value, typeof(PropertyDescriptorDg).Name);
				else System.Diagnostics.Debug.Print("### PropertyDescriptor is of type {1}.", value, typeof(PropertyDescriptorDg).Name);
#endif

				IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (editorService != null) {
					// fetch current Design (if changed)
					if (designBuffer != null && designBuffer != design)
						design = designBuffer;

					// Examine edited instances and determine whether the list item "Default Style" should be displayed.
					bool showItemDefaultStyle = false;
					bool showItemOpenEditor = false;
					if (context.Instance is Shape) {
						showItemDefaultStyle = ((Shape)context.Instance).Template != null;
						showItemOpenEditor = project.SecurityManager.IsGranted(Permission.Designs);
					} else if (context.Instance is object[]) {
						object[] objArr = (object[])context.Instance;
						int cnt = objArr.Length;
						showItemDefaultStyle = true;
						showItemOpenEditor = project.SecurityManager.IsGranted(Permission.Designs);
						for (int i = 0; i < cnt; ++i) {
							Shape shape = objArr[i] as Shape;
							if (shape == null || shape.Template == null) {
								showItemDefaultStyle = false;
								showItemOpenEditor = false;
								break;
							}
						}
					}

					StyleListBox styleListBox = null;
					try {
						Type styleType = null;
						if (value == null) {
							if (context.PropertyDescriptor.PropertyType == typeof(ICapStyle))
								styleType = typeof(CapStyle);
							else if (context.PropertyDescriptor.PropertyType == typeof(IColorStyle))
								styleType = typeof(ColorStyle);
							else if (context.PropertyDescriptor.PropertyType == typeof(IFillStyle))
								styleType = typeof(FillStyle);
							else if (context.PropertyDescriptor.PropertyType == typeof(ICharacterStyle))
								styleType = typeof(CharacterStyle);
							else if (context.PropertyDescriptor.PropertyType == typeof(ILineStyle))
								styleType = typeof(LineStyle);
							else if (context.PropertyDescriptor.PropertyType == typeof(IParagraphStyle))
								styleType = typeof(ParagraphStyle);
							else throw new NShapeUnsupportedValueException(context.PropertyDescriptor.PropertyType);

							if (project != null)
								styleListBox = new StyleListBox(editorService, project, design, styleType, showItemDefaultStyle, showItemOpenEditor);
							else styleListBox = new StyleListBox(editorService, design, styleType, showItemDefaultStyle, showItemOpenEditor);
						} else {
							if (project != null)
								styleListBox = new StyleListBox(editorService, project, design, value as Style, showItemDefaultStyle, showItemOpenEditor);
							else styleListBox = new StyleListBox(editorService, design, value as Style, showItemDefaultStyle, showItemOpenEditor);
						}

						editorService.DropDownControl(styleListBox);
						if (styleListBox.SelectedItem is IStyle)
							value = styleListBox.SelectedItem;
						else value = null;
					} finally {
						if (styleListBox != null) styleListBox.Dispose();
						styleListBox = null;
					}
				}
			}
			return value;
		}


		/// <override></override>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null)
				return UITypeEditorEditStyle.DropDown;
			else return base.GetEditStyle(context);
		}


		/// <override></override>
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return true;
		}


		/// <override></override>
		public override void PaintValue(PaintValueEventArgs e) {
			//base.PaintValue(e);
			if (e != null && e.Value != null) {
				Rectangle previewRect = Rectangle.Empty;
				previewRect.X = e.Bounds.X + 1;
				previewRect.Y = e.Bounds.Y + 1;
				previewRect.Width = (e.Bounds.Right - 1) - (e.Bounds.Left + 1);
				previewRect.Height = (e.Bounds.Bottom - 1) - (e.Bounds.Top + 1);

				// Set new GraphicsModes
				Matrix origTransform = e.Graphics.Transform;
				GdiHelpers.ApplyGraphicsSettings(e.Graphics, RenderingQuality.HighQuality);
				
				// Draw value
				if (e.Value is ICapStyle) 
					DrawStyleItem(e.Graphics, previewRect, (ICapStyle)e.Value);
				else if (e.Value is ICharacterStyle)
					DrawStyleItem(e.Graphics, previewRect, (ICharacterStyle)e.Value);
				else if (e.Value is IColorStyle)
					DrawStyleItem(e.Graphics, previewRect, (IColorStyle)e.Value);
				else if (e.Value is IFillStyle)
					DrawStyleItem(e.Graphics, previewRect, (IFillStyle)e.Value);
				else if (e.Value is ILineStyle)
					DrawStyleItem(e.Graphics, previewRect, (ILineStyle)e.Value);
				else if (e.Value is IParagraphStyle)
					DrawStyleItem(e.Graphics, previewRect, (IParagraphStyle)e.Value);

				// Restore original values
				GdiHelpers.ApplyGraphicsSettings(e.Graphics, RenderingQuality.DefaultQuality);
				e.Graphics.Transform = origTransform;
			}
		}


		/// <override></override>
		public override bool IsDropDownResizable {
			get { return true; }
		}


		private void DrawStyleItem(Graphics gfx, Rectangle previewBounds, ICapStyle capStyle) {
			Pen capPen = ToolCache.GetPen(design.LineStyles.Normal, null, capStyle);
			float scale = 1;
			if (capStyle.CapSize + 2 >= previewBounds.Height) {
				scale = Geometry.CalcScaleFactor(capStyle.CapSize + 2, capStyle.CapSize + 2, previewBounds.Width - 2, previewBounds.Height - 2);
				gfx.ScaleTransform(scale, scale);
			}
			int startX, endX, y;
			startX = previewBounds.Left;
			if (capPen.StartCap == LineCap.Custom)
				startX += (int)Math.Round(capStyle.CapSize - capPen.CustomStartCap.BaseInset);
			startX = (int)Math.Round(startX / scale);
			endX = previewBounds.Right;
			if (capPen.EndCap == LineCap.Custom)
				endX -= (int)Math.Round(capStyle.CapSize - capPen.CustomEndCap.BaseInset);
			endX = (int)Math.Round(endX / scale);
			y = (int)Math.Round((previewBounds.Y + ((float)previewBounds.Height / 2)) / scale);
			gfx.DrawLine(capPen, startX, y, endX, y);
		}


		private void DrawStyleItem(Graphics gfx, Rectangle previewBounds, ICharacterStyle charStyle) {
			Brush fontBrush = ToolCache.GetBrush(charStyle.ColorStyle);
			Font font = ToolCache.GetFont(charStyle);
			int height = Geometry.PointToPixel(charStyle.SizeInPoints, gfx.DpiY);
			float scale = Geometry.CalcScaleFactor(height, height, previewBounds.Width, previewBounds.Height);
			gfx.ScaleTransform(scale, scale);
			RectangleF layoutRect = RectangleF.Empty;
			layoutRect.X = 0;
			layoutRect.Y = 0;
			layoutRect.Width = (float)previewBounds.Width / scale;
			layoutRect.Height = (float)previewBounds.Height / scale;
			gfx.DrawString(string.Format("{0} {1} pt", charStyle.FontName, charStyle.SizeInPoints), 
				font, fontBrush, layoutRect, formatter);
		}


		private void DrawStyleItem(Graphics gfx, Rectangle previewBounds, IColorStyle colorStyle) {
			Brush brush = ToolCache.GetBrush(colorStyle);
			gfx.FillRectangle(brush, previewBounds);
		}


		private void DrawStyleItem(Graphics gfx, Rectangle previewBounds, IFillStyle fillStyle) {
			Brush fillBrush = ToolCache.GetBrush(fillStyle);
			// Transform
			if (fillBrush is LinearGradientBrush) {
				float srcGradLen = ((LinearGradientBrush)fillBrush).Rectangle.Width;
				float dstGradLen = previewBounds.Width / (float)Math.Cos(Geometry.DegreesToRadians(fillStyle.GradientAngle));
				float scale = dstGradLen / srcGradLen;
				((LinearGradientBrush)fillBrush).ResetTransform();
				((LinearGradientBrush)fillBrush).ScaleTransform(scale, scale);
				((LinearGradientBrush)fillBrush).RotateTransform(fillStyle.GradientAngle);
			} else if (fillBrush is TextureBrush) {
				if (fillStyle.ImageLayout == ImageLayoutMode.Stretch) {
					float scaleX = (float)previewBounds.Width / ((TextureBrush)fillBrush).Image.Width;
					float scaleY = (float)previewBounds.Height / ((TextureBrush)fillBrush).Image.Height;
					((TextureBrush)fillBrush).ScaleTransform(scaleX, scaleY);
				} else {
					float scale = Geometry.CalcScaleFactor(((TextureBrush)fillBrush).Image.Width, ((TextureBrush)fillBrush).Image.Height, previewBounds.Width, previewBounds.Height);
					((TextureBrush)fillBrush).ScaleTransform(scale, scale);
					((TextureBrush)fillBrush).TranslateTransform((((TextureBrush)fillBrush).Image.Width * scale) / 2, (((TextureBrush)fillBrush).Image.Height * scale) / 2);
				}
			}
			// Draw
			if (fillBrush != Brushes.Transparent) gfx.FillRectangle(fillBrush, previewBounds);
		}


		private void DrawStyleItem(Graphics gfx, Rectangle previewBounds, ILineStyle lineStyle) {
			Pen linePen = ToolCache.GetPen(lineStyle, null, null);
			int height = lineStyle.LineWidth + 2;
			bool scalePen = (height > previewBounds.Height);
			if (scalePen) {
				float scale = Geometry.CalcScaleFactor(height, height, previewBounds.Width, previewBounds.Height);
				linePen.ScaleTransform(scale, scale);
			}
			gfx.DrawLine(linePen, previewBounds.X, previewBounds.Y + (previewBounds.Height / 2), previewBounds.Right, previewBounds.Y + (previewBounds.Height / 2));
			if (scalePen) linePen.ResetTransform();
		}


		private void DrawStyleItem(Graphics gfx, Rectangle previewBounds, IParagraphStyle paragraphStyle) {
			StringFormat stringFormat = ToolCache.GetStringFormat(paragraphStyle);
			Rectangle r = Rectangle.Empty;
			r.X = previewBounds.X + paragraphStyle.Padding.Left;
			r.Y = previewBounds.Y + paragraphStyle.Padding.Top;
			r.Width = (previewBounds.Width * 10) - (paragraphStyle.Padding.Left + paragraphStyle.Padding.Right);
			r.Height = (previewBounds.Height * 10) - (paragraphStyle.Padding.Top + paragraphStyle.Padding.Bottom);

			// Transform
			float scale = Geometry.CalcScaleFactor(r.Width, r.Height, previewBounds.Width, previewBounds.Height);
			gfx.ScaleTransform(scale, scale);

			// Draw
			gfx.DrawString(previewText, Control.DefaultFont, Brushes.Black, r, stringFormat);
		}


		static StyleUITypeEditor() {
			previewText = "This is the first line of the sample text."
				+ Environment.NewLine + "This is line 2 of the text."
				+ Environment.NewLine + "Line 3 of the text.";
		}


		#region Fields

		private static Project projectBuffer;
		private static Design designBuffer;
		private static readonly string previewText;
		private Project project;
		private Design design;
		private StringFormat formatter = new StringFormat(StringFormatFlags.NoWrap);

		#endregion
	}

	#endregion

}
