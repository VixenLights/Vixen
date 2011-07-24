using System;
using System.Collections;


namespace Timeline
{
    public class RowChangedEventArgs : EventArgs
    {
        public TimelineRow Row { get; set; }
    }

    public class TimelineRowCollection : CollectionBase
    {
        internal event EventHandler<RowChangedEventArgs> RowAdded;
        internal event EventHandler<RowChangedEventArgs> RowRemoved;

        public int Add(TimelineRow row)
        {
            int ret = List.Add(row);

            if (RowAdded != null)
                RowAdded(this, new RowChangedEventArgs { Row = row });

            return ret;
        }

        public void Remove(TimelineRow row)
        {
            List.Remove(row);

            if (RowRemoved != null)
                RowRemoved(this, new RowChangedEventArgs { Row = row });
        }


        public void Insert(int index, TimelineRow row)
        {
            List.Insert(index, row);
        }

        public bool Contains(TimelineRow row)
        {
            return List.Contains(row);
        }

        public TimelineRow this[int index]
        {
            get { return (TimelineRow)List[index]; }
            set { List[index] = value; }
        }

        public int IndexOf(TimelineRow row)
        {
            return List.IndexOf(row);
        }


    }
}
