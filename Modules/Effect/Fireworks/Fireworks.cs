using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.Curves;
using VixenModules.Effect.Pixel;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Fireworks
{
	public class Fireworks:PixelEffectBase
	{
		private FireworksData _data;
		private List<RgbFireworks> _fireworkBursts;
		private readonly Random _random = new Random();
		private const int MaxFlakes = 10000;
		
		public Fireworks()
		{
			_data = new FireworksData();
		}
		
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
		[ProviderDisplayName(@"Velocity")]
		[ProviderDescription(@"Velocity")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 10, 1)]
		[PropertyOrder(1)]
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
		[ProviderDisplayName(@"Particles")]
		[ProviderDescription(@"Particles")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(2)]
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
		[ProviderDisplayName(@"ParticleFade")]
		[ProviderDescription(@"ParticleFade")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(3)]
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
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(1)]
		public int Speed
		{
			get { return _data.Speed; }
			set
			{
				_data.Speed = value;
				IsDirty = true;
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

		#region Color

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"Colors")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(1)]
		public List<Color> Colors
		{
			get { return _data.Colors; }
			set
			{
				_data.Colors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Level properties

		[Value]
		[ProviderCategory(@"Brightness", 3)]
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
				IsDirty = true;
			}
		}

		protected override void SetupRender()
		{
			InitFireworksBuffer();
			ResetBurstBuffer();

			var x25 = (int)(BufferWi * 0.25);
			var x75 = (int)(BufferWi * 0.75);
			var y25 = (int)(BufferHt * 0.25);
			var y75 = (int)(BufferHt * 0.75);

			ResetBurstBuffer();

			// Create new bursts
			for (int x = 0; x < Explosions; x++)
			{
				int start =  (int)(Rand01() * GetNumberFrames());

				var startX = x25 + (Rand() % (x75 - x25));
				var startY = y25 + (Rand() % (y75 - y25));
				if ((x75 - x25) > 0) startX = x25 + Rand() % (x75 - x25); else startX = 0;
				if ((y75 - y25) > 0) startY = y25 + Rand() % (y75 - y25); else startY = 0;

				HSV hsv = Colors.Count > 0 ? HSV.FromRGB(Colors[Rand() % Colors.Count]) : HSV.FromRGB(Color.White);

				for (int i = 0; i < Particles; i++)
				{
					_fireworkBursts[x * Particles + i].Reset(startX, startY, false, Velocity, hsv, start);
				}
			}
		}

		private void ResetBurstBuffer()
		{
			for (int i = 0; i < MaxFlakes; i++)
			{
				_fireworkBursts[i].Active = false;
			}
		}

		protected override void CleanUpRender()
		{
			_fireworkBursts = null;
		}

		protected override void RenderEffect(int frame, ref PixelFrameBuffer frameBuffer)
		{
			if (StringCount == 1) return;

			for (int i = 0; i < Particles*Explosions; i++)
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
					if (_fireworkBursts[i].Cycles >= MaxFlakes) // if (10000 == _fireworkBursts[i]._cycles)
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
			for (int i = 0; i < MaxFlakes; i++)
			{
				if (_fireworkBursts[i].Active)
				{
					var v = (float)(((ParticalFade * 10.0) - _fireworkBursts[i].Cycles) / (ParticalFade * 10.0));
					if (v < 0) v = 0.0f;
					HSV hsv = _fireworkBursts[i].HSV;
					hsv.V = v*level;
					frameBuffer.SetPixel((int)_fireworkBursts[i].X, (int)_fireworkBursts[i].Y, hsv);
				}
			}
			
				
		}

		private void InitFireworksBuffer()
		{
			if (_fireworkBursts == null)
			{
				_fireworkBursts = new List<RgbFireworks>(MaxFlakes);
				for (int burstNum = 0; burstNum < MaxFlakes; burstNum++)
				{
					RgbFireworks firework = new RgbFireworks();
					_fireworkBursts.Add(firework);
				}
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
	}
}
