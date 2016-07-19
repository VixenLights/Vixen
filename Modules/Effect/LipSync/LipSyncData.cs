using System;
using System.Runtime.Serialization;
using VixenModules.App.LipSyncApp;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.LipSync
{
    [DataContract]
    internal class LipSyncData : EffectTypeModuleData
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
        }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
            LipSyncData result = new LipSyncData();
            result.StaticPhoneme = StaticPhoneme;
            result.PhonemeMapping = PhonemeMapping;
            return result;
        }
    }
}
