using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using Vixen.Sys.State.Execution;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.ColorWash
{
	public class ColorWash : PixelEffectBase
	{
		private ColorWashData _data;
		public List<ColorGradient> WorkingColors;

		public ColorWash()
		{
			_data = new ColorWashData();
		}

		public override bool IsDirty
		{
			get
			{
				if (Colors.Any(x => !x.CheckLibraryReference()))
				{
					base.IsDirty = true;
				}

				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
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

		#region Config properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Type")]
		[ProviderDescription(@"Type")]
		[PropertyOrder(0)]
		public ColorWashType Type
		{
			get { return _data.Type; }
			set
			{
				_data.Type = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Iterations")]
		[ProviderDescription(@"Iterations")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(1)]
		public int Iterations
		{
			get { return _data.Iterations; }
			set
			{
				_data.Iterations = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Horizontal Fade")]
		[ProviderDescription(@"Horizontal Fade")]
		[PropertyOrder(4)]
		public bool HorizontalFade
		{
			get { return _data.HorizontalFade; }
			set
			{
				_data.HorizontalFade = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Vertical Fade")]
		[ProviderDescription(@"Vertical Fade")]
		[PropertyOrder(4)]
		public bool VerticalFade
		{
			get { return _data.VerticalFade; }
			set
			{
				_data.VerticalFade = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color properties


		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"ColorGradients")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(1)]
		public List<ColorGradient> Colors
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
				_data = value as ColorWashData;
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		protected override void SetupRender()
		{
			//Nothing to setup
		}

		protected override void CleanUpRender()
		{
			//Nothing to clean up
		}

		protected override void RenderEffect(int frame, ref PixelFrameBuffer frameBuffer)
		{
			double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;

			int x, y;
			WorkingColors = new List<ColorGradient>{new ColorGradient(Color.Black)};
			foreach (var colorGradient in Colors)
			{
				WorkingColors.Add(colorGradient);
			}

			HSV hsv2 = new HSV();
			var totalFrames = (TimeSpan.TotalMilliseconds / FrameTime);
			int cycleLen = ((((int)totalFrames - 1) / Iterations));
			Color color = GetMultiColorBlend((double)(frame % cycleLen) / cycleLen, frame);
			HSV hsv = HSV.FromRGB(color);
			double halfHt = (BufferHt - 1) / 2.0;
			double halfWi = (BufferWi - 1) / 2.0;
			for (x = 0; x < BufferWi; x++)
			{
				for (y = 0; y < BufferHt; y++)
				{
					hsv2 = hsv;
					switch (Type)
					{
							case ColorWashType.Centre:
								if (HorizontalFade && halfWi > 0) hsv2.V *= (float)(1.0 - Math.Abs(halfWi - x) / halfWi);
								if (VerticalFade && halfHt > 0) hsv2.V *= (float)(1.0 - Math.Abs(halfHt - y) / halfHt);
							break;
							case ColorWashType.Outer:
								if (HorizontalFade && halfWi > 0) hsv2.V *= (float)(Math.Abs(halfWi - x) / halfWi);
								if (VerticalFade && halfHt > 0) hsv2.V *= (float)(Math.Abs(halfHt - y) / halfHt);
							break;
							case ColorWashType.Invert:
								if (HorizontalFade && halfWi > 0) hsv2.V /= (float)(1 - Math.Abs(halfWi - x) / halfWi);
								if (VerticalFade && halfHt > 0) hsv2.V /= (float)(1 - Math.Abs(halfHt - y) / halfHt);
							break;
					}
					hsv2.V *= level;
					frameBuffer.SetPixel(x, y, hsv2);
				}
			}
		}

		private int ChannelBlend(int c1, int c2, double ratio)
		{
			return c1 + (int) Math.Floor(ratio* (c2 - c1) + 0.5);
		}

		public Color Get2ColorBlend(int coloridx1, int coloridx2, double ratio, int frame)
		{
			Color c1, c2;
			c1 = WorkingColors[coloridx1].GetColorAt((GetEffectTimeIntervalPosition(frame)*100)/100);
			c2 = WorkingColors[coloridx2].GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100);

			return Color.FromArgb(ChannelBlend(c1.R, c2.R, ratio), ChannelBlend(c1.G, c2.G, ratio),
								  ChannelBlend(c1.B, c2.B, ratio));
		}

		public Color GetMultiColorBlend(double n, int frame)
		{
			int colorcnt = WorkingColors.Count();
			if (n >= 1.0) n = 0.99999;
			if (n < 0.0) n = 0.0;
			double realidx = n * colorcnt;
			int coloridx1 = (int)Math.Floor(realidx);
			int coloridx2 = (coloridx1 + 1) % colorcnt;
			double ratio = realidx - coloridx1;
			return Get2ColorBlend(coloridx1, coloridx2, ratio, frame);
		}
	}
}
