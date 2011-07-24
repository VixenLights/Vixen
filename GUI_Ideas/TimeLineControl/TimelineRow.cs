using System;


namespace Timeline
{
    public class TimelineRow
    {
        protected TimelineElementCollection m_elements = new TimelineElementCollection();

        public TimelineRow()
        {
            m_elements.ElementAdded += new EventHandler<ElementChangedEventArgs>(m_elements_ElementAdded);
            m_elements.ElementRemoved += new EventHandler<ElementChangedEventArgs>(m_elements_ElementRemoved);
        }


        public int Height { get; set; }
        public object Tag { get; set; }

        public TimelineControl ParentControl { get; internal set; }

        public TimelineElementCollection Elements
        {
            get { return m_elements; }
        }


        protected void m_elements_ElementAdded(object sender, ElementChangedEventArgs e)
        {
            e.Element.ParentRow = this;
            if (ElementAdded != null)
                ElementAdded(this, e);
        }

        protected void m_elements_ElementRemoved(object sender, ElementChangedEventArgs e)
        {
            e.Element.ParentRow = null;
            if (ElementRemoved != null)
                ElementRemoved(this, e);
        }


        // These events are forwarded up from the internal container
        internal event EventHandler<ElementChangedEventArgs> ElementAdded;
        internal event EventHandler<ElementChangedEventArgs> ElementRemoved;
    }
}
