using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vixen.Sys;
using Common.Controls;
using Vixen.Sys.Output;

namespace VixenApplication
{
	public partial class ConfigControllersOutputs : Form
	{
		private OutputController _controller;
		private int _selectedOutputIndex;

		public ConfigControllersOutputs(OutputController controller)
		{
			InitializeComponent();
			_controller = controller;
		}

		private void ConfigControllersOutputs_Load(object sender, EventArgs e)
		{
			_populateOutputsList();
			_populateFormWithOutput(-1);
		}

		private void _populateListViewItemWithDetails(ListViewItem item, int outputIndex)
		{
			if (!IsIndexValid(outputIndex))
				return;

			CommandOutput output = _controller.Outputs[outputIndex];
			item.Text = (outputIndex + 1).ToString();
			item.SubItems.Add(output.Name);
			item.Tag = outputIndex;
		}

		private void _populateOutputsList()
		{
			listViewOutputs.BeginUpdate();
			listViewOutputs.Items.Clear();
			for (int i = 0; i < _controller.OutputCount; i++) {
				ListViewItem item = new ListViewItem();
				_populateListViewItemWithDetails(item, i);
				listViewOutputs.Items.Add(item);
			}
			listViewOutputs.EndUpdate();
		}

		private void _populateFormWithOutput(int outputIndex, bool forceUpdate = false)
		{
			if (outputIndex == _selectedOutputIndex && !forceUpdate)
				return;

			_selectedOutputIndex = outputIndex;

			if (!IsIndexValid(outputIndex)) {
				groupBox.Enabled = false;
				textBoxName.Text = "";
			} else {
				groupBox.Enabled = true;
				textBoxName.Text = _controller.Outputs[outputIndex].Name;
			}
		}

		private void _redrawOutputList()
		{
			int topIndex = listViewOutputs.Items.IndexOf(listViewOutputs.TopItem);
			int selectedIndex = listViewOutputs.SelectedIndices[0];
			_populateOutputsList();
			listViewOutputs.TopItem = listViewOutputs.Items[topIndex];
			listViewOutputs.Items[selectedIndex].Selected = true;
			_populateFormWithOutput(_selectedOutputIndex, true);
		}

		private bool IsIndexValid(int index)
		{
			return (index >= 0 && index < _controller.OutputCount);
		}

		private void listViewOutputs_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewOutputs.SelectedIndices.Count <= 0)
				_populateFormWithOutput(-1);
			else
				_populateFormWithOutput(listViewOutputs.SelectedIndices[0]);

			buttonBulkRename.Enabled = (listViewOutputs.SelectedIndices.Count > 1);
		}

		private void buttonUpdate_Click(object sender, EventArgs e)
		{
			_controller.Outputs[_selectedOutputIndex].Name = textBoxName.Text;
			_redrawOutputList();
		}

		private void buttonBulkRename_Click(object sender, EventArgs e)
		{
			if (listViewOutputs.SelectedItems.Count <= 1)
				return;

			List<string> oldNames = new List<string>();
			foreach (ListViewItem selectedItem in listViewOutputs.SelectedItems) {
				oldNames.Add(selectedItem.SubItems[1].Text);
			}

			//BulkRename renameForm = new BulkRename(oldNames.ToArray());

			//if (renameForm.ShowDialog() == DialogResult.OK) {
			//    for (int i = 0; i < listViewOutputs.SelectedItems.Count; i++) {
			//        if (i >= renameForm.NewNames.Length) {
			//            VixenSystem.Logging.Warn("ConfigControllersOutputs: bulk renaming outputs, and ran out of new names!");
			//            break;
			//        }
			//        int outputIndex = int.Parse(listViewOutputs.SelectedItems[i].Text) - 1;
			//        _controller.Outputs[outputIndex].Name = renameForm.NewNames[i];
			//    }

			//    _populateOutputsList();
			//    _populateFormWithOutput(_selectedOutputIndex, true);
			//}
		}
	}
}
