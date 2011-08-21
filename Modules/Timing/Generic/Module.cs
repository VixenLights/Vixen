using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Vixen.Module;
using Vixen.Module.Timing;

namespace Generic {
	public class Module : TimingModuleInstanceBase {
		private Stopwatch _stopwatch = new Stopwatch();
		private long _offset = 0;

		public override void Pause() {
			_stopwatch.Stop();
		}

		public override long Position {
			get { return _stopwatch.ElapsedMilliseconds + _offset; }
			set {
				if(!_stopwatch.IsRunning) {
					_offset = value;
				}
			}
		}

		public override void Resume() {
			if(!_stopwatch.IsRunning) {
				_stopwatch.Start();
			}
		}

		public override void Start() {
			if(!_stopwatch.IsRunning) {
				_stopwatch.Reset();
				_stopwatch.Start();
			}
		}

		public override void Stop() {
			_stopwatch.Stop();
		}
	}
}
