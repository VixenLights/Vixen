using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.LipSyncApp;

namespace VixenModules.Property.Face
{
	[DataContract]
	public class FaceData : ModuleDataModelBase
	{
		public override IModuleDataModel Clone() {
			var data = new FaceData();
			data.Phonemes = new HashSet<PhonemeType>(Phonemes);
			return data;
		}

		[DataMember]
		public HashSet<PhonemeType> Phonemes { get; set; }

		public FaceComponent FaceComponent { get; set; }

		public Color DefaultColor { get; set; }
	}
}
