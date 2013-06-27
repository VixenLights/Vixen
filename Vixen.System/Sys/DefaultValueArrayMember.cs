using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Vixen.Sys.Attribute;

namespace Vixen.Sys
{
	[Serializable]
	internal class DefaultValueArrayMember
	{
		private object _owner;
		private PropertyInfo[] _valueProperties;

		public DefaultValueArrayMember(object owner)
		{
			_owner = owner;
			_valueProperties =
				_owner.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof (ValueAttribute), true).Length == 1).
					ToArray();
		}

		/// <summary>
		/// Read-only array of values.  Writes to this array will not persist.
		/// Set this property's value to affect the array.
		/// </summary>
		public object[] Values
		{
			get { return _valueProperties.Select(x => x.GetValue(_owner, null)).ToArray(); }
			set
			{
				if (value.Length != _valueProperties.Length)
					throw new InvalidOperationException("Invalid number of values.  Expected " + _valueProperties.Length + ".");
				for (int i = 0; i < value.Length; i++) {
					_valueProperties[i].SetValue(_owner, value[i], null);
				}
			}
		}
	}
}