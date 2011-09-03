using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Timeline
{
    public class TimelineControlBase : UserControl
    {
		protected TimeSpan m_timePerPixel;
		protected TimeSpan m_visibleTimeStart;


		//private Timer m_HWheelTimer = new Timer();
		//private bool m_ignoreHWheel = true;

        internal TimelineControlBase()
        {
            DoubleBuffered = true;

			TimePerPixel = TimeSpan.FromTicks(100000);
			VisibleTimeStart = TimeSpan.FromSeconds(0);

			//m_HWheelTimer.Interval = 100;
			//m_HWheelTimer.Tick += m_HWheelTimer_Tick;
        }



		#region Properties
		// These can all be overridden in derived classes if needed.

		public virtual TimeSpan VisibleTimeStart
		{
			get { return m_visibleTimeStart; }
			set { m_visibleTimeStart = value; Invalidate(); }
		}

		/// <summary>
		/// Gets or sets the amount of time represented by one horizontal pixel.
		/// </summary>
		public virtual TimeSpan TimePerPixel
		{
			get { return m_timePerPixel; }
			set { m_timePerPixel = value; Invalidate(); }
		}

		/// <summary>
		/// The amount of time currently visible.
		/// </summary> 
		public virtual TimeSpan VisibleTimeSpan
		{
			get { return TimeSpan.FromTicks(ClientSize.Width * TimePerPixel.Ticks); }
		}

		public virtual TimeSpan VisibleTimeEnd
		{
			get { return VisibleTimeStart + VisibleTimeSpan; }
			set { VisibleTimeStart = value - VisibleTimeSpan; }
		}

		#endregion


		protected Single timeToPixels(TimeSpan t)
		{
			if (TimePerPixel.Ticks == 0)
				throw new DivideByZeroException("Time per pixel is zero!");

			return (Single)t.Ticks / (Single)TimePerPixel.Ticks;
		}

		protected TimeSpan pixelsToTime(int px)
        {
            return TimeSpan.FromTicks(px * this.TimePerPixel.Ticks);
        }



		#region Horizontal Scrolling Support

		//http://www.philosophicalgeek.com/2007/07/27/mouse-tilt-wheel-horizontal-scrolling-in-c/
        private const int WM_MOUSEHWHEEL = 0x020E;

		private static Int16 HIWORD(IntPtr ptr)
		{
			Int32 val32 = ptr.ToInt32();
			return (Int16)((val32 >> 16) & 0xFFFF);
		}

		private static Int16 LOWORD(IntPtr ptr)
		{
			Int32 val32 = ptr.ToInt32();
			return (Int16)(val32 & 0xFFFF);
		} 

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			if (m.HWnd != this.Handle)
			{
				return;
			}
			switch (m.Msg)
			{
				case WM_MOUSEHWHEEL:
					mouseHWheelMsg(m.WParam, m.LParam);
					m.Result = (IntPtr)1;
					break;

				default:
					break;

			}
		}


		public event EventHandler<MouseEventArgs> MouseHWheel;

		/*
		void m_HWheelTimer_Tick(object sender, EventArgs e)
		{
			m_ignoreHWheel = true;
			m_HWheelTimer.Stop();
		}
		*/
		

		private void mouseHWheelMsg(IntPtr wParam, IntPtr lParam)
		{
			Int32 tilt = HIWORD(wParam);
			Int32 keys = LOWORD(wParam);
			Int32 x = LOWORD(lParam);
			Int32 y = HIWORD(lParam);

			//Debug.WriteLine("mouseHWheelMsg   x={0}   y={1}   keys=0x{2:X}   tilt={3}",
			//	x, y, keys, tilt);

			//TODO: We always get at least two. So Ignore the first one.
			/*
			if (m_ignoreHWheel)
			{
				m_ignoreHWheel = false;
				return;
			}
			m_HWheelTimer.Stop();
			m_HWheelTimer.Start();
			*/

			fireMouseHWheelEvent(MouseButtons.None, 0, x, y, tilt);
		}

		protected void fireMouseHWheelEvent(MouseButtons buttons, int clicks, int x, int y, int delta)
		{
			MouseEventArgs args = new MouseEventArgs(buttons, clicks, x, y, delta);

			OnMouseHWheel(args);
			//let everybody else have a crack at it
			if (MouseHWheel != null)
			{
				MouseHWheel(this, args);
			}
		}

		protected virtual void OnMouseHWheel(MouseEventArgs args)
		{
			
		}

		#endregion

	}
}
