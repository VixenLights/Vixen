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
		private ObservableList<IScheduleItem> _items;

		private enum DateView { Day, Week, Agenda };

		public SchedulerForm(SchedulerData data) {
			InitializeComponent();
			
			_data = data;
			_scheduleService = new ScheduleService();
			_items = new ObservableList<IScheduleItem>();

			//scheduleDay.Dock = DockStyle.Fill;
			//scheduleWeek.Dock = DockStyle.Fill;
			//scheduleAgenda.Dock = DockStyle.Fill;
		}

		private void SchedulerForm_Load(object sender, EventArgs e) {
			checkBoxEnableSchedule.Checked = _data.IsEnabled;

			scheduleDayView.Items = _items;
			//others here

			_items.AddRange(_data.Items);

			_SetCurrentView(DateView.Day);
		}

		private void SchedulerForm_FormClosing(object sender, FormClosingEventArgs e) {
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
					_items.Add(item);
					_data.Items.Add(item);
				}
			}
		}

		private void scheduleDayView_ItemDoubleClick(object sender, ScheduleItemArgs e) {
			using(ScheduleItemEditForm scheduleItemEditForm = new ScheduleItemEditForm(e.Item as ScheduleItem)) {
				if(scheduleItemEditForm.ShowDialog() == DialogResult.OK) {
					// Force a redraw of the item.
					_items.Replace(e.Item, e.Item);
				}
			}
		}

		private void checkBoxEnableSchedule_CheckedChanged(object sender, EventArgs e) {
			_data.IsEnabled = true;
		}

		private void SchedulerForm_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete && scheduleDayView.SelectedItem != null) {
				if(MessageBox.Show("Delete scheduled run of \"" + System.IO.Path.GetFileName(scheduleDayView.SelectedItem.FilePath) + "\"?", "Vixen Scheduler", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
					IScheduleItem item = scheduleDayView.SelectedItem;
					_items.Remove(item);
					_data.Items.Remove(item);
				}
			}
		}
	}
}
