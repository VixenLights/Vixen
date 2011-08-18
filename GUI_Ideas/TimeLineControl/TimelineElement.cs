using System;
using System.Drawing;

namespace Timeline
{
    public class TimelineElement
    {
        public TimelineElement()
        {
        }


        #region Properties

        private TimeSpan m_offset;
        public TimeSpan Offset
        {
            get { return m_offset; }
            set { m_offset = value; updateParent(); }
        }

        private TimeSpan m_duration;
        public TimeSpan Duration
        {
            get { return m_duration; }
            set { m_duration = value; updateParent(); }
        }

        private Color m_backColor = Color.White;
        public Color BackColor
        {
            get { return m_backColor; }
            set { m_backColor = value; updateParent(); }
        }

        private object m_tag = null;
        public object Tag
        {
            get { return m_tag; }
            set { m_tag = value; updateParent(); }
        }

        #endregion

        #region Special Properties

        /// <summary>
        /// The TimelineRow to which this element belongs, or null if not contained in a TimelineRow.
        /// </summary>
        public TimelineRow ParentRow { get; internal set; }

        /// <summary>
        /// Returns true if the element is selected in the parent TimelineControl.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                if (ParentRow == null || ParentRow.ParentControl == null)
                    return false;
                return ParentRow.ParentControl.SelectedElements.Contains(this);
            }
        }

        #endregion

        #region Public methods

        public bool Select()
        {
            if (ParentRow == null || ParentRow.ParentControl == null)
                return false;
            ParentRow.ParentControl.SelectedElements.AddUnique(this);
            return true;
        }

        #endregion

        #region Private methods

        private void updateParent()
        {
            if (ParentRow == null || ParentRow.ParentControl == null)
                return;
            ParentRow.ParentControl.Refresh();
        }

        #endregion

        internal void Draw(Graphics graphics, Rectangle rect, DrawElementOptions options)
        {
        // BODY
            // Fill
            Brush b = new SolidBrush(BackColor);
            graphics.FillRectangle(b, rect);

        // BORDER
            // Width - bold if selected
            int b_wd = (options.HasFlag(DrawElementOptions.Selected)) ? 3 : 1;

			// Adjust the rect such that the border is completely inside it.
			Rectangle b_rect = new Rectangle(
				rect.Left + (b_wd / 2),
				rect.Top + (b_wd / 2),
				rect.Width - b_wd,
				rect.Height - b_wd
				);
			
			// Draw it!
            Pen border = new Pen(Color.Black);
            border.Width = b_wd;
			//border.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
			//graphics.DrawRectangle(border, rect);
			graphics.DrawRectangle(border, b_rect);
		}
    }


    public class ElementEventArgs : EventArgs
    {
        public TimelineElement Element { get; internal set; }
    }

    public class MultiElementEventArgs : EventArgs
    {
        public TimelineElementCollection Elements { get; internal set; }
    }

}