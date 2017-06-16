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
using Vixen.Sys;
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
		private int _circleCount;
		private static Random _random = new Random();
		private int _maxBuffer;
		private int _minBuffer;

		public Circles()
		{
			_data = new CirclesData();
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
		[ProviderDisplayName(@"CircleType")]
		[ProviderDescription(@"CircleType")]
		[PropertyOrder(0)]
		public CircleType CircleType
		{
			get { return _data.CircleType; }
			set
			{
				_data.CircleType = value;
				if (CircleType == CircleType.Circles) CircleRadialDirection = CircleRadialDirection.Out;
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

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"CircleRadialDirection")]
		[ProviderDescription(@"CircleRadialDirection")]
		[PropertyOrder(2)]
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
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
		}

		private void UpdateColorAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(4);
			{
				propertyStates.Add("Colors", CircleFill != CircleFill.Inverse);
				propertyStates.Add("BackgroundColor", CircleFill == CircleFill.Inverse);
				propertyStates.Add("CircleCountCurve", CircleFill != CircleFill.Inverse);
				propertyStates.Add("BallEdgeWidthCurve", CircleFill == CircleFill.Empty);
			}
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateTypeAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(4);
			{
				propertyStates.Add("CircleRadialDirection", CircleType != CircleType.Circles);
				propertyStates.Add("CenterSpeedCurve", CircleType != CircleType.Circles);
				propertyStates.Add("RadiusCurve", CircleType == CircleType.Circles);
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
			_maxBuffer = Math.Max(BufferHt - 1, BufferWi - 1);
			_minBuffer = Math.Min(BufferHt - 1, BufferWi - 1);
		}

		protected override void CleanUpRender()
		{
			
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			double intervalPos = GetEffectTimeIntervalPosition(frame);
			double intervalPosFactor = intervalPos*100;

			double centerSpeed = CalculateCenterSpeed(intervalPosFactor);
			double level = LevelCurve.GetValue(intervalPosFactor)/100;

			_circleCount = Convert.ToInt16(CircleCountCurve.GetValue(intervalPosFactor));

			for (int y = 0; y < BufferHt; y++)
			{
				for (int x = 0; x < BufferWi; x++)
				{
					CalculatePixel(x, y, level, frameBuffer, intervalPosFactor, intervalPos, frame);
				}
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
			for (int frame = 0; frame < numFrames; frame++)
			{

				frameBuffer.CurrentFrame = frame;
				double intervalPos = GetEffectTimeIntervalPosition(frame);
				double intervalPosFactor = intervalPos * 100;
				double level = LevelCurve.GetValue(intervalPosFactor) / 100;
				_circleCount = (int)(CircleCountCurve.GetValue(intervalPosFactor));
				foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
				{
					foreach (var elementLocation in elementLocations)
					{
						CalculatePixel(elementLocation.X, elementLocation.Y, level, frameBuffer, intervalPosFactor, intervalPos, frame);
					}
				}
			}
		}

		private void CalculatePixel(int x, int y, double level, IPixelFrameBuffer frameBuffer, double intervalPosFactor, double intervalPos, int frame)
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

			double radius1 = CircleType == CircleType.Radial ? _maxBuffer : CalculateRadialRadius(intervalPosFactor);
			
			//This saves going through all X and Y locations significantly reducing render times.
			if ((y >= ((BufferWi - 1) / 2) + radius1 + 1 || y <= ((BufferWi - 1) / 2) - radius1) && (x >= ((BufferWi - 1) / 2) + radius1 + 1 || x <= ((BufferWi - 1) / 2) - radius1)) return;

			double radius = radius1 / 2 / (_circleCount + 1);
			double currentRadius = radius;
			int colorIndex = 0;
			double distanceFromBallCenter = DistanceFromPoint(new Point((BufferWi - 1) / 2, (BufferHt - 1) / 2), x, y);
			//, x + frame, y + frame will move the Circle around the grid.

			int distance = distanceFromBallCenter > 1.4 && distanceFromBallCenter < 1.51
				? 2
				: (int) Math.Round(distanceFromBallCenter);

			if (distance >= CalculateRadialRadius(intervalPosFactor) && CircleFill == CircleFill.Inverse)
			{
				Color backColor = BackgroundColor.GetColorAt(intervalPos);
				frameBuffer.SetPixel(xCoord, yCoord, backColor);
				return;
			}

			int barht = _maxBuffer / _circleCount;
			if (barht < 1) barht = 1;
			int blockHt = Colors.Count * barht;
			int f_offset = frame * CalculateCenterSpeed(intervalPosFactor) / 4 % (blockHt + 1);
			barht = barht > 0 ? barht : 1;

			int outerRadius = _circleCount;
			if (CircleType == CircleType.Radial)
			{
				outerRadius = _maxBuffer;
				radius = radius1 / 2 / (_circleCount + 1);
			}

			for (int i = 0; i <= outerRadius; i++)
			{

				if (colorIndex >= Colors.Count)
					colorIndex = 0;

				if (CircleRadialDirection == CircleRadialDirection.In)
				{
					if (distance <= radius1 && distance >= radius1 - currentRadius)
					{
						SetFramePixel(i, f_offset, blockHt, barht, intervalPos, intervalPosFactor, level, frameBuffer, xCoord, yCoord, distance, radius1, colorIndex);
					}
					radius1 = radius1 - currentRadius;
				}
				else if (distance <= radius && distance >= radius - currentRadius) //distance - frame makes Circle go in or out.
				{
					SetFramePixel(i, f_offset, blockHt, barht, intervalPos, intervalPosFactor, level, frameBuffer, xCoord, yCoord, distance, radius, colorIndex);
				}

				colorIndex++;
				radius = radius + currentRadius;
			}
		}

		private void SetFramePixel(int i, int f_offset, int blockHt, int barht, double intervalPos, double intervalPosFactor, double level, IPixelFrameBuffer frameBuffer, int xCoord, int yCoord, int distance, double radius, int colorIndex)
		{
			int n = i - f_offset + blockHt;
			if (CircleType == CircleType.Radial) colorIndex = (n) % blockHt / barht;

			HSV hsv = HSV.FromRGB(Colors[colorIndex].GetColorAt(intervalPos));
			switch (CircleFill)
			{
				case CircleFill.Solid:
					hsv.V = hsv.V * level;
					frameBuffer.SetPixel(xCoord, yCoord, hsv);
					break;
				case CircleFill.Gradient:
					hsv = HSV.FromRGB(Colors[colorIndex].GetColorAt(distance / radius));
					hsv.V = hsv.V * level;
					frameBuffer.SetPixel(xCoord, yCoord, hsv);
					break;
				case CircleFill.Empty:
					if (distance >= radius - CalculateEdgeWidth(intervalPosFactor) && distance < radius)
						frameBuffer.SetPixel(xCoord, yCoord, hsv);
					break;
				case CircleFill.Fade:
					if (distance <= radius)
					{
						hsv.V *= 1.0 - distance / radius;
						hsv.V = hsv.V * level;
						frameBuffer.SetPixel(xCoord, yCoord, hsv);
					}
					break;
			}
		}

		private int CalculateCenterSpeed(double intervalPos)
		{
			return (int)ScaleCurveToValue(CenterSpeedCurve.GetValue(intervalPos), 10, 1); //34
		}

		private int CalculateRadialRadius(double intervalPos)
		{
			return (int)ScaleCurveToValue(RadiusCurve.GetValue(intervalPos), _maxBuffer, 1);
		}

		private int CalculateEdgeWidth(double intervalPosFactor)
		{
			int value = (int)ScaleCurveToValue(BallEdgeWidthCurve.GetValue(intervalPosFactor), 40, 1);
			if (value < 1) value = 1;
			return value;
		}
	}
}
