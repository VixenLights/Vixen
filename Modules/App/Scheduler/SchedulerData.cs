using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;

namespace Scheduler {
	[DataContract]
	public class SchedulerData : ModuleDataModelBase {
		private int _interval = 10;

		public SchedulerData() {
			Initialize();
		}

		[DataMember]
		public bool IsEnabled;

		[DataMember]
		public List<ScheduleItem> Events;

		[DataMember]
		public int CheckIntervalInSeconds {
			get { return _interval; }
			set {
				if(_interval != value && value != 0) {
					_interval = value;
				}
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context) {
			Initialize();
		}

		private void Initialize() {
			Events = new List<ScheduleItem>();
		}

		public override IModuleDataModel Clone() {
			return MemberwiseClone() as IModuleDataModel;
		}
	}
}
