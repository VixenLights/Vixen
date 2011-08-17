using System;
using System.Collections;
using System.Collections.Generic;

namespace Timeline
{
    public class ElementAddedOrRemovedEventArgs : EventArgs
    {
        public TimelineElement Element { get; set; }
    }

    /// <summary>
    /// A collection of TimelineElements. Unline TimelineRowCollection,
    /// the order of elements in the list does *not* matter, for the elements
    /// Offset and Duration properties dictate their location in the row.
    /// </summary>
    public class TimelineElementCollection : IEnumerable<TimelineElement>
    {
        private List<TimelineElement> m_list = new List<TimelineElement>();

        internal event EventHandler<ElementAddedOrRemovedEventArgs> ElementAdded;
        internal event EventHandler<ElementAddedOrRemovedEventArgs> ElementRemoved;

        public TimelineElement this[int index]
        {
            get { return m_list[index]; }
            set { m_list[index] = value; }
        }

        public void Add(TimelineElement element)
        {
            m_list.Add(element);

            if (ElementAdded != null)
                ElementAdded(this, new ElementAddedOrRemovedEventArgs { Element = element });
        }

        public bool AddUnique(TimelineElement element)
        {
            if (m_list.Contains(element))
                return false;

            Add(element);
            return true;
        }


        public void Remove(TimelineElement element)
        {
            m_list.Remove(element);

            if (ElementRemoved != null)
                ElementRemoved(this, new ElementAddedOrRemovedEventArgs { Element = element });
        }

        public bool Contains(TimelineElement element)
        {
            return m_list.Contains(element);
        }

        public void Clear()
        {
            m_list.Clear();
        }

        public bool IsEmpty { get { return (m_list.Count == 0); } }



        public IEnumerator<TimelineElement> GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_list.GetEnumerator();
        }
    }
}
