using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Text;
using System.Diagnostics;


namespace Timeline
{
    public class TimelineControl : UserControl
    {
        public TimelineControl()
        {
            

            this.DoubleBuffered = true;

            m_rows.RowAdded += new EventHandler<RowAddedOrRemovedEventArgs>(m_rows_RowAdded);
            m_rows.RowRemoved += new EventHandler<RowAddedOrRemovedEventArgs>(m_rows_RowRemoved);

            /*
            // How the hell do I get this scrolling to not return to zero?
            this.Scroll += new ScrollEventHandler(TimelineControl_Scroll);

            this.HorizontalScroll.Enabled = true;
            this.HorizontalScroll.Visible = true;
            this.HorizontalScroll.Minimum = 0;

            this.VerticalScroll.Enabled = false;
            this.VerticalScroll.Visible = true;

            this.AutoScroll = false;
             */

            this.AutoScroll = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetDefaultOptions();

            // These must be initialized in this order, otherwise divide by zero occurrs.
            VisibleTimeSpan = TimeSpan.FromSeconds(10.0);
            MaximumTime = TimeSpan.FromSeconds(60.0);
        }


        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            this.Invalidate();
        }


        //void TimelineControl_Scroll(object sender, ScrollEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine("this.Scroll: {0}", e.NewValue);
        //}

        

        
        /// <summary>
        /// The maximum amount of time represented by this TimelineControl.
        /// </summary>
        public TimeSpan MaximumTime
        {
            /*
            get { return pixelsToTime(HorizontalScroll.Maximum); }
            set { HorizontalScroll.Maximum = timeToPixels(value); }
             */

            get { return pixelsToTime(this.AutoScrollMinSize.Width); }
            set { AutoScrollMinSize = new Size(timeToPixels(value), AutoScrollMinSize.Height); }
        }

        /// <summary>
        /// The amount of time currently visible. Adjusting this implements zoom along the X (time) axis.
        /// </summary>
        public TimeSpan VisibleTimeSpan { get; set; }

        /// <summary>
        /// The time at the left of the control (the visible beginning).
        /// </summary>
        public TimeSpan VisibleTimeStart
        {
            /*
            get { return pixelsToTime(HorizontalScroll.Value); }
            set { HorizontalScroll.Value = timeToPixels(value); }
             */
            get { return pixelsToTime(AutoScrollOffset.X); }
            set { AutoScrollOffset = new Point(timeToPixels(value), AutoScrollOffset.Y); }
        }

        /// <summary>
        /// Gets the amount of time represented by one horizontal pixel.
        /// </summary>
        protected TimeSpan TimePerPixel
        {
            get { return TimeSpan.FromTicks(VisibleTimeSpan.Ticks / Width); }
        }

        
        

        private TimelineElementCollection m_selectedElements = new TimelineElementCollection();
        public TimelineElementCollection SelectedElements { get { return m_selectedElements; } }

        protected TimelineRowCollection m_rows = new TimelineRowCollection();
        public TimelineRowCollection Rows { get { return m_rows; } }

        void m_rows_RowAdded(object sender, RowAddedOrRemovedEventArgs e)
        {
            e.Row.ParentControl = this;
        }

        void m_rows_RowRemoved(object sender, RowAddedOrRemovedEventArgs e)
        {
            e.Row.ParentControl = null;
        }















        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            //base.OnKeyPress(e);

            if (e.KeyChar == (char)27)  // ESC
            {
                SelectedElements.Clear();
                endDrag();  // do this regardless of if we're dragging or not.

                this.Refresh();
            }

        }

        #region Select / Drag

        private const int DragThreshold = 4;
        private enum DragState
        {
            /// <summary>
            /// Not dragging, mouse is up.
            /// </summary>
            Normal = 0,

            /// <summary>
            /// Mouse down, but hasn't moved past threshold yet to be considered dragging
            /// </summary>
            Waiting,

            /// <summary>
            /// Actively dragging objects
            /// </summary>
            Dragging
        }
        private DragState m_dragState = DragState.Normal;

        private Point m_oldLoc;
        private TimelineElement m_mouseDownElement = null;

        private bool CtrlPressed { get { return Form.ModifierKeys.HasFlag(Keys.Control); } }

        private void dragWait(Point location)
        {
            m_dragState = DragState.Waiting;
            m_oldLoc = location;
        }

        private void beginDrag()
        {
            m_dragState = DragState.Dragging;
            this.Cursor = Cursors.SizeAll;
        }

        private void endDrag()
        {
            m_dragState = DragState.Normal;
            this.Cursor = Cursors.Default;
        }

        private void _translateMouseArgs(ref MouseEventArgs e)
        {
            // Translate this location based on the auto scroll position.
            Point p = e.Location;
            p.Offset(-AutoScrollPosition.X, -AutoScrollPosition.Y);

            // Just "fix" it :-)
            e = new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, e.Delta);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _translateMouseArgs(ref e);

            m_mouseDownElement = elementAt(e.Location);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    OnLeftMouseDown(e);
                    break;
                case MouseButtons.Middle:
                    break;
                case MouseButtons.None:
                    break;
                case MouseButtons.Right:
                    break;
                case MouseButtons.XButton1:
                    break;
                case MouseButtons.XButton2:
                    break;
                default:
                    break;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _translateMouseArgs(ref e);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    OnLeftMouseUp(e);
                    break;
                case MouseButtons.Middle:
                    break;
                case MouseButtons.None:
                    break;
                case MouseButtons.Right:
                    break;
                case MouseButtons.XButton1:
                    break;
                case MouseButtons.XButton2:
                    break;
                default:
                    break;
            }
        }

        private void OnLeftMouseDown(MouseEventArgs e)
        {
            // e is already translated.

            if (m_mouseDownElement == null)   // we clicked nothing - clear selection
            {
                SelectedElements.Clear();
            }
            else    // our mouse is down on something
            {
                if (m_mouseDownElement.IsSelected)
                {
                    // unselect
                    if (CtrlPressed)
                        SelectedElements.Remove(m_mouseDownElement);
                }
                else
                {
                    // select
                    if (!CtrlPressed)
                        SelectedElements.Clear();
                    SelectedElements.AddUnique(m_mouseDownElement);
                }

                dragWait(e.Location);
            }
            this.Refresh();
        }

        public event EventHandler<ElementMovedEventArgs> ElementsMoved;

        private void OnLeftMouseUp(MouseEventArgs e)
        {
            // e is already translated

            if (m_dragState == DragState.Dragging)
            {
                if (ElementsMoved != null)
                {
                    ElementMovedEventArgs evargs = new ElementMovedEventArgs();
                    evargs.MovedElements = new TimelineElementCollection();
                    foreach (var elem in SelectedElements)
                        evargs.MovedElements.Add(elem);
                    ElementsMoved(this, evargs);
                }
                
            }
            else
            {
                // If we're not dragging on mouse up, it could be a click on one of multiple
                // selected elements. (In which case we select only that one)
                if (m_mouseDownElement != null && !CtrlPressed)
                {
                    SelectedElements.Clear();
                    SelectedElements.Add(m_mouseDownElement);
                }
            }

            endDrag();  // we always do this, even if we weren't dragging.

            this.Refresh();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            _translateMouseArgs(ref e);

            if (m_dragState == DragState.Normal)
                return;

            if (m_dragState == DragState.Waiting)
            {
                if (Math.Abs(e.X - m_oldLoc.X) > DragThreshold)
                    beginDrag();
            }
            if (m_dragState == DragState.Dragging)
            {
                int dX = e.X - m_oldLoc.X;
                m_oldLoc.X = e.X;

                int dY = e.Y - m_oldLoc.Y;
                m_oldLoc.Y = e.Y;

                foreach (var elem in SelectedElements)
                {
                    elem.Offset += pixelsToTime(dX);
                }
            }

            this.Refresh();

        }
        #endregion


        #region Drawing

        private void _drawRows(Graphics g)
        {
            // Draw row separators
                int curY = 0;
                Pen p = new Pen(RowSeparatorColor);
                foreach (var row in Rows)
                {
                    curY += row.Height;
                    Point left = new Point(0, curY);
                    Point right = new Point(this.Width, curY);
                    g.DrawLine(p, left, right);
                }
        }

        private void _drawGridlines(Graphics g)
        {
            // Draw vertical gridlines
            int interval = timeToPixels(GridlineInterval);

            // calculate first tick - (it is the first multiple of interval greater than start)
            // believe it or not, this math is correct :-)
            int start = HorizontalScroll.Value / interval * interval + interval;

            for (int x = start; x < start + Width; x += interval)
            {
                Pen p = new Pen(Color.Black);
                p.DashStyle = DashStyle.Dash;
                g.DrawLine(p, x, 0, x, Height);
            }

        }

        private void _drawElements(Graphics g)
        {
            GraphicsState state = g.Save();

            // Draw each row
            int top = 0;    // y-coord of top of current row
            foreach (var row in Rows)
            {
                // Draw each element
                foreach (var element in row.Elements)
                {
                    DrawElementOptions options = DrawElementOptions.Normal;

                    if (SelectedElements.Contains(element))
                        options |= DrawElementOptions.Selected;

                    Point location = new Point(timeToPixels(element.Offset), top);
                    Size size = new Size(timeToPixels(element.Duration), row.Height);

                    // Translate the graphics so the element can draw at (0,0)
                    //Matrix m = new Matrix();
                    //m.Translate(location.X, location.Y);
                    //g.Transform = m;
                    g.TranslateTransform(location.X, location.Y);

                    // Calculate the rectangle this element is to be drawn in.
                    Rectangle rect = new Rectangle(0, 0, size.Width, size.Height);

                    // Prevent the element from drawing outside its bounds
                    g.Clip = new System.Drawing.Region(rect);

                    element.Draw(g, rect, options);

                    g.Restore(state);
                }

                top += row.Height;  // next row starts just below this row
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                _drawRows(e.Graphics);      // We can draw this pre-translation.

                e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);

                _drawGridlines(e.Graphics);
                _drawElements(e.Graphics);

                base.OnPaint(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unhandled Exception while drawing TimelineControl:\n\n" + ex.Message);
                throw;
            }
        }

        #endregion




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
            foreach (var searchrow in Rows)
            {
                if (searchrow == row)
                    return top;
                top += searchrow.Height;
            }
            throw new Exception("row not found");
        }


        /// <summary>
        /// Returns the element located at the current point in screen coordinates
        /// </summary>
        /// <param name="p">Screen coordinates</param>
        /// <returns>Element at given point, or null if none exists.</returns>
        protected TimelineElement elementAt(Point p)
        {
            // Translate 
            // First figure out which row we are in
            TimelineRow containingRow = null;
            int curheight = 0;
            foreach (var row in Rows)
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
            foreach (var elem in containingRow.Elements)
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
            GridlineInterval = TimeSpan.FromSeconds(1.0);
        }
        public Color RowSeparatorColor { get; set; }
        public TimeSpan GridlineInterval { get; set; }
        #endregion

        public TimelineElementCollection ElementsAtTime(TimeSpan time)
        {
            TimelineElementCollection col = new TimelineElementCollection();
            foreach (var row in Rows)
            {
                foreach (var elem in row.Elements)
                {
                    if ((time >= elem.Offset) && (time <= (elem.Offset + elem.Duration)))
                        col.Add(elem);
                }
            }

            return col;
        }

    }


    public class ElementMovedEventArgs : EventArgs
    {
        public TimelineElementCollection MovedElements { get; internal set; }
    }



    [Flags]
    public enum DrawElementOptions
    {
        Normal = 0x0,
        Selected = 0x1,
    }
}
