using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Diagnostics;


namespace Timeline
{
    public class TimelineControl : UserControl
    {
        public TimelineControl()
        {

            SetDefaultOptions();

            this.DoubleBuffered = true;

            MaximumTime = TimeSpan.FromMinutes(1.0);
            VisibleTime = TimeSpan.FromSeconds(10.0);

            //initializeDragTimer();
        }

        
        /// <summary>
        /// The maximum amount of time represented by this TimelineControl.
        /// </summary>
        public TimeSpan MaximumTime { get; set; }

        /// <summary>
        /// The amount of time currently visible. Adjusting this implements zoom along the X (time) axis.
        /// </summary>
        public TimeSpan VisibleTime { get; set; }

        protected TimelineRowCollection m_rows = new TimelineRowCollection();
        public TimelineRowCollection Rows { get { return m_rows; } }


        /// <summary>
        /// Gets the amount of time represented by one horizontal pixel.
        /// </summary>
        protected TimeSpan TimePerPixel
        {
            get
            {
                return TimeSpan.FromTicks(VisibleTime.Ticks / Width);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);

                // Draw row separators
                int curY = 0;
                Pen p = new Pen(RowSeparatorColor);
                foreach (TimelineRow row in Rows)
                {
                    curY += row.Height;
                    Point left = new Point(0, curY);
                    Point right = new Point(this.Width, curY);
                    e.Graphics.DrawLine(p, left, right);
                }

                // Draw each
                foreach (TimelineRow row in Rows)
                {
                    foreach (TimelineElement element in row.Elements)
                    {
                        DrawElement(e.Graphics, element);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unhandled Exception while drawing TimelineControl:\n\n" + ex.Message);
                throw;
            }
        }



        #region Drag


        #region Click Filtering before Drag

        /* So, this works, but it's a little flaky.  I'd go about it some different way, probably ingoring
         * the OnClick event alltogether, and implementing my own OnClick type of event, using a combination
         * of MouseDown / MouseUp.  It may not be needed at all anyway.
         */

        //private MouseEventArgs m_mouseDownInitialArgs;  // these are the args initially passed to OnMouseDown
        //private bool m_inhibitClickEvent = false;       // ignore the next OnClick event

        //private Timer m_dragTimer;
        //private const int InitialDragDelay = 100;  // start stupid high for testing
        //public int DragDelay
        //{
        //    get { return m_dragTimer.Interval; }
        //    set { m_dragTimer.Interval = value; }
        //}

        //private void initializeDragTimer()
        //{
        //    m_dragTimer = new Timer();
        //    m_dragTimer.Interval = InitialDragDelay;
        //    m_dragTimer.Tick += new EventHandler(beginDrag);
        //}


        //void beginDrag(object sender, EventArgs e)
        //{
        //    m_dragTimer.Stop();
        //    m_inhibitClickEvent = true;
        //    m_dragElement = elementAt(m_mouseDownInitialArgs.Location);
        //    if (m_dragElement != null)
        //    {
        //        m_mouseDownPoint = m_mouseDownInitialArgs.Location;
        //        m_mouseDownOffset = m_dragElement.Offset;
        //        this.Cursor = Cursors.SizeAll;
        //    }
        //}


        //protected override void OnMouseDown(MouseEventArgs e)
        //{
        //    //base.OnMouseDown(e);

        //    // When initially clicked, start the drag timer. If it expires (before OnClick cancels it)
        //    // we consider it a drag, not a click. Thus, call beginDrag, and block the OnClick!
        //    m_mouseDownInitialArgs = e;
        //    m_dragTimer.Start();
        //}

        //protected override void OnClick(EventArgs e)
        //{
        //    //base.OnClick(e);

        //    if (!m_inhibitClickEvent)
        //    {
        //        m_dragTimer.Stop();

        //        Debug.WriteLine("{0} Click", DateTime.Now.Second);
        //    }

        //    m_inhibitClickEvent = false;
        //}

        #endregion


        private TimelineElement m_dragElement = null;
        private Point m_mouseDownPoint;
        private TimeSpan m_mouseDownOffset;
       

        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_dragElement = elementAt(e.Location);
            if (m_dragElement != null)
            {
                m_mouseDownPoint = e.Location;
                m_mouseDownOffset = m_dragElement.Offset;
                this.Cursor = Cursors.SizeAll;
            }
        }




        protected override void OnMouseUp(MouseEventArgs e)
        {
            //base.OnMouseUp(e);

            m_dragElement = null;
            this.Cursor = Cursors.Default;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //base.OnMouseMove(e);

            if (m_dragElement != null)
            {
                // Displacement from original location
                //(I figure this is better than an event-by-event displacement, for sake of cancel/undo)
                int dX = e.X - m_mouseDownPoint.X;
                int dY = e.Y - m_mouseDownPoint.Y;

                // Calculate new placement
                m_dragElement.Offset = m_mouseDownOffset + pixelsToTime(dX);
                this.Refresh();
            }

        }
        #endregion


        protected void DrawElement(Graphics g, TimelineElement element)
        {
            // Calculate x-coord from time offset
            int x = timeToPixels(element.Offset);

            // Calculate y-coord from row number
            int y = topOfRow(element.Row);

            // Calculate width from duration
            int w = timeToPixels(element.Duration);

            // Calculate height from row
            int h = element.Row.Height;

            // Fill body
            Brush b = new SolidBrush(element.BackColor);
            g.FillRectangle(b, x, y, w, h);

            // Draw border
            Pen border = new Pen(Color.Black);
            g.DrawRectangle(border, x, y, w, h);
        }





        #region Utility Functions
        private int timeToPixels(TimeSpan t)
        {
            return (int)(t.Ticks / this.TimePerPixel.Ticks);
        }

        private TimeSpan pixelsToTime(int px)
        {
            return TimeSpan.FromTicks(px * this.TimePerPixel.Ticks);
        }


        protected int topOfRow(TimelineRow row)
        {
            int top = 0;
            foreach (TimelineRow searchrow in Rows)
            {
                if (searchrow == row)
                    return top;
                top += searchrow.Height;
            }
            throw new Exception("row not found");
        }

        protected TimelineElement elementAt(Point p)
        {
            // First figure out which row we are in
            TimelineRow containingRow = null;
            int curheight = 0;
            foreach (TimelineRow row in Rows)
            {
                if (p.Y < curheight + row.Height)
                {
                    containingRow = row;
                    break;
                }
                curheight += row.Height;
            }

            if (containingRow == null)
                return null;

            // Now figure out which element we are on
            foreach (TimelineElement elem in containingRow.Elements)
            {
                int elemX = timeToPixels(elem.Offset);
                int elemW = timeToPixels(elem.Duration);
                if (p.X >= elemX && p.X <= elemX + elemW)
                    return elem;
            }

            return null;
        }

        #endregion

        #region Options
        public void SetDefaultOptions()
        {
            RowSeparatorColor = Color.Black;
        }
        public Color RowSeparatorColor { get; set; }
        #endregion

    }


}
