﻿using System.Runtime.Serialization;
using Vixen.Module;

namespace Launcher
{
	[DataContract]
	public class  Data : ModuleDataModelBase
	{
		public Data()
		{
			Description = String.Empty;
			Executable = String.Empty;
			Arguments = String.Empty;
		}

		[DataMember]
		public string Description { get; set; }


		[DataMember]
		public string Executable { get; set; }

		[DataMember]
		public string Arguments { get; set; }

		public override IModuleDataModel Clone()
		{
			return (IModuleDataModel)this.MemberwiseClone();
		}
	}
}
