using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.App.SimpleSchedule
{
	[DataContract]
	public class SimpleSchedulerData : ModuleDataModelBase
	{
		public SimpleSchedulerData()
		{
			IsEnabled = false;
			CheckIntervalInSeconds = 10;
			ScheduledItems = new List<ScheduledItem>();
		}

		[DataMember]
		public bool IsEnabled { get; set; }

		[DataMember]
		public int CheckIntervalInSeconds { get; set; }

		[DataMember]
		public List<ScheduledItem> ScheduledItems { get; set; }

		public override IModuleDataModel Clone()
		{
			SimpleSchedulerData newData = (SimpleSchedulerData) MemberwiseClone();
			newData.ScheduledItems = ScheduledItems.ToList();
			return newData;
		}
	}
}