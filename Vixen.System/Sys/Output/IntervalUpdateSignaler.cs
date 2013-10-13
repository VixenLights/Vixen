using System.Diagnostics;
using System.Threading;

namespace Vixen.Sys.Output
{
	internal class IntervalUpdateSignaler : IOutputDeviceUpdateSignaler
	{
		private Stopwatch _stopwatch;
		private long _lastUpdateTime;
		private long _nextUpdateTime;

		public IntervalUpdateSignaler()
		{
			_stopwatch = Stopwatch.StartNew();
			_lastUpdateTime = 0;
			_nextUpdateTime = 0;
		}

		public IOutputDevice OutputDevice { private get; set; }

		public EventWaitHandle UpdateSignal { private get; set; }

		public void RaiseSignal()
		{
			if (_nextUpdateTime == 0)
				_nextUpdateTime = _stopwatch.ElapsedMilliseconds + OutputDevice.UpdateInterval;
			while (_stopwatch.ElapsedMilliseconds < _nextUpdateTime)
			{
				Thread.Sleep(1);
			}
			_nextUpdateTime += OutputDevice.UpdateInterval;

//			long timeLeft = OutputDevice.UpdateInterval - (_stopwatch.ElapsedMilliseconds - _lastUpdateTime);
//			_Sleep(timeLeft);
//			_lastUpdateTime = _stopwatch.ElapsedMilliseconds;
//			UpdateSignal.Set();
		}

		private void _Sleep(long timeInMs)
		{
			if (timeInMs > 1) {
				Thread.Sleep((int) timeInMs);
			}
		}
	}
}