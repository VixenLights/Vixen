using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.App.Scheduler {
	interface ISchedulerView {
		IEnumerable<IScheduleItem> Items { get; set; }
	}
}
