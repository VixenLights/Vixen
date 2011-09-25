using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CommandStandard.Types;
using CommonElements.ColorManagement.ColorModels;
using Vixen.Module;

namespace VixenModules.Effect.SetLevel
{
	[DataContract]
	class SetLevelData : ModuleDataModelBase
	{
		[DataMember]
		public Level level { get; set; }

		[DataMember]
		public RGB color { get; set; }

		public override IModuleDataModel Clone()
		{
			SetLevelData result = new SetLevelData();
			result.level = level;
			result.color = color;
			return result;
		}
	}
}
