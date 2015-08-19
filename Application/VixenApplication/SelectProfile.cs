using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Common.Controls;

namespace VixenApplication
{
	public partial class SelectProfile : Form
	{
		private string _dataFolder = string.Empty;

		public SelectProfile()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
		}

		public string DataFolder
		{
			get { return _dataFolder; }
			set { _dataFolder = value; }
		}

		private void SelectProfile_Load(object sender, EventArgs e)
		{
            PopulateProfileList();
		}

        private void PopulateProfileList()
        {
            XMLProfileSettings profile = new XMLProfileSettings();

            //Make sure we start with an empty listbox since we may repopulate after editing profiles
            listBoxProfiles.Items.Clear();
			int profileCount = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "ProfileCount", 0);
            for (int i = 0; i < profileCount; i++)
            {
                ProfileItem item = new ProfileItem();
				item.Name = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + i.ToString() + "/Name", "New Profile");
				item.DataFolder = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + i.ToString() + "/DataFolder", string.Empty);
                listBoxProfiles.Items.Add(item);
            }
        }
        private void buttonLoad_Click(object sender, EventArgs e)
		{
			LoadSelectedProfile();
		}

		private void LoadSelectedProfile()
		{
			if (listBoxProfiles.SelectedIndex >= 0) {
				ProfileItem item = listBoxProfiles.SelectedItem as ProfileItem;
				DataFolder = item.DataFolder;
				DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}

		private void listBoxProfiles_DoubleClick(object sender, EventArgs e)
		{
			LoadSelectedProfile();
		}

        private void buttonEditor_Click(object sender, EventArgs e)
        {
            DataProfileForm f = new DataProfileForm();
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                PopulateProfileList();
        }

		private void button_Paint(object sender, PaintEventArgs e)
		{
			ThemeButtonRenderer.OnPaint(sender, e, null);
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			ThemeButtonRenderer.ButtonHover = true;
			var btn = sender as Button;
			btn.Invalidate();
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			ThemeButtonRenderer.ButtonHover = false;
			var btn = sender as Button;
			btn.Invalidate();
		}
	}
}