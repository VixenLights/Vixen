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
		private List<ColorGradient> _newColors = new List<ColorGradient>();
		
		public PinWheel()
		{
			_data = new PinWheelData();
			EnableTargetPositioning(true, true);
			UpdateAttributes();
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
		[ProviderDisplayName(@"MovementType")]
		[ProviderDescription(@"MovementType")]
		[PropertyOrder(0)]
		public MovementType MovementType
		{
			get { return _data.MovementType; }
			set
			{
				_data.MovementType = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Movement")]
		[ProviderDescription(@"Movement")]
		//[NumberRange(1, 50, 1)]
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
		[ProviderDisplayName(@"Arms")]
		[ProviderDescription(@"Arms")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(2)]
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
		//[NumberRange(1, 100, 1)] Keep for range reference
		[PropertyOrder(3)]
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
		[ProviderDisplayName(@"Size")]
		[ProviderDescription(@"Size")]
		//[NumberRange(1, 400, 1)]
		[PropertyOrder(4)]
		public Curve SizeCurve
		{
			get { return _data.SizeCurve; }
			set
			{
				_data.SizeCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Twist")]
		[ProviderDescription(@"Twist")]
		//[NumberRange(-500, 500, 1)] Keep for range reference
		[PropertyOrder(5)]
		public Curve TwistCurve
		{
			get { return _data.TwistCurve; }
			set
			{
				_data.TwistCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"OffsetPercentage")]
		[ProviderDescription(@"OffsetPercentage")]
		[PropertyOrder(6)]
		public bool OffsetPercentage
		{
			get { return _data.OffsetPercentage; }
			set
			{
				_data.OffsetPercentage = value;
				IsDirty = true;
				UpdateOffsetAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"XOffset")]
		[ProviderDescription(@"XOffset")]
		//[NumberRange(-100, 100, 1)]
		[PropertyOrder(7)]
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"YOffset")]
		[ProviderDescription(@"YOffset")]
		//[NumberRange(-100, 100, 1)]
		[PropertyOrder(8)]
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

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Center Hub")]
		[ProviderDescription(@"CenterHub")]
		//[NumberRange(0, 100, 1)]
		[PropertyOrder(9)]
		public Curve CenterHubCurve
		{
			get { return _data.CenterHubCurve; }
			set
			{
				_data.CenterHubCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Rotation")]
		[ProviderDescription(@"Direction")]
		[PropertyOrder(10)]
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
		[PropertyOrder(11)]
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
		[ProviderDescription(@"GradientLevelPair")]
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
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
		[NumberRange(0, 100, 1)]
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
			UpdateStringOrientationAttributes(true);
			UpdateOffsetAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		//Used to hide Colors from user when Rainbow type is selected and unhides for the other types.
		private void UpdateOffsetAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			propertyStates.Add("OffsetPercentage", !OffsetPercentage);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
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

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/pinwheel/"; }
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
					temphsv.H = RandDouble();
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
			int degreesPerArm = 1;
			if (Arms > 0) degreesPerArm = 360 / Arms;
			
			var intervalPos = GetEffectTimeIntervalPosition(frame);
			var intervalPosFactor = intervalPos * 100;
			double pos = MovementType == MovementType.Iterations
				? intervalPos * CalculateSpeed(intervalPosFactor) * 360
				: intervalPos*CalculateSpeed(intervalPosFactor)*GetNumberFrames();
			var overallLevel = LevelCurve.GetValue(intervalPosFactor) / 100;

			double currentTwist = CalculateTwist(intervalPosFactor);

			var arms = CreateArms(degreesPerArm, pos, frame, overallLevel);
			float armsize = (float)(CalculateSize(intervalPosFactor) / 100.0);

			var origin = new Point(BufferWi / 2 + BufferWiOffset + CalculateXOffset(intervalPosFactor), BufferHt / 2 + BufferHtOffset + CalculateYOffset(intervalPosFactor));
			
			var xc = OffsetPercentage
				? BufferHt
				: DistanceFromPoint(origin, new Point(BufferWiOffset + BufferWi, BufferHtOffset + BufferHt));

			var maxRadius = xc * armsize;

			var angleRange = CalculateThickness(intervalPosFactor) / 100.0f * degreesPerArm / 2.0f;

			var centerStartPct = CalculateCenterStartPct(intervalPosFactor);

			for (int x = 0; x < BufferWi; x++)
			{
				for (int y = 0; y < BufferHt; y++)
				{
					RenderPoint(frameBuffer, currentTwist,  x, y, origin, maxRadius, arms, angleRange, overallLevel, true, centerStartPct);
				}
			}
			
		}
		
		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			int degreesPerArm = 1;
			if (Arms > 0) degreesPerArm = 360 / Arms;
			
			for (int frame = 0; frame < numFrames; frame++)
			{
				frameBuffer.CurrentFrame = frame;
				var intervalPos = GetEffectTimeIntervalPosition(frame);

				var intervalPosFactor = intervalPos * 100;

				double pos = MovementType == MovementType.Iterations
					? intervalPos * CalculateSpeed(intervalPosFactor) * 360
					: intervalPos * CalculateSpeed(intervalPosFactor) * GetNumberFrames();
				var overallLevel = LevelCurve.GetValue(intervalPosFactor) / 100;

				double currentTwist = CalculateTwist(intervalPosFactor);

				var arms = CreateArms(degreesPerArm, pos, frame, overallLevel);

				float armsize = (float)(CalculateSize(intervalPosFactor) / 100.0);

				var origin = new Point(BufferWi / 2 + BufferWiOffset + CalculateXOffset(intervalPosFactor), BufferHt / 2 + BufferHtOffset + CalculateYOffset(intervalPosFactor));
				
				var xc = OffsetPercentage
					? BufferHt
					: DistanceFromPoint(origin, new Point(BufferWiOffset + BufferWi, BufferHtOffset + BufferHt));

				var maxRadius = xc * armsize;

				var angleRange = CalculateThickness(intervalPosFactor) / 100.0f * degreesPerArm / 2.0f;

				var centerStartPct = CalculateCenterStartPct(intervalPosFactor);

				foreach (var elementLocation in frameBuffer.ElementLocations)
				{
					RenderPoint(frameBuffer, currentTwist, elementLocation.X, elementLocation.Y, origin, maxRadius, arms, angleRange, overallLevel, false, centerStartPct);			
				}
			}			
		}

		private int CalculateXOffset(double intervalPos)
		{
			return OffsetPercentage
				? (int) Math.Round(ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), BufferWi, -BufferWi))
				: (int) Math.Round(ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), 100, -100));
		}

		private int CalculateYOffset(double intervalPos)
		{
			return OffsetPercentage
				? (int) Math.Round(ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), -BufferHt, BufferHt))
				: (int) Math.Round(ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), 100, -100));
		}

		private double CalculateSpeed(double intervalPos)
		{
			var value = ScaleCurveToValue(SpeedCurve.GetValue(intervalPos), 50, 1) * FrameTime / 50d;
			if (value < 1) value = 1;

			return value;
		}

		private double CalculateSize(double intervalPos)
		{
			var value = ScaleCurveToValue(SizeCurve.GetValue(intervalPos), 400, 1);
			if (value < 1) value = 1;

			return value;
		}

		private double CalculateTwist(double intervalPos)
		{
			return ScaleCurveToValue(TwistCurve.GetValue(intervalPos), 500, -500);
		}

		private double CalculateThickness(double intervalPos)
		{
			var value = ThicknessCurve.GetIntValue(intervalPos);
			if (value < 1) value = 1;

			return value;
		}

		private double CalculateCenterStartPct(double intervalPos)
		{
			var value = CenterHubCurve.GetValue(intervalPos);
			if (value < 1) value = 1;

			return value/100.0;
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

		private void RenderPoint(IPixelFrameBuffer frameBuffer, double twist, int x, int y, Point origin, double maxRadius, List<Tuple<int, HSV>> arms, double angleRange, double overallLevel, bool invertY, double centerStartPct)
		{
			var radius = DistanceFromPoint(origin, x, y);
			if (radius > maxRadius || radius <= (centerStartPct*maxRadius))
			{
				return;
			}
			var angle = GetAngleDegree(origin, x, y);

			double degreesTwist = radius / maxRadius * twist;

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
					hsv.H = RandDouble();
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
	}
}
