using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Spiral
{
	[DataContract]
	public class SpiralData: EffectTypeModuleData
	{
		public SpiralData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			Direction = SpiralDirection.Forward;
			Speed = 1;
			Repeat = 1;
			Blend = false;
			RotationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 57.0, 57.0 }));
			ThicknessCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 60.0, 60.0 }));
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Orientation=StringOrientation.Vertical;
			SpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 70.0, 70.0 }));
			MovementType = MovementType.Iterations;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public SpiralDirection Direction { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public int Repeat { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Thickness { get; set; }

		[DataMember]
		public Curve ThicknessCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Rotation { get; set; }

		[DataMember]
		public Curve RotationCurve { get; set; }

		[DataMember]
		public bool Blend { get; set; }

		[DataMember]
		public bool Show3D { get; set; }

		[DataMember]
		public bool Grow { get; set; }

		[DataMember]
		public bool Shrink { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public MovementType MovementType { get; set; }

		[DataMember]
		public Curve SpeedCurve { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load
			if (RotationCurve == null)
			{
				double value = PixelEffectBase.ScaleValueToCurve(Rotation, 150d, -150d);
				RotationCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
				Rotation = 0;

				if (ThicknessCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(Thickness, 100, 1);
					ThicknessCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					Thickness = 0;
				}
			}

			if (SpeedCurve == null)
			{
				SpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 70.0, 70.0 }));
			}
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			SpiralData result = new SpiralData
			{
				Colors = Colors.ToList(),
				Direction = Direction,
				Speed = Speed,
				Repeat = Repeat,
				Orientation = Orientation,
				Show3D = Show3D,
				ThicknessCurve = new Curve(ThicknessCurve),
				RotationCurve = new Curve(RotationCurve),
				Blend = Blend,
				LevelCurve = new Curve(LevelCurve),
				Grow = Grow,
				Shrink = Shrink,
				SpeedCurve = new Curve(SpeedCurve),
				MovementType = MovementType
			};
			return result;
		}
	}
}
