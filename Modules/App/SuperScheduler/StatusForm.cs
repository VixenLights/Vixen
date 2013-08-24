using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.App.SuperScheduler
{
	public partial class StatusForm : Form
	{
		private SuperSchedulerData SchedulerData;
		private ScheduleExecutor Executor;

		public StatusForm(SuperSchedulerData data, ScheduleExecutor executor)
		{
			InitializeComponent();
			SchedulerData = data;
			Executor = executor;
		}

		private void StatusForm_Load(object sender, EventArgs e)
		{
			Location = SchedulerData.StatusForm_Position;
			PopulateShowList();
		}

		private void PopulateShowList()
		{
			foreach (Shows.Show show in Shows.ShowsData.ShowList) 
			//foreach (ScheduleItem scheduleItem in SchedulerData.Items)
			{
				bool foundIt = false;
				foreach (Common.Controls.ComboBoxItem oldItem in comboBoxShows.Items)
				{
					Shows.Show comboBoxScheduleItem = oldItem.Value as Shows.Show;
					if (comboBoxScheduleItem.ID == show.ID)
					{
						oldItem.Text = show.Name;
						foundIt = true;
						break;
					}
				}

				if (!foundIt)
				{
					Common.Controls.ComboBoxItem newItem = new Common.Controls.ComboBoxItem(show.Name, show);
					comboBoxShows.Items.Add(newItem);
				}
			}
		}

		public string Status
		{
			set
			{ 
				labelStatus.Text = value; 
			}
		}

		private void StatusForm_LocationChanged(object sender, EventArgs e)
		{
			SchedulerData.StatusForm_Position = Location;
		}

		private void StatusForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = (e.CloseReason == CloseReason.UserClosing);
		}

		int originalHeight = 0;
		private void buttonViewLog_Click(object sender, EventArgs e)
		{
			if (Height == 420)
			{
				Height = originalHeight;
			}
			else
			{
				originalHeight = Height;
				Height = 420;
			}
		}

		private delegate void AddLogEntryDelegate(string logEntry);
		public void AddLogEntry(string logEntry)
		{
			if (!IsDisposed)
			{
				if (this.InvokeRequired)
					this.Invoke(new AddLogEntryDelegate(AddLogEntry), logEntry);
				else
				{
					listBoxLog.Items.Insert(0, logEntry);
				}
			}
		}

		private void buttonStopNow_Click(object sender, EventArgs e)
		{
			Executor.Stop(false);
		}

		private void comboBoxShows_DropDown(object sender, EventArgs e)
		{
			PopulateShowList();
		}

		private void buttonStopGracefully_Click(object sender, EventArgs e)
		{
			Executor.Stop(true);
		}

		private void buttonPlayShowNow_Click(object sender, EventArgs e)
		{
			Common.Controls.ComboBoxItem item = comboBoxShows.SelectedItem as Common.Controls.ComboBoxItem;
			if (item != null)
			{
				Shows.Show show = item.Value as Shows.Show;
				Executor.Stop(false);

				ScheduleItem scheduleItem = new ScheduleItem();
				scheduleItem.ShowID = show.ID;
				scheduleItem.Start(true);
			}
		}

	}
}
