using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Controls.Wizard;
using Common.Resources;
using Common.Resources.Properties;

namespace VixenModules.App.ExportWizard
{
	public partial class BulkExportConfigStage : WizardStage
	{
		private readonly BulkExportWizardData _data;
		private BindingList<ExportProfile> _profiles;
		public BulkExportConfigStage(BulkExportWizardData data)
		{

			_data = data;
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			btnAddProfile.Image = Tools.GetIcon(Resources.add, iconSize);
			btnAddProfile.Text = "";
			btnDeleteProfile.Image = Tools.GetIcon(Resources.delete, iconSize);
			btnDeleteProfile.Text = "";
			btnRename.Image = Tools.GetIcon(Resources.cog_edit, iconSize);
			btnRename.Text = "";
			ThemeUpdateControls.UpdateControls(this);
		}

		public override void StageStart()
		{
			PopulateProfiles();
		}

		public override bool CanMoveNext
		{
			get { return _data.Profiles.Count > 0 && _data.ActiveProfile != null; }
		}

		private void PopulateProfiles()
		{
			int profileCount = _data.Profiles.Count;
			if (profileCount == 0)
			{
				ExportProfile profile = _data.CreateDefaultProfile();
				_data.Profiles.Add(profile);
			}

			_profiles = new BindingList<ExportProfile>(_data.Profiles);
			
			comboProfiles.DataSource = new BindingSource{ DataSource = _profiles};
			comboProfiles.SelectedIndex = 0;

			_WizardStageChanged();
		}

		private void comboProfiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			ExportProfile item = comboProfiles.SelectedItem as ExportProfile;
			if (item == null)
				return;

			_data.ActiveProfile = item;
		}

		private void buttonAddProfile_Click(object sender, EventArgs e)
		{
			var response = GetProfileName("New Profile");
			if (!response.Item1)
			{
				return;
			}

			ExportProfile item = _data.CreateDefaultProfile();
			item.Name = response.Item2;
			_profiles.Add(item);
			_data.ActiveProfile = item;
			comboProfiles.SelectedIndex = comboProfiles.Items.Count - 1;

			_WizardStageChanged();
		}

		private void buttonDeleteProfile_Click(object sender, EventArgs e)
		{
			ExportProfile item = comboProfiles.SelectedItem as ExportProfile;
			if (item == null)
				return;
			
			var messageBox = new MessageBoxForm("Are you sure you want to delete this profile?",
				@"Delete a Profile", MessageBoxButtons.OKCancel, SystemIcons.Warning);
			messageBox.ShowDialog();

			if (messageBox.DialogResult == DialogResult.OK)
			{
				_profiles.Remove(item);
				if (comboProfiles.Items.Count > 0)
				{
					comboProfiles.SelectedIndex = 0;
					_data.ActiveProfile = comboProfiles.SelectedItem as ExportProfile;
				}
				else
				{
					_data.ActiveProfile = null;
				}
			}

			_WizardStageChanged();
		}

		private void btnRename_Click(object sender, EventArgs e)
		{
			var response = GetProfileName("New Profile");
			if (response.Item1)
			{
				var profile = comboProfiles.SelectedItem as ExportProfile;
				if (profile != null)
				{
					profile.Name = response.Item2;
				}
			}
		}

		private Tuple<bool, string> GetProfileName(string initialName)
		{
			TextDialog dialog = new TextDialog("Enter a name for the new profile", "Profile Name", initialName);

			while (dialog.ShowDialog() == DialogResult.OK)
			{
				if (dialog.Response == string.Empty)
				{
					var messageBox = new MessageBoxForm("Profile name can not be blank.", "Error", MessageBoxButtons.OK, SystemIcons.Error);
					messageBox.ShowDialog();
				}
				else if (comboProfiles.Items.Cast<ExportProfile>().Any(items => items.Name == dialog.Response))
				{
					var messageBox = new MessageBoxForm("A profile with the name " + dialog.Response + @" already exists.", "Warning", MessageBoxButtons.OK, SystemIcons.Warning);
					messageBox.ShowDialog();
				}
				else
				{
					break;
				}

			}

			if (dialog.DialogResult == DialogResult.Cancel)
			{
				return new Tuple<bool, string>(false,String.Empty);
			}

			return new Tuple<bool, string>(true,  dialog.Response);
		}
	}
}
