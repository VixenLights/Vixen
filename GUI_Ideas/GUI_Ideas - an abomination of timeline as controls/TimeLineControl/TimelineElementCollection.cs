using System;
using System.Collections;
using System.Collections.Generic;

namespace Timeline
{
    /// <summary>
    /// A collection of TimelineElements. Unline TimelineRowCollection,
    /// the order of elements in the list does *not* matter, for the elements
    /// Offset and Duration properties dictate their location in the row.
    /// </summary>
    public class TimelineElementCollection : IEnumerable<TimelineElement>
    {
        private List<TimelineElement> m_list = new List<TimelineElement>();

        #region Events

        // These two events are here (and internal) so the parent TimelineRow can
        // mark said elements' ParentRow property with itself.
        internal event EventHandler<ElementEventArgs> ElementAdded;
        internal event EventHandler<ElementEventArgs> ElementRemoved;

        private void _elementAdded(TimelineElement element)
        {
            if (ElementAdded != null)
                ElementAdded(this, new ElementEventArgs { Element = element });
        }

        private void _elementRemoved(TimelineElement element)
        {
            if (ElementRemoved != null)
                ElementRemoved(this, new ElementEventArgs { Element = element });
        }

        // Public events
        public event EventHandler CollectionChanged;
        private void _collectionChanged()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new EventArgs());
        }

        #endregion


        public TimelineElement this[int index]
        {
            get { return m_list[index]; }

            // I don't think we want to allow this, b/c the order doesn't matter.
            //set
            //{
            //    m_list[index] = value;
            //    _collectionChanged();
            //}
        }

        public void Add(TimelineElement element)
        {
            m_list.Add(element);

            _elementAdded(element);
            _collectionChanged();
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
            _elementRemoved(element);
        }

        public bool Contains(TimelineElement element)
        {
            return m_list.Contains(element);
        }

        public void Clear()
        {
            m_list.Clear();
            _collectionChanged();
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
