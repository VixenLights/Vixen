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
		[ProviderDisplayName(@"BorderType")]
		[ProviderDescription(@"BorderType")]
		[PropertyOrder(1)]
		public BorderType BorderType
		{
			get { return _data.BorderType; }
			set
			{
				_data.BorderType = value;
				UpdateBoderControlAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BorderThickness")]
		[ProviderDescription(@"BorderThickness")]
		[PropertyOrder(2)]
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

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"TopBorderThickness")]
		[ProviderDescription(@"TopBorderThickness")]
		[PropertyOrder(2)]
		public Curve TopThicknessCurve
		{
			get { return _data.TopThicknessCurve; }
			set
			{
				_data.TopThicknessCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BottomBorderThickness")]
		[ProviderDescription(@"BottomBorderThickness")]
		[PropertyOrder(3)]
		public Curve BottomThicknessCurve
		{
			get { return _data.BottomThicknessCurve; }
			set
			{
				_data.BottomThicknessCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"LeftBorderThickness")]
		[ProviderDescription(@"LeftBorderThickness")]
		[PropertyOrder(4)]
		public Curve LeftThicknessCurve
		{
			get { return _data.LeftThicknessCurve; }
			set
			{
				_data.LeftThicknessCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"RightBorderThickness")]
		[ProviderDescription(@"RightBorderThickness")]
		[PropertyOrder(5)]
		public Curve RightThicknessCurve
		{
			get { return _data.RightThicknessCurve; }
			set
			{
				_data.RightThicknessCurve = value;
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
			UpdateBoderControlAttribute(false);
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private void UpdateBoderControlAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(5)
			{
				{"ThicknessCurve", BorderType == BorderType.Single},
				{"TopThicknessCurve", BorderType != BorderType.Single},
				{"BottomThicknessCurve", BorderType != BorderType.Single},
				{"LeftThicknessCurve", BorderType != BorderType.Single},
				{"RightThicknessCurve", BorderType != BorderType.Single}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
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

			if (effectFrame == 0)
			{
				_minBufferSize = (double)(Math.Min(BufferHt, BufferWi)) / 100;
			}

			var intervalPos = GetEffectTimeIntervalPosition(effectFrame);
			var intervalPosFactor = intervalPos * 100;


			double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100) / 100;
			double thickness = (ThicknessCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100)) * _minBufferSize / 2;
			double topThickness = TopThicknessCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100) * BufferHt / 100;
			double bottomThickness = BottomThicknessCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100) * BufferHt / 100;
			double leftThickness = LeftThicknessCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100) * BufferWi / 100;
			double rightThickness = RightThicknessCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100) * BufferWi / 100;

			HSV hsv = new HSV(0.5, 1.0, 1.0);

	//		int currentThickness = Convert.ToInt16(thickness * _minBufferSize);


			Color color = Color.GetColorAt((intervalPosFactor) / 100);
			hsv = HSV.FromRGB(color);
			hsv.V = hsv.V * level;
			for (int x = 0; x < BufferWi; x++)
			{
				for (int y = 0; y < BufferHt; y++)
				{
					CalculatePixel(x, y, frameBuffer, hsv, Convert.ToInt16(thickness), Convert.ToInt16(topThickness), Convert.ToInt16(bottomThickness), Convert.ToInt16(leftThickness), Convert.ToInt16(rightThickness));
				}
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{

			_minBufferSize = (double)(Math.Min(BufferHt, BufferWi)) / 100;
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);

			for (int effectFrame = 0; effectFrame < numFrames; effectFrame++)
			{
				frameBuffer.CurrentFrame = effectFrame;

				var intervalPos = GetEffectTimeIntervalPosition(effectFrame);
				var intervalPosFactor = intervalPos * 100;
				double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100) / 100;
				double thickness = (ThicknessCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100)) * _minBufferSize / 2;
				double topThickness = TopThicknessCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100) * BufferHt / 100;
				double bottomThickness = BottomThicknessCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100) * BufferHt / 100;
				double leftThickness = LeftThicknessCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100) * BufferWi / 100;
				double rightThickness = RightThicknessCurve.GetValue(GetEffectTimeIntervalPosition(effectFrame) * 100) * BufferWi / 100;
				if (effectFrame == 0)
				{
					_minBufferSize = (double)(Math.Min(BufferHt, BufferWi)) / 200;
				}

		//		int currentThickness = Convert.ToInt16(thickness * _minBufferSize);
				HSV hsv = new HSV(0.5, 1.0, 1.0);
				Color color = Color.GetColorAt((intervalPosFactor) / 100);
				hsv = HSV.FromRGB(color);
				hsv.V = hsv.V * level;

				foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
				{
					foreach (var elementLocation in elementLocations)
					{
							CalculatePixel(elementLocation.X, elementLocation.Y, frameBuffer, hsv, Convert.ToInt16(thickness), Convert.ToInt16(topThickness), Convert.ToInt16(bottomThickness), Convert.ToInt16(leftThickness), Convert.ToInt16(rightThickness));
					}
				}
			}

		}


		private void CalculatePixel(int x, int y, IPixelFrameBuffer frameBuffer, HSV hsv, int currentThickness, int topThickness, int bottomThickness, int leftThickness, int rightThickness)
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

			if (BorderType == BorderType.Single)
			{
				if (y <= currentThickness || y > BufferHt - currentThickness - 2 || x <= currentThickness ||
				    x > BufferWi - currentThickness - 2)
				{
					frameBuffer.SetPixel(xCoord, yCoord, hsv);
				}
			}
			else
			{
				if (y <= bottomThickness || y > BufferHt - topThickness - 2 || x <= leftThickness ||
				    x > BufferWi - rightThickness - 2)
				{
					frameBuffer.SetPixel(xCoord, yCoord, hsv);
				}
			}
		}
	}
}
