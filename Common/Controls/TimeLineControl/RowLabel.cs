using Common.Controls.Theme;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Common.Controls.Timeline
{
	[System.ComponentModel.DesignerCategory("")] // Prevent this from showing up in designer.
	public class RowLabel : UserControl
	{
		private const int LableFontHeight = 12;

		public RowLabel(Row parentRow)
			: this()
		{
			ParentRow = parentRow;
		}

		public RowLabel()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			ResizeBarWidth = 4;
			Resizing = false;
		}

		#region Properties

		private Row m_parentRow;

		public Row ParentRow
		{
			get { return m_parentRow; }
			set { m_parentRow = value; }
		}

		private static int m_toggleTreeButtonWidth = 28;

		public static int ToggleTreeButtonWidth
		{
			get { return m_toggleTreeButtonWidth; }
			set { m_toggleTreeButtonWidth = value; }
		}

		protected Rectangle IconArea { get; set; }
		protected Rectangle LabelArea { get; set; }
		protected Rectangle IndicatorArea { get; set; }
		protected int ResizeBarWidth { get; set; }
		protected bool Resizing { get; set; }
		protected Point LastMouseLocation { get; set; }

		#endregion

		#region Events

		internal event EventHandler TreeToggled;
		internal event EventHandler<ModifierKeysEventArgs> LabelClicked;
		internal event EventHandler<RowHeightChangedEventArgs> HeightChanged;
		internal event EventHandler HeightResized;
		internal event EventHandler RowContextMenuSelect;

		private void _TreeToggled()
		{
			if (TreeToggled != null) TreeToggled(this, EventArgs.Empty);
		}

		private void _LabelClicked(Keys k)
		{
			if (LabelClicked != null) LabelClicked(this, new ModifierKeysEventArgs(k));
		}

		private void _HeightChanged(int dh)
		{
			if (HeightChanged != null) HeightChanged(this, new RowHeightChangedEventArgs(dh));
		}

		private void _HeightResized()
		{
			if (HeightResized != null) HeightResized(this, EventArgs.Empty);
		}

		private void _RowContextMenuSelect()
		{
			if (RowContextMenuSelect != null) RowContextMenuSelect(this, EventArgs.Empty);
		}

		#endregion

		#region Event Handlers

		private bool MousePosContainsResizeBar(MouseEventArgs e)
		{
			if (LabelArea.Contains(e.Location) && e.Y > LabelArea.Height - ResizeBarWidth)
				return true;
			else
				return false;
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);

			if (MousePosContainsResizeBar(e))
				return;


			if ((ParentRow.ChildRows.Count > 0) && IconArea.Contains(e.Location)) {
				// if it's within the toggle button, toggle the tree
				_TreeToggled();
				Invalidate();
			}
			else if (LabelArea.Contains(e.Location) || ((ParentRow.ChildRows.Count == 0) && IconArea.Contains(e.Location))) {
				_LabelClicked(Form.ModifierKeys);
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.Button == MouseButtons.Left) {
				if (MousePosContainsResizeBar(e)) {
					Resizing = true;
					LastMouseLocation = e.Location;
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if (e.Button == MouseButtons.Left) {
				if (Resizing)
				{
					_HeightResized();
				}
				Resizing = false;
			}
			if (e.Button == MouseButtons.Right)
			{
				if (LabelArea.Contains(e.Location) || ((ParentRow.ChildRows.Count == 0) && IconArea.Contains(e.Location)))
				{
					_LabelClicked(Form.ModifierKeys);
				}
				_RowContextMenuSelect();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (MousePosContainsResizeBar(e))
				this.Cursor = Cursors.HSplit;
			else
				this.Cursor = Cursors.Default;

			if (Resizing) {
				int dy = e.Y - LastMouseLocation.Y;
				LastMouseLocation = e.Location;
				_HeightChanged(dy);
			}
		}

		#endregion

		#region Methods

		public Func<bool> ChildActiveIndicator = () => false;

		public Func<bool> ActiveIndicator = () => false;

		#endregion

		#region Drawing

		protected override void OnPaint(PaintEventArgs e)
		{
			using (SolidBrush backgroundBrush = new SolidBrush(ThemeColorTable.TimeLineLabelBackColor))
			{
				using (SolidBrush textBrush = new SolidBrush(ActiveIndicator()?ThemeColorTable.ActiveColor:ThemeColorTable.TimeLineForeColor))
				{
					using (Pen wholeBorderPen = new Pen(ThemeColorTable.TimeLineGridColor, 1))
					{
						wholeBorderPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
						using (Pen toggleBorderPen = new Pen(Color.DimGray, 1))
						{
							toggleBorderPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;

							var fontHeight = Math.Min(LableFontHeight, (int)(Height * 0.4));

							using (Font font = new Font(Font.FontFamily, fontHeight))
							{

								//var indicatorSize = e.Graphics.MeasureString("O", font);
								//var indicatorHeight = (int)(indicatorSize.Height * .3);
								//var indicatorAreaWidth = indicatorHeight;

								if (ParentRow.ChildRows.Count > 0 || ParentRow.ParentDepth == 0)
								{
									IconArea = new Rectangle(0, 0, ToggleTreeButtonWidth, Height);
									//IndicatorArea = new Rectangle(IconArea.Width, 0, indicatorAreaWidth, Height);
									//LabelArea = new Rectangle(IconArea.Width, 0, Width - IconArea.Width, Height);
								}
								else
								{
									var width = ToggleTreeButtonWidth / 2;
									IconArea = new Rectangle(0, 0, width, Height);
									//IndicatorArea = new Rectangle(IconArea.Width, 0, indicatorAreaWidth, Height);
									//LabelArea = new Rectangle(IconArea.Width, 0, Width - IconArea.Width, Height);
								}

								LabelArea = new Rectangle(IconArea.Width, 0, Width - IconArea.Width, Height);
								//var childActiveIndicator = new RectangleF(IndicatorArea.X, Height / 2f + 1, indicatorHeight, indicatorHeight);
								//var activeIndicatorArea = new RectangleF(IndicatorArea.X, Height / 2f - indicatorHeight - 1, indicatorHeight, indicatorHeight);

								Rectangle wholeBorderArea = new Rectangle(0, -1, Width - 1, Height);

								e.Graphics.FillRectangle(backgroundBrush, IconArea);
								e.Graphics.FillRectangle(backgroundBrush, IndicatorArea);
								//if (ActiveIndicator())
								//{
								//	e.Graphics.FillEllipse(Brushes.Lime, activeIndicatorArea);
								//}

								//if (ParentRow.ChildRows.Count > 0 || ParentRow.ParentDepth == 0)
								//{
								//	if (ChildActiveIndicator())
								//	{
								//		e.Graphics.FillEllipse(Brushes.Lime, childActiveIndicator);
								//	};
								//}
								e.Graphics.FillRectangle(backgroundBrush, LabelArea);

								//e.Graphics.DrawRectangle(toggleBorderPen, iconBorderArea);
								e.Graphics.DrawRectangle(wholeBorderPen, wholeBorderArea);

								using (StringFormat sf = new StringFormat())
								{
									sf.Alignment = StringAlignment.Near;
									sf.LineAlignment = StringAlignment.Center;
									sf.Trimming = StringTrimming.EllipsisCharacter;
									sf.FormatFlags = StringFormatFlags.NoWrap;
									Rectangle stringPos = new Rectangle(LabelArea.X, LabelArea.Y, LabelArea.Width - 6, LabelArea.Height);
									e.Graphics.DrawString(Name, font, textBrush, stringPos, sf);
								}
							}
						}
					}

				}
			}
		}

		#endregion
	}
}