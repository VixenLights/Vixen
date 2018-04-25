using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Shapes
{
	[DataContract]
	[KnownType(typeof(SerializableFont))]
	public class ShapesData : EffectTypeModuleData
	{
		public ShapesData()
		{
			Colors = new List<ColorGradient> { new ColorGradient(Color.Red) };
			AngleSpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 5.0, 5.0 }));
			CenterAngleSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 20.0, 20.0 }));
			SizeSpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 5.0, 5.0 }));
			CenterSizeSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 20.0, 20.0 }));
			SpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 5.0, 5.0 }));
			CenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 20.0, 20.0 }));
			ManualPosition = true;
			ShapeType = ShapeType.Bounce;
			SizeCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 20.0, 20.0 }));
			MaxSizeCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 20.0, 20.0 }));
			ShapeList = ShapeList.ChristmasShapes;
			GeometricShapes = GeometricShapes.A;
			ChristmasShapes = ChristmasShapes.A;
			FontShapes = FontShapes.A;
			XOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			AngleCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 100.0 }));
			GradientMode = GradientMode.AcrossElement;
			Orientation = StringOrientation.Vertical;
			Font = new SerializableFont(new Font("Times new roman", 10));
			LevelCurve = new Curve(CurveType.Flat100);
			RandomShapeSize = false;
			ShapeCountCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public Curve AngleSpeedVariationCurve { get; set; }

		[DataMember]
		public Curve CenterAngleSpeedCurve { get; set; }

		[DataMember]
		public Curve SizeSpeedVariationCurve { get; set; }

		[DataMember]
		public Curve CenterSizeSpeedCurve { get; set; }

		[DataMember]
		public Curve SpeedVariationCurve { get; set; }

		[DataMember]
		public Curve CenterSpeedCurve { get; set; }

		[DataMember]
		public Curve SizeCurve { get; set; }

		[DataMember]
		public Curve MaxSizeCurve { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public ShapeList ShapeList { get; set; }

		[DataMember]
		public ShapeType ShapeType { get; set; }

		[DataMember]
		public GeometricShapes GeometricShapes { get; set; }

		[DataMember]
		public ChristmasShapes ChristmasShapes { get; set; }

		[DataMember]
		public FontShapes FontShapes { get; set; }

		[DataMember]
		public bool ManualPosition { get; set; }

		[DataMember]
		public bool RandomShapeSize { get; set; }

		[DataMember]
		public GradientMode GradientMode { get; set; }

		[DataMember]
		public Curve YOffsetCurve { get; set; }

		[DataMember]
		public Curve XOffsetCurve { get; set; }

		[DataMember]
		public Curve AngleCurve { get; set; }

		[DataMember]
		public SerializableFont Font { get; set; }

		[DataMember]
		public Curve ShapeCountCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			ShapesData result = new ShapesData
			{
				Colors = Colors.ToList(),
				ShapeList = ShapeList,
				ShapeType = ShapeType,
				GeometricShapes = GeometricShapes,
				ChristmasShapes = ChristmasShapes,
				FontShapes = FontShapes,
				Orientation = Orientation,
				YOffsetCurve = new Curve(YOffsetCurve),
				XOffsetCurve = new Curve(XOffsetCurve),
				AngleCurve = new Curve(AngleCurve),
				GradientMode = GradientMode,
				Font = new SerializableFont(Font.FontValue),
				LevelCurve = LevelCurve,
				ShapeCountCurve = new Curve(ShapeCountCurve),
				RandomShapeSize = RandomShapeSize,
				SizeCurve = new Curve(SizeCurve),
				MaxSizeCurve = new Curve(MaxSizeCurve),
				AngleSpeedVariationCurve = new Curve(AngleSpeedVariationCurve),
				CenterAngleSpeedCurve = new Curve(CenterAngleSpeedCurve),
				SizeSpeedVariationCurve = new Curve(SizeSpeedVariationCurve),
				CenterSizeSpeedCurve = new Curve(CenterSizeSpeedCurve),
				SpeedVariationCurve = new Curve(SpeedVariationCurve),
				CenterSpeedCurve = new Curve(CenterSpeedCurve),
				ManualPosition = ManualPosition
			};
			return result;
		}
	}
}
