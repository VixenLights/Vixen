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

        public int Add(TimelineElement value)
        {
            int ret = List.Add(value);

            if (ElementAdded != null)
                ElementAdded(this, new ElementChangedEventArgs { Element = value });
            
            return ret;
        }

        public void Remove(TimelineElement value)
        {
            List.Remove(value);

            if (ElementRemoved != null)
                ElementRemoved(this, new ElementChangedEventArgs { Element = value });
        }

        public bool Contains(TimelineElement value)
        {
            return List.Contains(value);
        }



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
