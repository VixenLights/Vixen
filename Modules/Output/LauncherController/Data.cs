using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Output.LauncherController
{
	[DataContract]
	public class Data : ModuleDataModelBase
	{
		[DataMember]
		public bool HideLaunchedWindows { get; set; }

		public override IModuleDataModel Clone()
		{
			return MemberwiseClone() as IModuleDataModel;
		}
	}
}