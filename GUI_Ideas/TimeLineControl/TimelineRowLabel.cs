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

		private Rectangle ToggleButton { get; set; }
		private Rectangle LabelArea { get; set; }

		#endregion


		#region Events

		internal event EventHandler TreeToggled;
		private void _TreeToggled() { if (TreeToggled != null) TreeToggled(this, EventArgs.Empty); }

		#endregion


		#region Event Handlers

		protected override void OnMouseClick(MouseEventArgs e)
		{
			// if it's within the toggle button, toggle the tree
			if (ToggleButton.Contains(e.Location))
				_TreeToggled();
		}

		#endregion


		#region Methods

		#endregion


		#region Drawing

		protected override void OnPaint(PaintEventArgs e)
		{
			SolidBrush backgroundBrush = new SolidBrush(Color.LightBlue);
			SolidBrush closeBrush;
			SolidBrush text = new SolidBrush(Color.Black);
			Pen border = new Pen(Color.DarkBlue, 4);
			Font f = new Font("Arial", 12);

			if (ParentRow.ChildRows.Count > 0)
				closeBrush = new SolidBrush(Color.Blue);
			else
				closeBrush = new SolidBrush(Color.LightBlue);

			ToggleButton = new Rectangle(0, 0, ToggleTreeButtonWidth, Height);
			LabelArea = new Rectangle(ToggleTreeButtonWidth, 0, Width - ToggleTreeButtonWidth, Height);
			e.Graphics.FillRectangle(closeBrush, ToggleButton);
			e.Graphics.FillRectangle(backgroundBrush, LabelArea);
			e.Graphics.DrawString(Name, f, text, LabelArea);
			e.Graphics.DrawRectangle(border, LabelArea);
		}

		#endregion

	}
}
