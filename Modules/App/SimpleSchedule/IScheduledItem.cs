using System;

namespace VixenModules.App.SimpleSchedule
{
	internal interface IScheduledItem
	{
		Guid Id { get; }
		string ItemFilePath { get; }
		int DayOfWeek { get; }
		TimeSpan StartTime { get; }
		TimeSpan RunLength { get; }
		DateTime ScheduledItemStartDate { get; }
	}
}