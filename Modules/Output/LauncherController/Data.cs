using System;
using System.IO.Ports;
using System.Runtime.Serialization;
using System.Xml.Serialization;
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
			return this.MemberwiseClone() as IModuleDataModel;
		}
	}
}