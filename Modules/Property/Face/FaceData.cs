using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Property.Face
{
	[DataContract]
	public class FaceData : ModuleDataModelBase
	{
		public FaceData()
		{
			PhonemeList = new Dictionary<string, Boolean>();
			FaceComponents = new Dictionary<FaceComponent, bool>();
			DefaultColor = System.Drawing.Color.White;
		}

		public override IModuleDataModel Clone() {
			var data = new FaceData();
			data.PhonemeList = new Dictionary<string, Boolean>(PhonemeList);
			data.FaceComponents = new Dictionary<FaceComponent, bool>(FaceComponents);
			data.DefaultColor = DefaultColor;
			return data;
		}

		[DataMember]
		public Dictionary<string, Boolean> PhonemeList { get; set; }

		[DataMember]
		public Dictionary<FaceComponent, bool> FaceComponents { get; set; }

		[DataMember]
		public System.Drawing.Color DefaultColor { get; set; }
	}
}
