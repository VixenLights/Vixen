using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Vixen.Module.Trigger {
    [DataContract]
    public class DigitalTriggerInput : TriggerInput {
		private double _value;

        public DigitalTriggerInput(Guid id)
            : base(TriggerInputType.Digital, id) {
        }

        public DigitalTriggerInput()
            : base(TriggerInputType.Digital) {
            // Required for serialization.
        }

        [DataMember]
        public DigitalInputTriggerCondition TriggerCondition { get; set; }

		/// <summary>
		/// > 0 = Set, less than or equal to 0 = Reset
		/// </summary>
		public override double Value {
			get { return _value; }
			set {
				double existingValue = _value;
				_value = value;
				if(existingValue != value) {
					if(_IsReset(existingValue) && _IsSet(value) && TriggerCondition == DigitalInputTriggerCondition.ResetToSet) {
						OnTriggered(EventArgs.Empty);
					} else if(_IsSet(existingValue) && _IsReset(value) && TriggerCondition == DigitalInputTriggerCondition.SetToReset) {
						OnTriggered(EventArgs.Empty);
					}
				}
			}
		}

		private bool _IsSet(double value) {
			return value > 0;
		}

		private bool _IsReset(double value) {
			return value <= 0;
		}
    }
}
