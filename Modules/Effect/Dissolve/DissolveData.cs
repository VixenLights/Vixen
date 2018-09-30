using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Dissolve
{
	[DataContract]
	public class DissolveData : EffectTypeModuleData {

		[DataMember]
		public List<GradientLevelPair> Colors { get; set; }

		[DataMember]
		public DissolveMode DissolveMode { get; set; }

		[DataMember]
		public Curve DissolveCurve { get; set; }

		[DataMember]
		public Guid MarkCollectionId { get; set; }

		public DissolveData()
		{
			Colors = new List<GradientLevelPair> {new GradientLevelPair(Color.White, CurveType.Flat100)};
			DissolveMode = DissolveMode.TimeInterval;
			DissolveCurve = new Curve(new PointPairList(new[] { 100.0, 0.0 }, new[] { 0.0, 100.0 }));
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			var gradientLevelList = Colors.Select(glp => new GradientLevelPair(new ColorGradient(glp.ColorGradient), new Curve(glp.Curve))).ToList();
			var result = new DissolveData
            {
				Colors = gradientLevelList,
                DissolveMode = DissolveMode,
				MarkCollectionId = MarkCollectionId,
	            DissolveCurve = new Curve(DissolveCurve)
			};
			return result;
		}
	}
}