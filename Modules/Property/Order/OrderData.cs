using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Property.Order {
	[DataContract]
	public class OrderData : ModuleDataModelBase {
		public override IModuleDataModel Clone() {
			return (OrderData)MemberwiseClone();
		}

		[DataMember]
		public int Order { get; set; }
	}
}
