using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Module;
using Vixen.Module.App;

namespace VixenModules.App.LipSyncApp
{
	[DataContract]
	public class LipSyncMapItem
	{
		public LipSyncMapItem()
		{
			PhonemeList = new Dictionary<string, Boolean>();
			ElementColors = new Dictionary<PhonemeType, Color>();
		}

		public LipSyncMapItem(string name, int stringNum)
		{
			PhonemeList = new Dictionary<string, bool>();
			Name = name;
			StringNum = stringNum;
			ElementColor = Color.White;
			ElementGuid = new Guid();
			ElementColors = new Dictionary<PhonemeType, Color>();
		}

		public LipSyncMapItem Clone()
		{
			LipSyncMapItem retVal = new LipSyncMapItem();
			retVal.Name = Name;
			retVal.PhonemeList = new Dictionary<string, bool>(PhonemeList);
			retVal.StringNum = StringNum;
			retVal.ElementColor = ElementColor;
			retVal.ElementGuid = ElementGuid;
			retVal.ElementColors = new Dictionary<PhonemeType, Color>(ElementColors);

			return retVal;
		}

		[DataMember]
		public int StringNum { get; set; }

		[DataMember]
		public Dictionary<string, Boolean> PhonemeList { get; set; }

		[DataMember]
		public Color ElementColor { get; set; }

		[DataMember]
		public Dictionary<PhonemeType, Color> ElementColors { get; set; }

		[DataMember]
		private string _stringName;

		[DataMember]
		public Guid ElementGuid { get; set; }

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

	}

   
}