using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Sys;

namespace Vixen.Module.Input {
	abstract class InputInput : IInputInput {
		private double _value;

		public event EventHandler ValueChanged;

		public InputInput() { }

		public InputInput(Guid id) {
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

		abstract public Command GetCommand();

		virtual protected void OnValueChanged(EventArgs e) {
			if(ValueChanged != null) {
				ValueChanged(this, e);
			}
		}
	}
}
