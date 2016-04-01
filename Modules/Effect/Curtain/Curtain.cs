using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Pixel;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Curtain
{
	public class Curtain : PixelEffectBase
	{
		private CurtainData _data;
		private int _lastCurtainDir;
		private int _lastCurtainLimit;

		public Curtain()
		{
			_data = new CurtainData();
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
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[PropertyOrder(0)]
		public CurtainDirection Direction
		{
			get { return _data.Direction; }
			set
			{
				_data.Direction = value;
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

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Edge")]
		[ProviderDescription(@"Edge")]
		[PropertyOrder(3)]
		public CurtainEdge Edge
		{
			get { return _data.Edge; }
			set
			{
				_data.Edge = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Swag")]
		[ProviderDescription(@"Swag")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 10, 1)]
		[PropertyOrder(4)]
		public int Swag
		{
			get { return _data.Swag; }
			set
			{
				_data.Swag = value;
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
				_data = value as CurtainData;
				IsDirty = true;
			}
		}

		protected override void SetupRender()
		{
			if (Direction == CurtainDirection.CurtainOpen || Direction == CurtainDirection.CurtainOpenClose)
			{
				_lastCurtainDir = 1;
			}
			else
			{
				_lastCurtainDir = 0;
			}
		}

		protected override void CleanUpRender()
		{
			//Nothing to clean up
		}

		protected override void RenderEffect(int frame, ref PixelFrameBuffer frameBuffer)
		{
			var swagArray = new List<int>();
			int curtainDir, xlimit, middle, ylimit;
			int swaglen = BufferHt > 1 ? Swag*BufferWi/40 : 0;
			double position = (GetEffectTimeIntervalPosition(frame)*Speed)%1;

			if (swaglen > 0)
			{
				double a = (double) (BufferHt - 1)/(swaglen*swaglen);
				for (int x = 0; x < swaglen; x++)
				{
					swagArray.Add((int) (a*x*x));
				}
			}
			if (Direction < CurtainDirection.CurtainOpenClose)
			{
				xlimit = (int) (position*BufferWi);
				ylimit = (int) (position*BufferHt);
			}
			else
			{
				xlimit = (int) (position <= .5 ? position*2*BufferWi : (position - .5)*2*BufferWi);
				ylimit = (int) (position <= .5 ? position*2*BufferHt : (position - .5)*2*BufferHt);
			}
			if (Direction < CurtainDirection.CurtainOpenClose)
			{
				curtainDir = (int)Direction % 2;
			}
			else if (xlimit < _lastCurtainLimit)
			{
				curtainDir = 1 - _lastCurtainDir;
			}
			else
			{
				curtainDir = _lastCurtainDir;
			}
			_lastCurtainDir = curtainDir;
			_lastCurtainLimit = xlimit;
			if (curtainDir == 0)
			{
				xlimit = BufferWi - xlimit - 1;
				ylimit = BufferHt - ylimit - 1;
			}
			switch (Edge)
			{
				case CurtainEdge.Left:
					// left
					DrawCurtain(true, xlimit, swagArray, frameBuffer, frame);
					break;
				case CurtainEdge.Center:
					// center
					middle = (xlimit + 1)/2;
					DrawCurtain(true, middle, swagArray, frameBuffer, frame);
					DrawCurtain(false, middle, swagArray, frameBuffer, frame);
					break;
				case CurtainEdge.Right:
					// right
					DrawCurtain(false, xlimit, swagArray, frameBuffer, frame);
					break;
				case CurtainEdge.Bottom:

					// bottom
					DrawCurtainVertical(true, ylimit, swagArray, frameBuffer, frame);
					break;
				case CurtainEdge.Middle:
					// middle
					middle = (ylimit + 1)/2;
					DrawCurtainVertical(true, middle, swagArray, frameBuffer, frame);
					DrawCurtainVertical(false, middle, swagArray, frameBuffer, frame);
					break;
				case CurtainEdge.Top:
					// top
					DrawCurtainVertical(false, ylimit, swagArray, frameBuffer, frame);
					break;
			}
		}

		private void DrawCurtain(bool leftEdge, int xlimit, List<int> swagArray, PixelFrameBuffer frameBuffer, int frame)
		{
			int i, x, y;
			Color color;
			for (i = 0; i < xlimit; i++)
			{
				color = GetMultiColorBlend((double)i / BufferWi, frame);
				x = leftEdge ? BufferWi - i - 1 : i;
				for (y = BufferHt - 1; y >= 0; y--)
				{
					HSV hsv = HSV.FromRGB(color);
					hsv.V = hsv.V*LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame)*100)/100;
					frameBuffer.SetPixel(x, y, hsv);
				}
			}

			// swag
			for (i = 0; i < swagArray.Count; i++)
			{
				x = xlimit + i;
				color = GetMultiColorBlend((double)x / BufferWi, frame);
				if (leftEdge) x = BufferWi - x - 1;
				for (y = BufferHt - 1; y > swagArray[i]; y--)
				{
					HSV hsv = HSV.FromRGB(color);
					hsv.V = hsv.V*LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame)*100)/100;
					frameBuffer.SetPixel(x, y, hsv);
				}
			}
		}

		private void DrawCurtainVertical(bool topEdge, int ylimit, List<int> swagArray, PixelFrameBuffer frameBuffer,
			int frame)
		{
			int i, x, y;
			Color color;
			for (i = 0; i < ylimit; i++)
			{
				color = GetMultiColorBlend(((double)i / BufferHt), frame);
				y = topEdge ? BufferHt - i - 1 : i;
				for (x = BufferWi - 1; x >= 0; x--)
				{
					HSV hsv = HSV.FromRGB(color);
					hsv.V = hsv.V*LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame)*100)/100;
					frameBuffer.SetPixel(x, y, hsv);
				}
			}

			// swag
			for (i = 0; i < swagArray.Count(); i++)
			{
				y = ylimit + i;
				color = GetMultiColorBlend(((double)y / BufferHt), frame);
				if (topEdge) y = BufferHt - y - 1;
				for (x = BufferWi - 1; x > swagArray[i]; x--)
				{
					HSV hsv = HSV.FromRGB(color);
					hsv.V = hsv.V*LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame)*100)/100;
					frameBuffer.SetPixel(x, y, hsv);
				}
			}
		}

		public Color GetMultiColorBlend(double n, int frame)
		{
			int colorcnt = Colors.Count();
			if (colorcnt <= 1)
			{
				return Colors[0].GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100);
			}
			if (n >= 1.0) n = 0.99999;
			if (n < 0.0) n = 0.0;
			int coloridx1 = (int) Math.Floor(n*colorcnt);
			int coloridx2 = (coloridx1 + 1)%colorcnt;
			double ratio = n * colorcnt - coloridx1;
			return Get2ColorBlend(coloridx1, coloridx2, ratio, frame);
		}

		public Color Get2ColorBlend(int coloridx1, int coloridx2, double ratio, int frame)
		{
			Color c1, c2;
			c1 = Colors[coloridx1].GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100);
			c2 = Colors[coloridx2].GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100);

			return Color.FromArgb(ChannelBlend(c1.R, c2.R, ratio), ChannelBlend(c1.G, c2.G, ratio), ChannelBlend(c1.B, c2.B, ratio));
		}

		private int ChannelBlend(int c1, int c2, double ratio)
		{
			return c1 + (int) Math.Floor(ratio*(double) (c2 - c1) + 0.5);
		}
	}
}