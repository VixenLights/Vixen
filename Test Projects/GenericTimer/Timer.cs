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
    public class Timer : ITimingModuleInstance {
        private Stopwatch _stopwatch;
		private long _offset;

        public Timer() {
            _stopwatch = new Stopwatch();
        }

        public void Start() {
            if(!_stopwatch.IsRunning) {
                _stopwatch.Reset();
                _stopwatch.Start();
            }
        }

        public void Stop() {
            _stopwatch.Stop();
        }

        public void Pause() {
            _stopwatch.Stop();
        }

        public void Resume() {
            if(!_stopwatch.IsRunning) {
                _stopwatch.Start();
            }
        }

		public long Position {
            get { return _stopwatch.ElapsedMilliseconds + _offset; }
			set {
				if(!_stopwatch.IsRunning) {
					_offset = value;
				}
			}
        }

        public bool Setup() {
            return false;
        }

        public Guid TypeId {
            get { return Module._typeId; }
        }

        public Guid InstanceId { get; set; }

        public IModuleDataModel ModuleData { get; set; }

		public string TypeName { get; set; }

        public void Dispose() { }
    }
}
