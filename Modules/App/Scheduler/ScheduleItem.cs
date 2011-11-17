using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.App.Scheduler {
	public class ScheduleItem {
		public RecurrenceType RecurrenceType;
		/// <summary>
		/// i.e. Every x days, every x months
		/// </summary>
		public int UnitCount;
		/// <summary>
		/// i.e. 3rd Tuesday
		/// </summary>
		public int DayDateCount;
		/// <summary>
		/// Date of the month (1-31) or a day-of-week index (0-6).  Use -1 for the
		/// last day of the month.
		/// </summary>
		public int DayDate;
		/// <summary>
		/// Time of day when the scheduled item is valid.
		/// </summary>
		public TimeSpan RunTime;
		/// <summary>
		/// Date when the entry is first valid.
		/// </summary>
		public DateTime StartDate;
	}
}
