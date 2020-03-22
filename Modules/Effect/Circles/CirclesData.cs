using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Circles
{
	[DataContract]
	public class CirclesData : EffectTypeModuleData
	{

		public CirclesData()
		{
			Colors = new List<ColorGradient> { new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue) };
			CenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 8.0, 8.0 }));
			Inverse = false;
			CircleFill = CircleFill.Fade;
			CircleRadialDirection = CircleRadialDirection.Out;
			SizeCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			CircleCountCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 70.0, 70.0 }));
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			CircleEdgeWidthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 1.0, 1.0 }));
			XOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			Orientation = StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public CircleRadialDirection CircleRadialDirection { get; set; }

		[DataMember]
		public CircleFill CircleFill { get; set; }

		[DataMember]
		public bool Inverse { get; set; }

		[DataMember]
		public Curve CenterSpeedCurve { get; set; }

		[DataMember]
		public Curve CircleEdgeWidthCurve { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public Curve SizeCurve { get; set; }

		[DataMember]
		public Curve CircleCountCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public Curve XOffsetCurve { get; set; }

		[DataMember]
		public Curve YOffsetCurve { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			CirclesData result = new CirclesData
			{
				Colors = Colors.ToList(),
				CenterSpeedCurve = new Curve(CenterSpeedCurve),
				Orientation = Orientation,
				Inverse = Inverse,
				CircleFill = CircleFill,
				CircleEdgeWidthCurve = new Curve(CircleEdgeWidthCurve),
				CircleRadialDirection = CircleRadialDirection,
				SizeCurve = new Curve(SizeCurve),
				CircleCountCurve = new Curve(CircleCountCurve),
				YOffsetCurve = new Curve(YOffsetCurve),
				XOffsetCurve = new Curve(XOffsetCurve),
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
