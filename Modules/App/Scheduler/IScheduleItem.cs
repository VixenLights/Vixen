using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.App.Scheduler {
	public interface IScheduleItem {
		RecurrenceType RecurrenceType { get; set; }
		int DateUnitCount { get; set; }
		int DayCount { get; set; }
		int DayDate { get; set; }
		TimeSpan RunStartTime { get; set; }
		TimeSpan RunEndTime { get; set; }
		DateTime StartDate { get; set; }
		DateTime EndDate { get; set; }
		int RepeatIntervalMinutes { get; set; }
		bool RepeatsOnInterval { get; set; }
		bool RepeatsWithinBlock { get; set; }
	}
}
