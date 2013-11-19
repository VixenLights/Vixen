using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Resources;
using Common.Resources.Properties;

namespace VixenModules.App.SuperScheduler
{
	public partial class SetupScheduleForm : Form
	{
		ScheduleItem _scheduleItem;

		public SetupScheduleForm(ScheduleItem scheduleItem)
		{
			InitializeComponent();

			buttonHelp.Image = Tools.GetIcon(Resources.help, 16);
			
			_scheduleItem = scheduleItem;
		}

		public DateTime ValidDate(DateTime date) 
		{
			if (date < new DateTime(2000, 1, 1))
			{
				return DateTime.Now;
			}
			return date;
		}

		private void buttonHelp_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Scheduler_Main);
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (dateStart.Value > dateStop.Value)
			{
				MessageBox.Show("The end date of a show must fall after the start date.", "Date Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
				return;
			}
			_scheduleItem.StartDate = dateStart.Value;
			_scheduleItem.EndDate = dateStop.Value;
			_scheduleItem.Sunday = checkSunday.Checked;
			_scheduleItem.Monday = checkMonday.Checked;
			_scheduleItem.Tuesday = checkTuesday.Checked;
			_scheduleItem.Wednesday = checkWednesday.Checked;
			_scheduleItem.Thursday = checkThursday.Checked;
			_scheduleItem.Friday = checkFriday.Checked;
			_scheduleItem.Saturday = checkSaturday.Checked;
			_scheduleItem.StartTime = dateStartTime.Value;
			_scheduleItem.EndTime = dateEndTime.Value;
			_scheduleItem.Enabled = checkEnabled.Checked;

			if (comboBoxShow.SelectedIndex >= 0)
			{
				Shows.Show show = ((comboBoxShow.SelectedItem as Common.Controls.ComboBoxItem).Value) as Shows.Show;
				_scheduleItem.ShowID = show.ID;
			}
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void dateStart_ValueChanged(object sender, EventArgs e)
		{
			//_scheduleItem.StartDate = dateStart.Value;
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		private void SetupScheduleForm_Load(object sender, EventArgs e)
		{
			dateStart.Value = ValidDate(_scheduleItem.StartDate);
			dateStop.Value = ValidDate(_scheduleItem.EndDate);
			checkMonday.Checked = _scheduleItem.Monday;
			checkTuesday.Checked = _scheduleItem.Tuesday;
			checkWednesday.Checked = _scheduleItem.Wednesday;
			checkThursday.Checked = _scheduleItem.Thursday;
			checkFriday.Checked = _scheduleItem.Friday;
			checkSaturday.Checked = _scheduleItem.Saturday;
			checkSunday.Checked = _scheduleItem.Sunday;
			dateStartTime.Value = ValidDate(_scheduleItem.StartTime);
			dateEndTime.Value = ValidDate(_scheduleItem.EndTime);
			checkEnabled.Checked = _scheduleItem.Enabled;

			PopulateShowList(_scheduleItem.ShowID);
		}

		public void PopulateShowList(Guid showID)
		{
			foreach (Shows.Show show in Shows.ShowsData.ShowList)
			{
				Common.Controls.ComboBoxItem item = new Common.Controls.ComboBoxItem(show.Name, show);
				comboBoxShow.Items.Add(item);
				if (show.ID == showID)
				{
					comboBoxShow.SelectedItem = item;
				}
			}
		}

		private void dateStartTime_ValueChanged(object sender, EventArgs e)
		{
			SetTime();
		}

		private void dateEndTime_ValueChanged(object sender, EventArgs e)
		{
			SetTime();
		}

		public void SetTime()
		{
			TimeSpan t = TimeSpan.Zero;

			if (dateEndTime.Value < dateStartTime.Value)
			{
				DateTime newEndDate = new DateTime(dateEndTime.Value.Ticks);
				newEndDate = newEndDate.AddDays(1);
				t = newEndDate - dateStartTime.Value;
			}
			else
			{
				t = dateEndTime.Value - dateStartTime.Value;
			}

			labelDuration.Text = "(the show will last " + t.Hours + " hours and " + t.Minutes + " minutes)";
		}

		private void comboBoxShow_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

	}
}
