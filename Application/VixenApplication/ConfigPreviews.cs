using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Resources.Properties;
using Vixen.Factory;
using Vixen.Module;
using Vixen.Module.Preview;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace VixenApplication
{
	public partial class ConfigPreviews : Form
	{
		private OutputPreview _displayedController;
		private bool _changesMade;

		public ConfigPreviews()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			_displayedController = null;
		}

		private void ConfigPreviews_Load(object sender, EventArgs e)
		{
			_PopulateControllerList();
			_PopulateFormWithController(null);
		}

		private void listViewControllers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewControllers.SelectedItems.Count > 1 || listViewControllers.SelectedItems.Count == 0) {
				_PopulateFormWithController(null);
			}
			else {
				_PopulateFormWithController(listViewControllers.SelectedItems[0].Tag as OutputPreview);
			}
		}

		private void buttonAddController_Click(object sender, EventArgs e)
		{
			List<KeyValuePair<string, object>> outputModules = new List<KeyValuePair<string, object>>();
			var availableModules = ApplicationServices.GetAvailableModules<IPreviewModuleInstance>();
			foreach (KeyValuePair<Guid, string> kvp in availableModules) {
				outputModules.Add(new KeyValuePair<string, object>(kvp.Value, kvp.Key));
			}
			Common.Controls.ListSelectDialog addForm = new Common.Controls.ListSelectDialog("Add Preview", (outputModules));
			if (addForm.ShowDialog() == DialogResult.OK) {
				IModuleDescriptor moduleDescriptor = ApplicationServices.GetModuleDescriptor((Guid) addForm.SelectedItem);
				string name = moduleDescriptor.TypeName;
				PreviewFactory previewFactory = new PreviewFactory();
				OutputPreview preview = (OutputPreview) previewFactory.CreateDevice((Guid) addForm.SelectedItem, name);
				VixenSystem.Previews.Add(preview);
				// In the case of a controller that has a form, the form will not be shown
				// until this event handler completes.  To make sure it's in a visible state
				// before evaluating if it's running or not, we're calling DoEvents.
				// I hate DoEvents calls, so if you know of a better way...
				Application.DoEvents();

				// select the new controller, and then repopulate the list -- it will make sure the currently
				// displayed controller is selected.
				_PopulateFormWithController(preview);
				_PopulateControllerList();

				_changesMade = true;
			}
		}

		private void buttonDeleteController_Click(object sender, EventArgs e)
		{
			string message, title;
			if (listViewControllers.SelectedItems.Count > 1) {
				message = "Are you sure you want to delete the selected previews?";
				title = "Delete previews?";
			}
			else {
				message = "Are you sure you want to delete the selected preview?";
				title = "Delete preview?";
			}

			if (listViewControllers.SelectedItems.Count > 0) {
				if (MessageBox.Show(message, title, MessageBoxButtons.OKCancel) == DialogResult.OK) {
					foreach (ListViewItem item in listViewControllers.SelectedItems) {
						OutputPreview oc = item.Tag as OutputPreview;
						VixenSystem.Previews.Remove(oc);
					}
					_PopulateControllerList();
					_changesMade = true;
				}
			}
		}

		private void buttonUpdate_Click(object sender, EventArgs e)
		{
			if (_displayedController == null)
				return;

			_displayedController.Name = textBoxName.Text;

			_PopulateControllerList();

			_changesMade = true;
		}

		private void buttonConfigureController_Click(object sender, EventArgs e)
		{
			ConfigureSelectedController();
			_changesMade = true;
		}

		private void _PopulateControllerList()
		{
			listViewControllers.BeginUpdate();
			listViewControllers.Items.Clear();

			foreach (OutputPreview oc in VixenSystem.Previews) {
				ListViewItem item = new ListViewItem();
				item.Text = oc.Name;
				item.Checked = oc.IsRunning;
				item.SubItems.Add(ApplicationServices.GetModuleDescriptor(oc.ModuleId).TypeName);
				item.Tag = oc;
				listViewControllers.Items.Add(item);
			}

			listViewControllers.EndUpdate();

			foreach (ListViewItem item in listViewControllers.Items) {
				if (item.Tag == _displayedController)
					item.Selected = true;
			}
		}

		private void _PopulateFormWithController(OutputPreview oc)
		{
			_displayedController = oc;

			if (oc == null) {
				textBoxName.Text = string.Empty;
				buttonDeleteController.Enabled = false;
				groupBoxSelectedController.Enabled = false;
			}
			else {
				textBoxName.Text = oc.Name;
				buttonDeleteController.Enabled = true;
				groupBoxSelectedController.Enabled = true;
			}
		}

		private void ConfigureSelectedController()
		{
			if (listViewControllers.SelectedItems.Count == 1) {
				(listViewControllers.SelectedItems[0].Tag as OutputPreview).Setup();
			}
		}

		private void listViewControllers_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			OutputPreview preview = (listViewControllers.Items[e.Index].Tag as OutputPreview);
			if (e.NewValue == CheckState.Unchecked) {
				if (preview != null && preview.IsRunning) {
					VixenSystem.Previews.Stop(preview);
				}
			}
			else if (e.NewValue == CheckState.Checked) {
				if (preview != null && !preview.IsRunning) {
					VixenSystem.Previews.Start(preview);
				}
			}
		}

		private void ConfigPreviews_FormClosing(object sender, FormClosingEventArgs e)
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
					}
				}
			}
		}
	}
}