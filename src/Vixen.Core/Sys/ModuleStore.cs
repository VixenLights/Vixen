using Vixen.Module;
using Vixen.Services;
using Vixen.Sys.Output;

namespace Vixen.Sys
{
	public class ModuleStore
	{
		public static readonly string Directory = SystemConfig.Directory;
		public const string FileName = "ModuleStore.xml";
		public static readonly string PreviewNameBase = "PreviewStore_";
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
		/// Saves the Preview and ModuleStore files to the default location.
		/// </summary>
		/// <param name="previews">Specifies the collection of Previews to be split out.</param>
		public void Save(IEnumerable<IOutputDevice> previews = null)
		{
			// This is a SPECIAL USE-CASE only meant to help a few Users who need to recombine the preview files back into ModuleStore.xml
			// It should probably be deleted at some future time.
			if (Control.ModifierKeys.HasFlag(Keys.Shift) == true && Control.ModifierKeys.HasFlag(Keys.Control) == true)
			{
				SpecialSave();
				return;
			}

			ModuleStore filtered = new ModuleStore();

			// If there isn't a collection of previews passed in, use the System's config previews.
			if (previews == null)
			{
				previews = VixenSystem.SystemConfig.Previews;
			}

			// For each Preview that has been updated, create a new ModuleStore file with the Preview data.
			int countChanged = 0;
			foreach (var preview in previews.Where(x => x.ContentChanged == true))
			{
				filtered.LoadedFilePath = Directory + "\\" + PreviewNameBase + preview.Name + ".xml";
				filtered.InstanceData.DataModels = this.InstanceData.DataModels.Where(
						x => x.ToString() == "VixenModules.Preview.VixenPreview.VixenPreviewData" &&
								x.ModuleInstanceId == preview.Id);
				countChanged++;
				FileService.Instance.SaveModuleStoreFile(filtered, filtered.LoadedFilePath);
				preview.ContentChanged = false;
			}

			// Save the global ModuleStore file, only if needed
			if (countChanged == 0 || countChanged == previews.Count())
			{
				// Split out all BUT the Preview data and save the ModuleStore file.
				filtered.InstanceData.DataModels = this.InstanceData.DataModels.Where(x => x.ToString() != "VixenModules.Preview.VixenPreview.VixenPreviewData");
				filtered.TypeData.DataModels = this.TypeData.DataModels;
				FileService.Instance.SaveModuleStoreFile(filtered);
			}
		}

		/// <summary>
		/// This is a SPECIAL USE_CASE only meant to help a few Users who need to recombine the preview files back into ModuleStore.xml.
		/// </summary>
		private void SpecialSave()
		{
			FileService.Instance.SaveModuleStoreFile(this);
			foreach (var preview in VixenSystem.SystemConfig.Previews)
			{
				VixenSystem.DeleteModuleStorePreviewFile(preview.Name);
			}
			MessageBox.Show("Exit Vixen, then open the ModuleStore.xml file and change the first line from:\n\n<ModuleStore version=\"6\">\n\n      to\n\n<ModuleStore version=\"5\">", "Special Save Feature", MessageBoxButtons.OK, MessageBoxIcon.Information);
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