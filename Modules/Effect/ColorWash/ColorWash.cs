using System;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.ColorWash
{
	public class ColorWash : PixelEffectBase
	{
		private ColorWashData _data;
		private double _halfHt;
		private double _halfWi;

		public ColorWash()
		{
			_data = new ColorWashData();
			EnableTargetPositioning(true, true);
			InitAllAttributes();
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
				InitAllAttributes();
				IsDirty = true;
			}
		}

		private void InitAllAttributes()
		{
			UpdateStringOrientationAttributes(true);
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		protected override void SetupRender()
		{
			_halfHt = (BufferHt - 1) / 2.0;
			_halfWi = (BufferWi - 1) / 2.0;
		}

		protected override void CleanUpRender()
		{
			//Nothing to clean up
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
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
			for (int x = 0; x < BufferWi; x++)
			{
				for (int y = 0; y < BufferHt; y++)
				{
					var v = hsv.V;
					v = CalculateAdjustedV(v, x, y);
					v *= level;
					HSV hsv2 = hsv;
					hsv2.V = v; 
					frameBuffer.SetPixel(x, y, hsv2);
				}
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
			for (int frame = 0; frame < numFrames; frame++)
			{
				double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
				var iterationFrame = frame * Iterations % (numFrames);
				double position = GetEffectTimeIntervalPosition(iterationFrame);

				HSV hsv = HSV.FromRGB(Color.GetColorAt(position));
				if (Shimmer && frame % 2 != 0)
				{
					hsv.V = 0;
				}
				
				foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
				{
					foreach (var elementLocation in elementLocations)
					{
						var v = hsv.V;
						//Here we offset our x, y values to get us to zero based coordinates for our math to work out
						//Our effect is symetrical so we don't need to waste time flipping the coordinates around
						v = CalculateAdjustedV(v, elementLocation.X-BufferWiOffset, elementLocation.Y-BufferHtOffset);
						v *= level;
						HSV hsv2 = hsv;
						hsv2.V = v;
						frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, hsv2);
					}
				}
			}
		}

		private double CalculateAdjustedV(double v, int x, int y)
		{
			switch (Type)
			{
				case ColorWashType.Center:
					if (HorizontalFade && _halfWi > 0)
					{
						v *= (float) (1.0 - Math.Abs(_halfWi - x)/_halfWi);
					}
					if (VerticalFade && _halfHt > 0)
					{
						v *= (float) (1.0 - Math.Abs(_halfHt - y)/_halfHt);
					}
					break;
				case ColorWashType.Outer:
					if (HorizontalFade && _halfWi > 0)
					{
						v *= (float) (Math.Abs(_halfWi - x)/_halfWi);
					}
					if (VerticalFade && _halfHt > 0)
					{
						v *= (float) (Math.Abs(_halfHt - y)/_halfHt);
					}
					break;
				case ColorWashType.Invert:
					if (HorizontalFade && _halfWi > 0)
					{
						v /= (float) (1 - Math.Abs(_halfWi - x)/_halfWi);
					}
					if (VerticalFade && _halfHt > 0)
					{
						v /= (float) (1 - Math.Abs(_halfHt - y)/_halfHt);
					}
					break;
			}
			return v;
		}

		
	}
}
