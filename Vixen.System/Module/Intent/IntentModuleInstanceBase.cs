using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Module.Intent {
	abstract public class IntentModuleInstanceBase : ModuleInstanceBase, IIntentModuleInstance, IEqualityComparer<IIntentModuleInstance>, IEquatable<IIntentModuleInstance>, IEqualityComparer<IntentModuleInstanceBase>, IEquatable<IntentModuleInstanceBase> {
		//private PropertyInfo[] _valueProperties;
		private DefaultValueArrayMember _values;

		protected IntentModuleInstanceBase() {
			//_valueProperties = GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(ValueAttribute), true).Length == 1).ToArray();
			//_valueProperties = ValueAttribute.GetValueMembers(this);
			_values = new DefaultValueArrayMember(this);
		}

		abstract public Command GetCurrentState(TimeSpan timeOffset);

		virtual public TimeSpan TimeSpan { get; set; }

		//virtual public object[] Values { 
		//    get { return _valueProperties.Select(x => x.GetValue(this, null)).ToArray(); }
		//    set {
		//        if(value.Length != _valueProperties.Length) throw new InvalidOperationException("Invalid number of values.  Expected " + _valueProperties.Length + ".");
		//        for(int i = 0; i < value.Length; i++) {
		//            _valueProperties[i].SetValue(this, value[i], null);
		//        }
		//    }
		//}
		public virtual object[] Values {
			get { return _values.Values; }
			set { _values.Values = value; }
		}

		public bool Equals(IIntentModuleInstance x, IIntentModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IIntentModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IIntentModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(IntentModuleInstanceBase x, IntentModuleInstanceBase y) {
			return Equals(x as IIntentModuleInstance, y as IIntentModuleInstance);
		}

		public int GetHashCode(IntentModuleInstanceBase obj) {
			return GetHashCode(obj as IIntentModuleInstance);
		}

		public bool Equals(IntentModuleInstanceBase other) {
			return Equals(other as IIntentModuleInstance);
		}
	}
}
