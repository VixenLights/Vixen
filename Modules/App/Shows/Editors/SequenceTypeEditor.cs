using System;
using System.Windows.Forms;
using System.IO;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;
using Vixen.Services;

namespace VixenModules.App.Shows
{
	public partial class SequenceTypeEditor : TypeEditorBase
	{
		private readonly ShowItem _showItem;
		
		public SequenceTypeEditor(ShowItem showItem)
		{
			InitializeComponent();

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			
			buttonSelectSequence.Image = Tools.GetIcon(Resources.folder_explore, iconSize);
			buttonSelectSequence.Text = "";
			ThemeUpdateControls.UpdateControls(this);
			_showItem = showItem;
		}

		private void OpenFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string[] files = openFileDialog.FileNames;

			foreach (string file in files)
			{
				string folder = Path.GetDirectoryName(file);
				if (folder != null && !folder.StartsWith(SequenceService.SequenceDirectory))
				{
					e.Cancel = true;
					break;
				}
			}
		}
		
		private void buttonSelectSequence_Click(object sender, EventArgs e)
		{
			openFileDialog.InitialDirectory = SequenceService.SequenceDirectory;

			openFileDialog.Filter = @"Timed Sequence (*.tim)|*.tim"; 

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				textBoxSequence.Text = openFileDialog.FileName;
				labelSequence.Text = Path.GetFileName(_showItem.SequencePath);
				_showItem.SequencePath = openFileDialog.FileName;
				_showItem.Name = "Run sequence: " + Path.GetFileName(_showItem.SequencePath);
				FireChanged(_showItem.Name);
			}
		}

		private void SequenceTypeEditor_Load(object sender, EventArgs e)
		{
			textBoxSequence.Text = _showItem.SequencePath;
			labelSequence.Text = Path.GetFileName(_showItem.SequencePath);
		}

	}
}
