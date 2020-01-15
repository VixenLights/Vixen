using System;
using System.Collections.Generic;
using System.Text;
using Vixen.Module;
using System.Runtime.Serialization;

namespace VixenModules.Output.HelixController
{
	[DataContract]
	public class HelixData : ModuleDataModelBase
	{
		override public IModuleDataModel Clone()
		{
			return MemberwiseClone() as IModuleDataModel;
		}
	}
}
