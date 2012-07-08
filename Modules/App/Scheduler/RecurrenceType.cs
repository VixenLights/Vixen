using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.App.Scheduler {
	/// <summary>
	/// How often a ScheduleItem will be valid.
	/// </summary>
	public enum RecurrenceType {
		[EnumDescription("None")]
		None,
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
