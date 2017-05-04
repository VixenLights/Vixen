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
			SpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 30.0, 30.0 }));
			MovementType = MovementType.Iterations;
			SpacingCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
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

		[DataMember(EmitDefaultValue = false)]
		public int Speed { get; set; }

		[DataMember]
		public Curve SpeedCurve { get; set; }

		[DataMember]
		public int Type { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Spacing { get; set; }

		[DataMember]
		public Curve SpacingCurve { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load
			if (SpacingCurve == null)
			{
				double value = PixelEffectBase.ScaleValueToCurve(Spacing, 20, 1);
				SpacingCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
				Spacing = 0;

				if (SpeedCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(Speed, 100, 1);
					SpeedCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
					Speed = 0;
				}
			}
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			GarlandsData result = new GarlandsData
			{
				Colors = Colors.ToList(),
				Direction = Direction,
				Iterations = Iterations,
				SpeedCurve = new Curve(SpeedCurve),
				Orientation = Orientation,
				SpacingCurve = new Curve(SpacingCurve),
				Type = Type,
				MovementType = MovementType,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
