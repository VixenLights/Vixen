using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Drawing;
using CommonElements.ColorManagement.ColorModels;
using Vixen.Module;

namespace VixenModules.Effect.SetLevel
{
	[DataContract]
	class SetLevelData : ModuleDataModelBase
	{
		[DataMember]
		public double level { get; set; }

		[DataMember]
		public RGB color { get; set; }

		public SetLevelData()
		{
			level = 1;
			color = Color.White;
		}

		public override IModuleDataModel Clone()
		{
			SetLevelData result = new SetLevelData();
			result.level = level;
			result.color = color;
			return result;
		}
	}
}
