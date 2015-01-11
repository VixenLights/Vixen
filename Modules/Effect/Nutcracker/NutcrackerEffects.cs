using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Drawing;
using NLog;
using VixenModules.Effect.Nutcracker;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;

namespace VixenModules.Effect.Nutcracker
{
	public partial class NutcrackerEffects: IDisposable
	{
		#region Variables
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private NutcrackerData _data = null;
		private long _state;
		private int _lastPeriod, _curPeriod;
		private int _renderPeriod;
		private List<List<Color>> _pixels = new List<List<Color>>();
		private List<List<Color>> _tempbuf = new List<List<Color>>();
		private const double pi2 = Math.PI*2;
		//private int[] FireBuffer, WaveBuffer0, WaveBuffer1, WaveBuffer2 = new int[1];
		private int[] FireBuffer = new int[1];
		private Random random = new Random();
		private List<Color> FirePalette = new List<Color>();

		public enum Effects
		{
			Bars,
			Butterfly,
			ColorWash,
			Fire,
			Fireworks,
			Garlands,
			Life,
			Meteors,
			Movie,
			Picture,
			PictureTile,
			Snowflakes,
			Snowstorm,
			Spirals,
			Spirograph,
			Text,
			Tree,
			Twinkles,
			Curtain,
			Glediator
		}

		public enum PreviewType
		{
			None,
			Tree90,
			Tree180,
			Tree270,
			Tree360,
			Arch,
			HorizontalLine,
			VerticalLine,
			Grid,
			Cane
		}

		public enum StringDirection
		{
			Clockwise,
			CounterClockwise
		}

		public enum StringOrientations
		{
			Vertical,
			Horizontal
		}

		#endregion // Variables

		public NutcrackerEffects()
		{
			_state = 0;
			_lastPeriod = 0;
			_renderPeriod = 0;
			TimeInterval = 50;
		}

		/// <summary>
		/// Effect engine. Defaults to 50ms intervals
		/// </summary>
		/// <param name="data"></param>
		public NutcrackerEffects(NutcrackerData data):this(data, 50)
		{	
		}

		/// <summary>
		/// Effect engine. Defaults to 50ms intervals
		/// </summary>
		/// <param name="data"></param>
		/// <param name="timeInterval"></param>
		public NutcrackerEffects(NutcrackerData data, int timeInterval)
		{
			Data = data;
			TimeInterval = timeInterval;
		}

		public NutcrackerData Data
		{
			get { return _data; }
			set { _data = value; }
		}

		#region Properties

		public int StateInt
		{
			get { return Convert.ToInt32(_state); }
		}

		public long State
		{
			get { return _state; }
			set { _state = value; }
		}

		public List<List<Color>> Pixels
		{
			get { return _pixels; }
			set { _pixels = value; }
		}

		public int RenderPeriod
		{
			get { return _renderPeriod; }
			set { _renderPeriod = value; }
		}

		public Palette Palette
		{
			get { return Data.Palette; }
			set { Data.Palette = value; }
		}

		public int TimeInterval { get; set; }

		public bool FitToTime
		{
			private get { return Data.FitToTime; } 
			set { Data.FitToTime = value; }
		}

		public TimeSpan Duration { private get; set; }

		#endregion

		#region Nutcracker Utilities

		public static bool IsNutcrackerResource(string s) 
		{
			return s.Contains("VixenModules.Effect.Nutcracker");
		}
		
		private int rand()
		{
			return random.Next();
		}

		private void srand(int seed)
		{
			random = new Random(seed);
		}

		// return a random number between 0 and 1 inclusive
		private double rand01()
		{
			return random.NextDouble();
		}

		public void SetState(int period, int NewSpeed, bool ResetState)
		{
			if (ResetState) {
				State = 0;
			}
			else {
				State += (period - _lastPeriod)*NewSpeed;
			}
			Data.Speed = NewSpeed;

			_lastPeriod = _curPeriod = period;
		}

		public void SetNextState(bool ResetState)
		{
			if (Data != null) {
				RenderPeriod += 1;

				SetState(RenderPeriod, (int) Data.Speed, ResetState);
			}
		}

		private int _bufferHt = 0;

		public int BufferHt
		{
			get
			{
				//if (_pixels.Any() )
				//    return _pixels[0].Count();
				//else
				//    return 0;
				return _bufferHt;
			}
			set { _bufferHt = value; }
		}

		private int _bufferWi = 0;

		public int BufferWi
		{
			get
			{
				//return _pixels.Count(); 
				return _bufferWi;
			}
			set { _bufferWi = value; }
		}


		// return a value between c1 and c2
		private int ChannelBlend(int c1, int c2, double ratio)
		{
			return c1 + (int) Math.Floor(ratio*(double) (c2 - c1) + 0.5);
		}

		public int GetColorCount()
		{
			return Palette.Count();
		}

		public Color Get2ColorBlend(int coloridx1, int coloridx2, double ratio)
		{
			Color c1, c2;
			c1 = Palette.GetColor(coloridx1);
			c2 = Palette.GetColor(coloridx2);

			return Color.FromArgb(ChannelBlend(c1.R, c2.R, ratio), ChannelBlend(c1.G, c2.G, ratio),
			                      ChannelBlend(c1.B, c2.B, ratio));
			;
		}

		// 0 <= n < 1
		public Color GetMultiColorBlend(double n, bool circular)
		{
			int colorcnt = GetColorCount();
			if (colorcnt <= 1) {
				return Palette.GetColor(0);
			}
			if (n >= 1.0) n = 0.99999;
			if (n < 0.0) n = 0.0;
			double realidx = circular ? n*colorcnt : n*(colorcnt - 1);
			int coloridx1 = (int) Math.Floor(realidx);
			int coloridx2 = (coloridx1 + 1)%colorcnt;
			double ratio = realidx - (double) coloridx1;
			return Get2ColorBlend(coloridx1, coloridx2, ratio);
		}

		public static Image ConvertToGrayScale(Image srce)
		{
			Bitmap bmp = new Bitmap(srce.Width, srce.Height);
			using (Graphics gr = Graphics.FromImage(bmp)) {
				var matrix = new float[][]
				             	{
				             		new float[] {0.299f, 0.299f, 0.299f, 0, 0},
				             		new float[] {0.587f, 0.587f, 0.587f, 0, 0},
				             		new float[] {0.114f, 0.114f, 0.114f, 0, 0},
				             		new float[] {0, 0, 0, 1, 0},
				             		new float[] {0, 0, 0, 0, 1}
				             	};
				var ia = new System.Drawing.Imaging.ImageAttributes();
				ia.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(matrix));
				var rc = new Rectangle(0, 0, srce.Width, srce.Height);
				gr.DrawImage(srce, rc, 0, 0, srce.Width, srce.Height, GraphicsUnit.Pixel, ia);
				return bmp;
			}
		}

		public Bitmap ScaleImage(Image image, double scale)
		{
			int maxWidth = Convert.ToInt32((double) image.Width*scale);
			int maxHeight = Convert.ToInt32((double) image.Height*scale);
			var ratioX = (double) maxWidth/image.Width;
			var ratioY = (double) maxHeight/image.Height;
			var ratio = Math.Min(ratioX, ratioY);

			var newWidth = (int) (image.Width*ratio);
			var newHeight = (int) (image.Height*ratio);

			var newImage = new Bitmap(newWidth, newHeight);
			using (var g = Graphics.FromImage(newImage)) {
				g.DrawImage(image, 0, 0, newWidth, newHeight);
				return newImage;
			}
		}

		private double GetEffectTimeIntervalPosition()
		{
			double retval;
			if (Duration == TimeSpan.Zero)
			{
				retval = 1;
			}
			else
			{
				retval = _curPeriod/(Duration.TotalMilliseconds/TimeInterval);
			}
			return retval;
		}

		#endregion

		#region Pixels

		public void InitBuffer(int bufferWidth, int bufferHeight)
		{
            if (bufferWidth == 0)
                bufferWidth = 1;
            if (bufferHeight == 0)
                bufferHeight = 0;

			_pixels.Clear();
			_tempbuf.Clear();

			List<Color> column;
			for (int width = 0; width < bufferWidth; width++) {
				column = new List<Color>();
				_pixels.Add(column);
				column = new List<Color>();
				_tempbuf.Add(column);
				for (int height = 0; height < bufferHeight; height++) {
					_pixels[width].Add(Color.Transparent);
					_tempbuf[width].Add(Color.Transparent);
				}
			}

			_bufferWi = Pixels.Count();
			_bufferHt = Pixels[0].Count();

			
			//Array.Resize(ref WaveBuffer0, bufferWidth*bufferHeight);
			//Array.Resize(ref WaveBuffer1, bufferWidth*bufferHeight);
			//Array.Resize(ref WaveBuffer2, bufferWidth*bufferHeight);

			State = 0;
		}

		

		// 0,0 is lower left
		public void SetPixel(int x, int y, Color color)
		{
			if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt) {
				_pixels[x][y] = color;
			}
		}

		// 0,0 is lower left
		public void SetPixel(int x, int y, HSV hsv)
		{
			Color color = HSV.HSVtoColor(hsv);
			SetPixel(x, y, color);
		}

		public Color GetPixel(int pixelToGet)
		{
            Color color = Color.Transparent;
            int x = pixelToGet / BufferHt;
            int y = pixelToGet % BufferHt;
            if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt)
            {
                color = _pixels[x][y];
            }
			return color;
		}

		public Color GetPixel(int x, int y)
		{
			return _pixels[x][y];
		}


		public int PixelCount()
		{
			return BufferWi*BufferHt;
		}

		public void ClearPixels(Color color)
		{
			foreach (List<Color> column in Pixels) {
				for (int row = 0; row < column.Count; row++) {
					column[row] = color;
				}
			}
		}

		// 0,0 is lower left
		private void SetTempPixel(int x, int y, Color color)
		{
			if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt) {
				_tempbuf[x][y] = color;
			}
		}

		// 0,0 is lower left
		private Color GetTempPixel(int x, int y)
		{
			if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt) {
				return _tempbuf[x][y];
			}
			return Color.Black;
		}

		private void ClearTempBuf()
		{
			for (int x = 0; x < BufferWi; x++) {
				for (int y = 0; y < BufferHt; y++) {
					_tempbuf[x][y] = Color.Black;
				}
			}
		}

		private void CopyTempBufToPixels()
		{
			for (int x = 0; x < BufferWi; x++) {
				for (int y = 0; y < BufferHt; y++) {
					Pixels[x][y] = _tempbuf[x][y];
				}
			}
		}

		private void CopyPixelsToTempBuf()
		{
			for (int x = 0; x < BufferWi; x++) {
				for (int y = 0; y < BufferHt; y++) {
					_tempbuf[x][y] = Pixels[x][y];
				}
			}
		}

		#endregion

		#region Nutcracker Effects

		#region Bars

		public void RenderBars(int paletteRepeat, int direction, bool highlight, bool show3D)
		{
			int x,y,n,colorIdx;
			HSV hsv;
			int colorcnt=GetColorCount();
			int barCount = paletteRepeat * colorcnt;
			double position = GetEffectTimeIntervalPosition();
			if(barCount<1) barCount=1;


			if (direction < 4 || direction == 8 || direction == 9)
			{
				int barHt = BufferHt/barCount+1;
				if(barHt<1) barHt=1;
				int halfHt = BufferHt/2;
				int blockHt=colorcnt * barHt;
				if(blockHt<1) blockHt=1;
				int fOffset = Convert.ToInt32(FitToTime?position*blockHt: State/4 % blockHt);
				fOffset = Convert.ToInt32(direction == 8 || direction == 9 ? (State/20)*barHt: fOffset);
				direction = direction > 4?direction-8:direction;

				for (y=0; y<BufferHt; y++)
				{
					n=y+fOffset;
					colorIdx=(n % blockHt) / barHt;
					hsv= Palette.GetHSV(colorIdx);
					if (highlight && n % barHt == 0) hsv.Saturation=0.0f;
					if (show3D) hsv.Value *= (float)(barHt - n%barHt - 1) / barHt;
					switch (direction)
					{
					case 1:
						// down
						for (x=0; x<BufferWi; x++)
						{
							SetPixel(x,y,hsv);
						}
						break;
					case 2:
						// expand
						if (y <= halfHt) {
							for (x=0; x<BufferWi; x++)
							{
								SetPixel(x,y,hsv);
								SetPixel(x,BufferHt-y-1,hsv);
							}
						}
						break;
					case 3:
						// compress
						if (y >= halfHt) {
							for (x=0; x<BufferWi; x++)
							{
								SetPixel(x,y,hsv);
								SetPixel(x,BufferHt-y-1,hsv);
							}
						}
						break;
					default:
						// up
						for (x=0; x<BufferWi; x++)
						{
							SetPixel(x,BufferHt-y-1,hsv);
						}
						break;
					}
				}
			}
			else
			{
				int barWi = BufferWi/barCount+1;
				if(barWi<1) barWi=1;
				int halfWi = BufferWi/2;
				int blockWi=colorcnt * barWi;
				if(blockWi<1) blockWi=1;
				int fOffset = Convert.ToInt32(FitToTime?position*blockWi:State/4 % blockWi);
				fOffset = Convert.ToInt32(direction > 9 ? (State/20)*barWi: fOffset);
				direction = direction > 9?direction-6:direction;
				for (x=0; x<BufferWi; x++)
				{
					n=x+fOffset;
					colorIdx=(n % blockWi) / barWi;
					hsv = Palette.GetHSV(colorIdx);
					if (highlight && n % barWi == 0) hsv.Saturation=0.0f;
					if (show3D) hsv.Value *= (float)(barWi - n%barWi - 1) / barWi;
					switch (direction)
					{
					case 5:
						// right
						for (y=0; y<BufferHt; y++)
						{
							SetPixel(BufferWi-x-1,y,hsv);
						}
						break;
					case 6:
						// H-expand
						if (x <= halfWi) {
							for (y=0; y<BufferHt; y++)
							{
								SetPixel(x,y,hsv);
								SetPixel(BufferWi-x-1,y,hsv);
							}
						}
						break;
					case 7:
						// H-compress
						if (x >= halfWi) {
							for (y=0; y<BufferHt; y++)
							{
								SetPixel(x,y,hsv);
								SetPixel(BufferWi-x-1,y,hsv);
							}
						}
						break;
					default:
						// left
						for (y=0; y<BufferHt; y++)
						{
							SetPixel(x,y,hsv);
						}
						break;
					}
				}
			}
		}

		#endregion

		#region Garlands

		public void RenderGarlands(int GarlandType, int Spacing)
		{
			int x, y, yadj, ylimit, ring;
			double ratio;
			Color color;
			int PixelSpacing = Spacing*BufferHt/100 + 3;
			int limit = BufferHt*PixelSpacing*4;
			int GarlandsState = (limit - ((int) _state%limit))/4;
			for (ring = 0; ring < BufferHt; ring++) {
				ratio = (double) ring/(double) BufferHt;
				color = GetMultiColorBlend(ratio, false);
				y = GarlandsState - ring*PixelSpacing;
				ylimit = BufferHt - ring - 1;
				for (x = 0; x < BufferWi; x++) {
					yadj = y;
					switch (GarlandType) {
						case 1:
							switch (x%5) {
								case 2:
									yadj -= 2;
									break;
								case 1:
								case 3:
									yadj -= 1;
									break;
							}
							break;
						case 2:
							switch (x%5) {
								case 2:
									yadj -= 4;
									break;
								case 1:
								case 3:
									yadj -= 2;
									break;
							}
							break;
						case 3:
							switch (x%6) {
								case 3:
									yadj -= 6;
									break;
								case 2:
								case 4:
									yadj -= 4;
									break;
								case 1:
								case 5:
									yadj -= 2;
									break;
							}
							break;
						case 4:
							switch (x%5) {
								case 1:
								case 3:
									yadj -= 2;
									break;
							}
							break;
					}
					if (yadj < ylimit) yadj = ylimit;
					if (yadj < BufferHt) SetPixel(x, yadj, color);
				}
			}
		}

		#endregion

		#region Butterfly

		public void RenderButterfly(int colorScheme, int style, int chunks, int skip, int direction)
		{
			int x, y, d;
			double n, x1, y1, f;
			double h = 0.0;
			Color color;
			HSV hsv = new HSV();
			int maxframe = BufferHt*2;
			int frame = (int) ((BufferHt*(double) State/200.0)%maxframe);
			double offset = State/100.0;
			if (direction == 1) offset = -offset;
			for (x = 0; x < BufferWi; x++) {
				for (y = 0; y < BufferHt; y++) {
					switch (style) {
						case 1:
							n = Math.Abs((x*x - y*y)*Math.Sin(offset + ((x + y)*pi2/(BufferHt + BufferWi))));
							d = x*x + y*y + 1;
							h = n/d;
							break;
						case 2:
							f = (frame < maxframe/2) ? frame + 1 : maxframe - frame;
							x1 = (x - BufferWi/2.0)/f;
							y1 = (y - BufferHt/2.0)/f;
							h = Math.Sqrt(x1*x1 + y1*y1);
							break;
						case 3:
							f = (frame < maxframe/2) ? frame + 1 : maxframe - frame;
							f = f*0.1 + BufferHt/60.0;
							x1 = (x - BufferWi/2.0)/f;
							y1 = (y - BufferHt/2.0)/f;
							h = Math.Sin(x1)*Math.Cos(y1);
							break;
					}
					hsv.Saturation = 1.0f;
					hsv.Value = 1.0f;
					if (chunks <= 1 || ((int) (h*chunks))%skip != 0) {
						if (colorScheme == 0) {
							hsv.Hue = (float) h;
							SetPixel(x, y, hsv);
						}
						else {
							color = GetMultiColorBlend(h, false);
							SetPixel(x, y, color);
						}
					}
				}
			}
		}

		#endregion

		#region ColorWash

		public void RenderColorWash(bool HorizFade, bool VertFade, int RepeatCount)
		{
			const int SpeedFactor = 200;
			int x, y;
			Color color;
			HSV hsv2 = new HSV();
			int colorcnt = GetColorCount();
			int CycleLen = colorcnt*SpeedFactor;
			if (State > (colorcnt - 1)*SpeedFactor*RepeatCount && RepeatCount < 10) {
				//Console.WriteLine("1");
				color = GetMultiColorBlend((double) (RepeatCount%2), false);
			}
			else {
				//Console.WriteLine("2");
				color = GetMultiColorBlend((double) (State%CycleLen)/(double) CycleLen, true);
			}
			HSV hsv = HSV.ColorToHSV(color);
			double HalfHt = (double) (BufferHt - 1)/2.0;
			double HalfWi = (double) (BufferWi - 1)/2.0;
			for (x = 0; x < BufferWi; x++) {
				for (y = 0; y < BufferHt; y++) {
					hsv2.SetToHSV(hsv);
					if (HorizFade && HalfWi > 0) hsv2.Value *= (float) (1.0 - Math.Abs(HalfWi - x)/HalfWi);
					if (VertFade && HalfHt > 0) hsv2.Value *= (float) (1.0 - Math.Abs(HalfHt - y)/HalfHt);
					//SetPixel(x, y, hsv);
					SetPixel(x, y, hsv2);
					//SetPixel(x, y, color);
				}
			}
		}

		#endregion

		#region Fire

		// initialize FirePalette[]
		private void InitFirePalette()
		{
			HSV hsv = new HSV();
			Color color;

			FirePalette.Clear();
			//wxImage::HSVValue hsv;
			//wxImage::RGBValue rgb;
			//wxColour color;
			int i;
			// calc 100 reds, black to bright red
			hsv.Hue = 0.0f;
			hsv.Saturation = 1.0f;
			for (i = 0; i < 100; i++)
			{
				hsv.Value = (float)i / 100.0f;
				//rgb = wxImage::HSVtoRGB(hsv);
				//color.Set(rgb.red,rgb.green,rgb.blue);
				color = HSV.HSVtoColor(hsv);
				FirePalette.Add(color);
				//FirePalette.push_back(color);
			}

			// gives 100 hues red to yellow
			hsv.Value = 1.0f;
			for (i = 0; i < 100; i++)
			{
				//rgb = wxImage::HSVtoRGB(hsv);
				//color.Set(rgb.red,rgb.green,rgb.blue);
				color = HSV.HSVtoColor(hsv);
				//FirePalette.push_back(color);
				FirePalette.Add(color);
				hsv.Hue += 0.00166666f;
			}
		}

		// 0 <= x < BufferWi
		// 0 <= y < BufferHt
		private void SetFireBuffer(int x, int y, int PaletteIdx)
		{
			if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt) {
				FireBuffer[y*BufferWi + x] = PaletteIdx;
			}
		}

		// 0 <= x < BufferWi
		// 0 <= y < BufferHt
		private int GetFireBuffer(int x, int y)
		{
			if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt) {
				return FireBuffer[y*BufferWi + x];
			}
			return -1;
		}

		//private void SetWaveBuffer1(int x, int y, int value)
		//{
		//	if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt) {
		//		WaveBuffer1[y*BufferWi + x] = value;
		//	}
		//}

		//private void SetWaveBuffer2(int x, int y, int value)
		//{
		//	if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt) {
		//		WaveBuffer2[y*BufferWi + x] = value;
		//	}
		//}

		//private int GetWaveBuffer1(int x, int y)
		//{
		//	if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt) {
		//		return WaveBuffer1[y*BufferWi + x];
		//	}
		//	return -1;
		//}

		//private int GetWaveBuffer2(int x, int y)
		//{
		//	if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt) {
		//		return WaveBuffer2[y*BufferWi + x];
		//	}
		//	return -1;
		//}

		// 10 <= HeightPct <= 100
		private void RenderFire(int HeightPct)
		{
			if (FireBuffer.Count() != BufferWi*BufferHt)
			{
				Array.Resize(ref FireBuffer, BufferWi * BufferHt);
				InitFirePalette();
			}

			int x, y, i, r, v1, v2, v3, v4, n, new_index;
			if (State == 0) {
				for (i = 0; i < FireBuffer.Count(); i++) {
					FireBuffer[i] = 0;
				}
			}
			// build fire
			for (x = 0; x < BufferWi; x++) {
				r = x%2 == 0 ? 190 + (rand()%10) : 100 + (rand()%50);
				SetFireBuffer(x, 0, r);
			}
			int step = 255*100/BufferHt/HeightPct;
			int sum;
			for (y = 1; y < BufferHt; y++) {
				for (x = 0; x < BufferWi; x++) {
					v1 = GetFireBuffer(x - 1, y - 1);
					v2 = GetFireBuffer(x + 1, y - 1);
					v3 = GetFireBuffer(x, y - 1);
					v4 = GetFireBuffer(x, y - 1);
					n = 0;
					sum = 0;
					if (v1 >= 0) {
						sum += v1;
						n++;
					}
					if (v2 >= 0) {
						sum += v2;
						n++;
					}
					if (v3 >= 0) {
						sum += v3;
						n++;
					}
					if (v4 >= 0) {
						sum += v4;
						n++;
					}
					new_index = n > 0 ? sum/n : 0;
					if (new_index > 0) {
						new_index += (rand()%100 < 20) ? step : -step;
						if (new_index < 0) new_index = 0;
						if (new_index >= FirePalette.Count()) new_index = FirePalette.Count() - 1;
					}
					SetFireBuffer(x, y, new_index);
				}
			}
			for (y = 0; y < BufferHt; y++) {
				for (x = 0; x < BufferWi; x++) {
					//SetPixel(x,y,FirePalette[y]);
					Color color = FirePalette[GetFireBuffer(x, y)];
					if (color.R == 0 && color.G == 0 && color.B == 0)
						color = Color.Transparent;
					SetPixel(x, y, color);
				}
			}
		}

		#endregion // Fire

		#region Life

		private int LastLifeCount, LastLifeType = 0;
		private long LastLifeState = 0;

		private int Life_CountNeighbors(int x0, int y0)
		{
			//     2   3   4
			//     1   X   5
			//     0   7   6
			int[] n_x = {-1, -1, -1, 0, 1, 1, 1, 0};
			int[] n_y = {-1, 0, 1, 1, 1, 0, -1, -1};
			int x, y, cnt = 0;
			for (int i = 0; i < 8; i++) {
				x = (x0 + n_x[i])%BufferWi;
				y = (y0 + n_y[i])%BufferHt;
				if (x < 0) x += BufferWi;
				if (y < 0) y += BufferHt;
				if ((GetTempPixel(x, y) != Color.Black) && (GetTempPixel(x, y) != Color.Transparent)) cnt++;
			}
			return cnt;
		}

		// use tempbuf for calculations
		public void RenderLife(int Count, int Type)
		{
			int i, x, y, cnt;
			bool isLive;
			Color color;
			Count = BufferWi*BufferHt*Count/200 + 1;
			if (State == 0 || Count != LastLifeCount || Type != LastLifeType) {
				//Console.WriteLine("RenderLife Init");
				// seed tempbuf
				LastLifeCount = Count;
				LastLifeType = Type;
				ClearTempBuf();
				for (i = 0; i < Count; i++) {
					x = rand()%BufferWi;
					y = rand()%BufferHt;
					color = GetMultiColorBlend(rand01(), false);
					SetTempPixel(x, y, color);
				}
			}
			long TempState = (State%400)/20;
			if (TempState == LastLifeState) {
				//Pixels=tempbuf;
				CopyTempBufToPixels();
				return;
			}
			else {
				LastLifeState = TempState;
			}

			for (x = 0; x < BufferWi; x++) {
				for (y = 0; y < BufferHt; y++) {
					color = GetTempPixel(x, y);
					//isLive=(color.GetRGB() != 0);
					isLive = (color != Color.Black && color != Color.Transparent);
					cnt = Life_CountNeighbors(x, y);
					switch (Type) {
						case 0:
							// B3/S23
							/*
                        Any live cell with fewer than two live neighbours dies, as if caused by under-population.
                        Any live cell with two or three live neighbours lives on to the next generation.
                        Any live cell with more than three live neighbours dies, as if by overcrowding.
                        Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
                        */
							if (isLive && cnt >= 2 && cnt <= 3) {
								SetPixel(x, y, color);
							}
							else if (!isLive && cnt == 3) {
								color = GetMultiColorBlend(rand01(), false);
								SetPixel(x, y, color);
							}
							break;
						case 1:
							// B35/S236
							if (isLive && (cnt == 2 || cnt == 3 || cnt == 6)) {
								SetPixel(x, y, color);
							}
							else if (!isLive && (cnt == 3 || cnt == 5)) {
								color = GetMultiColorBlend(rand01(), false);
								SetPixel(x, y, color);
							}
							break;
						case 2:
							// B357/S1358
							if (isLive && (cnt == 1 || cnt == 3 || cnt == 5 || cnt == 8)) {
								SetPixel(x, y, color);
							}
							else if (!isLive && (cnt == 3 || cnt == 5 || cnt == 7)) {
								color = GetMultiColorBlend(rand01(), false);
								SetPixel(x, y, color);
							}
							break;
						case 3:
							// B378/S235678
							if (isLive && (cnt == 2 || cnt == 3 || cnt >= 5)) {
								SetPixel(x, y, color);
							}
							else if (!isLive && (cnt == 3 || cnt == 7 || cnt == 8)) {
								color = GetMultiColorBlend(rand01(), false);
								SetPixel(x, y, color);
							}
							break;
						case 4:
							// B25678/S5678
							if (isLive && (cnt >= 5)) {
								SetPixel(x, y, color);
							}
							else if (!isLive && (cnt == 2 || cnt >= 5)) {
								color = GetMultiColorBlend(rand01(), false);
								SetPixel(x, y, color);
							}
							break;
					}
				}
			}
			// copy new life state to tempbuf
			CopyPixelsToTempBuf();
		}

		#endregion // Life

		#region Meteors

		private List<MeteorClass> meteors = new List<MeteorClass>();

		// for meteor effect
		public class MeteorClass
		{
			public int x;
			public int y;
			public HSV hsv = new HSV();

			public bool HasExpired(int tailLength)
			{
				return (y + tailLength < 0);
			}
		}

		private void RenderMeteors(int MeteorType, int Count, int Length)
		{
			if (State == 0)
				meteors.Clear();
			int mspeed = (int) State/4;
			State -= mspeed*4 - 1;

			// create new meteors
			HSV hsv = new HSV();
			HSV hsv0 = new HSV();
			HSV hsv1 = new HSV();
			hsv0 = Palette.GetHSV(0);
			hsv1 = Palette.GetHSV(1);
			int colorcnt = GetColorCount();
			Count = BufferWi*Count/100;
			int TailLength = (BufferHt < 10) ? Length/10 : BufferHt*Length/100;
			if (TailLength < 1) TailLength = 1;
			int TailStart = BufferHt - TailLength;
			if (TailStart < 1) TailStart = 1;
			for (int i = 0; i < Count; i++) {
				MeteorClass m = new MeteorClass();
				m.x = rand()%BufferWi;
				m.y = BufferHt - 1 - (rand()%TailStart);
				switch (MeteorType) {
					case 1:
						m.hsv = HSV.SetRangeColor(hsv0, hsv1);
						break;
					case 2:
						m.hsv = Palette.GetHSV(rand()%colorcnt);
						break;
				}
				meteors.Add(m);
			}

			// render meteors
			foreach (MeteorClass meteor in meteors) {
				{
					for (int ph = 0; ph < TailLength; ph++) {
						switch (MeteorType) {
							case 0:
								hsv.Hue = (float) (rand()%1000)/1000.0f;
								hsv.Saturation = 1.0f;
								hsv.Value = 1.0f;
								break;
							default:
								hsv.SetToHSV(meteor.hsv);
								break;
						}
						hsv.Value *= (float) (1.0 - (double) ph/TailLength);
						SetPixel(meteor.x, meteor.y + ph, hsv);
					}
					meteor.y -= mspeed;
				}
			}
			// delete old meteors
			int meteorNum = 0;
			while (meteorNum < meteors.Count) {
				if (meteors[meteorNum].HasExpired(TailLength)) {
					meteors.RemoveAt(meteorNum);
				}
				else {
					meteorNum++;
				}
			}
		}

		#endregion // Meteors

		#region Fireworks

		//private RgbFireworks[] fireworkBursts = new RgbFireworks[20000];
		private List<RgbFireworks> fireworkBursts = null;

		private void InitFireworksBuffer()
		{
			if (fireworkBursts == null) {
				fireworkBursts = new List<RgbFireworks>();
				for (int burstNum = 0; burstNum < 20000; burstNum++) {
					RgbFireworks firework = new RgbFireworks();
					fireworkBursts.Add(firework);
				}
			}
		}

		public void RenderFireworks(int Number_Explosions, int Count, float Velocity, int Fade)
		{
			int idxFlakes = 0;
			int i = 0;
			int mod100;
			int x25, x75, y25, y75;
			const int maxFlakes = 1000;
			int startX;
			int startY; //, ColorIdx;
			float v;
			HSV hsv = new HSV();
			int colorCount = GetColorCount();

			// This does not work with 1 string, so use a try/catch block to prevent errors
			try {
				InitFireworksBuffer();

				if (State == 0) {
					for (i = 0; i < maxFlakes; i++) {
						fireworkBursts[i]._bActive = false;
					}
				}
				
				mod100 = (int) (State%(101 - Number_Explosions)*20);
				if (mod100 == 0) {
					x25 = (int) (BufferWi*0.25);
					x75 = (int) (BufferWi*0.75);
					y25 = (int) (BufferHt*0.25);
					y75 = (int) (BufferHt*0.75);
					startX = x25 + (rand()%(x75 - x25));
					startY = y25 + (rand()%(y75 - y25));

					// Create new bursts
					hsv = Palette.ActiveColors.Count>0?HSV.ColorToHSV(Palette.ActiveColors[rand() % colorCount]):HSV.ColorToHSV(Color.White);
					for (i = 0; i < Count; i++) {
						do {
							idxFlakes = (idxFlakes + 1)%maxFlakes;
						} while (fireworkBursts[idxFlakes]._bActive);
						fireworkBursts[idxFlakes].Reset(startX, startY, true, Velocity, hsv);
					}
				}
				else {
					for (i = 0; i < maxFlakes; i++) {
						// ... active flakes:
						if (fireworkBursts[i]._bActive) {
							// Update position
							fireworkBursts[i]._x += fireworkBursts[i]._dx;
							fireworkBursts[i]._y +=
								(float) (-fireworkBursts[i]._dy - fireworkBursts[i]._cycles*fireworkBursts[i]._cycles/10000000.0);
							// If this flake run for more than maxCycle, time to switch it off
							fireworkBursts[i]._cycles += 20;
							if (10000 == fireworkBursts[i]._cycles) // if (10000 == fireworkBursts[i]._cycles)
							{
								fireworkBursts[i]._bActive = false;
								continue;
							}
							// If this flake hit the earth, time to switch it off
							if (fireworkBursts[i]._y >= BufferHt) {
								fireworkBursts[i]._bActive = false;
								continue;
							}
							// Draw the flake, if its X-pos is within frame
							if (fireworkBursts[i]._x >= 0.0 && fireworkBursts[i]._x < BufferWi) {
								// But only if it is "under" the roof!
								if (fireworkBursts[i]._y >= 0.0) {
									// sean we need to set color here
								}
							}
							else {
								// otherwise it just got outside the valid X-pos, so switch it off
								fireworkBursts[i]._bActive = false;
							}
						}
					}
				}

				for (i = 0; i < 1000; i++) {
					if (fireworkBursts[i]._bActive) {
						v = (float) (((Fade*10.0) - fireworkBursts[i]._cycles)/(Fade*10.0));
						if (v < 0) v = 0.0f;
						hsv = fireworkBursts[i]._hsv;
						hsv.Value = v;
						SetPixel((int) fireworkBursts[i]._x, (int) fireworkBursts[i]._y, hsv);
					}
				}
			}
			catch {
			}
		}

		public class RgbFireworks
		{
			public const int maxCycle = 4096;
			public const int maxNewBurstFlakes = 10;
			public float _x;
			public float _y;
			public float _dx;
			public float _dy;
			public float vel;
			public float angle;
			public bool _bActive;
			public int _cycles;
			public HSV _hsv;
			private static Random random = new Random();

			public void Reset(int x, int y, bool active, float velocity, HSV hsv)
			{
				_x = x;
				_y = y;
				vel = (random.Next() - int.MaxValue/2)*velocity/(int.MaxValue/2);
				angle = (float) (2*Math.PI*random.Next()/int.MaxValue);
				_dx = (float) (vel*Math.Cos(angle));
				_dy = (float) (vel*Math.Sin(angle));
				_bActive = active;
				_cycles = 0;
				_hsv = hsv;
			}
		}

		#endregion

		#region Snowflakes

		private int LastSnowflakeCount = 0;
		private int LastSnowflakeType = 0;

		public void RenderSnowflakes(int Count, int SnowflakeType)
		{
			if (BufferHt <= 1) return; //Invalid configuration
			int i, n, y0, check, delta_y;
			int x = 0;
			int y = 0;
			Color color1, color2;
			if (State == 0 || Count != LastSnowflakeCount || SnowflakeType != LastSnowflakeType) {
				// initialize
				LastSnowflakeCount = Count;
				LastSnowflakeType = SnowflakeType;
				color1 = Palette.GetColor(0);
				color2 = Palette.GetColor(1);
				ClearTempBuf();
				// place Count snowflakes
				for (n = 0; n < Count; n++) {
					delta_y = BufferHt/4;
					y0 = (n%4)*delta_y;
					if (y0 + delta_y > BufferHt) delta_y = BufferHt - y0;
					// find unused space
					for (check = 0; check < 20; check++) {
						x = rand()%BufferWi;
						y = y0 + (rand()%delta_y);
						//if (GetTempPixelRGB(x,y) == 0) break;
						if (GetTempPixel(x, y) == Color.Black) break;
					}
					// draw flake, SnowflakeType=0 is random type
					switch (SnowflakeType == 0 ? rand()%5 : SnowflakeType - 1) {
						case 0:
							// single node
							SetTempPixel(x, y, color1);
							break;
						case 1:
							// 5 nodes
							if (x < 1) x += 1;
							if (y < 1) y += 1;
							if (x > BufferWi - 2) x -= 1;
							if (y > BufferHt - 2) y -= 1;
							SetTempPixel(x, y, color1);
							SetTempPixel(x - 1, y, color2);
							SetTempPixel(x + 1, y, color2);
							SetTempPixel(x, y - 1, color2);
							SetTempPixel(x, y + 1, color2);
							break;
						case 2:
							// 3 nodes
							if (x < 1) x += 1;
							if (y < 1) y += 1;
							if (x > BufferWi - 2) x -= 1;
							if (y > BufferHt - 2) y -= 1;
							SetTempPixel(x, y, color1);
							if (rand()%100 > 50) // % 2 was not so random
							{
								SetTempPixel(x - 1, y, color2);
								SetTempPixel(x + 1, y, color2);
							}
							else {
								SetTempPixel(x, y - 1, color2);
								SetTempPixel(x, y + 1, color2);
							}
							break;
						case 3:
							// 9 nodes
							if (x < 2) x += 2;
							if (y < 2) y += 2;
							if (x > BufferWi - 3) x -= 2;
							if (y > BufferHt - 3) y -= 2;
							SetTempPixel(x, y, color1);
							for (i = 1; i <= 2; i++) {
								SetTempPixel(x - i, y, color2);
								SetTempPixel(x + i, y, color2);
								SetTempPixel(x, y - i, color2);
								SetTempPixel(x, y + i, color2);
							}
							break;
						case 4:
							// 13 nodes
							if (x < 2) x += 2;
							if (y < 2) y += 2;
							if (x > BufferWi - 3) x -= 2;
							if (y > BufferHt - 3) y -= 2;
							SetTempPixel(x, y, color1);
							SetTempPixel(x - 1, y, color2);
							SetTempPixel(x + 1, y, color2);
							SetTempPixel(x, y - 1, color2);
							SetTempPixel(x, y + 1, color2);

							SetTempPixel(x - 1, y + 2, color2);
							SetTempPixel(x + 1, y + 2, color2);
							SetTempPixel(x - 1, y - 2, color2);
							SetTempPixel(x + 1, y - 2, color2);
							SetTempPixel(x + 2, y - 1, color2);
							SetTempPixel(x + 2, y + 1, color2);
							SetTempPixel(x - 2, y - 1, color2);
							SetTempPixel(x - 2, y + 1, color2);
							break;
						case 5:
							// 45 nodes (not enabled)
							break;
					}
				}
			}

			// move snowflakes
			int new_x, new_y, new_x2, new_y2;
			for (x = 0; x < BufferWi; x++) {
				new_x = (x + StateInt/20)%BufferWi; // CW
				new_x2 = (x - StateInt/20)%BufferWi; // CCW
				if (new_x2 < 0) new_x2 += BufferWi;
				for (y = 0; y < BufferHt; y++) {
					new_y = (y + StateInt/10)%BufferHt;
					new_y2 = (new_y + BufferHt/2)%BufferHt;
					color1 = GetTempPixel(new_x, new_y);
					//if (color1.GetRGB() == 0) GetTempPixel(new_x2,new_y2,color1);
					if (color1 == Color.Black) color1 = GetTempPixel(new_x2, new_y2);
					SetPixel(x, y, color1);
				}
			}
		}

		#endregion // Snowflakes

		#region Snowstorm

		private class SnowstormClass
		{
			public List<Point> points = new List<Point>();
			public HSV hsv;
			public int idx, ssDecay;

			public SnowstormClass()
			{
				points.Clear();
			}
		};

		//List<SnowstormClass> SnowstormList = new List<SnowstormClass>();
		private List<SnowstormClass> SnowstormItems = new List<SnowstormClass>();
		private int LastSnowstormCount = 0;

		private Point SnowstormVector(int idx)
		{
			Point xy = new Point();
			switch (idx) {
				case 0:
					xy.X = -1;
					xy.Y = 0;
					break;
				case 1:
					xy.X = -1;
					xy.Y = -1;
					break;
				case 2:
					xy.X = 0;
					xy.Y = -1;
					break;
				case 3:
					xy.X = 1;
					xy.Y = -1;
					break;
				case 4:
					xy.X = 1;
					xy.Y = 0;
					break;
				case 5:
					xy.X = 1;
					xy.Y = 1;
					break;
				case 6:
					xy.X = 0;
					xy.Y = 1;
					break;
				default:
					xy.X = -1;
					xy.Y = 1;
					break;
			}
			return xy;
		}

		private void SnowstormAdvance(SnowstormClass ssItem)
		{
			const int cnt = 8; // # of integers in each set in arr[]
			int[] arr = {30, 20, 10, 5, 0, 5, 10, 20, 20, 15, 10, 10, 10, 10, 10, 15};
			// 2 sets of 8 numbers, each of which add up to 100
			Point adv = SnowstormVector(7);
			int i0 = ssItem.idx%7 <= 4 ? 0 : cnt;
			int r = rand()%100;
			for (int i = 0, val = 0; i < cnt; i++) {
				val += arr[i0 + i];
				if (r < val) {
					adv = SnowstormVector(i);
					break;
				}
			}
			if (ssItem.idx%3 == 0) {
				adv.X *= 2;
				adv.Y *= 2;
			}
			//Point xy = ssItem.points.back() + adv;
			Point xy = ssItem.points[ssItem.points.Count - 1];
			xy.X += adv.X;
			xy.Y += adv.Y;

			xy.X %= BufferWi;
			xy.Y %= BufferHt;
			if (xy.X < 0) xy.X += BufferWi;
			if (xy.Y < 0) xy.Y += BufferHt;
			//ssItem.points.push_back(xy);
			ssItem.points.Add(xy);
		}

		public void RenderSnowstorm(int Count, int Length)
		{
			HSV hsv, hsv0, hsv1;
			hsv0 = Palette.GetHSV(0);
			hsv1 = Palette.GetHSV(1);
			int colorcnt = GetColorCount();
			Count = Convert.ToInt32(BufferWi*BufferHt*Count/2000) + 1;
			int TailLength = BufferWi*BufferHt*Length/2000 + 2;
			SnowstormClass ssItem;
			Point xy = new Point();
			int r;
			if (State == 0 || Count != LastSnowstormCount) {
				// create snowstorm elements
				LastSnowstormCount = Count;
				SnowstormItems.Clear();
				for (int i = 0; i < Count; i++) {
					ssItem = new SnowstormClass();
					ssItem.idx = i;
					ssItem.ssDecay = 0;
					ssItem.points.Clear();
					ssItem.hsv = HSV.SetRangeColor(hsv0, hsv1);
					// start in a random state
					r = rand()%(2*TailLength);
					if (r > 0) {
						xy.X = rand()%BufferWi;
						xy.Y = rand()%BufferHt;
						//ssItem.points.push_back(xy);
						ssItem.points.Add(xy);
					}
					if (r >= TailLength) {
						ssItem.ssDecay = r - TailLength;
						r = TailLength;
					}
					for (int j = 1; j < r; j++) {
						SnowstormAdvance(ssItem);
					}
					//SnowstormItems.push_back(ssItem);
					SnowstormItems.Add(ssItem);
				}
			}

			// render Snowstorm Items
			int sz;
			//int cnt = 0;
			//for (SnowstormList::iterator it = SnowstormItems.begin(); it != SnowstormItems.end(); ++it)
			foreach (SnowstormClass it in SnowstormItems) {
				if (it.points.Count() > TailLength) {
					if (it.ssDecay > TailLength) {
						it.points.Clear(); // start over
						it.ssDecay = 0;
					}
					else if (rand()%20 < Data.Speed) {
						it.ssDecay++;
					}
				}
				if (it.points.Count == 0) {
					xy.X = rand()%BufferWi;
					xy.Y = rand()%BufferHt;
					//it.points.push_back(xy);
					it.points.Add(xy);
				}
				else if (rand()%20 < Data.Speed) {
					SnowstormAdvance(it);
				}
				sz = it.points.Count();
				for (int pt = 0; pt < sz; pt++) {
					hsv = it.hsv;
					hsv.Value = (float) (1.0 - (double) (sz - pt + it.ssDecay)/TailLength);
					if (hsv.Value < 0.0) hsv.Value = 0.0f;
					SetPixel(it.points[pt].X, it.points[pt].Y, hsv);
				}
				//cnt++;
			}
		}

		#endregion // Snowstorm

		#region Spirals

		public void RenderSpirals(int PaletteRepeat, int Direction, int Rotation, int Thickness, bool Blend, bool Show3D)
		{
			int strand_base, strand, thick, x, y, ColorIdx;
			int colorcnt = GetColorCount();
			int SpiralCount = colorcnt*PaletteRepeat;
			int deltaStrands = BufferWi/SpiralCount;
			int SpiralThickness = (deltaStrands*Thickness/100) + 1;
			long SpiralState = State*Direction;
			HSV hsv;
			Color color;

			//timer.Reset(); timer.Start();

			for (int ns = 0; ns < SpiralCount; ns++) {
				strand_base = ns*deltaStrands;
				ColorIdx = ns%colorcnt;
				color = Palette.GetColor(ColorIdx);
				for (thick = 0; thick < SpiralThickness; thick++) {
					strand = (strand_base + thick)%BufferWi;
					for (y = 0; y < BufferHt; y++) {
						x = (strand + ((int) SpiralState/10) + (y*Rotation/BufferHt))%BufferWi;
						if (x < 0) x += BufferWi;
						if (Blend) {
							color = GetMultiColorBlend((double) (BufferHt - y - 1)/(double) BufferHt, false);
						}
						if (Show3D) {
							hsv = HSV.ColorToHSV(color);
							if (Rotation < 0) {
								hsv.Value = (float) ((double) (thick + 1)/SpiralThickness);
							}
							else {
								hsv.Value = (float) ((double) (SpiralThickness - thick)/SpiralThickness);
							}
							SetPixel(x, y, hsv);
						}
						else {
							SetPixel(x, y, color);
						}
					}
				}
			}

			//timer.Stop(); Console.WriteLine("RenderSpirals:" + timer.ElapsedMilliseconds);
		}

		#endregion // Spirals

		#region Twinkles

		public void RenderTwinkle(int count, int steps, bool strobe)
		{
			
			int y;
			int lights = Convert.ToInt32((BufferHt * BufferWi) * (count / 100.0)); ; // Count is in range of 1-100 from slider bar
			int step;
			if (lights > 0) step = BufferHt * BufferWi / lights;
			else step = 1;
			int maxModulo = steps;
			if (maxModulo < 2) maxModulo = 2;  // could we be getting 0 passed in?
			int maxModulo2 = maxModulo / 2;
			if (maxModulo2 < 1) maxModulo2 = 1;

			if (step < 1) step = 1;
			srand(strobe ? DateTime.Now.Millisecond : 1);

			int colorcnt = GetColorCount();

			int i = 0;

			for (y = 0; y < BufferHt; y++) // For my 20x120 megatree, BufferHt=120
			{
				int x;
				for (x = 0; x < BufferWi; x++) // BufferWi=20 in the above example
				{
					i++;

					if (i % step == 1 || step == 1) // Should we draw a light?
					{
						// Yes, so now decide on what color it should be

						int colorIdx = rand() % colorcnt;
						HSV hsv = Palette.GetHSV(colorIdx); //   we will define an hsv color model. 
						
						//  Note that we are adding state to this calculation, this causes a different blink rate for each light
						long i7 = (State + rand()) % maxModulo;

						if (i7 <= maxModulo2)
						{
							if (maxModulo2 > 0) hsv.Value = (float)((1.0 * i7) / maxModulo2);
							else hsv.Value = 0;
						}
						else
						{
							if (maxModulo2 > 0) hsv.Value = (float)((maxModulo - i7) * 1.0 / (maxModulo2));
							else hsv.Value = 0;
						}
						if (hsv.Value < 0.0) hsv.Value = 0.0f;

						if (strobe)
						{
							if (i7 == maxModulo2) hsv.Value = 1.0f;
							else hsv.Value = 0.0f;
						}


						//  we left the Hue and Saturation alone, we are just modifiying the Brightness Value
						SetPixel(x, y, hsv); // Turn pixel on
					}
				}
			}
		}

		#endregion // Twinkle

		#region Text

		// See partial class NutcrackerEffects_RenderText

		#endregion // Text

		#region Pictures

		
		//Refactored out to partial class Nutcracker_RenderPictures
		

		#endregion //Picture

		#region Spirograph

		public void RenderSpirograph(int int_R, int int_r, int int_d, bool Animate)
		{
			int i, x, y, xc, yc, ColorIdx;
			int mod1440, state360, d_mod;
			srand(1);
			float R, r, d, d_orig, t;
			double hyp, x2, y2;
			HSV hsv, hsv0, hsv1; //   we will define an hsv color model. The RGB colot model would have been "wxColour color;"
			int colorcnt = GetColorCount();

			xc = (int) (BufferWi/2); // 20x100 flex strips with 2 fols per strip = 40x50
			yc = (int) (BufferHt/2);
			R = (float) (xc*(int_R/100.0)); //  Radius of the large circle just fits in the width of model
			r = (float) (xc*(int_r/100.0)); // start little circle at 1/4 of max width
			if (r > R) r = R;
			d = (float) (xc*(int_d/100.0));

			//  palette.GetHSV(1, hsv1);
			//
			//    A hypotrochoid is a roulette traced by a point attached to a circle of radius r rolling around the inside of a fixed circle of radius R, where the point is a distance d from the center of the interior circle.
			//The parametric equations for a hypotrochoid are:[citation needed]
			//
			//  more info: http://en.wikipedia.org/wiki/Hypotrochoid
			//
			//x(t) = (R-r) * cos t + d*cos ((R-r/r)*t);
			//y(t) = (R-r) * sin t + d*sin ((R-r/r)*t);

			// Doesn't work with single strings so capture errors
			try {
				mod1440 = Convert.ToInt32(State%1440);
				state360 = Convert.ToInt32(State%360);
				d_orig = d;
				for (i = 1; i <= 360; i++) {
					if (Animate) d = (int) (d_orig + State/2)%100; // should we modify the distance variable each pass through?
					t = (float) ((i + mod1440)*Math.PI/180);
					x = Convert.ToInt32((R - r)*Math.Cos(t) + d*Math.Cos(((R - r)/r)*t) + xc);
					y = Convert.ToInt32((R - r)*Math.Sin(t) + d*Math.Sin(((R - r)/r)*t) + yc);

					if (colorcnt > 0) d_mod = (int) BufferWi/colorcnt;
					else d_mod = 1;

					x2 = Math.Pow((x - xc), 2);
					y2 = Math.Pow((y - yc), 2);
					hyp = (Math.Sqrt(x2 + y2)/BufferWi)*100.0;
					ColorIdx = (int) (hyp/d_mod);
					// Select random numbers from 0 up to number of colors the user has checked. 0-5 if 6 boxes checked

					if (ColorIdx >= colorcnt) ColorIdx = colorcnt - 1;

					hsv = Palette.GetHSV(ColorIdx); // Now go and get the hsv value for this ColorIdx


					hsv0 = Palette.GetHSV(0);
					ColorIdx = Convert.ToInt32((State + rand())%colorcnt);
					// Select random numbers from 0 up to number of colors the user has checked. 0-5 if 6 boxes checked
					hsv1 = Palette.GetHSV(ColorIdx); // Now go and get the hsv value for this ColorIdx

					SetPixel(x, y, hsv);
					//        if(i<=state360) SetPixel(x,y,hsv); // Turn pixel on
					//        else SetPixel(x,y,hsv1);
				}
			}
			catch {
			}
		}

		#endregion // Spirograph

		#region Tree

		public void RenderTree(int Branches)
		{
			int x, y, i, r, ColorIdx, pixels_per_branch;
			int maxFrame, mod, branch, row;
			int b, f_mod, m, frame;
			int number_garlands, f_mod_odd, s_odd_row, odd_even;
			float V, H;

			number_garlands = 1;
			srand(1); // always have the same random numbers for each frame (state)
			HSV hsv; //   we will define an hsv color model. The RGB colot model would have been "wxColour color;"
			pixels_per_branch = (int) (0.5 + BufferHt/Branches);
			if (pixels_per_branch == 0)
			{
				pixels_per_branch = 1;
			}
			maxFrame = (Branches + 1)*BufferWi;
			int colorcnt = GetColorCount();
			frame = (int) ((State/4)%maxFrame);

			i = 0;

			for (y = 0; y < BufferHt; y++) // For my 20x120 megatree, BufferHt=120
			{
				for (x = 0; x < BufferWi; x++) // BufferWi=20 in the above example
				{
					mod = y%pixels_per_branch;
					if (mod == 0) mod = pixels_per_branch;
					V = (float) (1 - (1.0*mod/pixels_per_branch)*0.70);
					i++;

					ColorIdx = rand()%colorcnt;
					// Select random numbers from 0 up to number of colors the user has checked. 0-5 if 6 boxes checked
					ColorIdx = 0;
					hsv = Palette.GetHSV(ColorIdx); // Now go and get the hsv value for this ColorIdx
					hsv.Value = V; // we have now set the color for the background tree

					//   $orig_rgbval=$rgb_val;
					branch = (int) ((y - 1)/pixels_per_branch);
					row = pixels_per_branch - mod; // now row=0 is bottom of branch, row=1 is one above bottom
					//  mod = which pixel we are in the branch
					//	mod=1,row=pixels_per_branch-1   top picrl in branch
					//	mod=2, second pixel down into branch
					//	mod=pixels_per_branch,row=0  last pixel in branch
					//
					//	row = 0, the $p is in the bottom row of tree
					//	row =1, the $p is in second row from bottom
					b = (int) ((State)/BufferWi)%Branches; // what branch we are on based on frame #
					//
					//	b = 0, we are on bottomow row of tree during frames 1 to BufferWi
					//	b = 1, we are on second row from bottom, frames = BufferWi+1 to 2*BufferWi
					//	b = 2, we are on third row from bottome, frames - 2*BufferWi+1 to 3*BufferWi
					f_mod = (int) ((State/4)%BufferWi);
					//   if(f_mod==0) f_mod=BufferWi;
					//	f_mod is  to BufferWi-1 on each row
					//	f_mod == 0, left strand of this row
					//	f_mod==BufferWi, right strand of this row
					//
					m = (x%6);
					if (m == 0) m = 6; // use $m to indicate where we are in horizontal pattern
					// m=1, 1sr strand
					// m=2, 2nd strand
					// m=6, last strand in 6 strand pattern

					r = branch%5;
					H = (float) (r/4.0);

					odd_even = b%2;
					s_odd_row = BufferWi - x + 1;
					f_mod_odd = BufferWi - f_mod + 1;

					if (branch <= b && x <= frame && // for branches below or equal to current row
					    (((row == 3 || (number_garlands == 2 && row == 6)) && (m == 1 || m == 6))
					     ||
					     ((row == 2 || (number_garlands == 2 && row == 5)) && (m == 2 || m == 5))
					     ||
					     ((row == 1 || (number_garlands == 2 && row == 4)) && (m == 3 || m == 4))
					    ))
						if ((odd_even == 0 && x <= f_mod) || (odd_even == 1 && s_odd_row <= f_mod)) {
							hsv.Hue = H;
							hsv.Saturation = 1.0f;
							hsv.Value = 1.0f;
						}
					//  we left the Hue and Saturation alone, we are just modifiying the Brightness Value
					SetPixel(x, y, hsv); // Turn pixel on
				}
			}
		}

		#endregion // Tree

		#region Movie

		private List<string> _moviePicturesFileList;

		private void LoadMoviePictureFileList(string dataFilePath)
		{
			if (Data.Movie_DataPath.Length > 0)
			{
				var imageFolder = Path.Combine(NutcrackerDescriptor.ModulePath, dataFilePath);
				_moviePicturesFileList = Directory.GetFiles(imageFolder).OrderBy(f => f).ToList();
			}
		} 

		private double currentMovieImageNum = 0.0;

		public void RenderMovie(int dir, string dataFilePath, int movieSpeed)
		{
			const int speedfactor = 4;

			if (_moviePicturesFileList == null || State == 0) {
				LoadMoviePictureFileList(dataFilePath);
			}

			// If we don't have any pictures, do nothing!
			if (_moviePicturesFileList == null || !_moviePicturesFileList.Any())
				return;

			int pictureCount = _moviePicturesFileList.Count;

			if (movieSpeed > 0) {
				currentMovieImageNum += ((movieSpeed*.01) + 1);
			}
			else if (movieSpeed < 0) {
				currentMovieImageNum += (100 + movieSpeed)*.01;
			}
			else {
				currentMovieImageNum++;
			}

			int currentImage = Convert.ToInt32(currentMovieImageNum);
			if (currentImage >= pictureCount || currentImage < 0)
				currentMovieImageNum = currentImage= 0;

			var currentMovieImage = new FastPixel.FastPixel(new Bitmap(Image.FromFile(_moviePicturesFileList[currentImage])));

			int imgwidth = currentMovieImage.Width;
			int imght = currentMovieImage.Height;
			int yoffset = (BufferHt + imght)/2;
			int xoffset = (imgwidth - BufferWi)/2;
			int limit = (dir < 2) ? imgwidth + BufferWi : imght + BufferHt;
			int movement = Convert.ToInt32((State%(limit*speedfactor))/speedfactor);

			// copy image to buffer
			currentMovieImage.Lock();
			Color fpColor = new Color();
			for (int x = 0; x < imgwidth; x++) {
				for (int y = 0; y < imght; y++) {
					fpColor = currentMovieImage.GetPixel(x, y);
					if (fpColor != Color.Transparent && fpColor != Color.Black) {
						switch (dir) {
							case 1:
								// left
								SetPixel(x + BufferWi - movement, yoffset - y, fpColor);
								break;
							case 2:
								// right
								SetPixel(x + movement - imgwidth, yoffset - y, fpColor);
								break;
							case 3:
								// up
								SetPixel(x - xoffset, movement - y, fpColor);
								break;
							case 4:
								// down
								SetPixel(x - xoffset, BufferHt + imght - y - movement, fpColor);
								break;
							default:
								// no movement - centered
								SetPixel(x - xoffset, yoffset - y, fpColor);
								break;
						}
					}
				}
			}
			currentMovieImage.Unlock(false);
			currentMovieImage.Dispose();
			
		}

		#endregion // Movie

		#region PictureTile

		private double movementX = 0.0;
		private double movementY = 0.0;
		private int lastState = 0;
		private double lastScale = -1;
		private string PictureTilePictureName = string.Empty;

		public void RenderPictureTile(int dir, double scale, bool useColor, bool useAlpha, int ColorReplacementSensitivity,
		                              string NewPictureName)
		{
			
			const int speedfactor = 4;
			Image image = null;
			try {

			
			if (NewPictureName == null || NewPictureName.Length == 0)
				return;

			if (State == 0) {
				lastState = 0;
				movementX = 0.0;
				movementY = 0.0;
				lastScale = -1;
			}

			if (NewPictureName != PictureTilePictureName || scale != lastScale) {
				if (IsNutcrackerResource(NewPictureName))
				{
					using (var fs = typeof(Nutcracker).Assembly.GetManifestResourceStream(NewPictureName))
					image = Image.FromStream(fs);     
				}
				else if (File.Exists(NewPictureName))
				{
					using (var fs = new FileStream(NewPictureName, FileMode.Open, FileAccess.Read))
					{
						image = Image.FromStream(fs);
					}
				}
				else
				{
					return;
				}
				if (scale != 0.0) {
					image = ScaleImage(image, scale/100);
				}
				if (useColor) {
					image = ConvertToGrayScale(image);
				}
				_fp = new FastPixel.FastPixel(new Bitmap(image));

				PictureTilePictureName = NewPictureName;
				lastScale = scale;
			}

			if (_fp != null) {
				int imageWidth = _fp.Width;
				int imageHeight = _fp.Height;
				double deltaX = 0;
				double deltaY = 0;

				if (dir > 0 && dir < 90) {
					deltaX = (double) dir/90;
				}
				else if (dir > 90 && dir < 180) {
					deltaX = (double) Math.Abs(dir - 180)/90;
				}
				else if (dir > 180 && dir < 270) {
					deltaX = -1*((double) Math.Abs(dir - 180)/90);
				}
				else if (dir > 270 && dir < 360) {
					deltaX = -1*((double) Math.Abs(dir - 360)/90);
				}

				if (dir >= 0 && dir < 90) {
					deltaY = (double) Math.Abs(dir - 90)/90;
				}
				else if (dir > 90 && dir <= 180) {
					deltaY = -1*((double) Math.Abs(dir - 90)/90);
				}
				else if (dir > 180 && dir < 270) {
					deltaY = -1*((double) Math.Abs(dir - 270)/90);
				}
				else if (dir > 270 && dir <= 360) {
					deltaY = (double) Math.Abs(270 - dir)/90;
				}

				movementX += (double) ((double) (State - lastState)/(double) speedfactor)*deltaX;
				movementY += (double) ((double) (State - lastState)/(double) speedfactor)*deltaY;
				lastState = (int) State;

				_fp.Lock();
				Color fpColor = new Color();
				int colorX = 0;
				int colorY = 0;

				for (int x = 0; x < BufferWi; x++) {
					for (int y = 0; y < BufferHt; y++) {
						colorX = x + Convert.ToInt32(movementX);
						colorY = y + Convert.ToInt32(movementY);

						if (colorX >= 0) {
							colorX = colorX%imageWidth;
						}
						else if (colorX < 0) {
							colorX = Convert.ToInt32(colorX%imageWidth) + imageWidth - 1;
						}

						if (colorY >= 0) {
							colorY = Convert.ToInt32(colorY%imageHeight);
						}
						else if (colorY < 0) {
							colorY = Convert.ToInt32(colorY%imageHeight) + imageHeight - 1;
						}
						if (colorX <= _fp.Width && colorY <= _fp.Height) {
							fpColor = _fp.GetPixel(colorX, colorY);

							if (fpColor != Color.Transparent) {
								// Are we re-assigning the colors for this
								if (useColor) {
									// For now, we're always going to use alpha. Maybe later, we'll 
									// Convert black and white to a single color
									//if (useAlpha)
									//{
									Color newColor = Palette.GetColor(0);
									// TODO
									// How high can GetBrightness go? Is it 0-100?
									// How about alpha? Is it 0-255?
									int a = Convert.ToInt32(fpColor.GetBrightness()*255);
									fpColor = Color.FromArgb(a, newColor.R, newColor.G, newColor.B);
									//}
									//else
									//{
									//    fpColor = Palette.GetColor(0);
									//}
								}
								else {
									//fpColor = Color.FromArgb(255, fpColor);
								}
								// Tree is up-side down, so draw the bitmp up-side down so it is right-side up.
								SetPixel(x, BufferHt - y - 1, fpColor);
							}
						}
					}
				}
				_fp.Unlock(false);
			}
			} finally {
				if (image != null)
					image.Dispose();
			}
		}

		#endregion //PictureTile

		#endregion // Nutcracker Effects

		public void RenderNextEffect(Effects effect)
		{
			RenderEffect(effect);
			SetNextState(false);
		}

		public void RenderEffect(Effects effect)
		{
			ClearPixels(Color.Transparent);
			switch (effect) {
				case Effects.Bars:
					RenderBars(Data.Bars_PaletteRepeat, Data.Bars_Direction, Data.Bars_Highlight, Data.Bars_3D);
					break;
				case Effects.Garlands:
					RenderGarlands(Data.Garland_Type, Data.Garland_Spacing);
					break;
				case Effects.Butterfly:
					RenderButterfly(Data.Butterfly_Colors, Data.Butterfly_Style, Data.Butterfly_BkgrdChunks, Data.Butterfly_BkgrdSkip, Data.Butterfly_Direction);
					break;
				case Effects.ColorWash:
					RenderColorWash(Data.ColorWash_FadeHorizontal, Data.ColorWash_FadeVertical, Data.ColorWash_Count);
					break;
				case Effects.Fire:
					RenderFire(Data.Fire_Height);
					break;
				case Effects.Life:
					RenderLife(Data.Life_CellsToStart, Data.Life_Type);
					break;
				case Effects.Meteors:
					RenderMeteors(Data.Meteor_Colors, Data.Meteor_Count, Data.Meteor_TrailLength);
					break;
				case Effects.Fireworks:
					RenderFireworks(Data.Fireworks_Explosions, Data.Fireworks_Particles, Data.Fireworks_Velocity, Data.Fireworks_Fade);
					break;
				case Effects.Snowflakes:
					RenderSnowflakes(Data.Snowflakes_Max, Data.Snowflakes_Type);
					break;
				case Effects.Snowstorm:
					RenderSnowstorm(Data.Snowstorm_MaxFlakes, Data.Snowstorm_TrailLength);
					break;
				case Effects.Spirals:
					RenderSpirals(Data.Spirals_PaletteRepeat, Data.Spirals_Direction, Data.Spirals_Rotation, Data.Spirals_Thickness,
					              Data.Spirals_Blend, Data.Spirals_3D);
					break;
				case Effects.Twinkles:
					RenderTwinkle(Data.Twinkles_Count, Data.Twinkles_Steps, Data.Twinkles_Strobe);
					break;
				case Effects.Text:
					RenderText(Data.Text_Top, Data.Text_Line1, Data.Text_Line2, Data.Text_Line3, Data.Text_Line4, Data.Text_Font, Data.Text_Direction,
					           Data.Text_TextRotation, Data.Text_CenterStop);
					break;
				case Effects.Picture:
					RenderPictures(Data.Picture_Direction, Data.Picture_FileName, Data.Picture_GifSpeed, Data.Picture_ScaleToGrid, Data.Picture_ScalePercent);
					break;
				case Effects.Spirograph:
					RenderSpirograph(Data.Spirograph_ROuter, Data.Spirograph_RInner, Data.Spirograph_Distance, Data.Spirograph_Animate);
					break;
				case Effects.Tree:
					RenderTree(Data.Tree_Branches);
					break;
				case Effects.Movie:
					RenderMovie(Data.Movie_MovementDirection, Data.Movie_DataPath, Data.Movie_PlaybackSpeed);
					break;
				case Effects.PictureTile:
					RenderPictureTile(Data.PictureTile_Direction, Data.PictureTile_Scaling, Data.PictureTile_ReplaceColor,
					                  Data.PictureTile_UseSaturation, Data.PictureTile_ColorReplacementSensitivity,
					                  Data.PictureTile_FileName);
					break;
				case Effects.Curtain:
					RenderCurtain(Data.Curtain_Edge,(CurtainType)Data.Curtain_Effect, Data.Curtain_SwagWidth, Data.Curtain_Repeat);
					break;
				case Effects.Glediator:
					RenderGlediator(Data.Glediator_FileName);
					break;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this); 
		}

		protected virtual void Dispose(bool disposing)
		{
			
			if (_pictureImage != null)
			{
				_pictureImage.Dispose();
			}	
				
		}
	}
}