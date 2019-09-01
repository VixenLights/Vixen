using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Controls.Wizard;
using Common.Resources;
using Common.Resources.Properties;
using NLog;

namespace VixenModules.App.TimedSequenceMapper.SequencePackageImport
{
	public partial class SequencePackageImportInputStage : WizardStage
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private readonly ImportConfig _data;

		public SequencePackageImportInputStage(ImportConfig data)
		{
			_data = data;
			InitializeComponent();
			
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());

			btnOuputFolderSelect.Image = btnMapFile.Image = Tools.GetIcon(Resources.folder_explore, iconSize);
			btnOuputFolderSelect.Text = btnMapFile.Text = "";

			ThemeUpdateControls.UpdateControls(this);
			txtProfileMap.BackColor = ThemeColorTable.BackgroundColor;
		}

		
		public override void StageStart()
		{
			if (!string.IsNullOrEmpty(_data.InputFile))
			{
				txtPackageFile.Text = _data.InputFile;
			}
			
			_WizardStageChanged();
		}

		private void btnOutputFolderSelect_Click(object sender, EventArgs e)
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.CheckPathExists = true;
			var dir = Path.GetDirectoryName(_data.InputFile);
			if (!Directory.Exists(dir))
			{
				openFileDialog.InitialDirectory = dir;
			}
			
			var filter = $"Vixen 3 Sequence Package (*.{Constants.PackageExtension})|*.{Constants.PackageExtension}";
			openFileDialog.Filter = filter;

			DialogResult dr = openFileDialog.ShowDialog();
			if (dr == DialogResult.OK)
			{
				_data.InputFile = openFileDialog.FileName;
				txtPackageFile.Text = _data.InputFile;
				_WizardStageChanged();
			}
		}

		private void btnMapFile_Click(object sender, EventArgs e)
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.CheckPathExists = true;
			var filter = $"Vixen 3 Map File (*.{Constants.MapExtension})|*.{Constants.MapExtension}";
			openFileDialog.Filter = filter;

			DialogResult dr = openFileDialog.ShowDialog();
			if (dr == DialogResult.OK)
			{
				_data.MapFile = openFileDialog.FileName;
				txtMapFile.Text = _data.MapFile;
				_WizardStageChanged();
			}
		}

		private void txtPackageFile_Leave(object sender, EventArgs e)
		{
			_data.InputFile = txtPackageFile.Text;
			_WizardStageChanged();
		}

		private void txtMapFile_Leave(object sender, EventArgs e)
		{
			_data.MapFile = txtMapFile.Text;
			_WizardStageChanged();
		}

		public override bool CanMoveNext
		{
			get
			{
				bool canMove = false;
				canMove = Path.HasExtension(_data.InputFile) && File.Exists(_data.InputFile) && IsPackageFile(_data.InputFile);
				if (!string.IsNullOrEmpty(_data.MapFile))
				{
					canMove = Path.HasExtension(_data.MapFile) && File.Exists(_data.MapFile) && Path.GetExtension(_data.MapFile).EndsWith(Constants.MapExtension);
				}

				return canMove;
			}
		}

		private bool IsPackageFile(string fileName)
		{
			try
			{
				using (var archive = ZipFile.OpenRead(fileName))
				{
					return archive.Entries.Any(x => x.FullName.StartsWith("Sequence") && x.FullName.EndsWith(".tim"));
				}
			}
			catch (Exception e)
			{
				Logging.Error(e, $"An error occured trying to validate a Sequence Package file. {fileName}");
			}

			return false;
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}

		
	}
}
