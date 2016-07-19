using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Candle
{
	[DataContract]
	public class CandleData : EffectTypeModuleData
	{
		public CandleData()
		{
			FlickerFrequency = 20;
			ChangePercentage = 0.2f;
			MinLevel = 0.1f;
			MaxLevel = 1;
			FlickerFrequencyDeviationCap = 0.25f;
			ChangePercentageDeviationCap = 0.5f;
			Color = Color.White;
			GroupLevel = 1;
		}

		[DataMember]
		public Color Color { get; set; }

		[DataMember]
		public int GroupLevel { get; set; }

		[DataMember]
		public int FlickerFrequency { get; set; }

		// Absolute, not relative to anything.
		[DataMember]
		public float ChangePercentage { get; set; }

		[DataMember]
		public float MinLevel { get; set; }

		[DataMember]
		public float MaxLevel { get; set; }

		[DataMember]
		public float FlickerFrequencyDeviationCap { get; set; }

		[DataMember]
		public float ChangePercentageDeviationCap { get; set; }

		[OnDeserialized]
		void OnDeserialized(StreamingContext c)
		{
			//Ensure defaults for new fields that might not be in older effects.
			if (GroupLevel <= 0)
			{
				GroupLevel = 1;
			}
			if (Color.IsEmpty)
			{
				Color = Color.White;
			}
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			return (CandleData) MemberwiseClone();
		}
	}
}