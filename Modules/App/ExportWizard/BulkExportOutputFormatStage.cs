using System;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Controls.Wizard;
using Common.Resources;
using Common.Resources.Properties;
using NLog;

namespace VixenModules.App.ExportWizard
{
	public partial class BulkExportOutputFormatStage : WizardStage
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private readonly BulkExportWizardData _data;
		
		public BulkExportOutputFormatStage(BulkExportWizardData data)
		{
			_data = data;
			InitializeComponent();
			
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());

			btnOuputFolderSelect.Image = Tools.GetIcon(Resources.folder_explore, iconSize);
			btnOuputFolderSelect.Text = "";

			btnAudioOutputFolder.Image = Tools.GetIcon(Resources.folder_explore, iconSize);
			btnAudioOutputFolder.Text = "";

			btnFalconUniverseFolder.Image = Tools.GetIcon(Resources.folder_explore, iconSize);
			btnFalconUniverseFolder.Text = "";

			ThemeUpdateControls.UpdateControls(this);

			txtFalconInfo.BackColor = ThemeColorTable.BackgroundColor;
			SizeFalconInfo();
		}

		private void SizeFalconInfo()
		{
			// amount of padding to add
			const int padding = 3;
			// get number of lines (first line is 0, so add 1)
			int numLines = txtFalconInfo.GetLineFromCharIndex(txtFalconInfo.TextLength) + 1;
			// get border thickness
			int border = txtFalconInfo.Height - txtFalconInfo.ClientSize.Height;
			// set height (height of one line * number of lines + spacing)
			txtFalconInfo.Height = txtFalconInfo.Font.Height * numLines + padding + border;
		}

		public override void StageStart()
		{
			outputFormatComboBox.Items.Clear();
			outputFormatComboBox.Items.AddRange(_data.Export.FormatTypes);
			outputFormatComboBox.Sorted = true;
			outputFormatComboBox.SelectedItem = _data.ActiveProfile.Format;

			resolutionComboBox.SelectedItem = _data.ActiveProfile.Interval.ToString();
			txtOutputFolder.Text = _data.ActiveProfile.OutputFolder;

			chkFppIncludeAudio.Checked = chkIncludeAudio.Checked = _data.ActiveProfile.IncludeAudio;
			chkRenameAudio.Enabled = btnAudioOutputFolder.Enabled = lblAudioExportPath.Enabled = txtAudioOutputFolder.Enabled = _data.ActiveProfile.IncludeAudio;
			chkRenameAudio.Checked = _data.ActiveProfile.RenameAudio;
			
			grpFalcon.Visible = _data.ActiveProfile.IsFalconFormat;
			grpAudio.Visible = grpSequence.Visible = !grpFalcon.Visible;
			chkCreateUniverseFile.Checked = _data.ActiveProfile.CreateUniverseFile;
			chkBackupUniverseFile.Checked = _data.ActiveProfile.BackupUniverseFile;
			txtFalconOutputFolder.Text = _data.ActiveProfile.FalconOutputFolder;

			txtAudioOutputFolder.Text = _data.ActiveProfile.AudioOutputFolder;

			_WizardStageChanged();
		}

		private void btnOuputFolderSelect_Click(object sender, EventArgs e)
		{
			var folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
			folderBrowserDialog.SelectedPath = _data.ActiveProfile.OutputFolder;
			folderBrowserDialog.Description = @"Select the sequence export location";

			DialogResult dr = folderBrowserDialog.ShowDialog();
			if (dr == DialogResult.OK)
			{
				_data.ActiveProfile.OutputFolder = folderBrowserDialog.SelectedPath;
				txtOutputFolder.Text = _data.ActiveProfile.OutputFolder;
				_WizardStageChanged();
			}

		}

		private void btnAudioOutputFolder_Click(object sender, EventArgs e)
		{
			var folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
			folderBrowserDialog.SelectedPath = _data.ActiveProfile.AudioOutputFolder;
			folderBrowserDialog.Description = @"Select the audio export location";

			DialogResult dr = folderBrowserDialog.ShowDialog();
			if (dr == DialogResult.OK)
			{
				_data.ActiveProfile.AudioOutputFolder = folderBrowserDialog.SelectedPath;
				txtAudioOutputFolder.Text = _data.ActiveProfile.AudioOutputFolder;
				_WizardStageChanged();
			}
		}

		private void btnFalconOutputFolder_Click(object sender, EventArgs e)
		{
			var folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
			folderBrowserDialog.SelectedPath = _data.ActiveProfile.FalconOutputFolder;
			folderBrowserDialog.Description = @"Select the universe file export location";

			DialogResult dr = folderBrowserDialog.ShowDialog();
			if (dr == DialogResult.OK)
			{
				UpdateFalconPaths(folderBrowserDialog.SelectedPath);
				ValidateFalconOutputFolder();
			}
		}

		private void UpdateFalconPaths(string path)
		{
			_data.ActiveProfile.FalconOutputFolder = path;
			txtFalconOutputFolder.Text = path;
			if (!string.IsNullOrEmpty(path))
			{
				_data.ActiveProfile.OutputFolder = Path.Combine(path, _data.ActiveProfile.IsFalconEffectFormat ? @"effects" : @"sequences");
				_data.ActiveProfile.AudioOutputFolder = Path.Combine(path, @"music");
			}
			
			_WizardStageChanged();
		}

		private bool ValidateFalconOutputFolder()
		{
			var path = _data.ActiveProfile.FalconOutputFolder;
			if (string.IsNullOrEmpty(path)) return false;
			if (CanTestPath() && Directory.Exists(path))
			{
				_data.ActiveProfile.FalconOutputFolder = path;
				var sequencePath = _data.ActiveProfile.OutputFolder;
				var musicPath = _data.ActiveProfile.AudioOutputFolder;
				if (!Directory.Exists(sequencePath) || !Directory.Exists(musicPath))
				{
					var messageBox =
						new MessageBoxForm(
							"Some of the Falcon path structure does not exist at the location specified. Do you want to create it?",
							"Create Falcon structure?", MessageBoxButtons.OKCancel, SystemIcons.Question);
					var result = messageBox.ShowDialog(this);
					if (result == DialogResult.OK)
					{
						try
						{
							Directory.CreateDirectory(sequencePath);
							Directory.CreateDirectory(musicPath);
							_WizardStageChanged();
							return true;
						}
						catch (Exception e)
						{
							messageBox = new MessageBoxForm($"Unable to create the directory structure.\n{e.Message}",
								"Error creating Falcon structure.", MessageBoxButtons.OK, SystemIcons.Error);
							messageBox.ShowDialog(this);
							Logging.Error(e, "An error occured trying to create the Falcon directory structure.");
						}
					}
				}
				else
				{
					return true;
				}
			}
			else
			{
				var messageBox =
					new MessageBoxForm(
						"The Falcon output path does not seem correct. Please verify.",
						"Invalid output path", MessageBoxButtons.OK, SystemIcons.Error);
				messageBox.ShowDialog(this);
			}

			return false;
		}

		public override bool CanMoveNext
		{
			get
			{
				if (_data.ActiveProfile.IsFalconFormat)
				{
					if (CanTestPath() && Directory.Exists(_data.ActiveProfile.FalconOutputFolder)
					    && Directory.Exists(_data.ActiveProfile.OutputFolder))
					{
						if (_data.ActiveProfile.IncludeAudio)
						{
							return Directory.Exists(_data.ActiveProfile.AudioOutputFolder);
						}

						return true;
					}

					return false;
				}

				bool ok = Directory.Exists(_data.ActiveProfile.OutputFolder);
				if (_data.ActiveProfile.IncludeAudio && !Directory.Exists((_data.ActiveProfile.AudioOutputFolder)))
				{
					ok = false;
				}

				return ok;
			}
		}

		private bool CanTestPath()
		{
			bool success = true;
			if (!string.IsNullOrEmpty(_data.ActiveProfile.FalconOutputFolder))
			{
				try
				{
					Uri uri = new Uri(_data.ActiveProfile.FalconOutputFolder);
					if (uri.HostNameType != UriHostNameType.Unknown && !string.IsNullOrEmpty(uri.Host))
					{
						success = PingHost(uri.Host);
					}
				}
				catch (Exception e)
				{
					success = false;
				}
				
			}
			
			return success;
		}

		public static bool PingHost(string nameOrAddress)
		{
			bool pingable = false;
			Ping pinger = null;

			try
			{
				pinger = new Ping();
				PingReply reply = pinger.Send(nameOrAddress,500);
				if (reply != null) pingable = reply.Status == IPStatus.Success;
			}
			catch (PingException)
			{
				// Discard PingExceptions and return false;
			}
			finally
			{
				pinger?.Dispose();
			}

			return pingable;
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}

		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}

		private void outputFormatComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			_data.ActiveProfile.Format = comboBox.SelectedItem.ToString();
			grpFalcon.Visible = _data.ActiveProfile.IsFalconFormat;
			grpAudio.Visible = grpSequence.Visible = !grpFalcon.Visible;
			if (_data.ActiveProfile.IsFalconFormat)
			{
				UpdateFalconPaths(_data.ActiveProfile.FalconOutputFolder);
				ValidateFalconOutputFolder();
			}
			_WizardStageChanged();
		}

		private void resolutionComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			_data.ActiveProfile.Interval = Convert.ToInt32(comboBox.SelectedItem);
		}

		private void chkIncludeAudio_CheckedChanged(object sender, EventArgs e)
		{
			if (_data.ActiveProfile.IsFalconFormat)
			{
				_data.ActiveProfile.RenameAudio = chkFppIncludeAudio.Checked;
				_data.ActiveProfile.IncludeAudio = chkFppIncludeAudio.Checked;
			}
			else
			{
				_data.ActiveProfile.IncludeAudio = chkIncludeAudio.Checked;
				chkRenameAudio.Enabled = btnAudioOutputFolder.Enabled = lblAudioExportPath.Enabled = txtAudioOutputFolder.Enabled = chkIncludeAudio.Checked;
			}
		}

		private void chkRenameAudio_CheckedChanged(object sender, EventArgs e)
		{
			_data.ActiveProfile.RenameAudio = chkRenameAudio.Checked;
		}

		private void chkCreateUniverseFile_CheckedChanged(object sender, EventArgs e)
		{
			_data.ActiveProfile.CreateUniverseFile = chkCreateUniverseFile.Checked;
		}

		private void chkBackupUniverseFile_CheckedChanged(object sender, EventArgs e)
		{
			_data.ActiveProfile.BackupUniverseFile = chkBackupUniverseFile.Checked;
		}

		private void txtFalconOutputFolder_Leave(object sender, EventArgs e)
		{
			UpdateFalconPaths(txtFalconOutputFolder.Text);
			ValidateFalconOutputFolder();
		}

		private void txtFalconOutputFolder_TextChanged(object sender, EventArgs e)
		{
			//UpdateFalconPaths(txtFalconOutputFolder.Text);
		}
	}
}
