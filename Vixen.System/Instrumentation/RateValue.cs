using System.Diagnostics;

namespace Vixen.Instrumentation {
	abstract public class RateValue : InstrumentationValue {
		private int _count;
		private Stopwatch _stopwatch;

		protected RateValue(string name)
			: base(name) {
			_stopwatch = Stopwatch.StartNew();
		}

		// Rate - values/second.
		override protected double _GetValue() {
			long msSinceLastSample = _stopwatch.ElapsedMilliseconds;
			_stopwatch.Restart();

			int count = _count;
			_count = 0;

			if(msSinceLastSample == 0) return 0;
			
			double rate = count * (1000d / msSinceLastSample);
			return rate;
		}

		public void Increment(int value = 1) {
			_count += value;
		}

		public void Clear() {
			_count = 0;
		}
	}
}
