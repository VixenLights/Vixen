using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Instrumentation {
	public interface IInstrumentation {
		void AddValue(IInstrumentationValue value);
		IEnumerable<string> ValueNames { get; }
		IEnumerable<IInstrumentationValue> Values { get; }
	}
}
