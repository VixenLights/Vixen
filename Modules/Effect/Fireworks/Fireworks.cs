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
		private const int MaxFlakes = 1000;
		
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
			
			var mod100 = (Speed*frame % (101 - Explosions) * 20);

			HSV hsv;
			if (mod100 == 0)
			{
				var x25 = (int)(BufferWi * 0.25);
				var x75 = (int)(BufferWi * 0.75);
				var y25 = (int)(BufferHt * 0.25);
				var y75 = (int)(BufferHt * 0.75);
				var startX = x25 + (Rand() % (x75 - x25));
				var startY = y25 + (Rand() % (y75 - y25)); 

				// Create new bursts
				hsv = Colors.Count > 0 ? HSV.FromRGB(Colors[Rand() % Colors.Count]) : HSV.FromRGB(Color.White);
				int idxFlakes = 0;
				for (int i = 0; i < Particles; i++)
				{
					do
					{
						idxFlakes = (idxFlakes + 1) % MaxFlakes;
					} while (_fireworkBursts[idxFlakes].Active);
					_fireworkBursts[idxFlakes].Reset(startX, startY, true, Velocity, hsv);
				}
			}
			else
			{
				for (int i = 0; i < MaxFlakes; i++)
				{
					// ... active flakes:
					if (_fireworkBursts[i].Active)
					{
						// Update position
						_fireworkBursts[i].X += _fireworkBursts[i].Dx;
						_fireworkBursts[i].Y +=
							(float)(-_fireworkBursts[i].Dy - _fireworkBursts[i].Cycles * _fireworkBursts[i].Cycles / 10000000.0);
						// If this flake run for more than maxCycle, time to switch it off
						_fireworkBursts[i].Cycles += 20;
						if (10000 == _fireworkBursts[i].Cycles) // if (10000 == _fireworkBursts[i]._cycles)
						{
							_fireworkBursts[i].Active = false;
							continue;
						}
						// If this flake hit the earth, time to switch it off
						if (_fireworkBursts[i].Y >= BufferHt)
						{
							_fireworkBursts[i].Active = false;
							continue;
						}
						// Draw the flake, if its X-pos is within frame
						if (_fireworkBursts[i].X >= 0.0 && _fireworkBursts[i].X < BufferWi)
						{
							// But only if it is "under" the roof!
							if (_fireworkBursts[i].Y >= 0.0)
							{
								// sean we need to set color here
							}
						}
						else
						{
							// otherwise it just got outside the valid X-pos, so switch it off
							_fireworkBursts[i].Active = false;
						}
					}
				}
			}
			double position = GetEffectTimeIntervalPosition(frame);
			double level = LevelCurve.GetValue(position * 100) / 100;
			for (int i = 0; i < 1000; i++)
			{
				if (_fireworkBursts[i].Active)
				{
					var v = (float)(((ParticalFade * 10.0) - _fireworkBursts[i].Cycles) / (ParticalFade * 10.0));
					if (v < 0) v = 0.0f;
					hsv = _fireworkBursts[i].HSV;
					hsv.V = v*level;
					frameBuffer.SetPixel((int)_fireworkBursts[i].X, (int)_fireworkBursts[i].Y, hsv);
				}
			}
			
				
		}

		private void InitFireworksBuffer()
		{
			if (_fireworkBursts == null)
			{
				_fireworkBursts = new List<RgbFireworks>(20000);
				for (int burstNum = 0; burstNum < 20000; burstNum++)
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

		
	}
}
