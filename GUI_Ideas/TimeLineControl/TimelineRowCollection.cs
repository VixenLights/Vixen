using System;
using System.Collections;
using System.Collections.Generic;


namespace Timeline
{
    public class RowAddedOrRemovedEventArgs : EventArgs
    {
        public TimelineRow Row { get; set; }
    }

    
    /// <summary>
    /// A collection of TimelineRow elements. Unlike TimelineElementCollection,
    /// the order matters, and dictates the order of the TimelineRows in the TimelineControl.
    /// </summary>
    public class TimelineRowCollection : IEnumerable<TimelineRow>
    {
        private List<TimelineRow> m_list = new List<TimelineRow>();

        internal event EventHandler<RowAddedOrRemovedEventArgs> RowAdded;
        internal event EventHandler<RowAddedOrRemovedEventArgs> RowRemoved;

        public void Add(TimelineRow row)
        {
            m_list.Add(row);

            if (RowAdded != null)
                RowAdded(this, new RowAddedOrRemovedEventArgs { Row = row });
        }

        public void Remove(TimelineRow row)
        {
            m_list.Remove(row);

            if (RowRemoved != null)
                RowRemoved(this, new RowAddedOrRemovedEventArgs { Row = row });
        }


        public void Insert(int index, TimelineRow row)
        {
            m_list.Insert(index, row);

            if (RowAdded != null)
                RowAdded(this, new RowAddedOrRemovedEventArgs { Row = row });
        }

        public bool Contains(TimelineRow row)
        {
            return m_list.Contains(row);
        }

        public TimelineRow this[int index]
        {
            get { return m_list[index]; }
            set { m_list[index] = value; }
        }

        public int IndexOf(TimelineRow row)
        {
            return m_list.IndexOf(row);
        }



        public IEnumerator<TimelineRow> GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_list.GetEnumerator();
        }
    }
}
