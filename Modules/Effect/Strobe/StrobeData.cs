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
		public Curve OnTimeCurve { get; set; }

		[DataMember]
		public int CycleTime { get; set; }

		[DataMember]
		public Curve CycleVariationCurve { get; set; }
		
		[DataMember]
		public int GroupLevel { get; set; }

		[DataMember]
		public StrobeSource StrobeSource { get; set; }

		[DataMember]
		public StrobeMode StrobeMode { get; set; }

		[DataMember]
		public Guid MarkCollectionId { get; set; }

		public StrobeData()
		{
			Colors = new ColorGradient(Color.White);
			IntensityCurve = new Curve(new PointPairList(new[] { 0.0, 50.0, 100.0 }, new[] { 0.0, 100.0, 0.0 }));
			CycleTime = 150;
			CycleVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			OnTimeCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			StrobeSource = StrobeSource.TimeInterval;
			StrobeMode = StrobeMode.Simple;
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			var result = new StrobeData
			{
				Colors = Colors,
				IntensityCurve = IntensityCurve,
				OnTimeCurve = OnTimeCurve,
				CycleVariationCurve = CycleVariationCurve,
				CycleTime = CycleTime,
				GroupLevel = GroupLevel,
				StrobeSource = StrobeSource,
				MarkCollectionId = MarkCollectionId,
				StrobeMode = StrobeMode
			};
			return result;
		}
	}
}