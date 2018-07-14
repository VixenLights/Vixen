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
	public class ShapesData : EffectTypeModuleData
	{
		public ShapesData()
		{
			OutlineColors = new List<ColorGradient> { new ColorGradient(Color.Red) };
			FirstFillColors = new List<ColorGradient> { new ColorGradient(Color.Blue) };
			SecondFillColors = new List<ColorGradient> { new ColorGradient(Color.Green) };
			AngleSpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 5.0, 5.0 }));
			CenterAngleSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 20.0, 20.0 }));
			SizeSpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 5.0, 5.0 }));
			CenterSizeSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 20.0, 20.0 }));
			SpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 5.0, 5.0 }));
			CenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 20.0, 20.0 }));
			ShapeType = ShapeType.Wrap;
			SizeMode = SizeMode.Out;
			FileName = String.Empty;
			StrokeFill = true;
			RandomSize = true;
			FadeType = FadeType.None;
			SizeCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 40.0, 40.0 }));
			SizeVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 30.0, 30.0 }));
			CenterSizeCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 20.0, 20.0 }));
			ShapeList = ShapeList.GeometricShapes;
			GeometricShapesList = GeometricShapesList.Square;
			ChristmasShapesList = ChristmasShapesList.SnowMan;
			HalloweenShapesList = HalloweenShapesList.Skull;
			XOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			AngleCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 100.0 }));
			Orientation = StringOrientation.Vertical;
			LevelCurve = new Curve(CurveType.Flat100); 
			ShapeOutLineCurve = new Curve(CurveType.Flat100);
			ShapeOutLineSpaceCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
			StrokeWidth = 1;
			StarInsideSize = 50;
			ShapeSizeRatio = 50;
			CrossSizeRatio = 100;
			PolygonSides = 6;
			RoundedCorner = false;
			RemoveShape = false;
			ScaleToGrid = false;
			RandomAngle = true;
			StarPoints = 7;
			NonIntersectingStarPoints = 6;
			SkipPoints = 3;
			Fill = false;
			ShapeCount = 10;
			ShapeCountCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 6.0, 6.0 }));
			ShapeMode = ShapeMode.None;
		}

		[DataMember]
		public List<ColorGradient> OutlineColors { get; set; }

		[DataMember]
		public List<ColorGradient> FirstFillColors { get; set; }

		[DataMember]
		public List<ColorGradient> SecondFillColors { get; set; }

		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public bool RoundedCorner { get; set; }
		
		[DataMember]
		public FadeType FadeType { get; set; }

		[DataMember]
		public int StarPoints { get; set; }

		[DataMember]
		public int NonIntersectingStarPoints { get; set; }

		[DataMember]
		public int SkipPoints { get; set; }

		[DataMember]
		public int PolygonSides { get; set; }

		[DataMember]
		public int ShapeSizeRatio { get; set; }

		[DataMember]
		public int CrossSizeRatio { get; set; }

		[DataMember]
		public int StarInsideSize { get; set; }

		[DataMember]
		public Curve ShapeOutLineCurve { get; set; }

		[DataMember]
		public int StrokeWidth { get; set; }

		[DataMember]
		public Curve ShapeOutLineSpaceCurve { get; set; }

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
		public Curve SizeVariationCurve { get; set; }

		[DataMember]
		public Curve CenterSizeCurve { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public ShapeList ShapeList { get; set; }
		
		[DataMember]
		public ShapeType ShapeType { get; set; }

		[DataMember]
		public SizeMode SizeMode { get; set; }

		[DataMember]
		public GeometricShapesList GeometricShapesList { get; set; }

		[DataMember]
		public ChristmasShapesList ChristmasShapesList { get; set; }

		[DataMember]
		public HalloweenShapesList HalloweenShapesList { get; set; }
		
		[DataMember]
		public bool RandomSize { get; set; }

		[DataMember]
		public bool Fill { get; set; }

		[DataMember]
		public bool StrokeFill { get; set; }

		[DataMember]
		public bool ScaleToGrid { get; set; }

		[DataMember]
		public bool RandomAngle { get; set; }

		[DataMember]
		public bool RemoveShape { get; set; }

		[DataMember]
		public Curve YOffsetCurve { get; set; }

		[DataMember]
		public Curve XOffsetCurve { get; set; }

		[DataMember]
		public Curve AngleCurve { get; set; }

		[DataMember]
		public Curve ShapeCountCurve { get; set; }

		[DataMember]
		public int ShapeCount { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public Guid MarkCollectionId { get; set; }

		[DataMember]
		public ShapeMode ShapeMode { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			ShapesData result = new ShapesData
			{
				OutlineColors = OutlineColors.ToList(),
				FirstFillColors = FirstFillColors.ToList(),
				SecondFillColors = SecondFillColors.ToList(),
				FileName = FileName,
				ShapeList = ShapeList,
				ShapeType = ShapeType,
				ScaleToGrid = ScaleToGrid,
				RandomAngle = RandomAngle,
				ShapeSizeRatio = ShapeSizeRatio,
				CrossSizeRatio = CrossSizeRatio,
				GeometricShapesList = GeometricShapesList,
				ChristmasShapesList = ChristmasShapesList,
				HalloweenShapesList = HalloweenShapesList,
				Orientation = Orientation,
				YOffsetCurve = new Curve(YOffsetCurve),
				XOffsetCurve = new Curve(XOffsetCurve),
				AngleCurve = new Curve(AngleCurve),
				LevelCurve = new Curve(LevelCurve),
				StrokeWidth = StrokeWidth,
				ShapeCountCurve = new Curve(ShapeCountCurve),
				ShapeCount = ShapeCount,
				RemoveShape = RemoveShape,
				SizeCurve = new Curve(SizeCurve),
				ShapeOutLineCurve = new Curve(ShapeOutLineCurve),
				ShapeOutLineSpaceCurve = new Curve(ShapeOutLineSpaceCurve),
				StarInsideSize = StarInsideSize,
				SizeVariationCurve = new Curve(SizeVariationCurve),
				CenterSizeCurve = new Curve(CenterSizeCurve),
				AngleSpeedVariationCurve = new Curve(AngleSpeedVariationCurve),
				CenterAngleSpeedCurve = new Curve(CenterAngleSpeedCurve),
				SizeSpeedVariationCurve = new Curve(SizeSpeedVariationCurve),
				CenterSizeSpeedCurve = new Curve(CenterSizeSpeedCurve),
				SpeedVariationCurve = new Curve(SpeedVariationCurve),
				CenterSpeedCurve = new Curve(CenterSpeedCurve),
				StarPoints = StarPoints,
				NonIntersectingStarPoints = NonIntersectingStarPoints,
				SkipPoints = SkipPoints,
				SizeMode = SizeMode,
				RandomSize = RandomSize,
				PolygonSides = PolygonSides,
				Fill = Fill,
				FadeType = FadeType,
				StrokeFill = StrokeFill,
				RoundedCorner = RoundedCorner,
				ShapeMode = ShapeMode,
				MarkCollectionId = MarkCollectionId
			};
			return result;
		}
	}
}
