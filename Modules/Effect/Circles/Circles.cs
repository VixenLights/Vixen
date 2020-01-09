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

namespace VixenModules.Effect.Circles
{
	public class Circles : PixelEffectBase
	{
		private CirclesData _data;
		private double _circleCount;
		private int _maxBuffer;
		private int _colorIndex;

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
		[ProviderDisplayName(@"RadialDirection")]
		[ProviderDescription(@"RadialDirection")]
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
		[ProviderDisplayName(@"RadialSpeed")]
		[ProviderDescription(@"RadialSpeed")]
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
		[ProviderDisplayName(@"Size")]
		[ProviderDescription(@"Size")]
		[PropertyOrder(5)]
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
		[ProviderDisplayName(@"CircleEdgeWidth")]
		[ProviderDescription(@"CircleEdgeWidth")]
		[PropertyOrder(6)]
		public Curve CircleEdgeWidthCurve
		{
			get { return _data.CircleEdgeWidthCurve; }
			set
			{
				_data.CircleEdgeWidthCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Movement

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"HorizontalOffset")]
		[ProviderDescription(@"HorizontalOffset")]
		[PropertyOrder(1)]
		public Curve XOffsetCurve
		{
			get { return _data.XOffsetCurve; }
			set
			{
				_data.XOffsetCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"VerticalOffset")]
		[ProviderDescription(@"VerticalOffset")]
		[PropertyOrder(2)]
		public Curve YOffsetCurve
		{
			get { return _data.YOffsetCurve; }
			set
			{
				_data.YOffsetCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 3)]
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
		[ProviderCategory(@"Brightness", 4)]
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
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
		}

		private void UpdateColorAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(4);
			{
				propertyStates.Add("CircleEdgeWidthCurve", CircleFill == CircleFill.Empty);
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
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/circles/"; }
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
			_maxBuffer = Math.Max(BufferHt - 1, BufferWi - 1);
		}

		protected override void CleanUpRender()
		{
			//Nothing to do
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			var bufferHt = BufferHt;
			var bufferWi = BufferWi;
			double intervalPos = GetEffectTimeIntervalPosition(frame);
			double intervalPosFactor = intervalPos*100;
			int xOffset = CalculateXOffset(intervalPosFactor);
			int yOffset = CalculateYOffset(intervalPosFactor);
			double level = LevelCurve.GetValue(intervalPosFactor) / 100;
			double radius1 = CalculateRadialSize(intervalPosFactor);
			double radius = radius1 / 2 / _circleCount;
			double currentRadius = radius;
			double edgeWidth = CalculateEdgeWidth(intervalPosFactor, currentRadius);
			double barht = _maxBuffer / _circleCount;
			if (CircleFill == CircleFill.Empty || CircleFill == CircleFill.Fade) barht /= _circleCount;
			if (barht < 1) barht = 1;
			double blockHt = Colors.Count * barht;
			double foffset = frame * CalculateCenterSpeed(intervalPosFactor) / 4 % (blockHt + 1);
			barht = barht > 0 ? barht : 1;

			_circleCount = CalculateCircleCount(intervalPosFactor);
			if (CircleFill == CircleFill.Fade || CircleFill == CircleFill.Empty) _circleCount /= 5;

			for (int y = 0; y < BufferHt; y++)
			{
				for (int x = 0; x < BufferWi; x++)
				{
					CalculatePixel(x, y, level, frameBuffer, intervalPos, radius, radius1, currentRadius, foffset, barht, blockHt, bufferWi, bufferHt, xOffset, yOffset, edgeWidth);
				}
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var bufferHt = BufferHt;
			var bufferWi = BufferWi;
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
			for (int frame = 0; frame < numFrames; frame++)
			{

				frameBuffer.CurrentFrame = frame;
				double intervalPos = GetEffectTimeIntervalPosition(frame);
				double intervalPosFactor = intervalPos * 100;
				int xOffset = CalculateXOffset(intervalPosFactor);
				int yOffset = CalculateYOffset(intervalPosFactor);
				double level = LevelCurve.GetValue(intervalPosFactor) / 100;
				double radius1 = CalculateRadialSize(intervalPosFactor);
				double radius = radius1 / 2 / _circleCount;
				double currentRadius = radius;
				double edgeWidth = CalculateEdgeWidth(intervalPosFactor, currentRadius);
				double barht = _maxBuffer / _circleCount;
				if (CircleFill == CircleFill.Empty || CircleFill == CircleFill.Fade) barht /= _circleCount;
				if (barht < 1) barht = 1;
				double blockHt = Colors.Count * barht;
				double foffset = frame * CalculateCenterSpeed(intervalPosFactor) / 4 % (blockHt + 1);
				barht = barht > 0 ? barht : 1;

				_circleCount = CalculateCircleCount(intervalPosFactor) / 2;
				if (CircleFill == CircleFill.Fade || CircleFill == CircleFill.Empty) _circleCount /= 5;
				foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
				{
					foreach (var elementLocation in elementLocations)
					{
						CalculatePixel(elementLocation.X, elementLocation.Y, level, frameBuffer, intervalPos, radius, radius1, currentRadius, foffset, barht, blockHt, bufferWi, bufferHt, xOffset, yOffset, edgeWidth);
					}
				}
			}
		}

		private void CalculatePixel(int x, int y, double level, IPixelFrameBuffer frameBuffer, double intervalPos, double radius, double radius1, double currentRadius, double foffset, double barht, double blockHt, int bufferWi, int bufferHt, int xOffset, int yOffset, double edgeWidth)
		{
			int yCoord = y;
			int xCoord = x;
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				//Flip me over so and offset my coordinates I can act like the string version
				y = Math.Abs((BufferHtOffset - y) + (bufferHt - 1 + BufferHtOffset));
				y = y - BufferHtOffset;
				x = x - BufferWiOffset;
			}

			//This saves going through all X and Y locations significantly reducing render times.
			if ((y >= ((bufferWi - 1) / 2) + radius1 + 1 || y <= ((bufferWi - 1) / 2) - radius1) && (x >= ((bufferWi - 1) / 2) + radius1 + 1 || x <= ((bufferWi - 1) / 2) - radius1)) return;

			double distanceFromBallCenter = DistanceFromPoint(new Point((bufferWi - 1) / 2, (bufferHt - 1) / 2), x + xOffset, y + yOffset);

			int distance = distanceFromBallCenter > 1.4 && distanceFromBallCenter < 1.51
				? 2
				: (int) Math.Round(distanceFromBallCenter);

			radius = (radius * _circleCount);
			bool finished = false;

			switch (CircleRadialDirection)
			{
				case CircleRadialDirection.In:
					for (int i = 0; i <= _circleCount; i++)
					{
						finished = SetFramePixel(i, foffset, blockHt, barht, intervalPos, level, frameBuffer, xCoord, yCoord,
							distance, radius, currentRadius, radius1, edgeWidth);
						if (finished) return;
						radius = radius - currentRadius;
					}
					break;
				case CircleRadialDirection.Out:
					for (int i = (int)_circleCount; i >= 0; i--)
					{
						finished = SetFramePixel(i, foffset, blockHt, barht, intervalPos, level, frameBuffer, xCoord, yCoord,
							distance, radius, currentRadius, radius1, edgeWidth);
						if (finished) return;
						radius = radius - currentRadius;
					}
					break;
			}
		}

		private bool SetFramePixel(int i, double foffset, double blockHt, double barht, double intervalPos, double level, IPixelFrameBuffer frameBuffer, int xCoord, int yCoord, int distance, double radius, double currentRadius, double radius1, double edgeWidth)
		{
			if (distance <= radius && distance >= radius - currentRadius)
			{
				double n = i - foffset + blockHt;

				_colorIndex = (int) ((n)%blockHt/barht);
				Color color = Colors[_colorIndex].GetColorAt(intervalPos);
				switch (CircleFill)
				{
					case CircleFill.GradientOverElement: //Gradient over Element
						color = Colors[_colorIndex].GetColorAt((1 / radius1) * radius);
						break;
					case CircleFill.Empty:
						if (!(distance >= radius - edgeWidth)) return false;
						break;
					case CircleFill.Fade:
						HSV hsv = HSV.FromRGB(color);
						hsv.V *= 1.0 - distance/radius * level;
						frameBuffer.SetPixel(xCoord, yCoord, hsv);
						return true;
				}
				if (level < 1)
				{
					HSV hsv = HSV.FromRGB(color);
					hsv.V = hsv.V * level;
					color = hsv.ToRGB();
				}
				frameBuffer.SetPixel(xCoord, yCoord, color);
				return true;
			}

			return false;
		}

		private double CalculateCenterSpeed(double intervalPos)
		{
			return ScaleCurveToValue(CenterSpeedCurve.GetValue(intervalPos), 10, 0)  * FrameTime / 50d;
		}

		private double CalculateRadialSize(double intervalPos)
		{
			return ScaleCurveToValue(SizeCurve.GetValue(intervalPos), _maxBuffer * 1.1, 1);
		}

		private double CalculateEdgeWidth(double intervalPosFactor, double currentRadius)
		{
			double value = ScaleCurveToValue(CircleEdgeWidthCurve.GetValue(intervalPosFactor), currentRadius, 1);
			if (value < 1) value = 1;
			return value;
		}

		private double CalculateCircleCount(double intervalPosFactor)
		{
			double value = (int)ScaleCurveToValue(CircleCountCurve.GetValue(intervalPosFactor), _maxBuffer / (double)2, 1);
			if (value < 1) value = 1;
			return value;
		}

		private int CalculateXOffset(double intervalPos)
		{
			return (int)ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), -_maxBuffer * 1.1, _maxBuffer * 1.1);
		}

		private int CalculateYOffset(double intervalPos)
		{
			return (int)ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), -_maxBuffer * 1.1, _maxBuffer * 1.1);
		}
	}
}
