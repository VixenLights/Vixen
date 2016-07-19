using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Controls.Timeline
{
	[System.ComponentModel.DesignerCategory("")] // Prevent this from showing up in designer.
	public class RowList : UserControl
	{
		public RowList()
		{
			VerticalOffset = 0;
			DottedLineColor = Color.Black;
			RowLabels = new List<RowLabel>();
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			EnableDisableHandlers(true);
			Font = SystemFonts.MessageBoxFont;
		}

		#region Properties

		// the offset at the top (when the control is scrolled)
		private int m_verticalOffset;

		public int VerticalOffset
		{
			get { return m_verticalOffset; }
			set
			{
				if (value < 0)
					value = 0;
				m_verticalOffset = value;
				DoLayout();
				Invalidate();
			}
		}

		public Color DottedLineColor { get; set; }

		// for some reason, even though the Control.Controls collection is supposed
		// to be a list (and therefore ordered), it's not adding controls in the order
		// we add them. So we're keeping our own list of controls.
		private List<RowLabel> RowLabels { get; set; }

		#endregion

		protected override void Dispose(bool disposing)
		{
			if (RowLabels != null) {
				RowLabels.Clear();
				RowLabels= null;
			}
			Row.RowHeightChanged -= RowLabelChangedHandler;
			Row.RowHeightResized -= RowLabelResizedHandler;
			Row.RowToggled -= RowLabelChangedHandler;
			base.Dispose(disposing);
		}
		#region Events

		#endregion

		#region Event Handlers

		protected void LabelVisibleChangedHandler(object sender, EventArgs e)
		{
			var lbl = sender as RowLabel;
			if (lbl !=null && !Controls.Contains(lbl))
			{
				Controls.Add(lbl);
			}
			Invalidate();
		}

		protected void RowLabelChangedHandler(object sender, EventArgs e)
		{
			DoLayout();
		}

		protected void RowLabelResizedHandler(object sender, EventArgs e)
		{
			
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			// do nothing on an actual Layout; we'll do the layout
			// manually when a row is toggled, (ie. only once)
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (Visible)
				DoLayout();
		}

		#endregion

		#region Methods
		public void EnableDisableHandlers(bool enabled = true)
		{
			if (enabled) {
				Row.RowToggled -= RowLabelChangedHandler;
				Row.RowHeightChanged -= RowLabelChangedHandler;
				Row.RowHeightResized -= RowLabelResizedHandler;
				Row.RowToggled += RowLabelChangedHandler;
				Row.RowHeightChanged += RowLabelChangedHandler;
				Row.RowHeightResized += RowLabelResizedHandler;
			} else {
				Row.RowToggled -= RowLabelChangedHandler;
				Row.RowHeightChanged -= RowLabelChangedHandler;
				Row.RowHeightResized -= RowLabelResizedHandler;
			}
		}
		private delegate void AddRowLabelDelegate(RowLabel trl);

		public void AddRowLabel(RowLabel trl)
		{
			if (this.InvokeRequired) {
				this.Invoke(new AddRowLabelDelegate(AddRowLabel), trl);
			}
			else {
				RowLabels.Add(trl);
				// Addint a control is VERY slow!
				if (trl.Visible)
				{
					Controls.Add(trl);
				}

				trl.VisibleChanged += LabelVisibleChangedHandler;
				// Don't call DoLayout. It'll get called after all the rows have been added!
				//DoLayout();
			}
		}

		public void RemoveRowLabel(RowLabel trl)
		{
			RowLabels.Remove(trl);
			Controls.Remove(trl);
			trl.VisibleChanged -= LabelVisibleChangedHandler;
			DoLayout();
		}

		#endregion

		#region Drawing

		public void DoLayout()
		{
			if (RowLabels == null)
				return;

			int offset = -VerticalOffset;
			foreach (RowLabel trl in RowLabels) {
				if (trl.Visible) {
					int x = trl.ParentRow.ParentDepth*RowLabel.ToggleTreeButtonWidth;
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
			int inset = RowLabel.ToggleTreeButtonWidth;
			// TODO: here's to hoping they don't want to go more than 20 levels deep...
			int[] dottedLineTops = new int[20];
			Point[] dottedLineBottoms = new Point[20];
			bool[] dottedLineLevelNeedsDrawing = new bool[20];
			int lastDepth = 0;
			Pen line = new Pen(DottedLineColor);
			line.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

			int top = -VerticalOffset;
			foreach (RowLabel trl in RowLabels) {
				if (trl.Visible) {
					int depth = trl.ParentRow.ParentDepth;

					// if we've gone back up the tree (eg. new branch), draw the dotted
					// line for that last depth that we saw
					for (int i = depth + 1; i <= lastDepth; i++) {
						if (dottedLineLevelNeedsDrawing[i] && dottedLineBottoms[i] != null) {
							Point p = dottedLineBottoms[i];
							e.Graphics.DrawLine(line, p, new Point(p.X, dottedLineTops[i - 1]));
							dottedLineLevelNeedsDrawing[i] = false;
						}
					}

					// if this is an indented row, draw a dotted line out and record its
					// point, to be drawn up later if this is the last point
					if (depth > 0) {
						Point lineStart = new Point((int) ((Single) (depth - 0.5)*inset), (int) (top + trl.Height*0.5));
						Point lineEnd = new Point(depth*inset, lineStart.Y);
						e.Graphics.DrawLine(line, lineStart, lineEnd);
						dottedLineBottoms[depth] = lineStart;
						dottedLineLevelNeedsDrawing[depth] = true;
					}

					lastDepth = depth;
					top += trl.Height;
					dottedLineTops[depth] = top;
				}
			}

			// draw any connecting lines needed if the last rows were offset
			for (int i = 1; i < dottedLineLevelNeedsDrawing.Length; i++) {
				if (dottedLineLevelNeedsDrawing[i] && dottedLineBottoms[i] != null) {
					Point p = dottedLineBottoms[i];
					e.Graphics.DrawLine(line, p, new Point(p.X, dottedLineTops[i - 1]));
				}
			}
		}

		#endregion
	}
}