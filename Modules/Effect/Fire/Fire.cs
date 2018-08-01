using System;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Fire
{
	public class Fire:PixelEffectBase
	{
		private FireData _data;
		private int[] _fireBuffer = new int[1];
		private readonly Random _random = new Random();
		
		public Fire()
		{
			_data = new FireData();
		}

		#region Setup

		[Value]
		public override StringOrientation StringOrientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Config

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Location")]
		[ProviderDescription(@"Location")]
		[PropertyOrder(0)]
		public FireDirection Location
		{
			get { return _data.Location; }
			set
			{
				_data.Location = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Height")]
		[ProviderDescription(@"Height")]
		[PropertyOrder(1)]
		public Curve Height
		{
			get { return _data.Height; }
			set
			{
				_data.Height = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"HueShift")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(1)]
		public Curve HueShiftCurve
		{
			get { return _data.HueShiftCurve; }
			set
			{
				_data.HueShiftCurve = value;
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

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/fire/"; }
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as FireData;
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		// 0 <= x < BufferWi
		// 0 <= y < BufferHt
		private int GetFireBuffer(int x, int y, int maxWi, int maxHt)
		{
			if (x >= 0 && x < maxWi && y >= 0 && y < maxHt)
			{
				return _fireBuffer[y * maxWi + x];
			}
			return -1;
		}

		private int Rand()
		{
			return _random.Next();
		}

		protected override void SetupRender()
		{
			_fireBuffer = new int[BufferWi*BufferHt];
		}

		protected override void CleanUpRender()
		{
			_fireBuffer = null;
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			double intervalPosFactor = GetEffectTimeIntervalPosition(frame) * 100;
			double level = LevelCurve.GetValue(intervalPosFactor) / 100;
			double hueShift = CalculateHueShift(intervalPosFactor);

			int x, y;

			int maxHt = BufferHt;
			int maxWi = BufferWi;
			if (Location == FireDirection.Left || Location == FireDirection.Right)
			{
				maxHt = BufferWi;
				maxWi = BufferHt;
			}

			// build fire
			for (x = 0; x < maxWi; x++)
			{
				var r = x % 2 == 0 ? 190 + (Rand() % 10) : 100 + (Rand() % 50);
				_fireBuffer[x] = r;
			}

			int h = (int)Height.GetValue(intervalPosFactor);

			if(h <= 0)
			{
				h = 1;
			}
			int step = 255 * 100 / maxHt / h;

			for (y = 1; y < maxHt; y++)
			{
				for (x = 0; x < maxWi; x++)
				{
					var v1 = GetFireBuffer(x - 1, y - 1, maxWi, maxHt);
					var v2 = GetFireBuffer(x + 1, y - 1, maxWi, maxHt);
					var v3 = GetFireBuffer(x, y - 1, maxWi, maxHt);
					var v4 = GetFireBuffer(x, y - 1, maxWi, maxHt);
					var n = 0;
					var sum = 0;
					if (v1 >= 0)
					{
						sum += v1;
						n++;
					}
					if (v2 >= 0)
					{
						sum += v2;
						n++;
					}
					if (v3 >= 0)
					{
						sum += v3;
						n++;
					}
					if (v4 >= 0)
					{
						sum += v4;
						n++;
					}
					var newIndex = n > 0 ? sum / n : 0;
					if (newIndex > 0)
					{
						newIndex += (Rand() % 100 < 20) ? step : -step;
						if (newIndex < 0) newIndex = 0;
						if (newIndex >= FirePalette.Count()) newIndex = FirePalette.Count() - 1;
					}
					_fireBuffer[y * maxWi + x] = newIndex;
				}
			}

			for (y = 0; y < maxHt; y++)
			{
				for (x = 0; x < maxWi; x++)
				{
					var colorIndex = GetFireBuffer(x, y, maxWi, maxHt);
					if (colorIndex == 0) continue; // No point going any further if color index is 0 (Black). Significantly reduces render time.
					
					int xp = x;
					int yp = y;
					if (Location == FireDirection.Top || Location == FireDirection.Right)
					{
						yp = maxHt - y - 1;
					}
					if (Location == FireDirection.Left || Location == FireDirection.Right)
					{
						int t = xp;
						xp = yp;
						yp = t;
					}
					HSV hsv = FirePalette.GetColor(colorIndex);
					if (hueShift > 0) hsv.H = hsv.H + hueShift / 100.0f;

					hsv.V *= level;
					
					frameBuffer.SetPixel(xp, yp, hsv);
				}
			}
		}

		private double CalculateHueShift(double intervalPos)
		{
			return ScaleCurveToValue(HueShiftCurve.GetValue(intervalPos), 100, 0);
		}
		
	}
}
