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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.WinFormsUI
{
	[ToolboxItem(false)]
	internal partial class StyleListBox : ListBox
	{
		public StyleListBox()
		{
			// Initialize Components
			InitializeComponent();
			DoubleBuffered = true;
			Sorted = true;
			SetStyle(ControlStyles.ResizeRedraw, true);
			UpdateStyles();

			this.HorizontalScrollbar = true;
			this.maxWidthFieldInfo = typeof (ListBox).GetField("maxWidth", System.Reflection.BindingFlags.GetField
			                                                               | System.Reflection.BindingFlags.NonPublic
			                                                               | System.Reflection.BindingFlags.Instance);
			this.matrix = new Matrix();

			this.styleItemFormatter = new StringFormat(StringFormatFlags.NoWrap);
			this.styleItemFormatter.Trimming = StringTrimming.Character;
			this.styleItemFormatter.Alignment = StringAlignment.Near;
			this.styleItemFormatter.LineAlignment = StringAlignment.Center;

			this.specialItemFormatter = new StringFormat(StringFormatFlags.NoWrap);
			this.specialItemFormatter.Trimming = StringTrimming.Character;
			this.specialItemFormatter.Alignment = StringAlignment.Center;
			this.specialItemFormatter.LineAlignment = StringAlignment.Center;
		}


		public StyleListBox(IWindowsFormsEditorService editorService)
			: this()
		{
			if (editorService == null) throw new ArgumentNullException("editorService");
			this.editorService = editorService;
		}


		public StyleListBox(IStyleSet styleSet, Style style, bool showDefaultStyleItem, bool showOpenDesignerItem) :
			this()
		{
			if (styleSet == null) throw new ArgumentNullException("design");
			if (style == null) throw new ArgumentNullException("style");
			Initialize(styleSet, showDefaultStyleItem, showOpenDesignerItem, style.GetType(), style);
		}


		public StyleListBox(IWindowsFormsEditorService editorService, IStyleSetProvider styleSetProvider, IStyleSet styleSet,
		                    Style style, bool showDefaultStyleItem, bool showOpenDesignerItem)
			: this(editorService)
		{
			if (styleSetProvider == null) throw new ArgumentNullException("styleSetProvider");
			if (styleSet == null) throw new ArgumentNullException("styleSet");
			if (style == null) throw new ArgumentNullException("style");
			this.styleSetProvider = styleSetProvider;
			Initialize(styleSet, showDefaultStyleItem, showOpenDesignerItem, style.GetType(), style);
		}


		public StyleListBox(IWindowsFormsEditorService editorService, IStyleSetProvider styleSetProvider, IStyleSet styleSet,
		                    Type selectedStyleType, bool showDefaultStyleItem, bool showOpenDesignerItem)
			: this(editorService)
		{
			if (styleSetProvider == null) throw new ArgumentNullException("styleSetProvider");
			if (styleSet == null) throw new ArgumentNullException("styleSet");
			if (selectedStyleType == null) throw new ArgumentNullException("selectedStyleType");
			this.styleSetProvider = styleSetProvider;
			Initialize(styleSet, showDefaultStyleItem, showOpenDesignerItem, selectedStyleType, null);
		}


		public StyleListBox(IWindowsFormsEditorService editorService, IStyleSet styleSet, Style style,
		                    bool showDefaultStyleItem, bool showOpenDesignerItem)
			: this(editorService)
		{
			if (styleSet == null) throw new ArgumentNullException("styleSet");
			if (style == null) throw new ArgumentNullException("style");
			Initialize(styleSet, showDefaultStyleItem, showOpenDesignerItem, style.GetType(), style);
		}


		public StyleListBox(IWindowsFormsEditorService editorService, IStyleSet styleSet, Type selectedStyleType,
		                    bool showDefaultStyleItem, bool showOpenDesignerItem)
			: this(editorService)
		{
			if (styleSet == null) throw new ArgumentNullException("styleSet");
			if (selectedStyleType == null) throw new ArgumentNullException("selectedStyleType");
			Initialize(styleSet, showDefaultStyleItem, showOpenDesignerItem, selectedStyleType, null);
		}

		#region [Public] Properties and Events

		/// <summary>
		/// Specifies the version of the assembly containing the component.
		/// </summary>
		[Category("NShape")]
		[Browsable(true)]
		public new string ProductVersion
		{
			get { return base.ProductVersion; }
		}


		[Browsable(false)]
		public Style SelectedStyle
		{
			get { return base.SelectedItem as Style; }
			set { base.SelectedItem = value; }
		}


		[Browsable(false)]
		public StyleCategory StyleCategory
		{
			get { return styleCategory; }
			set
			{
				bool clearItems = styleCategory != value;
				if (clearItems) {
					SuspendLayout();
					Items.Clear();
				}

				styleCategory = value;
				if (Items.Count == 0)
					CreateListBoxItems();

				if (clearItems) ResumeLayout();
			}
		}


		[ReadOnly(true)]
		[Browsable(false)]
		public IStyleSet StyleSet
		{
			get { return styleSet; }
			set
			{
				if (styleSet != value) {
					styleSet = value;
					Items.Clear();
				}
			}
		}


		[Category("Behavior")]
		public bool HighlightItems
		{
			get { return highlightItems; }
			set { highlightItems = value; }
		}


		[Category("Appearance")]
		public Color ItemBackgroundColor
		{
			get { return itemBackgroundColor; }
			set
			{
				if (itemBackgroundBrush != null) {
					itemBackgroundBrush.Dispose();
					itemBackgroundBrush = null;
				}
				itemBackgroundColor = value;
			}
		}


		[Category("Appearance")]
		public Color ItemHighlightedColor
		{
			get { return itemHighlightedColor; }
			set
			{
				if (itemHighlightedBrush != null) {
					itemHighlightedBrush.Dispose();
					itemHighlightedBrush = null;
				}
				itemHighlightedColor = value;
			}
		}


		[Category("Appearance")]
		public Color ItemSelectedColor
		{
			get { return itemSelectedColor; }
			set
			{
				if (itemSelectedBrush != null) {
					itemSelectedBrush.Dispose();
					itemSelectedBrush = null;
				}
				itemSelectedColor = value;
			}
		}


		[Category("Appearance")]
		public Color ItemFocusedColor
		{
			get { return itemFocusedColor; }
			set
			{
				if (itemFocusedBrush != null) {
					itemFocusedBrush.Dispose();
					itemFocusedBrush = null;
				}
				itemFocusedColor = value;
			}
		}


		[Category("Appearance")]
		public Color FocusBorderColor
		{
			get { return focusBorderColor; }
			set
			{
				if (focusBorderPen != null) {
					focusBorderPen.Dispose();
					focusBorderPen = null;
				}
				focusBorderColor = value;
			}
		}


		[Category("Appearance")]
		public Color ItemBorderColor
		{
			get { return itemBorderColor; }
			set
			{
				if (itemBorderPen != null) {
					itemBorderPen.Dispose();
					itemBorderPen = null;
				}
				itemBorderColor = value;
			}
		}


		[Category("Appearance")]
		public Color TextColor
		{
			get { return textColor; }
			set
			{
				if (textBrush != null) {
					textBrush.Dispose();
					textBrush = null;
				}
				textColor = value;
			}
		}

		#endregion

		#region [Public] Methods

		public void Clear()
		{
			Items.Clear();
			SelectedItem = null;
		}


		public void UpdateDesign()
		{
			CreateListBoxItems();
		}

		#endregion

		#region [Protected] Methods (overridden)

		/// <summary>Sorts the items in the <see cref="T:System.Windows.Forms.ListBox"></see>.</summary>
		protected override void Sort()
		{
			if (defaultStyleItem == null && openDesignerItem == null)
				base.Sort();
			else {
				// ToDo: Optimize this later. 
				// No need to optimize this right now as runtime is fast enough at the moment.
				// CAUTION! Avoid the 'Quicksort' algorithm here because the list is pre-sorted
				// which means that Quicksort's run time will be nearly O(N²) instead of O(N log N).
				if (Items.Count > 1) {
					bool swapped;
					do {
						int index = Items.Count - 1;
						swapped = false;
						while (index > 0) {
							// Compare the items
							if (CompareItems(Items[index], Items[index - 1]) < 0) {
								// Swap the items.
								object temp = Items[index];
								Items[index] = Items[index - 1];
								Items[index - 1] = temp;
								swapped = true;
							}
							// Decrement the counter.
							index -= 1;
						}
					} while ((swapped == true));
				}
			}
		}


		/// <override></override>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if (e.KeyData == Keys.Return || e.KeyData == Keys.Space)
				ExecuteSelection();
		}


		/// <override></override>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if (e.Button == MouseButtons.Left) {
				for (int i = 0; i < Items.Count; ++i) {
					if (Geometry.RectangleContainsPoint(GetItemRectangle(i), e.Location)) {
						ExecuteSelection();
						break;
					}
				}
			}
		}


		/// <override></override>
		protected override void OnResize(EventArgs e)
		{
			UpdateMaxItemWidth(null);
			base.OnResize(e);
		}


		/// <override></override>
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// do nothing because of DoubleBuffering
			base.OnPaintBackground(e);
		}


		/// <override></override>
		protected override void OnPaint(PaintEventArgs e)
		{
			//base.OnPaintBackground(e);
			base.OnPaint(e);
		}


		/// <override></override>
		protected override void OnMeasureItem(MeasureItemEventArgs e)
		{
			if (Items.Count > 0) {
				UpdateMaxItemWidth(e.Graphics, e.Index);

				e.ItemWidth = Width;
				if (Items[e.Index] is IStyle) {
					switch (styleCategory) {
						case StyleCategory.CapStyle:
							e.ItemHeight = ((ICapStyle) Items[e.Index]).CapSize;
							break;
						case StyleCategory.ColorStyle:
						case StyleCategory.FillStyle:
							e.ItemHeight = stdItemHeight;
							break;
						case StyleCategory.CharacterStyle:
							ICharacterStyle characterStyle = (ICharacterStyle) Items[e.Index];
							Font font = ToolCache.GetFont(characterStyle);
							e.ItemHeight = (int) Math.Ceiling(font.GetHeight(e.Graphics));
							break;
						case StyleCategory.LineStyle:
							e.ItemHeight = ((ILineStyle) Items[e.Index]).LineWidth + 4;
							break;
						case StyleCategory.ParagraphStyle:
							e.ItemHeight = dblItemHeight + dblItemHeight;
							break;
						default:
							throw new NShapeException(string.Format("Unexpected enum value '{0}'.", styleCategory));
					}
				}
				// correct calculated Height by the Height of the label's font
				float fontSizeInPixels = Font.GetHeight(e.Graphics);
				if (fontSizeInPixels > e.ItemHeight)
					e.ItemHeight = (int) Math.Round(fontSizeInPixels);
				e.ItemHeight += 4;
				if (e.ItemHeight < stdItemHeight)
					e.ItemHeight = 20;
			}
		}


		/// <override></override>
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (maxItemTextWidth < 0) UpdateMaxItemWidth(e.Graphics);
			const int txtMargin = 4;

			itemBounds.X = e.Bounds.X + 3;
			itemBounds.Y = e.Bounds.Y + 1;
			itemBounds.Width = (e.Bounds.Right - 3) - (e.Bounds.X + 3);
			itemBounds.Height = (e.Bounds.Bottom - 1) - (e.Bounds.Y + 1);

			previewRect.X = itemBounds.X + margin;
			previewRect.Y = itemBounds.Y + margin;
			previewRect.Width = itemBounds.Width - Math.Max(maxItemTextWidth, itemBounds.Width/4) - (2*margin) - (2*txtMargin);
			previewRect.Height = (itemBounds.Bottom - margin) - (itemBounds.Y + margin);

			labelLayoutRect.X = previewRect.Right + txtMargin;
			labelLayoutRect.Y = previewRect.Y;
			labelLayoutRect.Width = maxItemTextWidth;
			labelLayoutRect.Height = previewRect.Height;

			// Draw Item Background and Border
			e.Graphics.FillRectangle(ItemBackgroundBrush, itemBounds);
			if (itemBorderColor != Color.Transparent)
				e.Graphics.DrawRectangle(ItemBorderPen, itemBounds);

			// Draw Selection and/or Focus markers
			if ((e.State & DrawItemState.Selected) != 0)
				e.Graphics.FillRectangle(ItemSelectedBrush, itemBounds);
			if ((e.State & DrawItemState.Focus) != 0) {
				if (itemFocusedColor != Color.Transparent)
					e.Graphics.FillRectangle(ItemFocusedBrush, itemBounds);
				if (FocusBorderColor != Color.Transparent)
					e.Graphics.DrawRectangle(FocusBorderPen, itemBounds);
			}
			else if (HighlightItems && (e.State & DrawItemState.HotLight) != 0)
				if (ItemHighlightedColor != Color.Transparent)
					e.Graphics.FillRectangle(ItemHighlightedBrush, itemBounds);

			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

			if (Items.Count > 0 && e.Index >= 0) {
				if (Items[e.Index] is IStyle) {
					switch (StyleCategory) {
						case StyleCategory.CapStyle:
							DrawCapStyleItem((CapStyle) Items[e.Index], e);
							break;
						case StyleCategory.ColorStyle:
							ColorStyle colorStyle = (ColorStyle) Items[e.Index];
							Brush colorBrush = ToolCache.GetBrush(colorStyle);
							e.Graphics.FillRectangle(colorBrush, previewRect);
							e.Graphics.DrawRectangle(ItemBorderPen, previewRect);
							e.Graphics.DrawRectangle(Pens.Black, previewRect);
							e.Graphics.DrawString(colorStyle.Title, e.Font, TextBrush, labelLayoutRect, styleItemFormatter);
							break;
						case StyleCategory.FillStyle:
							DrawFillStyleItem((FillStyle) Items[e.Index], e);
							break;
						case StyleCategory.CharacterStyle:
							CharacterStyle charStyle = (CharacterStyle) Items[e.Index];
							Font font = ToolCache.GetFont(charStyle);
							Brush fontBrush = ToolCache.GetBrush(charStyle.ColorStyle);
							e.Graphics.DrawString(string.Format("{0} {1} pt", font.FontFamily.Name, font.SizeInPoints), font, fontBrush,
							                      previewRect, styleItemFormatter);
							e.Graphics.DrawString(charStyle.Title, e.Font, TextBrush, labelLayoutRect, styleItemFormatter);
							break;
						case StyleCategory.LineStyle:
							LineStyle lineStyle = (LineStyle) Items[e.Index];
							Pen linePen = ToolCache.GetPen(lineStyle, null, null);
							e.Graphics.DrawLine(linePen, previewRect.X, previewRect.Y + (previewRect.Height/2), previewRect.Right,
							                    previewRect.Y + (previewRect.Height/2));
							e.Graphics.DrawString(lineStyle.Title, e.Font, TextBrush, labelLayoutRect, styleItemFormatter);
							break;
						case StyleCategory.ParagraphStyle:
							ParagraphStyle paragraphStyle = (ParagraphStyle) Items[e.Index];
							StringFormat stringFormat = ToolCache.GetStringFormat(paragraphStyle);
							Rectangle r = Rectangle.Empty;
							r.X = previewRect.Left + paragraphStyle.Padding.Left;
							r.Y = previewRect.Top + paragraphStyle.Padding.Top;
							r.Width = previewRect.Width - (paragraphStyle.Padding.Left + paragraphStyle.Padding.Right);
							r.Height = previewRect.Height - (paragraphStyle.Padding.Top + paragraphStyle.Padding.Bottom);
							e.Graphics.DrawString(previewText, e.Font, TextBrush, r, stringFormat);
							e.Graphics.DrawRectangle(Pens.Black, previewRect);
							e.Graphics.DrawString(paragraphStyle.Title, e.Font, TextBrush, labelLayoutRect, styleItemFormatter);
							break;
						default:
							throw new NShapeException(string.Format("Unexpected enum value '{0}'.", styleCategory));
					}
				}
				else
					e.Graphics.DrawString(Items[e.Index].ToString().Trim(), e.Font, TextBrush, e.Bounds, specialItemFormatter);
			}
		}

		#endregion

		#region [Private] Methods

		private void Initialize(IStyleSet styleSet, bool showDefaultStyleItem, bool showOpenDesignerItem, Type styleType,
		                        IStyle style)
		{
			this.styleSet = styleSet;
			if (showDefaultStyleItem)
				defaultStyleItem = new DefaultStyleItem();
			if (showOpenDesignerItem && styleSetProvider != null)
				openDesignerItem = new OpenDesignerItem();

			StyleCategoryFromType(styleType);
			if (style != null) SelectedItem = preselectedStyle = style;
		}


		private void StyleCategoryFromType(Type styleType)
		{
			if (styleType == typeof (CapStyle))
				StyleCategory = StyleCategory.CapStyle;
			else if (styleType == typeof (CharacterStyle))
				StyleCategory = StyleCategory.CharacterStyle;
			else if (styleType == typeof (ColorStyle))
				StyleCategory = StyleCategory.ColorStyle;
			else if (styleType == typeof (FillStyle))
				StyleCategory = StyleCategory.FillStyle;
			else if (styleType == typeof (LineStyle))
				StyleCategory = StyleCategory.LineStyle;
			else if (styleType == typeof (ParagraphStyle))
				StyleCategory = StyleCategory.ParagraphStyle;
			else
				throw new NShapeException("Type StyleListBox does not support Type {0}.", styleType.Name);
		}


		private void CreateListBoxItems()
		{
			SuspendLayout();
			Sorted = false;
			if (Items.Count > 0)
				Items.Clear();

			if (styleSet != null) {
				// Add special item "Default Style"
				if (defaultStyleItem != null) Items.Add(defaultStyleItem);

				// Add style items
				System.Collections.IEnumerator styleEnumerator;
				switch (styleCategory) {
					case StyleCategory.CapStyle:
						styleEnumerator = styleSet.CapStyles.GetEnumerator();
						break;
					case StyleCategory.CharacterStyle:
						styleEnumerator = styleSet.CharacterStyles.GetEnumerator();
						break;
					case StyleCategory.ColorStyle:
						styleEnumerator = styleSet.ColorStyles.GetEnumerator();
						break;
					case StyleCategory.FillStyle:
						styleEnumerator = styleSet.FillStyles.GetEnumerator();
						break;
					case StyleCategory.LineStyle:
						styleEnumerator = styleSet.LineStyles.GetEnumerator();
						break;
					case StyleCategory.ParagraphStyle:
						styleEnumerator = styleSet.ParagraphStyles.GetEnumerator();
						break;
					default:
						throw new NShapeException(string.Format("Unexpected enum value '{0}'.", StyleCategory));
				}
				while (styleEnumerator.MoveNext())
					Items.Add(styleEnumerator.Current);
				maxItemTextWidth = -1;

				// Add special item "More..."
				if (openDesignerItem != null) Items.Add(openDesignerItem);
			}
			Sorted = true;
			ResumeLayout();
			Invalidate();
		}


		private void ExecuteSelection()
		{
			if (SelectedItem is OpenDesignerItem) {
				if (styleSetProvider is Project) {
					// Try to find the main window (the application's first opened window)
					Form parentForm = (Application.OpenForms.Count > 0) ? Application.OpenForms[0] : null;
					// Show dialog with parent form or otherwise it will show in background
					DesignEditorDialog dlg = new DesignEditorDialog((Project) styleSetProvider, styleCategory);
					if (parentForm == null) dlg.Show();
					else dlg.Show(parentForm);
				}
				SelectedItem = preselectedStyle;
			}
			if (editorService != null)
				editorService.CloseDropDown();
			else {
				// Notify ToolCache about changed styles only if no EditorService is set.
				// Otherwise the ToolCache re-creates the associated brush, pen, etc 
				// each time the user selects a style for a shape in the property grid.
				if (SelectedStyle is CapStyle || SelectedStyle is LineStyle)
					ToolCache.NotifyStyleChanged(SelectedStyle);
			}
		}

		#endregion

		#region [Private] Properties: Pens and Brushes

		private Brush ItemBackgroundBrush
		{
			get
			{
				if (itemBackgroundBrush == null)
					itemBackgroundBrush = new SolidBrush(ItemBackgroundColor);
				return itemBackgroundBrush;
			}
		}


		private Brush ItemHighlightedBrush
		{
			get
			{
				if (itemHighlightedBrush == null)
					itemHighlightedBrush = new SolidBrush(itemHighlightedColor);
				return itemHighlightedBrush;
			}
		}


		private Brush ItemSelectedBrush
		{
			get
			{
				if (itemSelectedBrush == null)
					itemSelectedBrush = new SolidBrush(itemSelectedColor);
				return itemSelectedBrush;
			}
		}


		private Brush TextBrush
		{
			get
			{
				if (textBrush == null)
					textBrush = new SolidBrush(textColor);
				return textBrush;
			}
		}


		private Brush ItemFocusedBrush
		{
			get
			{
				if (itemFocusedBrush == null)
					itemFocusedBrush = new SolidBrush(ItemFocusedColor);
				return itemFocusedBrush;
			}
		}


		private Pen FocusBorderPen
		{
			get
			{
				if (focusBorderPen == null) {
					focusBorderPen = new Pen(focusBorderColor);
					focusBorderPen.Alignment = PenAlignment.Inset;
				}
				return focusBorderPen;
			}
		}


		private Pen ItemBorderPen
		{
			get
			{
				if (itemBorderPen == null) {
					itemBorderPen = new Pen(itemBorderColor);
					itemBorderPen.Alignment = PenAlignment.Inset;
				}
				return itemBorderPen;
			}
		}

		#endregion

		#region [Private] Methods: Measure Text, Draw FillStyle and CapStyle items

		private void UpdateMaxItemWidth(Graphics gfx, int index)
		{
			Size txtSize = Size.Empty;
			if (gfx == null)
				txtSize = TextMeasurer.MeasureText(Items[index].ToString(), Font, Size.Empty, StringFormat.GenericDefault);
			else txtSize = TextMeasurer.MeasureText(gfx, Items[index].ToString(), Font, Size.Empty, StringFormat.GenericDefault);
			//Size txtSize = TextRenderer.MeasureText(Items[index].ToString(), Font);
			if (txtSize.Width > maxItemTextWidth) maxItemTextWidth = txtSize.Width;
		}


		private void UpdateMaxItemWidth(Graphics gfx)
		{
			Size txtSize = Size.Empty;
			maxItemTextWidth = -1;
			for (int i = Items.Count - 1; i >= 0; --i)
				UpdateMaxItemWidth(gfx, i);
		}


		private void DrawFillStyleItem(IFillStyle fillStyle, DrawItemEventArgs e)
		{
			Brush fillBrush = ToolCache.GetBrush(fillStyle);
			// Transform
			if (fillBrush is LinearGradientBrush) {
				float srcGradLen = ((LinearGradientBrush) fillBrush).Rectangle.Width;
				//float dstGradLen = previewRect.Width / (float)Math.Cos(Geometry.DegreesToRadians(fillStyle.GradientAngle));
				float dstGradLen =
					(float) Math.Sqrt((previewRect.Width*previewRect.Width) + (previewRect.Height*previewRect.Height));
				float scale = dstGradLen/srcGradLen;
				((LinearGradientBrush) fillBrush).ResetTransform();
				((LinearGradientBrush) fillBrush).TranslateTransform(previewRect.X, previewRect.Y);
				((LinearGradientBrush) fillBrush).ScaleTransform(scale, scale);
				((LinearGradientBrush) fillBrush).RotateTransform(fillStyle.GradientAngle);
			}
			else if (fillBrush is TextureBrush) {
				float scaleX = (float) previewRect.Width/((TextureBrush) fillBrush).Image.Width;
				float scaleY = (float) previewRect.Height/((TextureBrush) fillBrush).Image.Height;
				((TextureBrush) fillBrush).ResetTransform();
				((TextureBrush) fillBrush).TranslateTransform(previewRect.X, previewRect.Y);
				((TextureBrush) fillBrush).ScaleTransform(scaleX, scaleY);
			}
			// Draw
			if (fillBrush != Brushes.Transparent)
				e.Graphics.FillRectangle(fillBrush, previewRect);
			e.Graphics.DrawRectangle(ItemBorderPen, previewRect);
			e.Graphics.DrawRectangle(Pens.Black, previewRect);
			e.Graphics.DrawString(fillStyle.Title, e.Font, TextBrush, labelLayoutRect, styleItemFormatter);
		}


		private void DrawCapStyleItem(ICapStyle capStyle, DrawItemEventArgs e)
		{
			ILineStyle lineStyle = styleSet.LineStyles.Normal;
			Pen capPen = ToolCache.GetPen(lineStyle, capStyle, capStyle);
			Brush capBrush = null;
			PointF[] capPoints = null;

			int left = previewRect.Left;
			int right = previewRect.Right;
			if (capPen.StartCap == LineCap.Custom) {
				if (capPen.CustomStartCap.BaseInset > 0) {
					left += (int) Math.Round(capStyle.CapSize - capPen.CustomStartCap.BaseInset);
					right -= (int) Math.Round(capStyle.CapSize - capPen.CustomEndCap.BaseInset);
				}
			}
			int y = previewRect.Y + (previewRect.Height/2);
			// Start Cap
			if (HasCustomLineCap(capStyle)) {
				capBrush = ToolCache.GetBrush(capStyle.ColorStyle, lineStyle);
				ToolCache.GetCapPoints(capStyle, styleSet.LineStyles.Normal, ref capPoints);
				float angle = Geometry.RadiansToDegrees(Geometry.Angle(left, y, right, y));
				matrix.Reset();
				matrix.Translate(left, y);
				matrix.Rotate(angle + 90);
				matrix.TransformPoints(capPoints);
				e.Graphics.FillPolygon(capBrush, capPoints, System.Drawing.Drawing2D.FillMode.Alternate);
			}
			// End Cap
			if (HasCustomLineCap(capStyle)) {
				capBrush = ToolCache.GetBrush(capStyle.ColorStyle, lineStyle);
				ToolCache.GetCapPoints(capStyle, styleSet.LineStyles.Normal, ref capPoints);
				float angle = Geometry.RadiansToDegrees(Geometry.Angle(right, y, left, y));
				matrix.Reset();
				matrix.Translate(right, y);
				matrix.Rotate(angle + 90);
				matrix.TransformPoints(capPoints);
				e.Graphics.FillPolygon(capBrush, capPoints, System.Drawing.Drawing2D.FillMode.Alternate);
			}
			// Draw
			e.Graphics.DrawLine(capPen, left, y, right, y);
			e.Graphics.DrawString(capStyle.Title, e.Font, TextBrush, labelLayoutRect, styleItemFormatter);
		}


		public bool HasCustomLineCap(ICapStyle capStyle)
		{
			if (capStyle == null) return false;
			return (capStyle.CapShape != CapShape.None
			        && capStyle.CapShape != CapShape.Round
			        && capStyle.CapShape != CapShape.Flat
			        && capStyle.CapShape != CapShape.Peak);
		}

		#endregion

		#region [Private] Methods: Custom item compare implementation

		private int CompareItems(object a, object b)
		{
			if (a == null) throw new ArgumentNullException("a");
			if (b == null) throw new ArgumentNullException("b");
			if (a is DefaultStyleItem)
				return (b is DefaultStyleItem) ? 0 : -1;
			else if (a is OpenDesignerItem) {
				return (b is OpenDesignerItem) ? 0 : 1;
			}
			else {
				if (b is DefaultStyleItem) return 1;
				else if (b is OpenDesignerItem) return -1;
				else return string.Compare(a.ToString(), b.ToString(), StringComparison.InvariantCultureIgnoreCase);
			}
		}

		#endregion

		private class DefaultStyleItem
		{
			public DefaultStyleItem()
			{
			}

			public override string ToString()
			{
				return defaultStyleItemText;
			}

			private const string defaultStyleItemText = "Default Style";
		}


		private class OpenDesignerItem
		{
			public OpenDesignerItem()
			{
			}

			public override string ToString()
			{
				return openDesignerItemText;
			}

			private const string openDesignerItemText = "More...";
		}


		static StyleListBox()
		{
			previewText = "This is the first line of the sample text."
			              + Environment.NewLine + "This is line 2 of the text."
			              + Environment.NewLine + "Line 3 of the text.";
		}

		#region Fields

		private const int margin = 2;
		private const int stdItemHeight = 20;
		private const int dblItemHeight = 40;

		private static readonly string previewText;

		private StyleCategory styleCategory;
		private IStyleSet styleSet;
		private IStyleSetProvider styleSetProvider;
		private IWindowsFormsEditorService editorService;
		private DefaultStyleItem defaultStyleItem;
		private OpenDesignerItem openDesignerItem;

		// Graphical stuff
		private bool highlightItems = false;
		private int maxItemTextWidth = -1;
		// Colors
		private Color itemBackgroundColor = Color.FromKnownColor(KnownColor.Window);
		private Color itemHighlightedColor = Color.FromKnownColor(KnownColor.HighlightText);
		private Color itemSelectedColor = Color.FromKnownColor(KnownColor.MenuHighlight);
		private Color textColor = Color.FromKnownColor(KnownColor.WindowText);
		private Color itemFocusedColor = Color.Transparent;
		private Color focusBorderColor = Color.Transparent;
		private Color itemBorderColor = Color.Transparent;
		// Pens and Brushes
		private Brush itemBackgroundBrush;
		private Brush itemHighlightedBrush;
		private Brush itemSelectedBrush;
		private Brush itemFocusedBrush;
		private Brush textBrush;
		private Pen itemBorderPen;
		private Pen focusBorderPen;
		// Buffers
		private IStyle preselectedStyle;
		private Matrix matrix;
		private StringFormat specialItemFormatter;
		private StringFormat styleItemFormatter;
		private Rectangle itemBounds = Rectangle.Empty;
		private Rectangle previewRect = Rectangle.Empty;
		private Rectangle labelLayoutRect = Rectangle.Empty;

		//private NotifyObjectCollection itemsCollection;
		private System.Reflection.FieldInfo maxWidthFieldInfo;

		#endregion
	}
}