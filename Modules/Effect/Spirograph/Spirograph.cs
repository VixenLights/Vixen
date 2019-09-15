using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using Vixen.Sys.State.Execution;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Spirograph
{
	public class Spirograph : PixelEffectBase
	{
		private SpirographData _data;

		public Spirograph()
		{
			_data = new SpirographData();
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
		[PropertyOrder(0)]
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
		[ProviderDisplayName(@"Outer Circle Radius")]
		[ProviderDescription(@"Outer Circle Radius")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 200, 1)]
		[PropertyOrder(1)]
		public int OCR
		{
			get { return _data.OCR; }
			set
			{
				if (ICR > value)
					value = ICR + 2;
				_data.OCR = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Inner Circle Radius")]
		[ProviderDescription(@"Inner Circle Radius")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 200, 1)]
		[PropertyOrder(2)]
		public int ICR
		{
			get { return _data.ICR; }
			set
			{
				if (OCR <= value)
					value = OCR - 2;
				_data.ICR = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Spirographs")]
		[ProviderDescription(@"Spirographs")]
		//[NumberRange(1, 300, 1)]
		[PropertyOrder(3)]
		public Curve SpirographRangeCurve
		{
			get { return _data.SpirographRangeCurve; }
			set
			{
				_data.SpirographRangeCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Distance")]
		[ProviderDescription(@"Distance")]
		//[NumberRange(1, 100, 1)]
		[PropertyOrder(4)]
		public Curve DistanceCurve
		{
			get { return _data.DistanceCurve; }
			set
			{
				_data.DistanceCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Color Range")]
		[ProviderDescription(@"Color Range")]
		//[NumberRange(1, 200, 1)]
		[PropertyOrder(5)]
		public Curve RangeCurve
		{
			get { return _data.RangeCurve; }
			set
			{
				_data.RangeCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Animate Distance")]
		[ProviderDescription(@"Animate Distance")]
		[PropertyOrder(6)]
		public bool Animate
		{
			get { return _data.Animate; }
			set
			{
				_data.Animate = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"Color Type")]
		[ProviderDescription(@"Color Type")]
		[PropertyOrder(0)]
		public ColorType Type
		{
			get { return _data.Type; }
			set
			{
				_data.Type = value;
				IsDirty = true;
				UpdateColorAttribute();
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

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/spirograph/"; }
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as SpirographData;
				UpdateAttributes();
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private void UpdateAttributes()
		{
			UpdateColorAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		private void UpdateColorAttribute(bool refresh = true)
		{
			bool colorType = Type != ColorType.Rainbow;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			propertyStates.Add("Colors", colorType);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
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
			double level = LevelCurve.GetValue(intervalPosFactor) / 100;

			double rangeAdjust = CalculateRange(intervalPosFactor);
			int i, x, y, xc, yc, ColorIdx;
			int mod1440, d_mod;
			double state = frame * CalculateSpeed(intervalPosFactor);
			float R, r, d, d_orig, t;
			double hyp, x2, y2;
			Color col; //   we will define an hsv color model.
			int colorcnt = Colors.Count;

			xc = (int) (BufferWi/2); // 20x100 flex strips with 2 fols per strip = 40x50
			yc = (int) (BufferHt/2);
			R = (float) (xc*(OCR / 100.0)); //  Radius of the large circle just fits in the width of model
			r = (float)(xc * (ICR / 100.0)); // start little circle at 1/4 of max width
			if (r > R) r = R;
			d = (float)(xc * (CalculateDistance(intervalPosFactor) / 100.0));

			//    A hypotrochoid is a roulette traced by a point attached to a circle of radius r rolling around the inside of a fixed circle of radius R, where the point is a distance d from the center of the interior circle.
			//The parametric equations for a hypotrochoid are:[citation needed]
			//
			//  more info: http://en.wikipedia.org/wiki/Hypotrochoid
			//
			try {
				mod1440 = Convert.ToInt32(state % 1440);
				d_orig = d;
				for (i = 1; i <= CalculateSpirographRange(intervalPosFactor) * 18; i++)
				{
					if (Animate) d = (int)(d_orig + state / 2) % 100; // should we modify the distance variable each pass through?
					t = (float) ((i + mod1440)*Math.PI/180);
					x = Convert.ToInt32((R - r)*Math.Cos(t) + d*Math.Cos(((R - r)/r)*t) + xc);
					y = Convert.ToInt32((R - r)*Math.Sin(t) + d*Math.Sin(((R - r)/r)*t) + yc);

					if (colorcnt > 0) d_mod = (int)Math.Round(rangeAdjust / colorcnt);
					else d_mod = 1;

					x2 = Math.Pow((x - xc), 2);
					y2 = Math.Pow((y - yc), 2);
					hyp = Math.Sqrt(x2 + y2) / rangeAdjust * 100.0;

					switch (Type)
					{
							case ColorType.Random:
								ColorIdx = Rand(0, colorcnt);  // Select random numbers from 0 up to number of colors the user has added.
							break;
							case ColorType.Rainbow:
								ColorIdx = 1;
							break;
							default:
								ColorIdx = (int)(hyp / d_mod);
							break;
					}
					
					if (ColorIdx >= colorcnt) ColorIdx = colorcnt - 1;

					if (Type != ColorType.Rainbow)
					{
						col = Colors[ColorIdx].GetColorAt((intervalPosFactor) / 100);
						if (level < 1)
						{
							HSV hsv = HSV.FromRGB(col);
							hsv.V = hsv.V*level;
							col = hsv.ToRGB();
						}
					}
					else
					{
						HSV hsv = new HSV((float)(Rand() % 1000) / 1000.0f, 1.0f, 1.0f * level);
						col = hsv.ToRGB();
					}
					frameBuffer.SetPixel(x, y, col);
				}
			}
			catch {
			}
		}
		private double CalculateSpeed(double intervalPos)
		{
			return ScaleCurveToValue(SpeedCurve.GetValue(intervalPos), 100, 0);
		}

		private double CalculateDistance(double intervalPos)
		{
			var value = ScaleCurveToValue(DistanceCurve.GetValue(intervalPos), 100, 1);
			if (value < 1) value = 1;

			return value;
		}

		private double CalculateRange(double intervalPos)
		{
			return ScaleCurveToValue(RangeCurve.GetValue(intervalPos), 200, 1);
		}

		private double CalculateSpirographRange(double intervalPos)
		{
			return ScaleCurveToValue(SpirographRangeCurve.GetValue(intervalPos), 300, 1);
		}
	}
}