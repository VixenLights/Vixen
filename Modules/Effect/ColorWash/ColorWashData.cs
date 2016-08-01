using System.Drawing;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.ColorWash
{
	[DataContract]
	public class ColorWashData : EffectTypeModuleData
	{

		public ColorWashData()
		{
			Gradient = new ColorGradient();
			Gradient.Colors.Clear();
			Gradient.Colors.Add(new ColorPoint(Color.Red, 0.0));
			Gradient.Colors.Add(new ColorPoint(Color.Lime, .5));
			Gradient.Colors.Add(new ColorPoint(Color.Blue, 1.0));
			Type = ColorWashType.Center;
			Iterations = 1;
			VerticalFade = false;
			HorizontalFade = false;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public ColorGradient Gradient { get; set; }

		[DataMember]
		public ColorWashType Type { get; set; }

		[DataMember]
		public int Iterations { get; set; }

		[DataMember]
		public bool HorizontalFade { get; set; }

		[DataMember]
		public bool VerticalFade { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public bool Shimmer { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			ColorWashData result = new ColorWashData
			{
				Gradient = Gradient,
				Type = Type,
				Iterations = Iterations,
				Orientation = Orientation,
				VerticalFade = VerticalFade,
				HorizontalFade = HorizontalFade,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
