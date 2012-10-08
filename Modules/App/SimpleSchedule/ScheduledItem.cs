using System;
using System.Runtime.Serialization;

namespace VixenModules.App.SimpleSchedule {
	[DataContract]
	public class ScheduledItem : IScheduledItem {
		public ScheduledItem(string itemFilePath, int dayOfWeek, TimeSpan startTime, TimeSpan runLength) {
			ItemFilePath = itemFilePath;
			DayOfWeek = dayOfWeek;
			StartTime = startTime;
			RunLength = runLength;
		}

		[DataMember]
		public string ItemFilePath { get; set; }

		[DataMember]
		public int DayOfWeek { get; set; }

		[DataMember]
		public TimeSpan StartTime { get; set; }

		[DataMember]
		public TimeSpan RunLength { get; set; }
	}
}
