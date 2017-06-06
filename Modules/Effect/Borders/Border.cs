using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.Direct2D.Interop;
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
		[ProviderDisplayName(@"BorderShape")]
		[ProviderDescription(@"BorderShape")]
		[PropertyOrder(1)]
		public BorderShape BorderShape
		{
			get { return _data.BorderShape; }
			set
			{
				_data.BorderShape = value;
	//			UpdateBorderShapeAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BorderWidth")]
		[ProviderDescription(@"BorderWidth")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 50, 1)]
		[PropertyOrder(2)]
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BorderType")]
		[ProviderDescription(@"BorderType")]
		[PropertyOrder(3)]
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Border")]
		[ProviderDescription(@"Border")]
		[PropertyOrder(4)]
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
		[ProviderDisplayName(@"TopBorder")]
		[ProviderDescription(@"TopBorder")]
		[PropertyOrder(5)]
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
		[ProviderDisplayName(@"BottomBorder")]
		[ProviderDescription(@"BottomBorder")]
		[PropertyOrder(6)]
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
		[ProviderDisplayName(@"LeftBorder")]
		[ProviderDescription(@"LeftBorder")]
		[PropertyOrder(7)]
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
		[ProviderDisplayName(@"RightBorder")]
		[ProviderDescription(@"RightBorder")]
		[PropertyOrder(8)]
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"OuterBorderStarts")]
		[ProviderDescription(@"OuterBorderStarts")]
		[PropertyOrder(9)]
		public Curve OuterBorderStartsCurve
		{
			get { return _data.OuterBorderStartsCurve; }
			set
			{
				_data.OuterBorderStartsCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 2)]
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
			UpdateBorderControlAttribute(false);
			UpdateStringOrientationAttributes();
		//	UpdateBorderShapeAttribute(false);
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
					{"OuterBorderStartsCurve", false},
					{"BorderType", false}
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
					{"OuterBorderStartsCurve", true},
					{"BorderType", true}
				};
				SetBrowsable(propertyStates);
			}

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateBorderShapeAttribute(bool refresh = true)
		{
			if (BorderShape == BorderShape.Rectangle)
			{
				Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
				{
					
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
			_minBufferSize = (double)(Math.Min(BufferHt, BufferWi)) / 100;
		}

		protected override void CleanUpRender()
		{
			//Not required
		}

		protected override void RenderEffect(int effectFrame, IPixelFrameBuffer frameBuffer)
		{
			var intervalPosFactor = GetEffectTimeIntervalPosition(effectFrame) * 100;

			double level = LevelCurve.GetValue(intervalPosFactor) / 100;
			int thickness = Convert.ToInt16(ThicknessCurve.GetValue(intervalPosFactor) * _minBufferSize / 2);
			int topThickness = Convert.ToInt16(TopThicknessCurve.GetValue(intervalPosFactor) * BufferHt / 100);
			int bottomThickness = Convert.ToInt16(BottomThicknessCurve.GetValue(intervalPosFactor) * BufferHt / 100);
			int leftThickness = Convert.ToInt16(LeftThicknessCurve.GetValue(intervalPosFactor) * BufferWi / 100);
			int rightThickness = Convert.ToInt16(RightThicknessCurve.GetValue(intervalPosFactor) * BufferWi / 100);
			int borderWidth = Convert.ToInt16(OuterBorderStartsCurve.GetValue(intervalPosFactor) * _minBufferSize / 2);

			if (BorderMode == BorderMode.Simple)
			{
				thickness = SimpleBorderWidth;
				borderWidth = 0;
			}
			else if (BorderType == BorderType.Single)
			{
				rightThickness = thickness;
				topThickness = thickness;
				leftThickness = thickness;
				bottomThickness = thickness;
			}

			if (BorderShape == BorderShape.Ellipse)
			{
				FillEllipse(rightThickness, topThickness, leftThickness, bottomThickness, frameBuffer, effectFrame, borderWidth);
				return;
			}

			for (int x = 0; x < BufferWi; x++)
			{
				for (int y = 0; y < BufferHt; y++)
				{
					CalculatePixel(x, y, frameBuffer, thickness, topThickness,
						bottomThickness, leftThickness, rightThickness,
						intervalPosFactor, level, effectFrame, borderWidth);
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

				var intervalPosFactor = GetEffectTimeIntervalPosition(effectFrame) * 100;
				double level = LevelCurve.GetValue(intervalPosFactor) / 100;
				int thickness = Convert.ToInt16(ThicknessCurve.GetValue(intervalPosFactor) * _minBufferSize / 2);
				int topThickness = Convert.ToInt16(TopThicknessCurve.GetValue(intervalPosFactor) * BufferHt / 100);
				int bottomThickness = Convert.ToInt16(BottomThicknessCurve.GetValue(intervalPosFactor) * BufferHt / 100);
				int leftThickness = Convert.ToInt16(LeftThicknessCurve.GetValue(intervalPosFactor) * BufferWi / 100);
				int rightThickness = Convert.ToInt16(RightThicknessCurve.GetValue(intervalPosFactor) * BufferWi / 100);
				int borderWidth = Convert.ToInt16(OuterBorderStartsCurve.GetValue(intervalPosFactor) * _minBufferSize / 2);

				if (BorderMode == BorderMode.Simple)
				{
					thickness = SimpleBorderWidth;
					borderWidth = 0;
				}

				foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
				{
					foreach (var elementLocation in elementLocations)
					{
						CalculatePixel(elementLocation.X, elementLocation.Y, frameBuffer, thickness,
							topThickness, bottomThickness, leftThickness, rightThickness,
							intervalPosFactor, level, effectFrame, borderWidth);
					}
				}
			}
		}

		private void CalculatePixel(int x, int y, IPixelFrameBuffer frameBuffer, int thickness, int topThickness, int bottomThickness, int leftThickness, int rightThickness, double intervalPosFactor, double level, int effectFrame, double borderWidth)
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

			Color color = GraidentColorSelection(x, y, effectFrame);

			HSV hsv = HSV.FromRGB(color);
			hsv.V = hsv.V * level;

			if (BorderType == BorderType.Single || BorderMode == BorderMode.Simple)//Single Border Control
			{
				//Displays borders 
				if ((y <= thickness || y >= BufferHt - thickness || x <= thickness || x >= BufferWi - thickness)
					&& x > borderWidth && y < BufferHt - borderWidth && y > borderWidth && x < BufferWi - borderWidth)//&& x > thickness - borderWidth && y < BufferHt - thickness - 1 + borderWidth && y > thickness - borderWidth && x < BufferWi - thickness - 1 + borderWidth)
				{
					frameBuffer.SetPixel(xCoord, yCoord, hsv);
				}
			}
			else
			{
				//Displays Independent Borders
				if ((y <= bottomThickness || y >= BufferHt - topThickness || x <= leftThickness || x >= BufferWi - rightThickness)
					&& x > borderWidth && y < BufferHt - borderWidth && y > borderWidth && x < BufferWi - borderWidth)
				{
					frameBuffer.SetPixel(xCoord, yCoord, hsv);
				}
			}
		}
		private void FillEllipse(int rightThickness, int topThickness, int leftThickness, int bottomThickness, IPixelFrameBuffer frameBuffer, int effectFrame, int borderWidth)
		{
			int right = SimpleBorderWidth;
			int left = BufferHt - SimpleBorderWidth - 1;
			int top = SimpleBorderWidth;
			int bottom = BufferWi - SimpleBorderWidth - 1;

			if (BorderMode == BorderMode.Advanced)
			{
				right = bottomThickness;
				left = BufferHt - topThickness - 1;
				top = leftThickness;
				bottom = BufferWi - rightThickness - 1;
			}

			int a, b, x, y, temp;
			int old_y;
			int d1, d2;
			int a2, b2, a2b2, a2sqr, b2sqr, a4sqr, b4sqr;
			int a8sqr, b8sqr, a4sqr_b4sqr;
			int fn, fnw, fw;
			int fnn, fnnw, fnwn, fnwnw, fnww, fww, fwnw;

			if (right < left)
			{
				temp = left;
				left = right;
				right = temp;
			}
			if (bottom < top)
			{
				temp = top;
				top = bottom;
				bottom = temp;
			}

			a = (right - left) / 2;
			b = (bottom - top) / 2;

			x = 0;
			y = b;

			a2 = a * a;
			b2 = b * b;
			a2b2 = a2 + b2;
			a2sqr = a2 + a2;
			b2sqr = b2 + b2;
			a4sqr = a2sqr + a2sqr;
			b4sqr = b2sqr + b2sqr;
			a8sqr = a4sqr + a4sqr;
			b8sqr = b4sqr + b4sqr;
			a4sqr_b4sqr = a4sqr + b4sqr;

			fn = a8sqr + a4sqr;
			fnn = a8sqr;
			fnnw = a8sqr;
			fnw = a8sqr + a4sqr - b8sqr * a + b8sqr;
			fnwn = a8sqr;
			fnwnw = a8sqr + b8sqr;
			fnww = b8sqr;
			fwnw = b8sqr;
			fww = b8sqr;
			d1 = b2 - b4sqr * a + a4sqr;

			Color color = System.Drawing.Color.Black;
			HSV hsv = HSV.FromRGB(color);

			while ((fnw < a2b2) || (d1 < 0) || ((fnw - fn > b2) && (y > 0)))
			{
				DrawVLine(top + y, left + x, right - x, hsv, false, frameBuffer);
				// Replace with your own span filling function. The hard-coded numbers were color values for testing purposes and can be ignored.
				DrawVLine(bottom - y, left + x, right - x, hsv, false, frameBuffer);

				y--;
				if ((d1 < 0) || (fnw - fn > b2))
				{
					d1 += fn;
					fn += fnn;
					fnw += fnwn;
				}
				else
				{
					x++;
					d1 += fnw;
					fn += fnnw;
					fnw += fnwnw;
				}
			}

			fw = fnw - fn + b4sqr;
			d2 = d1 + (fw + fw - fn - fn + a4sqr_b4sqr + a8sqr) / 4;
			fnw += b4sqr - a4sqr;

			old_y = y + 1;

			
			while (x <= a)
			{
				
				if (y != old_y) // prevent overdraw
				{
					DrawVLine(top + y, left + x, right - x, hsv, false, frameBuffer);
					DrawVLine(bottom - y, left + x, right - x, hsv, false, frameBuffer);
				}

				old_y = y;
				x++;
				if (d2 < 0)
				{
					y--;
					d2 += fnw;
					fw += fwnw;
					fnw += fnwnw;
				}
				else
				{
					d2 += fw;
					fw += fww;
					fnw += fnww;
				}
			}

			//Inverses the generated Ellipse and applies the correct Gradient Color
			for (int i = 0 + borderWidth; i < BufferWi - borderWidth; i++)
			{
				for (int j = 0 + borderWidth; j < BufferHt - borderWidth; j++)
				{
					if (frameBuffer.GetColorAt(i, j) == System.Drawing.Color.Transparent)
					{
						color = GraidentColorSelection(i, j, effectFrame);
						frameBuffer.SetPixel(i, j, color);
					}
				}
			}
		}

		private void DrawVLine(int x, int ystart, int yend, HSV hsv, bool wrap, IPixelFrameBuffer framBuffer)
		{
			if (ystart > yend)
			{
				int i = ystart;
				ystart = yend;
				yend = i;
			}
			for (int y = ystart; y <= yend; y++)
			{
				if (wrap)
				{
					while (x < 0)
					{
						x += BufferWi;
					}
					while (y < 0)
					{
						y += BufferHt;
					}
					while (x > BufferWi)
					{
						x -= BufferWi;
					}
					while (y > BufferHt)
					{
						y -= BufferHt;
					}
				}
				if (x < BufferWi && y < BufferHt && x >= 0 && y >= 0)
				{
					framBuffer.SetPixel(x, y, hsv);
				}
			}
		}

		private Color GraidentColorSelection(int x, int y, int effectFrame)
		{
			Color color = new Color();
			switch (GradientMode)
			{
				case GradientMode.OverTime:
					color = Color.GetColorAt(GetEffectTimeIntervalPosition(effectFrame));
					break;
				case GradientMode.AcrossElement:
					color = Color.GetColorAt(((100 / (double)(BufferWi - 1)) * x) / 100);
					break;
				case GradientMode.VerticalAcrossElement:
					color = Color.GetColorAt(((100 / (double)(BufferHt - 1)) * (BufferHt - y)) / 100);
					break;
				case GradientMode.DiagonalTopBottomElement:
					color = Color.GetColorAt(((100 / (double)(BufferHt - 1) * (BufferHt - y)) + (100 / (double)(BufferWi - 1) * x)) / 200);
					break;
				case GradientMode.DiagonalBottomTopElement:
					color = Color.GetColorAt(((100 / (double)(BufferHt - 1) * y) + (100 / (double)(BufferWi - 1) * x)) / 200);
					break;
			}
			return color;
		}
	}
}
