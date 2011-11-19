using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.App.Scheduler {
	partial class SchedulerForm : Form {
		private SchedulerData _data;
		private ScheduleService _scheduleService;
		private DateView _currentView;
		private DateTime _currentDate;

		private enum DateView { Day, Week, Agenda };

		public SchedulerForm(SchedulerData data) {
			InitializeComponent();
			
			_data = data;
			_scheduleService = new ScheduleService();
			
			checkBoxEnableSchedule.Checked = data.IsEnabled;

			//scheduleDay.Dock = DockStyle.Fill;
			//scheduleWeek.Dock = DockStyle.Fill;
			//scheduleAgenda.Dock = DockStyle.Fill;
		}

		private void SchedulerForm_Load(object sender, EventArgs e) {
			_SetCurrentView(DateView.Day);
		}

		private void toolStripButtonToday_Click(object sender, EventArgs e) {
			_SetCurrentView(DateView.Day, DateTime.Today);
		}

		private void toolStripButtonDayView_Click(object sender, EventArgs e) {
			_SetCurrentView(DateView.Day);
		}

		private void toolStripButtonWeekView_Click(object sender, EventArgs e) {
			_SetCurrentView(DateView.Week);
		}

		private void toolStripButtonAgendaView_Click(object sender, EventArgs e) {
			_SetCurrentView(DateView.Agenda);
		}

		private void _SetCurrentView(DateView view) {
			_SetCurrentView(view, _currentDate);
		}

		private void _SetCurrentView(DateView view, DateTime date) {
			// Reset button for the previous view
			switch(_currentView) {
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

			_currentDate = date;
			_currentView = view;

			// Draw the new view
			switch(_currentView) {
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

		private void scheduleDayView_TimeDoubleClick(object sender, ScheduleEventArgs e) {
			ScheduleItem item = new ScheduleItem {
				RunStartTime = e.TimeOffset,
				RepeatsWithinBlock = false,
				RepeatsOnInterval = false
			};
			using(ScheduleItemEditForm scheduleItemEditForm = new ScheduleItemEditForm(item)) {
				if(scheduleItemEditForm.ShowDialog() == DialogResult.OK) {
					scheduleDayView.Items.Add(item);
				}
			}
		}
	}
}
