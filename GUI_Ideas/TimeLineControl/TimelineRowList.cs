using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Timeline
{
	public partial class TimelineRowList : UserControl
	{
		private int m_topOffset;

		public TimelineRowList()
		{
			TopOffset = 0;
			DottedLineColor = Color.Black;
			DoubleBuffered = true;
		}

		// the offset at the top (when the control is scrolled)
		public int TopOffset
		{
			get { return m_topOffset; }
			set { m_topOffset = value; PerformLayout(); Refresh(); }
		}

		public Color DottedLineColor { get; set; }


		protected override void OnLayout(LayoutEventArgs e)
		{
			int offset = -TopOffset;
			foreach (TimelineRowLabel trl in Controls) {
				if (trl.Visible) {
					int x = trl.ParentRow.ParentDepth * TimelineRowLabel.ToggleTreeButtonWidth;
					trl.Location = new Point(x, offset);
					trl.Width = Width - x;
					offset += trl.Height;
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			int inset = TimelineRowLabel.ToggleTreeButtonWidth;
			// TODO: here's to hoping they don't want to go more than 20 levels deep...
			int[] dottedLineTops = new int[20];
			Point[] dottedLineBottoms = new Point[20];
			int lastDepth = 0;
			Pen line = new Pen(DottedLineColor);
			line.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

			int top = -TopOffset;
			foreach (TimelineRowLabel trl in Controls) {
				if (trl.Visible) {
					int depth = trl.ParentRow.ParentDepth;

					// if we've gone back up the tree (eg. new branch), draw the dotted
					// line for that last depth that we saw
					if (depth < lastDepth && dottedLineBottoms[lastDepth] != null) {
						Point p = dottedLineBottoms[lastDepth];
						e.Graphics.DrawLine(line, p, new Point(p.X, dottedLineTops[lastDepth-1]));
					}

					// if this is an indented row, draw a dotted line out and record its
					// point, to be drawn up later if this is the last point
					if (depth > 0) {
						Point lineStart = new Point((int)((Single)(depth - 0.5) * inset), (int)(top + trl.Height * 0.5));
						Point lineEnd = new Point(depth * inset, lineStart.Y);
						e.Graphics.DrawLine(line, lineStart, lineEnd);
						dottedLineBottoms[depth] = lineStart;
					}

					lastDepth = depth;
					top += trl.Height;
					dottedLineTops[depth] = top;
				}
			}
		}
	}
}
