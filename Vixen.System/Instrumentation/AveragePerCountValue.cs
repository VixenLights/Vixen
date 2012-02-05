namespace Vixen.Instrumentation {
	class AveragePerCountValue : InstrumentationValue {
		private int _totalCount;
		private double _totalValue;

		protected AveragePerCountValue(string name)
			: base(name) {
		}

		// Average - values/count
		protected override double _GetValue() {
			return _totalValue / _totalCount;
		}

		public void Increment(double value = 1) {
			_totalValue += value;
			_totalCount++;
		}
	}
}
