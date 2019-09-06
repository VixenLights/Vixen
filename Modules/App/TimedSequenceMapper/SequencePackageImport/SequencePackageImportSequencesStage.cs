using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Controls.Wizard;

namespace VixenModules.App.TimedSequenceMapper.SequencePackageImport
{
	public partial class SequencePackageImportSequencesStage : WizardStage
	{
		private readonly ImportConfig _data;
		public SequencePackageImportSequencesStage(ImportConfig data)
		{
			_data = data;
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			lstSequences.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);
		}

		public override void StageStart()
		{
			ColumnAutoSize();
			InitializeSequenceList();
			ExtractSequences(_data.InputFile);
			lstSequences.ItemChecked += lstSequences_ItemChecked; 
		}

		#region Overrides of WizardStage

		/// <inheritdoc />
		public override Task StageEnd()
		{
			lstSequences.ItemChecked -= lstSequences_ItemChecked;
			return Task.CompletedTask;
		}

		#endregion

		private void lstSequences_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			if (e.Item.Tag is string v)
			{
				if (_data.Sequences.ContainsKey(v))
				{
					_data.Sequences[v] = e.Item.Checked;
					_WizardStageChanged();
				}
			}
		}

		private void InitializeSequenceList()
		{
			lstSequences.Items.Clear();
			if (_data.Sequences.Any())
			{
				lstSequences.BeginUpdate();
				foreach (var fileName in _data.Sequences)
				{
					ListViewItem item = new ListViewItem(fileName.Key);
					item.Tag = fileName;
					item.Checked = fileName.Value;
					lstSequences.Items.Add(item);
				}
				lstSequences.EndUpdate();
				ColumnAutoSize();
				_WizardStageChanged();
			}
		}

		private void ExtractSequences(string fileName)
		{
			using (ZipArchive archive = ZipFile.Open(fileName, ZipArchiveMode.Read))
			{
				var sequences = archive.Entries.Where(x => x.FullName.StartsWith("Sequence") && x.Name.EndsWith(".tim"));
				List<string> sequenceNames = new List<string>();
				foreach (var zipArchiveEntry in sequences)
				{
					var sequence = Path.GetFileNameWithoutExtension(zipArchiveEntry.FullName);
					sequenceNames.Add(sequence);
				}
				AddFiles(sequenceNames);
			}
		}

		private void AddFiles(IEnumerable<string> fileNames)
		{
			lstSequences.BeginUpdate();
			foreach (var fileName in fileNames)
			{
				if (!_data.Sequences.ContainsKey(fileName))
				{
					ListViewItem item = new ListViewItem(Path.GetFileName(fileName));
					item.Tag = fileName;
					item.Checked = true;
					lstSequences.Items.Add(item);
					_data.Sequences.Add(fileName, true);
				}
			}

			lstSequences.EndUpdate();
			ColumnAutoSize();

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
			get { return _data.Sequences.Any(x => x.Value); }
		}

		private void lstSequences_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && lstSequences.SelectedItems.Count > 0)
			{
				foreach (ListViewItem item in lstSequences.SelectedItems)
				{
					if(item.Tag is string v)
					{
						if (_data.Sequences.ContainsKey(v))
						{
							_data.Sequences[v] = item.Checked = false;
						}
					}
				}
				_WizardStageChanged();
			}
			else
			{
				if (e.KeyCode == Keys.Space && lstSequences.SelectedItems.Count > 0)
				{
					foreach (ListViewItem item in lstSequences.SelectedItems)
					{
						if (item.Tag is string v)
						{
							if (_data.Sequences.ContainsKey(v))
							{
								_data.Sequences[v] = item.Checked;
							}
						}
					}
					_WizardStageChanged();
				}
			}
		}
	}
}
