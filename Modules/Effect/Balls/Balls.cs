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

namespace VixenModules.Effect.Balls
{
	public class Balls : PixelEffectBase
	{
		private BallData _data;
		private readonly List<BallClass> _balls = new List<BallClass>();
		private readonly List<BallClass> _removeBalls = new List<BallClass>();
		private int _ballCount;
		private readonly Random _random = new Random();
		private int _minBuffer;
		private int _maxBuffer;
		private double _intervalPos;
		private double _intervalPosFactor;
		private int _radius;
		private double _centerSpeed;
		private double _speedVariation;
		private double _level;

		public Balls()
		{
			_data = new BallData();
			EnableTargetPositioning(true, true);
			UpdateAttributes();
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
		[ProviderDisplayName(@"BallType")]
		[ProviderDescription(@"BallType")]
		[PropertyOrder(0)]
		public BallType BallType
		{
			get { return _data.BallType; }
			set
			{
				_data.BallType = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BallFill")]
		[ProviderDescription(@"BallFill")]
		[PropertyOrder(1)]
		public BallFill BallFill
		{
			get { return _data.BallFill; }
			set
			{
				_data.BallFill = value;
				UpdateFillAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyOrder(2)]
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
		[PropertyOrder(3)]
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
		[ProviderDisplayName(@"BallSize")]
		[ProviderDescription(@"BallSize")]
		[PropertyOrder(4)]
		public Curve SizeCurve
		{
			get { return _data.SizeCurve; }
			set
			{
				_data.SizeCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BallCount")]
		[ProviderDescription(@"BallCount")]
		[PropertyOrder(5)]
		public Curve BallCountCurve
		{
			get { return _data.BallCountCurve; }
			set
			{
				_data.BallCountCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BallEdgeWidth")]
		[ProviderDescription(@"BallEdgeWidth")]
		[PropertyOrder(6)]
		public Curve BallEdgeWidthCurve
		{
			get { return _data.BallEdgeWidthCurve; }
			set
			{
				_data.BallEdgeWidthCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"RandomRadius")]
		[ProviderDescription(@"RandomRadius")]
		[PropertyOrder(7)]
		public bool RandomRadius
		{
			get { return _data.RandomRadius; }
			set
			{
				_data.RandomRadius = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Collide")]
		[ProviderDescription(@"Collide")]
		[PropertyOrder(8)]
		public bool Collide
		{
			get { return _data.Collide; }
			set
			{
				_data.Collide = value;
				UpdateCollideAttributes();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ChangeCollideColor")]
		[ProviderDescription(@"ChangeCollideColor")]
		[PropertyOrder(9)]
		public bool ChangeCollideColor
		{
			get { return _data.ChangeCollideColor; }
			set
			{
				_data.ChangeCollideColor = value;
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


		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"BackGroundColor")]
		[ProviderDescription(@"BackGroundColor")]
		[PropertyOrder(3)]
		public ColorGradient BackgroundColor
		{
			get { return _data.BackgroundColor; }
			set
			{
				_data.BackgroundColor = value;
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

		#region Update Attributes

		private void UpdateAttributes()
		{
			UpdateFillAttribute(false);
			UpdateCollideAttributes(false);
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
		}

		private void UpdateFillAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3);
			{
				propertyStates.Add("Colors", BallFill != BallFill.Inverse);
				propertyStates.Add("BackgroundColor", BallFill == BallFill.Inverse);
				propertyStates.Add("BallEdgeWidthCurve", BallFill == BallFill.Empty);
			}
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateCollideAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			{
				propertyStates.Add("ChangeCollideColor", Collide);
			}
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
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/balls/"; }
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as BallData;
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
			_minBuffer = Math.Min(BufferHt, BufferWi);
			_maxBuffer = Math.Max(BufferHt, BufferWi);
		}

		protected override void CleanUpRender()
		{
			_balls.Clear();
			_removeBalls.Clear();
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			_intervalPos = GetEffectTimeIntervalPosition(frame);
			_intervalPosFactor = _intervalPos * 100;

			_radius = CalculateSize(_intervalPosFactor);
			_centerSpeed = CalculateCenterSpeed(_intervalPosFactor);
			_speedVariation = CalculateSpeedVariation(_intervalPosFactor);
			_level = LevelCurve.GetValue(_intervalPosFactor) / 100;
			Color inverseBackColor = BackgroundColor.GetColorAt(_intervalPos);

			double minSpeed = _centerSpeed - (_speedVariation / 2);
			double maxSpeed = _centerSpeed + (_speedVariation / 2);

			_ballCount = CalculateBallCount(_intervalPosFactor);

			// Create new Balls and add balls due to increase in ball count curve.
			CreateBalls(minSpeed, maxSpeed);

			// Update Ball location, radius and speed.
			UpdateBalls();

			//Remove Excess Balls due to BallCount Curve.
			RemoveBalls();

			//Iterate through all grid locations.
			for (int y = 0; y < BufferHt; y++)
			{
				for (int x = 0; x < BufferWi; x++)
				{
					CalculatePixel(x, y, frameBuffer, inverseBackColor);
				}
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.Y).ThenBy(x => x.X).GroupBy(x => x.Y);

			for (int frame = 0; frame < numFrames; frame++)
			{
				frameBuffer.CurrentFrame = frame;
				_intervalPos = GetEffectTimeIntervalPosition(frame);
				_intervalPosFactor = _intervalPos*100;

				_radius = CalculateSize(_intervalPosFactor);
				_centerSpeed = CalculateCenterSpeed(_intervalPosFactor);
				_speedVariation = CalculateSpeedVariation(_intervalPosFactor);
				_level = LevelCurve.GetValue(_intervalPosFactor)/100;
				Color inverseBackColor = BackgroundColor.GetColorAt(_intervalPos);

				double minSpeed = _centerSpeed - (_speedVariation/2);
				double maxSpeed = _centerSpeed + (_speedVariation/2);

				_ballCount = CalculateBallCount(_intervalPosFactor);

				// Create new Balls and add balls due to increase in ball count curve.
				CreateBalls(minSpeed, maxSpeed);

				// Update Ball location, radius and speed.
				UpdateBalls();

				//Remove Excess Balls due to BallCount Curve.
				RemoveBalls();

				//Iterate through all grid locations.
				foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
				{
					foreach (var elementLocation in elementLocations)
					{
						CalculatePixel(elementLocation.X, elementLocation.Y, frameBuffer, inverseBackColor);
					}
				}
			}
		}

		private void UpdateBalls()
		{
			foreach (var ball in _balls)
			{
				if (_ballCount < _balls.Count - _removeBalls.Count)
				{
					//Removes balls if ball count curve position is below the current number of balls. Will only remove ones that hit the edge of the grid.
					if (ball.LocationX + _radius >= BufferWi - 1 || ball.LocationY + _radius >= BufferHt - 1 ||
					    ball.LocationX - _radius <= 1 || ball.LocationY + _radius <= 1)
					{
						_removeBalls.Add(ball);
					}
				}

				//Adjust ball speeds when user adjust Speed curve
				if (_centerSpeed > CalculateCenterSpeed(_intervalPosFactor - 1) ||
				    _centerSpeed < CalculateCenterSpeed(_intervalPosFactor - 1))
				{
					double ratio = CalculateCenterSpeed(_intervalPosFactor)/CalculateCenterSpeed(_intervalPosFactor - 1);
					ball.VelocityX *= ratio;
					ball.VelocityY *= ratio;
				}

				if (_speedVariation > CalculateSpeedVariation(_intervalPosFactor - 1) ||
				    _speedVariation < CalculateSpeedVariation(_intervalPosFactor - 1))
				{
					double ratio = CalculateSpeedVariation(_intervalPosFactor)/CalculateSpeedVariation(_intervalPosFactor - 1);
					ball.VelocityX *= ratio;
					ball.VelocityY *= ratio;
				}

				int previousBallSize = CalculateSize(_intervalPosFactor - 1);
				if (_radius > previousBallSize || _radius < previousBallSize)
				{
					double ratio = (double)CalculateSize(_intervalPosFactor) / previousBallSize;
					ball.Radius *= ratio;
				}

				// Move the ball.
				ball.LocationX = ball.LocationX + ball.VelocityX;
				ball.LocationY = ball.LocationY + ball.VelocityY;

				//Checks to see if any are going to collide and if they are then adjust their location and change to opposite direction.
				if (Collide)
				{
					foreach (var ball1 in _balls)
					{
						var maxRadius = Math.Max(ball.Radius, ball1.Radius);
						if ((ball1.LocationX + maxRadius >= ball.LocationX && ball1.LocationX - maxRadius < ball.LocationX &&
							 ball1.LocationY + maxRadius >= ball.LocationY && ball1.LocationY - maxRadius < ball.LocationY) &&
						    ball1.BallGuid != ball.BallGuid)
						{
							ball.VelocityX = -ball.VelocityX;
							ball.VelocityY = -ball.VelocityY;
							ball1.VelocityX = -ball1.VelocityX;
							ball1.VelocityY = -ball1.VelocityY;
							ball.LocationY = ball.LocationY + ball.VelocityY;
							ball.LocationX = ball.LocationX + ball.VelocityX;
							ball1.LocationY = ball1.LocationY + ball1.VelocityY;
							ball1.LocationX = ball1.LocationX + ball1.VelocityX;

							//Changes the Ball color when it collides with another ball and will not change to the same color.
							if (Colors.Count > 1 && ChangeCollideColor)
							{
								var colorIndex = 0;
								do
								{
									colorIndex = _random.Next(0, Colors.Count);
								} while (ball.ColorIndex == colorIndex);
								ball.ColorIndex = colorIndex;

								do
								{
									colorIndex = _random.Next(0, Colors.Count);
								} while (ball1.ColorIndex == colorIndex);
								ball1.ColorIndex = colorIndex;
							}
							break;
						}
					}
				}

				switch (BallType)
				{
					case BallType.Bounce:
						if (ball.LocationX - ball.Radius < 0)
						{
							ball.LocationX = ball.Radius;
							ball.VelocityX = -ball.VelocityX;
						}
						else if (ball.LocationX + ball.Radius >= BufferWi)
						{
							ball.LocationX = BufferWi - ball.Radius - 1;
							ball.VelocityX = -ball.VelocityX;
						}
						if (ball.LocationY - ball.Radius < 0)
						{
							ball.LocationY = ball.Radius;
							ball.VelocityY = -ball.VelocityY;
						}
						else if (ball.LocationY + ball.Radius >= BufferHt)
						{
							ball.LocationY = BufferHt - ball.Radius - 1;
							ball.VelocityY = -ball.VelocityY;
						}
						break;

					case BallType.Wrap:
						if (ball.LocationX + ball.Radius < 0)
						{
							ball.LocationX += BufferWi + (ball.Radius* 2);
						}
						if (ball.LocationY + ball.Radius < 0)
						{
							ball.LocationY += BufferHt + (ball.Radius * 2);
						}
						if (ball.LocationX - ball.Radius > BufferWi)
						{
							ball.LocationX -= BufferWi + (ball.Radius * 2);
						}
						if (ball.LocationY - ball.Radius > BufferHt)
						{
							ball.LocationY -= BufferHt + (ball.Radius * 2);
						}
						break;
				}
			}
		}

		private void CalculatePixel(int x, int y, IPixelFrameBuffer frameBuffer, Color inverseBackColor)
		{
			int yCoord = y;
			int xCoord = x;
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				//Flip me over so and offset my coordinates I can act like the string version
				y = Math.Abs((BufferHtOffset - y) + (BufferHt - 1 + BufferHtOffset));
				y = y - BufferHtOffset;
				x = x - BufferWiOffset;
			}
			bool inverseColor = false;
			foreach (var ball in _balls)
			{
				//This saves going through all X and Y locations, significantly reducing render times.
				if ((y >= ball.LocationY + ball.Radius + 1 || y <= ball.LocationY - ball.Radius) &&
					(x >= ball.LocationX + ball.Radius + 1 || x <= ball.LocationX - ball.Radius)) continue;

				double distanceFromBallCenter = DistanceFromPoint(new Point((int)Math.Round(ball.LocationX), (int)Math.Round(ball.LocationY)), x, y);

				double distance = distanceFromBallCenter > 1.4 && distanceFromBallCenter < 1.51
					? 2
					: distanceFromBallCenter;
				HSV hsv = HSV.FromRGB(Colors[ball.ColorIndex].GetColorAt((_intervalPos)));
				hsv.V = hsv.V*_level;

				switch (BallFill)
				{
					case BallFill.Solid:
						if (distance <= ball.Radius)
							frameBuffer.SetPixel(xCoord, yCoord, hsv);
						break;
					case BallFill.Empty:
						if (distance >= ball.Radius - CalculateEdgeWidth(_intervalPosFactor) && distance < ball.Radius)
							frameBuffer.SetPixel(xCoord, yCoord, hsv);
						break;
					case BallFill.Inverse:
						if (distance <= ball.Radius)
						{
							inverseColor = true;
							hsv = HSV.FromRGB(Color.Black);
							frameBuffer.SetPixel(xCoord, yCoord, hsv);
						}
						break;
					case BallFill.Fade:
						if (distance <= ball.Radius*0.9)
						{
							hsv.V *= 1.0 - distance/(double) ball.Radius;
							frameBuffer.SetPixel(xCoord, yCoord, hsv);
						}
						break;
					case BallFill.Gradient:
						if (distance <= ball.Radius)
						{
							hsv = HSV.FromRGB(Colors[ball.ColorIndex].GetColorAt(distance/(double) ball.Radius));
							hsv.V = hsv.V*_level;
							frameBuffer.SetPixel(xCoord, yCoord, hsv);
						}
						break;
				}
			}

			//Add the background color when no other color has been used for the balls.
			if (!inverseColor && BallFill == BallFill.Inverse)
			{
				frameBuffer.SetPixel(xCoord, yCoord, inverseBackColor);
			}
		}

		private void CreateBalls(double minSpeed, double maxSpeed)
		{
			while (_ballCount > _balls.Count)
			{
				BallClass m = new BallClass();

				//Sets Radius size and Ball location
				int radius = RandomRadius ? _random.Next(1, _radius + 1) : _radius;
				m.LocationX = _random.Next(radius, BufferWi - radius);
				m.LocationY = _random.Next(radius, BufferHt - radius);

				if (Collide)
				{
					//Will not add a ball if one exists in the same spot.
					foreach (BallClass ball in _balls)
					{
						if ((m.LocationX >= ball.LocationX && m.LocationX - radius < ball.LocationX) ||
						    (m.LocationY >= ball.LocationY && m.LocationY - radius < ball.LocationY)) return;
					}
				}

				double speed = _random.NextDouble() * (maxSpeed - minSpeed) + minSpeed;
				double vx = _random.NextDouble() * speed;
				double vy = _random.NextDouble() * speed;
				if (_random.Next(0, 2) == 0) vx = -vx;
				if (_random.Next(0, 2) == 0) vy = -vy;
				m.VelocityX = vx;
				m.VelocityY = vy;
				m.Radius = radius;
				m.BallGuid = Guid.NewGuid();
				m.ColorIndex = _random.Next(0, Colors.Count);
				_balls.Add(m);
			}
		}

		private void RemoveBalls()
		{
			if (_ballCount < _balls.Count)
			{
				foreach (var balls in _removeBalls)
				{
					_balls.Remove(balls);
				}
				_removeBalls.Clear();
			}
		}

		public class BallClass
		{
			public double LocationX;
			public double LocationY;
			public double VelocityX;
			public double VelocityY;
			public int ColorIndex;
			public double Radius;
			public Guid BallGuid;
		}

		private double CalculateCenterSpeed(double intervalPosFactor)
		{
			return ScaleCurveToValue(CenterSpeedCurve.GetValue(intervalPosFactor), (double)_maxBuffer / 10, 0);
		}

		private double CalculateSpeedVariation(double intervalPosFactor)
		{
			return ScaleCurveToValue(SpeedVariationCurve.GetValue(intervalPosFactor), (double)_maxBuffer / 10, 0);
		}

		private int CalculateSize(double intervalPosFactor)
		{
			int value = (int)ScaleCurveToValue(SizeCurve.GetValue(intervalPosFactor), (int)(_minBuffer / 2), 1);
			if (value < 1) value = 1;
			return value;
		}

		private int CalculateEdgeWidth(double intervalPosFactor)
		{
			int value = (int)ScaleCurveToValue(BallEdgeWidthCurve.GetValue(intervalPosFactor), 40, 1);
			if (value < 1) value = 1;
			return value;
		}

		private int CalculateBallCount(double intervalPosFactor)
		{
			int value = (int)ScaleCurveToValue(BallCountCurve.GetValue(intervalPosFactor), 100, 1);
			if (value < 1) value = 1;
			return value;
		}
	}
}
