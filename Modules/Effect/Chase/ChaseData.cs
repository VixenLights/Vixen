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

namespace VixenModules.Effect.Chase
{
	[DataContract]
	public class ChaseData : ModuleDataModelBase
	{
		//[DataMember]
		//public Curve LevelCurve { get; set; }

		//[DataMember]
		//public ColorGradient ColorGradient { get; set; }

		//public ChaseData()
		//{
		//    LevelCurve = new Curve();
		//    ColorGradient = new ColorGradient();
		//}

		//public override IModuleDataModel Clone()
		//{
		//    ChaseData result = new ChaseData();
		//    result.LevelCurve = LevelCurve;
		//    result.ColorGradient = ColorGradient;
		//    return result;
		//}
	}
}
