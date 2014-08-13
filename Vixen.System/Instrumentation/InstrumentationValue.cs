using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Instrumentation
{
	public abstract class InstrumentationValue : IInstrumentationValue
	{
		protected InstrumentationValue(string name)
		{
			Minimum = 0;
			Maximum = 0;
			Name = name;
		}

		public double Value
		{
			get
			{
				double value = _GetValue();
				if (value < Minimum) Minimum = value;
				if (value > Maximum) Maximum = value;
				return value;
			}
		}

		public string FormattedValue
		{
			get { return _GetFormattedValue(); }
		}

		public string Name { get; private set; }

		protected abstract double _GetValue();

		protected virtual string _GetFormattedValue()
		{
			return _GetValue().ToString();
		}

		public double Minimum { get; private set; }
		public double Maximum { get; private set; }
		public abstract void Reset();
	}
}