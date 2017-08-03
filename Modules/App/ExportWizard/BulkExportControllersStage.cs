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

			networkListView.DragDrop += networkListView_DragDrop;
			
			UpdateNetworkList();
		}

		private void UpdateNetworkList()
		{
			networkListView.Items.Clear();
			int startChan = 1;

			foreach (ControllerExportInfo info in _data.ControllerInfo.OrderBy(x => x.Index))
			{
				ListViewItem item = new ListViewItem(info.Name);
				item.Tag = info;
				item.SubItems.Add(info.Channels.ToString());
				item.SubItems.Add(startChan.ToString());
				item.SubItems.Add((startChan + info.Channels - 1).ToString());

				networkListView.Items.Add(item);

				startChan += info.Channels;
			}

			networkListView.ColumnAutoSize();
			networkListView.SetLastColumnWidth();

			_WizardStageChanged();
		}

		private void networkListView_DragDrop(object sender, DragEventArgs e)
		{
			ReIndexControllerChannels();
		}

		private void networkListView_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && networkListView.SelectedItems.Count > 0)
			{
				foreach (ListViewItem item in networkListView.SelectedItems)
				{
					_data.ControllerInfo.Remove(item.Tag as ControllerExportInfo);
					item.Remove();
				}
			}

			ReIndexControllerChannels();
		}

		public override bool CanMoveNext
		{
			get { return _data.ControllerInfo.Count > 0; }
		}

		private void ReIndexControllerChannels()
		{
			int startChan = 1;
			int index = 0;
			foreach (ListViewItem item in networkListView.Items)
			{
				var info = item.Tag as ControllerExportInfo;
				if (info != null) { info.Index = index;}
				int channels = Convert.ToInt32(item.SubItems[1].Text); //.Add(info.Channels.ToString());
				item.SubItems[2].Text = startChan.ToString();
				item.SubItems[3].Text = (startChan + info.Channels - 1).ToString();
				startChan += channels;
				index++;
			}

			_WizardStageChanged();
		}

		private void btnReset_Click(object sender, EventArgs e)
		{
			_data.InitializeControllerInfo();
			UpdateNetworkList();
		}
	}
}
