using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Instrumentation {
	abstract public class AverageValue : InstrumentationValue {
		private int _totalCount;
		private int _startTime;

		public AverageValue(string name)
			: base(name) {
			_startTime = Environment.TickCount;
		}

		// Average - values/second
		protected override double _GetValue() {
			int totalMs = Environment.TickCount - _startTime;
			double seconds = (double)totalMs / 1000;
			return _totalCount / seconds;
		}

		public void Increment() {
			_totalCount++;
		}
	}
}
