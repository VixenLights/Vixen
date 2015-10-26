using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;

namespace VixenModules.App.Scheduler
{
	internal partial class SchedulerForm : BaseForm
	{
		private SchedulerData _data;
		private ScheduleService _scheduleService;
		private DateView _currentView;

		private enum DateView
		{
			Day,
			Week,
			Agenda
		};

		public SchedulerForm(SchedulerData data)
		{
			InitializeComponent();

			_data = data;
			_scheduleService = new ScheduleService();
		}

		private void SchedulerForm_Load(object sender, EventArgs e)
		{
			checkBoxEnableSchedule.Checked = _data.IsEnabled;
			_SetCurrentView(DateView.Day, DateTime.Today);
		}

		private void SchedulerForm_FormClosing(object sender, FormClosingEventArgs e)
		{
		}

		private void toolStripButtonToday_Click(object sender, EventArgs e)
		{
			_SetCurrentView(DateView.Day, DateTime.Today);
		}

		private void toolStripButtonDayView_Click(object sender, EventArgs e)
		{
			_SetCurrentView(DateView.Day);
		}

		private void toolStripButtonWeekView_Click(object sender, EventArgs e)
		{
			_SetCurrentView(DateView.Week);
		}

		private void toolStripButtonAgendaView_Click(object sender, EventArgs e)
		{
			_SetCurrentView(DateView.Agenda);
		}

		private void _SetCurrentView(DateView view)
		{
			_SetCurrentView(view, _CurrentDate);
		}

		private void _SetCurrentView(DateView view, DateTime date)
		{
			// Reset button for the previous view
			switch (_currentView) {
				case DateView.Day:
					//scheduleDay.Visible = false;
					toolStripButtonDayView.Checked = false;
					break;
				case DateView.Week:
					//scheduleWeek.Visible = false;
					toolStripButtonWeekView.Checked = false;
					break;
				case DateView.Agenda:
					//scheduleAgenda.Visible = false;
					toolStripButtonAgendaView.Checked = false;
					break;
			}

			_CurrentDate = date;
			_currentView = view;

			// Draw the new view
			switch (_currentView) {
				case DateView.Day:
					//CompileApplicableTimers(date, m_oneDayTimeSpan);
					//scheduleDay.Visible = true;
					toolStripButtonDayView.Checked = true;
					break;
				case DateView.Week:
					//CompileApplicableTimers(_currentDate.AddDays(-((int)_currentDate.DayOfWeek)), m_oneWeekTimeSpan);
					//scheduleWeek.Visible = true;
					toolStripButtonWeekView.Checked = true;
					break;
				case DateView.Agenda:
					//CompileApplicableTimers(date, m_oneDayTimeSpan);
					//scheduleAgenda.Visible = true;
					toolStripButtonAgendaView.Checked = true;
					break;
			}
		}

		private DateTime _currentDate;

		private DateTime _CurrentDate
		{
			get { return _currentDate; }
			set
			{
				if (_currentDate != value) {
					_currentDate = value;
					_RefreshView();
				}
			}
		}

		private void _RefreshView()
		{
			switch (_currentView) {
				case DateView.Day:
					scheduleDayView.Items = _scheduleService.GetItems(_data.Items, _currentDate, _currentDate + TimeSpan.FromDays(1));
					scheduleDayView.CurrentDate = _currentDate;
					break;
				case DateView.Week:
					break;
				case DateView.Agenda:
					break;
			}
		}

		private void scheduleDayView_TimeDoubleClick(object sender, ScheduleEventArgs e)
		{
			ScheduleItem item = new ScheduleItem
			                    	{
			                    		RunStartTime = e.TimeOffset,
			                    		RepeatsWithinBlock = false,
			                    		RepeatsOnInterval = false
			                    	};
			using (ScheduleItemEditForm scheduleItemEditForm = new ScheduleItemEditForm(item)) {
				if (scheduleItemEditForm.ShowDialog() == DialogResult.OK) {
					_data.Items.Add(item);
					_RefreshView();
				}
			}
		}

		private void scheduleDayView_ItemDoubleClick(object sender, ScheduleItemArgs e)
		{
			using (ScheduleItemEditForm scheduleItemEditForm = new ScheduleItemEditForm(e.Item as ScheduleItem)) {
				if (scheduleItemEditForm.ShowDialog() == DialogResult.OK) {
					// Force a redraw of the item.
					_RefreshView();
				}
			}
		}

		private void checkBoxEnableSchedule_CheckedChanged(object sender, EventArgs e)
		{
			_data.IsEnabled = true;
		}

		private void SchedulerForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && scheduleDayView.SelectedItem != null) {
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Delete scheduled run of \"" + System.IO.Path.GetFileName(scheduleDayView.SelectedItem.FilePath) + "\"?",
						"Vixen Scheduler", true, false);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.OK)
				{
					IScheduleItem item = scheduleDayView.SelectedItem;
					_data.Items.Remove(item);
					_RefreshView();
				}
			}
		}

		private void scheduleDayView_LeftButtonClick(object sender, EventArgs e)
		{
			_CurrentDate += TimeSpan.FromDays(-1);
		}

		private void scheduleDayView_RightButtonClick(object sender, EventArgs e)
		{
			_CurrentDate += TimeSpan.FromDays(1);
		}
	}
}