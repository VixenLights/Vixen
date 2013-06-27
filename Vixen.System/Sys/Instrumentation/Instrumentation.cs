using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation
{
	public class Instrumentation : IInstrumentation
	{
		private Dictionary<string, IInstrumentationValue> _values;

		public Instrumentation()
		{
			_values = new Dictionary<string, IInstrumentationValue>();
		}

		public void AddValue(IInstrumentationValue value)
		{
			Debug.Assert(value != null);
			if (value != null && !_values.ContainsKey(value.Name)) {
				_values[value.Name] = value;
			}
		}

		public void RemoveValue(IInstrumentationValue value)
		{
			if (value != null) {
				_values.Remove(value.Name);
			}
		}

		public IEnumerable<string> ValueNames
		{
			get { return _values.Keys.ToArray(); }
		}

		public IEnumerable<IInstrumentationValue> Values
		{
			get { return _values.Values.ToArray(); }
		}

		public IInstrumentationValue GetValue(string name)
		{
			Debug.Assert(name != null);
			IInstrumentationValue value;
			_values.TryGetValue(name, out value);
			return value;
		}
	}
}