using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using VixenModules.Property.Face;

namespace VixenModules.App.LipSyncApp
{
	[DataContract]
	public class LipSyncMapItem
	{
		public LipSyncMapItem()
		{
			PhonemeList = new Dictionary<string, Boolean>(StringComparer.OrdinalIgnoreCase);
			FaceComponents = new Dictionary<FaceComponent, bool>();
		}

		public LipSyncMapItem(string name, int stringNum)
		{
			PhonemeList = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
			Name = name;
			StringNum = stringNum;
			ElementColor = Color.White;
			ElementGuid = new Guid();
			FaceComponents = new Dictionary<FaceComponent, bool>();
		}

		public LipSyncMapItem Clone()
		{
			LipSyncMapItem retVal = new LipSyncMapItem();
			retVal.Name = Name;
			retVal.PhonemeList = new Dictionary<string, bool>(PhonemeList, StringComparer.OrdinalIgnoreCase);
			retVal.StringNum = StringNum;
			retVal.ElementColor = ElementColor;
			retVal.ElementGuid = ElementGuid;
			retVal.FaceComponents = new Dictionary<FaceComponent, bool>(FaceComponents);

			return retVal;
		}

		[DataMember]
		public int StringNum { get; set; }

		[DataMember]
		public Dictionary<string, Boolean> PhonemeList { get; set; }

		[DataMember]
		public Color ElementColor { get; set; }

		[DataMember]
		private string _stringName;

		[DataMember]
		public Guid ElementGuid { get; set; }

		[DataMember]
		public Dictionary<FaceComponent, bool> FaceComponents { get; set; }

		public string Name 
		{ 
			get
			{
				if (_stringName == null)
				{
					_stringName = "String " + StringNum;
				}
				return _stringName;
			}
		
			set
			{
				_stringName = value;
			}
		}

		public override string ToString()
		{
			return Name;
		}

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