using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Instrumentation {
	abstract public class PercentValue : InstrumentationValue {
		private int _totalValues;
		private int _qualifyingValues;

		protected PercentValue(string name)
			: base(name) {
		}

		// Qualifying percent of total.
		override protected double _GetValue() {
			if(_totalValues == 0) return 0;
			return (double)_qualifyingValues / _totalValues;
		}

		protected override string _GetFormattedValue() {
			return _GetValue() * 100 + "%";
		}

		public void IncrementQualifying() {
			_qualifyingValues++;
			_totalValues++;
		}

		public void IncrementUnqualifying() {
			_totalValues++;
		}
	}
}
