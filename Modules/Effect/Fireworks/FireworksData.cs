using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
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
			Particles = 40;
			MinParticles = 10;
			MaxParticles = 50;
			ColorType = FireworksColorType.Standard;
			ParticleFade = 50;
			LevelCurve = new Curve(CurveType.Flat100);
			ExplosionSensitivity = 5;
			Sensitivity = -70;
			LowPass = false;
			LowPassFreq = 1000;
			HighPass = false;
			HighPassFreq = 500;
			Normalize = true;
			DecayTime = 1500;
			AttackTime = 52;
			EnableAudio = false;
			Gain = 0;
		}

		[DataMember]
		public List<ColorGradient> ColorGradients { get; set; }

		[DataMember]
		public FireworksColorType ColorType { get; set; }

		[DataMember]
		public int DecayTime { get; set; }

		[DataMember]
		public bool EnableAudio { get; set; }

		[DataMember]
		public int ExplosionSensitivity { get; set; }

		[DataMember]
		public int Gain { get; set; }

		[DataMember]
		public int AttackTime { get; set; }

		[DataMember]
		public int Velocity { get; set; }

		[DataMember]
		public int Sensitivity { get; set; }

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

		[DataMember]
		public bool LowPass { get; set; }

		[DataMember]
		public int LowPassFreq { get; set; }

		[DataMember]
		public bool HighPass { get; set; }

		[DataMember]
		public int HighPassFreq { get; set; }

		[DataMember]
		public bool Normalize { get; set; }

		[OnDeserialized]
		void OnDeserialized(StreamingContext c)
		{
			//Ensure defaults for new fields that were not in older effects.
			if (DecayTime == 0)
			{
				ExplosionSensitivity = 5;
				Sensitivity = -70;
				LowPass = false;
				LowPassFreq = 1000;
				HighPass = false;
				HighPassFreq = 500;
				Normalize = true;
				DecayTime = 1500;
				AttackTime = 52;
			}
		}
		
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
				Sensitivity = Sensitivity,
				RandomVelocity = RandomVelocity,
				ColorGradients = ColorGradients.ToList(),
				LowPass = LowPass,
				LowPassFreq = LowPassFreq,
				HighPass = HighPass,
				HighPassFreq = HighPassFreq,
				Normalize = Normalize,
				DecayTime = DecayTime,
				AttackTime = AttackTime,
				ExplosionSensitivity = ExplosionSensitivity,
				EnableAudio = EnableAudio,
			};
			return result;
		}
	}
}
