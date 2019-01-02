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

namespace VixenModules.Effect.Snowflakes
{
	[DataContract]
	public class SnowflakesData : EffectTypeModuleData
	{

		public SnowflakesData()
		{
			InnerColor = new List<ColorGradient> { new ColorGradient(Color.Blue) };
			OutSideColor = new List<ColorGradient> { new ColorGradient(Color.White) };
			SnowflakeType = SnowflakeType.Random;
			FlakeCountCurve =  new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			LevelCurve = new Curve(CurveType.Flat100);
			PixelCount = 15;
			SpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 8.0, 8.0 }));
			CenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 11.0, 11.0 }));
			MinDirection = 145;
			MaxDirection = 215;
			RandomBrightness = false;
			PointFlake45 = false;
			SnowflakeEffect = SnowflakeEffect.None;
			ColorType = SnowflakeColorType.Palette;
			Orientation=StringOrientation.Vertical;
			SnowBuildUp = false;
			InitialBuildUp = 0;
			BuildUpSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 15.0, 15.0 }));
			XCenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YCenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			XSpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
			YSpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
			SnowFlakeMovement = SnowFlakeMovement.None;
			WobbleCurve = new Curve(new PointPairList(new[] { 0.0, 33.0, 66.0, 100.0 }, new[] { 30.0, 70.0, 30.0, 70.0 }));
			WobbleVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
		}

		[DataMember]
		public List<ColorGradient> InnerColor { get; set; }

		[DataMember]
		public List<ColorGradient> OutSideColor { get; set; }

		[DataMember]
		public SnowflakeType SnowflakeType { get; set; }

		[DataMember]
		public bool SnowBuildUp { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Speed { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int FlakeCount { get; set; }

		[DataMember]
		public Curve FlakeCountCurve { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public SnowflakeColorType ColorType { get; set; }

		[DataMember]
		public int PixelCount { get; set; }

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

		[DataMember]
		public int InitialBuildUp { get; set; }

		[DataMember]
		public Curve BuildUpSpeedCurve { get; set; }

		[DataMember]
		public SnowflakeEffect SnowflakeEffect { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool RandomSpeed { get; set; }

		[DataMember]
		public bool RandomBrightness { get; set; }

		[DataMember]
		public bool PointFlake45 { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public Curve XCenterSpeedCurve { get; set; }

		[DataMember]
		public Curve YCenterSpeedCurve { get; set; }

		[DataMember]
		public Curve XSpeedVariationCurve { get; set; }

		[DataMember]
		public Curve YSpeedVariationCurve { get; set; }

		[DataMember]
		public SnowFlakeMovement SnowFlakeMovement { get; set; }

		[DataMember]
		public Curve WobbleCurve { get; set; }

		[DataMember]
		public Curve WobbleVariationCurve { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{

			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load
			if (BuildUpSpeedCurve == null)
			{
				double value;
				BuildUpSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 15.0, 15.0 }));

				if (SpeedVariationCurve == null)
				{
					double variation = RandomSpeed ? (MaxSpeed - MinSpeed) : 0.0;
					value = PixelEffectBase.ScaleValueToCurve(variation, 60, 1);
					SpeedVariationCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
				}

				if (FlakeCountCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(FlakeCount, 100, 1);
					FlakeCountCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					FlakeCount = 0;
				}

				if (CenterSpeedCurve == null)
				{
					double center = RandomSpeed ? (double)(MaxSpeed + MinSpeed) / 2 : Speed;

					value = PixelEffectBase.ScaleValueToCurve(center, 60, 1);
					CenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					MinSpeed = 0;
					MaxSpeed = 0;
					Speed = 0;
					RandomSpeed = true;
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
					WobbleCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
				}

				if (WobbleVariationCurve == null)
				{
					WobbleVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
				}
			}
		}


		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			SnowflakesData result = new SnowflakesData
			{
				SnowflakeType = SnowflakeType,
				FlakeCountCurve = new Curve(FlakeCountCurve),
				Orientation = Orientation,
				PointFlake45 = PointFlake45,
				LevelCurve = new Curve(LevelCurve),
				InnerColor = InnerColor.ToList(),
				OutSideColor = OutSideColor.ToList(),
				ColorType = ColorType,
				SpeedVariationCurve = new Curve(SpeedVariationCurve),
				CenterSpeedCurve = new Curve(CenterSpeedCurve),
				RandomBrightness = RandomBrightness,
				MinDirection = MinDirection,
				MaxDirection = MaxDirection,
				SnowflakeEffect = SnowflakeEffect,
				PixelCount = PixelCount,
				SnowBuildUp = SnowBuildUp,
				InitialBuildUp = InitialBuildUp,
				BuildUpSpeedCurve = new Curve(BuildUpSpeedCurve),
				XCenterSpeedCurve = new Curve(XCenterSpeedCurve),
				YCenterSpeedCurve = new Curve(YCenterSpeedCurve),
				XSpeedVariationCurve = new Curve(XSpeedVariationCurve),
				YSpeedVariationCurve = new Curve(YSpeedVariationCurve),
				SnowFlakeMovement = SnowFlakeMovement,
				WobbleVariationCurve = new Curve(WobbleVariationCurve),
				WobbleCurve = new Curve(WobbleCurve)
			};
			return result;
		}
	}
}
