using System;
using System.Runtime.Serialization;
using VixenModules.App.Curves;
using VixenModules.App.LipSyncApp;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.LipSync
{
	[DataContract]
	public class LipSyncData : EffectTypeModuleData
	{
		[DataMember]
		public PhonemeType StaticPhoneme { get; set; }

		[DataMember]
		public String PhonemeMapping { get; set; }

		[DataMember]
		public String LyricData { get; set; }

		public LipSyncData()
		{
			LyricData = string.Empty;
			StaticPhoneme = PhonemeType.REST;
			PhonemeMapping = string.Empty;
			ScaleToGrid = true;
			ScalePercent = 100;
			Level = 100;
		}

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public bool ScaleToGrid { get; set; }

		[DataMember]
		public int ScalePercent { get; set; }

		[DataMember]
		public int Level { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			LipSyncData result = new LipSyncData();
			result.StaticPhoneme = StaticPhoneme;
			result.PhonemeMapping = PhonemeMapping;
			result.ScaleToGrid = ScaleToGrid;
			result.Orientation = Orientation;
			result.Level = Level;
			return result;
		}
	}
}
