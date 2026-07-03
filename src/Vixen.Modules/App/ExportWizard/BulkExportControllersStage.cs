using Common.Controls.Theme;
using Common.Controls.Wizard;
using Vixen.Export;

namespace VixenModules.App.ExportWizard
{
	public partial class BulkExportControllersStage : WizardStage
	{
		private readonly BulkExportWizardData _data;
		private bool _reindexPending;

		public BulkExportControllersStage(BulkExportWizardData data)
		{
			_data = data;
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
		}

		protected override void OnLoad(EventArgs e)
		{
			networkListView.DragDrop += networkListView_DragDrop;
			networkListView.ItemChecked += NetworkListView_ItemChecked;
			networkListView.KeyDown += NetworkListView_KeyDown;
			btnEnableAll.Click += BtnEnableAll_Click;
			btnDisableAll.Click += BtnDisableAll_Click;
		}

		public override void StageStart()
		{
			_data.ActiveProfile.SyncronizeControllerInfo();
			UpdateNetworkList();
		}

		private void UpdateNetworkList()
		{

			networkListView.BeginUpdate();
			networkListView.Items.Clear();
			int startChan = 1;

			foreach (Controller info in _data.ActiveProfile.Controllers.OrderBy(x => x.Index))
			{
				ListViewItem item = new ListViewItem(info.Name);
				item.Tag = info;
				item.SubItems.Add(info.Channels.ToString());

				if (info.IsActive)
				{
					item.Checked = info.IsActive;
					item.SubItems.Add(startChan.ToString());
					item.SubItems.Add((startChan + info.Channels - 1).ToString());
					startChan += info.Channels;
				}
				else
				{
					item.SubItems.Add(String.Empty);
					item.SubItems.Add(String.Empty);
				}
				
				networkListView.Items.Add(item);
			}

			networkListView.ColumnAutoSize();
			networkListView.SetLastColumnWidth();

			networkListView.EndUpdate();

			_WizardStageChanged();
		}

		private void networkListView_DragDrop(object sender, DragEventArgs e)
		{
			ReIndexControllerChannels();
		}

		private void NetworkListView_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			// ListView's native checkbox handling toggles every highlighted row in one gesture
			// (Space bar, or a mouse click on any highlighted row's checkbox), firing this event
			// once per row synchronously before returning to the message loop. Coalesce those
			// bursts into a single reindex pass instead of reindexing once per row.
			if (_reindexPending)
			{
				return;
			}

			_reindexPending = true;
			BeginInvoke(new Action(() =>
			{
				_reindexPending = false;
				ReIndexControllerChannels();
			}));
		}

		private void NetworkListView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.A && e.Control)
			{
				networkListView.BeginUpdate();
				foreach (ListViewItem item in networkListView.Items)
				{
					item.Selected = true;
				}
				networkListView.EndUpdate();
				e.SuppressKeyPress = true;
			}
		}

		// WinForms checks ProcessCmdKey, bubbling from the focused control up through every parent
		// container, before it ever considers Escape a "dialog key" eligible to trigger the host
		// Form's CancelButton (WizardForm.Designer.cs wires buttonCancel as the CancelButton).
		// Intercepting Escape here — one level above networkListView, since BulkExportControllersStage
		// is itself a ContainerControl — consumes it before it can reach that CancelButton logic at
		// all, whenever the list has focus and something is highlighted. When nothing is highlighted,
		// this returns false and Escape falls through to cancel the wizard exactly as it always has.
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape && networkListView.Focused && networkListView.SelectedItems.Count > 0)
			{
				networkListView.SelectedItems.Clear();
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void BtnEnableAll_Click(object sender, EventArgs e)
		{
			SetAllChecked(true);
		}

		private void BtnDisableAll_Click(object sender, EventArgs e)
		{
			SetAllChecked(false);
		}

		private void SetAllChecked(bool isChecked)
		{
			networkListView.BeginUpdate();
			foreach (ListViewItem item in networkListView.Items)
			{
				item.Checked = isChecked;
			}
			networkListView.EndUpdate();
		}

		public override bool CanMoveNext
		{
			get { return _data.ActiveProfile.Controllers.Count > 0; }
		}

		private void ReIndexControllerChannels()
		{
			networkListView.BeginUpdate();
			int startChan = 1;
			int index = 0;
			foreach (ListViewItem item in networkListView.Items)
			{
				var info = item.Tag as Controller;
				if (info == null)
				{
					continue; // This should not happen!
				}
				info.Index = index;
				info.IsActive = item.Checked;

				if (info.IsActive)
				{
					int channels = Convert.ToInt32(item.SubItems[1].Text);
					item.SubItems[2].Text = startChan.ToString();
					item.SubItems[3].Text = (startChan + info.Channels - 1).ToString();
					startChan += channels;
				}
				else
				{
					item.SubItems[2].Text = String.Empty;
					item.SubItems[3].Text = String.Empty;
				}

				index++;
			}
			networkListView.EndUpdate();

			_WizardStageChanged();
		}
	}
}
