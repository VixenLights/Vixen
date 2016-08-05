using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.SnowStorm
{
	[DataContract]
	public class SnowStormData : EffectTypeModuleData
	{

		public SnowStormData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			Speed = 50;
			Count = 10;
			Length = 5;
			ReverseDirection = false;
			ColorType = SnowStormColorType.Palette;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public SnowStormColorType ColorType { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public bool ReverseDirection { get; set; }

		[DataMember]
		public int Count { get; set; }

		[DataMember]
		public int Length { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			SnowStormData result = new SnowStormData
			{
				Colors = Colors.ToList(),
				Speed = Speed,
				Orientation = Orientation,
				Count = Count,
				Length = Length,
				ReverseDirection = ReverseDirection,
				ColorType = ColorType,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
