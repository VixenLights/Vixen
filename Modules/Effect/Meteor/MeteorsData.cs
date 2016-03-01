using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Pixel;
using ZedGraph;

namespace VixenModules.Effect.Meteors
{
	[DataContract]
	public class MeteorsData: ModuleDataModelBase
	{

		public MeteorsData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			Speed = 7;
			PixelCount = 15;
			Direction = 180;
			MeteorType = MeteorsType.Palette;
			Length = 5;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public MeteorsType MeteorType { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public int Direction { get; set; }

		[DataMember]
		public int PixelCount { get; set; }

		[DataMember]
		public int Length { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		public override IModuleDataModel Clone()
		{
			MeteorsData result = new MeteorsData
			{
				Colors = Colors.ToList(),
				Speed = Speed,
				MeteorType = MeteorType,
				Length = Length,
				PixelCount = PixelCount,
				Orientation = Orientation,
				Direction = Direction,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
