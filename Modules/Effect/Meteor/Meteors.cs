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

namespace VixenModules.Effect.Meteors
{
	public class Meteors : PixelEffectBase
	{
		private MeteorsData _data;
		private readonly List<MeteorClass> _meteors = new List<MeteorClass>();
		private static Random _random = new Random();
		private double _gradientPosition = 0;

		public Meteors()
		{
			_data = new MeteorsData();
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
		[ProviderDisplayName(@"Color Type")]
		[ProviderDescription(@"Color Type")]
		[PropertyOrder(0)]
		public MeteorsColorType ColorType
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Meteor Effects")]
		[ProviderDescription(@"Meteor Effects")]
		[PropertyOrder(1)]
		public MeteorsEffect MeteorEffect
		{
			get { return _data.MeteorEffect; }
			set
			{
				_data.MeteorEffect = value;
				IsDirty = true;
				UpdateDirectionAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 360, 1)]
		[PropertyOrder(2)]
		public int Direction
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
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 50, 1)]
		[PropertyOrder(3)]
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
		[NumberRange(1, 200, 1)]
		[PropertyOrder(4)]
		public int PixelCount
		{
			get { return _data.PixelCount; }
			set
			{
				_data.PixelCount = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Tail Length")]
		[ProviderDescription(@"Tail Length")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 50, 1)]
		[PropertyOrder(5)]
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

		private void UpdateAttributes()
		{
			UpdateColorAttribute(false);
			UpdateDirectionAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		//Used to hide Colors from user when Rainbow type is selected and unhides for the other types.
		private void UpdateColorAttribute(bool refresh = true)
		{
			bool meteorType = ColorType != MeteorsColorType.RainBow;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			propertyStates.Add("Colors", meteorType);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateDirectionAttribute(bool refresh = true)
		{
			bool RandomDirection = MeteorEffect != MeteorsEffect.None;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			propertyStates.Add("Direction", !RandomDirection);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as MeteorsData;
				UpdateAttributes();
				IsDirty = true;
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

		protected override void RenderEffect(int frame, ref PixelFrameBuffer frameBuffer)
		{
			if (frame == 0)
				_meteors.Clear();
			int colorcnt = Colors.Count();
			int tailLength = (BufferHt < 10) ? Length/10 : BufferHt*Length/100;
			if (tailLength < 1) tailLength = 1;
			int tailStart = BufferHt;
			if (tailStart < 1) tailStart = 1;

			double _position = (double)Speed / 5;

			// create new meteors and maintain maximum number as per users selection.
			HSV hsv = new HSV();
			for (int i = 0; i < PixelCount; i++)
			{
				if (_meteors.Count >= PixelCount) continue;
				MeteorClass m = new MeteorClass();
				int direction;
				direction = MeteorEffect == MeteorsEffect.None ? Direction : _random.Next(0, 360);

				//Moving left and right
				if (direction > 0 && direction <= 90)
				{
					m.tailX = ((double)direction / 90);
					m.deltaX = m.tailX * _position;
				}
				else if (direction > 90 && direction <= 180)
				{
					m.tailX = ((double)Math.Abs(direction - 180) / 90);
					m.deltaX = m.tailX * _position;
				}
				else if (direction > 180 && direction <= 270)
				{
					m.tailX = -1 * ((double)Math.Abs(direction - 180) / 90);
					m.deltaX = m.tailX * _position;
				}
				else if (direction > 270 && direction <= 360)
				{
					m.tailX = -1 * ((double)Math.Abs(direction - 360) / 90);
					m.deltaX = m.tailX * _position;
				}

				//Moving up and down
				if (direction >= 0 && direction <= 90)
				{
					m.tailY = ((double)Math.Abs(direction - 90) / 90);
					m.deltaY = m.tailY * _position;
				}
				else if (direction > 90 && direction <= 180)
				{
					m.tailY = -1 * ((double)Math.Abs(direction - 90) / 90);
					m.deltaY = m.tailY * _position;
				}
				else if (direction > 180 && direction <= 270)
				{
					m.tailY = -1 * ((double)Math.Abs(direction - 270) / 90);
					m.deltaY = m.tailY * _position;
				}
				else if (direction > 270 && direction <= 360)
				{
					m.tailY = ((double)Math.Abs(270 - direction) / 90);
					m.deltaY = m.tailY * _position;
				}

				if (MeteorEffect == MeteorsEffect.Explode)
				{
					m.x = BufferWi/2;
					m.y = BufferHt/2;
				}
				else
				{
					m.x = rand()%BufferWi;
					m.y = (BufferHt - 1 - (rand()%tailStart));
				}
				m.deltaXOrig = m.deltaX;
				m.deltaYOrig = m.deltaY;

				switch (ColorType)
				{
					case MeteorsColorType.Range: //Random two colors are selected from the list for each meteor.
						m.hsv =
							SetRangeColor(HSV.FromRGB(Colors[rand()%colorcnt].GetColorAt((GetEffectTimeIntervalPosition(frame)*100)/100)),
								HSV.FromRGB(Colors[rand()%colorcnt].GetColorAt((GetEffectTimeIntervalPosition(frame)*100)/100)));
						break;
					case MeteorsColorType.Palette: //All colors are used
						m.hsv = HSV.FromRGB(Colors[rand()%colorcnt].GetColorAt((GetEffectTimeIntervalPosition(frame)*100)/100));
						break;
					case MeteorsColorType.Gradient:
						m.color = rand() % colorcnt;
						_gradientPosition = 100 / (double)tailLength / 100;
						m.hsv = HSV.FromRGB(Colors[m.color].GetColorAt(0));
						break;
				}
				_meteors.Add(m);
			}

			// render meteors
			foreach (MeteorClass meteor in _meteors)
			{
				for (int ph = 0; ph < tailLength; ph++)
				{
					int colorX = (meteor.x + Convert.ToInt32(meteor.deltaX) - (BufferWi / 100));
					int colorY = (meteor.y + Convert.ToInt32(meteor.deltaY) + (BufferHt / 100));

					if (colorX >= 0)
					{
						colorX = colorX%BufferWi;
					}
					else if (colorX < 0)
					{
						colorX = Convert.ToInt32(colorX%BufferWi) + BufferWi - 1;
					}

					if (colorY >= 0)
					{
						colorY = Convert.ToInt32((colorY%BufferHt));
					}
					else if (colorY < 0)
					{
						colorY = Convert.ToInt32(colorY%BufferHt) + BufferHt - 1;
					}

					switch (ColorType)
					{
						case MeteorsColorType.RainBow: //No user colors are used for Rainbow effect.
							meteor.hsv.H = (float) (rand()%1000)/1000.0f;
							meteor.hsv.S = 1.0f;
							meteor.hsv.V = 1.0f;
							break;
						case MeteorsColorType.Gradient:
							meteor.hsv = HSV.FromRGB(Colors[meteor.color].GetColorAt(_gradientPosition * ph));
							break;
					}
					hsv = meteor.hsv;
					hsv.V = hsv.V*LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame)*100)/100;
					//Adjusts the brightness based on the level curve
					hsv.V *= (float) (1.0 - ((double) ph/tailLength)*0.75);
					if (colorX >= BufferWi - 1 || colorY >= BufferHt - 1 || colorX <= 1 || colorY <= 1)
					{
						meteor.expired = true; //flags Meteors that have reached the end of the grid as expiried.
						break;
					}
					var decPlaces = (int)(((decimal)(meteor.tailX * ph) % 1) * 100);
					if (decPlaces <= 40 || decPlaces >=60)
						frameBuffer.SetPixel(colorX - (int)(Math.Round(meteor.tailX * ph)), colorY - (int)(Math.Round(meteor.tailY * ph)), hsv);
				}
				meteor.deltaX += meteor.deltaXOrig;
				meteor.deltaY += meteor.deltaYOrig;
			}

			// delete old meteors
			int meteorNum = 0;
			while (meteorNum < _meteors.Count)
			{
				if (_meteors[meteorNum].expired)
				{
					_meteors.RemoveAt(meteorNum);
				}
				else
				{
					meteorNum++;
				}
			}
		}

		// for Meteor effects
		public class MeteorClass
		{
			public int x;
			public int y;
			public double deltaX;
			public double deltaY;
			public double deltaXOrig;
			public double deltaYOrig;
			public double tailX;
			public double tailY;
			public HSV hsv = new HSV();
			public bool expired = false;
			public int color;
		}

		// generates a random number between Color num1 and and Color num2.
		private static float RandomRange(float num1, float num2)
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

		private int rand()
		{
			return _random.Next();
		}

		private static void InitRandom()
		{
			if (_random == null)
				_random = new Random();
		}

		//Use for Range type
		public static HSV SetRangeColor(HSV hsv1, HSV hsv2)
		{
			HSV newHsv = new HSV(RandomRange((float)hsv1.H, (float)hsv2.H),
								 RandomRange((float)hsv1.S, (float)hsv2.S),
								 1.0f);
			return newHsv;
		}
	}
}
