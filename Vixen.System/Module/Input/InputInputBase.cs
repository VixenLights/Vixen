using System;
using System.Runtime.Serialization;

namespace Vixen.Module.Input {
	abstract public class InputInputBase : IInputInput {
		private double _value;

		public event EventHandler ValueChanged;

		protected InputInputBase(string name) {
			Name = name;
		}

		[DataMember]
		public string Name { get; private set; }

		virtual public double Value {
			get { return _value; }
			set {
				if(!value.Equals(_value)) {
					_value = value;
					OnValueChanged(EventArgs.Empty);
				}
			}
		}

		virtual protected void OnValueChanged(EventArgs e) {
			if(ValueChanged != null) {
				ValueChanged(this, e);
			}
		}
	}
}
