using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.App.Scheduler
{
	internal partial class ScheduleDayView : UserControl, ISchedulerView
	{
		private DateTime _currentDate;

		public ScheduleDayView()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			CurrentDate = DateTime.Today;
		}

		public event EventHandler<ScheduleEventArgs> TimeDoubleClick
		{
			add { dayPanel.TimeDoubleClick += value; }
			remove { dayPanel.TimeDoubleClick -= value; }
		}

		public event EventHandler<ScheduleItemArgs> ItemDoubleClick
		{
			add { dayPanel.ItemDoubleClick += value; }
			remove { dayPanel.ItemDoubleClick -= value; }
		}

		public event EventHandler<ScheduleItemArgs> ItemClick
		{
			add { dayPanel.ItemClick += value; }
			remove { dayPanel.ItemClick -= value; }
		}

		public event EventHandler LeftButtonClick
		{
			add { headerPanel.LeftButtonClick += value; }
			remove { headerPanel.LeftButtonClick -= value; }
		}

		public event EventHandler RightButtonClick
		{
			add { headerPanel.RightButtonClick += value; }
			remove { headerPanel.RightButtonClick -= value; }
		}

		public IScheduleItem SelectedItem
		{
			get { return dayPanel.SelectedItem; }
		}

		public DateTime CurrentDate
		{
			get { return _currentDate; }
			set
			{
				_currentDate = value;
				headerPanel.Text = value.ToLongDateString();
				Refresh();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable<IScheduleItem> Items
		{
			get { return dayPanel.Items; }
			set { dayPanel.Items = value; }
		}
	}
}