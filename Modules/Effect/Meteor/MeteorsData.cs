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
			RandomMeteorPosition = false;
			MeteorEffect = MeteorsEffect.None;
			ColorType = MeteorsColorType.Palette;
			LengthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 5.0, 5.0 }));
			LevelCurve = new Curve(CurveType.Flat100);
			Orientation=StringOrientation.Vertical;
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
		public MeteorsEffect MeteorEffect { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool RandomSpeed { get; set; }

		[DataMember]
		public bool RandomMeteorPosition { get; set; }

		[DataMember]
		public bool RandomBrightness { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load

			if (SpeedVariationCurve == null)
			{
				double variation = RandomSpeed ? (MaxSpeed - MinSpeed) : 0.0;
				double value = PixelEffectBase.ScaleValueToCurve(variation, 200, 1);
				SpeedVariationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));

				if (CenterSpeedCurve == null)
				{
					double center = RandomSpeed ? (double)(MaxSpeed + MinSpeed) / 2 : Speed;
					value = PixelEffectBase.ScaleValueToCurve(center, 200, 1);
					CenterSpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					MinSpeed = 0;
					MaxSpeed = 0;
					Speed = 0;
					RandomSpeed = true;
				}

				if (PixelCountCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(PixelCount, 200, 1);
					PixelCountCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					PixelCount = 0;
				}

				if (LengthCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(Length, 100, 1);
					LengthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					Length = 0;
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
				RandomMeteorPosition = RandomMeteorPosition,
				Orientation = Orientation,
				Direction = Direction,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
