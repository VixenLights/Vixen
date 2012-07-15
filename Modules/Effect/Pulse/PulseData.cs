using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace VixenModules.Effect.Pulse
{
	[DataContract]
	public class PulseData : ModuleDataModelBase
	{
		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public ColorGradient ColorGradient { get; set; }

		public PulseData()
		{
			LevelCurve = new Curve();
			ColorGradient = new ColorGradient();
		}

		public override IModuleDataModel Clone()
		{
			PulseData result = new PulseData();
			result.LevelCurve = LevelCurve;
			result.ColorGradient = ColorGradient;
			return result;
		}
	}
}
