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
			HueShift = 0;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Orientation = StringOrientation.Vertical;
		}

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public FireDirection Location { get; set; }

		[DataMember]
		public Curve Height { get; set; }

		[DataMember]
		public int HueShift { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }
		
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			FireData result = new FireData
			{
				Location = Location,
				Height = Height,
				HueShift = HueShift,
				Orientation = Orientation,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
