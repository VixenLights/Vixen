using System;

namespace VixenModules.App.SimpleSchedule {
	interface IScheduledItem {
		Guid Id { get; }
		string ItemFilePath { get; }
		int DayOfWeek { get; }
		TimeSpan StartTime { get; }
		TimeSpan RunLength { get; }
	}
}
