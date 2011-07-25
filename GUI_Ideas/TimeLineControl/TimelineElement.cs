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
    }
}