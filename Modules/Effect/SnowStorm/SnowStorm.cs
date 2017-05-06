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

namespace VixenModules.Effect.SnowStorm
{
	public class SnowStorm : PixelEffectBase
	{
		private SnowStormData _data;
		private static Random _random = new Random();
		private double _gradientPosition = 0;
		private readonly List<SnowstormClass> _snowstormItems = new List<SnowstormClass>();
		private int _lastSnowstormCount = 0;

		public SnowStorm()
		{
			_data = new SnowStormData();
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
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
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
		[ProviderDisplayName(@"Count")]
		[ProviderDescription(@"Count")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(2)]
		public int Count
		{
			get { return _data.Count; }
			set
			{
				_data.Count = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Length")]
		[ProviderDescription(@"Length")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(3)]
		public int Length
		{
			get { return _data.Length; }
			set
			{
				_data.Length = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Reverse Direction")]
		[ProviderDescription(@"Reverse Direction")]
		[PropertyOrder(5)]
		public bool ReverseDirection
		{
			get { return _data.ReverseDirection; }
			set
			{
				_data.ReverseDirection = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"Color Type")]
		[ProviderDescription(@"Color Type")]
		[PropertyOrder(0)]
		public SnowStormColorType ColorType
		{
			get { return _data.ColorType; }
			set
			{
				_data.ColorType = value;
				IsDirty = true;
				UpdateColorAttribute();
				OnPropertyChanged();
			}
		}

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
				_data = value as SnowStormData;
				UpdateAttributes();
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private void UpdateAttributes()
		{
			UpdateColorAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		private void UpdateColorAttribute(bool refresh = true)
		{
			bool meteorType = ColorType != SnowStormColorType.RainBow;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			propertyStates.Add("Colors", meteorType);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		protected override void SetupRender()
		{
			//Nothing to setup
		}

		protected override void CleanUpRender()
		{
			//Nothing to clean up
		}

		private class SnowstormClass
		{
			public readonly List<Point> Points = new List<Point>();
			public HSV Hsv;
			public int Idx, SsDecay;
			public int Color;
			public bool Expired;

			public SnowstormClass()
			{
				Points.Clear();
			}
		};

		private Point SnowstormVector(int idx)
		{
			Point xy = new Point();
			switch (idx)
			{
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
			int[] arr = { 30, 20, 10, 5, 0, 5, 10, 20, 20, 15, 10, 10, 10, 10, 10, 15 };
			// 2 sets of 8 numbers, each of which add up to 100
			Point adv = SnowstormVector(7);
			int i0 = ssItem.Idx % 7 <= 4 ? 0 : cnt;
			int r = rand() % 100;
			for (int i = 0, val = 0; i < cnt; i++)
			{
				val += arr[i0 + i];
				if (r < val)
				{
					adv = SnowstormVector(i);
					break;
				}
			}
			if (ssItem.Idx % 3 == 0)
			{
				adv.X *= 2;
				adv.Y *= 2;
			}
			Point xy = ssItem.Points[ssItem.Points.Count - 1];
			xy.X += adv.X;
			xy.Y += adv.Y;

			xy.X %= BufferWi;
			xy.Y %= BufferHt;
			if (xy.X < 0) xy.X += BufferWi;
			if (xy.Y < 0) xy.Y += BufferHt;
			ssItem.Points.Add(xy);
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			int colorcnt = Colors.Count;
			int count = Convert.ToInt32(BufferWi * BufferHt * Count / 2000) + 1;
			int tailLength = BufferWi * BufferHt * Length / 2000 + 2;
			Point xy = new Point();
			int r;
			if (frame == 0 || count != _lastSnowstormCount)
			{
				_lastSnowstormCount = count;
				_snowstormItems.Clear();
			}
			// create snowstorm elements
			for (int i = 0; i < count; i++)
			{
				if (_snowstormItems.Count < count)
				{
					var ssItem = new SnowstormClass();
					ssItem.Idx = i;
					ssItem.SsDecay = 0;
					ssItem.Points.Clear();
					switch (ColorType)
					{
						case SnowStormColorType.Range: //Random two colors are selected from the list for each Snowstorms.
							ssItem.Hsv =
								SetRangeColor(HSV.FromRGB(Colors[rand()%colorcnt].GetColorAt((GetEffectTimeIntervalPosition(frame)*100)/100)),
									HSV.FromRGB(Colors[rand()%colorcnt].GetColorAt((GetEffectTimeIntervalPosition(frame)*100)/100)));
							break;
						case SnowStormColorType.Palette: //All colors are used
							ssItem.Hsv = HSV.FromRGB(Colors[rand()%colorcnt].GetColorAt((GetEffectTimeIntervalPosition(frame)*100)/100));
							break;
						case SnowStormColorType.Gradient:
							ssItem.Color = rand()%colorcnt;
							_gradientPosition = 100/(double) tailLength/100;
							ssItem.Hsv = HSV.FromRGB(Colors[ssItem.Color].GetColorAt(0));
							break;
					}
					// start in a random state
					r = rand()%(2*tailLength);
					if (r > 0)
					{
						xy.X = rand()%BufferWi;
						xy.Y = rand()%BufferHt;
						//ssItem.points.push_back(xy);
						ssItem.Points.Add(xy);
					}
					if (r >= tailLength)
					{
						ssItem.SsDecay = r - tailLength;
						r = tailLength;
					}
					for (int j = 1; j < r; j++)
					{
						SnowstormAdvance(ssItem);
					}
					_snowstormItems.Add(ssItem);
				}
			}

			// render Snowstorm Items
			foreach (SnowstormClass it in _snowstormItems)
			{
				if (it.Points.Count == 0)
				{
					xy.X = rand() % BufferWi;
					xy.Y = rand() % BufferHt;
					it.Points.Add(xy);
				}
				else if (rand() % 20 < Speed)
				{
					SnowstormAdvance(it);
				}
				var sz = it.Points.Count();
				for (int pt = 0; pt < sz; pt++)
				{
					switch (ColorType)
					{
						case SnowStormColorType.RainBow: //No user colors are used for Rainbow effect.
							it.Hsv.H = (float)(rand() % 1000) / 1000.0f;
							it.Hsv.S = 1.0f;
							it.Hsv.V = 1.0f;
							break;
						case SnowStormColorType.Gradient:
							it.Hsv = HSV.FromRGB(Colors[it.Color].GetColorAt(_gradientPosition * pt));
							break;
					}
					var hsv = it.Hsv;
					hsv.V = (float)(1.0 - (double)(sz - pt + it.SsDecay) / tailLength);
					hsv.V = hsv.V * LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
					if (it.Points[pt].X >= BufferWi - 1 || it.Points[pt].Y >= BufferHt - 1 || it.Points[pt].X <= 1 || it.Points[pt].Y <= 1)
					{
						it.Expired = true; //flags Snowstorms that have reached the end of the grid as expiried.
						break;
					}
					if (hsv.V < 0.0) hsv.V = 0.0f;
					if (!ReverseDirection)
					{
						frameBuffer.SetPixel(it.Points[pt].X, it.Points[pt].Y, hsv);
					}
					else
					{
						frameBuffer.SetPixel(BufferWi - it.Points[pt].X, it.Points[pt].Y, hsv);
					}
				}
			}
			// delete old Snowstorms
			int snowStorms = 0;
			while (snowStorms < _snowstormItems.Count)
			{
				if (_snowstormItems[snowStorms].Expired)
				{
					_snowstormItems.RemoveAt(snowStorms);
				}
				else
				{
					snowStorms++;
				}
			}
		}

		private int rand()
		{
			return _random.Next();
		}

		// generates a random number between Color num1 and and Color num2.
		private static float RandomRange(double num1, double num2)
		{
			double hi, lo;
			InitRandom();

			if (num1 < num2)
			{
				lo = num1;
				hi = num2;
			}
			else
			{
				lo = num2;
				hi = num1;
			}
			return (float)(_random.NextDouble() * (hi - lo) + lo);
		}

		private static void InitRandom()
		{
			if (_random == null)
				_random = new Random();
		}

		public static HSV SetRangeColor(HSV hsv, HSV hsv1)
		{
			HSV newHsv = new HSV(RandomRange(hsv.H, hsv1.H),
								 RandomRange(hsv.S, hsv1.S),
								 1.0f);
			return newHsv;
		}
	}
}
