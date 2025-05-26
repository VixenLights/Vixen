using Common.Controls.Theme;
using Common.Resources.Properties;
using Common.Controls;

namespace VixenApplication
{
	public partial class SelectProfile : BaseForm
	{
		private string _dataFolder = string.Empty;
		private int _profileNumber;

		public SelectProfile()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			listBoxProfiles.SelectedIndexChanged += ListBoxProfiles_SelectedIndexChanged;
			listBoxProfiles.DrawMode = DrawMode.OwnerDrawVariable;
			listBoxProfiles.DrawItem += ListBoxProfilesOnDrawItem;
			listBoxProfiles.MeasureItem += ListBoxProfilesOnMeasureItem;
		}

		private void ListBoxProfilesOnMeasureItem(object? sender, MeasureItemEventArgs e)
		{
			var profile = listBoxProfiles.Items[e.Index] as ProfileItem;
			if (profile != null)
			{
				var size = e.Graphics.MeasureString(profile.Name, Font);
				e.ItemHeight = (int)Math.Ceiling(size.Height);
			}
		}

		private void ListBoxProfilesOnDrawItem(object? sender, DrawItemEventArgs e)
		{
			e.DrawBackground();
			// Define the default color of the brush as black.
			Brush enabledBrush = new SolidBrush(e.ForeColor);
			Brush disabledBrush = new SolidBrush(ThemeColorTable.ForeColorDisabled);


			if (listBoxProfiles.Items[e.Index] is ProfileItem profile)
			{
				// Draw the current item text based on the current Font 
				// and the custom brush settings.
				e.Graphics.DrawString(profile.IsLocked ? string.Format("{0} - Locked", profile.Name) : profile.Name,
					e.Font??ThemeUpdateControls.StandardFont, profile.IsLocked ? disabledBrush : enabledBrush, e.Bounds, StringFormat.GenericTypographic);
				// If the ListBox has focus, draw a focus rectangle around the selected item.
				if (!profile.IsLocked)
				{
					e.DrawFocusRectangle();
				}
			}

		}

		private bool IsSelectedProfileLocked()
		{
			if (listBoxProfiles.SelectedItem is ProfileItem profile)
			{
				if (profile.IsLocked)
				{
					listBoxProfiles.SelectedIndex = -1;
					return true;
				}
			}

			return false;
		}

		private void ListBoxProfiles_SelectedIndexChanged(object? sender, EventArgs e)
		{
			IsSelectedProfileLocked();
		}

		public string DataFolder
		{
			get { return _dataFolder; }
			set { _dataFolder = value; }
		}

		public int ProfileNumber
		{
			get { return _profileNumber; }
			set { _profileNumber = value; }
		}

		public string ProfileName { get; set; } = String.Empty;

		private void SelectProfile_Load(object sender, EventArgs e)
		{
			PopulateProfileList();
		}

		private void PopulateProfileList()
		{
			XMLProfileSettings profile = new XMLProfileSettings();
			List<ProfileItem> profiles = new List<ProfileItem>();


			listBoxProfiles.BeginUpdate();
			//Make sure we start with an empty listbox since we may repopulate after editing profiles

			listBoxProfiles.Items.Clear();
			int profileCount = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "ProfileCount", 0);
			for (int i = 0; i < profileCount; i++)
			{
				var dataFolder = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + i.ToString() + "/DataFolder", string.Empty);
				// if (!VixenApp.IsProfileLocked(dataFolder)) //Only add the profile if it is not locked.
				//{
				ProfileItem item = new ProfileItem
				{
					Name = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + i.ToString() + "/Name",
					"New Profile"),
					DataFolder = dataFolder,
					IsLocked = VixenApp.IsProfileLocked(dataFolder),
					DateLastLoaded = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + i.ToString() + "/DateLastLoaded", DateTime.MinValue),
					ProfileNumber = i
				};

				profiles.Add(item);
				//}
			}

			profiles.Sort((x, y) => y.DateLastLoaded.CompareTo(x.DateLastLoaded));
			foreach (ProfileItem item in profiles)
			{
				listBoxProfiles.Items.Add(item);
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
			if (listBoxProfiles.SelectedIndex >= 0)
			{
				if (listBoxProfiles.SelectedItem is ProfileItem item)
				{
					DataFolder = item.DataFolder;
					ProfileName = item.Name;
					ProfileNumber = item.ProfileNumber;

					XMLProfileSettings profile = new XMLProfileSettings();
					profile.PutSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + ProfileNumber.ToString() + "/DateLastLoaded", DateTime.Now);
					DialogResult = DialogResult.OK;
					Close();
				}
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
			if (f.ShowDialog() == DialogResult.OK)
				PopulateProfileList();
		}
	}
}