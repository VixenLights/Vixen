using System;
using System.Drawing;

namespace Timeline
{
    public class TimelineElement
    {
        public TimelineElement()
        {
        }

        /// <summary>
        /// The TimelineRow to which this element belongs, or null if not contained in a TimelineRow.
        /// </summary>
        public TimelineRow ParentRow { get; internal set; }

        public TimeSpan Offset { get; set; }
        public TimeSpan Duration { get; set; }
        public Color BackColor { get; set; }

        public object Tag { get; set; }

        public bool Select()
        {
            if (ParentRow == null || ParentRow.ParentControl == null)
                return false;
            ParentRow.ParentControl.SelectedElements.AddUnique(this);
            return true;
        }

        public bool IsSelected
        {
            get
            {
                if (ParentRow == null || ParentRow.ParentControl == null)
                    return false;
                return ParentRow.ParentControl.SelectedElements.Contains(this);
            }
        }
    }
}