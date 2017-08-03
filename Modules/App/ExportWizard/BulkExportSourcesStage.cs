using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Controls.Wizard;
using Common.Resources;
using Common.Resources.Properties;
using Vixen.Services;

namespace VixenModules.App.ExportWizard
{
	public partial class BulkExportSourcesStage : WizardStage
	{
		private readonly BulkExportWizardData _data;
		
		public BulkExportSourcesStage(BulkExportWizardData data)
		{
			_data = data;
			InitializeComponent();

			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			toolStripSequences.Renderer = new ThemeToolStripRenderer();
			toolStripSequences.ImageScalingSize = new Size(iconSize, iconSize);

			btnAdd.DisplayStyle = ToolStripItemDisplayStyle.Image;
			btnAdd.Image = Tools.GetIcon(Resources.folder_open, iconSize);
			btnDelete.DisplayStyle = ToolStripItemDisplayStyle.Image;
			btnDelete.Image = Tools.GetIcon(Resources.delete_32, iconSize);

			//btnSelectSequences.Image = Tools.GetIcon(Resources.folder_explore, iconSize);
			//btnSelectSequences.Text = string.Empty;

			//btnDelete.Image = Tools.GetIcon(Resources.delete, iconSize);
			//btnDelete.Text = string.Empty;

			ThemeUpdateControls.UpdateControls(this);
			InitializeSequenceList();
		}

		private void InitializeSequenceList()
		{
			if (_data.SequenceFiles.Any())
			{
				
				lstSequences.BeginUpdate();
				foreach (var fileName in _data.SequenceFiles)
				{
					if (File.Exists(fileName))
					{
						ListViewItem item = new ListViewItem(Path.GetFileName(fileName));
						item.Tag = fileName;
						lstSequences.Items.Add(item);
					}
					else
					{
						_data.SequenceFiles.Remove(fileName);
					}
				}
				lstSequences.EndUpdate();
				ColumnAutoSize();

				_WizardStageChanged();
				
			}
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

		private void lstSequences_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && lstSequences.SelectedItems.Count > 0)
			{
				foreach (ListViewItem item in lstSequences.SelectedItems)
				{
					_data.SequenceFiles.Remove(item.Tag as string);
					item.Remove();
				}
			}
		}

		private void lstSequences_SelectedIndexChanged(object sender, EventArgs e)
		{
			btnDelete.Enabled = lstSequences.SelectedItems.Count > 0;
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in lstSequences.SelectedItems)
			{
				_data.SequenceFiles.Remove(item.Tag as string);
				item.Remove();
			}
		}

		
	}
}
