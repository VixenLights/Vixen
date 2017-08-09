using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Controls.Wizard;

namespace VixenModules.App.ExportWizard
{
	public partial class BulkExportCreateOrSelectStage : WizardStage
	{
		private readonly BulkExportWizardData _data;
		private BindingList<ExportProfile> _profiles;
		
		public BulkExportCreateOrSelectStage(BulkExportWizardData data)
		{
			_data = data;
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
		}

		public override void StageStart()
		{
			radioCreateNew.Checked = true;
			radioSelectExisting.Enabled = _data.Profiles.Any();
			PopulateProfiles();
		}

		public override bool CanMoveNext
		{
			get { return _data.ActiveProfile != null; }
		}


		private void PopulateProfiles()
		{
			//if (!_data.Profiles.Any())
			//{
			//	ExportProfile profile = _data.CreateDefaultProfile();
			//	_data.Profiles.Add(profile);
			//}

			_profiles = new BindingList<ExportProfile>(_data.Profiles);

			comboProfiles.DataSource = new BindingSource {DataSource = _profiles};
			if (_profiles.Any()){
				comboProfiles.SelectedIndex = 0;
			}

			_WizardStageChanged();
		}

		private void comboProfiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			ExportProfile item = comboProfiles.SelectedItem as ExportProfile;
			if (item == null)
				return;

			if (radioSelectExisting.Checked)
			{
				_data.ActiveProfile = item;
			}

			_WizardStageChanged();
		}

		private void radioCreateNew_CheckedChanged(object sender, System.EventArgs e)
		{
			radioSelectExisting.Checked = !radioCreateNew.Checked;
			lblSelect.Visible = comboProfiles.Visible = radioSelectExisting.Checked;
			if (radioCreateNew.Checked)
			{
				_data.ActiveProfile = _data.CreateDefaultProfile();
			}

			_WizardStageChanged();
		}

		private void radioSelectExisting_CheckedChanged(object sender, System.EventArgs e)
		{
			radioCreateNew.Checked = !radioSelectExisting.Checked;
			lblSelect.Visible = comboProfiles.Visible = radioSelectExisting.Checked;
			if (radioSelectExisting.Checked)
			{
				var exportProfile = comboProfiles.SelectedItem as ExportProfile;
				if (exportProfile != null)
				{
					_data.ActiveProfile = exportProfile.Clone() as ExportProfile;
				}
			}

			_WizardStageChanged();
		}
	}
}
