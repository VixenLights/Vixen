using System;
using System.IO;
using System.IO.Compression;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using Common.Resources;
using Common.Resources.Properties;
using Common.Controls;
using NLog;

namespace VixenApplication
{
	public partial class DataZipForm : Form
	{
		private int _fileCount;
		private int _filesComplete;
		private bool _doZip;
		private ProfileItem _item;
		private string _statusText;
		private static NLog.Logger Logging = LogManager.GetCurrentClassLogger();

		public DataZipForm()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			buttonSetSaveFolder.Image = Tools.GetIcon(Resources.folder, 16);
			buttonSetSaveFolder.Text = "";
			radioButtonZipEverything.Checked = true;
			groupBox2.Enabled = false;

			backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
			backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);

			_doZip = false;
			backgroundWorker1.RunWorkerAsync();
		}

		private void DataZipForm_Load(object sender, EventArgs e)
		{
			PopulateProfileList();
		}

		#region Background Thread
		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			while (true)
			{
				Thread.Sleep(500);
				if (_doZip)
				{
					CompressSelectedFiles();
					_doZip = false;
				}

			}
		}

		private void CompressSelectedFiles()
		{
			StartCompressUIState();

			ProfileItem item = _item;
			string folderName = item.DataFolder;
			string outPath = textBoxSaveFolder.Text + "\\" + textBoxFileName.Text + ".zip";

			_fileCount = 0;
			_filesComplete = 0;

			int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);
			String AppDataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\Vixen";
			int AppFolderOffSet = AppDataFolder.Length + (AppDataFolder.EndsWith("\\") ? 0 : 1);
			String LogDataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\Vixen 3\\logs";
			int LogFolderOffSet = LogDataFolder.Length + (LogDataFolder.EndsWith("\\") ? 0 : 1);

			if (radioButtonZipEverything.Checked)
			{
				_fileCount = CountFiles(folderName);
				_fileCount += CountFiles(AppDataFolder, false);
				_fileCount += CountFiles(LogDataFolder, false);

				UpdateStatus("Zipping everything please wait...");
				CompressFolder(folderName, outPath, folderOffset);
				//Now Get the files from the AppData Folder
				CompressFolder(AppDataFolder, outPath, AppFolderOffSet - 7, false);
				//Now Get the files from the Log folder
				CompressFolder(LogDataFolder, outPath, LogFolderOffSet - 6, false);
			}
			else
			{
				if (checkBoxApplication.Checked)
				{
					_fileCount = CountFiles(folderName);
					_fileCount += CountFiles(AppDataFolder, false);
					_fileCount += CountFiles(LogDataFolder, false);

					UpdateStatus("Zipping application files...");
					folderName = item.DataFolder;
					CompressFolder(folderName, outPath, folderOffset, false);
					//Now Get the files from the AppData Folder
					CompressFolder(AppDataFolder, outPath, AppFolderOffSet - 7, false);
					//Now Get the files from the Log folder
					CompressFolder(LogDataFolder, outPath, LogFolderOffSet - 6, false);
				}
				if (checkBoxModule.Checked)
				{
					UpdateStatus("Zipping module files...");
					folderName = item.DataFolder + "\\Module Data Files";
					_fileCount = CountFiles(folderName);
					CompressFolder(folderName, outPath, folderOffset);
				}
				if (checkBoxProgram.Checked)
				{
					UpdateStatus("Zipping program files...");
					folderName = item.DataFolder + "\\Program";
					_fileCount = CountFiles(folderName);
					CompressFolder(folderName, outPath, folderOffset);
				}
				if (checkBoxSequence.Checked)
				{
					UpdateStatus("Zipping sequence files...");
					folderName = item.DataFolder + "\\Sequence";
					_fileCount = CountFiles(folderName);
					CompressFolder(folderName, outPath, folderOffset);
				}
				if (checkBoxSystem.Checked)
				{
					UpdateStatus("Zipping system files...");
					folderName = item.DataFolder + "\\SystemData";
					_fileCount = CountFiles(folderName);
					CompressFolder(folderName, outPath, folderOffset);
				}
				if (checkBoxTemplate.Checked)
				{
					UpdateStatus("Zipping template files...");
					folderName = item.DataFolder + "\\Template";
					_fileCount = CountFiles(folderName);
					CompressFolder(folderName, outPath, folderOffset);
				}
			}

			EndCompressUIState();			
		}

		private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs args)
		{
			try
			{
				int value = (int)((float)_filesComplete / (float)_fileCount * 100.0);
				value = Math.Max(0, value);
				value = Math.Min(100, value);
				toolStripProgressBar.Value = value;
			}
			catch (Exception e) { }

		}
		#endregion

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

		private void buttonStartCancel_Click(object sender, EventArgs e)
		{
			if (_doZip == false)
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

				_item = comboBoxProfiles.SelectedItem as ProfileItem;
				if (_item == null)
				{
					//Oops.. Get outta here
					MessageBox.Show("Unable to find datafolder for that profile.", "Error");
					return;
				}

				string folderName = _item.DataFolder;
				string outPath = textBoxSaveFolder.Text + "\\" + textBoxFileName.Text + ".zip";

				if (System.IO.File.Exists(outPath))
				{
					if (MessageBox.Show("The file name you have enter already exists, do you wish to overwrite it ?", "File exists", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						System.IO.File.Delete(outPath);
					}
					else
					{
						return;
					}
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

				_doZip = true;
				buttonStartCancel.Text = "Stop";
				buttonClose.Enabled = false;
			}
			else
			{
				_doZip = false;
				backgroundWorker1.CancelAsync();
			}

		}

		private void StartCompressUIState()
		{
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new Action(StartCompressUIState));
			}
			else
			{
				toolStripProgressBar.Visible = true;
				this.UseWaitCursor = true;
				buttonStartCancel.UseWaitCursor = false;
				comboBoxProfiles.Enabled = false;
				textBoxFileName.Enabled = false;
				textBoxSaveFolder.Enabled = false;
				buttonSetSaveFolder.Enabled = false;
			}
		}

		private void EndCompressUIState()
		{
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new Action(EndCompressUIState));
			}
			else
			{
				UpdateStatus("Complete");
				toolStripProgressBar.Visible = false;
				this.UseWaitCursor = false;
				buttonStartCancel.Text = "Start";
				buttonClose.Enabled = true;
				comboBoxProfiles.Enabled = true;
				textBoxFileName.Enabled = true;
				textBoxSaveFolder.Enabled = true;
				buttonSetSaveFolder.Enabled = true;
			}
		}

		private void UpdateStatus(string text)
		{
			_statusText = text;
			_UpdateStatusInvoke();
		}

		private void _UpdateStatusInvoke()
		{
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new Action(_UpdateStatusInvoke));
				return;
			}
			else
			{
				toolStripStatusLabel.Text = _statusText;
			}
		}

		private int CountFiles(string folder, bool includeSubFolders = true)
		{
			int retVal = 0;
			if (Directory.Exists(folder))
			{
				retVal += Directory.GetFiles(folder).Length;
			}
  
			if (includeSubFolders == true)
			{
				string[] folders = Directory.GetDirectories(folder);
				foreach (string subFolder in folders)
				{
					retVal += CountFiles(subFolder);
				}
			}
			return retVal;
		}

		private void CompressFolder(string folder, string outPath, int folderOffset, bool includeSubFolders = true)
		{
			if (_doZip)
			{
				try
				{
					using (ZipArchive zipStream = ZipFile.Open(outPath, ZipArchiveMode.Update))
					{
						if (!Directory.Exists(folder))
							return;

						string[] files = Directory.GetFiles(folder);
						foreach (string filename in files)
						{
							if (_doZip)
							{
								string entryName = filename.Substring(folderOffset);
								try
								{
									zipStream.CreateEntryFromFile(filename, entryName);
								}
								catch (Exception e)
								{
									Logging.Warn("Zip Wizard - Could not add file:" + filename, e);
								}
								
								_filesComplete++;
								backgroundWorker1.ReportProgress(0);
							}
						}

					}
				}
				catch (Exception e)
				{
					Logging.Error("Zip Wizard - Unable to create archive", e);
				}

			}
			if (includeSubFolders != true)
				return;
			string[] folders = Directory.GetDirectories(folder);
			foreach (string subFolder in folders)
			{
					CompressFolder(subFolder, outPath, folderOffset);
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

		private void buttonClose_Click(object sender, EventArgs e)
		{

		}
	}
}
