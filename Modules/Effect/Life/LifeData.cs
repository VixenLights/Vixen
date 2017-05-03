using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Instrumentation;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Life
{
	[DataContract]
	public class LifeData : EffectTypeModuleData
	{

		public LifeData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			SpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 25.0, 25.0 }));
			CellsToStart = 50;
			Type = 1;
			Repeat = 1;
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
		public int Type { get; set; }

		[DataMember]
		public int Repeat { get; set; }

		[DataMember]
		public int CellsToStart { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load

			if (SpeedCurve == null)
			{
				double value = PixelEffectBase.ScaleValueToCurve(Speed, 20, 1);
				SpeedCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
				Speed = 0;
			}
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			LifeData result = new LifeData
			{
				Colors = Colors.ToList(),
				SpeedCurve = new Curve(SpeedCurve),
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
