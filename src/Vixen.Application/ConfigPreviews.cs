using Common.Controls;
using Common.Controls.Theme;
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
		private OutputPreview? _displayedPreview;
		private bool _changesMade;

		public ConfigPreviews()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			this.ShowInTaskbar = false;
			_displayedPreview = null;
			buttonDeletePreview.Enabled = buttonDuplicateSelected.Enabled = false;
		}

		private void ConfigPreviews_Load(object sender, EventArgs e)
		{
			_PopulatePreviewList();
			_PopulateFormWithPreview(null);
		}

		private void listViewPreviews_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (listViewPreviews.SelectedItems.Count > 1 || listViewPreviews.SelectedItems.Count == 0)
			{
				_PopulateFormWithPreview(null);
			}
			else
			{
				_PopulateFormWithPreview(listViewPreviews.SelectedItems[0].Tag as OutputPreview);
			}

			buttonDuplicateSelected.Enabled = buttonDeletePreview.Enabled = listViewPreviews.SelectedItems.Count > 0;
		}

		private void buttonAddPreview_Click(object sender, EventArgs e)
		{
			List<KeyValuePair<string, object>> outputModules = new List<KeyValuePair<string, object>>();
			var availableModules = ApplicationServices.GetAvailableModules<IPreviewModuleInstance>();
			var previewToAddKey = availableModules.Any()?availableModules.First().Key:Guid.Empty;
			
			if (outputModules.Count > 1)
			{
				foreach (KeyValuePair<Guid, string> kvp in availableModules)
				{
					outputModules.Add(new KeyValuePair<string, object>(kvp.Value, kvp.Key));
				}
				ListSelectDialog addForm = new ListSelectDialog("Add Preview", outputModules);
				if (addForm.ShowDialog() != DialogResult.OK)
				{
					return;
				}

				previewToAddKey = (Guid)addForm.SelectedItem;
			}
			
			IModuleDescriptor moduleDescriptor = ApplicationServices.GetModuleDescriptor(previewToAddKey);
			string name = moduleDescriptor.TypeName;
			PreviewFactory previewFactory = new PreviewFactory();
			OutputPreview preview = (OutputPreview)previewFactory.CreateDevice(previewToAddKey, name);
			VixenSystem.Previews.Add(preview);
			// In the case of a preview that has a form, the form will not be shown
			// until this event handler completes.  To make sure it's in a visible state
			// before evaluating if it's running or not, we're calling DoEvents.
			// I hate DoEvents calls, so if you know of a better way...
			Application.DoEvents();

			// Select the new preview, and then repopulate the list -- it will make sure the currently
			// displayed preview is selected.
			_PopulateFormWithPreview(preview);
			_PopulatePreviewList();
			ConfigureSelectedPreview();

			_changesMade = true;
			Refresh();
			
		}

		private void buttonDeletePreview_Click(object sender, EventArgs e)
		{
			string message, title;
			if (listViewPreviews.SelectedItems.Count > 1)
			{
				message = "Are you sure you want to delete the selected previews?";
				title = "Delete previews?";
			}
			else
			{
				message = "Are you sure you want to delete the selected preview?";
				title = "Delete preview?";
			}

			if (listViewPreviews.SelectedItems.Count > 0)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(message, title, false, true);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.OK)
				{
					foreach (ListViewItem item in listViewPreviews.SelectedItems)
					{
						OutputPreview oc = item.Tag as OutputPreview ?? throw new InvalidOperationException();
						XMLProfileSettings xml = new XMLProfileSettings();
						var name = $"Preview_{oc.ModuleInstanceId}";
						xml.DeleteNode(XMLProfileSettings.SettingType.AppSettings, name);
						VixenSystem.Previews.Remove(oc);
					}
					_PopulatePreviewList();
					_changesMade = true;
				}
			}
		}

		private void buttonDuplicateSelected_Click(object sender, EventArgs e)
		{
			if (listViewPreviews.SelectedItems.Count > 0)
			{
				foreach (ListViewItem item in listViewPreviews.SelectedItems)
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
					_PopulateFormWithPreview(preview);

				}

				_PopulatePreviewList();

				_changesMade = true;
				Refresh();
			}

		}

		private void buttonUpdate_Click(object sender, EventArgs e)
		{
			if (_displayedPreview == null)
				return;

			_displayedPreview.Name = textBoxName.Text;

			_PopulatePreviewList();

			_changesMade = true;
		}

		private void buttonConfigurePreview_Click(object sender, EventArgs e)
		{
			ConfigureSelectedPreview();
			_changesMade = true;
			Refresh();
		}

		private void _PopulatePreviewList()
		{
			listViewPreviews.BeginUpdate();
			listViewPreviews.Items.Clear();

			foreach (OutputPreview oc in VixenSystem.Previews)
			{
				ListViewItem item = new ListViewItem();
				item.Text = oc.Name;
				item.Checked = oc.IsRunning;
				item.SubItems.Add(ApplicationServices.GetModuleDescriptor(oc.ModuleId).TypeName);
				item.Tag = oc;
				listViewPreviews.Items.Add(item);
			}

			listViewPreviews.EndUpdate();
			ColumnAutoSize();

			foreach (ListViewItem item in listViewPreviews.Items)
			{
				if (item.Tag == _displayedPreview)
					item.Selected = true;
			}
		}

		public void ColumnAutoSize()
		{
			listViewPreviews.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			ListView.ColumnHeaderCollection cc = listViewPreviews.Columns;
			var width = (listViewPreviews.Width - (int)(listViewPreviews.Width * .06d)) / listViewPreviews.Columns.Count;
			for (int i = 0; i < cc.Count; i++)
			{
				cc[i].Width = width;
			}
		}

		private void _PopulateFormWithPreview(OutputPreview? oc)
		{
			_displayedPreview = oc;

			if (oc == null)
			{
				textBoxName.Text = string.Empty;
				textBoxName.Enabled = false;
				buttonUpdate.Enabled = false;
				buttonConfigurePreview.Enabled = label1.Enabled = label2.Enabled = false;
			}
			else
			{
				textBoxName.Text = oc.Name;
				textBoxName.Enabled = true;
				buttonUpdate.Enabled = true;
				buttonConfigurePreview.Enabled = label1.Enabled = label2.Enabled = true;
			}
		}

		private async void ConfigureSelectedPreview()
		{
			if (listViewPreviews.SelectedItems.Count == 1)
			{
				var preview = listViewPreviews.SelectedItems[0].Tag as OutputPreview;
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
						await Task.Delay(250);
						TopMost = true;
						Focus();
						BringToFront();
						TopMost = false;
					}
				}

			}
		}

		private async void listViewPreviews_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			OutputPreview? preview = listViewPreviews.Items[e.Index].Tag as OutputPreview;
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
				else if (DialogResult == DialogResult.OK)
				{
					e.Cancel = false;
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
