using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Garlands
{
	[DataContract]
	public class GarlandsData : EffectTypeModuleData
	{

		public GarlandsData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			Direction = GarlandsDirection.Up;
			Iterations = 1;
			Speed = 30;
			MovementType = MovementType.Iterations;
			Spacing = 2;
			Type = 0;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public GarlandsDirection Direction { get; set; }

		[DataMember]
		public MovementType MovementType { get; set; }

		[DataMember]
		public int Iterations { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public int Type { get; set; }

		[DataMember]
		public int Spacing { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			GarlandsData result = new GarlandsData
			{
				Colors = Colors.ToList(),
				Direction = Direction,
				Iterations = Iterations,
				Speed = Speed,
				Orientation = Orientation,
				Spacing = Spacing,
				Type = Type,
				MovementType = MovementType,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
