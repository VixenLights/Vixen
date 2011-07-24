using System;
using System.Collections;

namespace Timeline
{
    public class ElementChangedEventArgs : EventArgs
    {
        public TimelineElement Element { get; set; }
    }

    /// <summary>
    /// A strongly-typed collection of TimelineElements
    /// </summary>
    public class TimelineElementCollection : CollectionBase
    {
        internal event EventHandler<ElementChangedEventArgs> ElementAdded;
        internal event EventHandler<ElementChangedEventArgs> ElementRemoved;

        public int Add(TimelineElement element)
        {
            int ret = List.Add(element);

            if (ElementAdded != null)
                ElementAdded(this, new ElementChangedEventArgs { Element = element });
            
            return ret;
        }

        public int AddUnique(TimelineElement element)
        {
            if (!Contains(element))
                return Add(element);
            return -1;
        }


        public void Remove(TimelineElement element)
        {
            List.Remove(element);

            if (ElementRemoved != null)
                ElementRemoved(this, new ElementChangedEventArgs { Element = element });
        }

        public bool Contains(TimelineElement element)
        {
            return List.Contains(element);
        }

        public new void Clear()
        {
            List.Clear();
        }

        public bool Empty { get { return (List.Count == 0); } }

        #region Intentionally left out
        /*
         * Intentionally not implemented function:
         *  IndexOf
         *  Insert
         *  [] operator
         */

        /*
        public TimelineElement this[int index]
        {
            get { return (TimelineElement)List[index]; }
            set { List[index] = value; }
        }
        */

        /*
        public int IndexOf(TimelineElement value)
        {
            return List.IndexOf(value);
        }
        */
        #endregion

    }
}
