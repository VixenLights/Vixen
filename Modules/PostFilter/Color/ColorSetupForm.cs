using System;
using System.Linq;
using System.Windows.Forms;

namespace VixenModules.OutputFilter.Color {
	public partial class ColorSetupForm : Form {
		public ColorSetupForm(ColorData data) {
			InitializeComponent();
			_SelectedFilters = data.FilterOrder;
		}

		private void ColorSetupForm_Load(object sender, EventArgs e) {
			checkedListBox.Items.AddRange(Enum.GetValues(typeof(ColorFilter)).Cast<object>().ToArray());
		}

		public ColorFilter[] SelectedFilters { get; private set; }

		private ColorFilter[] _SelectedFilters {
			get { return checkedListBox.CheckedItems.Cast<ColorFilter>().ToArray(); }
			set {
				foreach(ColorFilter colorFilter in value) {
					checkedListBox.SetItemChecked(checkedListBox.Items.IndexOf(colorFilter), true);
				}
			}
		}

		private void _MoveFilter(int index, int delta) {
			object value = checkedListBox.Items[index];
			checkedListBox.Items.RemoveAt(index);
			index += delta;
			checkedListBox.Items.Insert(index, value);
			checkedListBox.SelectedIndex = index;
		}

		private void checkedListBox_SelectedIndexChanged(object sender, EventArgs e) {
			buttonMoveUp.Enabled = checkedListBox.SelectedIndex > 0;
			buttonMoveDown.Enabled = checkedListBox.SelectedIndex < (checkedListBox.Items.Count - 1);
		}

		private void buttonMoveUp_Click(object sender, EventArgs e) {
			_MoveFilter(checkedListBox.SelectedIndex, -1);
		}

		private void buttonMoveDown_Click(object sender, EventArgs e) {
			_MoveFilter(checkedListBox.SelectedIndex, 1);
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			SelectedFilters = _SelectedFilters;

			if(SelectedFilters.Length == 0) {
				DialogResult = DialogResult.None;
				MessageBox.Show("You don't have any filters set.");
			}
		}
	}
}
