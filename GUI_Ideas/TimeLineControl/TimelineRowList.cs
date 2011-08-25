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

		public TimelineRowList()
		{
			TopOffset = 0;
			DottedLineColor = Color.Black;
			RowLabels = new List<TimelineRowLabel>();
			DoubleBuffered = true;
			this.SetStyle(ControlStyles.ResizeRedraw, true);
		}

		#region Properties

		// the offset at the top (when the control is scrolled)
		private int m_topOffset;
		public int TopOffset
		{
			get { return m_topOffset; }
			set { m_topOffset = value; PerformLayout(); Invalidate(true); }
		}

		public Color DottedLineColor { get; set; }

		// for some reason, even though the Control.Controls collection is supposed
		// to be a list (and therefore ordered), it's not adding controls in the order
		// we add them. So we're keeping our own list of controls.
		private List<TimelineRowLabel> RowLabels { get; set; }

		#endregion


		#region Methods

		public void AddRowLabel(TimelineRowLabel trl)
		{
			RowLabels.Add(trl);
			Controls.Add(trl);
		}

		public void RemoveRowLabel(TimelineRowLabel trl)
		{
			RowLabels.Remove(trl);
			Controls.Remove(trl);
		}

		#endregion


		#region Drawing

		protected override void OnLayout(LayoutEventArgs e)
		{
			if (RowLabels == null)
				return;

			int offset = -TopOffset;
			foreach (TimelineRowLabel trl in RowLabels) {
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
			if (RowLabels == null)
				return;

			int inset = TimelineRowLabel.ToggleTreeButtonWidth;
			// TODO: here's to hoping they don't want to go more than 20 levels deep...
			int[] dottedLineTops = new int[20];
			Point[] dottedLineBottoms = new Point[20];
			int lastDepth = 0;
			Pen line = new Pen(DottedLineColor);
			line.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

			int top = -TopOffset;
			foreach (TimelineRowLabel trl in RowLabels) {
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

		#endregion
	}
}
