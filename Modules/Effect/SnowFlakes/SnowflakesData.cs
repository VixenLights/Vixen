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
			Speed = 5;
			FlakeCount = 10;
			LevelCurve = new Curve(CurveType.Flat100);
			PixelCount = 15;
			MaxSpeed = 10;
			MinSpeed = 5;
			MinDirection = 145;
			MaxDirection = 215;
			RandomSpeed = true;
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

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public int FlakeCount { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public SnowflakeColorType ColorType { get; set; }

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
		public SnowflakeEffect SnowflakeEffect { get; set; }

		[DataMember]
		public bool RandomSpeed { get; set; }

		[DataMember]
		public bool RandomBrightness { get; set; }

		[DataMember]
		public bool PointFlake45 { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			SnowflakesData result = new SnowflakesData
			{
				SnowflakeType = SnowflakeType,
				Speed = Speed,
				FlakeCount = FlakeCount,
				Orientation = Orientation,
				PointFlake45 = PointFlake45,
				LevelCurve = new Curve(LevelCurve),
				InnerColor = InnerColor.ToList(),
				OutSideColor = OutSideColor.ToList(),
				ColorType = ColorType,
				MaxSpeed = MaxSpeed,
				MinSpeed = MinSpeed,
				RandomBrightness = RandomBrightness,
				MinDirection = MinDirection,
				MaxDirection = MaxDirection,
				SnowflakeEffect = SnowflakeEffect,
				PixelCount = PixelCount,
				RandomSpeed = RandomSpeed
			};
			return result;
		}
	}
}
