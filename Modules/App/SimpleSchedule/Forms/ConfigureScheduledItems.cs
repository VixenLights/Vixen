using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Services;
using System.Windows.Forms.Calendar;
using System.Text.RegularExpressions;

namespace VixenModules.App.SimpleSchedule.Forms
{
    public partial class ConfigureScheduledItems : Form
    {
        Program _program;

        private string _filePath;
        private readonly CalendarItem _calendarItem;
		private long _runLength = 0;


        public ConfigureScheduledItems()
        {
            InitializeComponent();

        }
        public ConfigureScheduledItems(ScheduledItem item)
            : this()
        {
            _scheduledItem = item;
            _filePath = _scheduledItem.ItemFilePath;
			startTimePicker.Value = _scheduledItem.ScheduledItemStartDate;

            if (_scheduledItem.ItemFilePath.EndsWith(".pro"))
            {
                _program = ApplicationServices.LoadProgram(_scheduledItem.ItemFilePath);
                programLabel.Text = GetName();
            }
            else
            {
                sequenceLabel.Text = GetName();
                _program = new Program();
            }
        }

        public ConfigureScheduledItems(CalendarItem calendarItem)
            : this()
        {
            _calendarItem = calendarItem;
			startTimePicker.Value = _calendarItem.StartDate;
			_program = new Program();
        }

        private void BuildScheduledItem()
        {

            if (_scheduledItem != null)
            {
                //since you already have a scheduled item and you can control where and when on the calendar control
                //We only need to change the sequence or program here.
                _scheduledItem.ItemFilePath = _filePath;
            }
            else
            {
				TimeSpan t1 = GetRunLength();
				if (t1.Ticks == 0.0)
				{
					t1 = new TimeSpan(_runLength);
				}
					ScheduledItem item = new ScheduledItem(Guid.NewGuid(), _filePath, (int)_calendarItem.Date.DayOfWeek, _calendarItem.StartDate.TimeOfDay, t1) { ScheduledItemStartDate = _calendarItem.StartDate };
					_scheduledItem = item;
            }


        }
        private void selectProgramButton_Click(object sender, EventArgs e)
        {
            using (ConfigureProgram cp = new ConfigureProgram(_program))
            {
                if (cp.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _filePath = cp.ProgramName;
					_runLength = cp.ProgramDuration;
                    programLabel.Text = GetName();
                }

            }
        }

        private void selectSequenceButton_Click(object sender, EventArgs e)
        {
            openFileDialog.InitialDirectory = SequenceService.SequenceDirectory;
            openFileDialog.FileName = string.Empty;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _filePath = openFileDialog.FileName;
				ISequence seq = SequenceService.Instance.Load(_filePath);
				_runLength = seq.Length.Ticks;
				sequenceLabel.Text = seq.Name;
            }
			
        }

		private TimeSpan GetRunLength()
		{
			string[] hoursMins = runLengthTextBox.Text.Split(':');

			int hours = 0, minutes = 0;
			if(hoursMins.Length > 0) {
				int.TryParse(hoursMins[0], out hours);
			}
			if(hoursMins.Length > 1) {
				int.TryParse(hoursMins[1], out minutes);
			}

			return new TimeSpan(hours, minutes, 00);
		}

        private string GetName()
        {
            if (!String.IsNullOrEmpty(_filePath))
            {
                string name = _filePath.Substring(_filePath.LastIndexOf("\\") + 1);
                string finalname = name.Substring(0, name.LastIndexOf("."));
                return finalname;
            }
            else
            {
                return string.Empty;
            }
        }
        private void okButton_Click(object sender, EventArgs e)
        {
			if (!string.IsNullOrEmpty(runLengthTextBox.Text))
			{
				BuildScheduledItem();
			}
			else
			{
				MessageBox.Show("Please Enter Run Length", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
				DialogResult = DialogResult.None;
			}

        }

        public ScheduledItem _scheduledItem
        {
            get;
            set;
		}
    }
}
