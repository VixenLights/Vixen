using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Timeline
{
	public class TimelineRowLabel : UserControl
	{
		public TimelineRowLabel(TimelineRow parentRow)
			: this()
		{
			ParentRow = parentRow;
		}

		public TimelineRowLabel()
		{
			DoubleBuffered = true;
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			ResizeBarWidth = 6;
			Resizing = false;
		}

		#region Properties

		private TimelineRow m_parentRow;
		public TimelineRow ParentRow
		{
			get { return m_parentRow; }
			set { m_parentRow = value; }
		}

		static private int m_toggleTreeButtonWidth = 30;
		static public int ToggleTreeButtonWidth
		{
			get { return m_toggleTreeButtonWidth; }
			set { m_toggleTreeButtonWidth = value; }
		}

		private Rectangle ToggleArea { get; set; }
		private Rectangle LabelArea { get; set; }
		private int ResizeBarWidth { get; set; }
		private bool Resizing { get; set; }
		private Point LastMouseLocation { get; set; }

		#endregion


		#region Events

		internal event EventHandler TreeToggled;
		internal event EventHandler LabelClicked;
		internal event EventHandler<RowHeightChangedEventArgs> HeightChanged;

		private void _TreeToggled() { if (TreeToggled != null) TreeToggled(this, EventArgs.Empty); }
		private void _LabelClicked() { if (LabelClicked != null) LabelClicked(this, EventArgs.Empty); }
		private void _HeightChanged(int dh) { if (HeightChanged != null) HeightChanged(this, new RowHeightChangedEventArgs(dh)); }

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

			if (ToggleArea.Contains(e.Location)) {
				// if it's within the toggle button, toggle the tree
				_TreeToggled();
			} else if (LabelArea.Contains(e.Location)) {
				_LabelClicked();
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
				Resizing = false;
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

		#endregion


		#region Drawing

		protected override void OnPaint(PaintEventArgs e)
		{
			SolidBrush backgroundBrush = new SolidBrush(Color.LightBlue);
			SolidBrush closeBrush = new SolidBrush(Color.Blue);
			SolidBrush text = new SolidBrush(Color.Black);
			Pen border = new Pen(Color.DarkBlue, ResizeBarWidth);
			Font f = new Font("Arial", 12);

			if (ParentRow.ChildRows.Count > 0) {
				ToggleArea = new Rectangle(0, 0, ToggleTreeButtonWidth, Height);
				LabelArea = new Rectangle(ToggleTreeButtonWidth, 0, Width - ToggleTreeButtonWidth, Height);
			} else {
				ToggleArea = new Rectangle(0, 0, 0, 0);
				LabelArea = new Rectangle(0, 0, Width, Height);
			}

			e.Graphics.FillRectangle(closeBrush, ToggleArea);
			e.Graphics.FillRectangle(backgroundBrush, LabelArea);
			e.Graphics.DrawString(Name, f, text, LabelArea);

			e.Graphics.DrawLine(border, ToggleArea.Width, Height, Width, Height);
		}

		#endregion

	}

	public class RowHeightChangedEventArgs : EventArgs
	{
		public RowHeightChangedEventArgs(int heightChange)
		{
			HeightChange = heightChange;
		}

		public int HeightChange { get; internal set; }
	}
}
