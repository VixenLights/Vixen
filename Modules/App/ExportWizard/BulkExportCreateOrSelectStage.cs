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
		private bool _initializing;
		
		public BulkExportCreateOrSelectStage(BulkExportWizardData data)
		{
			_data = data;
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
		}

		public override void StageStart()
		{
			_initializing = true;
			radioSelectExisting.Enabled = _data.Profiles.Any();
			PopulateProfiles();
			if (_data.ActiveProfile != null)
			{
				var index = _data.Profiles.FindIndex(x => x.Id == _data.ActiveProfile.Id);
				if (index >= 0)
				{
					comboProfiles.SelectedIndex = index;
					radioSelectExisting.Checked = true;
					radioCreateNew.Checked = false;
				}
				else
				{
					radioSelectExisting.Checked = comboProfiles.Enabled = false;
					radioCreateNew.Checked = true;
				}
			}
			else
			{
				radioCreateNew.Checked = true;
				radioSelectExisting.Checked = lblSelect.Visible = comboProfiles.Visible = false;
				_data.ActiveProfile = _data.CreateDefaultProfile();
			}

			_initializing = false;

			_WizardStageChanged();
		}

		public override bool CanMoveNext
		{
			get { return _data.ActiveProfile != null; }
		}

		private void PopulateProfiles()
		{
			_profiles = new BindingList<ExportProfile>(_data.Profiles);

			comboProfiles.DataSource = new BindingSource {DataSource = _profiles};
			
			_WizardStageChanged();
		}

		private void comboProfiles_SelectedIndexChanged(object sender, EventArgs e)
		{

			if (_initializing) return;
			ExportProfile item = comboProfiles.SelectedItem as ExportProfile;
			if (item == null)
				return;

			if (radioSelectExisting.Checked)
			{
				_data.ActiveProfile = item.Clone() as ExportProfile;
			}

			_WizardStageChanged();
		}

		private void radioCreateNew_CheckedChanged(object sender, System.EventArgs e)
		{
			if (_initializing) return;
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
			if (_initializing) return;
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
