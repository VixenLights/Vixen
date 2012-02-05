using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation {
	public class Instrumentation : IInstrumentation {
		private Dictionary<string, IInstrumentationValue> _values;

		public Instrumentation() {
			_values = new Dictionary<string, IInstrumentationValue>();
		}

		public void AddValue(IInstrumentationValue value) {
			if(!_values.ContainsKey(value.Name)) {
				_values[value.Name] = value;
			}
		}

		public void RemoveValue(IInstrumentationValue value) {
			_values.Remove(value.Name);
		}

		public IEnumerable<string> ValueNames {
			get { return _values.Keys.ToArray(); }
		}

		public IEnumerable<IInstrumentationValue> Values {
			get { return _values.Values.ToArray(); }
		}

		public IInstrumentationValue GetValue(string name) {
			IInstrumentationValue value;
			_values.TryGetValue(name, out value);
			return value;
		}
	}
}
