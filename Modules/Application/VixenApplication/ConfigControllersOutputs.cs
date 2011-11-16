using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Vixen.Module.Output;
using Vixen.Module.Transform;
using Vixen.Sys;
using Vixen.Execution;
using CommonElements;

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

			OutputController.Output output = _controller.Outputs[outputIndex];
			item.Text = (outputIndex + 1).ToString();
			item.SubItems.Add(output.Name);
			String ts = "";
			if (_controller.OutputTransforms.ContainsKey(outputIndex)) {
				HashSet<Tuple<Guid, Guid>> transforms = _controller.OutputTransforms[outputIndex];
				foreach (Tuple<Guid, Guid> transform in transforms) {
					// Tuple is: item1 - module type GUID, item2 - module instance GUID
					if (ts == "")
						ts = ApplicationServices.GetModuleDescriptor(transform.Item1).TypeName;
					else
						ts += ", " + ApplicationServices.GetModuleDescriptor(transform.Item1).TypeName;
				}
			}
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
			listViewTransforms.Items.Clear();

			if (!IsIndexValid(outputIndex)) {
				groupBox.Enabled = false;
				textBoxName.Text = "";
			} else {
				groupBox.Enabled = true;
				textBoxName.Text = _controller.Outputs[outputIndex].Name;
				if (_controller.OutputTransforms.ContainsKey(outputIndex)) {
					HashSet<Tuple<Guid, Guid>> transforms = _controller.OutputTransforms[outputIndex];
					foreach (Tuple<Guid, Guid> transform in transforms) {
						// Tuple is: item1 - module type GUID, item2 - module instance GUID
						ListViewItem item = new ListViewItem();
						item.Text = ApplicationServices.GetModuleDescriptor(transform.Item1).TypeName;
						item.Tag = transform.Item2;		// save the instance ID to be able to configure it later
						listViewTransforms.Items.Add(item);
					}
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
			foreach (KeyValuePair<Guid, string> kvp in ApplicationServices.GetAvailableModules<ITransformModuleInstance>()) {
				newTransforms.Add(new KeyValuePair<string, object>(kvp.Value, kvp.Key));
			}
			ListSelectDialog addForm = new ListSelectDialog("Add Transform", (newTransforms));
			if (addForm.ShowDialog() == DialogResult.OK) {

				ModuleInstanceSpecification<int> allTransforms = _controller.OutputTransforms;
				HashSet<Tuple<Guid,Guid>> outputTransforms;
				if (allTransforms.ContainsKey(_selectedOutputIndex)) {
					outputTransforms = allTransforms[_selectedOutputIndex];
				} else {
					outputTransforms = new HashSet<Tuple<Guid,Guid>>();
				}

				ITransformModuleInstance newInstance = ApplicationServices.Get<ITransformModuleInstance>((Guid)addForm.SelectedItem);
				outputTransforms.Add(new Tuple<Guid, Guid>(newInstance.Descriptor.TypeId, newInstance.InstanceId));
				allTransforms[_selectedOutputIndex] = outputTransforms;
				_controller.OutputTransforms = allTransforms;
			}

			_redrawOutputList();
			_populateListViewItemWithDetails(listViewOutputs.Items[_selectedOutputIndex], _selectedOutputIndex);
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem lvi in listViewTransforms.SelectedItems) {
				Guid instanceToRemoveId = (Guid)lvi.Tag;

				ModuleInstanceSpecification<int> allTransforms = _controller.OutputTransforms;
				if (!allTransforms.ContainsKey(_selectedOutputIndex)) {
					VixenSystem.Logging.Error("ConfigControllersOutputs: can't remove selected transforms, as the " +
						"controller doesn't have any transforms for output index " + _selectedOutputIndex);
					continue;
				}

				HashSet<Tuple<Guid, Guid>> outputTransforms = allTransforms[_selectedOutputIndex];
				outputTransforms.Remove(outputTransforms.Single(x => x.Item2 == instanceToRemoveId));
				allTransforms[_selectedOutputIndex] = outputTransforms;
				_controller.OutputTransforms = allTransforms;
			}

			_redrawOutputList();
		}

		private void buttonConfigure_Click(object sender, EventArgs e)
		{
			ConfigureSelectedTransform();
		}

		private void ConfigureSelectedTransform()
		{
			if (listViewTransforms.SelectedItems.Count <= 0)
				return;

			Guid transformInstanceID = (Guid)listViewTransforms.SelectedItems[0].Tag;
			ITransformModuleInstance instance = _controller.OutputModule.GetTransforms(_selectedOutputIndex).Single(x => x.InstanceId == transformInstanceID);
			if (instance != null)
				instance.Setup();
		}

		private void buttonUpdate_Click(object sender, EventArgs e)
		{
			_controller.Outputs[_selectedOutputIndex].Name = textBoxName.Text;
			_redrawOutputList();
		}

		private void listViewTransforms_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewTransforms.SelectedItems.Count > 0) {
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
			ConfigureSelectedTransform();
		}
	}
}
