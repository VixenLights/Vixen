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
		private List<BallClass> _balls;
		private List<BallClass> _removeBalls;
		private int _ballCount;
		private int _minBuffer;
		private int _maxBuffer;
		private float _intervalPos;
		private double _intervalPosFactor;
		private int _radius;
		private double _centerSpeed;
		private double _speedVariation;
		private double _level;
		private int _bufferHt;
		private int _bufferWi;

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
		[ProviderDisplayName(@"RandomMaxCurve")]
		[ProviderDescription(@"RandomMaxCurve")]
		[PropertyOrder(7)]
		public Curve RandomMaxCurve
		{
			get { return _data.RandomMaxCurve; }
			set
			{
				_data.RandomMaxCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"RandomMovement")]
		[ProviderDescription(@"RandomMovement")]
		[PropertyOrder(8)]
		public bool RandomMovement
		{
			get { return _data.RandomMovement; }
			set
			{
				_data.RandomMovement = value;
				UpdateRandomMoveAttributes();
				 IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"RandomRadius")]
		[ProviderDescription(@"RandomRadius")]
		[PropertyOrder(9)]
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
		[PropertyOrder(10)]
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
		[PropertyOrder(11)]
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
			UpdateRandomMoveAttributes(false);
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

		private void UpdateRandomMoveAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			{
				propertyStates.Add("RandomMaxCurve", RandomMovement);
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
			_bufferWi = BufferWi;
			_bufferHt = BufferHt;
			  _minBuffer = Math.Min(_bufferHt, _bufferWi);
			_maxBuffer = Math.Max(_bufferHt, _bufferWi);
			_balls = new List<BallClass>(7);
			_removeBalls = new List<BallClass>();
		}

		protected override void CleanUpRender()
		{
			_balls = null;
			_removeBalls = null;
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			_intervalPos = (float)GetEffectTimeIntervalPosition(frame);
			_intervalPosFactor = _intervalPos * 100;

			_radius = CalculateSize(_intervalPosFactor);
			_centerSpeed = CalculateCenterSpeed(_intervalPosFactor);;
			_speedVariation = CalculateSpeedVariation(_intervalPosFactor);
			_level = LevelCurve.GetValue(_intervalPosFactor) / 100;
			int maxRandomTime = CalculateRandomMax(_intervalPosFactor);
			Color inverseBackColor = BackgroundColor.GetColorAt(_intervalPos);

			double minSpeed = _centerSpeed - (_speedVariation / 2);
			double maxSpeed = _centerSpeed + (_speedVariation / 2);

			_ballCount = CalculateBallCount(_intervalPosFactor);
			
			// Create new Balls and add balls due to increase in ball count curve.
			CreateBalls(minSpeed, maxSpeed, maxRandomTime);

			// Update Ball location, radius and speed.
			UpdateBalls(minSpeed, maxSpeed, maxRandomTime);

			//Remove Excess Balls due to BallCount Curve.
			RemoveBalls();

			//Iterate through all grid locations.
			for (int y = 0; y < _bufferHt; y++)
			{
				for (int x = 0; x < _bufferWi; x++)
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
				_intervalPos = (float)GetEffectTimeIntervalPosition(frame);
				_intervalPosFactor = _intervalPos*100;

				_radius = CalculateSize(_intervalPosFactor);
				_centerSpeed = CalculateCenterSpeed(_intervalPosFactor);
				_speedVariation = CalculateSpeedVariation(_intervalPosFactor);
				_level = LevelCurve.GetValue(_intervalPosFactor)/100;
				int maxRandomTime = CalculateRandomMax(_intervalPosFactor);
				Color inverseBackColor = BackgroundColor.GetColorAt(_intervalPos);

				double minSpeed = _centerSpeed - (_speedVariation/2);
				double maxSpeed = _centerSpeed + (_speedVariation/2);

				_ballCount = CalculateBallCount(_intervalPosFactor);

				// Create new Balls and add balls due to increase in ball count curve.
				CreateBalls(minSpeed, maxSpeed, maxRandomTime);

				// Update Ball location, radius and speed.
				UpdateBalls(minSpeed, maxSpeed, maxRandomTime);

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

		private void UpdateBalls(double minSpeed, double maxSpeed, int maxRandomTime)
		{
			foreach (var ball in _balls)
			{
				if (_ballCount < _balls.Count - _removeBalls.Count)
				{
					//Removes balls if ball count curve position is below the current number of balls. Will only remove ones that hit the edge of the grid.
					if (ball.LocationX + _radius >= _bufferWi - 1 || ball.LocationY + _radius >= _bufferHt - 1 ||
					    ball.LocationX - _radius <= 1 || ball.LocationY + _radius <= 1)
					{
						_removeBalls.Add(ball);
					}
				}
				
				// Move the ball.
				if (RandomMovement && ball.MoveCount == 0)
				{
					double speed = RandDouble() * (maxSpeed - minSpeed) + minSpeed;
					double vx = RandDouble() * speed;
					double vy = RandDouble() * speed;
					if (Rand(0, 2) == 0) vx = -vx;
					if (Rand(0, 2) == 0) vy = -vy;
					ball.VelocityX = vx;
					ball.VelocityY = vy;
					ball.MoveCount = Rand(5, maxRandomTime);
				}
				else
				{
					//Adjust ball speeds when user adjust Speed curve
					if (_centerSpeed > CalculateCenterSpeed(_intervalPosFactor - 1) ||
					    _centerSpeed < CalculateCenterSpeed(_intervalPosFactor - 1))
					{
						double ratio = CalculateCenterSpeed(_intervalPosFactor) /
						               CalculateCenterSpeed(_intervalPosFactor - 1);
						ball.VelocityX *= ratio;
						ball.VelocityY *= ratio;
					}

					if (_speedVariation > CalculateSpeedVariation(_intervalPosFactor - 1) ||
					    _speedVariation < CalculateSpeedVariation(_intervalPosFactor - 1))
					{
						double ratio = CalculateSpeedVariation(_intervalPosFactor) /
						               CalculateSpeedVariation(_intervalPosFactor - 1);
						ball.VelocityX *= ratio;
						ball.VelocityY *= ratio;
					}

					int previousBallSize = CalculateSize(_intervalPosFactor - 1);
					if (_radius > previousBallSize || _radius < previousBallSize)
					{
						double ratio = (double) CalculateSize(_intervalPosFactor) / previousBallSize;
						ball.Radius *= ratio;
					}
					ball.MoveCount--;
				}

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
									colorIndex = Rand(0, Colors.Count);
								} while (ball.ColorIndex == colorIndex);
								ball.ColorIndex = colorIndex;

								do
								{
									colorIndex = Rand(0, Colors.Count);
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
						else if (ball.LocationX + ball.Radius >= _bufferWi)
						{
							ball.LocationX = _bufferWi - ball.Radius - 1;
							ball.VelocityX = -ball.VelocityX;
						}
						if (ball.LocationY - ball.Radius < 0)
						{
							ball.LocationY = ball.Radius;
							ball.VelocityY = -ball.VelocityY;
						}
						else if (ball.LocationY + ball.Radius >= _bufferHt)
						{
							ball.LocationY = _bufferHt - ball.Radius - 1;
							ball.VelocityY = -ball.VelocityY;
						}
						break;

					case BallType.Wrap:
						if (ball.LocationX + ball.Radius < 0)
						{
							ball.LocationX += _bufferWi + (ball.Radius* 2);
						}
						if (ball.LocationY + ball.Radius < 0)
						{
							ball.LocationY += _bufferHt + (ball.Radius * 2);
						}
						if (ball.LocationX - ball.Radius > _bufferWi)
						{
							ball.LocationX -= _bufferWi + (ball.Radius * 2);
						}
						if (ball.LocationY - ball.Radius > _bufferHt)
						{
							ball.LocationY -= _bufferHt + (ball.Radius * 2);
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
				y = Math.Abs((BufferHtOffset - y) + (_bufferHt - 1 + BufferHtOffset));
				y = y - BufferHtOffset;
				x = x - BufferWiOffset;
			}
			
			foreach (var ball in _balls)
			{
				//This saves going through all X and Y locations, significantly reducing render times.
				if (y <= ball.LocationY + ball.Radius + 1 && y >= ball.LocationY - ball.Radius &&
				    x <= ball.LocationX + ball.Radius + 1 && x >= ball.LocationX - ball.Radius)
				{
					double distanceFromBallCenter =
						DistanceFromPoint(new Point((int) Math.Round(ball.LocationX), (int) Math.Round(ball.LocationY)), x, y);

					double distance = distanceFromBallCenter > 1.4 && distanceFromBallCenter < 1.51
						? 2
						: distanceFromBallCenter;

					switch (BallFill)
					{
						case BallFill.Solid:
							if (distance <= ball.Radius)
							{
								if (_level < 1)
								{
									HSV hsv = HSV.FromRGB(Colors[ball.ColorIndex].GetColorAt(_intervalPos));
									hsv.V *= _level;
									frameBuffer.SetPixel(xCoord, yCoord, hsv);
									return;
								}
								Color color = Colors[ball.ColorIndex].GetColorAt(_intervalPos);
								frameBuffer.SetPixel(xCoord, yCoord, color);
								return;
							}
							break;
						case BallFill.Empty:
							if (distance >= ball.Radius - CalculateEdgeWidth(_intervalPosFactor) && distance < ball.Radius)
							{
								if (_level < 1)
								{
									HSV hsv = HSV.FromRGB(Colors[ball.ColorIndex].GetColorAt(_intervalPos));
									hsv.V *= _level;
									frameBuffer.SetPixel(xCoord, yCoord, hsv);
									return;
								}
								Color color = Colors[ball.ColorIndex].GetColorAt(_intervalPos);
								frameBuffer.SetPixel(xCoord, yCoord, color);
								return;
							}
							break;
						case BallFill.Inverse:
							if (distance <= ball.Radius)
							{
								frameBuffer.SetPixel(xCoord, yCoord, Color.Black);
								return;
							}
							break;
						case BallFill.Fade:
							if (distance <= ball.Radius * 0.9)
							{
								HSV hsv = HSV.FromRGB(Colors[ball.ColorIndex].GetColorAt(_intervalPos));
								hsv.V *= (1.0 - distance / ball.Radius) * _level;
								frameBuffer.SetPixel(xCoord, yCoord, hsv);
								return;
							}
							break;
						case BallFill.Gradient:
							if (distance <= ball.Radius)
							{
								if (_level < 1)
								{
									HSV hsv = HSV.FromRGB(Colors[ball.ColorIndex].GetColorAt(distance / ball.Radius));
									hsv.V *= _level;
									frameBuffer.SetPixel(xCoord, yCoord, hsv);
									return;
								}
								Color color = Colors[ball.ColorIndex].GetColorAt(distance / ball.Radius);
								frameBuffer.SetPixel(xCoord, yCoord, color);
								return;
							}
							break;
					}
				}
				//Add the background color when no other color has been used for the balls.
				if (BallFill == BallFill.Inverse) frameBuffer.SetPixel(xCoord, yCoord, inverseBackColor);
			}
		}

		private void CreateBalls(double minSpeed, double maxSpeed, int maxRandomTime)
		{
			while (_ballCount > _balls.Count)
			{
				BallClass m = new BallClass();

				//Sets Radius size and Ball location
				int radius = RandomRadius ? Rand(1, _radius + 1) : _radius;
				m.LocationX = Rand(radius, _bufferWi - radius);
				m.LocationY = Rand(radius, _bufferHt - radius);

				if (Collide)
				{
					//Will not add a ball if one exists in the same spot.
					foreach (BallClass ball in _balls)
					{
						if ((m.LocationX >= ball.LocationX && m.LocationX - radius < ball.LocationX) ||
						    (m.LocationY >= ball.LocationY && m.LocationY - radius < ball.LocationY)) return;
					}
				}

				double speed = RandDouble() * (maxSpeed - minSpeed) + minSpeed;
				double vx = RandDouble() * speed;
				double vy = RandDouble() * speed;
				if (Rand(0, 2) == 0) vx = -vx;
				if (Rand(0, 2) == 0) vy = -vy;
				m.VelocityX = vx;
				m.VelocityY = vy;
				m.Radius = radius;
				m.BallGuid = Guid.NewGuid();
				m.ColorIndex = Rand(0, Colors.Count);
				m.MoveCount = Rand(5, maxRandomTime);
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
			public int MoveCount;
		}

		private double CalculateCenterSpeed(double intervalPosFactor)
		{
			return ScaleCurveToValue(CenterSpeedCurve.GetValue(intervalPosFactor), (double)_maxBuffer / 10, 0) * FrameTime / 50d;
		}

		private double CalculateSpeedVariation(double intervalPosFactor)
		{
			return ScaleCurveToValue(SpeedVariationCurve.GetValue(intervalPosFactor), (double)_maxBuffer / 10, 0) * FrameTime / 50d;
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

		private int CalculateRandomMax(double intervalPosFactor)
		{
			return (int)ScaleCurveToValue(RandomMaxCurve.GetValue(intervalPosFactor), 40, 5);
		}
	}
}
