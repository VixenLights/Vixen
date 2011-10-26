using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.Output;
using Vixen.Sys;


namespace VixenApplication
{
	public partial class ConfigControllers : Form
	{
		private OutputController _displayedController;
		public ConfigControllers()
		{
			InitializeComponent();
			_displayedController = null;
		}

		private void _PopulateControllerList()
		{
			listViewControllers.BeginUpdate();
			listViewControllers.Items.Clear();

			foreach(OutputController oc in VixenSystem.Controllers) {
				ListViewItem item = new ListViewItem();
				item.Text = oc.Name;
				item.SubItems.Add(Vixen.Sys.ApplicationServices.GetModuleDescriptor(oc.OutputModuleId).TypeName);
				item.SubItems.Add(oc.OutputCount.ToString());
				item.Tag = oc;
				listViewControllers.Items.Add(item);
			}

			listViewControllers.EndUpdate();

			foreach (ListViewItem item in listViewControllers.Items) {
				if (item.Tag == _displayedController)
					item.Selected = true;
			}
		}

		private void _PopulateFormWithController(OutputController oc)
		{
			_displayedController = oc;

			if (oc == null) {
				textBoxName.Text = "";
				numericUpDownOutputCount.Value = 0;
				buttonDeleteController.Enabled = false;
				groupBoxSelectedController.Enabled = false;
			} else {
				textBoxName.Text = oc.Name;
				numericUpDownOutputCount.Value = oc.OutputCount;
				buttonDeleteController.Enabled = true;
				groupBoxSelectedController.Enabled = true;
			}
		}

		private void ConfigControllers_Load(object sender, EventArgs e)
		{
			_PopulateControllerList();
			_PopulateFormWithController(null);
		}

		private void buttonAddController_Click(object sender, EventArgs e)
		{
			List<KeyValuePair<string, object>> outputModules = new List<KeyValuePair<string, object>>();
			foreach (KeyValuePair<Guid, string> kvp in ApplicationServices.GetAvailableModules<IOutputModuleInstance>()) {
				outputModules.Add(new KeyValuePair<string, object>(kvp.Value, kvp.Key));
			}
			CommonElements.ListSelectDialog addForm = new CommonElements.ListSelectDialog("Add Controller", (outputModules));
			if (addForm.ShowDialog() == DialogResult.OK) {
				IModuleDescriptor moduleDescriptor = ApplicationServices.GetModuleDescriptor((Guid)addForm.SelectedItem);
				string name = "New " + moduleDescriptor.TypeName + " Controller";
				OutputController oc = new OutputController(name, 0, (Guid)addForm.SelectedItem);
				VixenSystem.Controllers.AddController(oc);

				// select the new controller, and then repopulate the list -- it will make sure the currently
				// displayed controller is selected.
				_PopulateFormWithController(oc);
				_PopulateControllerList();
			}
		}

		private void buttonDeleteController_Click(object sender, EventArgs e)
		{
			string message, title;
			if (listViewControllers.SelectedItems.Count > 1) {
				message = "Are you sure you want to delete the selected controllers?";
				title = "Delete Controllers?";
			} else {
				message = "Are you sure you want to delete the selected controller?";
				title = "Delete Controller?";
			}

			if (listViewControllers.SelectedItems.Count > 0) {
				if (MessageBox.Show(message, title, MessageBoxButtons.OKCancel) == DialogResult.OK) {
					foreach (ListViewItem item in listViewControllers.SelectedItems) {
						OutputController oc = item.Tag as OutputController;
						VixenSystem.Controllers.RemoveController(oc);
					}
					_PopulateControllerList();
				}
			}
		}

		private void buttonUpdate_Click(object sender, EventArgs e)
		{
			if (_displayedController == null)
				return;

			_displayedController.Name = textBoxName.Text;

			// iterate through the outputs, and add new outputs with default names if needed
			int oldCount = _displayedController.OutputCount;
			int newCount = (int)numericUpDownOutputCount.Value;
			_displayedController.OutputCount = newCount;
			for (int i = oldCount; i < newCount; i++) {
				_displayedController.Outputs[i].Name = _displayedController.Name + "-" + (i + 1);
			}

			_PopulateControllerList();
		}

		private void buttonConfigureController_Click(object sender, EventArgs e)
		{
			if (listViewControllers.SelectedItems.Count == 1) {
				(listViewControllers.SelectedItems[0].Tag as OutputController).Setup();
			}
		}

		private void buttonGenerateChannels_Click(object sender, EventArgs e)
		{
			int controllerCount = 0;
			int outputCount = 0;

			if (listViewControllers.SelectedItems.Count >= 1) {
				foreach (ListViewItem item in (listViewControllers.SelectedItems)) {
					int channelsAdded = 0;
					
					// for each selected output controller, build up the controller and its list of output references.
					OutputController oc = (OutputController)item.Tag;
					List<ControllerReference> refsToAdd = new List<ControllerReference>();
					for (int i = 0; i < oc.OutputCount; i++) {
						refsToAdd.Add(new ControllerReference(oc.Id, i));
					}

					// iterate through all nodes, trying to find any of the references we will need to add. If we
					// find them, then remove them from the list as we don't need to add them anymore.
					foreach(ChannelNode node in VixenSystem.Nodes) {
						if (node.Channel != null) {
							foreach (ControllerReference cr in node.Channel.Patch) {
								if (refsToAdd.Contains(cr)) {
									refsToAdd.Remove(cr);
								}
							}
							if (refsToAdd.Count == 0) {
								break;
							}
						}
					}

					// add any controller references we have left.
					foreach (ControllerReference cr in refsToAdd) {
						string name = VixenSystem.Controllers.Get(cr.ControllerId).Outputs[cr.OutputIndex].Name;

						ChannelNode newNode = VixenSystem.Nodes.AddNewNode(name);
						if (newNode.Channel == null) {
							newNode.Channel = VixenSystem.Channels.AddChannel(name);
						}
						newNode.Channel.Patch.Add(cr);
						channelsAdded++;
					}

					if (channelsAdded > 0) {
						controllerCount++;
						outputCount += channelsAdded;
					}
				}

				if (outputCount > 0) {
					string message = outputCount + " channels added";
					if (listViewControllers.SelectedItems.Count > 1) {
						message += " for outputs on " + controllerCount + " controller" + ((controllerCount > 1) ? "s." : ".");
					} else {
						message += ".";
					}
					MessageBox.Show(message, "Channels Addded");
				} else {
					MessageBox.Show("All outputs for this controller are referenced in channels already.", "No Channels Addded");
				}
			}
		}

		private void listViewControllers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewControllers.SelectedItems.Count > 1 || listViewControllers.SelectedItems.Count == 0) {
				_PopulateFormWithController(null);
			} else {
				_PopulateFormWithController(listViewControllers.SelectedItems[0].Tag as OutputController);
			}
		}

		private void buttonConfigureOutputs_Click(object sender, EventArgs e)
		{
			if (listViewControllers.SelectedItems.Count > 0) {
				ConfigControllersOutputs outputsForm = new ConfigControllersOutputs(listViewControllers.SelectedItems[0].Tag as OutputController);
				outputsForm.ShowDialog();
			}
		}

	}
}
