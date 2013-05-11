using System;
using VixenModules.App.SimpleSchedule.StateObject;

namespace VixenModules.App.SimpleSchedule.Service {
	class ScheduledItemService {
		static private ScheduledItemService _instance;

		private ScheduledItemService() {
		}

		public static ScheduledItemService Instance {
			get { return _instance ?? (_instance = new ScheduledItemService()); }
		}

		public IScheduledItem CreateScheduledItem(IScheduledItem scheduledItem) {
			return new ScheduledItem(scheduledItem.Id, scheduledItem.ItemFilePath, scheduledItem.DayOfWeek, scheduledItem.StartTime, scheduledItem.RunLength);
		}

		public IScheduledItem CreateScheduledItem(string itemFilePath, int dayOfWeek, TimeSpan startTime, TimeSpan runLength) {
			return new ScheduledItem(Guid.NewGuid(), itemFilePath, dayOfWeek, startTime, runLength);
		}

		public IScheduledItemStateObject CreateSequenceStateObject(IScheduledItem item) {
			return new ScheduledSequence(item);
		}

		public IScheduledItemStateObject CreateProgramStateObject(IScheduledItem item) {
			return new ScheduledProgram(item);
		}

		public bool ScheduledItemQualifiesForExecution(IScheduledItemStateObject item) {
			return DateTime.Now >= item.Start && DateTime.Now < item.End;
		}

		public DateTime CalculateConcreteStartDateTime(IScheduledItem item) {
			return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, (int)item.StartTime.TotalHours, item.StartTime.Minutes, item.StartTime.Seconds);
		}

		public DateTime CalculateConcreteEndDateTime(IScheduledItem item) {
			return CalculateConcreteStartDateTime(item) + item.RunLength;
		}
	}
}
