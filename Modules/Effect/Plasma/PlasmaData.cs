using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Plasma
{
	[DataContract]
	public class PlasmaData : EffectTypeModuleData
	{

		public PlasmaData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			SpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			PlasmaStyle = 1;
			LineDensityCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			ColorType = PlasmaColorType.Normal;
			LevelCurve = new Curve(CurveType.Flat100);
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Speed { get; set; }

		[DataMember]
		public Curve SpeedCurve { get; set; }

		[DataMember]
		public int PlasmaStyle { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int LineDensity { get; set; }

		[DataMember]
		public Curve LineDensityCurve { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public PlasmaColorType ColorType { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load
			if (LineDensityCurve == null)
			{
				double value = PixelEffectBase.ScaleValueToCurve(LineDensity, 10, 1);
				LineDensityCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
				LineDensity = 0;

				if (SpeedCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(Speed, 100, 1);
					SpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					Speed = 0;
				}
			}
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			PlasmaData result = new PlasmaData
			{
				Colors = Colors.ToList(),
				SpeedCurve = new Curve(SpeedCurve),
				LineDensityCurve = new Curve(LineDensityCurve),
				PlasmaStyle = PlasmaStyle,
				Orientation = Orientation,
				ColorType = ColorType,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
