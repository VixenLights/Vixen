using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;

namespace Common.Controls.NameGeneration
{
	public partial class SubstitutionRenamer : BaseForm
	{
		private List<string> OldNames { get; set; }

		private List<ReplacePattern> _patterns = new List<ReplacePattern>();

		private bool _suspendNameGeneration = false;

		protected SubstitutionRenamer()
		{
			InitializeComponent();
			Icon = Resources.Properties.Resources.Icon_Vixen3;
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			buttonMovePatternUp.Image = Tools.GetIcon(Resources.Properties.Resources.arrow_up, iconSize);
			buttonMovePatternUp.Text = "";
			buttonMovePatternDown.Image = Tools.GetIcon(Resources.Properties.Resources.arrow_down, iconSize);
			buttonMovePatternDown.Text = "";
			buttonAddNewPattern.Image = Tools.GetIcon(Resources.Properties.Resources.add, iconSize);
			buttonAddNewPattern.Text = "";
			buttonDeletePattern.Image = Tools.GetIcon(Resources.Properties.Resources.delete, iconSize);
			buttonDeletePattern.Text = "";
			buttonOk.Enabled = false;
			ThemeUpdateControls.UpdateControls(this);
		}

		public SubstitutionRenamer(IEnumerable<string> oldNames): this()
		{
			OldNames = new List<string>(oldNames);
			listViewNames.Columns.Clear();
			listViewNames.Columns.Add(new ColumnHeader { Text = @"Old Name" });
			listViewNames.Columns.Add(new ColumnHeader { Text = @"New Name" });
		}

		private void SubstitutionRenamer_Load(object sender, EventArgs e)
		{
			ResizeListviewColumns();
			_patterns.Add(new ReplacePattern());
			SyncPatternsToListView();
			listViewPatterns.Items[0].Selected = true;
			PopulateNames();
		}

		public List<string> Names { get; set; }

		private void PopulateNames()
		{
			if (_suspendNameGeneration) return;

			Names = new List<string>(GenerateNames());

			listViewNames.BeginUpdate();
			listViewNames.Items.Clear();

			if (OldNames != null)
			{
				for (int i = 0; i < OldNames.Count; i++)
				{
					ListViewItem item = new ListViewItem();
					item.Text = OldNames[i];
					item.SubItems.Add(Names.Count > i ? Names[i] : "-");
					listViewNames.Items.Add(item);
				}
			}
			else
			{
				foreach (var name in Names)
				{
					ListViewItem item = new ListViewItem();
					item.Text = name;
					listViewNames.Items.Add(item);
				}
			}

			ResizeListviewColumns();

			listViewNames.EndUpdate();
		}

		private void SyncPatternsToListView()
		{
			listViewPatterns.BeginUpdate();
			listViewPatterns.Items.Clear();

			int i = 1;
			foreach (ReplacePattern pattern in _patterns)
			{
				ListViewItem item = new ListViewItem();
				SetPatternItemName(item, i, pattern);
				item.Tag = pattern;
				listViewPatterns.Items.Add(item);
				i++;
			}

			listViewPatterns.EndUpdate();
		}

		private void SetPatternItemName(ListViewItem item, int index, ReplacePattern pattern)
		{
			item.Text = string.Format("#{0} {1}", index, pattern.ToString());
		}

		private IEnumerable<string> GenerateNames()
		{
			List<string> newNames = new List<string>();

			if (_patterns.Any(x => !string.IsNullOrEmpty(x.Find)))
			{
				foreach (var oldName in OldNames)
				{
					var tmpName = oldName;
					foreach (var replacePattern in _patterns)
					{
						if (!string.IsNullOrEmpty(replacePattern.Find))
						{
							tmpName = tmpName.Replace(replacePattern.Find, replacePattern.Replace);
						}
					}
					newNames.Add(tmpName);
				}
			}
			else
			{
				return OldNames.ToList();
			}
			
			return newNames;
		}

		private void PopulateFindReplaceFields()
		{
			if (listViewPatterns.SelectedIndices.Count <= 0)
			{
				return;
			}

			_suspendNameGeneration = true;
			var pattern = _patterns[listViewPatterns.SelectedIndices[0]];
			txtFind.Text = pattern.Find;
			txtReplace.Text = pattern.Replace;
			_suspendNameGeneration = false;
		}

		private void PopulatePatternFields()
		{
			if (listViewPatterns.SelectedIndices.Count <= 0)
			{
				return;
			}
			var index = listViewPatterns.SelectedIndices[0];

			var pattern = _patterns[index];
			pattern.Find = txtFind.Text;
			pattern.Replace = txtReplace.Text;
			SetPatternItemName(listViewPatterns.Items[index], index+1, pattern);
		}

		private void ResizeListviewColumns()
		{
			listViewNames.ColumnAutoSize();
			listViewNames.SetLastColumnWidth();
		}

		private void EnableFindReplaceInput(bool enable)
		{
			txtFind.Enabled = lblFind.Enabled = enable;
			txtReplace.Enabled = lblReplace.Enabled = enable;
		}

		private void txtFind_TextChanged(object sender, EventArgs e)
		{
			if (_suspendNameGeneration) return;
			buttonOk.Enabled = _patterns.Any(x => !string.IsNullOrEmpty(x.Find));
			PopulatePatternFields();
			PopulateNames();
		}

		private void txtReplace_TextChanged(object sender, EventArgs e)
		{
			if (_suspendNameGeneration) return;
			PopulatePatternFields();
			PopulateNames();
		}

		private void buttonAddNewRule_Click(object sender, EventArgs e)
		{
			_patterns.Add(new ReplacePattern());
			SyncPatternsToListView();
			listViewPatterns.Items[listViewPatterns.Items.Count - 1].Selected = true;
		}

		private void buttonDeleteRule_Click(object sender, EventArgs e)
		{
			
			if (listViewPatterns.SelectedIndices.Count <= 0 || _patterns.Count() <= 1)
				return;

			int index = listViewPatterns.SelectedIndices[0];
			_patterns.RemoveAt(index);

			SyncPatternsToListView();
			if (index - 1 >= 0)
			{
				listViewPatterns.Items[index-1].Selected = true;
			}
			
			PopulateNames();
		}

		private void listViewPatterns_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewPatterns.SelectedIndices.Count <= 0)
			{
				EnableFindReplaceInput(false);
				return;
			}
			EnableFindReplaceInput(true);
			PopulateFindReplaceFields();
			PopulateNames();
		}

		private void listViewPatterns_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
			// If this item is the selected item
			if (e.Item.Selected)
			{
				// If the selected item has focus Set the colors to the normal colors for a selected item
				e.Item.ForeColor = SystemColors.HighlightText;
				e.Item.BackColor = SystemColors.Highlight;
			}
			else
			{
				// Set the normal colors for items that are not selected
				e.Item.ForeColor = listViewPatterns.ForeColor;
				e.Item.BackColor = listViewPatterns.BackColor;
			}
			e.DrawBackground();
			e.DrawText();
		}

		private void buttonMovePatternUp_Click(object sender, EventArgs e)
		{
			int index = listViewPatterns.SelectedIndices[0];
			if (listViewPatterns.SelectedIndices.Count <= 0)
				return;
			if (index <= 0)
				return;

			var pattern = _patterns[index - 1];
			_patterns[index - 1] = _patterns[index];
			_patterns[index] = pattern;
			SyncPatternsToListView();
			PopulateNames();
			listViewPatterns.Items[index - 1].Selected = true;
		}

		private void buttonMovePatternDown_Click(object sender, EventArgs e)
		{
			int index = listViewPatterns.SelectedIndices[0];
			if (listViewPatterns.SelectedIndices.Count <= 0)
				return;

			if (index >= _patterns.Count - 1)
				return;

			var pattern = _patterns[index + 1];
			_patterns[index + 1] = _patterns[index];
			_patterns[index] = pattern;
			SyncPatternsToListView();
			PopulateNames();
			listViewPatterns.Items[index + 1].Selected = true;
		}
	}

	internal class ReplacePattern
	{
		public string Find { get; set; }

		public string Replace { get; set; }

		public override string ToString()
		{
			string text = "Nothing to find.";
			if (!string.IsNullOrEmpty(Find))
			{
				text = string.Format("Replace '{0}' with '{1}'", Find, Replace);
			}

			return text;
		}
	}
}
