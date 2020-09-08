using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Controls.Wizard;
using Common.Resources;
using Common.Resources.Properties;
using Vixen.Services;

namespace VixenModules.App.TimedSequenceMapper.SequencePackageExport
{
	public partial class SequencePackageExportSourcesStage : WizardStage
	{
		private readonly TimedSequencePackagerData _data;
		public SequencePackageExportSourcesStage(TimedSequencePackagerData data)
		{
			_data = data;
			InitializeComponent();

			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			btnAdd.Image = Tools.GetIcon(Resources.folder_open, iconSize);
			btnAdd.Text = string.Empty;

			btnAddAll.Image = Tools.GetIcon(Resources.document_copies, iconSize);
			btnAddAll.Text = String.Empty;
			
			btnDelete.Image = Tools.GetIcon(Resources.delete_32, iconSize);
			btnDelete.Text = string.Empty;

			ThemeUpdateControls.UpdateControls(this);
		}

		public override void StageStart()
		{
			InitializeSequenceList();
		}

		private void InitializeSequenceList()
		{
			lstSequences.Items.Clear();
			if (_data.ExportSequenceFiles.Any())
			{
				lstSequences.BeginUpdate();
				foreach (var fileName in _data.ExportSequenceFiles.ToList())
				{
					if (File.Exists(fileName))
					{
						ListViewItem item = new ListViewItem(Path.GetFileName(fileName));
						item.Tag = fileName;
						lstSequences.Items.Add(item);
					}
					else
					{
						_data.ExportSequenceFiles.Remove(fileName);
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
				if (folder == null)
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
				AddFiles(selectSequencesDialog.FileNames);
			}
		}

		private void AddFiles(IEnumerable<string> fileNames)
		{
			bool warn = false;
			lstSequences.BeginUpdate();
			foreach (var fileName in fileNames)
			{
				string folder = Path.GetDirectoryName(fileName);
				if(folder != null && !folder.StartsWith(SequenceService.SequenceDirectory))
				{
					warn = true;
				}
				if (!_data.ExportSequenceFiles.Contains(fileName))
				{
					ListViewItem item = new ListViewItem(Path.GetFileName(fileName));
					item.Tag = fileName;
					lstSequences.Items.Add(item);
					_data.ExportSequenceFiles.Add(fileName);
				}
			}

			lstSequences.EndUpdate();
			ColumnAutoSize();

			if (warn)
			{
				NotifyPossibleForeignSequences();
			}

			_WizardStageChanged();
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
			get { return _data.ExportSequenceFiles.Count > 0; }
		}

		private void lstSequences_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && lstSequences.SelectedItems.Count > 0)
			{
				foreach (ListViewItem item in lstSequences.SelectedItems)
				{
					_data.ExportSequenceFiles.Remove(item.Tag as string);
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
				_data.ExportSequenceFiles.Remove(item.Tag as string);
				item.Remove();
			}
		}

		private void btnAddAll_Click(object sender, EventArgs e)
		{
			var files = Directory.GetFiles(SequenceService.SequenceDirectory, "*.tim", SearchOption.AllDirectories);

			AddFiles(files.Where(x => x.EndsWith(".tim")));
		}

		private void NotifyPossibleForeignSequences()
		{
			var messageBox = new MessageBoxForm("Some selected sequences are not located in the current profile sequence folder. " +
			                                    "Please ensure all sequences are compatible and play with the current setup before exporting for best results.", "Warning", MessageBoxButtons.OK, SystemIcons.Warning);
			messageBox.ShowDialog(this);
		}
	}
}
