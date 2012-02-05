using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Vixen.Sys;
using CommonElements;
using Vixen.Sys.Output;

namespace VixenApplication
{
	public partial class ConfigControllersOutputs : Form
	{
		private OutputController _controller;
		private int _selectedOutputIndex;
		//private List<ITransformModuleInstance> _clipboardTransforms;

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
			item.Tag = outputIndex;
			String ts = "";

			//foreach (ITransformModuleInstance transform in _controller.OutputModule.GetTransforms(outputIndex)) {
			//    if (ts == "")
			//        ts = transform.Descriptor.TypeName;
			//    else
			//        ts += ", " + transform.Descriptor.TypeName;
			//}

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

				//foreach (ITransformModuleInstance transform in _controller.OutputModule.GetTransforms(outputIndex)) {
				//    ListViewItem item = new ListViewItem();
				//    item.Text = transform.Descriptor.TypeName;
				//    item.Tag = transform;
				//    listViewTransforms.Items.Add(item);
				//}

				buttonDelete.Enabled = false;
				buttonConfigure.Enabled = false;
				button1.Enabled = outputIndex >= 0 && _controller.Outputs[outputIndex].PostFilters.Any(x => x.HasSetup);
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
			//List<KeyValuePair<string, object>> newTransforms = new List<KeyValuePair<string, object>>();
			//foreach (KeyValuePair<Guid, string> kvp in ApplicationServices.GetAvailableModules<ITransformModuleInstance>()) {
			//    newTransforms.Add(new KeyValuePair<string, object>(kvp.Value, kvp.Key));
			//}
			//ListSelectDialog addForm = new ListSelectDialog("Add Transform", (newTransforms));
			//if (addForm.ShowDialog() == DialogResult.OK) {

			//    ITransformModuleInstance newInstance = ApplicationServices.Get<ITransformModuleInstance>((Guid)addForm.SelectedItem);
			//    if (newInstance == null)
			//        VixenSystem.Logging.Error("ConfigControllersOutput: null instance trying to create transform!");
			//    else
			//        _controller.OutputModule.AddTransform(_selectedOutputIndex, newInstance);
			//}

			//_redrawOutputList();
			//_populateListViewItemWithDetails(listViewOutputs.Items[_selectedOutputIndex], _selectedOutputIndex);
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			//foreach (ListViewItem lvi in listViewTransforms.SelectedItems) {
			//    ITransformModuleInstance transform = lvi.Tag as ITransformModuleInstance;
			//    if (transform == null)
			//        continue;

			//    _controller.OutputModule.RemoveTransform(_selectedOutputIndex, transform.Descriptor.TypeId, transform.InstanceId);
			//}

			//_redrawOutputList();
		}

		private void buttonConfigure_Click(object sender, EventArgs e)
		{
			//ConfigureSelectedTransform();
		}

		//private void ConfigureSelectedTransform()
		//{
		//    if (listViewTransforms.SelectedItems.Count <= 0)
		//        return;

		//    ITransformModuleInstance transform = listViewTransforms.SelectedItems[0].Tag as ITransformModuleInstance;
		//    if (transform != null)
		//        transform.Setup();
		//}

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
			//ConfigureSelectedTransform();
		}

		private void copyTransformsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//if (listViewOutputs.Focused) {
			//    if (listViewOutputs.SelectedItems.Count != 1)
			//        return;

			//    int outputIndex = (int)listViewOutputs.SelectedItems[0].Tag;
			//    _clipboardTransforms = new List<ITransformModuleInstance>(_controller.OutputModule.GetTransforms(outputIndex));
			//} else if (listViewTransforms.Focused) {
			//    _clipboardTransforms = new List<ITransformModuleInstance>();
			//    foreach (ListViewItem lvi in listViewTransforms.SelectedItems) {
			//        ITransformModuleInstance instance = lvi.Tag as ITransformModuleInstance;
			//        if (instance != null)
			//            _clipboardTransforms.Add(instance);
			//    }
			//    if (_clipboardTransforms.Count <= 0)
			//        _clipboardTransforms = null;
			//}
		}

		private void pasteTransformsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//if (listViewOutputs.Focused) {
			//    foreach (ListViewItem lvi in listViewOutputs.SelectedItems) {
			//        int outputIndex = (int)lvi.Tag;

			//        foreach (ITransformModuleInstance sourceTransform in _clipboardTransforms) {
			//            ITransformModuleInstance newInstance = sourceTransform.Clone() as ITransformModuleInstance;
			//            _controller.OutputModule.AddTransform(outputIndex, newInstance);
			//        }
			//    }

			//    _redrawOutputList();
			//} else if (listViewTransforms.Focused) {
			//    foreach (ITransformModuleInstance sourceTransform in _clipboardTransforms) {
			//        ITransformModuleInstance newInstance = sourceTransform.Clone() as ITransformModuleInstance;
			//        _controller.OutputModule.AddTransform(_selectedOutputIndex, newInstance);
			//    }
			//    _populateFormWithOutput(_selectedOutputIndex, true);
			//}
		}

		private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			//if (listViewOutputs.Focused) {
			//    copyTransformsToolStripMenuItem.Enabled = false;

			//    foreach (ListViewItem lvi in listViewOutputs.SelectedItems) {
			//        int outputIndex = (int)lvi.Tag;
			//        IEnumerable<ITransformModuleInstance> transforms = _controller.OutputModule.GetTransforms(outputIndex);
			//        if (transforms != null && transforms.Count() > 0) {
			//            copyTransformsToolStripMenuItem.Enabled = true;
			//            break;
			//        }
			//    }

			//    pasteTransformsToolStripMenuItem.Enabled = _clipboardTransforms != null && listViewOutputs.SelectedItems.Count > 0;

			//} else if (listViewTransforms.Focused) {
			//    copyTransformsToolStripMenuItem.Enabled = listViewTransforms.SelectedItems.Count > 0;
			//    pasteTransformsToolStripMenuItem.Enabled = _clipboardTransforms != null;
			//} else {
			//    copyTransformsToolStripMenuItem.Enabled = false;
			//    pasteTransformsToolStripMenuItem.Enabled = false;
			//}
		}

		private void deleteAllTransformsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//if (listViewOutputs.Focused) {
			//    foreach (ListViewItem lvi in listViewOutputs.SelectedItems) {
			//        int outputIndex = (int)lvi.Tag;
			//        foreach (ITransformModuleInstance instance in _controller.OutputModule.GetTransforms(outputIndex).ToArray()) {
			//            _controller.OutputModule.RemoveTransform(outputIndex, instance.Descriptor.TypeId, instance.InstanceId);
			//        }
			//    }
			//} else if (listViewTransforms.Focused) {
			//    foreach (ListViewItem lvi in listViewTransforms.SelectedItems) {
			//        ITransformModuleInstance instance = lvi.Tag as ITransformModuleInstance;
			//        if (instance != null)
			//            _controller.OutputModule.RemoveTransform(_selectedOutputIndex, instance.Descriptor.TypeId, instance.InstanceId);
			//    }
			//}
		}

		private void button1_Click(object sender, EventArgs e) {
			_controller.Outputs[_selectedOutputIndex].PostFilters.First(x => x.HasSetup).Setup();
		}
	}
}
