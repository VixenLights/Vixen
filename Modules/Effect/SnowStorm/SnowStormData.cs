using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.SnowStorm
{
	[DataContract]
	public class SnowStormData : EffectTypeModuleData
	{

		public SnowStormData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			SpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			CountCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			LengthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 25.0, 25.0 }));
			ReverseDirection = false;
			ColorType = SnowStormColorType.Palette;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public SnowStormColorType ColorType { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Speed { get; set; }

		[DataMember]
		public Curve SpeedCurve { get; set; }

		[DataMember]
		public bool ReverseDirection { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Count { get; set; }

		[DataMember]
		public Curve CountCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Length { get; set; }

		[DataMember]
		public Curve LengthCurve { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load
			if (LengthCurve == null)
			{
				double value = PixelEffectBase.ScaleValueToCurve(Length, 20, 1);
				LengthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
				Length = 0;

				if (CountCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(Count, 100, 1);
					CountCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
					Count = 0;
				}
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
			SnowStormData result = new SnowStormData
			{
				Colors = Colors.ToList(),
				SpeedCurve = new Curve(SpeedCurve),
				Orientation = Orientation,
				CountCurve = new Curve(CountCurve),
				LengthCurve = new Curve(LengthCurve),
				ReverseDirection = ReverseDirection,
				ColorType = ColorType,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
