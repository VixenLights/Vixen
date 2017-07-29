using System.IO;
using System.Windows.Forms;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Controls.Wizard;
using Common.Resources;
using Common.Resources.Properties;
using Vixen.Services;

namespace VixenModules.App.ExportWizard
{
	public partial class BulkExportSources : WizardStage
	{
		private BulkExportWizardData _data;
		public BulkExportSources(BulkExportWizardData data)
		{
			_data = data;
			InitializeComponent();

			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());

			btnSelectSequences.Image = Tools.GetIcon(Resources.folder_explore, iconSize);
			btnSelectSequences.Text = "";
			
			ThemeUpdateControls.UpdateControls(this);
		}

		private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string[] files = selectSequencesDialog.FileNames;

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

		private void btnSelectSequences_Click(object sender, System.EventArgs e)
		{
			selectSequencesDialog.InitialDirectory = SequenceService.SequenceDirectory;

			selectSequencesDialog.Filter = @"Timed Sequence (*.tim)|*.tim";

			if (selectSequencesDialog.ShowDialog() == DialogResult.OK)
			{
				lstSequences.BeginUpdate();
				foreach (var fileName in selectSequencesDialog.FileNames)
				{
					ListViewItem item = new ListViewItem(Path.GetFileName(fileName));
					item.Tag = fileName;
					lstSequences.Items.Add(item);
					_data.SequenceFiles.Add(fileName);
				}
				lstSequences.EndUpdate();
				ColumnAutoSize();

				_WizardStageChanged();
			}
		}

		public void ColumnAutoSize()
		{
			lstSequences.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			ListView.ColumnHeaderCollection cc = lstSequences.Columns;
			for (int i = 0; i < cc.Count; i++)
			{
				cc[i].Width = lstSequences.Width - (int)(lstSequences.Width * .18d);
			}
		}

		public override bool CanMoveNext
		{
			get { return _data.SequenceFiles.Count > 0; }
		}
	}
}
