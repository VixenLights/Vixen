using System.Threading;
using Common.Controls;
using Common.Resources;
using Common.Resources.Properties;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;

namespace VixenApplication
{
	public partial class DataZipForm : Form
	{
		private bool _working;
		private ProfileItem _item;
		private delegate void StatusDelegate(string text);
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private readonly BackgroundWorker _bw = new BackgroundWorker();

		public DataZipForm()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			buttonSetSaveFolder.Image = Tools.GetIcon(Resources.folder, 16);
			_bw.WorkerReportsProgress=true;
			_bw.WorkerSupportsCancellation = true;
			_bw.DoWork += bw_DoWork;
			_bw.ProgressChanged += backgroundWorker1_ProgressChanged;

		}

		private void DataZipForm_Load(object sender, EventArgs e)
		{
			PopulateProfileList();
		}

		#region Background Thread
		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			_working = true;
			CompressSelectedFiles();
			_working = false;
		}

		private void ArchiveProfile(List<string> profileExclusions, bool includeLogs, bool includeUserSettings)
		{
			ProfileItem item = _item;
			
			string outPath = Path.Combine(textBoxSaveFolder.Text, textBoxFileName.Text + ".zip");

			String appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Vixen");
			String logDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Vixen 3", @"logs");

			string folderFullPath = Path.GetFullPath(item.DataFolder);
			string parentFolder = folderFullPath.Split(Path.DirectorySeparatorChar).Last();

			UpdateStatus("Zipping profile please wait...");
			Archive(item.DataFolder, profileExclusions, parentFolder, outPath);
			if (includeLogs)
			{
				UpdateStatus("Zipping logs please wait...");
				Archive(logDataFolder, new List<string>(), Path.Combine(parentFolder, @"Core Logs"), outPath);	
			}
			if (includeUserSettings)
			{
				UpdateStatus("Zipping user settings please wait...");
				Archive(appDataFolder, new List<string>(), Path.Combine(parentFolder, @"User Settings"), outPath);
			}
			
		}

		private void CompressSelectedFiles()
		{
			StartCompressUIState();

			var exclusions = new List<string>();
			if (!checkBoxModule.Checked)
			{
				exclusions.Add(@"\Module Data Files");
			}
			if (!checkBoxProgram.Checked)
			{
				exclusions.Add(@"\Program");
			}
			if (!checkBoxSequence.Checked)
			{
				exclusions.Add(@"\Sequence");
			}
			if (!checkBoxSystem.Checked)
			{
				exclusions.Add(@"\SystemData");
			}
			if (!checkBoxTemplate.Checked)
			{
				exclusions.Add(@"\Template");
			}
			if (!checkBoxMedia.Checked)
			{
				exclusions.Add(@"\Media");
			}

			ArchiveProfile(exclusions, checkBoxLogs.Checked, checkBoxUserSettings.Checked);
			
			EndCompressUIState();			
		}

		private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs args)
		{
			toolStripProgressBar.Value = args.ProgressPercentage;
		}
		#endregion

		private void PopulateProfileList()
		{
			var profile = new XMLProfileSettings();

			int profileCount = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "ProfileCount", 0);
			if (profileCount == 0)
			{
				MessageBox.Show(@"Unable to locate any profiles.");
				return;
			}

			for (int i = 0; i < profileCount; i++)
			{
				var item = new ProfileItem
				{
					Name = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + i.ToString() + "/Name", ""),
					DataFolder =
						profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + i.ToString() + "/DataFolder", "")
				};
				comboBoxProfiles.Items.Add(item);
			}
			comboBoxProfiles.SelectedIndex = 0;
			textBoxFileName.Text=@"VixenProfile";
			textBoxSaveFolder.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		}

		private void buttonSetSaveFolder_Click(object sender, EventArgs e)
		{
			if (folderBrowserSaveFolder.ShowDialog() == DialogResult.OK) {
				textBoxSaveFolder.Text = folderBrowserSaveFolder.SelectedPath;
			}
		}

		private void buttonStartCancel_Click(object sender, EventArgs e)
		{
			if (_working)
			{
				_bw.CancelAsync();
				return;
			}
			_item = comboBoxProfiles.SelectedItem as ProfileItem;
			if (_item == null)
			{
				//Oops.. Get outta here
				MessageBox.Show(@"Unable to find datafolder for that profile.", @"Error");
				return;
			}

			if (textBoxSaveFolder.Text == "")
			{
				MessageBox.Show(@"Please choose a folder to create the zip file in.", @"Missing save folder");
				return;
			}
			if (textBoxFileName.Text == "")
			{
				MessageBox.Show(@"Please choose a filename for the zip file.", @"Missing Zip file name");
				return;
			}

			if (".zip".Equals(Path.GetExtension(textBoxFileName.Text)))
			{
				textBoxFileName.Text = Path.GetFileNameWithoutExtension(textBoxFileName.Text);
			}
			
			string outPath = Path.Combine(textBoxSaveFolder.Text, textBoxFileName.Text + ".zip");

			if (!Directory.Exists(textBoxSaveFolder.Text))
			{
				if (MessageBox.Show(@"The destination folder does not exist, would you like to create it ?", @"Folder not found", MessageBoxButtons.YesNo) == DialogResult.No)
				{
					return;
				}

				Directory.CreateDirectory(textBoxSaveFolder.Text);
			}
			else if (File.Exists(outPath))
			{
				if (MessageBox.Show(@"The file name you have enter already exists, do you wish to overwrite it ?", @"File exists", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					File.Delete(outPath);
				}
				else
				{
					return;
				}
			}

			buttonStartCancel.Text = "Stop";
			buttonClose.Enabled = false;
			_bw.RunWorkerAsync();
			
		}

		private void StartCompressUIState()
		{
			if (InvokeRequired)
			{
				BeginInvoke(new Action(StartCompressUIState));
			}
			else
			{
				toolStripProgressBar.Visible = true;
				UseWaitCursor = true;
				buttonStartCancel.UseWaitCursor = false;
				comboBoxProfiles.Enabled = false;
				textBoxFileName.Enabled = false;
				textBoxSaveFolder.Enabled = false;
				buttonSetSaveFolder.Enabled = false;
			}
		}

		private void EndCompressUIState()
		{
			if (InvokeRequired)
			{
				BeginInvoke(new Action(EndCompressUIState));
			}
			else
			{
				UpdateStatus("Complete");
				toolStripProgressBar.Visible = false;
				UseWaitCursor = false;
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
			BeginInvoke(new StatusDelegate(_UpdateStatus), text);
		}

		private void _UpdateStatus(string text)
		{
			toolStripStatusLabel.Text = text;	
		}

		private void Archive(string folder, IList<string> exceptions, string parentFolder, string archivePath)
		{
			string folderFullPath = Path.GetFullPath(folder);
			
			IEnumerable<string> files = Directory.EnumerateFiles(folder,
					"*.*", SearchOption.AllDirectories);
			int fileCount = files.Count();
			int filesComplete = 0;
			using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Update))
			{
				foreach (string file in files)
				{
					if (_bw.CancellationPending)
					{
						break;
					}
					if (!Excluded(file, exceptions))
					{
						try
						{
							var addFile = Path.GetFullPath(file);
							if (addFile != archivePath)
							{
								addFile = addFile.Substring(folderFullPath.Length);
								archive.CreateEntryFromFile(file, parentFolder + addFile);
								filesComplete++;
								var value = (int)(filesComplete /(double)fileCount * 100.0);
								_bw.ReportProgress(value);
							}
						}
						catch (IOException ex)
						{
							Logging.Error("An error occured adding files to archive", ex);
						}
					}
				}
			}

		}

		private static bool Excluded(string file, IList<string> exceptions)
		{
			List<String> folderNames = (from folder in exceptions
										where folder.StartsWith(@"\")
											|| folder.StartsWith(@"/")
										select folder).ToList<string>();
			if (!exceptions.Contains(Path.GetExtension(file)))
			{
				return folderNames.Any(folderException => Path.GetDirectoryName(file).Contains(folderException));
			}
			return true;
		}

	}
}
