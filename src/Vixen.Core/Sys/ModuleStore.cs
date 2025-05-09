using Vixen.Module;
using Vixen.Services;
using Vixen.Sys.Output;

namespace Vixen.Sys
{
	internal class ModuleStore
	{
		public static readonly string Directory = SystemConfig.Directory;
		public const string FileName = "ModuleStore.xml";
		public const string PreviewNameBase = "\\ModuleStore_";
		public static readonly string DefaultFilePath = Path.Combine(Directory, FileName);

		public ModuleStore()
		{
			TypeData = new ModuleStaticDataSet();
			InstanceData = new ModuleLocalDataSet();
		}

		public string LoadedFilePath { get; set; }

		public ModuleStaticDataSet TypeData { get; set; }

		public ModuleLocalDataSet InstanceData { get; set; }

		/// <summary>
		/// Saves the module store to the default location.
		/// </summary>
		/// <param name="previews">Specifies the collection of Previews to be split out.</param>
		public void Save(IEnumerable<IOutputDevice> previews = null)
		{
			ModuleStore filtered = new ModuleStore();

			// Split out all BUT the preview data and save this ModuleStore file.
			filtered.InstanceData.DataModels = this.InstanceData.DataModels.Where(x => x.ToString() != "VixenModules.Preview.VixenPreview.VixenPreviewData");
			filtered.TypeData.DataModels = this.TypeData.DataModels;
			FileService.Instance.SaveModuleStoreFile(filtered);

			// if there isn't a collection of previews passed in, use the system config previews.
			if (previews == null)
			{
				previews = VixenSystem.SystemConfig.Previews;
			}

			// For each preview, create a new ModuleStore file with the preview data.
			foreach (var preview in previews)
			{
				filtered.LoadedFilePath = Directory + PreviewNameBase + preview.Name + ".xml";
				filtered.InstanceData.DataModels = this.InstanceData.DataModels.Where(
						x => x.ToString() == "VixenModules.Preview.VixenPreview.VixenPreviewData" &&
						     x.ModuleInstanceId == preview.Id);
				FileService.Instance.SaveModuleStoreFile(filtered, filtered.LoadedFilePath);
			}

			filtered.InstanceData.Dispose();
		}

		/// <summary>
		/// Combines two ModuleStore objects into one. The left ModuleStore is the base and the right ModuleStore is the adder
		/// </summary>
		/// <param name="left">Specifies the base ModuleStore</param>
		/// <param name="right">Specifies the adder ModuleStore</param>
		/// <returns></returns>
		public static ModuleStore operator +(ModuleStore left, ModuleStore right)
		{
			var moduleStore = new ModuleStore();
			moduleStore.TypeData = left.TypeData;
			moduleStore.InstanceData = left.InstanceData;

			if (right != null)
			{
				foreach (var dataModel in right.TypeData.DataModels)
				{
					moduleStore.TypeData.AddTo(dataModel.ModuleTypeId, dataModel.ModuleInstanceId, dataModel);
				}

				foreach (var dataModel in right.InstanceData.DataModels)
				{
					moduleStore.InstanceData.AddTo(dataModel.ModuleTypeId, dataModel.ModuleInstanceId, dataModel);
				}
			}

			return moduleStore;
		}
	}
}