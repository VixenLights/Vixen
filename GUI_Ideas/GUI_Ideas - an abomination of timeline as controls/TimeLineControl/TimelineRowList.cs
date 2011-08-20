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
	public partial class TimelineRowList : TimelineControlBase
	{
		public TimelineRowList()
		{
			TopOffset = 0;
		}

		#region Properties

		//public TimelineControl ParentControl { get; set; }
		//public TimelineControl ParentControl
		//{
		//    get
		//    {
		//        if (Parent is TimelineControl)
		//            return Parent as TimelineControl;
		//        else
		//            return null;
		//    }
		//}


		// the offset at the top (when the control is scrolled)
		public int TopOffset { get; set; }

		#endregion


		private void _drawRows(Graphics g)
		{
			int offset = -TopOffset;

			SolidBrush background = new SolidBrush(Color.LightBlue);
			SolidBrush text = new SolidBrush(Color.Black);
			Pen border = new Pen(Color.DarkBlue, 4);
			Font f = new Font("Arial", 12);

			//foreach (TimelineRow row in ParentControl.Rows) {
			foreach (TimelineRow row in new List<TimelineRow>()) {
				// if the row is completely hidden, don't even bother drawing it
				if (offset + row.Height < 0) {
					offset += row.Height;
					continue;
				}

				// draw the label for the row
				Rectangle r = new Rectangle(0, offset, Width, row.Height);
				g.FillRectangle(background, r);
				//g.DrawString(row.Name, f, text, r);
				g.DrawRectangle(border, r);

				// if the current draw point is off the bottom, don't draw anymore
				offset += row.Height;
				if (offset > Height)
					break;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			_drawRows(e.Graphics);
		}
	}
}
