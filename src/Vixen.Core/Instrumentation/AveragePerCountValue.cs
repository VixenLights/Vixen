namespace Vixen.Instrumentation
{
	internal class AveragePerCountValue : InstrumentationValue
	{
		private int _totalCount;
		private double _totalValue;

		protected AveragePerCountValue(string name)
			: base(name)
		{
		}

		// Average - values/count
		protected override double _GetValue()
		{
			return _totalValue/_totalCount;
		}

		public override void Reset()
		{
			_totalCount = 0;
			_totalValue = 0;
		}

		public void Increment(double value = 1)
		{
			_totalValue += value;
			_totalCount++;
		}
	}
}