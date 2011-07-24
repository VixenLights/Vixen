using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;


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

            m_rows.RowAdded += new EventHandler<RowChangedEventArgs>(m_rows_RowAdded);
            m_rows.RowRemoved += new EventHandler<RowChangedEventArgs>(m_rows_RowRemoved);
        }

        
        void m_rows_RowAdded(object sender, RowChangedEventArgs e)
        {
            e.Row.ParentControl = this;
        }

        void m_rows_RowRemoved(object sender, RowChangedEventArgs e)
        {
            e.Row.ParentControl = null;
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

        


        private TimelineElementCollection m_selectedElements = new TimelineElementCollection();
        public TimelineElementCollection SelectedElements { get { return m_selectedElements; } }


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

        protected override void OnMouseDown(MouseEventArgs e)
        {
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
            if (m_dragState == DragState.Dragging)
            {
                if (ElementsMoved != null)
                {
                    ElementMovedEventArgs evargs = new ElementMovedEventArgs();
                    evargs.MovedElements = new TimelineElementCollection();
                    foreach (TimelineElement elem in SelectedElements)
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

                foreach (TimelineElement elem in SelectedElements)
                {
                    elem.Offset += pixelsToTime(dX);
                }
            }

            this.Refresh();

        }
        #endregion


        #region Drawing


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
                        DrawElementOptions options = DrawElementOptions.Normal;

                        if (SelectedElements.Contains(element))
                            options |= DrawElementOptions.Selected;

                        DrawElement(e.Graphics, element, options);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unhandled Exception while drawing TimelineControl:\n\n" + ex.Message);
                throw;
            }
        }


        [Flags]
        protected enum DrawElementOptions
        {
            Normal      = 0x0,
            Selected    = 0x1,
        }

        protected void DrawElement(Graphics g, TimelineElement element)
        {
            DrawElement(g, element, DrawElementOptions.Normal);
        }
        protected void DrawElement(Graphics g, TimelineElement element, DrawElementOptions options)
        {
            // Calculate x-coord from time offset
            int x = timeToPixels(element.Offset);

            // Calculate y-coord from row number
            int y = topOfRow(element.ParentRow);

            // Calculate width from duration
            int w = timeToPixels(element.Duration);

            // Calculate height from row
            int h = element.ParentRow.Height;

            // Fill body
            Brush b = new SolidBrush(element.BackColor);
            g.FillRectangle(b, x, y, w, h);

            // Draw border
            Pen border = new Pen(Color.Black);
            border.Width = (options.HasFlag(DrawElementOptions.Selected)) ? 3.0f : 1.0f;
            g.DrawRectangle(border, x, y, w, h);
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

        public TimelineElementCollection ElementsAtTime(TimeSpan time)
        {
            TimelineElementCollection col = new TimelineElementCollection();
            foreach (TimelineRow row in Rows)
            {
                foreach (TimelineElement elem in row.Elements)
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
}
