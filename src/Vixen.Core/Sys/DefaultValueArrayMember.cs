using System.Collections.Concurrent;
using System.Reflection;
using Vixen.Sys.Attribute;

namespace Vixen.Sys
{
	[Serializable]
	internal class DefaultValueArrayMember
	{
		private static ConcurrentDictionary<Type, PropertyInfo[]> mycache = new ConcurrentDictionary<Type, PropertyInfo[]>();

		private object _owner;
		private PropertyInfo[] _valueProperties = null;

		public DefaultValueArrayMember(object owner)
		{
			_owner = owner;
			if( mycache.ContainsKey( owner.GetType()))
			{
				_valueProperties = mycache[owner.GetType()];
		}
			else
			{
				_valueProperties =_owner.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(ValueAttribute), true).Length == 1).ToArray();
				mycache[owner.GetType()] = _valueProperties;
			}
		}

		/// <summary>
		/// Read-only array of values.  Writes to this array will not persist.
		/// Set this property's value to affect the array.
		/// </summary>
		public object[] Values
		{
			get
			{
				if (_valueProperties == null) {
					_valueProperties= 	_owner.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(ValueAttribute), true).Length == 1).
							ToArray();
				}

				return
				_valueProperties.Select(x => x.GetValue(_owner, null)).ToArray();
			}
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