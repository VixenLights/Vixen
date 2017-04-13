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
			EnableTargetPositioning(true, true);
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
		[ProviderDisplayName(@"BladeType")]
		[ProviderDescription(@"BladeType")]
		[PropertyOrder(10)]
		public PinWheelBladeType PinWheelBladeType
		{
			get { return _data.PinWheelBladeType; }
			set
			{
				_data.PinWheelBladeType = value;
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

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{

			var origin = new Point(BufferWi / 2 + BufferWiOffset + XOffset, BufferHt / 2 + BufferHtOffset + YOffset);

			var xc = DistanceFromCenter(origin, new Point(BufferWiOffset + BufferWi, BufferHtOffset + BufferHt));

			int degreesPerArm = 1;
			if (Arms > 0) degreesPerArm = 360 / Arms;
			float armsize = (float)(Size / 100.0);
			
			double pos = (GetEffectTimeIntervalPosition(frame) * Speed * 360);
			var overallLevel = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;

			var arms = CreateArms(degreesPerArm, pos, frame, overallLevel);

			var maxRadius = xc * armsize;

			var angleRange = Thickness / 100.0f * degreesPerArm / 2.0f;

			for (int x = 0; x < BufferWi; x++)
			{
				for (int y = 0; y < BufferHt; y++)
				{
					RenderPoint(frameBuffer, x, y, origin, maxRadius, arms, angleRange, overallLevel, true);
				}
			}
			
		}
		
		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var origin = new Point(BufferWi/2+BufferWiOffset + XOffset, BufferHt / 2 + BufferHtOffset + YOffset);

			var xc = DistanceFromCenter(origin, new Point(BufferWiOffset + BufferWi, BufferHtOffset + BufferHt));

			int degreesPerArm = 1;
			if (Arms > 0) degreesPerArm = 360 / Arms;
			float armsize = (float)(Size / 100.0);

			for (int frame = 0; frame < numFrames; frame++)
			{
				frameBuffer.CurrentFrame = frame;
				double pos = (GetEffectTimeIntervalPosition(frame) * Speed * 360);
				var overallLevel = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;

				var arms = CreateArms(degreesPerArm, pos, frame, overallLevel);

				var maxRadius = xc * armsize;

				var angleRange = Thickness / 100.0f * degreesPerArm / 2.0f;

				foreach (var elementLocation in frameBuffer.ElementLocations)
				{
					RenderPoint(frameBuffer, elementLocation.X, elementLocation.Y, origin, maxRadius, arms, angleRange, overallLevel, false);			
				}
			}			
		}

		private List<Tuple<int, HSV>> CreateArms(int degreesPerArm, double pos, int frame, double overallLevel)
		{
			var arms = new List<Tuple<int, HSV>>();
			int colorcnt = Colors.Count;
			for (int a = 1; a <= Arms; a++)
			{
				var armAngle = (Rotation == RotationType.Backward
					? (int) (AddDegrees((a + 1) * degreesPerArm, pos))
					: (int) (AddDegrees((a + 1) * degreesPerArm, -pos)));
				var colorIdx = (a - 1) % colorcnt;
				var hsv = GetColor(colorIdx, frame);
				hsv.V = hsv.V * Colors[colorIdx].Curve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
				hsv.V = hsv.V * overallLevel;
				arms.Add(new Tuple<int, HSV>(armAngle, hsv));
			}
			return arms;
		}

		private void RenderPoint(IPixelFrameBuffer frameBuffer, int x, int y, Point origin, double maxRadius, List<Tuple<int, HSV>> arms, double angleRange, double overallLevel, bool invertY )
		{
			var radius = DistanceFromCenter(origin, x, y);
			if (radius > maxRadius || radius <= CenterStart)
			{
				return;
			}
			var angle = GetAngleDegree(origin, x, y);

			int degreesTwist = (int)(radius / maxRadius * Twist);

			for (int i = 0; i < arms.Count; i++)
			{
				double degrees = AddDegrees(arms[i].Item1, degreesTwist);

				if (IsAngleBetween(AddDegrees(degrees, -angleRange), AddDegrees(degrees, angleRange), angle))
				{
					HSV hsv;
					if (ColorType == PinWheelColorType.Gradient)
					{
						var colorIdx = i % Colors.Count;
						hsv = HSV.FromRGB(Colors[colorIdx].ColorGradient.GetColorAt(radius / maxRadius));
						hsv.V = hsv.V * Colors[colorIdx].Curve.GetValue(radius / maxRadius * 100) / 100;
						hsv.V = hsv.V * overallLevel;

					}
					else
					{
						hsv = arms[i].Item2;
					}

					switch (PinWheelBladeType)
					{
						case PinWheelBladeType.ThreeD:
							hsv.V = hsv.V * (1 - (DegreesDiffernce(degrees, angle) / angleRange));
							break;
						case PinWheelBladeType.Inverted3D:
							hsv.V = hsv.V * (DegreesDiffernce(degrees, angle) / angleRange);
							break;
						case PinWheelBladeType.Fan:
							var frontAngle = Rotation == RotationType.Forward ? angleRange : -angleRange;
							hsv.V = hsv.V * (DegreesDiffernce(AddDegrees(degrees, frontAngle), angle) / (2 * angleRange));
							break;
					}

					if (invertY)
					{
						frameBuffer.SetPixel(x, BufferHt - 1 - y, hsv);
					}
					else
					{
						frameBuffer.SetPixel(x, y, hsv);
					}
					
				}
			}
		}

		private HSV GetColor(int colorIdx, int frame)
		{
			HSV hsv = new HSV(0,0,0);
			switch (ColorType)
			{
				case PinWheelColorType.Rainbow: //No user colors are used for Rainbow effect.
					hsv.H = Rand();
					hsv.S = 1.0f;
					hsv.V = 1.0f;
					break;
				case PinWheelColorType.Random:
					hsv = HSV.FromRGB(_newColors[colorIdx].GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100));
					break;
				case PinWheelColorType.Standard:
					hsv = HSV.FromRGB(Colors[colorIdx].ColorGradient.GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100));
					break;
			}

			return hsv;
		}


		private static bool IsAngleBetween(double a, double b, double n)
		{
			n = (360 + (n % 360)) % 360;
			a = (3600000 + a) % 360;
			b = (3600000 + b) % 360;

			if (a < b)
			{
				return a <= n && n <= b;
			}
			return a <= n || n <= b;
		
		}

		private static double DegreesDiffernce(double angle1, double angle2)
		{
			return Math.Min(360 - Math.Abs(angle1 - angle2), Math.Abs(angle1 - angle2));
		}

		private static double GetAngleDegree(Point origin, int x, int y)
		{
			var n = 270 - (Math.Atan2(origin.Y - y, origin.X - x)) * 180 / Math.PI;
			return n % 360;
		}

		private static double DistanceFromCenter(Point origin, int x, int y)
		{
			return Math.Sqrt(Math.Pow((x - origin.X), 2) + Math.Pow((y - origin.Y), 2));
		}

		private static double DistanceFromCenter(Point origin, Point point)
		{
			return Math.Sqrt(Math.Pow((point.X - origin.X), 2) + Math.Pow((point.Y - origin.Y), 2));
		}

		private static double AddDegrees(double angle, double degrees)
		{
			var newAngle = (angle + degrees) % 360;
			if (newAngle < 0)
			{
				newAngle += 360;
			}

			return newAngle;
		}
		
		private static double Rand()
		{
			return Random.NextDouble();
		}
	}
}
