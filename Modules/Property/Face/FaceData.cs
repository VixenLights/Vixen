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
			PhonemeList = new Dictionary<string, Boolean>(StringComparer.OrdinalIgnoreCase);
			FaceComponents = new Dictionary<FaceComponent, bool>();
			DefaultColor = System.Drawing.Color.White;
		}

		public override IModuleDataModel Clone() {
			var data = new FaceData();
			data.PhonemeList = new Dictionary<string, Boolean>(PhonemeList, StringComparer.OrdinalIgnoreCase);
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

		[OnDeserialized]
		void OnDeserialized(StreamingContext c)
		{
			if (!PhonemeList.Comparer.Equals(StringComparer.OrdinalIgnoreCase))
			{
				PhonemeList = new Dictionary<string, Boolean>(PhonemeList, StringComparer.OrdinalIgnoreCase);
			}
		}
	}
}
