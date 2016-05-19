using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Instrumentation;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Pixel;
using ZedGraph;

namespace VixenModules.Effect.Life
{
	[DataContract]
	public class LifeData: ModuleDataModelBase
	{

		public LifeData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			Speed = 5;
			CellsToStart = 50;
			Type = 1;
			Repeat = 1;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 70.0, 70.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public int Type { get; set; }

		[DataMember]
		public int Repeat { get; set; }

		[DataMember]
		public int CellsToStart { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		public override IModuleDataModel Clone()
		{
			LifeData result = new LifeData
			{
				Colors = Colors.ToList(),
				Speed = Speed,
				CellsToStart = CellsToStart,
				Repeat = Repeat,
				Type = Type,
				Orientation = Orientation,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
