using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Fireworks
{
	[DataContract]
	public class FireworksData: EffectTypeModuleData
	{
		public FireworksData()
		{
			ColorGradients = new List<ColorGradient> { new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue) };
			Explosions = 10;
			RandomVelocity = false;
			Velocity = 5;
			MinVelocity = 1;
			MaxVelocity = 10;
			RandomParticles = true;
			Particles = 50;
			MinParticles = 10;
			MaxParticles = 90;
			ColorType = FireworksColorType.Standard;
			ParticleFade = 50;
			LevelCurve = new Curve(CurveType.Flat100);
		}

		[DataMember]
		public List<ColorGradient> ColorGradients { get; set; }

		[DataMember]
		public FireworksColorType ColorType { get; set; }

		[DataMember]
		public int Velocity { get; set; }

		[DataMember]
		public int MinVelocity { get; set; }

		[DataMember]
		public int MaxVelocity { get; set; }

		[DataMember]
		public bool RandomVelocity { get; set; }

		[DataMember]
		public bool RandomParticles { get; set; }

		[DataMember]
		public int MinParticles { get; set; }

		[DataMember]
		public int MaxParticles { get; set; }

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
				MinVelocity = MinVelocity,
				MaxVelocity = MaxVelocity,
				LevelCurve = new Curve(LevelCurve),
				ParticleFade = ParticleFade,
				RandomParticles = RandomParticles,
				Explosions = Explosions,
				ColorType = ColorType,
				Particles = Particles,
				MinParticles = MinParticles,
				MaxParticles = MaxParticles,
				RandomVelocity = RandomVelocity,
				ColorGradients = ColorGradients.ToList(),
			};
			return result;
		}
	}
}
