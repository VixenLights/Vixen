using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Butterfly
{
	[DataContract]
	public class ButterflyData: EffectTypeModuleData
	{

		public ButterflyData()
		{
			Gradient = new ColorGradient();
			Gradient.Colors.Clear();
			Gradient.Colors.Add(new ColorPoint(Color.Red,0.0));
			Gradient.Colors.Add(new ColorPoint(Color.Lime, 1.0));
			Iterations = 1;
			ColorScheme = ColorScheme.Gradient;
			ButterflyType = ButterflyType.Type1;
			Repeat = 1;
			BackgroundSkips = 2;
			BackgroundChunks = 1;
			Direction = Direction.Forward;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public ColorGradient Gradient { get; set; }

		[DataMember]
		public ButterflyType ButterflyType { get; set; }

		[DataMember]
		public Direction Direction { get; set; }

		[DataMember]
		public ColorScheme ColorScheme { get; set; }

		[DataMember]
		public int Repeat { get; set; }

		[DataMember]
		public int BackgroundSkips { get; set; }

		[DataMember]
		public int BackgroundChunks { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public int Iterations { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			ButterflyData result = new ButterflyData
			{
				
				ButterflyType = ButterflyType,
				Repeat = Repeat,
				BackgroundSkips = BackgroundSkips,
				Orientation = Orientation,
				LevelCurve = new Curve(LevelCurve),
				Gradient = Gradient,
				Direction = Direction,
				BackgroundChunks = BackgroundChunks,
				ColorScheme = ColorScheme,
				Iterations = Iterations
			};
			return result;
		}
	}
}
