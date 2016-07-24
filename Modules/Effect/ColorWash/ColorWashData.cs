using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
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
			Colors = new List<ColorGradient> { new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue) };
			Type = ColorWashType.Center;
			Iterations = 1;
			VerticalFade = true;
			HorizontalFade = true;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

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
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			ColorWashData result = new ColorWashData
			{
				Colors = Colors.ToList(),
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
