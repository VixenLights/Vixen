using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Effect.ImageGrid
{
	[DataContract]
	public class Data : ModuleDataModelBase
	{
		[DataMember]
		public string FilePath { get; set; }

		public override IModuleDataModel Clone()
		{
			return (Data) MemberwiseClone();
		}
	}
}