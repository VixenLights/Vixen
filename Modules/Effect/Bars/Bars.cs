using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using Common.Controls.ColorManagement.ColorModels;
using NLog;
using Vixen.Attributes;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using Vixen.TypeConverters;
using VixenModules.App.ColorGradients;
using VixenModules.Effect.Pixel;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Bars
{
	public class Bars:PixelEffectBase
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private BarsData _data;
		private EffectIntents _elementData;

		public Bars()
		{
			_data = new BarsData();
		}

		protected override void _PreRender(CancellationTokenSource tokenSource = null)
		{
			_elementData = new EffectIntents();

			foreach (ElementNode node in TargetNodes)
			{
				if (tokenSource != null && tokenSource.IsCancellationRequested)
					return;

				if (node != null)
					RenderNode(node);
			}
		}

		public override bool IsDirty
		{
			get
			{
				if(Colors.Any(x => !x.CheckLibraryReference()))
				{
					base.IsDirty = true;
				}

				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
		}

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		[Value]
		[ProviderCategory(@"Config", 0)]
		[ProviderDisplayName(@"Orientation")]
		[ProviderDescription(@"Orientation")]
		//[PropertyOrder(1)]
		public StringOrientation Orientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Direction", 0)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		//[PropertyOrder(1)]
		public BarDirection Direction
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
		[ProviderCategory(@"Color", 1)]
		[ProviderDisplayName(@"Color")]
		[ProviderDescription(@"Color")]
		//[PropertyOrder(1)]
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

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Color")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1,20,1)]
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
		[ProviderDisplayName(@"Repeat")]
		[ProviderDescription(@"Repeat")]
		[PropertyEditor("SliderEditor")]
		public int Repeat
		{
			get { return _data.Repeat; }
			set
			{
				_data.Repeat = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Highlight")]
		[ProviderDescription(@"Highlight")]
		public bool Highlight
		{
			get { return _data.Highlight; }
			set
			{
				_data.Highlight = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Show3D")]
		[ProviderDescription(@"Show3D")]
		public bool Show3D
		{
			get { return _data.Show3D; }
			set
			{
				_data.Show3D = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as BarsData;
				IsDirty = true;
				//InitAllAttributes();
			}
		}

		
		private void RenderNode(ElementNode node)
		{
			var frameMs = 50;
			int wid;
			int ht;
			if (Orientation ==StringOrientation.Horizontal)
			{
				wid = MaxPixelsPerString;
				ht = StringCount;
			}
			else
			{
				wid = StringCount;
				ht = MaxPixelsPerString;
			}
			int nFrames = (int)(TimeSpan.TotalMilliseconds / 50);
			
			InitBuffer();
			int numElements = node.Count();
			
			TimeSpan startTime = TimeSpan.Zero;
			
			// OK, we're gonna create 1 intent per element
			// that intent will hold framesToRender Color values
			// that it will parcel out as intent states are called for...
			
			// set up arrays to hold the generated colors
			var pixels = new RGBValue[numElements][];
			for (int eidx = 0; eidx < numElements; eidx++)
				pixels[eidx] = new RGBValue[nFrames];
			List<ElementNode> nodes = FindLeafParents().ToList();
			// generate all the pixels
			for (int frameNum = 0; frameNum < nFrames; frameNum++)
			{
				RenderBars(frameNum);
				// peel off this frames pixels...
				if (Orientation == StringOrientation.Horizontal)
				{
					int i = 0;
					for (int y = 0; y < ht; y++)
					{
						for (int x = 0; x < StringPixelCounts[y]; x++)
						{
							pixels[i][frameNum] = _pixels[x][y];
							i++;
						}
					}
				}
				else
				{
					int i = 0;
					for (int x = 0; x < wid; x++)
					{
						for (int y = 0; y < StringPixelCounts[x]; y++)
						{
							pixels[i][frameNum] = _pixels[x][y];
							i++;
						}
					}
				}
			};

			// create the intents
			var frameTs = new TimeSpan(0, 0, 0, 0, frameMs);
			List<Element> elements = node.ToList();
			for (int eidx = 0; eidx < numElements; eidx++)
			{
				IIntent intent = new StaticArrayIntent<RGBValue>(frameTs, pixels[eidx], TimeSpan);
				_elementData.AddIntentForElement(elements[eidx].Id, intent, startTime);
			}

			
		
		}

		private double GetEffectTimeIntervalPosition(int frame)
		{
			double retval;
			if (TimeSpan == TimeSpan.Zero)
			{
				retval = 1;
			}
			else
			{
				retval = frame / (TimeSpan.TotalMilliseconds / 50);
			}
			return retval;
		}

		private RGBValue[][] _pixels;
		public void InitBuffer()
		{
			
			if (Orientation == StringOrientation.Horizontal)
			{
				_pixels = new RGBValue[MaxPixelsPerString][];
				for (int i = 0; i < MaxPixelsPerString; i++)
				{
					_pixels[i] = new RGBValue[StringCount];
				}	
			}
			else
			{
				_pixels = new RGBValue[StringCount][];
				for (int i = 0; i < StringCount; i++)
				{
					_pixels[i] = new RGBValue[MaxPixelsPerString];
				}
			}
		}

		public int BufferHt
		{
			get
			{
				return Orientation == StringOrientation.Horizontal?StringCount:MaxPixelsPerString;	
			} 
			
		}

		public int BufferWi
		{
			get
			{
				return Orientation == StringOrientation.Horizontal ? MaxPixelsPerString : StringCount;
			}
		}

		// 0,0 is lower left
		public void SetPixel(int x, int y, Color color)
		{
			if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt)
			{
				_pixels[x][y] = new RGBValue(color);
			}
		}

		// 0,0 is lower left
		public void SetPixel(int x, int y, HSV hsv)
		{
			Color color = hsv.ToRGB().ToArgb();
			SetPixel(x, y, color);
		}

		public void RenderBars(int frame)
		{
			int x, y, n, colorIdx;
			//HSV hsv;
			int colorcnt = Colors.Count();
			int barCount = Repeat * colorcnt;
			double position = GetEffectTimeIntervalPosition(frame);
			if (barCount < 1) barCount = 1;


			if (Direction < BarDirection.Left || Direction == BarDirection.AlternateUp || Direction == BarDirection.AlternateDown)
			{
				int barHt = BufferHt / barCount + 1;
				if (barHt < 1) barHt = 1;
				int halfHt = BufferHt / 2;
				int blockHt = colorcnt * barHt;
				if (blockHt < 1) blockHt = 1;
				int fOffset = Convert.ToInt32(position * blockHt);
				fOffset = Convert.ToInt32(Direction == BarDirection.AlternateUp || Direction == BarDirection.AlternateDown ? (Speed*frame / 20) * barHt : fOffset);
				//direction = direction > 4 ? direction - 8 : direction;
				
				for (y = 0; y < BufferHt; y++)
				{
					n = y + fOffset;
					colorIdx = (n % blockHt) / barHt;
					Color c = Colors[colorIdx].GetColorAt( (((double)n % blockHt) / barHt) - colorIdx  );
					var hsv = HSV.FromRGB(c);
					if (Highlight && n % barHt == 0) hsv.S = 0.0f;
					if (Show3D) hsv.V *= (float)(barHt - n % barHt - 1) / barHt;
					switch (Direction)
					{
						case BarDirection.Down:
						case BarDirection.AlternateDown:
							// down
							for (x = 0; x < BufferWi; x++)
							{
								SetPixel(x, y, hsv);
							}
							break;
						case BarDirection.Expand:
							// expand
							if (y <= halfHt)
							{
								for (x = 0; x < BufferWi; x++)
								{
									SetPixel(x, y, hsv);
									SetPixel(x, BufferHt - y - 1, hsv);
								}
							}
							break;
						case BarDirection.Compress:
							// compress
							if (y >= halfHt)
							{
								for (x = 0; x < BufferWi; x++)
								{
									SetPixel(x, y, hsv);
									SetPixel(x, BufferHt - y - 1, hsv);
								}
							}
							break;
						default:
							// up & AlternateUp
							for (x = 0; x < BufferWi; x++)
							{
								SetPixel(x, BufferHt - y - 1, hsv);
							}
							break;
					}
				}
			}
			else
			{
				int barWi = BufferWi / barCount + 1;
				if (barWi < 1) barWi = 1;
				int halfWi = BufferWi / 2;
				int blockWi = colorcnt * barWi;
				if (blockWi < 1) blockWi = 1;
				int fOffset = Convert.ToInt32(position * blockWi);
				fOffset = Convert.ToInt32(Direction > BarDirection.AlternateDown ? (Speed*frame / 20) * barWi : fOffset);
				//direction = direction > 9 ? direction - 6 : direction;
				for (x = 0; x < BufferWi; x++)
				{
					n = x + fOffset;
					colorIdx = (n % blockWi) / barWi;
					Color c = Colors[colorIdx].GetColorAt((((double)n % blockWi) / barWi) - colorIdx);
					var hsv = HSV.FromRGB(c);
					if (Highlight && n % barWi == 0) hsv.S = 0.0f;
					if (Show3D) hsv.V *= (float)(barWi - n % barWi - 1) / barWi;
					switch (Direction)
					{
						case BarDirection.Right:
						case BarDirection.AlternateRight:
							// right
							for (y = 0; y < BufferHt; y++)
							{
								SetPixel(BufferWi - x - 1, y, hsv);
							}
							break;
						case BarDirection.HExpand:
							// H-expand
							if (x <= halfWi)
							{
								for (y = 0; y < BufferHt; y++)
								{
									SetPixel(x, y, hsv);
									SetPixel(BufferWi - x - 1, y, hsv);
								}
							}
							break;
						case BarDirection.HCompress:
							// H-compress
							if (x >= halfWi)
							{
								for (y = 0; y < BufferHt; y++)
								{
									SetPixel(x, y, hsv);
									SetPixel(BufferWi - x - 1, y, hsv);
								}
							}
							break;
						default:
							// left & AlternateLeft
							for (y = 0; y < BufferHt; y++)
							{
								SetPixel(x, y, hsv);
							}
							break;
					}
				}
			}
		}
	}
}
