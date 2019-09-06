using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Catel.IoC;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Sys;
using VixenModules.App.TimedSequenceMapper.SequencePackageExport;
using VixenModules.App.TimedSequenceMapper.SequencePackageImport;

namespace VixenModules.App.TimedSequenceMapper
{
	public class TimedSequencePackagerModule : AppModuleInstanceBase
	{
		private TimedSequencePackagerData _data;
		private TimedSequencePackagerData _workingData;
		private IApplication _application;
		private SequencePackageExportWizard _exportWizard;
		private SequencePackageImportWizard _importWizard;
		
		public static string MenuIdRoot = "SequenceImportExportRoot";
		public static string MenuIdExportName = "Export Sequence Package";
		public static string MenuIdImportName = "Import Sequence Package";

		public override void Loading()
		{
			var serviceLocator = ServiceLocator.Default;
			serviceLocator.AutoRegisterTypesViaAttributes = true;
			AddApplicationMenu();
		}

		public override void Unloading()
		{
			RemoveMenu();
		}

		public override IApplication Application
		{
			set => _application = value;
		}

		public override IModuleDataModel StaticModuleData
		{
			get => _data;
			set => _data = (TimedSequencePackagerData)value;
		}

		private void AddApplicationMenu()
		{
			if (AppSupportsCommands)
			{
				var toolsMenu = GetToolsMenu();
				var exportCommand = new AppCommand(MenuIdRoot, MenuIdExportName);
				var importCommand = new AppCommand(MenuIdRoot, MenuIdImportName);
				exportCommand.Click += ExportCommand_Click;
				importCommand.Click += ImportCommand_Click;
				toolsMenu.Add(importCommand);
				toolsMenu.Add(exportCommand);
			}
		}

		private void RemoveMenu()
		{
			if (_application?.AppCommands != null)
			{
				_application.AppCommands.Remove(MenuIdImportName);
				_application.AppCommands.Remove(MenuIdExportName);
			}
		}

		private void InitializeExportForm()
		{
			_workingData = _data.DeepCopy();
			_exportWizard = new SequencePackageExportWizard(_workingData);
			_exportWizard.WizardFinished += ExportWizard_WizardFinished;
		}

		private void InitializeImportForm()
		{
			var data = new ImportConfig();
			_importWizard = new SequencePackageImportWizard(data);
			_importWizard.WizardFinished += ImportWizard_WizardFinished;
		}

		private void ImportWizard_WizardFinished(object sender, System.EventArgs e)
		{
			_importWizard.WizardFinished -= ImportWizard_WizardFinished;
			_importWizard = null;
		}

		private async void ExportWizard_WizardFinished(object sender, System.EventArgs e)
		{
			if (_exportWizard.WizardDialogResult == DialogResult.OK)
			{
				//Update the original data model with the updated data.
				//We can't just replace it because it is cached elsewhere.
				_data.ExportOutputFile = _workingData.ExportOutputFile;
				_data.ExportIncludeAudio = _workingData.ExportIncludeAudio;
				_data.ExportSequenceFiles = _workingData.ExportSequenceFiles.ToList();
			}
			_exportWizard.WizardFinished -= ExportWizard_WizardFinished;
			_exportWizard = null;
			await VixenSystem.SaveModuleConfigAsync();
		}

		private void ImportCommand_Click(object sender, System.EventArgs e)
		{
			if (_importWizard == null)
			{
				InitializeImportForm();
			}

			_importWizard.Start(true);
		}

		private void ExportCommand_Click(object sender, System.EventArgs e)
		{
			if (_exportWizard == null)
			{
				InitializeExportForm();
			}

			_exportWizard.Start(true);
		}

		private bool AppSupportsCommands => _application?.AppCommands != null;

		private AppCommand GetToolsMenu()
		{
			AppCommand toolsMenu = _application.AppCommands.Find("Tools");
			if (toolsMenu == null)
			{
				toolsMenu = new AppCommand("Tools", "Tools");
				_application.AppCommands.Add(toolsMenu);
			}
			return toolsMenu;
		}
	}
}