using System;

namespace VixenModules.App.SimpleSchedule.Service {
	class ScheduledItemService {
		static public bool ScheduledItemQualifiesForExecution(IScheduledItemStateObject item) {
			return DateTime.Now >= item.Start && DateTime.Now < item.End;
		}

		static public DateTime CalculateConcreteStartDateTime(IScheduledItem item) {
			return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, (int)item.StartTime.TotalHours, item.StartTime.Minutes, item.StartTime.Seconds);
		}

		static public DateTime CalculateConcreteEndDateTime(IScheduledItem item) {
			return CalculateConcreteStartDateTime(item) + item.RunLength;
		}
	}
}
