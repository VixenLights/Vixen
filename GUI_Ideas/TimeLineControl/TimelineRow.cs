using System;


namespace Timeline
{
    /// <summary>
    /// Represents a row in a TimelineControl, which contains TimelineElements.
    /// </summary>
    public class TimelineRow
    {
        protected TimelineElementCollection m_elements = new TimelineElementCollection();

        public TimelineRow()
        {
            m_elements.ElementAdded += new EventHandler<ElementEventArgs>(m_elements_ElementAdded);
            m_elements.ElementRemoved += new EventHandler<ElementEventArgs>(m_elements_ElementRemoved);
        }

        #region Properties

        private int m_height;
        public int Height
        {
            get { return m_height; }
            set { m_height = value; updateParent(); }
        }

        private object m_tag;
        public object Tag
        {
            get { return m_tag; }
            set { m_tag = value; updateParent(); }
        }

        #endregion

        #region Special Properties

        // TODO: implement set, such that when assigning a new timeline control to it,
        // it is removed from the parent control, and added to the new one :-)
        // Do the same for TimelineElement, where it will be even more useful.
		// MS 15/08: can't do this nicely, I think? The way the parent TimelineControl
		// uses events (RowAdded, RowRemoved) to enforce this ParentControl variable means
		// we shouldn't be changing parent controls from here. Whatever is doing the move
		// from one control to another should be calling the parent control methods (Add,
		// Remove) to move the row instead.
        public TimelineGrid ParentControl { get; internal set; }

        public TimelineElementCollection Elements
        {
            get { return m_elements; }
        }

        #endregion

        // Event handlers to forward the added/removed events up from the collection
        protected void m_elements_ElementAdded(object sender, ElementEventArgs e)
        {
            e.Element.ParentRow = this;
            if (ElementAdded != null)
                ElementAdded(this, e);

            updateParent();
        }

        protected void m_elements_ElementRemoved(object sender, ElementEventArgs e)
        {
            e.Element.ParentRow = null;
            if (ElementRemoved != null)
                ElementRemoved(this, e);

            updateParent();
        }

        // These events are forwarded up from the internal container
        internal event EventHandler<ElementEventArgs> ElementAdded;
        internal event EventHandler<ElementEventArgs> ElementRemoved;


        private void updateParent()
        {
            if (ParentControl == null)
                return;
            ParentControl.Refresh();
        }
    }
}
