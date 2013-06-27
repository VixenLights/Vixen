using System;
using System.Drawing;
using System.Windows.Forms;

namespace VixenApplication
{
	public partial class DataProfileForm : Form
	{
		private string defaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Vixen 3";
		private ProfileItem currentItem = null;

		public DataProfileForm()
		{
			InitializeComponent();
		}

		private void DataProfileForm_Load(object sender, EventArgs e)
		{
			PopulateProfileList();
			PopulateLoadProfileSection(true);
		}

		private void PopulateLoadProfileSection(bool firstTime)
		{
			XMLProfileSettings profile = new XMLProfileSettings();

			radioButtonAskMe.Checked = (profile.GetSetting("Profiles/LoadAction", "Ask") == "Ask");
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
					int loadItemNum = profile.GetSetting("Profiles/ProfileToLoad", 0);
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
			SaveCurrentItem();
			profile.PutSetting("Profiles/ProfileCount", comboBoxProfiles.Items.Count);
			for (int i = 0; i < comboBoxProfiles.Items.Count; i++) {
				ProfileItem item = comboBoxProfiles.Items[i] as ProfileItem;
				profile.PutSetting("Profiles/" + "Profile" + i.ToString() + "/Name", item.Name);
				profile.PutSetting("Profiles/" + "Profile" + i.ToString() + "/DataFolder", item.DataFolder);
			}

			if (radioButtonAskMe.Checked)
				profile.PutSetting("Profiles/LoadAction", "Ask");
			else
				profile.PutSetting("Profiles/LoadAction", "LoadSelected");

			if (comboBoxLoadThisProfile.SelectedIndex >= 0)
				profile.PutSetting("Profiles/ProfileToLoad", comboBoxLoadThisProfile.SelectedIndex);

			DialogResult = System.Windows.Forms.DialogResult.OK;

			Close();
		}

		private void buttonSetDataFolder_Click(object sender, EventArgs e)
		{
			folderBrowserDataFolder.SelectedPath = textBoxDataFolder.Text;
			if (folderBrowserDataFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				textBoxDataFolder.Text = folderBrowserDataFolder.SelectedPath;
			}
		}

		private void PopulateProfileList()
		{
			XMLProfileSettings profile = new XMLProfileSettings();

			int profileCount = profile.GetSetting("Profiles/ProfileCount", 0);
			if (profileCount == 0) {
				ProfileItem item = new ProfileItem();
				item.Name = "Default";
				item.DataFolder = defaultFolder;
				comboBoxProfiles.Items.Add(item);
			}
			else {
				for (int i = 0; i < profileCount; i++) {
					ProfileItem item = new ProfileItem();
					item.Name = profile.GetSetting("Profiles/" + "Profile" + i.ToString() + "/Name", "New Profile");
					item.DataFolder = profile.GetSetting("Profiles/" + "Profile" + i.ToString() + "/DataFolder", defaultFolder);
					comboBoxProfiles.Items.Add(item);
				}
			}
			comboBoxProfiles.SelectedIndex = 0;
		}

		private void SaveCurrentItem()
		{
			if (currentItem != null) {
				currentItem.Name = textBoxProfileName.Text;
				currentItem.DataFolder = textBoxDataFolder.Text;
			}
		}

		private void PopulateProfileSettings()
		{
			SaveCurrentItem();
			ProfileItem item = comboBoxProfiles.SelectedItem as ProfileItem;
			if (item != null) {
				textBoxProfileName.Text = item.Name;
				textBoxDataFolder.Text = item.DataFolder;
			}
			currentItem = item;
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
			ProfileItem item = new ProfileItem();
			item.Name = "New Profile";
			item.DataFolder = defaultFolder;
			comboBoxProfiles.Items.Add(item);
			comboBoxProfiles.SelectedIndex = comboBoxProfiles.Items.Count - 1;
			PopulateLoadProfileSection(false);
		}

		private void buttonDeleteProfile_Click(object sender, EventArgs e)
		{
			ProfileItem item = comboBoxProfiles.SelectedItem as ProfileItem;
			if (item != null) {
				if (
					MessageBox.Show(
						"Are you sure you want to delete this profile? The data folder and all its contents will remain and must be removed manually.",
						"Delete a Profile", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) ==
					System.Windows.Forms.DialogResult.Yes) {
					comboBoxProfiles.Items.Remove(item);
					if (comboBoxProfiles.Items.Count > 0) {
						comboBoxProfiles.SelectedIndex = 0;
					}
					PopulateLoadProfileSection(false);
				}
			}
		}

		// ComboBox Items don't update themselves when they're changed in the list.
		// This is my simple work-around. Just drawing the thing myself!
		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ComboBox c = sender as ComboBox;
			if (e.Index > -1) {
				ProfileItem item = c.Items[e.Index] as ProfileItem;
				if (e.State == DrawItemState.Focus)
					e.Graphics.FillRectangle(Brushes.Blue, e.Bounds);
				else
					e.Graphics.FillRectangle(new SolidBrush(e.BackColor), e.Bounds);
				e.Graphics.DrawString(item.Name, System.Drawing.SystemFonts.DefaultFont, Brushes.Black, e.Bounds);
			}
			else {
				e.Graphics.FillRectangle(new SolidBrush(c.BackColor), e.Bounds);
			}
		}

		private void textBoxProfileName_Leave(object sender, EventArgs e)
		{
			SaveCurrentItem();
		}
	}
}