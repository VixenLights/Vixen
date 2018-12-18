using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Strobe
{
	[DataContract] 
	public class StrobeData : EffectTypeModuleData {

		[DataMember]
		public ColorGradient Colors { get; set; }

		[DataMember]
		public Curve IntensityCurve { get; set; }

		[DataMember]
		public Curve CycleRatioCurve { get; set; }

		[DataMember]
		public int Interval { get; set; }

		[DataMember]
		public Curve IntervalCurve { get; set; }

		[DataMember]
		public int GroupLevel { get; set; }

		public StrobeData()
		{
			Colors = new ColorGradient(Color.White);
			IntensityCurve = new Curve(new PointPairList(new[] { 0.0, 50.0, 100.0 }, new[] { 0.0, 100.0, 0.0 }));
			Interval = 1000;
			IntervalCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			CycleRatioCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			var result = new StrobeData
			{
				Colors = Colors,
				IntensityCurve = IntensityCurve,
				CycleRatioCurve = CycleRatioCurve,
				Interval = Interval,
				GroupLevel = GroupLevel
			};
			return result;
		}
	}
}