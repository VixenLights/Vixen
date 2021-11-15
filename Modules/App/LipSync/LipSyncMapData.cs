using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using Vixen.Module;
using Vixen.Sys;
using Common.Controls.ColorManagement.ColorModels;
using VixenModules.Property.Face;

namespace VixenModules.App.LipSyncApp
{
	public class LipSyncMapData : ModuleDataModelBase
	{
		private readonly ConcurrentDictionary<string, Image> _imageCache = new ConcurrentDictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
		public const string EyesClosedTag = "_EC";

		public LipSyncMapData()
		{
			IsDefaultMapping = false;
			IsMatrix = true; //All maps are now matrix / image based
			Notes = string.Empty;
			UsingDefaults = true;
		}

		public LipSyncMapData(LipSyncMapData mapSetup)
		{
			IsCurrentLibraryMapping = mapSetup.IsCurrentLibraryMapping;
			LibraryReferenceName = (string)mapSetup.LibraryReferenceName.Clone();
			IsDefaultMapping = mapSetup.IsDefaultMapping;
			IsMatrix = mapSetup.IsMatrix;
			Notes = mapSetup.Notes;
			UsingDefaults = mapSetup.UsingDefaults;
		}

		public override IModuleDataModel Clone()
		{
			LipSyncMapData newInstance = new LipSyncMapData();
			newInstance.LibraryReferenceName = LibraryReferenceName;
			newInstance.IsDefaultMapping = false;
			newInstance.IsMatrix = IsMatrix;
			newInstance.Notes = Notes;
			newInstance.UsingDefaults = UsingDefaults;

			return newInstance;
		}

		[DataMember]
		public bool IsMatrix { get; set; }

		//Deprecated
		[DataMember(EmitDefaultValue = false)]
		[Obsolete("No longer used.", false)]
		public bool StringsAreRows { get; set; }

		//Deprecated
		[DataMember(EmitDefaultValue = false)]
		[Obsolete("No longer used.", false)]
		public List<LipSyncMapItem> MapItems { get; set; }

		[DataMember]
		public bool IsCurrentLibraryMapping { get; set; }

		[DataMember]
		public bool IsDefaultMapping { get; set; }

		[DataMember]
		protected string _libraryReferenceName;

		public string PictureDirectory => Paths.ModuleDataFilesPath + "\\LipSync\\" + LibraryReferenceName + "\\";

		[DataMember(EmitDefaultValue = false)]
		public string Notes { get; set; }

		[DataMember]
		public bool UsingDefaults { get; set; }

		
		public Image ImageForPhoneme(PhonemeType phoneme)
		{
			return ImageForPhoneme(phoneme.ToString());
		}

		public Image ImageForPhoneme(string phoneme)
		{
			if (!_imageCache.TryGetValue(phoneme, out var image))
			{
				image = RetrieveImage(Path.Combine(PictureDirectory, $"{phoneme}.bmp"));
				_imageCache.TryAdd(phoneme, image);
			}

			return image;
		}

		private static Image RetrieveImage(string filename)
		{
			using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				return Image.FromStream(fs);
			}
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

		//Deprecated
		[Obsolete("No longer used.", false)]
		public LipSyncMapItem FindMapItem(Guid id)
		{
			return MapItems.Find(x => x.ElementGuid.Equals(id));
		}

		internal void ClearImageCache()
		{
			var oldImages = _imageCache.Values;
			_imageCache.Clear();
			foreach (var oldImage in oldImages)
			{
				oldImage.Dispose();
			}
		}

		public Tuple<double, Color> ConfiguredColorAndIntensity(LipSyncMapItem item)
		{
			double intensityRetVal = 0;
			Color colorRetVal = Color.Black;

			if (!IsMatrix)
			{
				if (item != null)
				{
					HSV hsvVal = HSV.FromRGB(item.ElementColor);
					hsvVal.V = 1;
					colorRetVal = hsvVal.ToRGB();
					intensityRetVal = HSV.VFromRgb(item.ElementColor);
				}
			}
			
			return new Tuple<double, Color>(intensityRetVal, colorRetVal);
		}

		public bool PhonemeState(string phonemeName, LipSyncMapItem item)
		{
			bool retVal = false;

			item?.PhonemeList.TryGetValue(phonemeName, out retVal);

			return retVal;
		}

		public bool IsFaceComponentType(FaceComponent type, LipSyncMapItem item)
		{
			bool retVal = false;

			item?.FaceComponents.TryGetValue(type, out retVal);

			return retVal;
		}

		public override string ToString()
		{
			return LibraryReferenceName;
		}
	   
	}
}
