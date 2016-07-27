using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
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

namespace VixenModules.Effect.Circles
{
	public class Circles : PixelEffectBase
	{
		private CirclesData _data;
		private readonly List<BallClass> _balls = new List<BallClass>();
		private readonly List<TempBallClass> _tempBalls = new List<TempBallClass>();
		private List<TempBallClass> _tempBalls1 = new List<TempBallClass>();
		private int _circleCount;
		private IPixelFrameBuffer _tempBuffer;
		private static Random _random = new Random();

		public Circles()
		{
			_data = new CirclesData();
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
				UpdateRadialAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"CircleRadialDirection")]
		[ProviderDescription(@"CircleRadialDirection")]
		[PropertyOrder(1)]
		public CircleRadialDirection CircleRadialDirection
		{
			get { return _data.CircleRadialDirection; }
			set
			{
				_data.CircleRadialDirection = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"CircleFill")]
		[ProviderDescription(@"CircleFill")]
		[PropertyOrder(2)]
		public CircleFill CircleFill
		{
			get { return _data.CircleFill; }
			set
			{
				_data.CircleFill = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

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
			UpdateRadialAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		//Used to hide Colors from user when Rainbow type is selected and unhides for the other types.
		private void UpdateRadialAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2);
			{
				propertyStates.Add("CircleRadialDirection", CircleType == CircleType.Radial || CircleType == CircleType.Radial3D);
				propertyStates.Add("CircleFill", CircleType != CircleType.Radial && CircleType != CircleType.Radial3D);
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
		}

		protected override void CleanUpRender()
		{
			_balls.Clear();
			_tempBalls.Clear();
			_tempBalls1.Clear();
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			var intervalPos = GetEffectTimeIntervalPosition(frame);
			var intervalPosFactor = intervalPos*100;

			int radius = Convert.ToInt16(CalculateRadius(intervalPosFactor));
			var centerSpeed = CalculateCenterSpeed(intervalPosFactor);
			var radialSpeed = CalculateRadialSpeed(intervalPosFactor);
			var speedVariation = CalculateSpeedVariation(intervalPosFactor);

			var minSpeed = centerSpeed - (speedVariation / 2);
			var maxSpeed = centerSpeed + (speedVariation / 2);
			if (minSpeed < 50)
				minSpeed = 50;
			if (maxSpeed > 350)
				maxSpeed = 350;

			_circleCount = Convert.ToInt16(CircleCountCurve.GetValue(intervalPosFactor));
			double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;

			if (CircleType == CircleType.Radial || CircleType == CircleType.Radial3D)
			{
				int start_x = BufferWi / 2;
				int start_y = BufferHt / 2;
				int currentframe = frame;
				if (CircleRadialDirection == CircleRadialDirection.Out)
				{
					currentframe = GetNumberFrames() - frame;
				}

				int effectState = currentframe * (radialSpeed);
				RenderRadial(start_x, start_y, radius, _circleCount, level, effectState, intervalPos, frameBuffer, null);
				return;
			}

			HSV hsv = new HSV();
			if (frame == 0)
			{
				// Create initial Balls/Circles.
				for (int i = 0; i < _circleCount; i++)
				{
					int rad = RandomRadius ? _random.Next(3, radius) : radius;
					CreateBalls(minSpeed, maxSpeed, rad);
				}
			}

			//Creates more balls if the Circle count curve position is above the current number of balls
			if (_circleCount > _balls.Count)
			{
				int rad = RandomRadius ? _random.Next(3, radius) : radius;
				CreateBalls(minSpeed, maxSpeed, rad);
			}
			else if (_circleCount < _balls.Count) //Removes balls if circle count curve position is below the current number of balls. Will only remove ones that hit the edge of the grid.
			{
				foreach (var ball in _balls)
				{
					if (ball.LocationX + radius == BufferWi || ball.LocationY + radius == BufferHt ||
					    ball.LocationX - radius == 0 || ball.LocationY + radius == 0)
					{
						_balls.Remove(ball);
						break;
					}
				}
			}

			//Adjust ball speeds when user adjust Speed curve
			if (centerSpeed > CalculateCenterSpeed(intervalPosFactor - 1) || centerSpeed < CalculateCenterSpeed(intervalPosFactor - 1))
			{
				double ratio = (double)CalculateCenterSpeed(intervalPosFactor) / CalculateCenterSpeed(intervalPosFactor - 1);
				foreach (var ball in _balls)
				{
					ball.VelocityX *= ratio;
					ball.VelocityY *= ratio;
				}
			}

			if (speedVariation > CalculateSpeedVariation(intervalPosFactor - 1) || speedVariation < CalculateSpeedVariation(intervalPosFactor - 1))
			{
				double ratio = (double)CalculateSpeedVariation(intervalPosFactor) / CalculateSpeedVariation(intervalPosFactor - 1);
				foreach (var ball in _balls)
				{
					ball.VelocityX *= ratio;
					ball.VelocityY *= ratio;
				}
			}

			foreach (var ball in _balls)
			{
				// Move the ball.
				double new_x = ball.LocationX + ball.VelocityX;
				double new_y = ball.LocationY + ball.VelocityY;

				if (Collide)
				{
					foreach (var ball1 in _balls)
					{
						//Need to fix up this collide stuff
						if ((ball1.LocationX + ball1.Radius >= new_x && ball1.LocationX - ball1.Radius < new_x &&
						     ball1.LocationY + ball1.Radius >= new_y && ball1.LocationY - ball1.Radius < new_y) &&
						    ball1.BallGuid != ball.BallGuid)
						{

							ball.VelocityX = -ball.VelocityX;
							ball.VelocityY = -ball.VelocityY;
							ball1.VelocityX = -ball1.VelocityX;
							ball1.VelocityY = -ball1.VelocityY;
							ball1.LocationY = ball1.LocationY + ball1.VelocityY;
							ball1.LocationX = ball1.LocationX + ball1.VelocityX;
							new_y = new_y + ball.VelocityY;
							new_x = new_x + ball.VelocityX;
							break;
						}
					}
				}

				ball.LocationX = new_x;
				ball.LocationY = new_y;

				if (CircleType == CircleType.Bounce)
				{
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
				}
				hsv = HSV.FromRGB(Colors[ball.ColorIndex].GetColorAt((intervalPos)));
				hsv.V = hsv.V*level;

				switch (CircleFill)
				{
					case CircleFill.Fade:
						DrawFadingCircle(Convert.ToInt16(new_x), Convert.ToInt16(new_y), ball.Radius, hsv, frameBuffer, ball);
						break;
					case CircleFill.Solid:
						DrawCircle(Convert.ToInt16(new_x), Convert.ToInt16(new_y), ball.Radius, hsv, true, frameBuffer, ball);
						break;
					case CircleFill.Empty:
						DrawCircle(Convert.ToInt16(new_x), Convert.ToInt16(new_y), ball.Radius, hsv, false, frameBuffer, ball);
						break;
					case CircleFill.Inverse:
						DrawCircle(Convert.ToInt16(new_x), Convert.ToInt16(new_y), ball.Radius, hsv, false, frameBuffer, ball);
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
						break;
				}
			}

		//	_tempBalls1 = _tempBalls;
		//	_tempBalls.Clear();
		}

		private void CreateBalls(double minSpeed, double maxSpeed, int radius)
		{
			BallClass m = new BallClass();

			m.LocationX = _random.Next(radius, BufferWi - radius);
			m.LocationY = _random.Next(radius, BufferHt - radius);

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

		private void DrawCircle(int x0, int y0, int radius, HSV hsv, bool filled, IPixelFrameBuffer framBuffer, BallClass ball)
		{
			int x = radius;
			int y = 0;
			int radiusError = 1 - x;

			if (CircleType == CircleType.Wrap)
			{
				while (x0 < 0)
				{
					ball.LocationX = x0 += BufferWi;
				}
				while (y0 < 0)
				{
					ball.LocationY = y0 += BufferHt;
				}
				while (x0 > BufferWi)
				{
					ball.LocationX = x0 -= BufferWi;
				}
				while (y0 > BufferHt)
				{
					ball.LocationY = y0 -= BufferHt;
				}
			}

			while (x >= y)
			{

				if (CircleFill == CircleFill.Inverse && CircleType != CircleType.Radial)
				{
					Color color = Color.Black;
					HSV hsv1 = HSV.FromRGB(color);
					DrawVLine(x0 - x, y0 - y, y0 + y, hsv1, framBuffer, ball);
					DrawVLine(x0 + x, y0 - y, y0 + y, hsv1, framBuffer, ball);
					DrawVLine(x0 - y, y0 - x, y0 + x, hsv1, framBuffer, ball);
					DrawVLine(x0 + y, y0 - x, y0 + x, hsv1, framBuffer, ball);
				}
				else if (!filled)
				{
					
					framBuffer.SetPixel(x + x0, y + y0, hsv);
					framBuffer.SetPixel(y + x0, x + y0, hsv);
					framBuffer.SetPixel(-x + x0, y + y0, hsv);
					framBuffer.SetPixel(-y + x0, x + y0, hsv);
					framBuffer.SetPixel(-x + x0, -y + y0, hsv);
					framBuffer.SetPixel(-y + x0, -x + y0, hsv);
					framBuffer.SetPixel(x + x0, -y + y0, hsv);
					framBuffer.SetPixel(y + x0, -x + y0, hsv);
				}
				else
				{
					DrawVLine(x0 - x, y0 - y, y0 + y, hsv, framBuffer, ball);
					DrawVLine(x0 + x, y0 - y, y0 + y, hsv, framBuffer, ball);
					DrawVLine(x0 - y, y0 - x, y0 + x, hsv, framBuffer, ball);
					DrawVLine(x0 + y, y0 - x, y0 + x, hsv, framBuffer, ball);
				}

				y++;
				if (radiusError < 0)
				{
					radiusError += 2 * y + 1;
				}
				else
				{
					x--;
					radiusError += 2 * (y - x) + 1;
				}
			}
		}

		private void DrawFadingCircle(int x0, int y0, int radius, HSV hsv, IPixelFrameBuffer frameBuffer, BallClass ball)
		{
			int r = radius;

			double full_brightness = hsv.V;
			while (r >= 0)
			{
				hsv.V = full_brightness * (1.0 - r / (double)radius);
				if (hsv.V > 0.2)
				{
					DrawCircle(x0, y0, r, hsv, true, frameBuffer, ball);
				}
				r--;
			}
		}

		private void DrawVLine(int x, int ystart, int yend, HSV hsv, IPixelFrameBuffer framBuffer, BallClass ball)
		{
			if (ystart > yend)
			{
				int i = ystart;
				ystart = yend;
				yend = i;
			}

	//		var ballCollide = false;
			for (int y = ystart; y <= yend; y++)
			{
				if (x < BufferWi && y < BufferHt && x >= 0 && y >= 0)
				{
					//if (Collide)
					//{
					//	foreach (var tempBallClass in _tempBalls1)
					//	{
					//		if (!ballCollide && tempBallClass.TempX == x && tempBallClass.TempY == y &&
					//			ball.BallGuid != tempBallClass.BallGuid)
					//		{
					//			ball.VelocityX = -(ball.VelocityX);
					//			ball.VelocityY = -(ball.VelocityY);
					//			//foreach (var tempBallClass1 in _balls)
					//			//{
					//			//	if (tempBallClass.BallGuid == tempBallClass1.BallGuid)
					//			//	{
					//			//		tempBallClass1.VelocityX = -(ball.VelocityX + 0.2);
					//			//		tempBallClass1.VelocityY = -(ball.VelocityY + 0.2);
					//			//	}
					//			//}

					//			ballCollide = true;
					//			return;
					//		}
					//	}
					//}

					framBuffer.SetPixel(x, y, hsv);

					//TempBallClass m = new TempBallClass();
					//m.BallGuid = ball.BallGuid;
					//m.TempX = x;
					//m.TempY = y;
					//_tempBalls.Add(m);
				}
			}
		}

		private void RenderRadial(int x, int y, int thickness, int number, double level, int effectState, double intervalPos, IPixelFrameBuffer frameBuffer, BallClass ball)
		{
			HSV hsv = new HSV();
			int ii, n;
			int colorIdx;

			int position = Convert.ToInt16(intervalPos * 1000) % 1;
			int minBufferSize = Math.Max(BufferWi, BufferHt);
			int barht = minBufferSize / (11 + 1);// 11 was thickness
			if (barht < 1) barht = 1;
			int maxRadius = effectState > minBufferSize ? minBufferSize : effectState / 2 + thickness;
			int blockHt = Colors.Count * barht;
			int f_offset = effectState / 4 % (blockHt + 1);

			barht = barht > 0 ? barht : 1;

			HSV lastColor = new HSV();
			for (ii = maxRadius; ii >= 0; ii--)
			{
				n = ii - f_offset + blockHt;
				colorIdx = (n) % blockHt / barht;
				hsv = HSV.FromRGB(Colors[colorIdx].GetColorAt(intervalPos));

				hsv.V = hsv.V * level;
				if (CircleType == CircleType.Radial3D)
				{
					hsv.H *= (float)(barht - n % barht - 1) / barht; //= (float)((double)(ii + 1) / maxRadius); //1.0 * (ii - intervalPos) / maxRadius;// / ((double)maxRadius / number * 70);
					//hsv.hue = 1.0*(ii+effectState)/(maxRadius/number);
		//			if (hsv.H > 1.0) hsv.H = hsv.H - (long)hsv.H;
		//			hsv.S = 1.0;
				}
				if (lastColor != hsv)
				{
					DrawCircle(x, y, ii, hsv, true, frameBuffer, ball);
					lastColor = hsv;
				}
			}
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

		public class TempBallClass
		{
			public int TempX;
			public int TempY;
			public Guid BallGuid;
		}

		private int CalculateCenterSpeed(double intervalPos)
		{
			return (int)ScaleCurveToValue(CenterSpeedCurve.GetValue(intervalPos), 200, 50);
		}

		private int CalculateRadialSpeed(double intervalPos)
		{
			return (int)ScaleCurveToValue(CenterSpeedCurve.GetValue(intervalPos), 193, 199);
		}

		private int CalculateSpeedVariation(double intervalPos)
		{
			return (int)ScaleCurveToValue(SpeedVariationCurve.GetValue(intervalPos), 300, 0);
		}

		private int CalculateRadius(double intervalPos)
		{
			var value = (int)ScaleCurveToValue(RadiusCurve.GetValue(intervalPos), 20, 0);

			return value;
		}
	}
}
