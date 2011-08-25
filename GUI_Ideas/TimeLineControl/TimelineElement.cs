using System;
using System.Drawing;
using System.Collections.Generic;

namespace Timeline
{
    public class TimelineElement
    {
		private TimeSpan m_startTime;
		private TimeSpan m_duration;
		private Color m_backColor = Color.White;
		private object m_tag = null;
		private bool m_selected = false;

        public TimelineElement()
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
					return;
				m_startTime = value;
				_ElementChanged();
			}
        }

		/// <summary>
		/// Gets or sets the time duration of this element (width).
		/// </summary>
        public TimeSpan Duration
        {
            get { return m_duration; }
			set { m_duration = value; _ElementChanged(); }
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
			set { m_backColor = value; _ElementChanged(); }
        }

        public object Tag
        {
            get { return m_tag; }
			set { m_tag = value; _ElementChanged(); }
        }

		public bool Selected
		{
			get { return m_selected; }
			set { m_selected = value; _ElementChanged(); _ElementSelected(); }
		}

		#endregion

		#region Events

		internal static event EventHandler ElementChanged;
		internal static event EventHandler ElementSelected;

		private void _ElementChanged() { if (ElementChanged != null) ElementChanged(this, EventArgs.Empty); }
		private void _ElementSelected() { if (ElementSelected != null) ElementSelected(this, EventArgs.Empty); }

		#endregion


		public virtual void Draw(Graphics graphics, Rectangle rect)
        {
        // BODY
            // Fill
            Brush b = new SolidBrush(BackColor);
            graphics.FillRectangle(b, rect);

        // BORDER
            // Width - bold if selected
            int b_wd = Selected ? 3 : 1;

			// Adjust the rect such that the border is completely inside it.
			Rectangle b_rect = new Rectangle(
				rect.Left + (b_wd / 2),
				rect.Top + (b_wd / 2),
				rect.Width - b_wd,
				rect.Height - b_wd
				);
			
			// Draw it!
            Pen border = new Pen(Color.Black);
            border.Width = b_wd;
			//border.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
			//graphics.DrawRectangle(border, rect);
			graphics.DrawRectangle(border, b_rect);
		}
    }

	public class ElementEventArgs : EventArgs
	{
		public ElementEventArgs(TimelineElement te)
		{
			Element = te;
		}

		public TimelineElement Element { get; internal set; }
	}

	public class MultiElementEventArgs : EventArgs
	{
		public List<TimelineElement> Elements { get; internal set; }
	}
}