using System.ComponentModel;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Controls.Wizard;
using Common.Resources;
using Common.Resources.Properties;
using Vixen.Sys;

namespace VixenModules.App.ExportWizard
{
	/// <summary>
	/// Stage 1 of the Export Wizard. Allows the user to select, add, rename, or delete export profiles.
	/// </summary>
	public partial class BulkExportConfigStage : WizardStage
	{
		private readonly BulkExportWizardData _data;
		private BindingList<ExportProfile> _profiles;

		/// <summary>
		/// Initializes a new instance of <see cref="BulkExportConfigStage"/> with the shared wizard data.
		/// </summary>
		/// <param name="data">The shared wizard data that carries profiles and the active profile.</param>
		public BulkExportConfigStage(BulkExportWizardData data)
		{
			_data = data;
			InitializeComponent();
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			btnAddProfile.Image = Tools.GetIcon(Resources.add, iconSize);
			btnAddProfile.Text = "";
			btnDeleteProfile.Image = Tools.GetIcon(Resources.delete, iconSize);
			btnDeleteProfile.Text = "";
			btnRename.Image = Tools.GetIcon(Resources.cog_edit, iconSize);
			btnRename.Text = "";
			ThemeUpdateControls.UpdateControls(this);
		}

		/// <inheritdoc/>
		public override void StageStart()
		{
			PopulateProfiles();
		}

		/// <inheritdoc/>
		public override bool CanMoveNext
		{
			get { return _data.Profiles.Count > 0 && _data.ActiveProfile != null; }
		}

		private void PopulateProfiles()
		{
			if (_data.Profiles.Count == 0)
			{
				ExportProfile profile = _data.CreateDefaultProfile();
				_data.Profiles.Add(profile);
			}

			_profiles = new BindingList<ExportProfile>(_data.Profiles);

			comboProfiles.DataSource = new BindingSource { DataSource = _profiles };
			comboProfiles.SelectedIndex = 0;

			_data.ActiveProfile = _profiles[0].Clone() as ExportProfile;

			UpdateButtonStates();
			_WizardStageChanged();
		}

		private void UpdateButtonStates()
		{
			btnAddProfile.Enabled = true;
			btnDeleteProfile.Enabled = _profiles != null && _profiles.Count > 0;
			btnRename.Enabled = comboProfiles.SelectedItem is ExportProfile;
		}

		private void comboProfiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboProfiles.SelectedItem is not ExportProfile item)
				return;

			_data.ActiveProfile = item.Clone() as ExportProfile;
			UpdateButtonStates();
			_WizardStageChanged();
		}

		private void buttonAddProfile_Click(object sender, EventArgs e)
		{
			var response = GetProfileName("New Profile", string.Empty);
			if (!response.Item1)
				return;

			ExportProfile item = _data.CreateDefaultProfile();
			item.Name = response.Item2;
			_profiles.Add(item);
			_data.ActiveProfile = item.Clone() as ExportProfile;
			comboProfiles.SelectedIndex = comboProfiles.Items.Count - 1;

			UpdateButtonStates();
			_WizardStageChanged();
		}

		private async void buttonDeleteProfile_Click(object sender, EventArgs e)
		{
			if (comboProfiles.SelectedItem is not ExportProfile item)
				return;

			var messageBox = new MessageBoxForm(
				$"Are you sure you want to delete {item.Name}?",
				"Confirm Delete", MessageBoxButtons.OKCancel, SystemIcons.Warning);
			await messageBox.ShowDialogAsync();

			if (messageBox.DialogResult != DialogResult.OK)
				return;

			_profiles.Remove(item);

			if (_profiles.Count == 0)
			{
				ExportProfile defaultProfile = _data.CreateDefaultProfile();
				defaultProfile.Name = "Default";
				_profiles.Add(defaultProfile);
			}

			comboProfiles.SelectedIndex = 0;
			_data.ActiveProfile = _profiles[0].Clone() as ExportProfile;

			try
			{
				await VixenSystem.SaveModuleConfigAsync();
			}
			catch (Exception ex)
			{
				var errorBox = new MessageBoxForm(
					$"The profile was removed but could not be saved to disk:\n{ex.Message}",
					"Save Error", MessageBoxButtons.OK, SystemIcons.Error);
				await errorBox.ShowDialogAsync();
			}

			UpdateButtonStates();
			_WizardStageChanged();
		}

		private async void btnRename_Click(object sender, EventArgs e)
		{
			if (comboProfiles.SelectedItem is not ExportProfile profile)
				return;

			var response = GetProfileName("Rename Profile", profile.Name, profile.Name);
			if (!response.Item1)
				return;

			if (response.Item2 == profile.Name)
				return;

			profile.Name = response.Item2;

			try
			{
				await VixenSystem.SaveModuleConfigAsync();
			}
			catch (Exception ex)
			{
				var errorBox = new MessageBoxForm(
					$"The profile was renamed but could not be saved to disk:\n{ex.Message}",
					"Save Error", MessageBoxButtons.OK, SystemIcons.Error);
				await errorBox.ShowDialogAsync();
			}

			UpdateButtonStates();
		}

		/// <summary>
		/// Displays a text-input dialog and returns the trimmed profile name entered by the user.
		/// </summary>
		/// <param name="dialogTitle">The title shown on the dialog window.</param>
		/// <param name="initialName">The value pre-populated in the text field.</param>
		/// <param name="excludeName">
		/// A profile name to exclude from the duplicate check. Pass the current profile name when renaming
		/// so that the profile is not considered a duplicate of itself.
		/// </param>
		/// <returns>
		/// A tuple where the first element is <see langword="true"/> if the user confirmed a valid name,
		/// and the second element is the trimmed name. Returns <see langword="false"/> and an empty string
		/// when the user cancels.
		/// </returns>
		private Tuple<bool, string> GetProfileName(string dialogTitle, string initialName, string excludeName = null)
		{
			TextDialog dialog = new TextDialog("Enter a name for the profile", dialogTitle, initialName);

			while (dialog.ShowDialog() == DialogResult.OK)
			{
				string trimmed = dialog.Response.Trim();
				if (trimmed == string.Empty)
				{
					var messageBox = new MessageBoxForm(
						"Profile name can not be blank.", "Error", MessageBoxButtons.OK, SystemIcons.Error);
					messageBox.ShowDialog();
				}
				else if (comboProfiles.Items.Cast<ExportProfile>()
					.Any(p => p.Name == trimmed && p.Name != excludeName))
				{
					var messageBox = new MessageBoxForm(
						"A profile with the name " + trimmed + @" already exists.",
						"Warning", MessageBoxButtons.OK, SystemIcons.Warning);
					messageBox.ShowDialog();
				}
				else
				{
					return new Tuple<bool, string>(true, trimmed);
				}
			}

			return new Tuple<bool, string>(false, string.Empty);
		}
	}
}
