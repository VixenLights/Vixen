namespace Vixen.Instrumentation
{
	public abstract class CountValue : InstrumentationValue
	{
		private double _value;

		protected CountValue(string name)
			: base(name)
		{
		}

		// Simple count.
		protected override double _GetValue()
		{
			return _value;
		}

		public void Add(double value)
		{
			_value += value;
		}

		public override void Reset()
		{
			_value = 0;
		}
	}
}