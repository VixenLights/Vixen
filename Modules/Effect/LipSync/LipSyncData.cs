using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.LipSyncApp;

namespace VixenModules.Effect.LipSync
{
    [DataContract]
    internal class LipSyncData : ModuleDataModelBase
    {
        [DataMember]
        public PhonemeType StaticPhoneme { get; set; }

        [DataMember]
        public String PhonemeMapping { get; set; }

        [DataMember]
        public String LyricData { get; set; }

        public LipSyncData()
        {
            StaticPhoneme = PhonemeType.REST;
            PhonemeMapping = "";
        }

        public override IModuleDataModel Clone()
        {
            LipSyncData result = new LipSyncData();
            result.StaticPhoneme = StaticPhoneme;
            result.PhonemeMapping = PhonemeMapping;
            return result;
        }
    }
}
