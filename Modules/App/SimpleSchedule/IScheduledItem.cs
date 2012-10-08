using System;

namespace VixenModules.App.SimpleSchedule {
	interface IScheduledItem {
		string ItemFilePath { get; }
		int DayOfWeek { get; }
		TimeSpan StartTime { get; }
		TimeSpan RunLength { get; }
	}
}
