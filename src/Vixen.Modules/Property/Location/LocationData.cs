using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Property.Location {
	[DataContract]
	public class LocationData : ModuleDataModelBase {
		public override IModuleDataModel Clone() {
			return (LocationData)MemberwiseClone();
		}

		[DataMember]
		public int X { get; set; }

		[DataMember]
		public int Y { get; set; }

		[DataMember]
		public int Z { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			if (X < 0)
			{
				X = 0;
			}

			if (Y < 0)
			{
				Y = 0;
			}

			if (Z < 0)
			{
				Z = 0;
			}
		}
	}
}
