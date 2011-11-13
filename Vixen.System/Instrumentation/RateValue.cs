using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Instrumentation {
	abstract public class RateValue : InstrumentationValue {
		private int _count;
		private int _lastSampleTime;

		public RateValue(string name)
			: base(name) {
			_lastSampleTime = Environment.TickCount;
		}

		// Rate - values/second.
		override protected double _GetValue() {
			int msSinceLastSample = Environment.TickCount - _lastSampleTime;
			_lastSampleTime = Environment.TickCount;

			int count = _count;
			_count = 0;

			if(msSinceLastSample == 0) return 0;
			
			double rate = count * (1000d / msSinceLastSample);
			return rate;
		}

		public void Increment() {
			_count++;
		}
	}
}
