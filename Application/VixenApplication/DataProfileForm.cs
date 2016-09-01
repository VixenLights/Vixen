using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;
using Common.Controls;
using Common.Controls.Scaling;

namespace VixenApplication
{
	public partial class DataProfileForm : BaseForm
	{
		public static readonly string DefaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Vixen 3";
		private ProfileItem _currentItem;
		
		public DataProfileForm()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			Icon = Resources.Icon_Vixen3;
			int iconSize = (int)(16*ScalingTools.GetScaleFactor());
			buttonAddProfile.Image = Tools.GetIcon(Resources.add, iconSize);
			buttonAddProfile.Text = "";
			buttonDeleteProfile.Image = Tools.GetIcon(Resources.delete, iconSize);
			buttonDeleteProfile.Text = "";
			buttonSetDataFolder.Image = Tools.GetIcon(Resources.folder, iconSize);
			buttonSetDataFolder.Text = "";
			ThemeUpdateControls.UpdateControls(this);
		}

		private void DataProfileForm_Load(object sender, EventArgs e)
		{
			PopulateProfileList();
			PopulateLoadProfileSection(true);
		}

		private void PopulateLoadProfileSection(bool firstTime)
		{
			XMLProfileSettings profile = new XMLProfileSettings();

			radioButtonAskMe.Checked = (profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "LoadAction", "LoadSelected") == "Ask");
			radioButtonLoadThisProfile.Checked = (!radioButtonAskMe.Checked);

			ProfileItem selectedItem = comboBoxLoadThisProfile.SelectedItem as ProfileItem;

			comboBoxLoadThisProfile.Items.Clear();

			if (comboBoxProfiles.Items.Count == 0)
				return;

			foreach (ProfileItem item in comboBoxProfiles.Items) {
				comboBoxLoadThisProfile.Items.Add(item);
			}

			if (firstTime) {
				if (radioButtonLoadThisProfile.Checked) {
					int loadItemNum = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "ProfileToLoad", 0);
					if (loadItemNum < comboBoxLoadThisProfile.Items.Count)
						comboBoxLoadThisProfile.SelectedIndex = loadItemNum;
				}
			}
			else 
				// If the combo box was already populated, we want to select the same item.
				if (selectedItem != null && comboBoxLoadThisProfile.Items.Contains(selectedItem)) {
					comboBoxLoadThisProfile.SelectedItem = selectedItem;
				}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			XMLProfileSettings profile = new XMLProfileSettings();
            List<string> checkName = new List<string>();
            List<string> checkDataFolder = new List<string>();
			List<string> duplicateItems = new List<string>();
			List<string> checkDataPath = new List<string>();
            bool duplicateName = false;
            bool duplicateDataFolder = false;
			bool invalidDataPath = false;
            
			//Check for null values, duplicate profile name or datapath and non rooted datapath
			foreach (ProfileItem item in comboBoxProfiles.Items)
			{
				if (item.Name == null || item.DataFolder == null)
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("One or more of your profile entries has a blank name or data folder. You must correct this before continuing.",
						@"Warning - Blank Entries", false, false);
					messageBox.ShowDialog();
				}

				if (checkName.Contains(item.Name))
				{
					duplicateName = true;
					duplicateItems.Add(item.Name + ":\t " + item.DataFolder);
				}
				checkName.Add(item.Name);

				if (checkDataFolder.Contains(item.DataFolder))
				{
					duplicateDataFolder = true;
					duplicateItems.Add(item.Name + ":\t " + item.DataFolder);
				}

				checkDataFolder.Add(item.DataFolder);

				if (!Path.IsPathRooted(item.DataFolder) || !item.DataFolder.Contains(@"\"))
				{
					invalidDataPath = true;
					checkDataPath.Add(item.Name + ":\t " + item.DataFolder);
				}
			}

			if (duplicateName || duplicateDataFolder)
			{
				//Not pretty here, but works well on the dialog
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("A duplicate profile name, or data path exists." + Environment.NewLine + Environment.NewLine +
						@"The duplicate items found were:" + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, duplicateItems) + Environment.NewLine + Environment.NewLine +
						@"Click OK to accept and contine, or Cancel to go back and edit.",
						@"Warning - Duplicate Entries", false, true);
				messageBox.ShowDialog();

				if (messageBox.DialogResult != DialogResult.OK)
				{
					DialogResult = DialogResult.None;
					return;
				}
			}

			if (invalidDataPath)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("An invalid profile data folder exists." + Environment.NewLine + Environment.NewLine +
					@"The items with invalid paths were:" + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, checkDataPath) + Environment.NewLine + Environment.NewLine +
					@"Click OK to accept and contine, or Cancel to go back and edit.",
					@"Warning - Invalid Data Path", false, true);
				messageBox.ShowDialog();

				if (messageBox.DialogResult != DialogResult.OK)
				{
					DialogResult = DialogResult.None;
					return;
				}
			}

			SaveCurrentItem();
			profile.PutSetting(XMLProfileSettings.SettingType.Profiles, "ProfileCount", comboBoxProfiles.Items.Count);
			for (int i = 0; i < comboBoxProfiles.Items.Count; i++) {
				ProfileItem item = comboBoxProfiles.Items[i] as ProfileItem;
				profile.PutSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + i + "/Name", item.Name);
				profile.PutSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + i + "/DataFolder", item.DataFolder);
				if (!Directory.Exists(item.DataFolder))
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Exclamation; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("The data directory '" + item.DataFolder + "' for profile '" + item.Name + "' does not exist.  Would you like to create it?",
						Application.ProductName, true, false);
					messageBox.ShowDialog();
					if (messageBox.DialogResult == DialogResult.OK)
					{
						try
						{
							Directory.CreateDirectory(item.DataFolder);
						}
						catch (Exception)
						{
							//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
							MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
							messageBox = new MessageBoxForm("Could not create new profile directory: " + item.DataFolder + Environment.NewLine + Environment.NewLine +
								"Click OK to ignore and continue, or Cancel to go back and edit.",
								"Error", false, true);
							messageBox.ShowDialog();
							if (messageBox.DialogResult == DialogResult.Cancel)
							{
								DialogResult = DialogResult.None;
								return;
							}
						}
					}
				}
            }

			profile.PutSetting(XMLProfileSettings.SettingType.Profiles, "LoadAction",
				radioButtonAskMe.Checked ? "Ask" : "LoadSelected");

			if (comboBoxLoadThisProfile.SelectedIndex >= 0)
				profile.PutSetting(XMLProfileSettings.SettingType.Profiles, "ProfileToLoad", comboBoxLoadThisProfile.SelectedIndex);
			
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonSetDataFolder_Click(object sender, EventArgs e)
		{
			folderBrowserDataFolder.SelectedPath = textBoxDataFolder.Text;
			if (folderBrowserDataFolder.ShowDialog() == DialogResult.OK)
			{
				textBoxDataFolder.Text = folderBrowserDataFolder.SelectedPath;
			}
		}

		private void PopulateProfileList()
		{
			XMLProfileSettings profile = new XMLProfileSettings();

			int profileCount = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "ProfileCount", 0);
			if (profileCount == 0)
			{
				ProfileItem item = new ProfileItem {Name = "Default", DataFolder = DefaultFolder};
				comboBoxProfiles.Items.Add(item);
			}
			else
			{
				for (int i = 0; i < profileCount; i++)
				{
					ProfileItem item = new ProfileItem
					{
						Name = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + i + "/Name", "New Profile"),
						DataFolder = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + i + "/DataFolder", DefaultFolder)
					};
					comboBoxProfiles.Items.Add(item);
				}
			}
			comboBoxProfiles.SelectedIndex = 0;
		}

		private void SaveCurrentItem()
		{
			if (_currentItem != null) {
				_currentItem.Name = textBoxProfileName.Text;
				_currentItem.DataFolder = textBoxDataFolder.Text;
			}
		}

		private void PopulateProfileSettings()
		{
			SaveCurrentItem();
			ProfileItem item = comboBoxProfiles.SelectedItem as ProfileItem;
			if (item != null)
			{
				textBoxProfileName.Text = item.Name;
				textBoxDataFolder.Text = item.DataFolder;
			}
			_currentItem = item;
		}

		private void comboBoxProfiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			PopulateProfileSettings();
		}

		private void comboBoxProfiles_SelectionChangeCommitted(object sender, EventArgs e)
		{
			PopulateProfileSettings();
		}

		private void buttonAddProfile_Click(object sender, EventArgs e)
		{
			SaveCurrentItem();

			TextDialog dialog = new TextDialog("Enter a name for the new profile","Profile Name","New Profile");

			while (dialog.ShowDialog() == DialogResult.OK)
			{
				if (dialog.Response == string.Empty)
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Profile name can not be blank.",
						"Error", false, false);
					messageBox.ShowDialog();
				}
				
				if (comboBoxProfiles.Items.Cast<ProfileItem>().Any(items => items.Name == dialog.Response))
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("A profile with the name " + dialog.Response + @" already exists.", "", false, false);
					messageBox.ShowDialog();
				}

				if (dialog.Response != string.Empty && comboBoxProfiles.Items.Cast<ProfileItem>().All(items => items.Name != dialog.Response))
				{
					break;
				}

			}

			if (dialog.DialogResult == DialogResult.Cancel)
				return;

			ProfileItem item = new ProfileItem { Name = dialog.Response, DataFolder = DefaultFolder + " " + dialog.Response };
			comboBoxProfiles.Items.Add(item);
			comboBoxProfiles.SelectedIndex = comboBoxProfiles.Items.Count - 1;
			PopulateLoadProfileSection(false);
		}

		private void buttonDeleteProfile_Click(object sender, EventArgs e)
		{
			ProfileItem item = comboBoxProfiles.SelectedItem as ProfileItem;
			if (item == null)
				return;
			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm("Are you sure you want to delete this profile? The data folder and all its contents will remain and must be removed manually.",
				@"Delete a Profile", true, true);
			messageBox.ShowDialog();

			if (messageBox.DialogResult == DialogResult.OK)
			{
				comboBoxProfiles.Items.Remove(item);
				if (comboBoxProfiles.Items.Count > 0)
				{
					comboBoxProfiles.SelectedIndex = 0;
				}

				PopulateLoadProfileSection(false);
			}
		}

		// ComboBox Items don't update themselves when they're changed in the list.
		// This is my simple work-around. Just drawing the thing myself!
		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}

		private void textBoxProfileName_Leave(object sender, EventArgs e)
		{
			SaveCurrentItem();
		}

		private void buttonZipWizard_Click(object sender, EventArgs e)
		{
			DataZipForm f = new DataZipForm();
			f.ShowDialog();
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;

		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}