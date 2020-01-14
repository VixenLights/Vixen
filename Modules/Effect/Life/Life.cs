using System;
using System.Collections.Generic;
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

namespace VixenModules.Effect.Life
{
	public class Life : PixelEffectBase
	{
		private LifeData _data;
		private long _lastLifeState = 0;
		private List<List<Color>> _tempbuf;

		public Life()
		{
			_data = new LifeData();
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
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		//[NumberRange(1, 20, 1)]
		[PropertyOrder(1)]
		public Curve SpeedCurve
		{
			get { return _data.SpeedCurve; }
			set
			{
				_data.SpeedCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Cells To Start")]
		[ProviderDescription(@"Cells To Start")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(3)]
		public int CellsToStart
		{
			get { return _data.CellsToStart; }
			set
			{
				_data.CellsToStart = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Type")]
		[ProviderDescription(@"Type")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 4, 1)]
		[PropertyOrder(4)]
		public int Type
		{
			get { return _data.Type; }
			set
			{
				_data.Type = value;
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

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/life/"; }
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as LifeData;
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		protected override void SetupRender()
		{
			_tempbuf = new List<List<Color>>();

			for (int width = 0; width < BufferWi; width++)
			{
				List<Color> column = new List<Color>();
				_tempbuf.Add(column);
				for (int height = 0; height < BufferHt; height++)
				{
					_tempbuf[width].Add(Color.Transparent);
				}
			}
		}

		protected override void CleanUpRender()
		{
			_tempbuf = null;
		}

		private int Life_CountNeighbors(int x0, int y0)
		{
			//     2   3   4
			//     1   X   5
			//     0   7   6
			int[] n_x = { -1, -1, -1, 0, 1, 1, 1, 0 };
			int[] n_y = { -1, 0, 1, 1, 1, 0, -1, -1 };
			int x, y, cnt = 0;
			for (int i = 0; i < 8; i++)
			{
				x = (x0 + n_x[i]) % BufferWi;
				y = (y0 + n_y[i]) % BufferHt;
				if (x < 0) x += BufferWi;
				if (y < 0) y += BufferHt;
				if ((GetTempPixel(x, y) != Color.Transparent)) cnt++;
			}
			return cnt;
		}

		// use tempbuf for calculations
		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer) 
		{
			double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
			var intervalPos = GetEffectTimeIntervalPosition(frame);
			var intervalPosFactor = intervalPos * 100;
			int i, x, y;
			Color color;
			double state = frame * CalculateSpeed(intervalPosFactor);
			int count = BufferWi * BufferHt * CellsToStart / 200 + 1;
			if (frame == 0)
			{
				//ClearTempBuf();
				for (i = 0; i < count; i++)
				{
					x = Rand() % BufferWi;
					y = Rand() % BufferHt;
					color = GetMultiColorBlend(RandDouble(), frame);
					if (level < 1)
					{
						HSV hsv = HSV.FromRGB(color);
						hsv.V = hsv.V * level;
						color = hsv.ToRGB();
					}
					_tempbuf[x][y] = color;
				}
			}
			long tempState = (long)(state % 400) / 20;
			if (tempState == _lastLifeState)
			{
				//Pixels=tempbuf;
				CopyTempBufToPixels(frameBuffer);
				return;
			}
			_lastLifeState = tempState;

			for (x = 0; x < BufferWi; x++)
			{
				for (y = 0; y < BufferHt; y++)
				{
					color = GetTempPixel(x, y);
					//isLive=(color.GetRGB() != 0);
					bool isLive = (color != Color.Black && color != Color.Transparent);
					int cnt = Life_CountNeighbors(x, y);
					switch (Type)
					{
						case 0:
							// B3/S23
							/*
                        Any live cell with fewer than two live neighbours dies, as if caused by under-population.
                        Any live cell with two or three live neighbours lives on to the next generation.
                        Any live cell with more than three live neighbours dies, as if by overcrowding.
                        Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
                        */
							if (isLive && cnt >= 2 && cnt <= 3)
							{
								SetFramePixel(frameBuffer, color, level, x, y);
							}
							else if (!isLive && cnt == 3)
							{
								color = GetMultiColorBlend(RandDouble(), frame);
								SetFramePixel(frameBuffer, color, level, x, y);
							}
							break;
						case 1:
							// B35/S236
							if (isLive && (cnt == 2 || cnt == 3 || cnt == 6))
							{
								color = GetMultiColorBlend(RandDouble(), frame);
								SetFramePixel(frameBuffer, color, level, x, y);
							}
							else if (!isLive && (cnt == 3 || cnt == 5))
							{
								color = GetMultiColorBlend(RandDouble(), frame);
								SetFramePixel(frameBuffer, color, level, x, y);
							}
							break;
						case 2:
							// B357/S1358
							if (isLive && (cnt == 1 || cnt == 3 || cnt == 5 || cnt == 8))
							{
								SetFramePixel(frameBuffer, color, level, x, y);
							}
							else if (!isLive && (cnt == 3 || cnt == 5 || cnt == 7))
							{
								color = GetMultiColorBlend(RandDouble(), frame);
								SetFramePixel(frameBuffer, color, level, x, y);
							}
							break;
						case 3:
							// B378/S235678
							if (isLive && (cnt == 2 || cnt == 3 || cnt >= 5))
							{
								SetFramePixel(frameBuffer, color, level, x, y);
							}
							else if (!isLive && (cnt == 3 || cnt == 7 || cnt == 8))
							{
								color = GetMultiColorBlend(RandDouble(), frame);
								SetFramePixel(frameBuffer, color, level, x, y);
							}
							break;
						case 4:
							// B25678/S5678
							if (isLive && (cnt >= 5))
							{
								SetFramePixel(frameBuffer, color, level, x, y);
							}
							else if (!isLive && (cnt == 2 || cnt >= 5))
							{
								color = GetMultiColorBlend(RandDouble(), frame);
								SetFramePixel(frameBuffer, color, level, x, y);
							}
							break;
					}
				}
			}
			// copy new life state to tempbuf
			CopyPixelsToTempBuf(frameBuffer);
		}

		private void SetFramePixel(IPixelFrameBuffer frameBuffer, Color color, double level, int x, int y)
		{
			if (level < 1)
			{
				HSV hsv = HSV.FromRGB(color);
				hsv.V = hsv.V * level;
				color = hsv.ToRGB();
			}
			frameBuffer.SetPixel(x, y, color);
		}

		private double CalculateSpeed(double intervalPos)
		{
			var value = ScaleCurveToValue(SpeedCurve.GetValue(intervalPos), 20, 1) * FrameTime / 50d;
			if (value < 1) value = 1;

			return value;
		}

		public Color GetMultiColorBlend(double n, int frame)
		{
			int colorcnt = Colors.Count();
			if (colorcnt <= 1) return Colors[0].GetColorAt(GetEffectTimeIntervalPosition(frame));
			if (n >= 1.0) n = 0.99999;
			if (n < 0.0) n = 0.0;
			int coloridx1 = (int)Math.Floor(n * colorcnt);
			int coloridx2 = (coloridx1 + 1) % colorcnt;
			double ratio = n * colorcnt - coloridx1;
			return Get2ColorBlend(coloridx1, coloridx2, ratio, frame);
		}

		public Color Get2ColorBlend(int coloridx1, int coloridx2, double ratio, int frame)
		{
			var pos = GetEffectTimeIntervalPosition(frame);
			var c1 = Colors[coloridx1].GetColorAt(pos);
			var c2 = Colors[coloridx2].GetColorAt(pos);

			return Color.FromArgb(ChannelBlend(c1.R, c2.R, ratio), ChannelBlend(c1.G, c2.G, ratio), ChannelBlend(c1.B, c2.B, ratio));
		}

		private int ChannelBlend(int c1, int c2, double ratio)
		{
			return c1 + (int)Math.Floor(ratio * (double)(c2 - c1) + 0.5);
		}

		private void CopyPixelsToTempBuf(IPixelFrameBuffer frameBuffer)
		{
			for (int x = 0; x < BufferWi; x++)
			{
				for (int y = 0; y < BufferHt; y++)
				{
					_tempbuf[x][y] = frameBuffer.GetColorAt(x,y);
				}
			}
		}

		// 0,0 is lower left
		private Color GetTempPixel(int x, int y)
		{
			if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt)
			{
				return _tempbuf[x][y];
			}
			return Color.Transparent;
		}

		private void CopyTempBufToPixels(IPixelFrameBuffer frameBuffer)
		{
			for (int x = 0; x < BufferWi; x++) {
				for (int y = 0; y < BufferHt; y++) {
					frameBuffer.SetPixel(x, y, _tempbuf[x][y]);
				}
			}
		}
	}
}
