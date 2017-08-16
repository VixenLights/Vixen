using System;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Sys;

namespace VixenModules.App.ExportWizard
{
	public class ExportWizardModule : AppModuleInstanceBase
	{
		private const string MenuId = "ExportWizard_Main";
		private IApplication _application;
		private BulkExportWizard _exportWizard;
		private BulkExportWizardData _data;

		public override IApplication Application
		{
			set { _application = value; }
		}

		public override void Loading()
		{
			//InitializeForm();
			_AddMenu();
		}

		public override void Unloading()
		{
			_exportWizard = null;
			_RemoveMenu();
		}

		private void InitializeForm()
		{
			_exportWizard = new BulkExportWizard(_data);
			_exportWizard.WizardFinished += ExportWizardClosed;
		}

		private void OnMainMenuOnClick(object sender, EventArgs e)
		{
			if (_exportWizard == null) {
				InitializeForm();
			}

			_exportWizard.Start(true);
		}

		private void _AddMenu()
		{
			if (_application != null
			    && _application.AppCommands != null) {
				AppCommand toolsMenu = _application.AppCommands.Find("Tools");
				if (toolsMenu == null)
				{
					toolsMenu = new AppCommand("Tools", "Tools");
					_application.AppCommands.Add(toolsMenu);
				}
				var myMenu = new AppCommand(MenuId, "Export Wizard");
				myMenu.Click += OnMainMenuOnClick;
				toolsMenu.Add(myMenu);
			}
		}

		private void _RemoveMenu()
		{
			if (_application != null
			    && _application.AppCommands != null) {
				_application.AppCommands.Remove(MenuId);
			}
		}

		private void ExportWizardClosed(object sender, EventArgs e)
		{
			if (_exportWizard.WizardDialogResult == DialogResult.Cancel)
			{
				_data.ActiveProfile = null;
			}
			_exportWizard.WizardFinished -= ExportWizardClosed;
			_exportWizard = null;
		}

		public override IModuleDataModel StaticModuleData
		{
			get { return _data; }
			set
			{
				_data = value as BulkExportWizardData;
			}
		}
	}
}