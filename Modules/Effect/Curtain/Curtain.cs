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
using VixenModules.Effect.Effect;
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
				_data = value as CurtainData;
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		protected override void SetupRender()
		{
			if (Direction == CurtainDirection.CurtainOpen || Direction == CurtainDirection.CurtainOpenClose)
			{
				_lastCurtainDir = 0;
			}
			else
			{
				_lastCurtainDir = 1;
			}
		}

		protected override void CleanUpRender()
		{
			//Nothing to clean up
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			var swagArray = new List<int>();
			int curtainDir, xlimit, middle, ylimit;
			int swaglen = BufferHt > 1 ? Swag*BufferWi/40 : 0;
			double position = (GetEffectTimeIntervalPosition(frame)*Speed)%1;
			double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;

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
					DrawCurtain(true, xlimit, swagArray, frameBuffer, level, (BufferWi- 1));
					break;
				case CurtainEdge.Center:
					// center
					middle = (xlimit + 1)/2;
					DrawCurtain(true, middle, swagArray, frameBuffer, level, (BufferWi / 2 - 1));
					DrawCurtain(false, middle, swagArray, frameBuffer, level, (BufferWi / 2 - 1));
					break;
				case CurtainEdge.Right:
					// right
					DrawCurtain(false, xlimit, swagArray, frameBuffer, level, (BufferWi - 1));
					break;
				case CurtainEdge.Bottom:

					// bottom
					DrawCurtainVertical(true, ylimit, swagArray, frameBuffer, level, (BufferHt - 1));
					break;
				case CurtainEdge.Middle:
					// middle
					middle = (ylimit + 1)/2;
					DrawCurtainVertical(true, middle, swagArray, frameBuffer, level, (BufferHt / 2 - 1));
					DrawCurtainVertical(false, middle, swagArray, frameBuffer, level, (BufferHt / 2 - 1));
					break;
				case CurtainEdge.Top:
					// top
					DrawCurtainVertical(false, ylimit, swagArray, frameBuffer, level, (BufferHt - 1));
					break;
			}
		}

		private void DrawCurtain(bool leftEdge, int xlimit, List<int> swagArray, IPixelFrameBuffer frameBuffer, double level, int width)
		{
			int i, x, y;
			for (i = 0; i < xlimit; i++)
			{
				HSV hsv = HSV.FromRGB(Color.GetColorAt((double)i / width));
				x = leftEdge ? BufferWi - i - 1 : i;
				for (y = BufferHt - 1; y >= 0; y--)
				{
					hsv.V = hsv.V*level;
					frameBuffer.SetPixel(x, y, hsv);
				}
			}

			// swag
			for (i = 0; i < swagArray.Count; i++)
			{
				x = xlimit + i;
				HSV hsv = HSV.FromRGB(Color.GetColorAt((double)x / width));
				if (leftEdge) x = BufferWi - x - 1;
				for (y = BufferHt - 1; y > swagArray[i]; y--)
				{
					hsv.V = hsv.V*level;
					frameBuffer.SetPixel(x, y, hsv);
				}
			}
		}

		private void DrawCurtainVertical(bool topEdge, int ylimit, List<int> swagArray, IPixelFrameBuffer frameBuffer,
			double level, int width)
		{
			int i, x, y;
			for (i = 0; i < ylimit; i++)
			{
				HSV hsv = HSV.FromRGB(Color.GetColorAt((double)i / width));
				y = topEdge ? BufferHt - i - 1 : i;
				for (x = BufferWi - 1; x >= 0; x--)
				{
					hsv.V = hsv.V*level;
					frameBuffer.SetPixel(x, y, hsv);
				}
			}

			// swag
			for (i = 0; i < swagArray.Count(); i++)
			{
				y = ylimit + i;
				HSV hsv = HSV.FromRGB(Color.GetColorAt((double)y / width));
				if (topEdge) y = BufferHt - y - 1;
				for (x = BufferWi - 1; x > swagArray[i]; x--)
				{
					hsv.V = hsv.V*level;
					frameBuffer.SetPixel(x, y, hsv);
				}
			}
		}

		
	}
}