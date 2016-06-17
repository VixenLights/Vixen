using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Fireworks
{
	[DataContract]
	public class FireworksData: EffectTypeModuleData
	{
		public FireworksData()
		{
			Colors = new List<Color> { Color.Red, Color.Lime, Color.Blue };
			Particles = 50;
			Explosions = 10;
			Velocity = 5;
			ParticleFade = 50;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
		}

		[DataMember]
		public List<Color> Colors { get; set; }

		[DataMember]
		public int Velocity { get; set; }

		[DataMember]
		public int Explosions { get; set; }

		[DataMember]
		public int ParticleFade { get; set; }

		[DataMember]
		public int Particles { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }
		
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			FireworksData result = new FireworksData
			{
				Velocity = Velocity,
				LevelCurve = new Curve(LevelCurve),
				ParticleFade = ParticleFade,
				Explosions = Explosions,
				Particles = Particles,
				Colors = Colors.ToList()
			};
			return result;
		}
	}
}
