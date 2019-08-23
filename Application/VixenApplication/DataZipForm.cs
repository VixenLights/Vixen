using System.Drawing;
using System.Threading;
using Common.Controls;
using Common.Controls.Theme;
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
using Common.Controls.Scaling;

namespace VixenApplication
{
	public partial class DataZipForm : BaseForm
	{
		private bool _working;
		private ProfileItem _item;
		private delegate void StatusDelegate(string text);
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private readonly BackgroundWorker _bw = new BackgroundWorker();

		public DataZipForm()
		{
			InitializeComponent();
			statusStrip1.Renderer = new ThemeToolStripRenderer();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			buttonSetSaveFolder.Image = Tools.GetIcon(Resources.folder, iconSize);
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
			var success = Archive(item.DataFolder, profileExclusions, parentFolder, outPath);
			if (success && includeLogs)
			{
				UpdateStatus("Zipping logs please wait...");
				success = Archive(logDataFolder, new List<string>(), Path.Combine(parentFolder, @"Core Logs"), outPath);	
			}
			if (success && includeUserSettings)
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
				var item = new ProfileItem
				{
					Name = "Default",
					DataFolder = DataProfileForm.DefaultFolder
				};
				comboBoxProfiles.Items.Add(item);
			}
			else
			{
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
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Unable to find datafolder for that profile.", @"Error", false, false);
				messageBox.ShowDialog();
				return;
			}

			if (textBoxSaveFolder.Text == "")
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Please choose a folder to create the zip file in.", @"Missing save folder", false, false);
				messageBox.ShowDialog();
				return;
			}

			var invalidChars = Path.GetInvalidPathChars();
			if (textBoxSaveFolder.Text.Any(s => invalidChars.Contains(s)))
			{
				var messageBox = new MessageBoxForm("The folder path for the zip file contains invalid characters.", @"Invalid Folder Path.", MessageBoxButtons.OK, SystemIcons.Error);
				messageBox.ShowDialog();
				return;
			}

			if (textBoxFileName.Text == "")
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Information; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Please choose a filename for the zip file.", @"Missing Zip file name", false, false);
				messageBox.ShowDialog();
				return;
			}

			invalidChars = Path.GetInvalidFileNameChars();
			if (textBoxFileName.Text.Any(s => invalidChars.Contains(s)))
			{
				var messageBox = new MessageBoxForm("The filename for the zip file contains invalid characters.", @"Invalid Zip file name", MessageBoxButtons.OK, SystemIcons.Error);
				messageBox.ShowDialog();
				return;
			}

			if (".zip".Equals(Path.GetExtension(textBoxFileName.Text)))
			{
				textBoxFileName.Text = Path.GetFileNameWithoutExtension(textBoxFileName.Text);
			}
			
			string outPath = Path.Combine(textBoxSaveFolder.Text, textBoxFileName.Text + ".zip");

			if (!Directory.Exists(textBoxSaveFolder.Text))
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("The destination folder does not exist, would you like to create it ?", @"Folder not found", true, false);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.No)
				{
					return;
				}

				Directory.CreateDirectory(textBoxSaveFolder.Text);
			}
			else if (File.Exists(outPath))
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("The file name you have enter already exists, do you wish to overwrite it ?", @"File exists", true, false);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.OK)
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

		private bool Archive(string folder, IList<string> exceptions, string parentFolder, string archivePath)
		{
			bool success = false;
			string folderFullPath = Path.GetFullPath(folder);
			
			IEnumerable<string> files = Directory.EnumerateFiles(folder,
					"*.*", SearchOption.AllDirectories);
			int fileCount = files.Count();
			int filesComplete = 0;
			try
			{
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
					}
				}
				success = true;
			}
			catch (Exception ex)
			{
				Logging.Error(ex, "An error occurred adding files to archive");
				MessageBoxForm mbf = new MessageBoxForm($"An error occurred during the zip process.\n\r {ex.Message}","Error Zipping Files",MessageBoxButtons.OK, SystemIcons.Error);
				mbf.ShowDialog(this);
			}

			return success;

		}

		private static bool Excluded(string file, IList<string> exceptions)
		{
			//exclude the lock file
			if (file.EndsWith(".lock"))
			{
				return true;
			}

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
