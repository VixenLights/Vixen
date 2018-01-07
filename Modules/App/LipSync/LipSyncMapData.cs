using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Sys;
using Common.Controls.ColorManagement.ColorModels;

namespace VixenModules.App.LipSyncApp
{
	public class LipSyncMapData : ModuleDataModelBase
	{
		public LipSyncMapData()
		{
			MapItems = new List<LipSyncMapItem>();
			MapItems.Add(new LipSyncMapItem());
			IsDefaultMapping = false;
			StringsAreRows = false;
			GroupsAllowed = false;
			RecursionAllowed = true;
			IsMatrix = false;
			Notes = "";
			UsingDefaults = true;

			//Deprecated
			MatrixStringCount = 1;
			MatrixPixelsPerString = 1;
			ZoomLevel = 1;
			StartNode = "";
		}

		public LipSyncMapData(List<string> stringNames)
		{
			int stringNum = 0;
			MapItems = new List<LipSyncMapItem>();
			foreach(string stringName in stringNames)
			{
				MapItems.Add(new LipSyncMapItem(stringName,stringNum++));
			}
			StartNode = "";
			StringsAreRows = false;
			GroupsAllowed = false;
			RecursionAllowed = true;
			IsMatrix = false;
			UsingDefaults = false;

			//Deprecated
			MatrixStringCount = 1;
			MatrixPixelsPerString = 1;
			ZoomLevel = 1;
			Notes = "";
		}

		public LipSyncMapData(LipSyncMapData mapSetup)
		{
			MapItems = new List<LipSyncMapItem>(mapSetup.MapItems);
			IsCurrentLibraryMapping = mapSetup.IsCurrentLibraryMapping;
			LibraryReferenceName = (string)mapSetup.LibraryReferenceName.Clone();
			IsDefaultMapping = mapSetup.IsDefaultMapping;
			StringCount = mapSetup.StringCount;
			StartNode = mapSetup.StartNode;
			StringsAreRows = mapSetup.StringsAreRows;
			GroupsAllowed = mapSetup.GroupsAllowed;
			RecursionAllowed = mapSetup.RecursionAllowed;
			IsMatrix = mapSetup.IsMatrix;
			Notes = mapSetup.Notes;
			UsingDefaults = mapSetup.UsingDefaults;

			//Deprecated Variables
			MatrixStringCount = mapSetup.MatrixStringCount;
			MatrixPixelsPerString = mapSetup.MatrixPixelsPerString;
			ZoomLevel = mapSetup.ZoomLevel;

		}

		public override IModuleDataModel Clone()
		{
			LipSyncMapData newInstance = new LipSyncMapData();
			newInstance.MapItems = new List<LipSyncMapItem>();

			foreach (LipSyncMapItem item in MapItems)
			{
				newInstance.MapItems.Add(item.Clone());
			}
			newInstance.StringCount = StringCount;
			newInstance.LibraryReferenceName = LibraryReferenceName;
			newInstance.IsDefaultMapping = false;
			newInstance.StartNode = StartNode;
			newInstance.StringsAreRows = StringsAreRows;
			newInstance.GroupsAllowed = GroupsAllowed;
			newInstance.RecursionAllowed = RecursionAllowed;
			newInstance.IsMatrix = IsMatrix;
			newInstance.Notes = Notes;
			newInstance.UsingDefaults = UsingDefaults;

			//Deprecated Variables
			newInstance.MatrixPixelsPerString = MatrixPixelsPerString;
			newInstance.MatrixStringCount = MatrixStringCount;
			newInstance.ZoomLevel = ZoomLevel;

			return newInstance;
		}

		[DataMember]
		public int StringCount { get; set; }

		//Deprecated
		[DataMember]
		public int MatrixStringCount { get; set; }

		//Deprecated
		[DataMember]
		public int MatrixPixelsPerString { get; set; }

		[DataMember]
		public bool IsMatrix { get; set; }

		[DataMember]
		public string StartNode { get; set; }

		//Deprecated
		[DataMember]
		public int ZoomLevel { get; set; }

		[DataMember]
		public bool StringsAreRows { get; set; }

		[DataMember]
		public List<LipSyncMapItem> MapItems { get; set; }

		[DataMember]
		public bool IsCurrentLibraryMapping { get; set; }

		[DataMember]
		public bool IsDefaultMapping { get; set; }

		[DataMember]
		public bool GroupsAllowed { get; set; }

		[DataMember]
		public bool RecursionAllowed { get; set; }

		[DataMember]
		protected string _libraryReferenceName;

		[DataMember]
		public string PictureDirectory
		{
			get
			{
				return Paths.ModuleDataFilesPath + "\\LipSync\\" + LibraryReferenceName + "\\";
			}
		}

		[DataMember]
		public string Notes { get; set; }

		[DataMember]
		public bool UsingDefaults { get; set; }

		public string PictureFileName(PhonemeType phoneme)
		{
			return PictureDirectory + phoneme.ToString() + ".bmp";
		}

		public string LibraryReferenceName
		{
			get
			{
				if (_libraryReferenceName == null)
					return string.Empty;
				else
					return _libraryReferenceName;
			}
			set { _libraryReferenceName = value; }
		}

		public LipSyncMapItem FindMapItem(string itemName)
		{
			return MapItems.Find(x => x.Name.Equals(itemName));
		}

		public double ConfiguredIntensity(string itemName, PhonemeType phoneme, LipSyncMapItem item = null)
		{
			double retVal = 0;
			
			if (item == null)
			{
				item = FindMapItem(itemName);
			}
			
			if (item != null)
			{

				if (!this.IsMatrix)
				{
					if (item.PhonemeList[phoneme.ToString()] == true)
					{
						HSV hsvVal = HSV.FromRGB(new RGB(item.ElementColor));
						retVal = hsvVal.V;
					}
				}
			}
			return retVal;

		}

		public Color ConfiguredColor(string itemName, PhonemeType phoneme, LipSyncMapItem item = null)
		{
			Color retVal = Color.Black;
			
			if (item == null)
			{
				item = FindMapItem(itemName);
			}
			
			if (item != null)
			{
				if (!this.IsMatrix) 
				{
					if (item.PhonemeList[phoneme.ToString()] == true)
					{
						HSV hsvVal = HSV.FromRGB(new RGB(item.ElementColor));
						hsvVal.V = 1;
						retVal = hsvVal.ToRGB().ToArgb();
					}
				}
			}
			return retVal;
		}

		public bool PhonemeState(string itemName, string phonemeName, LipSyncMapItem item = null)
		{
			bool retVal = false;
			
			if (item == null)
			{
				item = FindMapItem(itemName);
			}
			
			if (item != null)
			{
				item.PhonemeList.TryGetValue(phonemeName, out retVal);
			}

			return retVal;
		}


		public override string ToString()
		{
			return LibraryReferenceName;
		}
	   
	}
}
