using System.Collections.Generic;

namespace Vixen.Instrumentation
{
	public interface IInstrumentation
	{
		void AddValue(IInstrumentationValue value);
		void RemoveValue(IInstrumentationValue value);
		IEnumerable<string> ValueNames { get; }
		IEnumerable<IInstrumentationValue> Values { get; }
	}
}