using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.Curves;

namespace VixenModules.Intent.Pulse
{
	[DataContract]
	public class PulseData : ModuleDataModelBase
	{
		[DataMember]
		public Curve LevelCurve { get; set; }

		public PulseData()
		{
			LevelCurve = new Curve();
		}

		public override IModuleDataModel Clone()
		{
			PulseData result = new PulseData();
			result.LevelCurve = LevelCurve;
			return result;
		}
	}
}
