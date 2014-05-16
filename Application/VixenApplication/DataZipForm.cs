using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Common.Resources;
using Common.Resources.Properties;
using Common.Controls;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace VixenApplication
{
	public partial class DataZipForm : Form
	{
		public DataZipForm()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			buttonSetSaveFolder.Image = Tools.GetIcon(Resources.folder, 16);
			buttonSetSaveFolder.Text = "";
			radioButtonZipEverything.Checked = true;
			groupBox2.Enabled = false;
		}

		private void DataZipForm_Load(object sender, EventArgs e)
		{
			PopulateProfileList();
		}

		private void PopulateProfileList()
		{
			XMLProfileSettings profile = new XMLProfileSettings();

			int profileCount = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "ProfileCount", 0);
			if (profileCount == 0)
			{
				MessageBox.Show("Unable to locate any profiles.");
				return;
			}
			else
			{
				for (int i = 0; i < profileCount; i++)
				{
					ProfileItem item = new ProfileItem();
					item.Name = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + i.ToString() + "/Name", "");
					item.DataFolder = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + i.ToString() + "/DataFolder", "");
					comboBoxProfiles.Items.Add(item);
				}
			}
			comboBoxProfiles.SelectedIndex = 0;
		}

		private void buttonSetSaveFolder_Click(object sender, EventArgs e)
		{
			if (folderBrowserSaveFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				textBoxSaveFolder.Text = folderBrowserSaveFolder.SelectedPath;
			}
		}

		private void buttonCreateZip_Click(object sender, EventArgs e)
		{

			if (textBoxSaveFolder.Text == "")
			{
				MessageBox.Show("Please choose a folder to create the zip file in.", "Missing save folder");
				return;
			}
			if (textBoxFileName.Text == "")
			{
				MessageBox.Show("Please choose a filename for the zip file.", "Missing Zip file name");
				return;
			}
			ProfileItem item = comboBoxProfiles.SelectedItem as ProfileItem;
			if (item == null) {
				//Oops.. Get outta here
				MessageBox.Show("Unable to find datafolder for that profile.", "Error");
				return;
			}

			string profileName = item.Name;
			string folderName = item.DataFolder;
			string outPath = textBoxSaveFolder.Text + "\\" + textBoxFileName.Text + ".zip";

			if (System.IO.File.Exists(outPath))
			{
				if (MessageBox.Show("The file name you have enter already exists, do you wish to overwrite it ?", "File exists", MessageBoxButtons.YesNo) == DialogResult.No)
					return;
			}

			if (!Directory.Exists(textBoxSaveFolder.Text))
			{
				if (MessageBox.Show("The destination folder does not exists, would you like to create it ?", "Folder not found", MessageBoxButtons.YesNo) == DialogResult.No)
				{
					return;
				}
				else
					Directory.CreateDirectory(textBoxSaveFolder.Text);
			}

			toolStripStatusLabel.Text = "Zipping Data please wait...";
			Cursor.Current = Cursors.WaitCursor;
			toolStripProgressBar.Visible = true;
			buttonCreateZip.Enabled = false;
			Application.DoEvents();

			FileStream fsOut = File.Create(outPath);
			ZipOutputStream zipStream = new ZipOutputStream(fsOut);
			zipStream.SetLevel(3);
			//zipStream.Password = "Vixen";
			int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);
			String AppDataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\Vixen";
			int AppFolderOffSet = AppDataFolder.Length + (AppDataFolder.EndsWith("\\") ? 0 : 1);
			String LogDataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\Vixen 3\\logs";
			int LogFolderOffSet = LogDataFolder.Length + (LogDataFolder.EndsWith("\\") ? 0 : 1);

			if (radioButtonZipEverything.Checked)
			{
				CompressFolder(folderName, zipStream, folderOffset);
				//Now Get the files from the AppData Folder
				CompressFolder(AppDataFolder, zipStream, AppFolderOffSet - 7, false);
				//Now Get the files from the Log folder
				CompressFolder(LogDataFolder, zipStream, LogFolderOffSet - 6, false);
			}
			else
			{
				if (checkBoxApplication.Checked)
				{
					folderName = item.DataFolder;
					CompressFolder(folderName, zipStream, folderOffset, false);
					//Now Get the files from the AppData Folder
					CompressFolder(AppDataFolder, zipStream, AppFolderOffSet - 7, false);
					//Now Get the files from the Log folder
					CompressFolder(LogDataFolder, zipStream, LogFolderOffSet - 6, false);
				}
				if (checkBoxModule.Checked)
				{
					folderName = item.DataFolder + "\\Module Data Files";
					CompressFolder(folderName, zipStream, folderOffset);
				}
				if (checkBoxProgram.Checked)
				{
					folderName = item.DataFolder + "\\Program";
					CompressFolder(folderName, zipStream, folderOffset);
				}
				if (checkBoxSequence.Checked)
				{
					folderName = item.DataFolder + "\\Sequence";
					CompressFolder(folderName, zipStream, folderOffset);
				}
				if (checkBoxSystem.Checked)
				{
					folderName = item.DataFolder + "\\SystemData";
					CompressFolder(folderName, zipStream, folderOffset);
				}
				if (checkBoxSystem.Checked)
				{
					folderName = item.DataFolder + "\\Template";
					CompressFolder(folderName, zipStream, folderOffset);
				}
			}
			
			zipStream.IsStreamOwner = true;
			zipStream.Close();
			toolStripStatusLabel.Text = "Zip File Created";
			Cursor.Current = Cursors.Default;
			toolStripProgressBar.Visible = false;
			buttonCreateZip.Enabled = true;
		}

		private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset, bool includeSubFolders = true)
		{
			if (!Directory.Exists(path))
				return;

			string[] files = Directory.GetFiles(path);
			foreach (string filename in files)
			{
				FileInfo fi = new FileInfo(filename);
				string entryName = filename.Substring(folderOffset);
				entryName = ZipEntry.CleanName(entryName);
				ZipEntry newEntry = new ZipEntry(entryName);
				newEntry.DateTime = fi.LastWriteTime;
				newEntry.Size = fi.Length;
				zipStream.PutNextEntry(newEntry);

				byte[] buffer = new byte[4096];
				using (FileStream streamReader = File.OpenRead(filename))
				{
					StreamUtils.Copy(streamReader, zipStream, buffer);
				}
				zipStream.CloseEntry();
			}
			if (includeSubFolders != true)
				return;
			string[] folders = Directory.GetDirectories(path);
			foreach (string folder in folders)
			{
					CompressFolder(folder, zipStream, folderOffset);
			}
		}

		private void radioButtonZipEverything_Click(object sender, EventArgs e)
		{
			groupBox2.Enabled = false;
		}

		private void radioButtonUsersChoice_Click(object sender, EventArgs e)
		{
			groupBox2.Enabled = true;
		}
	}
}
