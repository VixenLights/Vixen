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

namespace VixenModules.Effect.Borders
{
	public class Border : PixelEffectBase
	{
		private BorderData _data;
		private double _minBufferSize;

		public Border()
		{
			_data = new BorderData();
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
		[ProviderDisplayName(@"BorderThickness")]
		[ProviderDescription(@"BorderThickness")]
		public Curve ThicknessCurve
		{
			get { return _data.ThicknessCurve; }
			set
			{
				_data.ThicknessCurve = value;
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
		[PropertyOrder(2)]
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

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as BorderData;
				InitAllAttributes();
				IsDirty = true;
			}
		}

		private void InitAllAttributes()
		{
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
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
			double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100) / 100;
			double thickness = ThicknessCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100);

			HSV hsv = new HSV(0.5, 1.0, 1.0);


			if (effectFrame == 0)
			{
				_minBufferSize = (double) (Math.Min(BufferHt, BufferWi)) / 200;
			}

			int currentThickness = Convert.ToInt16(thickness * _minBufferSize);
			Color color = Color.GetColorAt((intervalPosFactor) / 100);
			hsv = HSV.FromRGB(color);
			hsv.V = hsv.V * level;
			for (int x = 0; x < BufferWi; x++)
			{
				for (int y = 0; y < BufferHt; y++)
				{
						CalculatePixel(x, y, frameBuffer, hsv, currentThickness);
				}
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{

			_minBufferSize = (double)(Math.Min(BufferHt, BufferWi)) / 200;
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);

			for (int effectFrame = 0; effectFrame < numFrames; effectFrame++)
			{
				frameBuffer.CurrentFrame = effectFrame;

				var intervalPos = GetEffectTimeIntervalPosition(effectFrame);
				var intervalPosFactor = intervalPos * 100;
				double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100) / 100;
				double thickness = ThicknessCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100);
				if (effectFrame == 0)
				{
					_minBufferSize = (double)(Math.Min(BufferHt, BufferWi)) / 200;
				}

				int currentThickness = Convert.ToInt16(thickness * _minBufferSize);
				HSV hsv = new HSV(0.5, 1.0, 1.0);
				Color color = Color.GetColorAt((intervalPosFactor) / 100);
				hsv = HSV.FromRGB(color);
				hsv.V = hsv.V * level;

				foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
				{
					foreach (var elementLocation in elementLocations)
					{
							CalculatePixel(elementLocation.X, elementLocation.Y, frameBuffer, hsv, currentThickness);
					}
				}
			}

		}


		private void CalculatePixel(int x, int y, IPixelFrameBuffer frameBuffer, HSV hsv, int currentThickness)
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
			if (y <= currentThickness || y > BufferHt - currentThickness - 2 || x <= currentThickness ||
				x > BufferWi - currentThickness - 2)
			{
				frameBuffer.SetPixel(xCoord, yCoord, hsv);
			}
		}
	}
}
