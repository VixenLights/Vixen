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

namespace VixenModules.Effect.Meteors
{
	[DataContract]
	public class MeteorsData : EffectTypeModuleData
	{

		public MeteorsData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			PixelCountCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			Direction = 180;
			SpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 12.0, 12.0 }));
			CenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 14.0, 14.0 }));
			MinDirection = 0;
			MaxDirection = 360;
			RandomBrightness = false;
			EnableGroundLevel = false;
			MeteorEffect = MeteorsEffect.None;
			ColorType = MeteorsColorType.Palette;
			LengthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 5.0, 5.0 }));
			GroundLevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
			LevelCurve = new Curve(CurveType.Flat100);
			Orientation=StringOrientation.Vertical;
			GroundColor = new ColorGradient(Color.ForestGreen);
			MeteorPerString = false;
			MeteorStartPosition = MeteorStartPosition.InitiallyRandom;
			FlipDirection = false;
			CountPerString = false;
			XCenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YCenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			XSpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
			YSpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
			MeteorMovement = MeteorMovement.None;
			WobbleCurve = new Curve(new PointPairList(new[] { 0.0, 33.0, 66.0, 100.0 }, new[] { 30.0, 70.0, 30.0, 70.0 }));
			WobbleVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public MeteorsColorType ColorType { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Speed { get; set; }

		[DataMember]
		public int Direction { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int PixelCount { get; set; }

		[DataMember]
		public Curve PixelCountCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int MaxSpeed { get; set; }

		[DataMember]
		public Curve SpeedVariationCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int MinSpeed { get; set; }

		[DataMember]
		public Curve CenterSpeedCurve { get; set; }

		[DataMember]
		public int MaxDirection { get; set; }

		[DataMember]
		public int MinDirection { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Length { get; set; }

		[DataMember]
		public Curve LengthCurve { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public bool EnableGroundLevel { get; set; }

		[DataMember]
		public Curve GroundLevelCurve { get; set; }

		[DataMember]
		public MeteorsEffect MeteorEffect { get; set; }
		
		[DataMember(EmitDefaultValue = false)]
		public bool RandomSpeed { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool RandomMeteorPosition { get; set; }

		[DataMember]
		public MeteorStartPosition MeteorStartPosition { get; set; }

		[DataMember]
		public bool RandomBrightness { get; set; }

		[DataMember]
		public ColorGradient GroundColor { get; set; }
		
		[DataMember]
		public StringOrientation Orientation { get; set; }
		
		[DataMember]
		public bool MeteorPerString { get; set; }

		[DataMember]
		public bool FlipDirection { get; set; }

		[DataMember]
		public bool CountPerString { get; set; }

		[DataMember]
		public Curve XCenterSpeedCurve { get; set; }

		[DataMember]
		public Curve YCenterSpeedCurve { get; set; }

		[DataMember]
		public Curve XSpeedVariationCurve { get; set; }

		[DataMember]
		public Curve YSpeedVariationCurve { get; set; }
		
		[DataMember]
		public MeteorMovement MeteorMovement { get; set; }

		[DataMember]
		public Curve WobbleCurve { get; set; }

		[DataMember]
		public Curve WobbleVariationCurve { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load
			if (GroundLevelCurve == null)
			{
				double value;

				GroundLevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
				GroundColor = new ColorGradient(Color.ForestGreen);

				if (SpeedVariationCurve == null)
				{
					double variation = RandomSpeed ? (MaxSpeed - MinSpeed) : 0.0;
					value = PixelEffectBase.ScaleValueToCurve(variation, 200, 1);
					SpeedVariationCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
				}
				if (CenterSpeedCurve == null)
				{
					double center = RandomSpeed ? (double) (MaxSpeed + MinSpeed)/2 : Speed;
					value = PixelEffectBase.ScaleValueToCurve(center, 200, 1);
					CenterSpeedCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
					MinSpeed = 0;
					MaxSpeed = 0;
					Speed = 0;
					RandomSpeed = true;
				}

				if (PixelCountCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(PixelCount, 200, 1);
					PixelCountCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
					PixelCount = 0;
				}

				if (LengthCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(Length, 100, 1);
					LengthCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
					Length = 0;
				}
			}

			if (XCenterSpeedCurve == null)
			{
				XCenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));

				if (YCenterSpeedCurve == null)
				{
					YCenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
				}
				if (YCenterSpeedCurve == null)
				{
					YCenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
				}
				if (XSpeedVariationCurve == null)
				{
					XSpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
				}
				if (YSpeedVariationCurve == null)
				{
					YSpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
				}

				if (WobbleCurve == null)
				{
					WobbleCurve = new Curve(new PointPairList(new[] { 0.0, 33.0, 66.0, 100.0 }, new[] { 30.0, 70.0, 30.0, 70.0 }));
				}

				if (WobbleVariationCurve == null)
				{
					WobbleVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
				}
			}
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			MeteorsData result = new MeteorsData
			{
				Colors = Colors.ToList(),
				ColorType = ColorType,
				LengthCurve = new Curve(LengthCurve),
				SpeedVariationCurve = new Curve(SpeedVariationCurve),
				CenterSpeedCurve = new Curve(CenterSpeedCurve),
				RandomBrightness = RandomBrightness,
				MinDirection = MinDirection,
				MaxDirection = MaxDirection,
				MeteorEffect = MeteorEffect,
				PixelCountCurve = new Curve(PixelCountCurve),
				Orientation = Orientation,
				Direction = Direction,
				LevelCurve = new Curve(LevelCurve),
				GroundLevelCurve = new Curve(GroundLevelCurve),
				GroundColor = GroundColor,
				EnableGroundLevel = EnableGroundLevel,
				MeteorPerString = MeteorPerString,
				MeteorStartPosition = MeteorStartPosition,
				FlipDirection = FlipDirection,
				CountPerString = CountPerString,
				XCenterSpeedCurve = new Curve(XCenterSpeedCurve),
				YCenterSpeedCurve = new Curve(YCenterSpeedCurve),
				XSpeedVariationCurve = new Curve(XSpeedVariationCurve),
				YSpeedVariationCurve = new Curve(YSpeedVariationCurve),
				MeteorMovement = MeteorMovement,
				WobbleVariationCurve = new Curve(WobbleVariationCurve),
				WobbleCurve = new Curve(WobbleCurve)
			};
			return result;
		}
	}
}
