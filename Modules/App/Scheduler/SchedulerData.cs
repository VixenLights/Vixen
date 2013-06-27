using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.App.Scheduler
{
	[DataContract]
	[KnownType(typeof (ScheduleItem))]
	public class SchedulerData : ModuleDataModelBase
	{
		private int _interval = 2;

		public SchedulerData()
		{
			Initialize();
		}

		[DataMember] public bool IsEnabled;

		[DataMember] public List<IScheduleItem> Items;

		[DataMember]
		public int CheckIntervalInSeconds
		{
			get { return _interval; }
			set
			{
				if (_interval != value && value != 0) {
					_interval = value;
				}
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			Initialize();
		}

		//[OnDeserialized]
		//private void OnDeserialized(StreamingContext context) {
		//}

		private void Initialize()
		{
			Items = new List<IScheduleItem>();
		}

		public override IModuleDataModel Clone()
		{
			return MemberwiseClone() as IModuleDataModel;
		}
	}
}