using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scheduler {
	partial class SchedulerForm : Form {
		private SchedulerData _data;

		public SchedulerForm(SchedulerData data) {
			InitializeComponent();
			_data = data;
		}
	}
}
