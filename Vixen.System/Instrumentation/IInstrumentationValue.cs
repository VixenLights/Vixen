using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Instrumentation
{
	public interface IInstrumentationValue
	{
		double Value { get; }
		string FormattedValue { get; }
		string Name { get; }
		double Minimum { get; }
		double Maximum { get; }
		void Reset();
	}
}