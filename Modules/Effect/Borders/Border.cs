using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using Vixen.Sys.State.Execution;
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
		[ProviderDisplayName(@"BorderMode")]
		[ProviderDescription(@"BorderMode")]
		[PropertyOrder(0)]
		public BorderMode BorderMode
		{
			get { return _data.BorderMode; }
			set
			{
				_data.BorderMode = value;
				UpdateBorderControlAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

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
				UpdateBorderControlAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Border", 2)]
		[ProviderDisplayName(@"BorderWidth")]
		[ProviderDescription(@"BorderWidth")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(0)]
		public int SimpleBorderWidth
		{
			get { return _data.SimpleBorderWidth; }
			set
			{
				_data.SimpleBorderWidth = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Border", 2)]
		[ProviderDisplayName(@"BorderThickness")]
		[ProviderDescription(@"BorderThickness")]
		[PropertyOrder(1)]
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
		[ProviderCategory(@"Border", 2)]
		[ProviderDisplayName(@"TopBorderWidth")]
		[ProviderDescription(@"TopBorderWidth")]
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
		[ProviderCategory(@"Border", 2)]
		[ProviderDisplayName(@"BottomBorderWidth")]
		[ProviderDescription(@"BottomBorderWidth")]
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
		[ProviderCategory(@"Border", 2)]
		[ProviderDisplayName(@"LeftBorderWidth")]
		[ProviderDescription(@"LeftBorderWidth")]
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
		[ProviderCategory(@"Border", 2)]
		[ProviderDisplayName(@"RightBorderWidth")]
		[ProviderDescription(@"RightBorderWidth")]
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

		[Value]
		[ProviderCategory(@"Border", 2)]
		[ProviderDisplayName(@"BorderWidth")]
		[ProviderDescription(@"BorderWidth")]
		[PropertyOrder(6)]
		public Curve BorderSizeCurve
		{
			get { return _data.BorderSizeCurve; }
			set
			{
				_data.BorderSizeCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Border", 2)]
		[ProviderDisplayName(@"BorderHeight")]
		[ProviderDescription(@"BorderHeight")]
		[PropertyOrder(7)]
		public Curve BorderHeightCurve
		{
			get { return _data.BorderHeightCurve; }
			set
			{
				_data.BorderHeightCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		[Value]
		[ProviderCategory(@"Movement", 3)]
		[ProviderDisplayName(@"XOffset")]
		[ProviderDescription(@"XOffset")]
		[PropertyOrder(0)]
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
		[ProviderCategory(@"Movement", 3)]
		[ProviderDisplayName(@"YOffset")]
		[ProviderDescription(@"YOffset")]
		[PropertyOrder(1)]
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

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 4)]
		[ProviderDisplayName(@"GradientMode")]
		[ProviderDescription(@"GradientMode")]
		[PropertyOrder(1)]
		public GradientMode GradientMode
		{
			get { return _data.GradientMode; }
			set
			{
				_data.GradientMode = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 4)]
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
		[ProviderCategory(@"Brightness", 5)]
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

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/border/"; }
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
			UpdateBorderControlAttribute(false);
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private void UpdateBorderControlAttribute(bool refresh = true)
		{
			if (BorderMode == BorderMode.Simple)
			{
				Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(8)
				{
					{"SimpleBorderWidth", true},
					{"ThicknessCurve", false},
					{"TopThicknessCurve", false},
					{"BottomThicknessCurve", false},
					{"LeftThicknessCurve", false},
					{"RightThicknessCurve", false},
					{"BorderSizeCurve", false},
					{"BorderHeightCurve", false},
					{"BorderType", false},
					{"YOffsetCurve", false},
					{"XOffsetCurve", false}
				};
				SetBrowsable(propertyStates);
			}
			else
			{
				Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(8)
				{
					{"SimpleBorderWidth", false},
					{"ThicknessCurve", BorderType == BorderType.Single},
					{"TopThicknessCurve", BorderType != BorderType.Single},
					{"BottomThicknessCurve", BorderType != BorderType.Single},
					{"LeftThicknessCurve", BorderType != BorderType.Single},
					{"RightThicknessCurve", BorderType != BorderType.Single},
					{"BorderSizeCurve", true},
					{"BorderHeightCurve", true},
					{"BorderType", true},
					{"YOffsetCurve", true},
					{"XOffsetCurve", true}
				};
				SetBrowsable(propertyStates);
			}

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		protected override void SetupRender()
		{
			_minBufferSize = Math.Min(BufferHt, BufferWi);
		}

		protected override void CleanUpRender()
		{
			//Not required
		}

		protected override void RenderEffect(int effectFrame, IPixelFrameBuffer frameBuffer)
		{
			var intervalPos = GetEffectTimeIntervalPosition(effectFrame);
			var intervalPosFactor = intervalPos * 100;
			var bufferWi = BufferWi;
			var bufferHt = BufferHt;

			double level = LevelCurve.GetValue(intervalPosFactor) / 100;
			int thickness = (int)Math.Round(CalculateBorderThickness(intervalPosFactor) / 2);
			int topThickness = (int)Math.Round(TopThicknessCurve.GetValue(intervalPosFactor) * bufferHt / 100);
			int bottomThickness = (int)Math.Round(BottomThicknessCurve.GetValue(intervalPosFactor) * bufferHt / 100);
			int leftThickness = (int)Math.Round(LeftThicknessCurve.GetValue(intervalPosFactor) * bufferWi / 100);
			int rightThickness = (int)Math.Round(RightThicknessCurve.GetValue(intervalPosFactor) * bufferWi / 100);
			int borderHeight = (int)CalculateBorderHeight(intervalPosFactor) / 2;
			int borderWidth = (int)(CalculateBorderSize(intervalPosFactor) / 2);
			int xOffsetAdj = CalculateXOffset(intervalPosFactor) * (bufferWi - borderWidth) / 100;
			int yOffsetAdj = CalculateYOffset(intervalPosFactor) * (bufferHt - borderHeight) / 100;
			Color color = Color.GetColorAt(GetEffectTimeIntervalPosition(effectFrame));

			if (BorderMode == BorderMode.Simple)
			{
				thickness = SimpleBorderWidth;
				borderWidth = 0;
				borderHeight = 0;
			}
			else if (BorderType == BorderType.Single)
			{
				rightThickness = thickness;
				topThickness = thickness;
				leftThickness = thickness;
				bottomThickness = thickness;
			}

			for (int x = 0; x < bufferWi; x++)
			{
				for (int y = 0; y < bufferHt; y++)
				{
					CalculatePixel(x, y, frameBuffer, thickness, topThickness,
						bottomThickness, leftThickness, rightThickness, level, borderWidth, borderHeight, color, xOffsetAdj, yOffsetAdj, ref bufferHt, ref bufferWi);
				}
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var bufferWi = BufferWi;
			var bufferHt = BufferHt;

			for (int effectFrame = 0; effectFrame < numFrames; effectFrame++)
			{
				frameBuffer.CurrentFrame = effectFrame;

				var intervalPos = GetEffectTimeIntervalPosition(effectFrame);
				var intervalPosFactor = intervalPos * 100;

				double level = LevelCurve.GetValue(intervalPos);
				int thickness = (int)Math.Round(CalculateBorderThickness(intervalPosFactor) / 2);
				int topThickness = (int)(TopThicknessCurve.GetValue(intervalPosFactor) * bufferHt / 100);
				int bottomThickness = (int)(BottomThicknessCurve.GetValue(intervalPosFactor) * bufferHt / 100);
				int leftThickness = (int)(LeftThicknessCurve.GetValue(intervalPosFactor) * bufferWi / 100);
				int rightThickness = (int)(RightThicknessCurve.GetValue(intervalPosFactor) * bufferWi / 100);
				int borderHeight = (int)CalculateBorderHeight(intervalPosFactor) / 2;
				int borderWidth = (int)(CalculateBorderSize(intervalPosFactor) / 2);
				int xOffsetAdj = CalculateXOffset(intervalPosFactor) * (bufferWi - borderWidth) / 100;
				int yOffsetAdj = CalculateYOffset(intervalPosFactor) * (bufferHt - borderHeight) / 100;
				Color color = Color.GetColorAt(GetEffectTimeIntervalPosition(effectFrame));

				if (BorderMode == BorderMode.Simple)
				{
					thickness = SimpleBorderWidth;
					borderWidth = 0;
					borderHeight = 0;
					xOffsetAdj = 0;
					yOffsetAdj = 0;
				}
				else if (BorderType == BorderType.Single)
				{
					rightThickness = thickness;
					topThickness = thickness;
					leftThickness = thickness;
					bottomThickness = thickness;
				}

				foreach (var elementLocation in frameBuffer.ElementLocations)
				{
					CalculatePixel(elementLocation.X, elementLocation.Y, frameBuffer, thickness,
						topThickness, bottomThickness, leftThickness, rightThickness, level, borderWidth, borderHeight,
						color, xOffsetAdj, yOffsetAdj, ref bufferHt, ref bufferWi);
				}
			}
		}

		private void CalculatePixel(int x, int y, IPixelFrameBuffer frameBuffer, int thickness, int topThickness, int bottomThickness, int leftThickness, int rightThickness, double level, int borderWidth, int borderHeight, Color color, int xOffsetAdj, int yOffsetAdj, ref int bufferHt, ref int bufferWi)
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

			x -= xOffsetAdj;
			y -= yOffsetAdj;

			if (BorderType == BorderType.Single || BorderMode == BorderMode.Simple)//Single Border Control
			{
				//Displays borders 
				if ((y < borderHeight + thickness || y >= bufferHt - borderHeight - thickness || x < borderWidth + thickness || x >= bufferWi - borderWidth - thickness)
					&& x >= borderWidth && y < bufferHt - borderHeight && y >= borderHeight && x < bufferWi - borderWidth)
				{
					color = GetColor(x, y, color, level, bufferHt, bufferWi);
					frameBuffer.SetPixel(xCoord, yCoord, color);
				}
			}
			else
			{
				//Displays Independent Borders
				if ((y < borderHeight + bottomThickness || y >= bufferHt - borderHeight - topThickness || x < borderWidth + leftThickness || x >= bufferWi - borderWidth - rightThickness)
					&& x >= borderWidth && y < bufferHt - borderHeight && y >= borderHeight && x < bufferWi - borderWidth)
				{
					color = GetColor(x, y, color, level, bufferHt, bufferWi);
					frameBuffer.SetPixel(xCoord, yCoord, color);
				}
			}
		}
		
		private Color GetColor(int x, int y, Color color, double level, int bufferHt, int bufferWi)
		{
			if (GradientMode != GradientMode.OverTime)
			{
				switch (GradientMode)
				{
					case GradientMode.AcrossElement:
						color = Color.GetColorAt(100 / (double)bufferWi * x / 100);
						break;
					case GradientMode.VerticalAcrossElement:
						color = Color.GetColorAt(100 / (double)bufferHt * (bufferHt - y) / 100);
						break;
					case GradientMode.DiagonalTopBottomElement:
						color = Color.GetColorAt(
							(100 / (double)bufferHt * (bufferHt - y) + 100 / (double)bufferWi * x) / 200);
						break;
					case GradientMode.DiagonalBottomTopElement:
						color = Color.GetColorAt((100 / (double)bufferHt * y + 100 / (double)bufferWi * x) / 200);
						break;
				}
			}
			if (level < 1)
			{
				HSV hsv = HSV.FromRGB(color);
				hsv.V *= level;
				color = hsv.ToRGB();
			}
			return color;
		}

		private double CalculateBorderSize(double intervalPosFactor)
		{
			return ScaleCurveToValue(BorderSizeCurve.GetValue(intervalPosFactor), 0, BufferWi);
		}

		private double CalculateBorderThickness(double intervalPosFactor)
		{
			return ScaleCurveToValue(ThicknessCurve.GetValue(intervalPosFactor), _minBufferSize, 2);
		}

		private double CalculateBorderHeight(double intervalPosFactor)
		{
			return ScaleCurveToValue(BorderHeightCurve.GetValue(intervalPosFactor), 0, BufferHt);
		}

		private int CalculateXOffset(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), 100, -100));
		}

		private int CalculateYOffset(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), 100, -100));
		}
	}
}
