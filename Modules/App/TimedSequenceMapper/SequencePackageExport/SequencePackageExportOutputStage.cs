using System;
using System.IO;
using System.Windows.Forms;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Controls.Wizard;
using Common.Resources;
using Common.Resources.Properties;
using NLog;
using Vixen.Sys;

namespace VixenModules.App.TimedSequenceMapper.SequencePackageExport
{
	public partial class SequencePackageExportOutputStage : WizardStage
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private readonly TimedSequencePackagerData _data;
		private const string PackageExtension = "vpkg";
		public SequencePackageExportOutputStage(TimedSequencePackagerData data)
		{
			_data = data;
			InitializeComponent();
			
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());

			btnOuputFolderSelect.Image = Tools.GetIcon(Resources.folder_explore, iconSize);
			btnOuputFolderSelect.Text = "";

			ThemeUpdateControls.UpdateControls(this);

		}

		
		public override void StageStart()
		{
			var path = _data.ExportOutputFile;
			if (!Path.HasExtension(_data.ExportOutputFile))
			{
				if (Directory.Exists(_data.ExportOutputFile))
				{
					path = Path.Combine(_data.ExportOutputFile, $"{VixenSystem.ProfileName}.{PackageExtension}");
				}
				else
				{
					path = Path.Combine(Path.Combine(Paths.DataRootPath, "Export"), $"{VixenSystem.ProfileName}.{PackageExtension}");
				}
			}

			_data.ExportOutputFile = path;
			txtOutputFolder.Text = path;
			chkIncludeAudio.Checked = _data.ExportIncludeAudio;
			
			_WizardStageChanged();
		}

		private void btnOutputFolderSelect_Click(object sender, EventArgs e)
		{
			var saveFileDialog = new SaveFileDialog();
			saveFileDialog.CheckPathExists = true;
			//saveFileDialog.CreatePrompt = true;
			saveFileDialog.OverwritePrompt = true;
			var dir = Path.GetDirectoryName(_data.ExportOutputFile);
			if (!Directory.Exists(dir))
			{
				dir = Path.Combine(Paths.DataRootPath, "Export");
			}
			saveFileDialog.InitialDirectory = dir;
			saveFileDialog.FileName = $"{VixenSystem.ProfileName}.{PackageExtension}";

			var filter = $"Vixen 3 Sequence Package (*.{PackageExtension})|*.{PackageExtension}";
			saveFileDialog.Filter = filter;

			DialogResult dr = saveFileDialog.ShowDialog();
			if (dr == DialogResult.OK)
			{
				_data.ExportOutputFile = saveFileDialog.FileName;
				txtOutputFolder.Text = _data.ExportOutputFile;
				_WizardStageChanged();
			}

		}

		private void txtOutputFolder_Leave(object sender, EventArgs e)
		{
			_data.ExportOutputFile = txtOutputFolder.Text;
			_WizardStageChanged();
		}
		
		public override bool CanMoveNext => Directory.Exists(Path.GetDirectoryName(_data.ExportOutputFile)) && Path.HasExtension(_data.ExportOutputFile);

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}

		private void chkIncludeAudio_CheckedChanged(object sender, EventArgs e)
		{
			_data.ExportIncludeAudio = chkIncludeAudio.Checked;
		}
	}
}
