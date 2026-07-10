using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Property.Grid
{
	[DataContract]
	public class Data : ModuleDataModelBase
	{
		[DataMember]
		public int Width { get; set; }

		[DataMember]
		public int Height { get; set; }

		public override IModuleDataModel Clone()
		{
			return (Data) MemberwiseClone();
		}
	}
}