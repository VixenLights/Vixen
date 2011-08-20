using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace Timeline
{
    /// <summary>
    /// Represents a row in a TimelineControl, which contains TimelineElements.
    /// </summary>
	public class TimelineRow : UserControl
    {
		//protected TimelineElementCollection m_elements = new TimelineElementCollection();
		private HashSet<TimelineElement> m_selectedElements;

        public TimelineRow()
        {
			DoubleBuffered = true;
			//m_elements.ElementAdded += new EventHandler<ElementEventArgs>(m_elements_ElementAdded);
			//m_elements.ElementRemoved += new EventHandler<ElementEventArgs>(m_elements_ElementRemoved);
			m_selectedElements = new HashSet<TimelineElement>();
		}

        #region Properties


		public TimelineGrid ParentGrid
		{
			get
			{
				if (Parent is TimelineGrid)
					return Parent as TimelineGrid;
				else
					return null;
			}
		}

		public TimeSpan VisibleTimeStart { get { return ParentGrid.VisibleTimeStart; } }
		public TimeSpan MajorGridlineInterval { get { return ParentGrid.MajorGridlineInterval; } }
		public Color MajorGridlineColor { get { return ParentGrid.MajorGridlineColor; } }
		public Color RowSeparatorColor { get { return ParentGrid.RowSeparatorColor; } }

		public Single timeToPixels(TimeSpan t) { return ParentGrid.timeToPixels(t); }
		public TimeSpan pixelsToTime(int px) { return ParentGrid.pixelsToTime(px); }

		#endregion


		#region Methods

		internal bool IsChildSelected(TimelineElement te)
		{
			if (m_selectedElements.Contains(te))
				return true;
			else
				return false;
		}

		internal void SelectChild(TimelineElement te)
		{
			m_selectedElements.Add(te);
		}

		internal void DeselectChild(TimelineElement te)
		{
			m_selectedElements.Remove(te);
		}

		internal void DeselectAllChildren()
		{
			m_selectedElements.Clear();
		}

		#endregion


		private void _drawGridlines(Graphics g)
		{
			// Draw vertical gridlines
			Single interval = timeToPixels(MajorGridlineInterval);

			// calculate first tick - (it is the first multiple of interval greater than start)
			// believe it or not, this math is correct :-)
			Single start = timeToPixels(VisibleTimeStart) - (timeToPixels(VisibleTimeStart) % interval) + interval;

			for (Single x = start; x < start + Width; x += interval) {
				Pen p = new Pen(MajorGridlineColor);
				p.DashStyle = DashStyle.Dash;
				g.DrawLine(p, x, 0, x, Height);
			}
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			_drawGridlines(e.Graphics);
			Pen p = new Pen(RowSeparatorColor);
			e.Graphics.DrawLine(p, 0, Height - 1, Width, Height - 1);
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);

			foreach (TimelineElement te in Controls) {
				te.Location = new Point((int)timeToPixels(te.TimeOffset), 0);
				te.Width = (int)timeToPixels(te.Duration);
				te.Height = Height - 1;
			}
		}


		protected void ElementMoved(object sender, EventArgs e)
		{
			if (Controls.Contains(sender as TimelineElement)) {
				PerformLayout();
			}
		}

		//// Event handlers to forward the added/removed events up from the collection
		//protected void m_elements_ElementAdded(object sender, ElementEventArgs e)
		//{
		//    e.Element.ParentRow = this;
		//    if (ElementAdded != null)
		//        ElementAdded(this, e);

		//    updateParent();
		//}

		//protected void m_elements_ElementRemoved(object sender, ElementEventArgs e)
		//{
		//    e.Element.ParentRow = null;
		//    if (ElementRemoved != null)
		//        ElementRemoved(this, e);

		//    updateParent();
		//}

		//// These events are forwarded up from the internal container
		//internal event EventHandler<ElementEventArgs> ElementAdded;
		//internal event EventHandler<ElementEventArgs> ElementRemoved;


		//private void updateParent()
		//{
		//    if (ParentControl == null)
		//        return;
		//    ParentControl.Refresh();
		//}


	}
}
