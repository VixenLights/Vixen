using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Pixel;
using ZedGraph;

namespace VixenModules.Effect.Spiral
{
	[DataContract]
	public class SpiralData: ModuleDataModelBase
	{
		public SpiralData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			Direction = SpiralDirection.Forward;
			Speed = 1;
			Repeat = 1;
			Blend = false;
			Rotation = 20;
			Thickness = 60;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public SpiralDirection Direction { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public int Repeat { get; set; }

		[DataMember]
		public int Thickness { get; set; }

		[DataMember]
		public int Rotation { get; set; }

		[DataMember]
		public bool Blend { get; set; }

		[DataMember]
		public bool Show3D { get; set; }

		[DataMember]
		public bool Grow { get; set; }

		[DataMember]
		public bool Shrink { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public byte Layer { get; set; }

		public override IModuleDataModel Clone()
		{
			SpiralData result = new SpiralData
			{
				Colors = Colors.ToList(),
				Direction = Direction,
				Speed = Speed,
				Repeat = Repeat,
				Orientation = Orientation,
				Show3D = Show3D,
				Thickness = Thickness,
				Rotation = Rotation,
				Blend = Blend,
				LevelCurve = new Curve(LevelCurve),
				Grow = Grow,
				Shrink = Shrink,
				Layer = Layer
			};
			return result;
		}
	}
}
