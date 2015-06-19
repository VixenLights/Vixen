using System;
using System.CodeDom;
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
		private BarsData _data;
		
		public Bars()
		{
			_data = new BarsData();
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

		
		[Value]
		[ProviderCategory(@"Config", 0)]
		[ProviderDisplayName(@"Orientation")]
		[ProviderDescription(@"Orientation")]
		[Browsable(false)]
		public StringOrientation Orientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				StringOrientation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[PropertyOrder(0)]
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Color")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(3)]
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
		[ProviderDisplayName(@"Repeat")]
		[ProviderDescription(@"Repeat")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 10, 1)]
		[PropertyOrder(2)]
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
		[PropertyOrder(4)]
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
		[PropertyOrder(4)]
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

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"FitTime")]
		[ProviderDescription(@"FitTime")]
		[PropertyOrder(6)]
		public bool FitToTime
		{
			get { return _data.FitToTime; }
			set
			{
				_data.FitToTime = value;
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
			}
		}

		protected override void RenderEffect(int frame)
		{
			int x, y, n, colorIdx;
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
				int fOffset = Convert.ToInt32(FitToTime ? position * blockHt : Speed * frame / 4 % blockHt);
				fOffset = Convert.ToInt32(Direction == BarDirection.AlternateUp || Direction == BarDirection.AlternateDown ? (Speed*frame / 20) * barHt : fOffset);
				
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
				int fOffset = Convert.ToInt32(FitToTime ? position * blockWi : Speed * frame / 4 % blockWi);
				fOffset = Convert.ToInt32(Direction > BarDirection.AlternateDown ? (Speed*frame / 20) * barWi : fOffset);
				
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
