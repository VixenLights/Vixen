using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Transform;
using System.Runtime.Serialization;

namespace VixenModules.Transform.DimmingCurve
{
	[DataContract]
	class DimmingCurveData : ModuleDataModelBase
	{
		public DimmingCurveData()
		{
			Curve = new CachingCurve();
		}

		[DataMember]
		public CachingCurve Curve { get; set; }

		public override IModuleDataModel Clone()
		{
			DimmingCurveData result = new DimmingCurveData();
			result.Curve = new CachingCurve(Curve);
			return result;
		}
	}
}
