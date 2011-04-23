using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Vixen.Module.Timing;

namespace Vixen.Sys {
	class SystemClock : ITiming {
		private Stopwatch _time = new Stopwatch();

		public long Position {
			get { return _time.ElapsedMilliseconds; }
			set { }
		}

		public void Start() {
			_time.Restart();
		}

		public void Stop() {
			_time.Stop();
		}

		public void Pause() {
			_time.Stop();
		}

		public void Resume() {
			_time.Start();
		}

		public bool IsRunning {
			get { return _time.IsRunning; }
		}
	}
}
