using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Services;
using System.Windows.Forms.Calendar;

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
			_program = new Program();
        }

        private void BuildScheduledItem()
        {

            if (_scheduledItem != null)
            {
                //since you already have a scheduled item and you can control where and when on the
                //calendar control
                //We only need to change the sequence or program here.
                _scheduledItem.ItemFilePath = _filePath;
            }
            else
            {
				if (_calendarItem.Duration.Ticks > 0)
				{
					_runLength = _calendarItem.Duration.Ticks;
				}
	
                //since this is new create the whole enchilada
                ScheduledItem item = new ScheduledItem(Guid.NewGuid(), _filePath, (int)_calendarItem.Date.DayOfWeek, _calendarItem.StartDate.TimeOfDay, new TimeSpan(_runLength)) { ScheduledItemStartDate = _calendarItem.StartDate };
				
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
				ISequence seq = _LoadSequence(_filePath);
				if(seq == null) return;

				//sequenceLabel.Text = GetName();
				sequenceLabel.Text = seq.Name;
				_runLength = seq.Length.Ticks;
            }
        }

		private ISequence _LoadSequence(string filePath) {
			Cursor = Cursors.WaitCursor;

			try {
				return SequenceService.Instance.Load(filePath);
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			} finally {
				Cursor = Cursors.Default;
			}

			return null;
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
            BuildScheduledItem();
        }

        public ScheduledItem _scheduledItem
        {
            get;
            set;
        }
    }
}
