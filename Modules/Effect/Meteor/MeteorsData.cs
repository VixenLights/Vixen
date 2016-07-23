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
			Speed = 30;
			PixelCount = 20;
			Direction = 180;
			MaxSpeed = 40;
			MinSpeed = 15;
			MinDirection = 0;
			MaxDirection = 360;
			RandomBrightness = false;
			RandomSpeed = true;
			RandomMeteorPosition = false;
			MeteorEffect = MeteorsEffect.None;
			ColorType = MeteorsColorType.Palette;
			Length = 5;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public MeteorsColorType ColorType { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public int Direction { get; set; }

		[DataMember]
		public int PixelCount { get; set; }

		[DataMember]
		public int MaxSpeed { get; set; }

		[DataMember]
		public int MinSpeed { get; set; }

		[DataMember]
		public int MaxDirection { get; set; }

		[DataMember]
		public int MinDirection { get; set; }

		[DataMember]
		public int Length { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public MeteorsEffect MeteorEffect { get; set; }

		[DataMember]
		public bool RandomSpeed { get; set; }

		[DataMember]
		public bool RandomMeteorPosition { get; set; }

		[DataMember]
		public bool RandomBrightness { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			MeteorsData result = new MeteorsData
			{
				Colors = Colors.ToList(),
				Speed = Speed,
				ColorType = ColorType,
				Length = Length,
				MaxSpeed = MaxSpeed,
				MinSpeed = MinSpeed,
				RandomBrightness = RandomBrightness,
				MinDirection = MinDirection,
				MaxDirection = MaxDirection,
				MeteorEffect = MeteorEffect,
				PixelCount = PixelCount,
				RandomMeteorPosition = RandomMeteorPosition,
				RandomSpeed = RandomSpeed,
				Orientation = Orientation,
				Direction = Direction,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
