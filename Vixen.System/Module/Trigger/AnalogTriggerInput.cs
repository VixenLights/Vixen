using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Vixen.Module.Trigger
{
	[DataContract]
	public class AnalogTriggerInput : TriggerInput
	{
		private double _value;
		private bool _wasTriggered = false;

		public AnalogTriggerInput(Guid id)
			: base(TriggerInputType.Analog, id)
		{
		}

		public AnalogTriggerInput()
			: base(TriggerInputType.Analog)
		{
			// Required for serialization.
		}

		[DataMember]
		public AnalogInputTriggerCondition TriggerCondition { get; set; }

		[DataMember]
		public double ThresholdValue { get; set; }

		[DataMember]
		public double RangeLow { get; set; }

		[DataMember]
		public double RangeHigh { get; set; }

		public override double Value
		{
			get { return _value; }
			set
			{
				if (_value != value) {
					_value = value;
					// Only trigger if going from the untriggered to the
					// triggered state.
					if (_IsTriggered) {
						if (!_wasTriggered) {
							OnTriggered(EventArgs.Empty);
							_wasTriggered = true;
						}
					}
					else {
						_wasTriggered = false;
					}
				}
			}
		}

		private bool _IsTriggered
		{
			get
			{
				switch (TriggerCondition) {
					case AnalogInputTriggerCondition.ExceedsThreshold:
						return _value >= ThresholdValue;
					case AnalogInputTriggerCondition.SubcedesThreshold:
						return _value <= ThresholdValue;
					case AnalogInputTriggerCondition.WithinRange:
						return (_value >= RangeLow) && (_value <= RangeHigh);
					case AnalogInputTriggerCondition.WithoutRange:
						return (_value < RangeLow) || (_value > RangeHigh);
				}
				;
				return false;
			}
		}
	}
}