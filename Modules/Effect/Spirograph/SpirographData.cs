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
			SpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 2.0, 2.0 }));
			Animate = true;
			OCR = 88;
			ICR = 21;
			SpirographRangeCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 33.0, 33.0 }));
			Type = ColorType.Standard;
			RangeCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 35.0, 35.0 }));
			ColorChase = false;
			DistanceCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Speed { get; set; }

		[DataMember]
		public Curve SpeedCurve { get; set; }

		[DataMember]
		public ColorType Type { get; set; }

		[DataMember]
		public int OCR { get; set; }

		[DataMember]
		public int ICR { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Range { get; set; }

		[DataMember]
		public Curve RangeCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Distance { get; set; }

		[DataMember]
		public Curve DistanceCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int SpirographRange { get; set; }

		[DataMember]
		public Curve SpirographRangeCurve { get; set; }

		[DataMember]
		public bool Animate { get; set; }

		[DataMember]
		public bool ColorChase { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load
			if (DistanceCurve == null)
			{
				double value = PixelEffectBase.ScaleValueToCurve(Distance, 100, 1);
				DistanceCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
				Distance = 0;

				if (RangeCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(Range, 200, 1);
					RangeCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
					Range = 0;
				}
				if (SpirographRangeCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(SpirographRange, 300, 1);
					SpirographRangeCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
					SpirographRange = 0;
				}
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
			SpirographData result = new SpirographData
			{
				Colors = Colors.ToList(),
				Speed = Speed,
				SpeedCurve = new Curve(SpeedCurve),
				Type = Type,
				OCR = OCR,
				ICR = ICR,
				Range = Range,
				RangeCurve = new Curve(RangeCurve),
				SpirographRange = SpirographRange,
				SpirographRangeCurve = new Curve(SpirographRangeCurve),
				Distance = Distance,
				DistanceCurve = new Curve(DistanceCurve),
				ColorChase = ColorChase,
				Orientation = Orientation,
				Animate = Animate,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
