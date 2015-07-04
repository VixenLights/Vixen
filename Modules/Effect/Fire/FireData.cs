using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.Curves;
using VixenModules.Effect.Pixel;
using ZedGraph;

namespace VixenModules.Effect.Fire
{
	[DataContract]
	public class FireData: ModuleDataModelBase
	{
		public FireData()
		{
			Location = FireDirection.Bottom;
			Height = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			HueShift = 0;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
		}

		[DataMember]
		public FireDirection Location { get; set; }

		[DataMember]
		public Curve Height { get; set; }

		[DataMember]
		public int HueShift { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		public override IModuleDataModel Clone()
		{
			FireData result = new FireData
			{
				Location = Location,
				Height = Height,
				HueShift = HueShift,
				LevelCurve = new Curve(LevelCurve)
			};
			return result;
		}
	}
}
