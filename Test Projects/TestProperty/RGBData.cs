using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;

namespace TestProperty {
	[DataContract]
	public class RGBData : IModuleDataModel {
		[DataMember]
		public Guid RedChannelId { get; set; }

		[DataMember]
		public Guid GreenChannelId { get; set; }

		[DataMember]
		public Guid BlueChannelId { get; set; }

		public Guid ModuleTypeId { get; set; }

		public IModuleDataSet ModuleDataSet { get; set; }

		public Guid ModuleInstanceId { get; set; }

		public IModuleDataModel Clone() {
			RGBData instance = new RGBData();

			instance.RedChannelId = RedChannelId;
			instance.GreenChannelId = GreenChannelId;
			instance.BlueChannelId = BlueChannelId;
			
			return instance;
		}
	}
}
