using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.App.Scheduler {
	partial class ScheduleDayView : UserControl, ISchedulerView {
		public event EventHandler<ScheduleEventArgs> TimeDoubleClick {
			add { dayPanel.TimeDoubleClick += value; }
			remove { dayPanel.TimeDoubleClick -= value; }
		}

		public ScheduleDayView() {
			InitializeComponent();

			dayPanel.BackColorChanged += (sender, e) => BackColor = dayPanel.BackColor;
		}

		public IList<IScheduleItem> Items {
			get { return dayPanel.Items; }
			set { dayPanel.Items = value; }
		}

	}
}
