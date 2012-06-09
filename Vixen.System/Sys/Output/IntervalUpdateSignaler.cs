using System.Diagnostics;
using System.Threading;

namespace Vixen.Sys.Output {
	class IntervalUpdateSignaler : IOutputDeviceUpdateSignaler {
		private Stopwatch _stopwatch;
		private long _lastUpdateTime;

		public IntervalUpdateSignaler() {
			_stopwatch = Stopwatch.StartNew();
			_lastUpdateTime = 0;
		}

		public IOutputDevice OutputDevice { private get; set; }

		public EventWaitHandle UpdateSignal { private get; set; }

		public void RaiseSignal() {
			long timeLeft = OutputDevice.UpdateInterval - (_stopwatch.ElapsedMilliseconds - _lastUpdateTime);
			_Sleep(timeLeft);
			_lastUpdateTime = _stopwatch.ElapsedMilliseconds;
			UpdateSignal.Set();
		}

		private void _Sleep(long timeInMs) {
			if(timeInMs > 1) {
				Thread.Sleep((int)timeInMs);
			}
		}
	}
}
