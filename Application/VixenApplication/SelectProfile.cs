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
	public partial class SelectProfile : BaseForm
	{
		private string _dataFolder = string.Empty;

		public SelectProfile()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
			listBoxProfiles.SelectedIndexChanged += ListBoxProfiles_SelectedIndexChanged;
			listBoxProfiles.DrawMode = DrawMode.OwnerDrawVariable;
			listBoxProfiles.DrawItem += ListBoxProfilesOnDrawItem;
			listBoxProfiles.MeasureItem += ListBoxProfilesOnMeasureItem;
		}

		private void ListBoxProfilesOnMeasureItem(object sender, MeasureItemEventArgs e)
		{
			var profile = listBoxProfiles.Items[e.Index] as ProfileItem;
			if (profile != null)
			{
				var size = e.Graphics.MeasureString(profile.Name, Font);
				e.ItemHeight = (int)Math.Ceiling(size.Height);
			}
		}

		private void ListBoxProfilesOnDrawItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();
			// Define the default color of the brush as black.
			Brush enabledBrush = new SolidBrush(e.ForeColor);
			Brush disabledBrush = new SolidBrush(ThemeColorTable.ForeColorDisabled);


			var profile = listBoxProfiles.Items[e.Index] as ProfileItem;
			if (profile != null)
			{
				// Draw the current item text based on the current Font 
				// and the custom brush settings.
				e.Graphics.DrawString(profile.IsLocked?string.Format("{0} - Locked", profile.Name):profile.Name,
					e.Font, profile.IsLocked?disabledBrush:enabledBrush, e.Bounds, StringFormat.GenericTypographic);
				// If the ListBox has focus, draw a focus rectangle around the selected item.
				if (!profile.IsLocked)
				{
					e.DrawFocusRectangle();
				}
			}
			
		}

		private bool IsSelectedProfileLocked()
		{
			var profile = listBoxProfiles.SelectedItem as ProfileItem;
			if (profile != null)
			{
				if (profile.IsLocked)
				{
					listBoxProfiles.SelectedIndex = -1;
					return true;
				}
			}

			return false;
		}

		private void ListBoxProfiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			IsSelectedProfileLocked();
		}

		public string DataFolder
		{
			get { return _dataFolder; }
			set { _dataFolder = value; }
		}

		public string ProfileName { get; set; }

		private void SelectProfile_Load(object sender, EventArgs e)
		{
			PopulateProfileList();
		}

        private void PopulateProfileList()
        {
            XMLProfileSettings profile = new XMLProfileSettings();

			listBoxProfiles.BeginUpdate();
            //Make sure we start with an empty listbox since we may repopulate after editing profiles
            listBoxProfiles.Items.Clear();
			int profileCount = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "ProfileCount", 0);
            for (int i = 0; i < profileCount; i++)
            {
				var dataFolder = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + i.ToString() + "/DataFolder", string.Empty);
	           // if (!VixenApplication.IsProfileLocked(dataFolder)) //Only add the profile if it is not locked.
	            //{
		        ProfileItem item = new ProfileItem();
		        item.Name = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + i.ToString() + "/Name",
			        "New Profile");
		        item.DataFolder = dataFolder;
	            item.IsLocked = VixenApplication.IsProfileLocked(dataFolder);
		        listBoxProfiles.Items.Add(item);

	            //}
            }

			listBoxProfiles.EndUpdate();
        }
        private void buttonLoad_Click(object sender, EventArgs e)
		{
			if (!IsSelectedProfileLocked())
			{
				LoadSelectedProfile();
			}
		}

		private void LoadSelectedProfile()
		{
			if (listBoxProfiles.SelectedIndex >= 0) {
				ProfileItem item = listBoxProfiles.SelectedItem as ProfileItem;
				DataFolder = item.DataFolder;
				ProfileName = item.Name;
				DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}

		private void listBoxProfiles_DoubleClick(object sender, EventArgs e)
		{
			if (!IsSelectedProfileLocked())
			{
				LoadSelectedProfile();
			}
		}

        private void buttonEditor_Click(object sender, EventArgs e)
        {
            DataProfileForm f = new DataProfileForm();
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                PopulateProfileList();
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
	}
}