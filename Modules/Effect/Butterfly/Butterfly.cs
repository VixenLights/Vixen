using System;
using System.ComponentModel;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Pixel;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Butterfly
{
	public class Butterfly:PixelEffectBase
	{
		private ButterflyData _data;
		private const double pi2 = 6.283185307;
		
		public Butterfly()
		{
			_data = new ButterflyData();
		}

		[Browsable(false)]
		public override StringOrientation StringOrientation
		{
			get { return StringOrientation.Vertical; }
			set
			{
				
			}
		}

		#region Config properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ButterflyType")]
		[ProviderDescription(@"ButterflyType")]
		[PropertyOrder(0)]
		public ButterflyType ButterflyType
		{
			get { return _data.ButterflyType; }
			set
			{
				_data.ButterflyType = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Repeat")]
		[ProviderDescription(@"Repeat")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(1)]
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
		[ProviderDisplayName(@"BackgroundSkips")]
		[ProviderDescription(@"BackgroundSkips")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(2, 10, 1)]
		[PropertyOrder(2)]
		public int BackgroundSkips
		{
			get { return _data.BackgroundSkips; }
			set
			{
				_data.BackgroundSkips = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BackgroundChunks")]
		[ProviderDescription(@"BackgroundChunks")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 10, 1)]
		[PropertyOrder(2)]
		public int BackgroundChunks
		{
			get { return _data.BackgroundChunks; }
			set
			{
				_data.BackgroundChunks = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[PropertyOrder(3)]
		public Direction Direction
		{
			get { return _data.Direction; }
			set
			{
				_data.Direction = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"ColorScheme")]
		[ProviderDescription(@"ColorScheme")]
		[PropertyOrder(1)]
		public ColorScheme ColorScheme
		{
			get { return _data.ColorScheme; }
			set
			{
				_data.ColorScheme = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"Color")]
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
				_data = value as ButterflyData;
				IsDirty = true;
			}
		}

		protected override void SetupRender()
		{
			//Not required
		}

		protected override void CleanUpRender()
		{
			//Not required
		}

		protected override void RenderEffect(int effectFrame, ref PixelFrameBuffer frameBuffer)
		{
			int repeat = Repeat;
			switch (ButterflyType)
			{
				case ButterflyType.Type1 :
				case ButterflyType.Type5:
					repeat = Repeat*3;
					break;
				case ButterflyType.Type4:
					repeat = Repeat*6;
					break;
			}

			double h=0.0;
			int maxframe=BufferHt;
			double position = GetEffectTimeIntervalPosition(effectFrame);
			int curState = (int)(TimeSpan.TotalMilliseconds*position*repeat);
			int frame = (BufferHt * curState / (int)TimeSpan.TotalMilliseconds) % maxframe;
			double offset=curState/TimeSpan.TotalMilliseconds;
    
    
			if(Direction==Direction.Forward) offset = -offset;
			for (int x=0; x<BufferWi; x++)
			{
				int y;
				for (y=0; y<BufferHt; y++)
				{
					double n;
					double x1;
					double y1;
					double f;
					int d;
					int x0;
					int y0;
					switch (ButterflyType)
					{
					case ButterflyType.Type1:
						//  http://mathworld.wolfram.com/ButterflyFunction.html
						n = Math.Abs((x*x - y*y) * Math.Sin(offset + ((x+y)*pi2 / (BufferHt+BufferWi))));
						d = x*x + y*y;

						//  This section is to fix the colors on pixels at {0,1} and {1,0}
						x0=x+1;
						y0=y+1;
						if((x==0 && y==1))
						{
							n = Math.Abs((x*x - y0*y0) * Math.Sin (offset + ((x+y0)*pi2 / (BufferHt+BufferWi))));
							d = x*x + y0*y0;
						}
						if((x==1 && y==0))
						{
							n = Math.Abs((x0*x0 - y*y) * Math.Sin (offset + ((x0+y)*pi2 / (BufferHt+BufferWi))));
							d = x0*x0 + y*y;
						}
						// end of fix

						h=d>0.001 ? n/d : 0.0;
						break;

					case ButterflyType.Type2:
						f=(frame < maxframe/2) ? frame+1 : maxframe - frame;
						x1=(x -BufferWi/2.0)/f;
						y1=(y-BufferHt/2.0)/f;
						h=Math.Sqrt(x1*x1+y1*y1);
						break;

					case ButterflyType.Type3:
						f=(frame < maxframe/2) ? frame+1 : maxframe - frame;
						f=f*0.1+BufferHt/60.0;
						x1 = (x-BufferWi/2.0)/f;
						y1 = (y-BufferHt/2.0)/f;
						h=Math.Sin(x1) * Math.Cos(y1);
						break;

					case ButterflyType.Type4:
						//  http://mathworld.wolfram.com/ButterflyFunction.html
						n = ((x*x - y*y) * Math.Sin (offset + ((x+y)*pi2 / (BufferHt+BufferWi))));
						d = x*x + y*y;

						//  This section is to fix the colors on pixels at {0,1} and {1,0}
						x0=x+1;
						y0=y+1;
						if((x==0 && y==1))
						{
							n = ((x*x - y0*y0) * Math.Sin (offset + ((x+y0)*pi2 / (BufferHt+BufferWi))));
							d = x*x + y0*y0;
						}
						if((x==1 && y==0))
						{
							n = ((x0*x0 - y*y) * Math.Sin (offset + ((x0+y)*pi2 / (BufferHt+BufferWi))));
							d = x0*x0 + y*y;
						}
						// end of fix

						h=d>0.001 ? n/d : 0.0;
						
						var  fractpart = h - (Math.Floor(h));
						h=fractpart;
						if(h<0) h=1.0+h;
						break;

					case ButterflyType.Type5:
						//  http://mathworld.wolfram.com/ButterflyFunction.html
							n = Math.Abs((x*x - y*y) * Math.Sin (offset + ((x+y)*pi2 / (BufferHt*BufferWi))));
						d = x*x + y*y;

						//  This section is to fix the colors on pixels at {0,1} and {1,0}
						x0=x+1;
						y0=y+1;
						if((x==0 && y==1))
						{
							n = Math.Abs((x*x - y0*y0) * Math.Sin (offset + ((x+y0)*pi2 / (BufferHt*BufferWi))));
							d = x*x + y0*y0;
						}
						if((x==1 && y==0))
						{
							n = Math.Abs((x0*x0 - y*y) * Math.Sin (offset + ((x0+y)*pi2 / (BufferHt*BufferWi))));
							d = x0*x0 + y*y;
						}
						// end of fix

						h=d>0.001 ? n/d : 0.0;
						break;

					}
					HSV hsv = new HSV(h, 1.0, 1.0);
					double level = LevelCurve.GetValue(position * 100) / 100;
					if (BackgroundChunks <= 1 || (int)(h*BackgroundChunks) % BackgroundSkips != 0)
					{
						if (ColorScheme == ColorScheme.Gradient)
						{
							Color color = Color.GetColorAt(h);
							hsv = HSV.FromRGB(color);
						}
						hsv.V = hsv.V * level;
						frameBuffer.SetPixel(x, y, hsv);
					}
				}
			}
		}

		
	}
}
