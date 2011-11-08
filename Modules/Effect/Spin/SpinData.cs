using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CommonElements.ColorManagement.ColorModels;
using Vixen.Module;
using Vixen.Commands.KnownDataTypes;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace VixenModules.Effect.Spin
{
	[DataContract]
	public class SpinData : ModuleDataModelBase
	{
		//[DataMember]
		//public Curve LevelCurve { get; set; }

		//[DataMember]
		//public ColorGradient ColorGradient { get; set; }

		//public SpinData()
		//{
		//    LevelCurve = new Curve();
		//    ColorGradient = new ColorGradient();
		//}

		//public override IModuleDataModel Clone()
		//{
		//    SpinData result = new SpinData();
		//    result.LevelCurve = LevelCurve;
		//    result.ColorGradient = ColorGradient;
		//    return result;
		//}

		public SpinData()
		{
		}

		public override IModuleDataModel Clone()
		{
			throw new NotImplementedException();
		}
	}
}
