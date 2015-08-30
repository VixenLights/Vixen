using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Resources.Properties;
using Vixen.Data.Flow;
using Vixen.Factory;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Output;


namespace VixenApplication
{
	public partial class ConfigControllers : Form
	{
		private OutputController _displayedController;
		private bool _internal;
		private bool _changesMade;

		public ConfigControllers()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			_displayedController = null;
		}

		private void _PopulateControllerList()
		{
			listViewControllers.BeginUpdate();
			listViewControllers.Items.Clear();

			foreach (OutputController oc in VixenSystem.OutputControllers) {
				ListViewItem item = new ListViewItem();
				item.Text = oc.Name;
				item.Checked = oc.IsRunning;
				item.SubItems.Add(ApplicationServices.GetModuleDescriptor(oc.ModuleId).TypeName);
				item.SubItems.Add(oc.OutputCount.ToString());
				item.Tag = oc;
				// I'm sorry for this.  Someone know of a better way?
				_internal = true;
				listViewControllers.Items.Add(item);
				_internal = false;
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
				textBoxName.Text = string.Empty;
				numericUpDownOutputCount.Value = 0;
				buttonDeleteController.Enabled = false;
				groupBoxSelectedController.Enabled = false;
			}
			else {
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
			foreach (KeyValuePair<Guid, string> kvp in ApplicationServices.GetAvailableModules<IControllerModuleInstance>()) {
				outputModules.Add(new KeyValuePair<string, object>(kvp.Value, kvp.Key));
			}
			Common.Controls.ListSelectDialog addForm = new Common.Controls.ListSelectDialog("Add Controller", (outputModules));
			if (addForm.ShowDialog() == DialogResult.OK) {
				IModuleDescriptor moduleDescriptor = ApplicationServices.GetModuleDescriptor((Guid) addForm.SelectedItem);
				string name = moduleDescriptor.TypeName;
				ControllerFactory controllerFactory = new ControllerFactory();
				OutputController oc = (OutputController) controllerFactory.CreateDevice((Guid) addForm.SelectedItem, name);
				VixenSystem.OutputControllers.Add(oc);
				// In the case of a controller that has a form, the form will not be shown
				// until this event handler completes.  To make sure it's in a visible state
				// before evaluating if it's running or not, we're calling DoEvents.
				// I hate DoEvents calls, so if you know of a better way...
				Application.DoEvents();

				// select the new controller, and then repopulate the list -- it will make sure the currently
				// displayed controller is selected.
				_PopulateFormWithController(oc);
				_PopulateControllerList();

				//We added a controller so set the _changesMade to true
				_changesMade = true;
			}
		}

		private void buttonDeleteController_Click(object sender, EventArgs e)
		{
			string message, title;
			if (listViewControllers.SelectedItems.Count > 1) {
				message = "Are you sure you want to delete the selected controllers?";
				title = "Delete Controllers?";
			}
			else {
				message = "Are you sure you want to delete the selected controller?";
				title = "Delete Controller?";
			}

			if (listViewControllers.SelectedItems.Count > 0) {
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(message, title, false, true);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.OK)
				{
					foreach (ListViewItem item in listViewControllers.SelectedItems) {
						OutputController oc = item.Tag as OutputController;
						VixenSystem.OutputControllers.Remove(oc);
					}
					_PopulateControllerList();

					//we deleted at least one controller so set changes made = true
					_changesMade = true;
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
			int newCount = (int) numericUpDownOutputCount.Value;
			_displayedController.OutputCount = newCount;
			for (int i = oldCount; i < newCount; i++) {
				_displayedController.Outputs[i].Name = string.Format("{0}-{1}", _displayedController.Name, (i + 1));
			}

			_PopulateControllerList();

			//we added controller outputs so set changes made to true
			_changesMade = true;
		}

		private void buttonConfigureController_Click(object sender, EventArgs e)
		{
			ConfigureSelectedController();
		}

		private void listViewControllers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewControllers.SelectedItems.Count > 1 || listViewControllers.SelectedItems.Count == 0) {
				_PopulateFormWithController(null);
			}
			else {
				_PopulateFormWithController(listViewControllers.SelectedItems[0].Tag as OutputController);
			}
		}

		private void buttonConfigureOutputs_Click(object sender, EventArgs e)
		{
			if (listViewControllers.SelectedItems.Count > 0) {
				using (
					ConfigControllersOutputs outputsForm =
						new ConfigControllersOutputs(listViewControllers.SelectedItems[0].Tag as OutputController)) {
					if (outputsForm.ShowDialog() == DialogResult.OK) {
						//make the assumption that changes were made in the Controller channel setup
						_changesMade = true;
					}
				}
			}
		}

		private void listViewControllers_DoubleClick(object sender, EventArgs e)
		{
			ConfigureSelectedController();
		}

		private void ConfigureSelectedController()
		{
			if (listViewControllers.SelectedItems.Count == 1) {
				(listViewControllers.SelectedItems[0].Tag as OutputController).Setup();

				//We made a change to the controller configuration so set our changes made to true
				_changesMade = true;
			}
		}

		private void listViewControllers_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			// This is going to be fired every time something is added to the listview.
			if (!_internal) {
				OutputController controller = e.Item.Tag as OutputController;
				if (e.Item.Checked) {
					VixenSystem.OutputControllers.Start(controller);
				}
				else {
					VixenSystem.OutputControllers.Stop(controller);
				}
			}
		}

		private void ConfigControllers_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_changesMade) {
				if (DialogResult == DialogResult.Cancel) {
					switch (
						MessageBox.Show(this, "All changes will be lost if you continue, do you wish to continue?", "Are you sure?",
						                MessageBoxButtons.YesNo, MessageBoxIcon.Question)) {
						                	case DialogResult.No:
						                		e.Cancel = true;
						                		break;
						                	default:
						                		break;
					}
				}
				else if (DialogResult == DialogResult.OK) {
					e.Cancel = false;
				}
				else {
					switch (e.CloseReason) {
						case CloseReason.UserClosing:
							e.Cancel = true;
							break;
						default:
							break;
					}
				}
			}
		}
	}
}