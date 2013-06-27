using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Vixen.Module.Timing;

namespace Vixen.Sys
{
	internal class SystemClock : ITiming
	{
		private Stopwatch _time = new Stopwatch();

		public TimeSpan Position
		{
			get { return _time.Elapsed; }
			set { }
		}

		public void Start()
		{
			_time.Restart();
		}

		public void Stop()
		{
			_time.Stop();
		}

		public void Pause()
		{
			_time.Stop();
		}

		public void Resume()
		{
			_time.Start();
		}

		public bool IsRunning
		{
			get { return _time.IsRunning; }
		}

		public bool SupportsVariableSpeeds
		{
			get { return false; }
		}

		public float Speed
		{
			get { return 1; } // 1 = 100%
			set { throw new NotSupportedException(); }
		}
	}
}