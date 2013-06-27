using System;
using System.Runtime.Serialization;

namespace VixenModules.App.SimpleSchedule
{
	[DataContract]
	public class ScheduledItem : IScheduledItem
	{
		public ScheduledItem(Guid id, string itemFilePath, int dayOfWeek, TimeSpan startTime, TimeSpan runLength)
		{
			Id = id;
			ItemFilePath = itemFilePath;
			DayOfWeek = dayOfWeek;
			StartTime = startTime;
			RunLength = runLength;
		}

		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public string ItemFilePath { get; set; }

		[DataMember]
		public int DayOfWeek { get; set; }

		[DataMember]
		public TimeSpan StartTime { get; set; }

		[DataMember]
		public TimeSpan RunLength { get; set; }

		[DataMember]
		public DateTime ScheduledItemStartDate { get; set; }
	}
}