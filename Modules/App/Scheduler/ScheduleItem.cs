using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace VixenModules.App.Scheduler
{
	[DataContract]
	internal class ScheduleItem : IScheduleItem
	{
		//private RecurrenceType _recurrenceType;

		public ScheduleItem()
		{
			StartDate = EndDate = DateTime.Today;
		}

		[DataMember]
		public RecurrenceType RecurrenceType { get; set; }

		/// <summary>
		/// Ex: Every x days, every x months
		/// </summary>
		[DataMember]
		public int DateUnitCount { get; set; }

		/// <summary>
		/// Ex: 3rd Tuesday
		/// </summary>
		[DataMember]
		public int DayCount { get; set; }

		/// <summary>
		/// Date of the month (1-31) or a day-of-week index (0-6).  Use -1 for the
		/// last day of the month.
		/// </summary>
		[DataMember]
		public int DayDate { get; set; }

		/// <summary>
		/// Time of day when the scheduled item is valid.
		/// </summary>
		[DataMember]
		public TimeSpan RunStartTime { get; set; }

		/// <summary>
		/// Time of day when the scheduled item stop being valid.
		/// </summary>
		[DataMember]
		public virtual TimeSpan RunEndTime { get; set; }

		/// <summary>
		/// First date when the entry is valid.
		/// </summary>
		[DataMember]
		public DateTime StartDate { get; set; }

		/// <summary>
		/// Last date when the entry is valid.
		/// </summary>
		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public int RepeatIntervalMinutes { get; set; }

		[DataMember]
		public string FilePath { get; set; }

		public bool RepeatsOnInterval
		{
			get { return RepeatIntervalMinutes != 0; }
			set { if (!value) RepeatIntervalMinutes = 0; }
		}

		public bool RepeatsWithinBlock
		{
			get { return RunEndTime != RunStartTime; }
			set { if (!value) RunEndTime = RunStartTime; }
		}

		public bool IsExecuting;

		public DateTime LastExecutedAt;

		//[OnDeserializing]
		//public void Deserializing(StreamingContext context) {
		//}
		//[OnDeserialized]
		//public void Deserialized(StreamingContext context) {
		//}
	}
}