using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public abstract class RepeatEditor:Control
	{

		/// <summary>
		///     The Property for the Delay property.
		///     Flags:              Can be used in style rules
		///     Default Value:      Depend on SPI_GETKEYBOARDDELAY from SystemMetrics
		/// </summary>
		public static readonly DependencyProperty DelayProperty
			= DependencyProperty.Register("Delay", typeof(int), typeof(IntegerEditor),
										  new FrameworkPropertyMetadata(GetKeyboardDelay()),
										  IsDelayValid);

		/// <summary>
		///     The Property for the Interval property.
		///     Flags:              Can be used in style rules
		///     Default Value:      Depend on SPI_GETKEYBOARDSPEED from SystemMetrics
		/// </summary>
		public static readonly DependencyProperty IntervalProperty
			= DependencyProperty.Register("Interval", typeof(int), typeof(IntegerEditor),
										  new FrameworkPropertyMetadata(GetKeyboardSpeed()),
										  IsIntervalValid);


		/// <summary>
		///     Specifies the amount of time, in milliseconds, to wait before repeating begins.
		/// Must be non-negative
		/// </summary>
		[Bindable(true), Category("Behavior")]
		public int Delay
		{
			get
			{
				return (int)GetValue(DelayProperty);
			}
			set
			{
				SetValue(DelayProperty, value);
			}
		}

		/// <summary>
		///     Specifies the amount of time, in milliseconds, between repeats once repeating starts.
		/// Must be non-negative
		/// </summary>
		[Bindable(true), Category("Behavior")]
		public int Interval
		{
			get
			{
				return (int)GetValue(IntervalProperty);
			}
			set
			{
				SetValue(IntervalProperty, value);
			}
		}


		private DispatcherTimer _timer;
		private static bool IsDelayValid(object value) { return ((int)value) >= 0; }
		private static bool IsIntervalValid(object value) { return ((int)value) > 0; }

		/// <summary>
		/// Starts a _timer ticking
		/// </summary>
		protected void StartTimer()
		{
			if (_timer == null)
			{
				_timer = new DispatcherTimer();
				_timer.Tick += new EventHandler(OnTimeout);
			}
			else if (_timer.IsEnabled)
				return;

			_timer.Interval = TimeSpan.FromMilliseconds(Delay);
			_timer.Start();
		}

		/// <summary>
		/// Stops a _timer that has already started
		/// </summary>
		protected void StopTimer()
		{
			if (_timer != null)
			{
				_timer.Stop();
			}
		}

		/// <summary>
		/// This is the handler for when the repeat _timer expires. All we do
		/// is invoke a click.
		/// </summary>
		/// <param name="sender">Sender of the event</param>
		/// <param name="e">Event arguments</param>
		private void OnTimeout(object sender, EventArgs e)
		{
			TimeSpan interval = TimeSpan.FromMilliseconds(Interval);
			if (_timer.Interval != interval)
				_timer.Interval = interval;

			SetValue();
		}

		protected abstract void SetValue();

		/// <summary>
		/// Retrieves the keyboard repeat-delay setting, which is a value in the range from 0
		/// (approximately 250 ms delay) through 3 (approximately 1 second delay).
		/// The actual delay associated with each value may vary depending on the hardware.
		/// </summary>
		/// <returns></returns>
		internal static int GetKeyboardDelay()
		{
			int delay = SystemParameters.KeyboardDelay;
			// SPI_GETKEYBOARDDELAY 0,1,2,3 correspond to 250,500,750,1000ms
			if (delay < 0 || delay > 3)
				delay = 0;
			return (delay + 1) * 250;
		}

		/// <summary>
		/// Retrieves the keyboard repeat-speed setting, which is a value in the range from 0
		/// (approximately 2.5 repetitions per second) through 31 (approximately 30 repetitions per second).
		/// The actual repeat rates are hardware-dependent and may vary from a linear scale by as much as 20%
		/// </summary>
		/// <returns></returns>
		internal static int GetKeyboardSpeed()
		{
			int speed = SystemParameters.KeyboardSpeed;
			// SPI_GETKEYBOARDSPEED 0,...,31 correspond to 1000/2.5=400,...,1000/30 ms
			if (speed < 0 || speed > 31)
				speed = 31;
			return (31 - speed) * (400 - 1000 / 30) / 31 + 1000 / 30;
		}

	}
}
