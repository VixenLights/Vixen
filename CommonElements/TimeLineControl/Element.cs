using System;
using System.Drawing;
using System.Collections.Generic;

namespace CommonElements.Timeline
{
	[Serializable]
    public class Element : IComparable<Element>
    {
		private TimeSpan m_startTime;
		private TimeSpan m_duration;
		private Color m_backColor = Color.White;
		private object m_tag = null;
		private bool m_selected = false;

        public Element()
        {
        }


        #region Properties

        /// <summary>
        /// Gets or sets the starting time of this element (left side).
        /// </summary>
        public TimeSpan StartTime
        {
            get { return m_startTime; }
			set
			{
				if (value < TimeSpan.Zero)
					value = TimeSpan.Zero;

				if (m_startTime == value)
					return;

				m_startTime = value;
				OnTimeChanged();
			}
        }

		/// <summary>
		/// Gets or sets the time duration of this element (width).
		/// </summary>
        public TimeSpan Duration
        {
            get { return m_duration; }
			set
			{
				if (m_duration == value)
					return;

				m_duration = value;
				OnTimeChanged();
			}
        }

		/// <summary>
		/// Gets or sets the ending time of this element (right side).
		/// Changing this value adjusts the duration. The start time is unaffected.
		/// </summary>
		public TimeSpan EndTime
		{
			get { return StartTime + Duration; }
			set { Duration = (value - StartTime); }
		}


        public Color BackColor
        {
            get { return m_backColor; }
			set { m_backColor = value; OnContentChanged(); }
        }

        public object Tag
        {
            get { return m_tag; }
			set { m_tag = value; OnContentChanged(); }
        }

		public bool Selected
		{
			get { return m_selected; }
			set
			{
				if (m_selected == value)
					return;
				
				m_selected = value;
				OnSelectedChanged();
			}
		}

		#endregion

		#region Events

        /// <summary>
        /// Occurs when some of this element's other content changes.
        /// </summary>
		public event EventHandler ContentChanged;

        /// <summary>
        /// Occurs when this element's Selected state changes.
        /// </summary>
		public event EventHandler SelectedChanged;

        /// <summary>
        /// Occurs when one of this element's time propeties changes.
        /// </summary>
		public event EventHandler TimeChanged;

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Raises the ContentChanged event.
        /// </summary>
		protected virtual void OnContentChanged()
        {
            if (ContentChanged != null)
                ContentChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the SelectedChanged event.
        /// </summary>
        protected virtual void OnSelectedChanged()
        {
            if (SelectedChanged != null)
                SelectedChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the TimeChanged event.
        /// </summary>
        protected virtual void OnTimeChanged()
        {
            if (TimeChanged != null)
                TimeChanged(this, EventArgs.Empty);
        }

		#endregion


		#region Methods

		public int CompareTo(Element other)
		{
			int rv = StartTime.CompareTo(other.StartTime);
			if (rv != 0)
				return rv;
			else
				return EndTime.CompareTo(other.EndTime);
		}

		public void MoveStartTime(TimeSpan offset)
		{
			if (m_startTime + offset < TimeSpan.Zero)
				offset = -m_startTime;

			m_duration -= offset;
			StartTime += offset;
		}

		#endregion


		#region Drawing

		public virtual Bitmap Draw(Size imageSize)
		{
			Brush b = new SolidBrush(BackColor);
			Bitmap result = new Bitmap(imageSize.Width, imageSize.Height);

			Graphics g = Graphics.FromImage(result);
			g.FillRectangle(b, 0, 0, imageSize.Width, imageSize.Height);

			// Width - bold if selected
			int b_wd = Selected ? 3 : 1;

			// Adjust the rect such that the border is completely inside it.
			Rectangle b_rect = new Rectangle(
				(b_wd / 2),
				(b_wd / 2),
				imageSize.Width - b_wd,
				imageSize.Height - b_wd
				);
			
			// Draw it!
			Pen border = new Pen(Color.Black);
			border.Width = b_wd;
			//border.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
			//graphics.DrawRectangle(border, rect);
			g.DrawRectangle(border, b_rect);

			return result;
		}
		#endregion
	}
}