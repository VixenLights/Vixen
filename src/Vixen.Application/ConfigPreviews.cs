﻿using Common.Controls;
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
		private OutputPreview? _displayedController;
		private bool _changesMade;

		public ConfigPreviews()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			this.ShowInTaskbar = false;
			_displayedController = null;
			buttonDeleteController.Enabled = buttonDuplicateSelected.Enabled = false;
		}

		private void ConfigPreviews_Load(object sender, EventArgs e)
		{
			_PopulateControllerList();
			_PopulateFormWithController(null);
		}

		private void listViewControllers_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (listViewControllers.SelectedItems.Count > 1 || listViewControllers.SelectedItems.Count == 0)
			{
				_PopulateFormWithController(null);
			}
			else
			{
				_PopulateFormWithController(listViewControllers.SelectedItems[0].Tag as OutputPreview);
			}

			buttonDuplicateSelected.Enabled = buttonDeleteController.Enabled = listViewControllers.SelectedItems.Count > 0;
		}

		private void buttonAddController_Click(object sender, EventArgs e)
		{
			List<KeyValuePair<string, object>> outputModules = new List<KeyValuePair<string, object>>();
			var availableModules = ApplicationServices.GetAvailableModules<IPreviewModuleInstance>();
			foreach (KeyValuePair<Guid, string> kvp in availableModules)
			{
				outputModules.Add(new KeyValuePair<string, object>(kvp.Value, kvp.Key));
			}
			ListSelectDialog addForm = new ListSelectDialog("Add Preview", (outputModules));
			if (addForm.ShowDialog() == DialogResult.OK)
			{
				IModuleDescriptor moduleDescriptor = ApplicationServices.GetModuleDescriptor((Guid)addForm.SelectedItem);
				string name = moduleDescriptor.TypeName;
				PreviewFactory previewFactory = new PreviewFactory();
				OutputPreview preview = (OutputPreview)previewFactory.CreateDevice((Guid)addForm.SelectedItem, name);
				VixenSystem.Previews.Add(preview);
				VixenSystem.SystemConfig.Updated = true;

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
			if (listViewControllers.SelectedItems.Count > 1)
			{
				message = "Are you sure you want to delete the selected previews?";
				title = "Delete previews?";
			}
			else
			{
				message = "Are you sure you want to delete the selected preview?";
				title = "Delete preview?";
			}

			if (listViewControllers.SelectedItems.Count > 0)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(message, title, false, true);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.OK)
				{
					foreach (ListViewItem item in listViewControllers.SelectedItems)
					{
						OutputPreview oc = item.Tag as OutputPreview ?? throw new InvalidOperationException();
						XMLProfileSettings xml = new XMLProfileSettings();
						var name = $"Preview_{oc.ModuleInstanceId}";
						xml.DeleteNode(XMLProfileSettings.SettingType.AppSettings, name);
						VixenSystem.Previews.Remove(oc);
					}
					_PopulateControllerList();
					_changesMade = true;
					VixenSystem.SystemConfig.Updated = true;
				}
			}
		}

		private void buttonDuplicateSelected_Click(object sender, EventArgs e)
		{
			if (listViewControllers.SelectedItems.Count > 0)
			{
				foreach (ListViewItem item in listViewControllers.SelectedItems)
				{
					OutputPreview op = item.Tag as OutputPreview ?? throw new InvalidOperationException();

					PreviewFactory previewFactory = new PreviewFactory();
					OutputPreview preview = (OutputPreview)previewFactory.CreateDevice(op.ModuleId, op.Name + "-copy");
					if (preview.PreviewModule is IPreviewModuleInstance newInstance)
					{
						if (op.PreviewModule is IPreviewModuleInstance origInstance)
						{
							var md = origInstance.ModuleData.Clone();
							//The new module will have it's own instance data. If we want to replace it we need to replace it in the
							//ModuleStore as well so it will be saved. So remove it and then assign it and then update it in the store
							md.ModuleDataSet.RemoveModuleInstanceData(newInstance);
							newInstance.ModuleData = md;
							md.ModuleDataSet.AssignModuleInstanceData(newInstance);
						}

					}
					VixenSystem.Previews.Add(preview);
					_PopulateFormWithController(preview);

				}

				_PopulateControllerList();

				_changesMade = true;
				VixenSystem.SystemConfig.Updated = true;
				Refresh();
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

			foreach (OutputPreview oc in VixenSystem.Previews)
			{
				ListViewItem item = new ListViewItem();
				item.Text = oc.Name;
				item.Checked = oc.IsRunning;
				item.SubItems.Add(ApplicationServices.GetModuleDescriptor(oc.ModuleId).TypeName);
				item.Tag = oc;
				listViewControllers.Items.Add(item);
			}

			listViewControllers.EndUpdate();
			ColumnAutoSize();

			foreach (ListViewItem item in listViewControllers.Items)
			{
				if (item.Tag == _displayedController)
					item.Selected = true;
			}
		}

		public void ColumnAutoSize()
		{
			listViewControllers.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			ListView.ColumnHeaderCollection cc = listViewControllers.Columns;
			var width = (listViewControllers.Width - (int)(listViewControllers.Width * .06d)) / listViewControllers.Columns.Count;
			for (int i = 0; i < cc.Count; i++)
			{
				cc[i].Width = width;
			}
		}

		private void _PopulateFormWithController(OutputPreview? oc)
		{
			_displayedController = oc;

			if (oc == null)
			{
				textBoxName.Text = string.Empty;
				textBoxName.Enabled = false;
				buttonUpdate.Enabled = false;
				buttonConfigureController.Enabled = label1.Enabled = label2.Enabled = false;
			}
			else
			{
				textBoxName.Text = oc.Name;
				textBoxName.Enabled = true;
				buttonUpdate.Enabled = true;
				buttonConfigureController.Enabled = label1.Enabled = label2.Enabled = true;
			}
		}

		private async void ConfigureSelectedController()
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

					// Mark that some data within the Preview has changed.
					preview.ContentChanged = true;

					if (running)
					{
						preview.Start();
						await Task.Delay(250);
						TopMost = true;
						Focus();
						BringToFront();
						TopMost = false;
					}
				}

			}
		}

		private async void listViewControllers_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			OutputPreview? preview = listViewControllers.Items[e.Index].Tag as OutputPreview;
			if (preview == null)
			{
				return;
			}
			if (e.NewValue == CheckState.Unchecked)
			{
				if (preview.IsRunning)
				{
					VixenSystem.Previews.Stop(preview);
				}
			}
			else if (e.NewValue == CheckState.Checked)
			{
				if (!preview.IsRunning)
				{
					VixenSystem.Previews.Start(preview);
					//A bit of a kludge, but need a bit of delay to give the preview a chance to load
					//before we force ourselves back on top.
					await Task.Delay(250);
					TopMost = true;
					TopMost = false;
				}
			}
		}

		private void ConfigPreviews_FormClosing(object sender, FormClosingEventArgs e)
		{


			if (_changesMade)
			{
				if (DialogResult == DialogResult.Cancel)
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("All changes will be lost if you continue, do you wish to continue?", "Are you sure?", true, false);
					messageBox.ShowDialog();
					switch (messageBox.DialogResult)
					{
						case DialogResult.No:
							e.Cancel = true;
							break;
					}
				}

				// The User has accepted the changes, so we'll need to do some post-processing on the resultant files.
				else if (DialogResult == DialogResult.OK)
				{
					e.Cancel = false;

					// First, let's check that there are no duplicates
					foreach (var preview in VixenSystem.Previews)
					{
						// Check for duplicate Preview names
						if (VixenSystem.Previews.Count(x => x.Name == preview.Name) > 1)
						{
							var messageBox = new MessageBoxForm($"Preview names must be unique.\n\nAdjust the name \"{preview.Name}\" accordingly in order to continue.", "Duplicate Preview Name", MessageBoxButtons.OK, SystemIcons.Warning);
							messageBox.ShowDialog();
							e.Cancel = true;
							return;
						}
					}

					Vixen.Sys.SystemConfig prevSystemConfig = Vixen.Services.FileService.Instance.LoadSystemConfigFile(Path.Combine(VixenSystem.GetSystemDataPath(), Vixen.Sys.SystemConfig.FileName));

					foreach (var preview in prevSystemConfig.Previews)
					{
						// Check if we deleted any Previews
						if (VixenSystem.Previews.Count(x => x.Id == preview.Id) == 0)
						{
							VixenSystem.DeleteModuleStorePreviewFile(preview.Name);
							VixenSystem.SystemConfig.Updated = true;
						}

						// Check if we renamed any Previews
						else if (VixenSystem.Previews.Count(x => x.Name == preview.Name) == 0)
						{
							VixenSystem.RenameModuleStorePreviewFile(preview.Name, VixenSystem.Previews.First(x => x.Id == preview.Id).Name);
							VixenSystem.SystemConfig.Updated = true;
						}
					}
				}
				else
				{
					switch (e.CloseReason)
					{
						case CloseReason.UserClosing:
							e.Cancel = true;
							break;
					}
				}
			}
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}

	}
}