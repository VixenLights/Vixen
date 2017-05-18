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
		}

		[DataMember]
		public List<ColorGradient> InnerColor { get; set; }

		[DataMember]
		public List<ColorGradient> OutSideColor { get; set; }

		[DataMember]
		public SnowflakeType SnowflakeType { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Speed { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public Curve SpeedCurve { get; set; }

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
		public SnowflakeEffect SnowflakeEffect { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool RandomSpeed { get; set; }

		[DataMember]
		public bool RandomBrightness { get; set; }

		[DataMember]
		public bool PointFlake45 { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{

			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load
			if (SpeedVariationCurve == null)
			{
				int variation = MaxSpeed - MinSpeed;
				double value = PixelEffectBase.ScaleValueToCurve(variation, 60, 1);
				SpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));

				if (FlakeCountCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(FlakeCount, 100, 1);
					FlakeCountCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					FlakeCount = 0;
				}

				if (SpeedCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(Speed, 60, 1);
					SpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					Speed = 0;
				}

				if (CenterSpeedCurve == null)
				{
					double center = (double)(MaxSpeed + MinSpeed) / 2;
					value = PixelEffectBase.ScaleValueToCurve(center, 60, 1);
					CenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					MinSpeed = 0;
					MaxSpeed = 0;
				}
			}

			//Due to the upgrade to a center Speed and Spead Variation there was no need to have the Speedcurve and Random checkbox.
			//This is to convert any existing effects that didn't use Random Speed.
			if (!RandomSpeed)
			{
				CenterSpeedCurve = SpeedCurve;
				SpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
				RandomSpeed = true;
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
				PixelCount = PixelCount
			};
			return result;
		}
	}
}
