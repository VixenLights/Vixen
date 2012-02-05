using System.Diagnostics;

namespace Vixen.Instrumentation {
	abstract public class AveragePerSecondValue : InstrumentationValue {
		private int _totalCount;
		private Stopwatch _stopwatch;

		protected AveragePerSecondValue(string name)
			: base(name) {
			_stopwatch = Stopwatch.StartNew();
		}

		// Average - values/second
		protected override double _GetValue() {
			long totalMs = _stopwatch.ElapsedMilliseconds;
			double seconds = (double)totalMs / 1000;
			return _totalCount / seconds;
		}

		public void Increment() {
			_totalCount++;
		}
	}
}
