using System;
using System.Runtime.Serialization;
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
		public MappingType MappingType { get; set; }

		[DataMember]
		public String LyricData { get; set; }

		public LipSyncData()
		{
			LyricData = string.Empty;
			StaticPhoneme = PhonemeType.REST;
			PhonemeMapping = string.Empty;
			MappingType = MappingType.FaceDefinition;
			ScaleToGrid = true;
			ScalePercent = 100;
			Level = 100;
			ShowOutline = false;
			EyeMode = EyeMode.Off;
		}

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public LipSyncMode LipSyncMode { get; set; }

		[DataMember]
		public bool ScaleToGrid { get; set; }

		[DataMember]
		public int ScalePercent { get; set; }

		[DataMember]
		public Guid MarkCollectionId { get; set; }

		[DataMember]
		public int Level { get; set; }

		[DataMember]
		public bool ShowOutline { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool AllowMarkGaps { get; set; }

		[DataMember]
		public EyeMode EyeMode { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			LipSyncData result = new LipSyncData();
			result.LyricData = LyricData;
			result.StaticPhoneme = StaticPhoneme;
			result.PhonemeMapping = PhonemeMapping;
			result.ScaleToGrid = ScaleToGrid;
			result.Orientation = Orientation;
			result.Level = Level;
			result.MarkCollectionId = MarkCollectionId;
			result.LipSyncMode = LipSyncMode;
			result.MappingType = MappingType;
			result.ShowOutline = ShowOutline;
			result.EyeMode = EyeMode;
			result.AllowMarkGaps = AllowMarkGaps;
			return result;
		}
	}
}
