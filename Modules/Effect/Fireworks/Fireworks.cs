using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Module.Media;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Media.Audio;

namespace VixenModules.Effect.Fireworks
{
	public class Fireworks:PixelEffectBase
	{
		private FireworksData _data;
		private List<RgbFireworks> _fireworkBursts;
		private static Random _random = new Random();
		private int _maxFlakes;
		private readonly AudioUtilities _audioUtilities;
		private const int Spacing = 50;
		private int _explosion;
		
		public Fireworks()
		{
			_data = new FireworksData();
			_audioUtilities = new AudioUtilities();
		}

		[Browsable(false)]
		private AudioUtilities AudioUtilities { get { return _audioUtilities; } }

		#region Config

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Explosions")]
		[ProviderDescription(@"Explosions")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(0)]
		public int Explosions
		{
			get { return _data.Explosions; }
			set
			{
				_data.Explosions = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Explosion Sensitivity")]
		[ProviderDescription(@"Varies the Explosion sensitivity")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 10, 1)]
		[PropertyOrder(1)]
		public int ExplosionSensitivity
		{
			get { return _data.ExplosionSensitivity; }
			set
			{
				_data.ExplosionSensitivity = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Random Velocity")]
		[ProviderDescription(@"Random Velocity")]
		[PropertyOrder(2)]
		public bool RandomVelocity
		{
			get { return _data.RandomVelocity; }
			set
			{
				_data.RandomVelocity = value;
				IsDirty = true;
				UpdateRandomVelocityAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Velocity")]
		[ProviderDescription(@"Velocity")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 10, 1)]
		[PropertyOrder(3)]
		public int Velocity
		{
			get { return _data.Velocity; }
			set
			{
				_data.Velocity = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Min Velocity")]
		[ProviderDescription(@"Min Velocity")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 10, 1)]
		[PropertyOrder(4)]
		public int MinVelocity
		{
			get { return _data.MinVelocity; }
			set
			{
				if (MaxVelocity <= value)
					value = MaxVelocity - 1;  //Ensures MinVelocity is above MaxVelocity
				_data.MinVelocity = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Max Velocity")]
		[ProviderDescription(@"Max Velocity")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 10, 1)]
		[PropertyOrder(5)]
		public int MaxVelocity
		{
			get { return _data.MaxVelocity; }
			set
			{
				if (MinVelocity > value)
					value = MinVelocity + 1;  //Ensures MaxVelocity is above MinVelocity
				_data.MaxVelocity = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Random Particles")]
		[ProviderDescription(@"Random Particles")]
		[PropertyOrder(6)]
		public bool RandomParticles
		{
			get { return _data.RandomParticles; }
			set
			{
				_data.RandomParticles = value;
				IsDirty = true;
				UpdateRandomParticlesAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Particles")]
		[ProviderDescription(@"Particles")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(7)]
		public int Particles
		{
			get { return _data.Particles; }
			set
			{
				_data.Particles = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Min Particles")]
		[ProviderDescription(@"Min Particles")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(8)]
		public int MinParticles
		{
			get { return _data.MinParticles; }
			set
			{
				if (MaxParticles <= value)
					value = MaxParticles - 1; //Ensures MinParticles is below MaxParticles
				_data.MinParticles = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Max Particles")]
		[ProviderDescription(@"Max Particles")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(9)]
		public int MaxParticles
		{
			get { return _data.MaxParticles; }
			set
			{
				if (MinParticles > value)
					value = MinParticles + 1;  //Ensures MaxParticles is above MinParticles
				_data.MaxParticles = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ParticleFade")]
		[ProviderDescription(@"ParticleFade")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(10)]
		public int ParticalFade
		{
			get { return _data.ParticleFade; }
			set
			{
				_data.ParticleFade = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Enable Audio")]
		[ProviderDescription(@"Synchronise Fireworks to the music")]
		[PropertyOrder(11)]
		public bool EnableAudio
		{
			get { return _data.EnableAudio; }
			set
			{
				_data.EnableAudio = value;
				IsDirty = true;
				UpdateAudioAttributes();
				OnPropertyChanged();
			}
		}

		[Browsable(false)]
		public override StringOrientation StringOrientation
		{
			get
			{
				return StringOrientation.Vertical;
			}
			set
			{
				//Read only
			}
		}

		#endregion

		#region Audio

		[Value]
		[ProviderCategory(@"Audio", 2)]
		[ProviderDisplayName(@"Volume Sensitivity")]
		[ProviderDescription(@"The range of the volume levels displayed by the effect")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-200, 0, 1)]
		[PropertyOrder(0)]
		public int Sensitivity
		{
			get { return _data.Sensitivity; }
			set
			{
				_data.Sensitivity = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Audio", 2)]
		[ProviderDisplayName(@"Gain")]
		[ProviderDescription(@"Boosts the volume")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 200, .5)]
		[PropertyOrder(1)]
		public int Gain
		{
			get { return _data.Gain * 10; }
			set
			{
				_data.Gain = value / 10;
				_audioUtilities.Gain = value / 10;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Audio", 2)]
		[ProviderDescription(@"Brings the peak volume of the selected audio range to the top of the effect")]
		[PropertyOrder(3)]
		public bool Normalize
		{
			get { return _data.Normalize; }
			set
			{
				_audioUtilities.Normalize = value;
				_data.Normalize = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Audio", 2)]
		[ProviderDisplayName(@"LowPassFilter")]
		[ProviderDescription(@"Ignores frequencies above a given frequency")]
		[PropertyOrder(4)]
		public bool LowPass
		{
			get { return _data.LowPass; }
			set
			{
				_data.LowPass = value;
				_audioUtilities.LowPass = value;
				IsDirty = true;
				UpdateLowHighPassAttributes();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Audio", 2)]
		[ProviderDisplayName(@"LowPassFrequency")]
		[ProviderDescription(@"Ignore frequencies above this value")]
		[PropertyOrder(5)]
		public int LowPassFreq
		{
			get { return _data.LowPassFreq; }
			set
			{
				_data.LowPassFreq = value;
				_audioUtilities.LowPassFreq = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Audio", 2)]
		[ProviderDisplayName(@"HighPassFilter")]
		[ProviderDescription(@"Ignores frequencies below a given frequency")]
		[PropertyOrder(6)]
		public bool HighPass
		{
			get { return _data.HighPass; }
			set
			{
				_data.HighPass = value;
				_audioUtilities.HighPass = value;
				IsDirty = true;
				UpdateLowHighPassAttributes();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Audio", 2)]
		[ProviderDisplayName(@"HighPassFrequency")]
		[ProviderDescription(@"Ignore frequencies below this value")]
		[PropertyOrder(7)]
		public int HighPassFreq
		{
			get { return _data.HighPassFreq; }
			set
			{
				_data.HighPassFreq = value;
				_audioUtilities.HighPassFreq = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Audio", 2)]
		[ProviderDisplayName(@"DecayTime")]
		[ProviderDescription(@"How quickly the effect falls from a volume peak")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 5000, 300)]
		[PropertyOrder(8)]
		public int DecayTime
		{
			get { return _data.DecayTime; }
			set
			{
				_data.DecayTime = value;
				_audioUtilities.DecayTime = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Audio", 2)]
		[ProviderDisplayName(@"AttackTime")]
		[ProviderDescription(@"How quickly the effect initially reacts to a volume peak")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 300, 10)]
		[PropertyOrder(9)]
		public int AttackTime
		{
			get { return _data.AttackTime; }
			set
			{
				_data.AttackTime = value;
				_audioUtilities.AttackTime = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}


		#endregion

		#region Color

		[Value]
		[ProviderCategory(@"Color", 3)]
		[ProviderDisplayName(@"Color Type")]
		[ProviderDescription(@"Color Type")]
		[PropertyOrder(0)]
		public FireworksColorType ColorType
		{
			get { return _data.ColorType; }
			set
			{
				_data.ColorType = value;
				IsDirty = true;
				UpdateColorAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 3)]
		[ProviderDisplayName(@"Colors")]
		[ProviderDescription(@"Colors")]
		[PropertyOrder(2)]
		public List<ColorGradient> ColorGradients
		{
			get { return _data.ColorGradients; }
			set
			{
				_data.ColorGradients = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Level properties

		[Value]
		[ProviderCategory(@"Brightness", 4)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
		public Curve LevelCurve
		{
			get { return _data.LevelCurve; }
			set
			{
				_data.LevelCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as FireworksData;
				UpdateAttributes();
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		#region Update Attributes
		private void UpdateAttributes()
		{
			UpdateAudioAttributes(false);
			UpdateRandomVelocityAttribute(false);
			UpdateColorAttribute(false);
			UpdateRandomParticlesAttribute(false);
			UpdateLowHighPassAttributes(false);
			TypeDescriptor.Refresh(this);
		}

		private void UpdateRandomVelocityAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3);
			propertyStates.Add("Velocity", !RandomVelocity);
			propertyStates.Add("MaxVelocity", RandomVelocity);
			propertyStates.Add("MinVelocity", RandomVelocity);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateRandomParticlesAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3);
			propertyStates.Add("Particles", !RandomParticles);
			propertyStates.Add("MaxParticles", RandomParticles);
			propertyStates.Add("MinParticles", RandomParticles);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateEnableAudioAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3);
			propertyStates.Add("EnableAudio", _audioUtilities.AudioLoaded);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateAudioAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(10);
			propertyStates.Add("Sensitivity", EnableAudio);
			propertyStates.Add("LowPass", EnableAudio);
			propertyStates.Add("LowPassFreq", EnableAudio);
			propertyStates.Add("HighPass", EnableAudio);
			propertyStates.Add("HighPassFreq", EnableAudio);
			propertyStates.Add("Range", EnableAudio);
			propertyStates.Add("Normalize", EnableAudio);
			propertyStates.Add("DecayTime", EnableAudio);
			propertyStates.Add("AttackTime", EnableAudio);
			propertyStates.Add("Gain", EnableAudio);
			propertyStates.Add("Explosions", !EnableAudio);
			propertyStates.Add("ExplosionSensitivity", EnableAudio);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}

			UpdateLowHighPassAttributes();
		}

		//Used to hide Colors from user when Rainbow type is selected and unhides for the other types.
		private void UpdateColorAttribute(bool refresh = true)
		{
			bool fireworkType = ColorType != FireworksColorType.RainBow & ColorType != FireworksColorType.Random;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			propertyStates.Add("ColorGradients", fireworkType);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		protected void UpdateLowHighPassAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{"LowPassFreq", LowPass},
				{"HighPassFreq", HighPass}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		#endregion

		protected override void SetupRender()
		{
			foreach (IMediaModuleInstance module in Media)
			{
				var audio = module as Audio;
				if (audio != null)
				{
					if (audio.Channels == 0)
					{
						continue;
					}

					_audioUtilities.TimeSpan = TimeSpan;
					_audioUtilities.StartTime = StartTime;
					_audioUtilities.ReloadAudio(audio);
				}
			}
			UpdateEnableAudioAttribute();
			var x25 = (int)(BufferWi * 0.25);
			var x75 = (int)(BufferWi * 0.75);
			var y25 = (int)(BufferHt * 0.25);
			var y75 = (int)(BufferHt * 0.75);
			
			if (EnableAudio)
			{
				List<int> audioExplosions = new List<int>();
				//Determines frame number for each explosion to start based off the audio
				for (int i = 0; i < (int)(TimeSpan.TotalMilliseconds / Spacing); i++)
				{
					var currentValue = _audioUtilities.VolumeAtTime(i * Spacing);

					if (currentValue > ((double)Sensitivity / 10))
					{
						audioExplosions.Add(i);
						i += ExplosionSensitivity;
					}
				}
				_maxFlakes = RandomParticles ? audioExplosions.Count * MaxParticles : audioExplosions.Count * Particles;

				InitFireworksBuffer();

				for (int x = 0; x < audioExplosions.Count; x++)
				{
					_explosion = x;
					CreateExplosions(audioExplosions[x], x75, x25, y75, y25);
				}
				_explosion = audioExplosions.Count;
			}
			else
			{
				_maxFlakes = RandomParticles ? Explosions * MaxParticles : Explosions * Particles;
				InitFireworksBuffer();
				for (int x = 0; x < Explosions; x++)
				{
					_explosion = x;
					CreateExplosions(0, x75, x25, y75, y25);
				}
				_explosion = Explosions;
			}
		}

		private void CreateExplosions(int ii, int x75, int x25, int y75, int y25)
		{
			HSV hsv = new HSV();
			if (ColorType == FireworksColorType.Random)
			{
				hsv.H = (float)(Rand() % 1000) / 1000.0f;
				hsv.S = 1.0f;
				hsv.V = 1.0f;
			}
			int start = EnableAudio ? ii : (int) (Rand01()*GetNumberFrames());

			int startX;
			int startY;
			if ((x75 - x25) > 0) startX = x25 + Rand() % (x75 - x25);
			else startX = 0;
			if ((y75 - y25) > 0) startY = y25 + Rand() % (y75 - y25);
			else startY = 0;

			int colorLocation = Rand() % ColorGradients.Count;

			int randomParticles = RandomParticles ? _random.Next(MinParticles, MaxParticles) : Particles;

			for (int i = 0; i < randomParticles; i++)
			{
				int velocity = RandomVelocity ? _random.Next(MinVelocity, MaxVelocity) : Velocity;
				_fireworkBursts[_explosion * randomParticles + i].Reset(startX, startY, false, velocity, hsv, start, colorLocation);
			}
		}

		protected override void CleanUpRender()
		{
			_fireworkBursts = null;
			_audioUtilities.FreeMem();
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			if (StringCount == 1) return;

			int colorcnt = ColorGradients.Count;

			for (int i = 0; i < _maxFlakes; i++)
			{
				if (_fireworkBursts[i].StartPeriod == frame)
				{
					_fireworkBursts[i].Active = true;
				}

				// ... active flakes:
				if (_fireworkBursts[i].Active)
				{
					// Update position
					_fireworkBursts[i].X += _fireworkBursts[i].Dx;
					_fireworkBursts[i].Y +=
						(float) (-_fireworkBursts[i].Dy - _fireworkBursts[i].Cycles*_fireworkBursts[i].Cycles/10000000.0);
					// If this flake run for more than maxCycle, time to switch it off
					_fireworkBursts[i].Cycles += 20;
					if (_fireworkBursts[i].Cycles >= _maxFlakes)
					{
						_fireworkBursts[i].Active = false;
						continue;
					}
					// If this flake hit the earth or is out of bounds, time to switch it off
					if (_fireworkBursts[i].Y >= BufferHt || _fireworkBursts[i].Y < 0 || _fireworkBursts[i].X < 0 || _fireworkBursts[i].X >= BufferWi)
					{
						_fireworkBursts[i].Active = false;
					}
				}
			}

			double position = GetEffectTimeIntervalPosition(frame);
			double level = LevelCurve.GetValue(position * 100) / 100;
			for (int i = 0; i < _maxFlakes; i++)
			{
				if (_fireworkBursts[i].Active)
				{
					var v = (float)(((ParticalFade * 10.0) - _fireworkBursts[i].Cycles) / (ParticalFade * 10.0));
					if (v < 0) v = 0.0f;
					switch (ColorType)
					{
						case FireworksColorType.Range: //Random two colors are selected from the list for each Firework.
							_fireworkBursts[i].HSV =
								SetRangeColor(HSV.FromRGB(ColorGradients[Rand() % colorcnt].GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100)),
									HSV.FromRGB(ColorGradients[Rand() % colorcnt].GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100)));
							break;
						case FireworksColorType.Palette: //All colors are used
							_fireworkBursts[i].HSV = HSV.FromRGB(ColorGradients[Rand() % colorcnt].GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100));
							break;
						case FireworksColorType.RainBow: //No user colors are used for Rainbow effect.
							_fireworkBursts[i].HSV.H = (float)(Rand() % 1000) / 1000.0f;
							_fireworkBursts[i].HSV.S = 1.0f;
							_fireworkBursts[i].HSV.V = 1.0f;
							break;
						case FireworksColorType.Standard:
							_fireworkBursts[i].HSV = HSV.FromRGB(ColorGradients[_fireworkBursts[i].ColorLocation].GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100));
							break;
					}
					HSV hsv = _fireworkBursts[i].HSV;
					hsv.V = v*level;
					frameBuffer.SetPixel((int)_fireworkBursts[i].X, (int)_fireworkBursts[i].Y, hsv);
				}
			}
		}

		private void InitFireworksBuffer()
		{
			_fireworkBursts = null;
			_fireworkBursts = new List<RgbFireworks>(_maxFlakes);
			for (int burstNum = 0; burstNum < _maxFlakes; burstNum++)
			{
				RgbFireworks firework = new RgbFireworks();
				firework.Active = false;
				firework.StartPeriod = -1; //Ensures there is no false firework at pixel 0,0 on Frame 0
				_fireworkBursts.Add(firework);
			}
		}

		private int Rand()
		{
			return _random.Next();
		}

		private double Rand01()
		{
			return _random.NextDouble();
		}

		// generates a random number between Color num1 and and Color num2.
		private static float RandomRange(float num1, float num2)
		{
			double hi, lo;
			InitRandom();

			if (num1 < num2)
			{
				lo = num1;
				hi = num2;
			}
			else
			{
				lo = num2;
				hi = num1;
			}
			return (float)(_random.NextDouble() * (hi - lo) + lo);
		}

		private static void InitRandom()
		{
			if (_random == null)
				_random = new Random();
		}

		//Use for Range type
		public static HSV SetRangeColor(HSV hsv1, HSV hsv2)
		{
			HSV newHsv = new HSV(RandomRange((float)hsv1.H, (float)hsv2.H),
								 RandomRange((float)hsv1.S, (float)hsv2.S),
								 1.0f);
			return newHsv;
		}
	}
}
