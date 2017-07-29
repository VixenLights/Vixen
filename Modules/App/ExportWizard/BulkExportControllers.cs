using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Common.Controls.Wizard;
using Vixen.Export;

namespace VixenModules.App.ExportWizard
{
	public partial class BulkExportControllers : WizardStage
	{
		private BulkExportWizardData _data;
		public BulkExportControllers(BulkExportWizardData data)
		{
			_data = data;
			InitializeComponent();

			networkListView.DragDrop += networkListView_DragDrop;
			
			UpdateNetworkList();
		}

		private void UpdateNetworkList()
		{
			List<ControllerExportInfo> exportInfo = _data.Export.ControllerExportInfo;

			networkListView.Items.Clear();
			int startChan = 1;

			foreach (ControllerExportInfo info in exportInfo)
			{
				ListViewItem item = new ListViewItem(info.Name);
				item.Tag = info;
				item.SubItems.Add(info.Channels.ToString());
				item.SubItems.Add(string.Format("Channels {0} to {1}", startChan, startChan + info.Channels - 1));

				networkListView.Items.Add(item);

				startChan += info.Channels;
			}

			networkListView.ColumnAutoSize();
			networkListView.SetLastColumnWidth();
		}

		private void networkListView_DragDrop(object sender, DragEventArgs e)
		{

			int startChan = 1;
			int index = 0;
			foreach (ListViewItem item in networkListView.Items)
			{
				var info = item.Tag as ControllerExportInfo;
				if (info != null) info.Index = index;
				int channels = Convert.ToInt32(item.SubItems[1].Text);  //.Add(info.Channels.ToString());
				item.SubItems[2].Text = string.Format("Channels {0} to {1}", startChan, startChan + channels - 1);
				startChan += channels;
				index++;
			}

		}
	}
}
