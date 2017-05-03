using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Factory;
using Vixen.Module;
using Vixen.Module.Preview;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace VixenApplication
{
	public partial class ConfigPreviews : BaseForm
	{
		private OutputPreview _displayedController;
		private bool _changesMade;

		public ConfigPreviews()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			this.ShowInTaskbar = false;
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
				Refresh();
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
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(message, title, false, true);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.OK)
				{
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
			Refresh();
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
			ColumnAutoSize();

			foreach (ListViewItem item in listViewControllers.Items) {
				if (item.Tag == _displayedController)
					item.Selected = true;
			}
		}

		public void ColumnAutoSize()
		{
			listViewControllers.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			ListView.ColumnHeaderCollection cc = listViewControllers.Columns;
			var width = (listViewControllers.Width - (int) (listViewControllers.Width*.06d))/listViewControllers.Columns.Count;
			for (int i = 0; i < cc.Count; i++)
			{
				cc[i].Width = width;
			}
		}

		private void _PopulateFormWithController(OutputPreview oc)
		{
			_displayedController = oc;

			if (oc == null) {
				textBoxName.Text = string.Empty;
				buttonDeleteController.Enabled = false;
				textBoxName.Enabled = false;
				buttonUpdate.Enabled = false;
				buttonUpdate.ForeColor = ThemeColorTable.ForeColorDisabled;
				buttonConfigureController.Enabled = false;
				buttonDeleteController.ForeColor = ThemeColorTable.ForeColorDisabled;
				buttonConfigureController.ForeColor = ThemeColorTable.ForeColorDisabled;
				label1.ForeColor = ThemeColorTable.ForeColorDisabled;
				label2.ForeColor = ThemeColorTable.ForeColorDisabled;
			}
			else {
				textBoxName.Text = oc.Name;
				buttonDeleteController.Enabled = true;
				textBoxName.Enabled = true;
				buttonUpdate.Enabled = true;
				buttonUpdate.ForeColor = ThemeColorTable.ForeColor;
				buttonConfigureController.Enabled = true;
				buttonDeleteController.ForeColor = ThemeColorTable.ForeColor;
				buttonConfigureController.ForeColor = ThemeColorTable.ForeColor;
				label1.ForeColor = ThemeColorTable.ForeColor;
				label2.ForeColor = ThemeColorTable.ForeColor;
			}
		}

		private void ConfigureSelectedController()
		{
			if (listViewControllers.SelectedItems.Count == 1)
			{
				var preview = listViewControllers.SelectedItems[0].Tag as OutputPreview;
				if (preview != null)
				{
					var running = preview.IsRunning;
					if (running)
					{
						preview.Stop();
					}
					preview.Setup();
					if (running)
					{
						preview.Start();
						Thread.Sleep(250);
						TopMost = true;
						Focus();
						BringToFront();
						TopMost = false;
					}
				}
				
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
					//A bit of a kludge, but need a bit of delay to give the preview a chance to load
					//before we force ourselves back on top.
					Thread.Sleep(10); 
					TopMost = true;
					TopMost = false;
				}	
			}
		}

		private void ConfigPreviews_FormClosing(object sender, FormClosingEventArgs e)
		{
			

			if (_changesMade) {
				if (DialogResult == DialogResult.Cancel) {
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("All changes will be lost if you continue, do you wish to continue?", "Are you sure?", true, false);
					messageBox.ShowDialog();
					switch (messageBox.DialogResult)
					{
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

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;

		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}