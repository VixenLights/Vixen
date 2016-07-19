using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Bars
{
	[DataContract]
	public class BarsData: EffectTypeModuleData
	{

		public BarsData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			Direction = BarDirection.Up;
			Speed = 1;
			Repeat = 1;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public BarDirection Direction { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public int Repeat { get; set; }

		[DataMember]
		public bool Highlight { get; set; }

		[DataMember]
		public bool Show3D { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			BarsData result = new BarsData
			{
				Colors = Colors.ToList(),
				Direction = Direction,
				Speed = Speed,
				Repeat = Repeat,
				Orientation = Orientation,
				Show3D = Show3D,
				Highlight = Highlight,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
