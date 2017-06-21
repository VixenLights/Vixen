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

namespace VixenModules.Effect.Life
{
	public class Life : PixelEffectBase
	{
		private LifeData _data;
		private long _lastLifeState = 0;
		private Random random = new Random();
		private List<List<Color>> _tempbuf = new List<List<Color>>();

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
			if (_tempbuf != null)
			{
				_tempbuf.Clear();
			}

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
			//Nothing to clean up
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

		private int rand()
		{
			return random.Next();
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
				ClearTempBuf();
				for (i = 0; i < count; i++)
				{
					x = rand() % BufferWi;
					y = rand() % BufferHt;
					color = GetMultiColorBlend(rand01(), frame);
					HSV hsv = HSV.FromRGB(color);
					hsv.V = hsv.V * level;
					color = hsv.ToRGB();
					SetTempPixel(x, y, color);
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
								HSV hsv = HSV.FromRGB(color);
								hsv.V = hsv.V * level;
								frameBuffer.SetPixel(x, y, hsv);
							}
							else if (!isLive && cnt == 3)
							{
								color = GetMultiColorBlend(rand01(), frame);
								HSV hsv = HSV.FromRGB(color);
								hsv.V = hsv.V * level;
								frameBuffer.SetPixel(x, y, hsv);
							}
							break;
						case 1:
							// B35/S236
							if (isLive && (cnt == 2 || cnt == 3 || cnt == 6))
							{
								color = GetMultiColorBlend(rand01(), frame);
								HSV hsv = HSV.FromRGB(color);
								hsv.V = hsv.V * level;
								frameBuffer.SetPixel(x, y, hsv);
							}
							else if (!isLive && (cnt == 3 || cnt == 5))
							{
								color = GetMultiColorBlend(rand01(), frame);
								HSV hsv = HSV.FromRGB(color);
								hsv.V = hsv.V * level;
								frameBuffer.SetPixel(x, y, hsv);
							}
							break;
						case 2:
							// B357/S1358
							if (isLive && (cnt == 1 || cnt == 3 || cnt == 5 || cnt == 8))
							{
								HSV hsv = HSV.FromRGB(color);
								hsv.V = hsv.V * level;
								frameBuffer.SetPixel(x, y, hsv);
							}
							else if (!isLive && (cnt == 3 || cnt == 5 || cnt == 7))
							{
								color = GetMultiColorBlend(rand01(), frame);
								HSV hsv = HSV.FromRGB(color);
								hsv.V = hsv.V * level;
								frameBuffer.SetPixel(x, y, hsv);
							}
							break;
						case 3:
							// B378/S235678
							if (isLive && (cnt == 2 || cnt == 3 || cnt >= 5))
							{
								HSV hsv = HSV.FromRGB(color);
								hsv.V = hsv.V * level;
								frameBuffer.SetPixel(x, y, hsv);
							}
							else if (!isLive && (cnt == 3 || cnt == 7 || cnt == 8))
							{
								color = GetMultiColorBlend(rand01(), frame);
								HSV hsv = HSV.FromRGB(color);
								hsv.V = hsv.V * level;
								frameBuffer.SetPixel(x, y, hsv);
							}
							break;
						case 4:
							// B25678/S5678
							if (isLive && (cnt >= 5))
							{
								HSV hsv = HSV.FromRGB(color);
								hsv.V = hsv.V * level;
								frameBuffer.SetPixel(x, y, hsv);
							}
							else if (!isLive && (cnt == 2 || cnt >= 5))
							{
								color = GetMultiColorBlend(rand01(), frame);
								HSV hsv = HSV.FromRGB(color);
								hsv.V = hsv.V * level;
								frameBuffer.SetPixel(x, y, hsv);
							}
							break;
					}
				}
			}
			// copy new life state to tempbuf
			CopyPixelsToTempBuf(frameBuffer);
		}

		private double CalculateSpeed(double intervalPos)
		{
			var value = ScaleCurveToValue(SpeedCurve.GetValue(intervalPos), 20, 1);
			if (value < 1) value = 1;

			return value;
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
			int coloridx1 = (int)Math.Floor(n * colorcnt);
			int coloridx2 = (coloridx1 + 1) % colorcnt;
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

		// 0,0 is lower left
		private void SetTempPixel(int x, int y, Color color)
		{
			if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt)
			{
				_tempbuf[x][y] = color;
			}
		}

		private void CopyTempBufToPixels(IPixelFrameBuffer frameBuffer)
		{
			for (int x = 0; x < BufferWi; x++) {
				for (int y = 0; y < BufferHt; y++) {
					frameBuffer.SetPixel(x, y, _tempbuf[x][y]);
				}
			}
		}

		private void ClearTempBuf()
		{
			for (int x = 0; x < BufferWi; x++)
			{
				for (int y = 0; y < BufferHt; y++)
				{
					_tempbuf[x][y] = Color.Transparent;
				}
			}
		}

		// return a random number between 0 and 1 inclusive
		private double rand01()
		{
			return random.NextDouble();
		}
	}
}
