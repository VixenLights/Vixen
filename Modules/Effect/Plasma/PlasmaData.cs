using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Plasma
{
	[DataContract]
	public class PlasmaData : EffectTypeModuleData
	{

		public PlasmaData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			Speed = 10;
			PlasmaStyle = 1;
			LineDensity = 1;
			ColorType = PlasmaColorType.Normal;
			LevelCurve = new Curve(CurveType.Flat100);
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public int PlasmaStyle { get; set; }

		[DataMember]
		public int LineDensity { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public PlasmaColorType ColorType { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			PlasmaData result = new PlasmaData
			{
				Colors = Colors.ToList(),
				Speed = Speed,
				LineDensity = LineDensity,
				PlasmaStyle = PlasmaStyle,
				Orientation = Orientation,
				ColorType = ColorType,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
