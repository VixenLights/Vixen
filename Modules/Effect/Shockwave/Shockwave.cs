using System;
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

namespace VixenModules.Effect.Shockwave
{
	public class Shockwave:PixelEffectBase
	{
		private ShockwaveData _data;
		
		public Shockwave()
		{
			_data = new ShockwaveData();
			EnableTargetPositioning(true, true);
			InitAllAttributes();
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
		[ProviderDisplayName(@"Center X")]
		[ProviderDescription(@"CenterX")]
		//[NumberRange(0, 100, 1)]
		[PropertyOrder(3)]
		public Curve CenterXCurve
		{
			get { return _data.CenterXCurve; }
			set
			{
				_data.CenterXCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Center Y")]
		[ProviderDescription(@"CenterY")]
		//[NumberRange(0, 100, 1)]
		[PropertyOrder(4)]
		public Curve CenterYCurve
		{
			get { return _data.CenterYCurve; }
			set
			{
				_data.CenterYCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Radius")]
		[ProviderDescription(@"RadiusCurve")]
		//[NumberRange(0, 750, 1)]
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
		[ProviderDisplayName(@"ScaledRadius")]
		[ProviderDescription(@"ScaledRadius")]
		[PropertyOrder(6)]
		public bool ScaledRadius
		{
			get { return _data.ScaledRadius; }
			set
			{
				_data.ScaledRadius = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Width")]
		[ProviderDescription(@"WidthCurve")]
		//[NumberRange(0, 255, 1)]
		[PropertyOrder(7)]
		public Curve WidthCurve
		{
			get { return _data.WidthCurve; }
			set
			{
				_data.WidthCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Acceleration")]
		[ProviderDescription(@"WaveAcceleration")]
		//[NumberRange(-10, 10, 1)]
		[PropertyOrder(9)]
		public Curve AccelerationCurve
		{
			get { return _data.AccelerationCurve; }
			set
			{
				_data.AccelerationCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		

		#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"ColorGradient")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(1)]
		public ColorGradient Color
		{
			get { return _data.Gradient; }
			set
			{
				_data.Gradient = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"Blend Edges")]
		[ProviderDescription(@"Blend the edges of the wave.")]
		[PropertyOrder(2)]
		public bool BlendEdges
		{
			get { return _data.BlendEdges; }
			set
			{
				_data.BlendEdges = value;
				IsDirty = true;
				OnPropertyChanged();
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
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/shockwave/"; }
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as ShockwaveData;
				InitAllAttributes();
				IsDirty = true;
			}
		}

		private void InitAllAttributes()
		{
			UpdateStringOrientationAttributes(true);
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		protected override void SetupRender()
		{
			//Not required
		}

		protected override void CleanUpRender()
		{
			//Not required
		}

		protected override void RenderEffect(int effectFrame, IPixelFrameBuffer frameBuffer)
		{
			var intervalPos = GetEffectTimeIntervalPosition(effectFrame);
			var intervalPosFactor = intervalPos * 100;
			double effectPositionAdjust = CalculateAcceleration(intervalPos, CalculateAcceleration(intervalPosFactor))*100.0;
			Color c = Color.GetColorAt(intervalPos);

			double posX;
			double posY;

			if (StringOrientation == StringOrientation.Vertical)
			{
				posX = BufferWi * CalculateCenterY(intervalPosFactor) / 100.0;
				posY = BufferHt * CalculateCenterX(intervalPosFactor) / 100.0;
			}
			else
			{
				posX = BufferWi * CalculateCenterX(intervalPosFactor) / 100.0;
				posY = BufferHt * CalculateCenterY(intervalPosFactor) / 100.0;
			}

			Point centerPoint = new Point((int)posX, (int)posY);
			double centerRadius = CalculateRadius(effectPositionAdjust, ScaledRadius?Math.Max(BufferHt, BufferWi):750);
			double halfWidth = CalculateWidth(effectPositionAdjust) / 2.0;
			var radius1 = Math.Max(0.0, centerRadius - halfWidth);
			var radius2 = centerRadius + halfWidth;
			
			Point currentPoint = Point.Empty;

			for (currentPoint.X = 0; currentPoint.X < BufferWi; currentPoint.X++)
			{
				for (currentPoint.Y = 0; currentPoint.Y < BufferHt; currentPoint.Y++)
				{
					var distance = DistanceFromCenter(centerPoint, currentPoint);
					if (ContainsPoint(distance, radius1, radius2))
					{
						if (BlendEdges)
						{
							double colorPct = 1.0 - Math.Abs(distance - centerRadius) / halfWidth;
							if (colorPct > 0.0)
							{
								HSV hsv = HSV.FromRGB(c);
								hsv.V = hsv.V * colorPct;
								frameBuffer.SetPixel(currentPoint.X, currentPoint.Y, hsv);
							}
						}
						else
						{
							frameBuffer.SetPixel(currentPoint.X, currentPoint.Y, c);
						}
					}
				}
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			//It kinda stinks so much of this code duplicates the RenderEffect method, but 
			//there are enough differences in the two that it needs to be. Trying to refactor
			//small parts of it out into reusable chunks is probably not fruitful at this time.
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
			Point currentPoint = Point.Empty;

			for (int effectFrame = 0; effectFrame < numFrames; effectFrame++)
			{
				var intervalPos = GetEffectTimeIntervalPosition(effectFrame);
				var intervalPosFactor = intervalPos * 100;
				double posX = BufferWi * CalculateCenterX(intervalPosFactor) / 100.0 + BufferWiOffset; 
				double posY = BufferHt * (100-CalculateCenterY(intervalPosFactor)) / 100.0 + BufferHtOffset;

				Point centerPoint = new Point((int)posX, (int)posY);

				frameBuffer.CurrentFrame = effectFrame;
				double effectPositionAdjust = CalculateAcceleration(intervalPos, CalculateAcceleration(intervalPosFactor))*100.0;
				Color c = Color.GetColorAt(intervalPos);
				double centerRadius = CalculateRadius(effectPositionAdjust, ScaledRadius?Math.Max(BufferHt, BufferWi):750);
				double halfWidth = CalculateWidth(effectPositionAdjust) / 2.0;

				var radius1 = Math.Max(0.0, centerRadius - halfWidth);
				var radius2 = centerRadius + halfWidth;

				foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
				{
					foreach (var elementLocation in elementLocations)
					{
						currentPoint.X = elementLocation.X;
						currentPoint.Y = elementLocation.Y;
						var distance = DistanceFromCenter(centerPoint, currentPoint);
						if (ContainsPoint(distance, radius1, radius2))
						{
							if (BlendEdges)
							{
								double colorPct = 1.0 - Math.Abs(distance - centerRadius)/halfWidth;
								if (colorPct > 0.0)
								{
									HSV hsv = HSV.FromRGB(c);
									hsv.V = hsv.V*colorPct;
									frameBuffer.SetPixel(currentPoint.X, currentPoint.Y, hsv);
								}
								else
								{
									frameBuffer.SetPixel(currentPoint.X, currentPoint.Y, System.Drawing.Color.Transparent);
								}
							}
							else
							{

								frameBuffer.SetPixel(currentPoint.X, currentPoint.Y, c);

							}
						}
						else
						{
							frameBuffer.SetPixel(currentPoint.X, currentPoint.Y, System.Drawing.Color.Transparent);
						}
					}
				}

			}

		}

		private double CalculateWidth(double intervalPos)
		{
			return ScaleCurveToValue(WidthCurve.GetValue(intervalPos), 255, 0);
		}

		private double CalculateCenterX(double intervalPos)
		{
			return ScaleCurveToValue(CenterXCurve.GetValue(intervalPos), 100, 0);
		}

		private double CalculateCenterY(double intervalPos)
		{
			return ScaleCurveToValue(CenterYCurve.GetValue(intervalPos), 100, 0);
		}

		private double CalculateAcceleration(double intervalPos)
		{
			return ScaleCurveToValue(AccelerationCurve.GetValue(intervalPos), 10, -10);
		}

		private double CalculateRadius(double intervalPos, int scale)
		{
			return ScaleCurveToValue(RadiusCurve.GetValue(intervalPos), scale, 0);
		}

		private double DistanceFromCenter(Point center, Point point)
		{
			return Math.Sqrt(Math.Pow((point.X - center.X), 2) + Math.Pow((point.Y - center.Y),2));
		}

		private bool ContainsPoint(double distance, double innerRadius, double outerRadius)
		{
			return distance <= outerRadius && distance >= innerRadius;
		}

	}
}
