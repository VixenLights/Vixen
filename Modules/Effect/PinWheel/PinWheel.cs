using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.PinWheel
{
	public class PinWheel : PixelEffectBase
	{
		private PinWheelData _data;
		private static readonly Random Random = new Random();
		private List<ColorGradient> _newColors = new List<ColorGradient>();
		private const double Pi180 = (Math.PI / 180);

		public PinWheel()
		{
			_data = new PinWheelData();
		}

		public override bool IsDirty
		{
			get
			{
				if (Colors.Any(x => !x.ColorGradient.CheckLibraryReference() || !x.Curve.CheckLibraryReference()))
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
		[ProviderDisplayName(@"Iterations")]
		[ProviderDescription(@"Iterations")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 50, 1)]
		[PropertyOrder(0)]
		public int Speed
		{
			get { return _data.Speed; }
			set
			{
				_data.Speed = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Arms")]
		[ProviderDescription(@"Arms")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(1)]
		public int Arms
		{
			get { return _data.Arms; }
			set
			{
				_data.Arms = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Thickness")]
		[ProviderDescription(@"Thickness")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(2)]
		public int Thickness
		{
			get { return _data.Thickness; }
			set
			{
				_data.Thickness = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Size")]
		[ProviderDescription(@"Size")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 400, 1)]
		[PropertyOrder(3)]
		public int Size
		{
			get { return _data.Size; }
			set
			{
				_data.Size = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Twist")]
		[ProviderDescription(@"Twist")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-500, 500, 1)]
		[PropertyOrder(4)]
		public int Twist
		{
			get { return _data.Twist; }
			set
			{
				_data.Twist = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"XOffset")]
		[ProviderDescription(@"XOffset")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-100, 100, 1)]
		[PropertyOrder(5)]
		public int XOffset
		{
			get { return _data.XOffset; }
			set
			{
				_data.XOffset = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"YOffset")]
		[ProviderDescription(@"YOffset")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-100, 100, 1)]
		[PropertyOrder(6)]
		public int YOffset
		{
			get { return _data.YOffset; }
			set
			{
				_data.YOffset = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Center Start")]
		[ProviderDescription(@"Center Start")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(7)]
		public int CenterStart
		{
			get { return _data.CenterStart; }
			set
			{
				_data.CenterStart = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Rotation")]
		[ProviderDescription(@"Direction")]
		[PropertyOrder(8)]
		public RotationType Rotation
		{
			get { return _data.Rotation; }
			set
			{
				_data.Rotation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"3D")]
		[ProviderDescription(@"3D")]
		[PropertyOrder(9)]
		public bool PinWheel3D
		{
			get { return _data.PinWheel3D; }
			set
			{
				_data.PinWheel3D = value;
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
		public PinWheelColorType ColorType
		{
			get { return _data.ColorType; }
			set
			{
				_data.ColorType = value;
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
		public List<GradientLevelPair> Colors
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
		[ProviderDisplayName(@"Overall Brightness")]
		[ProviderDescription(@"Overall Brightness")]
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
			TypeDescriptor.Refresh(this);
		}

		//Used to hide Colors from user when Rainbow type is selected and unhides for the other types.
		private void UpdateColorAttribute(bool refresh = true)
		{
			bool colorType = ColorType != PinWheelColorType.Rainbow && ColorType != PinWheelColorType.Random;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			propertyStates.Add("Colors", colorType);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}
		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as PinWheelData;
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
			if (ColorType == PinWheelColorType.Random)
			{
				HSV temphsv = new HSV();
				_newColors = new List<ColorGradient>();
				for (int newColr = 0; newColr < Arms; newColr++)
				{
					temphsv.H = Rand();
					temphsv.S = 1.0f;
					temphsv.V = 1.0f;
					_newColors.Add(new ColorGradient(temphsv.ToRGB()));
				}
			}
		}

		protected override void CleanUpRender()
		{
			_newColors = null;
		}

		private HSV _hsv;
		
		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			var overallLevel = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
			int colorcnt = Colors.Count;

			var xc = Math.Max(BufferWi, BufferHt)/2;

			double pos = (GetEffectTimeIntervalPosition(frame) * Speed * 360);

			int degreesPerArm = 1;
			if (Arms > 0) degreesPerArm = 360/Arms;
			float armsize = (float) (Size/100.0);
			for (int a = 1; a <= Arms; a++)
			{
				var colorIdx = a%colorcnt;
				switch (ColorType)
				{
					case PinWheelColorType.Rainbow: //No user colors are used for Rainbow effect.
						_hsv.H = Rand();
						_hsv.S = 1.0f;
						_hsv.V = 1.0f;
						break;
					case PinWheelColorType.Random:
						_hsv = HSV.FromRGB(_newColors[colorIdx].GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100));
						break;
					case PinWheelColorType.Standard:
						_hsv = HSV.FromRGB(Colors[colorIdx].ColorGradient.GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100));
						break;
				}

				var baseDegrees = Rotation==RotationType.Backward ? (int) ((a + 1)*degreesPerArm + pos) : (int) ((a + 1)*degreesPerArm - pos);
				_hsv.V = _hsv.V * Colors[colorIdx].Curve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
				_hsv.V = _hsv.V * overallLevel;
				Draw_arm(frameBuffer, baseDegrees, xc * armsize, Twist, _hsv, XOffset, YOffset, frame, colorIdx, overallLevel);
				
				//Adjusts arm thickness
				var tmax = (float)((Thickness / 100.0) * degreesPerArm / 2.0);
				float t;
				for (t = 1; t <= tmax; t++)
				{
					if (PinWheel3D)
					{
						_hsv.V = _hsv.V*((tmax - t)/tmax);
					}
					Draw_arm(frameBuffer, baseDegrees - t, xc*armsize, Twist, _hsv, XOffset, YOffset, frame, colorIdx, overallLevel);
					Draw_arm(frameBuffer, baseDegrees + t, xc*armsize, Twist, _hsv, XOffset, YOffset, frame, colorIdx, overallLevel);
				}
			}
		}

		private void Draw_arm(IPixelFrameBuffer frameBuffer, float baseDegrees, float maxRadius, int twist, HSV hsv, int xOffset, int yOffset, int frame, int colorIdx, double overallLevel)
		{
			int xc = BufferWi/2;
			int yc = BufferHt/2;
			xc = (int)(xc + (xOffset / 100.0) * xc); // XOffset is from -100 to 100
			yc = (int)(yc + (yOffset / 100.0) * yc);

			for (double r = 0.0; r <= maxRadius; r += 0.5)
			{
				int degreesTwist = (int) ((r/maxRadius)*twist);
				int degrees = (int)(baseDegrees + degreesTwist);
				double phi = degrees*Pi180;
				int x = (int) (r*Math.Cos(phi) + xc);
				int y = (int) (r*Math.Sin(phi) + yc);
				switch (ColorType)
				{
					case PinWheelColorType.Gradient: //Applies gradient over each arm
						hsv = HSV.FromRGB(maxRadius > (double)(Math.Max(BufferHt, BufferWi)) / 2 ? Colors[colorIdx].ColorGradient.GetColorAt((100 / ((double)(Math.Max(BufferHt, BufferWi)) / 2) * r) / 100) : Colors[colorIdx].ColorGradient.GetColorAt((100 / maxRadius * r) / 100));
						hsv.V = hsv.V * Colors[colorIdx].Curve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
						hsv.V = hsv.V * overallLevel;
						break;
				}
				if (r > CenterStart)
					frameBuffer.SetPixel(x, y, hsv);
			}
		}

		private static double Rand()
		{
			return Random.NextDouble();
		}
	}
}
