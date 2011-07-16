using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sys;
using System.Diagnostics;
using Vixen.Module;
using Vixen.Module.Timing;

namespace GenericTimer {
    public class Timer : TimingModuleInstanceBase {
        private Stopwatch _stopwatch;
		private long _offset;

        public Timer() {
            _stopwatch = new Stopwatch();
        }

        override public void Start() {
            if(!_stopwatch.IsRunning) {
                _stopwatch.Reset();
                _stopwatch.Start();
            }
        }

		override public void Stop() {
            _stopwatch.Stop();
        }

		override public void Pause() {
            _stopwatch.Stop();
        }

		override public void Resume() {
            if(!_stopwatch.IsRunning) {
                _stopwatch.Start();
            }
        }

		override public long Position {
            get { return _stopwatch.ElapsedMilliseconds + _offset; }
			set {
				if(!_stopwatch.IsRunning) {
					_offset = value;
				}
			}
        }
	}
}
