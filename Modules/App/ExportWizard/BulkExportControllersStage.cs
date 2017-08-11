using System;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Controls.Wizard;
using Vixen.Export;

namespace VixenModules.App.ExportWizard
{
	public partial class BulkExportControllersStage : WizardStage
	{
		private readonly BulkExportWizardData _data;

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
			ReIndexControllerChannels();
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
