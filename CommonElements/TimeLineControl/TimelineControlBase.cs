using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace CommonElements.Timeline
{

	/// <summary>
	/// The base class for all time-related controls in the TimelineControl.
	/// </summary>
	public abstract class TimelineControlBase : UserControl
	{
		protected TimelineControlBase(TimeInfo timeinfo)
		{
			TimeInfo = timeinfo;
			TimeInfo.TimePerPixelChanged += TimePerPixelChanged;
			TimeInfo.VisibleTimeStartChanged += VisibleTimeStartChanged;
			
			DoubleBuffered = true;
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			//SetStyle(ControlStyles.UserPaint, true);
		}

		protected TimeInfo TimeInfo { get; private set; }


		/// <summary>
		/// The beginning time of the visible region.
		/// </summary>
		public virtual TimeSpan VisibleTimeStart
		{
			get { return TimeInfo.VisibleTimeStart; }
			set { TimeInfo.VisibleTimeStart = value; }
		}

		/// <summary>
		/// The amount of time represented by one (horizontal pixel)
		/// </summary>
		public virtual TimeSpan TimePerPixel
		{
			get { return TimeInfo.TimePerPixel; }
			set { TimeInfo.TimePerPixel = value; }	
		}


		/// <summary>
		/// The amount of time currently visible.
		/// </summary> 
		protected TimeSpan VisibleTimeSpan
		{
			get { return TimeSpan.FromTicks(ClientSize.Width * TimePerPixel.Ticks); }
		}


		/// <summary>
		/// The ending time of the visible region.
		/// </summary>
		protected TimeSpan VisibleTimeEnd
		{
			get { return VisibleTimeStart + VisibleTimeSpan; }
		}

		/// <summary>
		/// Converts time to pixels, based on the current TimePerPixel resolution.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		protected Single timeToPixels(TimeSpan t)
		{
			if (TimePerPixel.Ticks == 0)
				throw new DivideByZeroException("Time per pixel is zero!");

			return (Single)t.Ticks / (Single)TimePerPixel.Ticks;
		}

		/// <summary>
		/// Converts pixels to time, based on the current TimePerPixel resolution.
		/// </summary>
		/// <param name="px"></param>
		/// <returns></returns>
		protected TimeSpan pixelsToTime(int px)
		{
			return TimeSpan.FromTicks(px * TimePerPixel.Ticks);
		}


		// Overridable event handlers
		protected virtual void VisibleTimeStartChanged(object sender, EventArgs e)
		{
		}

		protected virtual void TimePerPixelChanged(object sender, EventArgs e)
		{
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

		private void mouseHWheelMsg(IntPtr wParam, IntPtr lParam)
		{
			Int32 tilt = HIWORD(wParam);
			Int32 keys = LOWORD(wParam);
			Int32 x = LOWORD(lParam);
			Int32 y = HIWORD(lParam);

			fireMouseHWheelEvent(MouseButtons.None, 0, x, y, tilt);
		}

		private void fireMouseHWheelEvent(MouseButtons buttons, int clicks, int x, int y, int delta)
		{
			MouseEventArgs args = new MouseEventArgs(buttons, clicks, x, y, delta);
			OnMouseHWheel(args);
		}

		protected virtual void OnMouseHWheel(MouseEventArgs args)
		{
			if (MouseHWheel != null)
			{
				MouseHWheel(this, args);
			}
		}

		#endregion

	}

}
