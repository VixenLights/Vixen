using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Bars
{
	[DataContract]
	public class BarsData: EffectTypeModuleData
	{
		public BarsData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			Direction = BarDirection.Up;
			Speed = 1;
			Repeat = 1;
			MovementType = MovementType.Iterations;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			SpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			Orientation =StringOrientation.Vertical;
            ZigZagAmplitude = 25;
            ZigZagBarThickness = 5;
            ZigZagSpacing = 5;           
            BarType = BarType.Flat;
            ZigZagPeriod = 25;
			RotationAngle = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
			HighlightPercentage = 5;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public BarDirection Direction { get; set; }

		[DataMember]
		public MovementType MovementType { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public Curve SpeedCurve { get; set; }

		[DataMember]
		public int Repeat { get; set; }

		[DataMember]
		public bool Highlight { get; set; }

		[DataMember]
		public bool Show3D { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

        [DataMember]
        public int ZigZagAmplitude { get; set; }

        [DataMember]
        public int ZigZagBarThickness { get; set; }

        [DataMember]
        public BarType BarType { get; set; }

        [DataMember]
        public int ZigZagSpacing { get; set; }

        [DataMember]
        public int ZigZagPeriod { get; set; }

        [DataMember]
        public Curve RotationAngle { get; set; }

        [DataMember]
        public int HighlightPercentage { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load
			if (SpeedCurve == null)
			{
				SpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			}
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			BarsData result = new BarsData
			{
				Colors = Colors.ToList(),
				Direction = Direction,
				Speed = Speed,
				Repeat = Repeat,
				Orientation = Orientation,
				Show3D = Show3D,
				MovementType = MovementType,
				Highlight = Highlight,
				LevelCurve = new Curve(LevelCurve),
				SpeedCurve = new Curve(SpeedCurve),
                ZigZagAmplitude = ZigZagAmplitude,
                BarType = BarType,
                ZigZagBarThickness = ZigZagBarThickness,
                ZigZagSpacing = ZigZagSpacing,               
                ZigZagPeriod = ZigZagPeriod,
				RotationAngle = RotationAngle,
				HighlightPercentage = HighlightPercentage,
            };
			return result;
		}
	}
}
