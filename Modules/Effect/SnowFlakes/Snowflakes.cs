using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Snowflakes
{
	public class Snowflakes:PixelEffectBase
	{
		private SnowflakesData _data;

		private readonly List<SnowFlakeClass> _snowFlakes = new List<SnowFlakeClass>();
		private static Random _random = new Random();
		
		public Snowflakes()
		{
			_data = new SnowflakesData();
		}

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

		#region Config properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"FlakeType")]
		[ProviderDescription(@"FlakeType")]
		[PropertyOrder(0)]
		public SnowflakeType SnowflakeType
		{
			get { return _data.SnowflakeType; }
			set
			{
				_data.SnowflakeType = value;
				IsDirty = true;
				UpdateFlakeAttribute();
				OnPropertyChanged();
			}
		}

		//This is done so the user can exclude 45 Point Flakes from the Random selection as they would be too big on a small Matrix.
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Include45Pt")]
		[ProviderDescription(@"Include45Pt")]
		[PropertyOrder(1)]
		public bool PointFlake45
		{
			get { return _data.PointFlake45; }
			set
			{
				_data.PointFlake45 = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MovementType")]
		[ProviderDescription(@"MovementType")]
		[PropertyOrder(2)]
		public SnowflakeEffect SnowflakeEffect
		{
			get { return _data.SnowflakeEffect; }
			set
			{
				_data.SnowflakeEffect = value;
				IsDirty = true;
				UpdateDirectionAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MinAngle")]
		[ProviderDescription(@"MinAngle")]
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
		[ProviderDisplayName(@"MaxAngle")]
		[ProviderDescription(@"MaxAngle")]
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
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
//		[NumberRange(1, 60, 1)]
		[PropertyOrder(5)]
		public Curve CenterSpeedCurve
		{
			get { return _data.CenterSpeedCurve; }
			set
			{
				_data.CenterSpeedCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"SpeedVariation")]
		[ProviderDescription(@"SpeedVariation")]
//		[NumberRange(2, 60, 1)]
		[PropertyOrder(6)]
		public Curve SpeedVariationCurve
		{
			get { return _data.SpeedVariationCurve; }
			set
			{
				_data.SpeedVariationCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"FlakeCount")]
		[ProviderDescription(@"FlakeCount")]
	//	[NumberRange(1, 100, 1)]
		[PropertyOrder(7)]
		public Curve FlakeCountCurve
		{
			get { return _data.FlakeCountCurve; }
			set
			{
				_data.FlakeCountCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"ColorType")]
		[ProviderDescription(@"ColorType")]
		[PropertyOrder(0)]
		public SnowflakeColorType ColorType
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
		[ProviderDisplayName(@"CenterColor")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(1)]
		public List<ColorGradient> InnerColor
		{
			get { return _data.InnerColor; }
			set
			{
				_data.InnerColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"OuterColor")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(2)]
		public List<ColorGradient> OutSideColor
		{
			get { return _data.OutSideColor; }
			set
			{
				_data.OutSideColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Level properties

		[Value]
		[ProviderCategory(@"Brightness", 3)]
		[ProviderDisplayName(@"RandomIntensity")]
		[ProviderDescription(@"RandomIntensity")]
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
			UpdateFlakeAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		//Used to hide Colors from user when Rainbow type is selected and unhides for the other types.
		private void UpdateColorAttribute(bool refresh = true)
		{
			bool snowFlakeType = ColorType != SnowflakeColorType.RainBow;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2);
			propertyStates.Add("InnerColor", snowFlakeType);
			propertyStates.Add("OutSideColor", snowFlakeType);
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
			if (SnowflakeEffect == SnowflakeEffect.Explode)
			{
				direction = true;
			}
			if (SnowflakeEffect == SnowflakeEffect.RandomDirection)
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

		private void UpdateFlakeAttribute(bool refresh = true)
		{
			bool snowFlakeType = SnowflakeType == SnowflakeType.Random;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			propertyStates.Add("PointFlake45", snowFlakeType);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
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
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/snowflakes/"; }
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as SnowflakesData;
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
			//Not needed
		}

		protected override void CleanUpRender()
		{
			_snowFlakes.Clear();
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			int colorcntOutSide = OutSideColor.Count;
			int colorcntInside = InnerColor.Count;
			int minDirection = 1;
			int maxDirection = 360;
			var intervalPos = GetEffectTimeIntervalPosition(frame);
			var intervalPosFactor = intervalPos * 100;

			// create new SnowFlakes and maintain maximum number as per users selection.
			int flakeCount = SnowflakeEffect == SnowflakeEffect.Explode && frame < CalculateCount(intervalPosFactor) ? 1 : CalculateCount(intervalPosFactor);

			var centerSpeed = CalculateCenterSpeed(intervalPosFactor);
			var spreadSpeed = CalculateSpeedVariation(intervalPosFactor);
			var minSpeed = centerSpeed - (spreadSpeed / 2);
			var maxSpeed = centerSpeed + (spreadSpeed / 2);
			if (minSpeed < 1)
				minSpeed = 1;
			if (maxSpeed > 60)
				maxSpeed = 60;
			
			for (int i = 0; i < flakeCount; i++)
			{
				double position = (double)_random.Next(minSpeed, maxSpeed + 1) / 5;
				if (_snowFlakes.Count >= CalculateCount(intervalPosFactor)) continue;
				SnowFlakeClass m = new SnowFlakeClass();
				if (SnowflakeEffect == SnowflakeEffect.RandomDirection)
				{
					minDirection = MinDirection;
					maxDirection = MaxDirection;
				}

				int direction;
				if (SnowflakeEffect == SnowflakeEffect.None)
					direction = _random.Next(145, 215); //Set Range for standard Snowflakes as we don't want to just have them going straight down or two dirctions like the original Snowflake effect.
				else
				{
					//This is to generate random directions between the Min and Max values
					//However if Someone makes the MaxDirection lower then the Min Direction then
					//the new direction will be the inverserve of the Min and Max effectively changing
					//the range from a downward motion to an upward motion, increasing the feature capability.
					direction = maxDirection <= minDirection
						? (_random.Next(1, 3) == 1 ? _random.Next(1, maxDirection) : _random.Next(minDirection, 360))
						: _random.Next(minDirection, maxDirection);
				}

				//Moving (direction)
				if (direction > 0 && direction <= 90)
				{
					m.DeltaX = ((double)direction / 90) * position;
					m.DeltaY = ((double)Math.Abs(direction - 90) / 90) * position;
				}
				else if (direction > 90 && direction <= 180)
				{
					m.DeltaX = ((double)Math.Abs(direction - 180) / 90) * position;
					m.DeltaY = (-1 * ((double)Math.Abs(direction - 90) / 90)) * position;
				}
				else if (direction > 180 && direction <= 270)
				{
					m.DeltaX = (-1 * ((double)Math.Abs(direction - 180) / 90)) * position;
					m.DeltaY = (-1 * ((double)Math.Abs(direction - 270) / 90)) * position;
				}
				else if (direction > 270 && direction <= 360)
				{
					m.DeltaX = (-1 * ((double)Math.Abs(direction - 360) / 90)) * position;
					m.DeltaY = ((double)Math.Abs(270 - direction) / 90) * position;
				}

				//Start position for Snowflake
				if (SnowflakeEffect == SnowflakeEffect.Explode) //Will start in the centre of the grid
				{
					m.X = BufferWi / 2;
					m.Y = BufferHt / 2;
				}
				else
				{
					m.X = _random.Next() % BufferWi;
					if (frame == 0)
						m.Y = _random.Next() % BufferHt; //This is used so for the first lot of Snowflakes they will start in a random position and then fall from the edge.
					else
					{
						m.Y = BufferHt;
					}
				}
				m.DeltaXOrig = m.DeltaX;
				m.DeltaYOrig = m.DeltaY;

				m.Type = SnowflakeType == SnowflakeType.Random ? RandomFlakeType<SnowflakeType>() : SnowflakeType;

				//Set the SnowFlake colors during the creation of the snowflake.
				switch (ColorType)
				{
					case SnowflakeColorType.Range: //Random two colors are selected from the list for each SnowFlake and then the color range between them are used.
						m.OuterHsv = SetRangeColor(HSV.FromRGB(OutSideColor[rand() % colorcntOutSide].GetColorAt((intervalPosFactor) / 100)),
								HSV.FromRGB(OutSideColor[rand() % colorcntOutSide].GetColorAt((intervalPosFactor) / 100)));
						m.InnerHsv = SetRangeColor(HSV.FromRGB(InnerColor[rand() % colorcntInside].GetColorAt((intervalPosFactor) / 100)),
								HSV.FromRGB(InnerColor[rand() % colorcntInside].GetColorAt((intervalPosFactor) / 100)));
						break;
					case SnowflakeColorType.Palette: //All user colors are used
						m.OuterHsv = HSV.FromRGB(OutSideColor[rand() % colorcntOutSide].GetColorAt((intervalPosFactor) / 100));
						m.InnerHsv = HSV.FromRGB(InnerColor[rand() % colorcntInside].GetColorAt((intervalPosFactor) / 100));
						break;
					default:
						m.InnerHsv = HSV.FromRGB(InnerColor[rand() % colorcntInside].GetColorAt((intervalPosFactor) / 100));
					break;
				}
				m.HsvBrightness = RandomBrightness ? _random.NextDouble() * (1.0 - .25) + .25 : 1; //Adds a random brightness to each Snowflake making it look more realistic
				_snowFlakes.Add(m);
			}

			// render all SnowFlakes
			foreach (SnowFlakeClass snowFlakes in _snowFlakes)
			{
				snowFlakes.DeltaX += snowFlakes.DeltaXOrig;
				snowFlakes.DeltaY += snowFlakes.DeltaYOrig;
				for (int c = 0; c < 1; c++)
				{
					//Sets the new position the SnowFlake is moving to
					int colorX = (snowFlakes.X + Convert.ToInt32(snowFlakes.DeltaX) - (BufferWi / 100));
					int colorY = (snowFlakes.Y + Convert.ToInt32(snowFlakes.DeltaY) + (BufferHt / 100));

					if (SnowflakeEffect != SnowflakeEffect.Explode)//Modifies the colorX and colorY when the Explode effect is not used.
					{
						colorX = colorX%BufferWi;
						colorY = colorY%BufferHt;
					}

					if (ColorType == SnowflakeColorType.RainBow) //No user colors are used for Rainbow effect. Color selection for user will be hidden.
					{
						snowFlakes.OuterHsv.H = (float) (rand()%1000)/1000.0f;
						snowFlakes.OuterHsv.S = 1.0f;
						snowFlakes.OuterHsv.V = 1.0f;
					}
					//Added the color and then adjusts brightness based on effect time position, randon Brightness and over all brightness level.
					HSV hsvInner = snowFlakes.OuterHsv;
					HSV hsvOuter = snowFlakes.InnerHsv;
					hsvInner.V *= snowFlakes.HsvBrightness * LevelCurve.GetValue(intervalPosFactor) / 100;
					hsvOuter.V *= snowFlakes.HsvBrightness * LevelCurve.GetValue(intervalPosFactor) / 100;

					if (colorX >= BufferWi || colorY >= BufferHt || colorX <= 0 || colorY <= 0)
					{
						snowFlakes.Expired = true; //flags SnowFlakes that have reached the end of the grid as expiried. Allows new Snowflakes to be created.
						break;
					}

					//Render SnowFlakes
					switch (snowFlakes.Type)
					{
						case SnowflakeType.Single:
							// single node
							frameBuffer.SetPixel(colorX, colorY, hsvInner);
							break;
						case SnowflakeType.Five:
							// 5 nodes
							frameBuffer.SetPixel(colorX, colorY, hsvOuter); //Inner point of the Flake
							frameBuffer.SetPixel(colorX - 1, colorY, hsvInner);
							frameBuffer.SetPixel(colorX + 1, colorY, hsvInner);
							frameBuffer.SetPixel(colorX, colorY - 1, hsvInner);
							frameBuffer.SetPixel(colorX, colorY + 1, hsvInner);
							break;
						case SnowflakeType.Three:
							// 3 nodes
							frameBuffer.SetPixel(colorX, colorY, hsvOuter); //Inner point of the Flake
							if (rand()%100 > 50)
							{
								frameBuffer.SetPixel(colorX - 1, colorY, hsvInner);
								frameBuffer.SetPixel(colorX + 1, colorY, hsvInner);
							}
							else
							{
								frameBuffer.SetPixel(colorX, colorY - 1, hsvInner);
								frameBuffer.SetPixel(colorX, colorY + 1, hsvInner);
							}
							break;
						case SnowflakeType.Nine:
							// 9 nodes
							frameBuffer.SetPixel(colorX, colorY, hsvOuter); //Inner point of the Flake
							int i;
							for (i = 1; i <= 2; i++)
							{
								frameBuffer.SetPixel(colorX - i, colorY, hsvInner);
								frameBuffer.SetPixel(colorX + i, colorY, hsvInner);
								frameBuffer.SetPixel(colorX, colorY - i, hsvInner);
								frameBuffer.SetPixel(colorX, colorY + i, hsvInner);
							}
							break;
						case SnowflakeType.Thirteen:
							// 13 nodes
							frameBuffer.SetPixel(colorX, colorY, hsvOuter); //Inner point of the Flake
							frameBuffer.SetPixel(colorX - 1, colorY, hsvInner);
							frameBuffer.SetPixel(colorX + 1, colorY, hsvInner);
							frameBuffer.SetPixel(colorX, colorY - 1, hsvInner);
							frameBuffer.SetPixel(colorX, colorY + 1, hsvInner);

							frameBuffer.SetPixel(colorX - 1, colorY + 2, hsvInner);
							frameBuffer.SetPixel(colorX + 1, colorY + 2, hsvInner);
							frameBuffer.SetPixel(colorX - 1, colorY - 2, hsvInner);
							frameBuffer.SetPixel(colorX + 1, colorY - 2, hsvInner);
							frameBuffer.SetPixel(colorX + 2, colorY - 1, hsvInner);
							frameBuffer.SetPixel(colorX + 2, colorY + 1, hsvInner);
							frameBuffer.SetPixel(colorX - 2, colorY - 1, hsvInner);
							frameBuffer.SetPixel(colorX - 2, colorY + 1, hsvInner);
							break;
						case SnowflakeType.FortyFive:
							// 45 nodes
							int ii = 4;
							for (int j = -4; j < 5; j++)
							{
								if (colorX <= BufferWi && colorY <= BufferHt)
									frameBuffer.SetPixel(colorX + j, colorY + ii, hsvInner);
								ii--;
							}
							for (int j = -4; j < 5; j++)
							{
								if (colorX <= BufferWi && colorY <= BufferHt)
									frameBuffer.SetPixel(colorX + j, colorY + j, hsvInner);
							}
							if (colorX <= BufferWi && colorY <= BufferHt)
							{
								frameBuffer.SetPixel(colorX - 2, colorY + 3, hsvInner);
								frameBuffer.SetPixel(colorX - 3, colorY + 2, hsvInner);
								frameBuffer.SetPixel(colorX - 3, colorY - 2, hsvInner);
								frameBuffer.SetPixel(colorX - 2, colorY - 3, hsvInner);
								frameBuffer.SetPixel(colorX + 2, colorY + 3, hsvInner);
								frameBuffer.SetPixel(colorX + 2, colorY - 3, hsvInner);
								frameBuffer.SetPixel(colorX + 3, colorY + 2, hsvInner);
								frameBuffer.SetPixel(colorX + 3, colorY - 2, hsvInner);
							}
							for (int j = -5; j < 6; j++)
							{
								if (colorX <= BufferWi && colorY <= BufferHt)
									frameBuffer.SetPixel(colorX, colorY + j, hsvInner);
							}
							for (int j = -5; j < 6; j++)
							{
								if (colorX <= BufferWi && colorY <= BufferHt)
									frameBuffer.SetPixel(colorX + j, colorY, hsvInner);
							}
							if (colorX <= BufferWi && colorY <= BufferHt)
								frameBuffer.SetPixel(colorX, colorY, hsvOuter); //Inner point of the Flake
							break;
					}
				}
			}

			// deletes SnowFlakes that have expired when reaching the edge of the grid, allowing new Snowflakes to be created.
			int snowFlakeNum = 0;
			while (snowFlakeNum < _snowFlakes.Count)
			{
				if (_snowFlakes[snowFlakeNum].Expired)
				{
					_snowFlakes.RemoveAt(snowFlakeNum);
				}
				else
				{
					snowFlakeNum++;
				}
			}
		}

		// SnowFlakes
		public class SnowFlakeClass
		{
			public int X;
			public int Y;
			public double DeltaX;
			public double DeltaY;
			public double DeltaXOrig;
			public double DeltaYOrig;
			public HSV OuterHsv = new HSV();
			public HSV InnerHsv = new HSV();
			public bool Expired = false;
			public SnowflakeType Type;
			public double HsvBrightness;
		}

		private int CalculateCount(double intervalPos)
		{
			var value = (int)ScaleCurveToValue(FlakeCountCurve.GetValue(intervalPos), 100, 1);
			if (value < 1) value = 1;

			return value;
		}

		private int CalculateSpeedVariation(double intervalPos)
		{
			var value = (int)ScaleCurveToValue(SpeedVariationCurve.GetValue(intervalPos), 60, 1);
			if (value < 1) value = 1;

			return value;
		}

		private int CalculateCenterSpeed(double intervalPos)
		{
			var value = (int)ScaleCurveToValue(CenterSpeedCurve.GetValue(intervalPos), 60, 1);
			if (value < 1) value = 1;

			return value;
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

		private T RandomFlakeType<T>()
		{
			T randomEnum;
			bool exclude45PtFlakes; //This is done so the user can exclude 45 Point Flakes from the Random selection as they would be too big on a small Matrix.
			do
			{
				exclude45PtFlakes = false;
				Array values = Enum.GetValues(typeof(T));
				randomEnum = (T)values.GetValue(rand(values.Length));
				if (!PointFlake45 && randomEnum.ToString() == "FortyFive")
					exclude45PtFlakes = true;
			} while (exclude45PtFlakes);
			
			return randomEnum;
		}

		private int rand(int seed)
		{
			return _random.Next(seed);
		}
	}
}
