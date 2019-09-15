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

namespace VixenModules.Effect.Plasma
{
	public class Plasma : PixelEffectBase
	{
		private PlasmaData _data;
		private const double Pi = Math.PI;

		public Plasma()
		{
			_data = new PlasmaData();
			EnableTargetPositioning(true, true);
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
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		//[NumberRange(1, 100, 1)]
		[PropertyOrder(1)]
		public Curve SpeedCurve
		{
			get { return _data.SpeedCurve; }
			set
			{
				_data.SpeedCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Plasma Style")]
		[ProviderDescription(@"Plasma Style")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 10, 1)]
		[PropertyOrder(2)]
		public int PlasmaStyle
		{
			get { return _data.PlasmaStyle; }
			set
			{
				_data.PlasmaStyle = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Line Density")]
		[ProviderDescription(@"Line Density")]
		//[NumberRange(1, 10, 1)]
		[PropertyOrder(2)]
		public Curve LineDensityCurve
		{
			get { return _data.LineDensityCurve; }
			set
			{
				_data.LineDensityCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color properties
		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"Color type")]
		[ProviderDescription(@"Color type")]
		[PropertyOrder(0)]
		public PlasmaColorType ColorType
		{
			get { return _data.ColorType; }
			set
			{
				_data.ColorType = value;
				UpdateColorAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

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
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
		}

		//Used to hide Colors from user when Preset color types are used.
		private void UpdateColorAttribute(bool refresh = true)
		{
			bool colorType = ColorType != PlasmaColorType.Normal;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			propertyStates.Add("Colors", !colorType);
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
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/plasma/"; }
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as PlasmaData;
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
			//Nothing to clean up
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			var intervalPos = GetEffectTimeIntervalPosition(frame);
			var intervalPosFactor = intervalPos * 100;
			var speed = CalculateSpeed(intervalPosFactor);
			double position = intervalPosFactor * speed;
			double plasmaSpeed = (101 - speed) * 3;
			var time = (position + 1.0)/plasmaSpeed;
			double level = LevelCurve.GetValue(intervalPosFactor) / 100;
			double lineDensity = CalculateLineDensity(intervalPosFactor) * Pi;

			for (int x = 0; x < BufferWi; x++)
			{
				for (int y = 0; y < BufferHt; y++)
				{
					CalculatePixel(x, y, time, frame, level, frameBuffer, lineDensity);
				}
			}
		}
		
		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
			for (int frame = 0; frame < numFrames; frame++)
			{
				var intervalPos = GetEffectTimeIntervalPosition(frame);
				var intervalPosFactor = intervalPos * 100;
				var speed = CalculateSpeed(intervalPosFactor);
				frameBuffer.CurrentFrame = frame;
				double position = intervalPosFactor * speed;
				double plasmaSpeed = (101 - speed) * 3;
				var time = (position + 1.0) / plasmaSpeed;
				double level = LevelCurve.GetValue(intervalPosFactor) / 100;
				double lineDensity = CalculateLineDensity(intervalPosFactor) * Pi;

				foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
				{
					foreach (var elementLocation in elementLocations)
					{
						CalculatePixel(elementLocation.X, elementLocation.Y, time, frame, level, frameBuffer, lineDensity);
					}
				}
			}
		}

		private void CalculatePixel(int x, int y, double time, int frame, double level, IPixelFrameBuffer frameBuffer, double lineDensity)
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
			double rx = ((float) x/(BufferWi));
			double ry = ((float) y/(BufferHt));
			
			// 1st equation
			var v = Math.Sin(rx*10 + time);

			//  second equation
			v += Math.Sin(10*(rx*Math.Sin(time/2) + ry*Math.Cos(time/3)) + time);

			//  third equation
			var cx = rx + .5*Math.Sin(time/5);
			var cy = ry + .5*Math.Cos(time/3);
			v += Math.Sin(Math.Sqrt((PlasmaStyle*50)*((cx*cx) + (cy*cy)) + time));

			v += Math.Sin(rx + time);
			v += Math.Sin((ry + time)/2.0);
			v += Math.Sin((rx + ry + time)/2.0);

			v += Math.Sin(Math.Sqrt(rx*rx + ry*ry) + time);
			v = v/2.0;

			Color color = Color.Transparent;
			switch (ColorType)
			{
				case PlasmaColorType.Normal:

					var h = Math.Sin(v * lineDensity + 2 * Pi / 3) + 1 * 0.5;
					color = GetMultiColorBlend(h/2, frame);
					break;
				case PlasmaColorType.Preset1:
					color = Color.FromArgb((int)((Math.Sin(v * lineDensity) + 1) * 128), (int)((Math.Cos(v * lineDensity) + 1) * 128),
						0);
					break;
				case PlasmaColorType.Preset2:
					color = Color.FromArgb(1, (int)((Math.Cos(v * lineDensity) + 1) * 128),
						(int)((Math.Sin(v * lineDensity) + 1) * 128));
					break;

				case PlasmaColorType.Preset3:
					color = Color.FromArgb((int)((Math.Sin(v * lineDensity) + 1) * 128),
						(int)((Math.Sin(v * lineDensity + 2 * Pi / 3) + 1) * 128), (int)((Math.Sin(v * lineDensity + 4 * Pi / 3) + 1) * 128));
					break;
				case PlasmaColorType.Preset4:
					color = Color.FromArgb((int)((Math.Sin(v * lineDensity) + 1) * 128), (int)((Math.Sin(v * lineDensity) + 1) * 128),
						(int)((Math.Sin(v * lineDensity) + 1) * 128));
					break;
			}
			if (level < 1)
			{
				HSV hsv = HSV.FromRGB(color);
				hsv.V = hsv.V * level;
				color = hsv.ToRGB();
			}
			frameBuffer.SetPixel(xCoord, yCoord, color);

		}

		private double CalculateSpeed(double intervalPos)
		{
			return ScaleCurveToValue(SpeedCurve.GetValue(intervalPos), 100, 0);
		}

		private double CalculateLineDensity(double intervalPos)
		{
			var value = ScaleCurveToValue(LineDensityCurve.GetValue(intervalPos), 10, 1);
			if (value < 1) value = 1;

			return value;
		}

		public Color GetMultiColorBlend(double n, int frame)
		{
			int colorcnt = Colors.Count();
			if (colorcnt <= 1)
			{
				return Colors[0].GetColorAt(GetEffectTimeIntervalPosition(frame));
			}
			if (n >= 1.0) n = 0.99999;
			if (n < 0.0) n = 0.0;
			int coloridx1 = (int)Math.Floor(n * colorcnt);
			int coloridx2 = (coloridx1 + 1)%colorcnt;
			double ratio = n * colorcnt - coloridx1;
			return Get2ColorBlend(coloridx1, coloridx2, ratio, frame);
		}

		public Color Get2ColorBlend(int coloridx1, int coloridx2, double ratio, int frame)
		{
			var pos = GetEffectTimeIntervalPosition(frame);
			var c1 = Colors[coloridx1].GetColorAt(pos);
			var c2 = Colors[coloridx2].GetColorAt(pos);

			return Color.FromArgb(ChannelBlend(c1.R, c2.R, ratio), ChannelBlend(c1.G, c2.G, ratio), ChannelBlend(c1.B, c2.B, ratio));
		}

		private int ChannelBlend(int c1, int c2, double ratio)
		{
			return c1 + (int) Math.Floor(ratio*(double) (c2 - c1) + 0.5);
		}
	}
}