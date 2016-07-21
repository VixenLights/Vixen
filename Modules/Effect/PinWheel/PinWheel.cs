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
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.PinWheel
{
	public class PinWheel : PixelEffectBase
	{
		private PinWheelData _data;
		private static Random _random = new Random();
		private List<ColorGradient> _newColors = new List<ColorGradient>();

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
		[NumberRange(1, 100, 1)]
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
		[ProviderDescription(@"Rotation")]
		[PropertyOrder(8)]
		public bool Rotation
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
			//Nothing to setup
		}

		protected override void CleanUpRender()
		{
			//Nothing to clean up
		}

		private HSV hsv;
		private int xc_adj, yc_adj;

		protected override void RenderEffect(int frame, ref PixelFrameBuffer frameBuffer)
		{
			if (frame == 0)
			{
				HSV temphsv = new HSV();
				_newColors = new List<ColorGradient>();
				for (int newColr = 0; newColr < Arms; newColr++)
				{
					temphsv.H = (float)(rand() % 1000) / 1000.0f;
					temphsv.S = 1.0f;
					temphsv.V = 1.0f;
					_newColors.Add(new ColorGradient(temphsv.ToRGB()));
				}
			}
			int a, xc, ColorIdx, base_degrees;
			float t, tmax;
			int colorcnt = Colors.Count;

			xc = (int) (Math.Max(BufferWi, BufferHt)/2);

			double pos = (GetEffectTimeIntervalPosition(frame) * Speed * 360);

			int degrees_per_arm = 1;
			if (Arms > 0) degrees_per_arm = 360/Arms;
			float armsize = (float) (Size/100.0);
			for (a = 1; a <= Arms; a++)
			{
				ColorIdx = a%colorcnt;
				switch (ColorType)
				{
					case PinWheelColorType.Rainbow: //No user colors are used for Rainbow effect.
						hsv.H = (float)(rand() % 1000) / 1000.0f;
						hsv.S = 1.0f;
						hsv.V = 1.0f;
						break;
					case PinWheelColorType.Random:
						hsv = HSV.FromRGB(_newColors[ColorIdx].GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100));
						break;
					case PinWheelColorType.Standard:
						hsv = HSV.FromRGB(Colors[ColorIdx].ColorGradient.GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100));
						break;
				}

				base_degrees = Rotation ? (int) ((a + 1)*degrees_per_arm + pos) : (int) ((a + 1)*degrees_per_arm - pos);
				hsv.V = hsv.V * Colors[ColorIdx].Curve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
				hsv.V = hsv.V * LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
				Draw_arm(frameBuffer, base_degrees, xc * armsize, Twist, hsv, XOffset, YOffset, frame, ColorIdx);
				
				//Adjusts arm thinkness
				tmax = (float)((Thickness / 100.0) * degrees_per_arm / 2.0);
				for (t = 1; t <= tmax; t++)
				{
					if (PinWheel3D)
					{
						hsv.V = hsv.V*((tmax - t)/tmax);
					}
					Draw_arm(frameBuffer, base_degrees - t, xc*armsize, Twist, hsv, XOffset, YOffset, frame, ColorIdx);
					Draw_arm(frameBuffer, base_degrees + t, xc*armsize, Twist, hsv, XOffset, YOffset, frame, ColorIdx);
				}
			}
		}

		private void Draw_arm(PixelFrameBuffer frameBuffer, float base_degrees, float max_radius, int twist, HSV hsv, int XOffset, int YOffset, int frame, int ColorIdx)
		{
			double r;
			double pi180 = (Math.PI/180);
			int xc = BufferWi/2;
			int yc = BufferHt/2;
			xc = (int)(xc + (XOffset / 100.0) * xc); // XOffset is from -100 to 100
			yc = (int)(yc + (YOffset / 100.0) * yc);

			for (r = 0.0; r <= max_radius; r += 0.5)
			{
				int degrees_twist = (int) ((r/max_radius)*twist);
				int degrees = (int)(base_degrees + degrees_twist);
				double phi = degrees*pi180;
				int x = (int) (r*Math.Cos(phi) + xc);
				int y = (int) (r*Math.Sin(phi) + yc);
				switch (ColorType)
				{
					case PinWheelColorType.Gradient: //Applies gradient over each arm
						hsv = HSV.FromRGB(max_radius > (double)(Math.Max(BufferHt, BufferWi)) / 2 ? Colors[ColorIdx].ColorGradient.GetColorAt((100 / ((double)(Math.Max(BufferHt, BufferWi)) / 2) * r) / 100) : Colors[ColorIdx].ColorGradient.GetColorAt((100 / max_radius * r) / 100));
						hsv.V = hsv.V * Colors[ColorIdx].Curve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
						hsv.V = hsv.V * LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
						break;
				}
				if (r > CenterStart)
					frameBuffer.SetPixel(x, y, hsv);
			}
		}

		private int rand()
		{
			return _random.Next();
		}
	}
}
