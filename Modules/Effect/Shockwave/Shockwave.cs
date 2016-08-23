using System;
using System.Drawing;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
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
		[ProviderDescription(@"The X adjustment for the center of the wave.")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(3)]
		public int CenterX
		{
			get { return _data.CenterX; }
			set
			{
				_data.CenterX = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Center Y")]
		[ProviderDescription(@"The Y adjustment for the center of the wave.")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(4)]
		public int CenterY
		{
			get { return _data.CenterY; }
			set
			{
				_data.CenterY = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Start Radius")]
		[ProviderDescription(@"The starting radius of the wave.")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 250, 1)]
		[PropertyOrder(5)]
		public int StartRadius
		{
			get { return _data.StartRadius; }
			set
			{
				_data.StartRadius = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"End Radius")]
		[ProviderDescription(@"The ending radius of the wave.")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 250, 1)]
		[PropertyOrder(6)]
		public int EndRadius
		{
			get { return _data.EndRadius; }
			set
			{
				_data.EndRadius = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}


		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Start Width")]
		[ProviderDescription(@"The starting width of the wave.")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 255, 1)]
		[PropertyOrder(7)]
		public int StartWidth
		{
			get { return _data.StartWidth; }
			set
			{
				_data.StartWidth = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"End Width")]
		[ProviderDescription(@"The ending width of the wave.")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 255, 1)]
		[PropertyOrder(8)]
		public int EndWidth
		{
			get { return _data.EndWidth; }
			set
			{
				_data.EndWidth = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Acceleration")]
		[ProviderDescription(@"Controls how the wave accelerates out from the center."
				+" Positive numbers will go faster the further from the center it gets, while negative will slow it down.")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-10, 10, 1)]
		[PropertyOrder(9)]
		public int Acceleration
		{
			get { return _data.Acceleration; }
			set
			{
				_data.Acceleration = value;
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
			double position = GetEffectTimeIntervalPosition(effectFrame);
			double effectPositionAdjust = CalculateAcceleration(position, Acceleration);
			Color c = Color.GetColorAt(position);

			double posX = BufferWi * CenterX / 100.0;
			double posY = BufferHt * CenterY / 100.0;
			Point centerPoint = new Point((int)posX, (int)posY);
			double centerRadius = StartRadius + (EndRadius - StartRadius) * effectPositionAdjust;
			double halfWidth = (StartWidth + (EndWidth - StartWidth) * effectPositionAdjust) / 2.0;
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
			double posX = (BufferWi * CenterX / 100.0) + BufferWiOffset;
			double posY = (BufferHt * CenterY / 100.0) + BufferHtOffset;
			Point centerPoint = new Point((int)posX, (int)posY);
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
			Point currentPoint = Point.Empty;

			for (int effectFrame = 0; effectFrame < numFrames; effectFrame++)
			{
				double position = GetEffectTimeIntervalPosition(effectFrame);
				double effectPositionAdjust = CalculateAcceleration(position, Acceleration);
				Color c = Color.GetColorAt(position);
				double centerRadius = StartRadius + (EndRadius - StartRadius) * effectPositionAdjust;
				double halfWidth = (StartWidth + (EndWidth - StartWidth) * effectPositionAdjust) / 2.0;
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
