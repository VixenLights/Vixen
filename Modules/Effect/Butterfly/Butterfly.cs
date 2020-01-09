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
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Butterfly
{
	public class Butterfly:PixelEffectBase
	{
		private ButterflyData _data;
		private const double pi2 = 6.283185307;
		private double _position;
		private bool _negPosition;

		public Butterfly()
		{
			_data = new ButterflyData();
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
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[PropertyOrder(1)]
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

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MovementType")]
		[ProviderDescription(@"MovementType")]
		[PropertyOrder(2)]
		public MovementType MovementType
		{
			get { return _data.MovementType; }
			set
			{
				_data.MovementType = value;
				IsDirty = true;
				UpdateMovementTypeAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Iterations")]
		[ProviderDescription(@"Iterations")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(3)]
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
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyOrder(4)]
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
		[ProviderDisplayName(@"Repeat")]
		[ProviderDescription(@"Repeat")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(5)]
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
		[PropertyOrder(6)]
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
		[PropertyOrder(7)]
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
				UpdateGradientColorAttribute();
				OnPropertyChanged();
			}
		}

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

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/butterfly/"; }
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as ButterflyData;
				InitAllAttributes();
				IsDirty = true;
			}
		}

		private void InitAllAttributes()
		{
			UpdateGradientColorAttribute(false);
			UpdateStringOrientationAttributes();
			UpdateMovementTypeAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		private void UpdateMovementTypeAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{ "SpeedCurve", MovementType == MovementType.Speed},
				{ "Iterations", MovementType != MovementType.Speed},
				{ "Direction", MovementType != MovementType.Speed}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateGradientColorAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"Color", ColorScheme == ColorScheme.Gradient}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		protected override void SetupRender()
		{
			_position = 0;
		}

		protected override void CleanUpRender()
		{
			//Not required
		}

		protected override void RenderEffect(int effectFrame, IPixelFrameBuffer frameBuffer)
		{
			
			int repeat = ConfigureRepeat();
			int maxframe=BufferHt;
			double position;
			double intervalPosFactor = GetEffectTimeIntervalPosition(effectFrame) * 100;

			if (MovementType == MovementType.Iterations)
			{
				position = (GetEffectTimeIntervalPosition(effectFrame) * Iterations) % 1;
			}
			else
			{
				_position += CalculateSpeed(intervalPosFactor) / 200;
				if (_position < 0)
				{
					_negPosition = true;
					position = -_position % 1;
				}
				else
				{
					_negPosition = false;
					position = _position % 1;
				}
			}
			
			int curState = (int)(TimeSpan.TotalMilliseconds* position * repeat);
			int frame = (BufferHt * curState / (int)TimeSpan.TotalMilliseconds) % maxframe;
			double offset=curState/TimeSpan.TotalMilliseconds;
			double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100) / 100;
			
			switch (MovementType)
			{
				case MovementType.Speed:
				{
					if (!_negPosition) offset = -offset;
					break;
				}
				default:
				{
					if (Direction == Direction.Forward)
					{
						offset = -offset;
					}
					break;
				}
			}

			int bufferDim = 0;
			if (ButterflyType == ButterflyType.Type1 || ButterflyType == ButterflyType.Type4)
			{
				bufferDim = BufferHt + BufferWi;
			}
			else if(ButterflyType == ButterflyType.Type5)
			{
				bufferDim = BufferHt*BufferWi;
			}
			for (int x=0; x<BufferWi; x++)
			{
				for (int y=0; y<BufferHt; y++)
				{
					CalculatePixel(x, y, bufferDim, offset, frame, maxframe, level, frameBuffer);
				}
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			int repeat = ConfigureRepeat();
			int maxframe = BufferHt;
			double position;

			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);

			for (int effectFrame = 0; effectFrame < numFrames; effectFrame++)
			{
				frameBuffer.CurrentFrame = effectFrame;

				double intervalPosFactor = GetEffectTimeIntervalPosition(effectFrame) * 100;

				if (MovementType == MovementType.Iterations)
				{
					position = (GetEffectTimeIntervalPosition(effectFrame) * Iterations) % 1;
				}
				else
				{
					_position += CalculateSpeed(intervalPosFactor) / 200;
					if (_position < 0)
					{
						_negPosition = true;
						position = -_position % 1;
					}
					else
					{
						_negPosition = false;
						position = _position % 1;
					}
				}

				int curState = (int)(TimeSpan.TotalMilliseconds * position * repeat);
				int frame = (BufferHt * curState / (int)TimeSpan.TotalMilliseconds) % maxframe;
				double offset = curState / TimeSpan.TotalMilliseconds;
				double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100) / 100;

				switch (MovementType)
				{
					case MovementType.Speed:
					{
						if (!_negPosition) offset = -offset;
						break;
					}
					default:
					{
						if (Direction == Direction.Forward)
						{
							offset = -offset;
						}
						break;
					}
				}

				int bufferDim = 0;
				if (ButterflyType == ButterflyType.Type1 || ButterflyType == ButterflyType.Type4)
				{
					bufferDim = BufferHt + BufferWi;
				}
				else if (ButterflyType == ButterflyType.Type5)
				{
					bufferDim = BufferHt * BufferWi;
				}

				foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
				{
					foreach (var elementLocation in elementLocations)
					{
						CalculatePixel(elementLocation.X, elementLocation.Y, bufferDim, offset, frame, maxframe, level, frameBuffer);
					}
				}
			}
			
		}


		private void CalculatePixel(int x, int y, float bufferDim, double offset, int frame, int maxframe, 
			double level, IPixelFrameBuffer frameBuffer)
		{
			double n;
			double x1;
			double y1;
			double f;
			int d;
			int x0;
			int y0;
			double h = 0.0;
			int yCoord = y;
			int xCoord = x;
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				//Flip me over so and offset my coordinates I can act like the string version
				y = Math.Abs((BufferHtOffset-y) + (BufferHt-1+BufferHtOffset));
				y = y - BufferHtOffset;
				x = x - BufferWiOffset;
			}
			switch (ButterflyType)
			{
				case ButterflyType.Type1:
					//  http://mathworld.wolfram.com/ButterflyFunction.html
					n = Math.Abs((x * x - y * y) * Math.Sin(offset + ((x + y) * pi2 / (bufferDim))));
					d = x * x + y * y;

					//  This section is to fix the colors on pixels at {0,1} and {1,0}
					if ((x == 0 && y == 1))
					{
						y0 = y + 1;
						n = Math.Abs((x * x - y0 * y0) * Math.Sin(offset + ((x + y0) * pi2 / (bufferDim))));
						d = x * x + y0 * y0;
					}
					if ((x == 1 && y == 0))
					{
						x0 = x + 1;
						n = Math.Abs((x0 * x0 - y * y) * Math.Sin(offset + ((x0 + y) * pi2 / (bufferDim))));
						d = x0 * x0 + y * y;
					}
					// end of fix

					h = d > 0.001 ? n / d : 0.0;
					break;

				case ButterflyType.Type2:
					f = (frame < maxframe / 2) ? frame + 1 : maxframe - frame;
					x1 = (x - BufferWi / 2.0) / f;
					y1 = (y - BufferHt / 2.0) / f;
					h = Math.Sqrt(x1 * x1 + y1 * y1);
					break;

				case ButterflyType.Type3:
					f = (frame < maxframe / 2) ? frame + 1 : maxframe - frame;
					f = f * 0.1 + BufferHt / 60.0;
					x1 = (x - BufferWi / 2.0) / f;
					y1 = (y - BufferHt / 2.0) / f;
					h = Math.Sin(x1) * Math.Cos(y1);
					break;

				case ButterflyType.Type4:
					//  http://mathworld.wolfram.com/ButterflyFunction.html
					n = ((x * x - y * y) * Math.Sin(offset + ((x + y) * pi2 / (bufferDim))));
					d = x * x + y * y;

					//  This section is to fix the colors on pixels at {0,1} and {1,0}
					x0 = x + 1;
					y0 = y + 1;
					if ((x == 0 && y == 1))
					{
						n = ((x * x - y0 * y0) * Math.Sin(offset + ((x + y0) * pi2 / (bufferDim))));
						d = x * x + y0 * y0;
					}
					if ((x == 1 && y == 0))
					{
						n = ((x0 * x0 - y * y) * Math.Sin(offset + ((x0 + y) * pi2 / (bufferDim))));
						d = x0 * x0 + y * y;
					}
					// end of fix

					h = d > 0.001 ? n / d : 0.0;

					var fractpart = h - (Math.Floor(h));
					h = fractpart;
					if (h < 0) h = 1.0 + h;
					break;

				case ButterflyType.Type5:
					//  http://mathworld.wolfram.com/ButterflyFunction.html
					n = Math.Abs((x * x - y * y) * Math.Sin(offset + ((x + y) * pi2 / (bufferDim))));
					d = x * x + y * y;

					//  This section is to fix the colors on pixels at {0,1} and {1,0}
					x0 = x + 1;
					y0 = y + 1;
					if ((x == 0 && y == 1))
					{
						n = Math.Abs((x * x - y0 * y0) * Math.Sin(offset + ((x + y0) * pi2 / (bufferDim))));
						d = x * x + y0 * y0;
					}
					if ((x == 1 && y == 0))
					{
						n = Math.Abs((x0 * x0 - y * y) * Math.Sin(offset + ((x0 + y) * pi2 / (bufferDim))));
						d = x0 * x0 + y * y;
					}
					// end of fix

					h = d > 0.001 ? n / d : 0.0;
					break;

			}
			
			if (BackgroundChunks <= 1 || (int) (h*BackgroundChunks)%BackgroundSkips != 0)
			{
				if (ColorScheme == ColorScheme.Gradient)
				{
					Color color = Color.GetColorAt(h);
					if (level < 1)
					{
						HSV hsv = HSV.FromRGB(color);
						hsv.V = hsv.V * level;
						frameBuffer.SetPixel(xCoord, yCoord, hsv);
					}
					else
					{
						frameBuffer.SetPixel(xCoord, yCoord, color);
					}

				}
				else
				{
					HSV hsv = new HSV(h, 1.0, level);
					frameBuffer.SetPixel(xCoord, yCoord, hsv);
				}
				
			}
			else
			{
				frameBuffer.SetPixel(xCoord, yCoord, UseBaseColor?BaseColor:System.Drawing.Color.Transparent);
			}
		}

		private int ConfigureRepeat()
		{
			int repeat = Repeat;
			switch (ButterflyType)
			{
				case ButterflyType.Type1:
				case ButterflyType.Type5:
					repeat = Repeat*3;
					break;
				case ButterflyType.Type4:
					repeat = Repeat*6;
					break;
			}
			return repeat;
		}

		private double CalculateSpeed(double intervalPos)
		{
			return ScaleCurveToValue(SpeedCurve.GetValue(intervalPos), 80, -80)  * FrameTime / 50d;
		}
	}
}
