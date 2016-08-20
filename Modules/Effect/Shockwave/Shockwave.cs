using System;
using System.Drawing;
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

			double centerRadius = StartRadius + (EndRadius - StartRadius) * effectPositionAdjust;
			double halfWidth = (StartWidth + (EndWidth - StartWidth) * effectPositionAdjust) / 2.0;
			var radius1 = centerRadius - halfWidth;
			var radius2 = centerRadius + halfWidth;
			
			double step = GetStepAngle(StartRadius, EndRadius);
			
			for (double currentAngle = 0.0; currentAngle <= 360.0; currentAngle += step)
			{
				for (double r = Math.Max(0.0, radius1); r <= radius2; r += 0.5)
				{
					double x1 = Math.Sin(ToRadians(currentAngle)) * r + posX;
					double y1 = Math.Cos(ToRadians(currentAngle)) * r + posY;

					if (BlendEdges)
					{
						if (x1 >= 0 && x1 < BufferWi && y1 >= 0 && y1 < BufferHt)
						{
							double colorPct = 1.0 - Math.Abs(r - centerRadius)/halfWidth;
							if (colorPct > 0.0)
							{
								HSV hsv = HSV.FromRGB(c);
								hsv.V = hsv.V*colorPct;
								frameBuffer.SetPixel((int) x1, (int) y1, hsv);
							}
						}
					}
					else
					{
						if (x1 >= 0 && x1 < BufferWi && y1 >= 0 && y1 < BufferHt)
						{
							frameBuffer.SetPixel((int)x1, (int)y1, c);
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
			
			double step = GetStepAngle(StartRadius, EndRadius);

			for (int effectFrame = 0; effectFrame < numFrames; effectFrame++)
			{
				frameBuffer.InitializeNextFrame();
				double position = GetEffectTimeIntervalPosition(effectFrame);
				double effectPositionAdjust = CalculateAcceleration(position, Acceleration);
				Color c = Color.GetColorAt(position);
				double centerRadius = StartRadius + (EndRadius - StartRadius) * effectPositionAdjust;
				double halfWidth = (StartWidth + (EndWidth - StartWidth) * effectPositionAdjust) / 2.0;
				var radius1 = centerRadius - halfWidth;
				var radius2 = centerRadius + halfWidth;
				
				for (double currentAngle = 0.0; currentAngle <= 360.0; currentAngle += step)
				{
					for (double r = Math.Max(0.0, radius1); r <= radius2; r += 0.5)
					{
						double x1 = (Math.Sin(ToRadians(currentAngle)) * r + posX);
						double y1 = (Math.Cos(ToRadians(currentAngle)) * r + posY);

						if (BlendEdges)
						{
							if (x1 >= 0 && x1 < BufferWi + BufferWiOffset && y1 >= 0 && y1 < BufferHt + BufferHtOffset)
							{
								double colorPct = 1.0 - Math.Abs(r - centerRadius)/halfWidth;
								if (colorPct > 0.0)
								{
									HSV hsv = HSV.FromRGB(c);
									hsv.V = hsv.V*colorPct;
									frameBuffer.UpdatePixel(effectFrame, (int) x1, (int) y1, hsv);
								}
							}
						}
						else
						{
							if (x1 >= 0 && x1 < BufferWi + BufferWiOffset && y1 >= 0 && y1 < BufferHt + BufferHtOffset)
							{
								frameBuffer.SetPixel((int)x1, (int)y1, c);
							}
						}
					}
				}
			}

		}


		private double ToRadians(double angle)
		{
			return (Math.PI / 180) * angle;
		}


	}
}
