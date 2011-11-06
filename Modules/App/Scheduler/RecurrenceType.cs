using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler {
	/// <summary>
	/// How often a ScheduleEntry item will be valid.
	/// </summary>
	public enum RecurrenceType {
		[EnumDescription("Daily")]
		Daily,
		[EnumDescription("Weekly")]
		Weekly,
		[EnumDescription("Monthly - Date of Month")]
		MonthlyDateOfMonth,
		[EnumDescription("Monthly - Day of Week")]
		MonthlyDayOfWeek
	};
}
