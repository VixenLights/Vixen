using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Module;
using Vixen.Sys;
using VixenModules.Property.Face;

namespace VixenModules.App.LipSyncApp 
{
	[DataContract]
	internal class LipSyncMapStaticData : ModuleDataModelBase
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		[DataMember]
		public Rectangle SelectorWindowBounds { get; set; }

		[DataMember]
		private Dictionary<string, LipSyncMapData> _library;

		[DataMember]
		internal bool MapMigrationCompleted { get; set; }

		public Dictionary<string, LipSyncMapData> Library
		{
			get
			{
				if (_library == null)
				{
					_library = new Dictionary<string, LipSyncMapData>();
					LipSyncMapData mapData = new LipSyncMapData();
					mapData.LibraryReferenceName = "New Map";
					_library.Add("New Map", mapData);
					MapMigrationCompleted = true;
				}

				return _library;
			}
			set { _library = value; }
		}

		public override IModuleDataModel Clone()
		{
			LipSyncMapStaticData result = new LipSyncMapStaticData();
			result.Library = new Dictionary<string, LipSyncMapData>(Library);
			return result;
		}

		internal void MigrateMaps()
		{
			//Here we will attempt to take the non matrix maps and turn them into properties.
			foreach (var lipSyncMapData in _library)
			{
				if (!lipSyncMapData.Value.IsMatrix)
				{
					var map = lipSyncMapData.Value;
					foreach (var lipSyncMapItem in map.MapItems)
					{
						var node = VixenSystem.Nodes.GetElementNode(lipSyncMapItem.ElementGuid);
						if(node == null) continue;
						Logging.Info($"Creating properties for element {node?.Name}");
						FaceModule fm;
						if (node.Properties.Contains(FaceDescriptor.ModuleId))
						{
							fm = node.Properties.Get(FaceDescriptor.ModuleId) as FaceModule;

						}
						else
						{
							fm = node.Properties.Add(FaceDescriptor.ModuleId) as FaceModule;
						}

						if (fm == null) continue;

						fm.DefaultColor = lipSyncMapItem.ElementColor;
						fm.PhonemeList = new Dictionary<string, bool>(lipSyncMapItem.PhonemeList);
						fm.FaceComponents = new Dictionary<FaceComponent, bool>(lipSyncMapItem.FaceComponents);
					}
					
				}

				MapMigrationCompleted = true;
			}
		}
	}
}
