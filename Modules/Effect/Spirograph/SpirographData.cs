using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Spirograph
{
	[DataContract]
	public class SpirographData : EffectTypeModuleData
	{

		public SpirographData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			Speed = 2;
			Animate = true;
			OCR = 88;
			ICR = 21;
			SpirographRange = 100;
			Type = ColorType.Standard;
			Range = 70;
			ColorChase = false;
			Distance = 50;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public ColorType Type { get; set; }

		[DataMember]
		public int OCR { get; set; }

		[DataMember]
		public int ICR { get; set; }

		[DataMember]
		public int Range { get; set; }

		[DataMember]
		public int Distance { get; set; }

		[DataMember]
		public int SpirographRange { get; set; }

		[DataMember]
		public bool Animate { get; set; }

		[DataMember]
		public bool ColorChase { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			SpirographData result = new SpirographData
			{
				Colors = Colors.ToList(),
				Speed = Speed,
				Type = Type,
				OCR = OCR,
				ICR = ICR,
				Range = Range,
				SpirographRange = SpirographRange,
				Distance = Distance,
				ColorChase = ColorChase,
				Orientation = Orientation,
				Animate = Animate,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
