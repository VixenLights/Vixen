namespace Vixen.Instrumentation
{
	public abstract class DoubleValue : InstrumentationValue
	{
		private double _value;

		protected DoubleValue(string name)
			: base(name)
		{
		}

		public void Set(double value)
		{
			_value = value;
		}

		protected override double _GetValue()
		{
			return _value;
		}
	}
}