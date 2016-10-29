using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using Vixen.Sys.State.Execution;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
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
		[ProviderDisplayName(@"Min Direction")]
		[ProviderDescription(@"Min Direction in Degrees")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 360, 1)]
		[PropertyOrder(3)]
		public int MinDirection
		{
			get { return _data.MinDirection; }
			set
			{
				_data.MinDirection = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Max Direction")]
		[ProviderDescription(@"Max Direction in Degrees")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 360, 1)]
		[PropertyOrder(4)]
		public int MaxDirection
		{
			get { return _data.MaxDirection; }
			set
			{
				_data.MaxDirection = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Random Speed")]
		[ProviderDescription(@"Random Meteor Speed Between Min and Max value")]
		[PropertyOrder(5)]
		public bool RandomSpeed
		{
			get { return _data.RandomSpeed; }
			set
			{
				_data.RandomSpeed = value;
				IsDirty = true;
				UpdateRandomSpeedAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 200, 1)]
		[PropertyOrder(6)]
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
		[ProviderDisplayName(@"Min Speed")]
		[ProviderDescription(@"Min Speed")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 200, 1)]
		[PropertyOrder(7)]
		public int MinSpeed
		{
			get { return _data.MinSpeed; }
			set
			{
				if (MaxSpeed <= value)
					value = MaxSpeed - 1; //Ensures MinSpeed is below MaxSpeed
				_data.MinSpeed = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Max Speed")]
		[ProviderDescription(@"Max Speed")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 200, 1)]
		[PropertyOrder(8)]
		public int MaxSpeed
		{
			get { return _data.MaxSpeed; }
			set
			{
				if (MinSpeed > value)
					value = MinSpeed + 1;  //Ensures MaxSpeed is above MinSpeed
				_data.MaxSpeed = value;
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
		[PropertyOrder(9)]
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
		[NumberRange(1, 100, 1)]
		[PropertyOrder(10)]
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
		[ProviderDisplayName(@"Random Meteor Position")]
		[ProviderDescription(@"Creates new Meteors at a Random Position")]
		[PropertyOrder(11)]
		public bool RandomMeteorPosition
		{
			get { return _data.RandomMeteorPosition; }
			set
			{
				_data.RandomMeteorPosition = value;
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
		[ProviderDisplayName(@"Random Intensity")]
		[ProviderDescription(@"Chnages the Intensity for each Meteor")]
		[PropertyOrder(0)]
		public bool RandomBrightness
		{
			get { return _data.RandomBrightness; }
			set
			{
				_data.RandomBrightness = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Brightness", 3)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
		[PropertyOrder(1)]
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

		#region Update Attributes
		private void UpdateAttributes()
		{
			UpdateColorAttribute(false);
			UpdateDirectionAttribute(false);
			UpdateRandomSpeedAttribute(false);
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
		private void UpdateRandomSpeedAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3);
			propertyStates.Add("Speed", !RandomSpeed);
			propertyStates.Add("MaxSpeed", RandomSpeed);
			propertyStates.Add("MinSpeed", RandomSpeed);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateDirectionAttribute(bool refresh = true)
		{
			bool direction = false;
			bool variableDirection = false;
			if (MeteorEffect == MeteorsEffect.Explode)
			{
				direction = true;
			}
			if (MeteorEffect == MeteorsEffect.RandomDirection)
			{
				direction = true;
				variableDirection = true;
			}
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3);
			propertyStates.Add("Direction", !direction);
			propertyStates.Add("MinDirection", variableDirection);
			propertyStates.Add("MaxDirection", variableDirection);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		#endregion

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

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		protected override void SetupRender()
		{
			//Nothing to setup
		}

		protected override void CleanUpRender()
		{
			//Nothing to clean up
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			if (frame == 0)
				_meteors.Clear();
			int colorcnt = Colors.Count();
			int tailLength = (BufferHt < 10) ? Length/10 : BufferHt*Length/100;
			int minDirection = 1;
			int maxDirection = 360;
			if (tailLength < 1) tailLength = 1;
			int tailStart = BufferHt;
			if (tailStart < 1) tailStart = 1;

			// create new meteors and maintain maximum number as per users selection.
			HSV hsv = new HSV();
			int pixelCount = frame < PixelCount
				? (!RandomMeteorPosition && frame > PixelCount ? 1 : (PixelCount < 10 ? PixelCount : PixelCount / 10))
				: PixelCount;

			for (int i = 0; i < pixelCount; i++)
			{
				double position = RandomSpeed ? (double)_random.Next(MinSpeed, MaxSpeed + 1) / 20 : (double)Speed / 20;
				if (_meteors.Count >= PixelCount) continue;
				MeteorClass m = new MeteorClass();
				if (MeteorEffect == MeteorsEffect.RandomDirection)
				{
					minDirection = MinDirection;
					maxDirection = MaxDirection;
				}

				int direction;
				if (MeteorEffect == MeteorsEffect.None)
					direction = Direction; //Set Range for standard Meteor as we don't want to just have them going straight down or two dirctions like the original Meteor effect.
				else
				{
					//This is to generate random directions between the Min and Max values
					//However if Someone makes the MaxDirection lower then the Min Direction then
					//the new direction will be the inverserve of the Min and Max effectively changing
					//the range from a downward motion to an upward motion, increasing the feature capability.
					if (maxDirection <= minDirection)
					{
						//used for the Upward movement of the Meteor (added feature)
						direction = _random.Next(1, 3) == 1 ? _random.Next(1, maxDirection) : _random.Next(minDirection, 360);
					}
					else
					{
						//used for the downward movemnet of the Meteor (standard way)
						direction = _random.Next(minDirection, maxDirection);
					}
				}
				//Moving
				m.X = rand() % BufferWi;
				m.Y = rand() % BufferHt;
				if (direction >= 0 && direction <= 90)
				{
					m.TailX = ((double)direction / 90);
					m.DeltaX = m.TailX * position;
					m.TailY = ((double)Math.Abs(direction - 90) / 90);
					m.DeltaY = m.TailY * position;
					if (_random.NextDouble() >= (double)(90 - direction) / 100)
					{
						m.X = 0;
						m.Y = rand() % BufferHt;
					}
					else
					{
						m.X = rand()%BufferWi;
						m.Y = 0;
					}
				}
				else if (direction > 90 && direction <= 180)
				{
					m.TailX = ((double)Math.Abs(direction - 180) / 90);
					m.DeltaX = m.TailX * position;
					m.TailY = -1 * ((double)Math.Abs(direction - 90) / 90);
					m.DeltaY = m.TailY * position;
					if (_random.NextDouble() >= (double)(180 - direction) / 100)
					{
						m.X = rand() % BufferWi;
						m.Y = BufferHt;
					}
					else
					{
						m.X = 0;
						m.Y = rand() % BufferHt;
					}
				}
				else if (direction > 180 && direction <= 270)
				{
					m.TailX = -1 * ((double)Math.Abs(direction - 180) / 90);
					m.DeltaX = m.TailX * position;
					m.TailY = -1 * ((double)Math.Abs(direction - 270) / 90);
					m.DeltaY = m.TailY * position;
					if (_random.NextDouble() >= (double)(270 - direction) / 100)
					{
						m.X = BufferWi;
						m.Y = rand() % BufferHt;
					}
					else
					{
						m.X = rand() % BufferWi;
						m.Y = BufferHt;
					}
				}
				else if (direction > 270 && direction <= 360)
				{
					m.TailX = -1 * ((double)Math.Abs(direction - 360) / 90);
					m.DeltaX = m.TailX * position;
					m.TailY = ((double)Math.Abs(270 - direction) / 90);
					m.DeltaY = m.TailY * position;
					if (_random.NextDouble() >= (double)(360-direction)/100)
					{
						m.X = rand() % BufferWi;
						m.Y = 0;
					}
					else
					{
						m.X = BufferWi;
						m.Y = rand() % BufferHt;
					}
				}

				if (MeteorEffect == MeteorsEffect.Explode)
				{
					m.X = BufferWi/2;
					m.Y = BufferHt/2;
				}
				else
				{
					if (RandomMeteorPosition || frame < PixelCount)
					{
						m.X = rand() % BufferWi;
						m.Y = (BufferHt - 1 - (rand() % tailStart));
					}
				}
				m.DeltaXOrig = m.DeltaX;
				m.DeltaYOrig = m.DeltaY;

				switch (ColorType)
				{
					case MeteorsColorType.Range: //Random two colors are selected from the list for each meteor.
						m.Hsv =
							SetRangeColor(HSV.FromRGB(Colors[rand()%colorcnt].GetColorAt((GetEffectTimeIntervalPosition(frame)*100)/100)),
								HSV.FromRGB(Colors[rand()%colorcnt].GetColorAt((GetEffectTimeIntervalPosition(frame)*100)/100)));
						break;
					case MeteorsColorType.Palette: //All colors are used
						m.Hsv = HSV.FromRGB(Colors[rand()%colorcnt].GetColorAt((GetEffectTimeIntervalPosition(frame)*100)/100));
						break;
					case MeteorsColorType.Gradient:
						m.Color = rand() % colorcnt;
						_gradientPosition = 100 / (double)tailLength / 100;
						m.Hsv = HSV.FromRGB(Colors[m.Color].GetColorAt(0));
						break;
				}
				m.HsvBrightness = RandomBrightness ? _random.NextDouble() * (1.0 - .25) + .25 : 1;
				_meteors.Add(m);
			}

			// render meteors
			foreach (MeteorClass meteor in _meteors)
			{
				meteor.DeltaX += meteor.DeltaXOrig;
				meteor.DeltaY += meteor.DeltaYOrig;
				int colorX = (meteor.X + Convert.ToInt32(meteor.DeltaX) - (BufferWi / 100));
				int colorY = (meteor.Y + Convert.ToInt32(meteor.DeltaY) + (BufferHt / 100));

				for (int ph = 0; ph < tailLength; ph++)
				{
					switch (ColorType)
					{
						case MeteorsColorType.RainBow: //No user colors are used for Rainbow effect.
							meteor.Hsv.H = (float) (rand()%1000)/1000.0f;
							meteor.Hsv.S = 1.0f;
							meteor.Hsv.V = 1.0f;
							break;
						case MeteorsColorType.Gradient:
							meteor.Hsv = HSV.FromRGB(Colors[meteor.Color].GetColorAt(_gradientPosition*ph));
							break;
					}
					hsv = meteor.Hsv;
					hsv.V *= meteor.HsvBrightness;
					hsv.V *= (float) (1.0 - ((double) ph/tailLength)*0.75);
					//Adjusts the brightness based on the level curve
					hsv.V = hsv.V * LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
					var decPlaces = (int) (((decimal) (meteor.TailX*ph)%1)*100);
					if (decPlaces <= 40 || decPlaces >= 60)
						frameBuffer.SetPixel(colorX - (int) (Math.Round(meteor.TailX*ph)), colorY - (int) (Math.Round(meteor.TailY*ph)),
							hsv);
				}
				if (colorX >= BufferWi + tailLength || colorY >= BufferHt + tailLength || colorX < 0 - tailLength ||
				    colorY < 0 - tailLength)
				{
					meteor.Expired = true; //flags Meteors that have reached the end of the grid as expiried.
					//	break;
				}
			}

			// delete old meteors
			int meteorNum = 0;
			while (meteorNum < _meteors.Count)
			{
				if (_meteors[meteorNum].Expired)
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
			public int X;
			public int Y;
			public double DeltaX;
			public double DeltaY;
			public double DeltaXOrig;
			public double DeltaYOrig;
			public double TailX;
			public double TailY;
			public HSV Hsv = new HSV();
			public bool Expired = false;
			public int Color;
			public double HsvBrightness;
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
