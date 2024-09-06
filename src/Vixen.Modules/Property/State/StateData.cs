using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Property.State
{
	[DataContract]
	public class StateData : ModuleDataModelBase
	{
		public override IModuleDataModel Clone() {
			var data = new StateData
			{
				StateName = StateName,
				ItemName = ItemName,
				ItemColor = ItemColor
			};
			return data;
		}

		[DataMember]
		public string StateName { get; set; } = "State Name 1";

		[DataMember]
		public string ItemName { get; set; } = "Item Name 1";

		[DataMember]
		public System.Drawing.Color ItemColor { get; set; } = System.Drawing.Color.White;

		[OnDeserialized]
		void OnDeserialized(StreamingContext c)
		{
			
		}
	}
}
