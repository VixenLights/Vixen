using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Timeline
{
	public class TimelineRowLabel : TimelineControlBase
	{
		public TimelineRowLabel()
		{
			BackColor = Color.White;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
            // Fill
            Brush b = new SolidBrush(BackColor);
            e.Graphics.FillRectangle(b, e.Graphics.VisibleClipBounds);

			RectangleF b_rect = new RectangleF(
				e.Graphics.VisibleClipBounds.Left + 2,
				e.Graphics.VisibleClipBounds.Top + 2,
				e.Graphics.VisibleClipBounds.Width - 4,
				e.Graphics.VisibleClipBounds.Height - 4
				);
			
            Pen border = new Pen(Color.Black);
            border.Width = 4;
			e.Graphics.DrawRectangle(border, b_rect.Left, b_rect.Top, b_rect.Width, b_rect.Height);
		}
	}
}
