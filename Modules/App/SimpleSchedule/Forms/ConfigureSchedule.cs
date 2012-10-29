using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Calendar;

namespace VixenModules.App.SimpleSchedule.Forms
{
    public partial class ConfigureSchedule : Form
    {
        private SimpleSchedulerData _data;
        private List<CalendarItem> _calendarItems;
        CalendarItem contextItem;


        public ConfigureSchedule(SimpleSchedulerData data)
        {
            InitializeComponent();

            monthView1.MonthTitleColor = monthView1.MonthTitleColorInactive = CalendarColorTable.FromHex("#C2DAFC");
            monthView1.ArrowsColor = CalendarColorTable.FromHex("#77A1D3");
            monthView1.DaySelectedBackgroundColor = CalendarColorTable.FromHex("#F4CC52");
            monthView1.DaySelectedTextColor = monthView1.ForeColor;


            _data = data;
        }

        private void PlaceItems()
        {
            calendar1.Items.Clear();
            _calendarItems = new List<CalendarItem>();
            foreach (ScheduledItem item in _data.ScheduledItems)
            {

                string name = item.ItemFilePath.Substring(item.ItemFilePath.LastIndexOf("\\") + 1);
                string finalname = name.Substring(0, name.LastIndexOf("."));
                CalendarItem ci = new CalendarItem(calendar1, item.ScheduledItemStartDate, item.RunLength, finalname) { Tag = item.Id };
                _calendarItems.Add(ci);
                calendar1.Items.Add(ci);
            }
        }

        private void SetScrollValues()
        {
            try
            {
                int increments = (60 / Convert.ToInt32(calendar1.TimeScale));
                int TotalSlots = increments * 24;
                int visible = calendar1.GetVisibleTimeUnits();
                vScrollBar1.Maximum = TotalSlots - 1;
                vScrollBar1.SmallChange = increments;
                vScrollBar1.LargeChange = visible;
            }
            catch (Exception ex)
            {
                // ignore exceptions
            }
        }


        private void monthView1_SelectionChanged(object sender, EventArgs e)
        {
            calendar1.SetViewRange(monthView1.SelectionStart, monthView1.SelectionEnd);
        }

        private void ConfigureSchedule_Load(object sender, EventArgs e)
        {
            enableScheduleCheckBox.Checked = _data.IsEnabled;
            PlaceItems();
        }

        private void disableScheduleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (enableScheduleCheckBox.Checked)
            {
                _data.IsEnabled = true;
            }
            else
                _data.IsEnabled = false;
        }

         private void calendar1_LoadItems_1(object sender, CalendarLoadEventArgs e)
        {
            if (_data.ScheduledItems.Count > 0)
            {
                PlaceItems();
            }

            SetScrollValues();
        }

        private void calendar1_ItemDatesChanged(object sender, ref CalendarItemEventArgs e)
        {
            CalendarItem ci = e.Item;
            //we have moved or modified this item so lets update the configuration information.
            ScheduledItem item = _data.ScheduledItems.Find(x => x.Id == (Guid)ci.Tag);
            item.RunLength = ci.Duration;
            item.ScheduledItemStartDate = ci.StartDate;
            item.StartTime = ci.StartDate.TimeOfDay;

            //now update the item in the _data.scheduleditems list
            _data.ScheduledItems.RemoveAll(x => x.Id == (Guid)ci.Tag);

            //now add the new item back to the _data.scheduledItems
            _data.ScheduledItems.Add(item);
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            contextItem = calendar1.ItemAt(contextMenuStrip1.Bounds.Location);
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CalendarItem item = ReturnCalendarItem();

            if (item != null)
            {
                MessageBox.Show("Item already exists.\r\n" + "If you wish to change date/time just drag and drop where you want the item to be.",
                    "Item Exists", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                item = new CalendarItem(calendar1, calendar1.SelectedElementStart.Date, calendar1.SelectedElementEnd.Date, "New Item to Add");
                using (ConfigureScheduledItems csi = new ConfigureScheduledItems(item))
                {
                    if (csi.ShowDialog() == DialogResult.OK)
                    {
                        _data.ScheduledItems.Add(csi._scheduledItem);
                        PlaceItems();
                    }
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            CalendarItem item = ReturnCalendarItem();
            DialogResult result;
            if (item != null)
            {
               result =  MessageBox.Show("Are you sure you want to delete item!!!", "Delete Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
               if (result == System.Windows.Forms.DialogResult.Yes)
               {
                   _data.ScheduledItems.RemoveAll(x => x.Id == (Guid)item.Tag);
                   PlaceItems();
               }
            }
        }

        private void changeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CalendarItem item = ReturnCalendarItem();
            ScheduledItem schitem = _data.ScheduledItems.Find(x => x.Id == (Guid)item.Tag);

            using (ConfigureScheduledItems csi = new ConfigureScheduledItems(schitem))
            {
                if (csi.ShowDialog() == DialogResult.OK)
                {
                    _data.ScheduledItems.Add(csi._scheduledItem);
                    PlaceItems();
                }
            }

        }
        private void calendar1_TimeUnitsOffsetChanged(object sender, EventArgs e)
        {
            vScrollBar1.Value = Math.Abs(calendar1.TimeUnitsOffset);
        }

        private void ConfigureSchedule_Resize(object sender, EventArgs e)
        {
            int increments = (60 / Convert.ToInt32(calendar1.TimeScale));
            int TotalSlots = increments * 24;
            int visible = calendar1.GetVisibleTimeUnits();
            SetScrollValues();
            if (Math.Abs(calendar1.TimeUnitsOffset) + visible > TotalSlots)
            {
                //resize caused the bottom display to exceed the last slot so we need to adjust the top position
                calendar1.TimeUnitsOffset = -(TotalSlots - visible - 1);
                vScrollBar1.Value = Math.Abs(calendar1.TimeUnitsOffset);
            }

        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            calendar1.TimeUnitsOffset = (-e.NewValue);
        }

        private CalendarItem ReturnCalendarItem()
        {
            IEnumerable<CalendarItem> items = calendar1.GetSelectedItems();
            if (items.Count() > 0)
            {
                CalendarItem item = items.ElementAt(0);
                return item;
            }
            else
            {
                return null;
            }
        }
    }
}
