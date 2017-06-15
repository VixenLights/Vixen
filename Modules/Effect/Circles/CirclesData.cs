using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using NLog.LayoutRenderers.Wrappers;
using Vixen.Module;
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
			SpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 60.0, 60.0 }));
			CenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 40.0, 40.0 }));
			Filled = false;
			Wrap = false;
			Inverse = false;
			RandomRadius = false;
			Collide = false;
			CircleType = CircleType.Circles;
			CircleFill = CircleFill.Solid;
			BackgroundColor = new ColorGradient(Color.Snow);
			Fade = true;
			CircleRadialDirection = CircleRadialDirection.Out;
			RadiusCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			CircleCountCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 7.0, 7.0 }));
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			BallEdgeWidthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 1.0, 1.0 }));
			Orientation = StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public Curve SpeedVariationCurve { get; set; }

		[DataMember]
		public CircleRadialDirection CircleRadialDirection { get; set; }

		[DataMember]
		public CircleFill CircleFill { get; set; }

		[DataMember]
		public CircleType CircleType { get; set; }

		[DataMember]
		public bool Inverse { get; set; }

		[DataMember]
		public bool Collide { get; set; }

		[DataMember]
		public bool RandomRadius { get; set; }

		[DataMember]
		public ColorGradient BackgroundColor { get; set; }

		[DataMember]
		public Curve CenterSpeedCurve { get; set; }

		[DataMember]
		public Curve BallEdgeWidthCurve { get; set; }

		[DataMember]
		public bool Filled { get; set; }

		[DataMember]
		public bool Fade { get; set; }

		[DataMember]
		public bool Wrap { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public Curve RadiusCurve { get; set; }

		[DataMember]
		public Curve CircleCountCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			CirclesData result = new CirclesData
			{
				Colors = Colors.ToList(),
				SpeedVariationCurve = new Curve(SpeedVariationCurve),
				CenterSpeedCurve = new Curve(CenterSpeedCurve),
				Orientation = Orientation,
				Inverse = Inverse,
				Filled = Filled,
				Wrap = Wrap,
				Collide = Collide,
				CircleFill = CircleFill,
				Fade = Fade,
				BallEdgeWidthCurve = new Curve(BallEdgeWidthCurve),
				CircleRadialDirection = CircleRadialDirection,
				CircleType = CircleType,
				RandomRadius = RandomRadius,
				RadiusCurve = new Curve(RadiusCurve),
				CircleCountCurve = new Curve(CircleCountCurve),
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
