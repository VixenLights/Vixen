using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Controls.Wizard;
using Common.Resources;
using Common.Resources.Properties;
using NLog;
using VixenModules.App.TimedSequenceMapper.SequenceElementMapper.ViewModels;
using VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Views;

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

			btnCreateMap.Enabled = false;
			btnEditMap.Enabled = false;
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
			if (!string.IsNullOrEmpty(_data.InputFile))
			{
				var dir = Path.GetDirectoryName(_data.InputFile);
				if (!Directory.Exists(dir))
				{
					openFileDialog.InitialDirectory = dir;
				}
			}
			
			var filter = $"Vixen 3 Sequence Package (*.{Constants.PackageExtension})|*.{Constants.PackageExtension}";
			openFileDialog.Filter = filter;

			DialogResult dr = openFileDialog.ShowDialog();
			if (dr == DialogResult.OK)
			{
				_data.InputFile = openFileDialog.FileName;
				txtPackageFile.Text = _data.InputFile;
				btnCreateMap.Enabled = IsPackageFile(_data.InputFile);
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
				btnEditMap.Enabled = true;
				_WizardStageChanged();
			}
		}

		private void txtPackageFile_Leave(object sender, EventArgs e)
		{
			_data.InputFile = txtPackageFile.Text;
			btnCreateMap.Enabled = IsPackageFile(_data.InputFile);
			_WizardStageChanged();
		}

		private void txtMapFile_Leave(object sender, EventArgs e)
		{
			
			if (File.Exists(txtMapFile.Text))
			{
				_data.MapFile = txtMapFile.Text;
				btnEditMap.Enabled = true;
			}
			else
			{
				btnEditMap.Enabled = false;
			}
			_WizardStageChanged();
		}

		public override bool CanMoveNext
		{
			get
			{
				bool canMove = Path.HasExtension(_data.InputFile) && File.Exists(_data.InputFile) && Path.HasExtension(_data.MapFile) && File.Exists(_data.MapFile) &&
				               Path.GetExtension(_data.MapFile).EndsWith(Constants.MapExtension) && IsPackageFile(_data.InputFile);
				
				return canMove;
			}
		}

		private bool IsPackageFile(string fileName)
		{
			if (Path.HasExtension(fileName) && File.Exists(fileName))
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
			}
			
			return false;
		}

		private string ExtractElementTree(string packageFile)
		{
			string elementTreeFile = string.Empty;
			if(File.Exists(packageFile))
			{
				using (var package = ZipFile.OpenRead(packageFile))
				{
					var etEntry =
						package.GetEntry(Path.Combine("ProfileInfo", $"ElementTree.{Constants.ElementTreeExtension}"));
					if (etEntry != null)
					{
						elementTreeFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
						etEntry.ExtractToFile(elementTreeFile);
					}
				}
			}

			return elementTreeFile;
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
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

		private async void btnCreateMap_Click(object sender, EventArgs e)
		{
			var fileName = ExtractElementTree(_data.InputFile);
			ElementMapperViewModel vm = new ElementMapperViewModel(new Dictionary<Guid, string>(), String.Empty, fileName, String.Empty);
			ElementMapperView mapper = new ElementMapperView(vm);
			ElementHost.EnableModelessKeyboardInterop(mapper);
			var response = mapper.ShowDialog();

			if (response.HasValue && response.Value)
			{
				if (!string.IsNullOrEmpty(vm.ElementMapFilePath))
				{
					_data.MapFile = txtMapFile.Text = vm.ElementMapFilePath;
					_WizardStageChanged();
				}
			}
			
		}

		private async void btnEditMap_Click(object sender, EventArgs e)
		{
			var fileName = ExtractElementTree(_data.InputFile);
			ElementMapperViewModel vm = new ElementMapperViewModel(new Dictionary<Guid, string>(), String.Empty, fileName, _data.MapFile);
			ElementMapperView mapper = new ElementMapperView(vm);
			ElementHost.EnableModelessKeyboardInterop(mapper);
			var response = mapper.ShowDialog();

			if (response.HasValue && response.Value)
			{
				if (!string.IsNullOrEmpty(vm.ElementMapFilePath))
				{
					_data.MapFile = txtMapFile.Text = vm.ElementMapFilePath;
					_WizardStageChanged();
				}
			}
		}
	}
}
