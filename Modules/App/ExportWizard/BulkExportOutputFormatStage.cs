using System;
using System.IO;
using System.Windows.Forms;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Controls.Wizard;
using Common.Resources;
using Common.Resources.Properties;

namespace VixenModules.App.ExportWizard
{
	public partial class BulkExportOutputFormatStage : WizardStage
	{
		private readonly ExportProfile _profile;
		public BulkExportOutputFormatStage(BulkExportWizardData data)
		{
			_profile = data.ActiveProfile;
			InitializeComponent();
			
			outputFormatComboBox.Items.Clear();
			outputFormatComboBox.Items.AddRange(data.Export.FormatTypes);
			outputFormatComboBox.Sorted = true;

			outputFormatComboBox.SelectedItem = _profile.Format;

			resolutionComboBox.SelectedItem = _profile.Interval.ToString();

			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());

			btnOuputFolderSelect.Image = Tools.GetIcon(Resources.folder_explore, iconSize);
			btnOuputFolderSelect.Text = "";

			btnAudioOutputFolder.Image = Tools.GetIcon(Resources.folder_explore, iconSize);
			btnAudioOutputFolder.Text = "";

			txtOutputFolder.Text = _profile.OutputFolder;

			chkIncludeAudio.Checked = _profile.IncludeAudio;
			chkRenameAudio.Enabled = btnAudioOutputFolder.Enabled = lblAudioExportPath.Enabled = txtAudioOutputFolder.Enabled = _profile.IncludeAudio;
			chkRenameAudio.Checked = _profile.RenameAudio;

			txtAudioOutputFolder.Text = _profile.AudioOutputFolder;

			ThemeUpdateControls.UpdateControls(this);

		}

		private void btnOuputFolderSelect_Click(object sender, EventArgs e)
		{
			var folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
			folderBrowserDialog.SelectedPath = _profile.OutputFolder;
			folderBrowserDialog.Description = @"Select the sequence export location";

			DialogResult dr = folderBrowserDialog.ShowDialog();
			if (dr == DialogResult.OK)
			{
				_profile.OutputFolder = folderBrowserDialog.SelectedPath;
				txtOutputFolder.Text = _profile.OutputFolder;
			}

		}

		private void btnAudioOutputFolder_Click(object sender, EventArgs e)
		{
			var folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
			folderBrowserDialog.SelectedPath = _profile.AudioOutputFolder;
			folderBrowserDialog.Description = @"Select the audio export location";

			DialogResult dr = folderBrowserDialog.ShowDialog();
			if (dr == DialogResult.OK)
			{
				_profile.AudioOutputFolder = folderBrowserDialog.SelectedPath;
				txtAudioOutputFolder.Text = _profile.AudioOutputFolder;
			}
		}

		public override bool CanMoveNext
		{
			get { return Directory.Exists(_profile.OutputFolder); }
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
			_profile.Format = comboBox.SelectedItem.ToString();
		}

		private void resolutionComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			_profile.Interval = Convert.ToInt32(comboBox.SelectedItem);
		}

		private void chkIncludeAudio_CheckedChanged(object sender, EventArgs e)
		{
			chkRenameAudio.Enabled = btnAudioOutputFolder.Enabled = lblAudioExportPath.Enabled = txtAudioOutputFolder.Enabled = chkIncludeAudio.Checked;
			_profile.IncludeAudio = chkIncludeAudio.Checked;
		}

		private void chkRenameAudio_CheckedChanged(object sender, EventArgs e)
		{
			_profile.RenameAudio = chkRenameAudio.Checked;
		}

		
	}
}
