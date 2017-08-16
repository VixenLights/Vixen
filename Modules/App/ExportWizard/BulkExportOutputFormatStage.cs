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
			
			ThemeUpdateControls.UpdateControls(this);

		}

		public override void StageStart()
		{
			outputFormatComboBox.Items.Clear();
			outputFormatComboBox.Items.AddRange(_data.Export.FormatTypes);
			outputFormatComboBox.Sorted = true;
			outputFormatComboBox.SelectedItem = _data.ActiveProfile.Format;

			resolutionComboBox.SelectedItem = _data.ActiveProfile.Interval.ToString();
			txtOutputFolder.Text = _data.ActiveProfile.OutputFolder;

			chkIncludeAudio.Checked = _data.ActiveProfile.IncludeAudio;
			chkRenameAudio.Enabled = btnAudioOutputFolder.Enabled = lblAudioExportPath.Enabled = txtAudioOutputFolder.Enabled = _data.ActiveProfile.IncludeAudio;
			chkRenameAudio.Checked = _data.ActiveProfile.RenameAudio;

			txtAudioOutputFolder.Text = _data.ActiveProfile.AudioOutputFolder;
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
			}
		}

		public override bool CanMoveNext
		{
			get { return Directory.Exists(_data.ActiveProfile.OutputFolder); }
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
		}

		private void resolutionComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			_data.ActiveProfile.Interval = Convert.ToInt32(comboBox.SelectedItem);
		}

		private void chkIncludeAudio_CheckedChanged(object sender, EventArgs e)
		{
			chkRenameAudio.Enabled = btnAudioOutputFolder.Enabled = lblAudioExportPath.Enabled = txtAudioOutputFolder.Enabled = chkIncludeAudio.Checked;
			_data.ActiveProfile.IncludeAudio = chkIncludeAudio.Checked;
		}

		private void chkRenameAudio_CheckedChanged(object sender, EventArgs e)
		{
			_data.ActiveProfile.RenameAudio = chkRenameAudio.Checked;
		}

		
	}
}
