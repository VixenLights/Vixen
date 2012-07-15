using System;
using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Input.DirectXJoystick {
	[DataContract]
	public class DirectXJoystickData : ModuleDataModelBase {
		public override IModuleDataModel Clone() {
			return MemberwiseClone() as IModuleDataModel;
		}

		[DataMember]
		public Guid DeviceId { get; set; }
	}
}
