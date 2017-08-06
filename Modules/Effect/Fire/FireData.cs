using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Fire
{
	[DataContract]
	public class FireData: EffectTypeModuleData
	{
		public FireData()
		{
			Location = FireDirection.Bottom;
			Height = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			HueShiftCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
			Orientation = StringOrientation.Vertical;
		}

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public FireDirection Location { get; set; }

		[DataMember]
		public Curve Height { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int HueShift { get; set; }

		[DataMember]
		public Curve HueShiftCurve { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load
			if (HueShiftCurve == null)
			{
				double value = PixelEffectBase.ScaleValueToCurve(HueShift, 100, 0);
				HueShiftCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { value, value }));
				HueShift = 0;
			}
		}
		
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			FireData result = new FireData
			{
				Location = Location,
				Height = Height,
				Orientation = Orientation,
				LevelCurve = new Curve(LevelCurve),
				HueShiftCurve = new Curve(HueShiftCurve)
			};
			return result;
		}
	}
}
