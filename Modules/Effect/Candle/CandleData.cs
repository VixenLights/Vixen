using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Vixen.Data.Value;
using Vixen.Module;

namespace VixenModules.Effect.Candle {
	[DataContract]
	public class CandleData : ModuleDataModelBase {
		public CandleData() {
			FlickerFrequency = 20;
			ChangePercentage = new PositionValue(0.2f);
			MinLevel = new PositionValue(0.1f);
			MaxLevel = new PositionValue(1);
			FlickerFrequencyDeviationCap = new PositionValue(0.25f);
			ChangePercentageDeviationCap = new PositionValue(0.5f);
		}

		[DataMember]
		public int FlickerFrequency { get; set; }

		// Absolute, not relative to anything.
		[DataMember]
		public PositionValue ChangePercentage { get; set; }

		[DataMember]
		public PositionValue MinLevel { get; set; }

		[DataMember]
		public PositionValue MaxLevel { get; set; }

		[DataMember]
		public PositionValue FlickerFrequencyDeviationCap { get; set; }

		[DataMember]
		public PositionValue ChangePercentageDeviationCap { get; set; }

		public override IModuleDataModel Clone() {
			return (CandleData)MemberwiseClone();
		}
	}
}
