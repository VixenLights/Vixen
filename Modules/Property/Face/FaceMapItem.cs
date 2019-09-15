using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;

namespace VixenModules.Property.Face
{
	public class FaceMapItem
	{
		public FaceMapItem()
		{
			PhonemeList = new Dictionary<string, Boolean>();
			FaceComponents = new Dictionary<FaceComponent, bool>();
			ElementColor = System.Drawing.Color.White;
		}

		public FaceMapItem Clone()
		{
			FaceMapItem retVal = new FaceMapItem();
			retVal.PhonemeList = new Dictionary<string, bool>(PhonemeList);
			retVal.ElementColor = ElementColor;
			retVal.ElementGuid = ElementGuid;
			retVal.FaceComponents = new Dictionary<FaceComponent, bool>(FaceComponents);

			return retVal;
		}

		[DataMember]
		public Dictionary<string, Boolean> PhonemeList { get; set; }

		[DataMember]
		public System.Drawing.Color ElementColor { get; set; }

		[DataMember]
		public Guid ElementGuid { get; set; }

		[DataMember]
		public Dictionary<FaceComponent, bool> FaceComponents { get; set; }

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (FaceComponents == null)
			{
				FaceComponents = new Dictionary<FaceComponent, bool>();
				if (PhonemeList.Values.Any(x => x))
				{
					FaceComponents.Add(FaceComponent.Mouth, true);
				}
			}
		}

	}

   
}