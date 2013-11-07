using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation
{
	public class Instrumentation : IInstrumentation
	{
		private OrderedDictionary _values;

		public Instrumentation()
		{
			_values = new OrderedDictionary();
		}

		public void AddValue(IInstrumentationValue value)
		{
			Debug.Assert(value != null);
			// last one wins... old one is orphaned
			// this is nicer for providers that "re-register counters
			if (value != null) {
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
			get {
				var a = new string[_values.Keys.Count];
				_values.Keys.CopyTo(a, 0);
				return a;   
			}
		}

		public IEnumerable<IInstrumentationValue> Values
		{
			get	{
				var a = new IInstrumentationValue[_values.Keys.Count];
				_values.Values.CopyTo(a, 0);
				return a;
			}
		}

		public IInstrumentationValue GetValue(string name)
		{
			Debug.Assert(name != null);
			if( _values.Contains( name))
				return _values[name] as IInstrumentationValue;
			return null;
		}
	}
}