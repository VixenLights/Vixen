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
		
		public ColorWash()
		{
			_data = new ColorWashData();
		}

		//public override bool IsDirty
		//{
		//	get
		//	{
		//		if (Color.CheckLibraryReference())
		//		{
		//			base.IsDirty = true;
		//		}

		//		return base.IsDirty;
		//	}
		//	protected set { base.IsDirty = value; }
		//}

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

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Shimmer")]
		[ProviderDescription(@"Shimmer")]
		[PropertyOrder(5)]
		public bool Shimmer
		{
			get { return _data.Shimmer; }
			set
			{
				_data.Shimmer = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color properties


		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"ColorGradient")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(2)]
		public ColorGradient Color
		{
			get { return _data.Gradient; }
			set
			{
				_data.Gradient = value;
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
			if (Shimmer && frame%2 != 0)
			{
				return;
			}
			double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
			int totalFrames = GetNumberFrames();
			var iterationFrame = frame*Iterations%(totalFrames);
			double position = GetEffectTimeIntervalPosition(iterationFrame);
			HSV hsv = HSV.FromRGB(Color.GetColorAt(position));
			double halfHt = (BufferHt - 1) / 2.0;
			double halfWi = (BufferWi - 1) / 2.0;
			for (int x = 0; x < BufferWi; x++)
			{
				for (int y = 0; y < BufferHt; y++)
				{
					var v = hsv.V;
					switch (Type)
					{
							case ColorWashType.Center:
								if (HorizontalFade && halfWi > 0)
								{
									v *= (float)(1.0 - Math.Abs(halfWi - x) / halfWi);
								}
								if (VerticalFade && halfHt > 0)
								{
									v *= (float)(1.0 - Math.Abs(halfHt - y) / halfHt);
								}
								break;
							case ColorWashType.Outer:
								if (HorizontalFade && halfWi > 0)
								{
									v *= (float)(Math.Abs(halfWi - x) / halfWi);
								}
								if (VerticalFade && halfHt > 0)
								{
									v *= (float)(Math.Abs(halfHt - y) / halfHt);
								}
								break;
							case ColorWashType.Invert:
								if (HorizontalFade && halfWi > 0)
								{
									v /= (float)(1 - Math.Abs(halfWi - x) / halfWi);
								}
								if (VerticalFade && halfHt > 0)
								{
									v /= (float)(1 - Math.Abs(halfHt - y) / halfHt);
								}
								break;
					}
					v *= level;
					HSV hsv2 = hsv;
					hsv2.V = v; 
					frameBuffer.SetPixel(x, y, hsv2);
				}
			}
		}

		//private int ChannelBlend(int c1, int c2, double ratio)
		//{
		//	return c1 + (int) Math.Floor(ratio* (c2 - c1) + 0.5);
		//}

		//public Color Get2ColorBlend(int coloridx1, int coloridx2, double ratio, int frame)
		//{
		//	Color c1, c2;
		//	c1 = Colors[coloridx1].GetColorAt((GetEffectTimeIntervalPosition(frame)*100)/100);
		//	c2 = Colors[coloridx2].GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100);

		//	return Color.FromArgb(ChannelBlend(c1.R, c2.R, ratio), ChannelBlend(c1.G, c2.G, ratio),
		//						  ChannelBlend(c1.B, c2.B, ratio));
		//}

		//public Color GetMultiColorBlend(double n, int frame)
		//{
		//	int colorcnt = Colors.Count();
		//	if (n >= 1.0) n = 0.99999;
		//	if (n < 0.0) n = 0.0;
		//	double realidx = n * colorcnt;
		//	int coloridx1 = (int)Math.Floor(realidx);
		//	int coloridx2 = (coloridx1 + 1) % colorcnt;
		//	double ratio = realidx - coloridx1;
		//	return Get2ColorBlend(coloridx1, coloridx2, ratio, frame);
		//}
	}
}
