using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace Vixen.Module.Input {
	abstract class InputInputBase : IInputInput {
		private double _value;

		public event EventHandler ValueChanged;

		public InputInputBase() { }

		public InputInputBase(Guid id) {
			this.Id = id;
		}

		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		virtual public double Value {
			get { return _value; }
			set {
				if(value != _value) {
					_value = value;
					OnValueChanged(EventArgs.Empty);
				}
			}
		}

		abstract public IEffectModuleInstance Effect { get; }

		virtual protected void OnValueChanged(EventArgs e) {
			if(ValueChanged != null) {
				ValueChanged(this, e);
			}
		}
	}
}
