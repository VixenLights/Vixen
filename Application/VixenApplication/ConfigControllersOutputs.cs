using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Vixen.Module.OutputFilter;
using Vixen.Sys;
using Common.Controls;
using Vixen.Sys.Output;
using Vixen.Services;

namespace VixenApplication
{
	public partial class ConfigControllersOutputs : Form
	{
		private OutputController _controller;
		private int _selectedOutputIndex;
		private List<IOutputFilterModuleInstance> _clipboardFilters;

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

			string ts = string.Join(", ", _controller.GetAllOutputFilters(outputIndex).Select(x => x.Descriptor.TypeName));

			item.SubItems.Add(ts);
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
			listViewFilters.Items.Clear();

			if (!IsIndexValid(outputIndex)) {
				groupBox.Enabled = false;
				textBoxName.Text = "";
			} else {
				groupBox.Enabled = true;
				textBoxName.Text = _controller.Outputs[outputIndex].Name;

				foreach(var filter in _controller.GetAllOutputFilters(outputIndex)) {
					ListViewItem item = new ListViewItem();
					item.Text = filter.Descriptor.TypeName;
					item.Tag = filter;
					listViewFilters.Items.Add(item);
				}

				buttonDelete.Enabled = false;
				buttonConfigure.Enabled = false;
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

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			List<KeyValuePair<string, object>> newTransforms = new List<KeyValuePair<string, object>>();
			foreach(KeyValuePair<Guid, string> kvp in ApplicationServices.GetAvailableModules<IOutputFilterModuleInstance>()) {
				newTransforms.Add(new KeyValuePair<string, object>(kvp.Value, kvp.Key));
			}
			ListSelectDialog addForm = new ListSelectDialog("Add Filter", (newTransforms));
			if(addForm.ShowDialog() == DialogResult.OK) {

				IOutputFilterModuleInstance newInstance = ApplicationServices.Get<IOutputFilterModuleInstance>((Guid)addForm.SelectedItem);
				if(newInstance == null)
					VixenSystem.Logging.Error("ConfigControllersOutput: null instance trying to create filter!");
				else
					_controller.AddOutputFilter(_selectedOutputIndex, newInstance);
			}

			_redrawOutputList();
			_populateListViewItemWithDetails(listViewOutputs.Items[_selectedOutputIndex], _selectedOutputIndex);
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			foreach(ListViewItem lvi in listViewFilters.SelectedItems) {
				IOutputFilterModuleInstance filter = (IOutputFilterModuleInstance)lvi.Tag;
				_controller.RemoveOutputFilter(_selectedOutputIndex, filter);
			}

			_redrawOutputList();
		}

		private void buttonConfigure_Click(object sender, EventArgs e)
		{
			ConfigureSelectedFilter();
		}

		private void ConfigureSelectedFilter() {
			IOutputFilterModuleInstance transform = (IOutputFilterModuleInstance)listViewFilters.SelectedItems[0].Tag;
			if(transform != null)
				transform.Setup();
		}

		private void buttonUpdate_Click(object sender, EventArgs e)
		{
			_controller.Outputs[_selectedOutputIndex].Name = textBoxName.Text;
			_redrawOutputList();
		}

		private void listViewTransforms_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewFilters.SelectedItems.Count > 0) {
				buttonDelete.Enabled = true;
				buttonConfigure.Enabled = true;
			} else {
				buttonDelete.Enabled = false;
				buttonConfigure.Enabled = false;
			}
		}

		private void buttonBulkRename_Click(object sender, EventArgs e)
		{
			if (listViewOutputs.SelectedItems.Count <= 1)
				return;

			List<string> oldNames = new List<string>();
			foreach (ListViewItem selectedItem in listViewOutputs.SelectedItems) {
				oldNames.Add(selectedItem.SubItems[1].Text);
			}

			BulkRename renameForm = new BulkRename(oldNames.ToArray());

			if (renameForm.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				for (int i = 0; i < listViewOutputs.SelectedItems.Count; i++) {
					if (i >= renameForm.NewNames.Length) {
						VixenSystem.Logging.Warn("ConfigControllersOutputs: bulk renaming outputs, and ran out of new names!");
						break;
					}
					int outputIndex = int.Parse(listViewOutputs.SelectedItems[i].Text) - 1;
					_controller.Outputs[outputIndex].Name = renameForm.NewNames[i];
				}

				_populateOutputsList();
				_populateFormWithOutput(_selectedOutputIndex, true);
			}
		}

		private void listViewTransforms_DoubleClick(object sender, EventArgs e)
		{
			ConfigureSelectedFilter();
		}

		private void copyTransformsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(listViewOutputs.Focused) {
				if(listViewOutputs.SelectedItems.Count != 1)
					return;

				int outputIndex = (int)listViewOutputs.SelectedItems[0].Tag;
				_clipboardFilters = new List<IOutputFilterModuleInstance>(_controller.GetAllOutputFilters(outputIndex));
			} else if(listViewFilters.Focused) {
				_clipboardFilters = new List<IOutputFilterModuleInstance>();
				foreach(ListViewItem lvi in listViewFilters.SelectedItems) {
					IOutputFilterModuleInstance instance = lvi.Tag as IOutputFilterModuleInstance;
					if(instance != null)
						_clipboardFilters.Add(instance);
				}
				if(_clipboardFilters.Count <= 0)
					_clipboardFilters = null;
			}
		}

		private void pasteTransformsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(listViewOutputs.Focused) {
				foreach(ListViewItem lvi in listViewOutputs.SelectedItems) {
					int outputIndex = (int)lvi.Tag;

					foreach(IOutputFilterModuleInstance sourceTransform in _clipboardFilters) {
						IOutputFilterModuleInstance newInstance = (IOutputFilterModuleInstance)sourceTransform.Clone();
						_controller.AddOutputFilter(outputIndex, newInstance);
					}
				}

				_redrawOutputList();
			} else if(listViewFilters.Focused) {
				foreach(IOutputFilterModuleInstance sourceTransform in _clipboardFilters) {
					IOutputFilterModuleInstance newInstance = (IOutputFilterModuleInstance)sourceTransform.Clone();
					_controller.AddOutputFilter(_selectedOutputIndex, newInstance);
				}
				_populateFormWithOutput(_selectedOutputIndex, true);
			}
		}

		private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			if(listViewOutputs.Focused) {
				copyTransformsToolStripMenuItem.Enabled = false;

				foreach(ListViewItem lvi in listViewOutputs.SelectedItems) {
					int outputIndex = (int)lvi.Tag;
					IEnumerable<IOutputFilterModuleInstance> filters = _controller.GetAllOutputFilters(outputIndex);
					if(filters != null && filters.Any()) {
						copyTransformsToolStripMenuItem.Enabled = true;
						break;
					}
				}

				pasteTransformsToolStripMenuItem.Enabled = _clipboardFilters != null && listViewOutputs.SelectedItems.Count > 0;

			} else if(listViewOutputs.Focused) {
				copyTransformsToolStripMenuItem.Enabled = listViewOutputs.SelectedItems.Count > 0;
				pasteTransformsToolStripMenuItem.Enabled = _clipboardFilters != null;
			} else {
				copyTransformsToolStripMenuItem.Enabled = false;
				pasteTransformsToolStripMenuItem.Enabled = false;
			}
		}

		private void deleteAllTransformsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(listViewOutputs.Focused) {
				foreach(ListViewItem lvi in listViewOutputs.SelectedItems) {
					int outputIndex = (int)lvi.Tag;
					foreach(IOutputFilterModuleInstance instance in _controller.GetAllOutputFilters(outputIndex).ToArray()) {
						_controller.RemoveOutputFilter(outputIndex, instance);
					}
				}
			} else if(listViewFilters.Focused) {
				foreach(ListViewItem lvi in listViewFilters.SelectedItems) {
					IOutputFilterModuleInstance instance = (IOutputFilterModuleInstance)lvi.Tag;
					if(instance != null)
						_controller.RemoveOutputFilter(_selectedOutputIndex, instance);
				}
			}
		}
	}
}
