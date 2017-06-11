using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Annotations;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Circles
{
	public class Circles : PixelEffectBase
	{
		private CirclesData _data;
		private readonly List<BallClass> _balls = new List<BallClass>();
		private int _circleCount;
		private static Random _random = new Random();
		private int _maxBuffer;

		public Circles()
		{
			_data = new CirclesData();
	//		EnableTargetPositioning(true, true);
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
		[ProviderDisplayName(@"CircleType")]
		[ProviderDescription(@"CircleType")]
		[PropertyOrder(0)]
		public CircleType CircleType
		{
			get { return _data.CircleType; }
			set
			{
				_data.CircleType = value;
				UpdateTypeAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"CircleFill")]
		[ProviderDescription(@"CircleFill")]
		[PropertyOrder(1)]
		public CircleFill CircleFill
		{
			get { return _data.CircleFill; }
			set
			{
				_data.CircleFill = value;
				UpdateColorAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		//[Value]
		//[ProviderCategory(@"Config", 1)]
		//[ProviderDisplayName(@"CircleRadialDirection")]
		//[ProviderDescription(@"CircleRadialDirection")]
		//[PropertyOrder(2)]
		//public CircleRadialDirection CircleRadialDirection
		//{
		//	get { return _data.CircleRadialDirection; }
		//	set
		//	{
		//		_data.CircleRadialDirection = value;
		//		IsDirty = true;
		//		OnPropertyChanged();
		//	}
		//}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyOrder(3)]
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
		[PropertyOrder(4)]
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
		[ProviderDisplayName(@"Radius")]
		[ProviderDescription(@"Radius")]
		[PropertyOrder(5)]
		public Curve RadiusCurve
		{
			get { return _data.RadiusCurve; }
			set
			{
				_data.RadiusCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"CircleCount")]
		[ProviderDescription(@"CircleCount")]
		[PropertyOrder(6)]
		public Curve CircleCountCurve
		{
			get { return _data.CircleCountCurve; }
			set
			{
				_data.CircleCountCurve = value;
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
			UpdateColorAttribute(false);
			UpdateTypeAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		private void UpdateColorAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2);
			{
				propertyStates.Add("Colors", CircleFill != CircleFill.Inverse);
				propertyStates.Add("BackgroundColor", CircleFill == CircleFill.Inverse);
			}
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateTypeAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2);
			{
				propertyStates.Add("RandomRadius", CircleType != CircleType.Circles);
				propertyStates.Add("Collide", CircleType != CircleType.Circles);
			}
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
				_data = value as CirclesData;
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
			_maxBuffer = Math.Max(BufferHt, BufferWi);
		}

		protected override void CleanUpRender()
		{
			_balls.Clear();
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			double intervalPos = GetEffectTimeIntervalPosition(frame);
			double intervalPosFactor = intervalPos * 100;

			int radius = Convert.ToInt16(CalculateRadius(intervalPosFactor));
			double centerSpeed = CalculateCenterSpeed(intervalPosFactor);
			double speedVariation = CalculateSpeedVariation(intervalPosFactor);
			double level = LevelCurve.GetValue(intervalPosFactor) / 100;

			double minSpeed = centerSpeed - (speedVariation / 2);
			double maxSpeed = centerSpeed + (speedVariation / 2);
			if (minSpeed < 100)
				minSpeed = 100;
			if (maxSpeed > 350)
				maxSpeed = 350;

			_circleCount = Convert.ToInt16(CircleCountCurve.GetValue(intervalPosFactor));

			//Generates Circles from center of grid.
			if (CircleType == CircleType.Circles)
			{
				for (int y = 0; y < BufferHt; y++)
				{
					for (int x = 0; x < BufferWi; x++)
					{
						HSV hsv = new HSV();
						double radius1 = (CalculateRadialRadius(intervalPosFactor) / (double)2 / (_circleCount + 1));
						double currentRadius = radius1;
						int colorIndex = 0;
						double distanceFromBallCenter = DistanceFromPoint(new Point(BufferWi / 2, BufferHt / 2), x, y);//, x + frame, y + frame will move the Circle around the grid.

						int distance = distanceFromBallCenter > 1.4 && distanceFromBallCenter < 1.51
							? 2
							: (int)Math.Round(distanceFromBallCenter);

						if (distance >= CalculateRadialRadius(intervalPosFactor) && CircleFill == CircleFill.Inverse) //distance - frame makes Circle go in or out.
						{
							Color backColor = BackgroundColor.GetColorAt(intervalPos);
							frameBuffer.SetPixel(x, y, backColor);
							continue;
						}

						for (int i = 0; i < _circleCount; i++)
						{
							if (colorIndex == Colors.Count)
								colorIndex = 0;

							hsv = HSV.FromRGB(Colors[colorIndex].GetColorAt(intervalPos));
							if (distance <= radius1 && distance >= radius1 - currentRadius) //distance - frame makes Circle go in or out.
							{
								switch (CircleFill)
								{
									case CircleFill.Solid:
										hsv.V = hsv.V * level;
										frameBuffer.SetPixel(x, y, hsv);
										break;
									case CircleFill.Gradient:
										hsv = HSV.FromRGB(Colors[colorIndex].GetColorAt(distance / radius1));
										hsv.V = hsv.V * level;
										frameBuffer.SetPixel(x, y, hsv);
										break;
									case CircleFill.Empty:
										if (distance == (int) radius1)
										{
											hsv.V = hsv.V*level;
											frameBuffer.SetPixel(x, y, hsv);
										}
										break;
									case CircleFill.Fade:
										if (distance <= radius1)
										{
											hsv.V *= 1.0 - distance / radius1;
											hsv.V = hsv.V * level;
											frameBuffer.SetPixel(x, y, hsv);
										}
										break;
								}
							}
							colorIndex++;

							radius1 = radius1 + currentRadius;
						}
					}
				}
				return;
			}

			//HSV hsv = new HSV();
			//int ii, n;
			//int colorIdx;

			//int position = Convert.ToInt16(intervalPos * 1000) % 1;
			//int minBufferSize = Math.Max(BufferWi, BufferHt);
			//int barht = minBufferSize / (11 + 1);// 11 was thickness
			//if (barht < 1) barht = 1;
			//int maxRadius = effectState > minBufferSize ? minBufferSize : effectState / 2 + thickness;
			//int blockHt = Colors.Count * barht;
			//int f_offset = effectState / 4 % (blockHt + 1);

			//barht = barht > 0 ? barht : 1;

			//HSV lastColor = new HSV();
			//for (ii = maxRadius; ii >= 0; ii--)
			//{
			//	n = ii - f_offset + blockHt;
			//	colorIdx = (n) % blockHt / barht;
			//	hsv = HSV.FromRGB(Colors[colorIdx].GetColorAt(intervalPos));

			//	hsv.V = hsv.V * level;
			//	if (CircleType == CircleType.Radial3D)
			//	{
			//		hsv.H *= (float)(barht - n % barht - 1) / barht; //= (float)((double)(ii + 1) / maxRadius); //1.0 * (ii - intervalPos) / maxRadius;// / ((double)maxRadius / number * 70);
			//		//hsv.hue = 1.0*(ii+effectState)/(maxRadius/number);
			//		//			if (hsv.H > 1.0) hsv.H = hsv.H - (long)hsv.H;
			//		//			hsv.S = 1.0;
			//	}
			//	if (lastColor != hsv)
			//	{
			//		DrawCircle(x, y, ii, hsv, true, frameBuffer, ball);
			//		lastColor = hsv;
			//	}
			//}

			if (frame == 0)
			{
				if (CircleType != CircleType.Circles)
				{
					// Create initial Balls/Circles.
					for (int i = 0; i < _circleCount; i++)
					{
						int rad = RandomRadius ? _random.Next(3, radius) : radius;
						CreateBalls(minSpeed, maxSpeed, rad);
					}
				}
			}

			//Creates more balls if the Circle count curve position is above the current number of balls
			if (_circleCount > _balls.Count)
			{
				int rad = RandomRadius ? _random.Next(3, radius) : radius;
				CreateBalls(minSpeed, maxSpeed, rad);
			}

			foreach (var ball in _balls)
			{
				if (_circleCount < _balls.Count)
				{ //Removes balls if circle count curve position is below the current number of balls. Will only remove ones that hit the edge of the grid.
					if (ball.LocationX + radius == BufferWi || ball.LocationY + radius == BufferHt ||
					    ball.LocationX - radius == 0 || ball.LocationY + radius == 0)
					{
						_balls.Remove(ball);
						break;
					}
				}

				//Adjust ball speeds when user adjust Speed curve
				if (centerSpeed > CalculateCenterSpeed(intervalPosFactor - 1) || centerSpeed < CalculateCenterSpeed(intervalPosFactor - 1))
				{
					double ratio = (double)CalculateCenterSpeed(intervalPosFactor) / CalculateCenterSpeed(intervalPosFactor - 1);
					ball.VelocityX *= ratio;
					ball.VelocityY *= ratio;
				}

				if (speedVariation > CalculateSpeedVariation(intervalPosFactor - 1) || speedVariation < CalculateSpeedVariation(intervalPosFactor - 1))
				{
					double ratio = (double)CalculateSpeedVariation(intervalPosFactor) / CalculateSpeedVariation(intervalPosFactor - 1);
					ball.VelocityX *= ratio;
					ball.VelocityY *= ratio;
				}

				if (radius > CalculateRadius(intervalPosFactor - 1) || radius < CalculateRadius(intervalPosFactor - 1))
				{
				//	double ratio = (double)CalculateRadius(intervalPosFactor) / CalculateRadius(intervalPosFactor - 1);
					ball.Radius = CalculateRadius(intervalPosFactor);
				}

				// Move the ball.
				int new_x = (int)(ball.LocationX + ball.VelocityX);
				int new_y = (int)(ball.LocationY + ball.VelocityY);

				//Checks to see if any are going to collide and if they are then adjust there location and change to opposite direction.
				if (Collide)
				{
					foreach (var ball1 in _balls)
					{
						var minRadius = Math.Min(ball.Radius, ball1.Radius);
						if ((ball1.LocationX + minRadius >= new_x && ball1.LocationX - minRadius < new_x &&
							 ball1.LocationY + minRadius >= new_y && ball1.LocationY - minRadius < new_y) &&
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
							break;
						}
					}
				}

				ball.LocationX = new_x;
				ball.LocationY = new_y;

				switch (CircleType)
				{
					case CircleType.Bounce:
						if (new_x - ball.Radius < 0)
						{
							ball.LocationX = 0 + ball.Radius;
							ball.VelocityX = -ball.VelocityX;
						}
						else if (new_x + ball.Radius >= BufferWi)
						{
							ball.LocationX = BufferWi - ball.Radius - 1;
							ball.VelocityX = -ball.VelocityX;
						}
						if (new_y - ball.Radius < 0)
						{
							ball.LocationY = 0 + ball.Radius;
							ball.VelocityY = -ball.VelocityY;
						}
						else if (new_y + ball.Radius >= BufferHt)
						{
							ball.LocationY = BufferHt - ball.Radius - 1;
							ball.VelocityY = -ball.VelocityY;
						}
						break;

					case CircleType.Wrap:
						if (new_x < 0)
						{
							ball.LocationX = new_x += BufferWi;
						}
						if (new_y < 0)
						{
							ball.LocationY = new_y += BufferHt;
						}
						if (new_x > BufferWi)
						{
							ball.LocationX = new_x -= BufferWi;
						}
						if (new_y > BufferHt)
						{
							ball.LocationY = new_y -= BufferHt;
						}
						break;
				}

				for (int y = new_y - ball.Radius; y < new_y + ball.Radius + 1; y++)
				{
					for (int x = new_x - ball.Radius; x < new_x + ball.Radius + 1; x++)
					{
						HSV hsv = HSV.FromRGB(Colors[ball.ColorIndex].GetColorAt((intervalPos)));
						hsv.V = hsv.V*level;
						double distanceFromBallCenter = DistanceFromPoint(new Point(new_x, new_y), x, y);

						int distance = distanceFromBallCenter > 1.4 && distanceFromBallCenter < 1.51 ? 2 : (int) Math.Round(distanceFromBallCenter);

						switch (CircleFill)
						{
							case CircleFill.Solid:
								if (distance <= ball.Radius) frameBuffer.SetPixel(x, y, hsv);
								break;
							case CircleFill.Empty:
								if (distance == ball.Radius) frameBuffer.SetPixel(x, y, hsv);
								break;
							case CircleFill.Inverse:
								hsv = HSV.FromRGB(Color.Black);
								if (distance <= ball.Radius) frameBuffer.SetPixel(x, y, hsv);
								break;
							case CircleFill.Fade:
								if (distance <= ball.Radius)
								{
									hsv.V *= 1.0 - distance/(double) ball.Radius;
									frameBuffer.SetPixel(x, y, hsv);
								}
								break;
							case CircleFill.Gradient:
								if (distance <= ball.Radius)
								{
									hsv = HSV.FromRGB(Colors[ball.ColorIndex].GetColorAt(distance/(double) ball.Radius));
									hsv.V = hsv.V*level;
									frameBuffer.SetPixel(x, y, hsv);
								}
								break;
						}
					}
				}

				if (CircleFill == CircleFill.Inverse)
				{
					Color backColor = BackgroundColor.GetColorAt(intervalPos);
					for (int i = 0; i < BufferWi; i++)
					{
						for (int j = 0; j < BufferHt; j++)
						{
							if (frameBuffer.GetColorAt(i, j) == Color.Transparent)
							{
								frameBuffer.SetPixel(i, j, backColor);
							}
						}
					}
				}
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
			for (int frame = 0; frame < numFrames; frame++)
			{
				foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
				{
					foreach (var elementLocation in elementLocations)
					{
					}
				}
			}
		}

		private void CalculatePixel(int new_x, int new_y, int x, int y, int frame, double level, IPixelFrameBuffer frameBuffer, HSV hsv, BallClass ball, double intervalPos)
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
		}

		private void CreateBalls(double minSpeed, double maxSpeed, int radius)
		{
			BallClass m = new BallClass();
			m.LocationX = _random.Next(radius, BufferWi - radius);
			m.LocationY = _random.Next(radius, BufferHt - radius);

			//Will not add a ball if one exists in the same spot.
			foreach (var ball in _balls)
			{
				if ((m.LocationX >= ball.LocationX && m.LocationX - radius < ball.LocationX) || (m.LocationY >= ball.LocationY && m.LocationY - radius < ball.LocationY))
				{
					return;
				}
			}

			double vx = (double)_random.Next(Convert.ToInt16(minSpeed), Convert.ToInt16(maxSpeed)) / 100;
			double vy = (double)_random.Next(Convert.ToInt16(minSpeed), Convert.ToInt16(maxSpeed)) / 100;
			if (_random.Next(0, 2) == 0) vx = -vx;
			if (_random.Next(0, 2) == 0) vy = -vy;
			m.VelocityX = vx;
			m.VelocityY = vy;
			m.Radius = radius;
			m.BallGuid = Guid.NewGuid();
			m.ColorIndex = _random.Next(0, Colors.Count);
			_balls.Add(m);
		}

		public class BallClass
		{
			public double LocationX;
			public double LocationY;
			public double VelocityX;
			public double VelocityY;
			public int ColorIndex;
			public int Radius;
			public Guid BallGuid;
		}

		private int CalculateCenterSpeed(double intervalPos)
		{
			return (int)ScaleCurveToValue(CenterSpeedCurve.GetValue(intervalPos), 300, 100); //34
		}

		private int CalculateSpeedVariation(double intervalPos)
		{
			return (int)ScaleCurveToValue(SpeedVariationCurve.GetValue(intervalPos), 300, 0);
		}

		private int CalculateRadius(double intervalPos)
		{
			var value = (int)ScaleCurveToValue(RadiusCurve.GetValue(intervalPos), 20, 1);

			return value;
		}

		private int CalculateRadialRadius(double intervalPos)
		{
			var value = (int)ScaleCurveToValue(RadiusCurve.GetValue(intervalPos), _maxBuffer, 1);

			return value;
		}
	}
}
