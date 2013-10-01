using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using System.Drawing;

namespace VixenModules.App.SuperScheduler
{
	[DataContract]
	public class SuperSchedulerData : ModuleDataModelBase
	{
		private List<ScheduleItem> _items = null;

		public SuperSchedulerData()
		{
			IsEnabled = false;
		}

		[DataMember]
		public bool IsEnabled { get; set; }

		private int _checkIntervalInSeconds = 1;
		[DataMember]
		public int CheckIntervalInSeconds 
		{
			get
			{
				if (_checkIntervalInSeconds < 1)
					_checkIntervalInSeconds = 1;
				return _checkIntervalInSeconds;
			}
			set
			{
				_checkIntervalInSeconds = value;
			}
		}

		[DataMember]
		public Point StatusForm_Position;

		[DataMember]
		public List<ScheduleItem> Items
		{
			get
			{
				if (_items == null)
					_items = new List<ScheduleItem>();
				return _items;
			}
			set
			{
				_items = value;
			}
		}

		public override IModuleDataModel Clone()
		{
			SuperSchedulerData newData = (SuperSchedulerData)MemberwiseClone();
			//newData.ScheduledItems = ScheduledItems.ToList();
			return newData;
		}
	}
}