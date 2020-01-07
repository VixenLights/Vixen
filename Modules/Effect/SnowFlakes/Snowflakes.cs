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

namespace VixenModules.Effect.Snowflakes
{
	public class Snowflakes:PixelEffectBase
	{
		private SnowflakesData _data;

		private List<SnowFlakeClass> _snowFlakes;
		private int _increaseFlakeCount;
		private int _snowfalakeCountAdjust;
		private double _xSpeedAdjustment;
		private double _ySpeedAdjustment;
		private int _wobbleAdjustment;
		private int _maxBufferSize;
		private double Xposition;
		private double Yposition;
		private bool Offset;

		public Snowflakes()
		{
			_data = new SnowflakesData();
		}

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

		#region Config properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"FlakeType")]
		[ProviderDescription(@"FlakeType")]
		[PropertyOrder(0)]
		public SnowflakeType SnowflakeType
		{
			get { return _data.SnowflakeType; }
			set
			{
				_data.SnowflakeType = value;
				IsDirty = true;
				UpdateFlakeAttribute();
				OnPropertyChanged();
			}
		}

		//This is done so the user can exclude 45 Point Flakes from the Random selection as they would be too big on a small Matrix.
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Include45Pt")]
		[ProviderDescription(@"Include45Pt")]
		[PropertyOrder(1)]
		public bool PointFlake45
		{
			get { return _data.PointFlake45; }
			set
			{
				_data.PointFlake45 = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MovementType")]
		[ProviderDescription(@"MovementType")]
		[PropertyOrder(2)]
		public SnowflakeEffect SnowflakeEffect
		{
			get { return _data.SnowflakeEffect; }
			set
			{
				_data.SnowflakeEffect = value;
				IsDirty = true;
				if (SnowflakeEffect == SnowflakeEffect.Explode)
					SnowBuildUp = false;
				UpdateDirectionAttribute();
				UpdateFlakeBuildUpAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Fade")]
		[ProviderDescription(@"Fade")]
		[PropertyOrder(3)]
		public FadeType FadeType
		{
			get { return _data.FadeType; }
			set
			{
				_data.FadeType = value;
				IsDirty = true;
				UpdateFadeAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"BuildUpSpeed")]
		[ProviderDescription(@"BuildUpSpeed")]
		[PropertyOrder(4)]
		public Curve BuildUpSpeedCurve
		{
			get { return _data.BuildUpSpeedCurve; }
			set
			{
				_data.BuildUpSpeedCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"SnowBuildUp")]
		[ProviderDescription(@"SnowBuildUp")]
		[PropertyOrder(5)]
		public bool SnowBuildUp
		{
			get { return _data.SnowBuildUp; }
			set
			{
				_data.SnowBuildUp = value;
				IsDirty = true;
				UpdateFlakeBuildUpAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"InitialBuildUp")]
		[ProviderDescription(@"InitialBuildUp")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(6)]
		public int InitialBuildUp
		{
			get { return _data.InitialBuildUp; }
			set
			{
				_data.InitialBuildUp = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MinAngle")]
		[ProviderDescription(@"MinAngle")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 360, 1)]
		[PropertyOrder(7)]
		public int MinDirection
		{
			get { return _data.MinDirection; }
			set
			{
				_data.MinDirection = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MaxAngle")]
		[ProviderDescription(@"MaxAngle")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 360, 1)]
		[PropertyOrder(8)]
		public int MaxDirection
		{
			get { return _data.MaxDirection; }
			set
			{
				_data.MaxDirection = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"FadeSpeed")]
		[ProviderDescription(@"FadeSpeed")]
		[PropertyOrder(9)]
		public Curve FadeSpeed
		{
			get { return _data.FadeSpeed; }
			set
			{
				_data.FadeSpeed = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyOrder(10)]
		public Curve CenterSpeedCurve
		{
			get { return _data.CenterSpeedCurve; }
			set
			{
				_data.CenterSpeedCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"SpeedVariation")]
		[ProviderDescription(@"SpeedVariation")]
		[PropertyOrder(11)]
		public Curve SpeedVariationCurve
		{
			get { return _data.SpeedVariationCurve; }
			set
			{
				_data.SpeedVariationCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"FlakeCount")]
		[ProviderDescription(@"FlakeCount")]
		[PropertyOrder(12)]
		public Curve FlakeCountCurve
		{
			get { return _data.FlakeCountCurve; }
			set
			{
				_data.FlakeCountCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"HFlakeCount")]
		[ProviderDescription(@"HFlakeCount")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(2, 50, 1)]
		[PropertyOrder(13)]
		public int HFlakeCount
		{
			get { return _data.HFlakeCount; }
			set
			{
				_data.HFlakeCount = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"VFlakeCount")]
		[ProviderDescription(@"VFlakeCount")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(2, 50, 1)]
		[PropertyOrder(14)]
		public int VFlakeCount
		{
			get { return _data.VFlakeCount; }
			set
			{
				_data.VFlakeCount = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Movement")]
		[ProviderDescription(@"MovementMode")]
		[PropertyOrder(15)]
		public SnowFlakeMovement SnowFlakeMovement
		{
			get { return _data.SnowFlakeMovement; }
			set
			{
				_data.SnowFlakeMovement = value;
				IsDirty = true;
				UpdateDirectionAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"RandomIntensity")]
		[ProviderDescription(@"RandomIntensity")]
		[PropertyOrder(16)]
		public bool RandomBrightness
		{
			get { return _data.RandomBrightness; }
			set
			{
				_data.RandomBrightness = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Movement

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"Wobble")]
		[ProviderDescription(@"Wobble")]
		[PropertyOrder(0)]
		public Curve WobbleCurve
		{
			get { return _data.WobbleCurve; }
			set
			{
				_data.WobbleCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"WobbleVariation")]
		[ProviderDescription(@"WobbleVariation")]
		[PropertyOrder(1)]
		public Curve WobbleVariationCurve
		{
			get { return _data.WobbleVariationCurve; }
			set
			{
				_data.WobbleVariationCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"XSpeed")]
		[ProviderDescription(@"XSpeed")]
		[PropertyOrder(2)]
		public Curve XCenterSpeedCurve
		{
			get { return _data.XCenterSpeedCurve; }
			set
			{
				_data.XCenterSpeedCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"XSpeedVariation")]
		[ProviderDescription(@"XSpeedVariation")]
		[PropertyOrder(3)]
		public Curve XSpeedVariationCurve
		{
			get { return _data.XSpeedVariationCurve; }
			set
			{
				_data.XSpeedVariationCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"YSpeed")]
		[ProviderDescription(@"YSpeed")]
		[PropertyOrder(4)]
		public Curve YCenterSpeedCurve
		{
			get { return _data.YCenterSpeedCurve; }
			set
			{
				_data.YCenterSpeedCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"YSpeedVariation")]
		[ProviderDescription(@"YSpeedVariation")]
		[PropertyOrder(5)]
		public Curve YSpeedVariationCurve
		{
			get { return _data.YSpeedVariationCurve; }
			set
			{
				_data.YSpeedVariationCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 3)]
		[ProviderDisplayName(@"ColorType")]
		[ProviderDescription(@"ColorType")]
		[PropertyOrder(0)]
		public SnowflakeColorType ColorType
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
		[ProviderCategory(@"Color", 1)]
		[ProviderDisplayName(@"CycleColor")]
		[ProviderDescription(@"CycleColor")]
		[PropertyOrder(1)]
		public bool CycleColor
		{
			get { return _data.CycleColor; }
			set
			{
				_data.CycleColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 3)]
		[ProviderDisplayName(@"CenterColor")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(2)]
		public List<ColorGradient> InnerColor
		{
			get { return _data.InnerColor; }
			set
			{
				_data.InnerColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 3)]
		[ProviderDisplayName(@"OuterColor")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(3)]
		public List<ColorGradient> OutSideColor
		{
			get { return _data.OutSideColor; }
			set
			{
				_data.OutSideColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Level properties

		[Value]
		[ProviderCategory(@"Brightness", 4)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
		[PropertyOrder(1)]
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
			UpdateDirectionAttribute(false);
			UpdateFadeAttribute(false);
			UpdateFlakeAttribute(false);
			UpdateFlakeBuildUpAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		//Used to hide Colors from user when Rainbow type is selected and unhides for the other types.
		private void UpdateColorAttribute(bool refresh = true)
		{
			bool snowFlakeType = ColorType != SnowflakeColorType.RainBow;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2);
			propertyStates.Add("InnerColor", snowFlakeType);
			propertyStates.Add("OutSideColor", snowFlakeType);
			propertyStates.Add("CycleColor", FadeType != FadeType.None && ColorType != SnowflakeColorType.Alternate);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}
		private void UpdateFadeAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			propertyStates.Add("FadeSpeed", FadeType != FadeType.None);
			propertyStates.Add("CycleColor", FadeType != FadeType.None && ColorType != SnowflakeColorType.Alternate);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateFlakeBuildUpAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2);
			propertyStates.Add("InitialBuildUp", SnowBuildUp && SnowflakeEffect <= (SnowflakeEffect) 2);
			propertyStates.Add("BuildUpSpeedCurve", SnowBuildUp && SnowflakeEffect <= (SnowflakeEffect) 2);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateDirectionAttribute(bool refresh = true)
		{
			bool direction = false;
			bool variableDirection = false;
			if (SnowflakeEffect == SnowflakeEffect.Explode)
			{
				direction = true;
			}
			if (SnowflakeEffect == SnowflakeEffect.RandomDirection)
			{
				direction = true;
				variableDirection = true;
			}
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(16);
			propertyStates.Add("Direction", !direction && SnowflakeEffect <= (SnowflakeEffect)2);
			propertyStates.Add("MinDirection", variableDirection && SnowflakeEffect <= (SnowflakeEffect)2);
			propertyStates.Add("MaxDirection", variableDirection && SnowflakeEffect <= (SnowflakeEffect)2);
			propertyStates.Add("SnowBuildUp", SnowflakeEffect != SnowflakeEffect.Explode && SnowFlakeMovement == SnowFlakeMovement.None && SnowflakeEffect <= (SnowflakeEffect) 2);
			propertyStates.Add("WobbleCurve", SnowFlakeMovement >= SnowFlakeMovement.Wobble && SnowflakeEffect <= (SnowflakeEffect)2);
			propertyStates.Add("WobbleVariationCurve", SnowFlakeMovement >= SnowFlakeMovement.Wobble && SnowflakeEffect <= (SnowflakeEffect)2);
			propertyStates.Add("XCenterSpeedCurve", SnowFlakeMovement == SnowFlakeMovement.Speed || SnowflakeEffect > (SnowflakeEffect)2);
			propertyStates.Add("YCenterSpeedCurve", SnowFlakeMovement == SnowFlakeMovement.Speed || SnowflakeEffect > (SnowflakeEffect)2);
			propertyStates.Add("XSpeedVariationCurve", SnowFlakeMovement == SnowFlakeMovement.Speed || SnowflakeEffect > (SnowflakeEffect)2);
			propertyStates.Add("YSpeedVariationCurve", SnowFlakeMovement == SnowFlakeMovement.Speed || SnowflakeEffect > (SnowflakeEffect)2);
			propertyStates.Add("HFlakeCount", SnowflakeEffect > (SnowflakeEffect) 2); 
			propertyStates.Add("VFlakeCount", SnowflakeEffect > (SnowflakeEffect) 2);
			propertyStates.Add("FlakeCountCurve", SnowflakeEffect <= (SnowflakeEffect) 2);
			propertyStates.Add("SnowFlakeMovement", SnowflakeEffect <= (SnowflakeEffect) 2);
			propertyStates.Add("CenterSpeedCurve", SnowflakeEffect <= (SnowflakeEffect)2);
			propertyStates.Add("SpeedVariationCurve", SnowflakeEffect <= (SnowflakeEffect)2);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}

			if (SnowFlakeMovement != SnowFlakeMovement.None) SnowBuildUp = false;
			UpdateFlakeBuildUpAttribute();
		}

		private void UpdateFlakeAttribute(bool refresh = true)
		{
			bool snowFlakeType = SnowflakeType == SnowflakeType.Random;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2);
			propertyStates.Add("PointFlake45", snowFlakeType);
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
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/snowflakes/"; }
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as SnowflakesData;
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
			_increaseFlakeCount = 0;
			_snowfalakeCountAdjust = 0;
			_snowFlakes = new List<SnowFlakeClass>();
			_xSpeedAdjustment = 1;
			_ySpeedAdjustment = 1;
			_wobbleAdjustment = 1;
			_maxBufferSize = Math.Max(BufferHt / 2, BufferWi / 2);
		}

		protected override void CleanUpRender()
		{
			_snowFlakes = null;
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			int colorcntOutSide = OutSideColor.Count;
			int colorcntInside = InnerColor.Count;
			int minDirection = 1;
			int maxDirection = 360;
			var intervalPosFactor = GetEffectTimeIntervalPosition(frame) * 100;
			double level = LevelCurve.GetValue(intervalPosFactor) / 100;

			int flakeCount;
			// create new SnowFlakes and maintain maximum number as per users selection.
			if (SnowflakeEffect < (SnowflakeEffect) 3)
			{
				flakeCount =
					(int) (SnowflakeEffect == SnowflakeEffect.Explode && frame < CalculateCount(intervalPosFactor)
						? 1
						: CalculateCount(intervalPosFactor));
			}
			else
			{
				flakeCount = VFlakeCount * HFlakeCount;
			}

			int initialBuildUp = !SnowBuildUp
				? 0
				: (int) (CalculateInitialBuildUp() + (CalculateBuildUpSpeed(intervalPosFactor)*(frame/(double) 100)));

			var centerSpeed = CalculateCenterSpeed(intervalPosFactor);
			var spreadSpeed = CalculateSpeedVariation(intervalPosFactor);
			var fadeSpeed = CalculateFadeSpeed(intervalPosFactor);
			var minSpeed = centerSpeed - (spreadSpeed / 2);
			var maxSpeed = centerSpeed + (spreadSpeed / 2);
			if (minSpeed < 1) minSpeed = 1;
			if (maxSpeed > 60) maxSpeed = 60;

			double xSpeedRatio = 1;
			double ySpeedRatio = 1;
			double minXSpeed = 1;
			double maxXSpeed = 1;
			double minYSpeed = 1;
			double maxYSpeed = 1;
			int minWobble = 1;
			int maxWobble = 1;
			double wobbleRatio = 1;
			double XGridSpacing = ((double)BufferWi / HFlakeCount - 1) +1;
			double YGridSpacing = ((double)BufferHt / VFlakeCount - 1) +1;
			Yposition = YGridSpacing / 2;
			Xposition = XGridSpacing / 2;

			switch (SnowFlakeMovement)
			{
				case SnowFlakeMovement.Speed:
				case SnowFlakeMovement.None when SnowflakeEffect > (SnowflakeEffect)2:
				case SnowFlakeMovement.Bounce when SnowflakeEffect > (SnowflakeEffect)2:
				case SnowFlakeMovement.Wobble when SnowflakeEffect > (SnowflakeEffect)2:
				case SnowFlakeMovement.Wobble2 when SnowflakeEffect > (SnowflakeEffect)2:
				case SnowFlakeMovement.Wrap when SnowflakeEffect > (SnowflakeEffect)2:
					{
					// Horizontal Speed Control
					double xCenterSpeed = CalculateXCenterSpeed(intervalPosFactor);
					double xSpreadSpeed = CalculateXSpeedVariation(intervalPosFactor);
					minXSpeed = xCenterSpeed - (xSpreadSpeed / 2);
					maxXSpeed = xCenterSpeed + (xSpreadSpeed / 2);
					if (minXSpeed < -100) minXSpeed = -100;
					if (maxXSpeed > 100) maxXSpeed = 100;
					if (xCenterSpeed != 0)
					{
						if (frame != 0) xSpeedRatio = xCenterSpeed / _xSpeedAdjustment;
						_xSpeedAdjustment = xCenterSpeed;
					}

					// Vertical Speed Control
					double yCenterSpeed = CalculateYCenterSpeed(intervalPosFactor);
					double ySpreadSpeed = CalculateYSpeedVariation(intervalPosFactor);
					minYSpeed = yCenterSpeed - (ySpreadSpeed / 2);
					maxYSpeed = yCenterSpeed + (ySpreadSpeed / 2);
					if (minYSpeed < -100) minYSpeed = -100;
					if (maxYSpeed > 100) maxYSpeed = 100;
					if (yCenterSpeed != 0)
					{
						if (frame != 0) ySpeedRatio = yCenterSpeed / _ySpeedAdjustment;
						_ySpeedAdjustment = yCenterSpeed;
					}

					break;
				}

				// Wobble Control
				case SnowFlakeMovement.Wobble:
				case SnowFlakeMovement.Wobble2:
				{
					double wobbleCenterPosition = CalculateWobbleCenter(intervalPosFactor);
					double wobbleSpreadPosition = CalculateWobbleVariation(intervalPosFactor);
					minWobble = (int) (wobbleCenterPosition - (wobbleSpreadPosition / 2));
					maxWobble = (int) (wobbleCenterPosition + (wobbleSpreadPosition / 2));
					if (minWobble < -_maxBufferSize) minWobble = -_maxBufferSize;
					if (maxWobble > _maxBufferSize) maxWobble = _maxBufferSize;
					if (wobbleCenterPosition != 0)
					{
						if (frame != 0) wobbleRatio = wobbleCenterPosition / _wobbleAdjustment;
						_wobbleAdjustment = (int) wobbleCenterPosition;
					}

					break;
				}
			}

			for (int i = 0; i < flakeCount; i++)
			{
				SnowFlakeClass m = new SnowFlakeClass();

				m.Type = SnowflakeType == SnowflakeType.Random ? RandomFlakeType<SnowflakeType>() : SnowflakeType;

				switch (m.Type)
				{
					case SnowflakeType.Five:
					case SnowflakeType.Nine:
						m.SnowflakeWidth = 2;
						break;
					case SnowflakeType.Thirteen:
					case SnowflakeType.ThirteenV2:
					case SnowflakeType.Seventeen:
						m.SnowflakeWidth = 3;
						break;
					case SnowflakeType.FortyFive:
					case SnowflakeType.TwentyNine:
						m.SnowflakeWidth = 5;
						break;
					default:
						m.SnowflakeWidth = 1;
						break;
				}

				if (SnowflakeEffect != SnowflakeEffect.Grid && SnowflakeEffect != SnowflakeEffect.GridOffset)
				{
					if (_snowFlakes.Count >=
						CalculateCount(intervalPosFactor) + _increaseFlakeCount - _snowfalakeCountAdjust) break;
					double position = (RandDouble() * ((maxSpeed + 1) - minSpeed) + minSpeed) / 5;
					if (SnowflakeEffect == SnowflakeEffect.RandomDirection)
					{
						minDirection = MinDirection;
						maxDirection = MaxDirection;
					}

					int direction;
					if (SnowflakeEffect == SnowflakeEffect.None)
						direction = Rand(145,
							215); //Set Range for standard Snowflakes as we don't want to just have them going straight down or two dirctions like the original Snowflake effect.
					else
					{
						//This is to generate random directions between the Min and Max values
						//However if Someone makes the MaxDirection lower then the Min Direction then
						//the new direction will be the inverserve of the Min and Max effectively changing
						//the range from a downward motion to an upward motion, increasing the feature capability.
						direction = maxDirection <= minDirection
							? (Rand(1, 3) == 1 ? Rand(1, maxDirection) : Rand(minDirection, 360))
							: Rand(minDirection, maxDirection);
					}

					//Moving (direction)
					if (direction > 0 && direction <= 90)
					{
						m.DeltaX = ((double) direction / 90) * position;
						m.DeltaY = ((double) Math.Abs(direction - 90) / 90) * position;
						m.WobbleX = ((double) Math.Abs(direction - 90) / 90);
						m.WobbleY = -1 * ((double) Math.Abs(direction) / 90);
						if (RandDouble() >= (double) (90 - direction) / 100)
						{
							m.X = 0;
							m.Y = Rand() % BufferHt;
						}
						else
						{
							m.X = Rand() % BufferWi;
							m.Y = 0;
						}
					}
					else if (direction > 90 && direction <= 180)
					{
						m.DeltaX = ((double) Math.Abs(direction - 180) / 90) * position;
						m.DeltaY = (-1 * ((double) Math.Abs(direction - 90) / 90)) * position;
						m.WobbleX = -1 * ((double) Math.Abs(direction - 90) / 90);
						m.WobbleY = -1 * ((double) Math.Abs(direction - 180) / 90);
						if (RandDouble() >= (double) (180 - direction) / 100)
						{
							m.X = Rand() % BufferWi;
							m.Y = BufferHt;
						}
						else
						{
							m.X = 0;
							m.Y = Rand() % BufferHt;
						}
					}
					else if (direction > 180 && direction <= 270)
					{
						m.DeltaX = (-1 * ((double) Math.Abs(direction - 180) / 90)) * position;
						m.DeltaY = (-1 * ((double) Math.Abs(direction - 270) / 90)) * position;
						m.WobbleX = ((double) Math.Abs(direction - 90) / 90);
						m.WobbleY = -1 * ((double) Math.Abs(direction - 180) / 90);
						if (RandDouble() >= (double) (270 - direction) / 100)
						{
							m.X = BufferWi;
							m.Y = Rand() % BufferHt;
						}
						else
						{
							m.X = Rand() % BufferWi;
							m.Y = BufferHt;
						}
					}
					else if (direction > 270 && direction <= 360)
					{
						m.DeltaX = (-1 * ((double) Math.Abs(direction - 360) / 90)) * position;
						m.DeltaY = ((double) Math.Abs(270 - direction) / 90) * position;
						m.WobbleX = -1 * ((double) Math.Abs(direction - 270) / 90);
						m.WobbleY = -1 * ((double) Math.Abs(direction - 360) / 90);
						if (RandDouble() >= (double) (360 - direction) / 100)
						{
							m.X = Rand() % BufferWi;
							m.Y = 0;
						}
						else
						{
							m.X = BufferWi;
							m.Y = Rand() % BufferHt;
						}
					}

					//Start position for Snowflake
					if (SnowflakeEffect == SnowflakeEffect.Explode) //Will start in the centre of the grid
					{
						m.X = BufferWi / 2;
						m.Y = BufferHt / 2;
					}

					if (frame == 0)
					{
						m.X = Rand() % BufferWi - 1;
						m.Y = Rand(0, BufferHt - 1);
					}

					if (frame == 0)
					{
						m.X = Rand(0, BufferWi);
						m.Y = Rand(0, BufferHt);
					}

					m.DeltaXOrig = m.DeltaX;
					m.DeltaYOrig = m.DeltaY;

					m.Wobble = Rand(minWobble, maxWobble);
					if (Rand(0, 2) == 1 && SnowFlakeMovement == SnowFlakeMovement.Wobble2) m.Wobble = -m.Wobble;
				}
				else
				{
					if (frame != 0) break;
					// Sets grid location of Snowflakes when Grid or Grid Offset is selected

					var gridOffset = Offset ? SnowflakeEffect == SnowflakeEffect.GridOffset ? YGridSpacing / 2 : 0 : 0;

					m.X = (int)Xposition;
					m.Y = (int)((int)Yposition + gridOffset);

					Xposition += XGridSpacing ;
					if ((i + 1) % HFlakeCount == 0)
					{
						Yposition += YGridSpacing;
						Xposition = XGridSpacing / 2;
						if(HFlakeCount % 2 != 0) Offset = !Offset;
					}
					Offset = !Offset;
				}

				if (SnowFlakeMovement == SnowFlakeMovement.Speed || SnowflakeEffect > (SnowflakeEffect)2)
				{
					m.XSpeed = (RandDouble() * ((maxXSpeed) - minXSpeed) + minXSpeed);
					m.YSpeed = (RandDouble() * ((maxYSpeed) - minYSpeed) + minYSpeed);
				}

				//Set the SnowFlake colors during the creation of the snowflake.
				switch (ColorType)
				{
					case SnowflakeColorType.Range
						: //Random two colors are selected from the list for each SnowFlake and then the color range between them are used.
						m.OuterHsv = SetRangeColor(
							HSV.FromRGB(OutSideColor[Rand() % colorcntOutSide].GetColorAt((intervalPosFactor) / 100)),
							HSV.FromRGB(OutSideColor[Rand() % colorcntOutSide].GetColorAt((intervalPosFactor) / 100)));
						m.InnerHsv = SetRangeColor(
							HSV.FromRGB(InnerColor[Rand() % colorcntInside].GetColorAt((intervalPosFactor) / 100)),
							HSV.FromRGB(InnerColor[Rand() % colorcntInside].GetColorAt((intervalPosFactor) / 100)));
						break;
					case SnowflakeColorType.Palette: //All user colors are used
					case SnowflakeColorType.Alternate when SnowflakeEffect <= (SnowflakeEffect)2:
						m.OuterHsv = HSV.FromRGB(OutSideColor[Rand() % colorcntOutSide]
							.GetColorAt((intervalPosFactor) / 100));
						m.InnerHsv =
							HSV.FromRGB(InnerColor[Rand() % colorcntInside].GetColorAt((intervalPosFactor) / 100));
						break;
					case SnowflakeColorType.Alternate:
						m.OuterHsv = HSV.FromRGB(OutSideColor[i % colorcntOutSide]
							.GetColorAt((intervalPosFactor) / 100));
						m.InnerHsv =
							HSV.FromRGB(InnerColor[i % colorcntInside].GetColorAt((intervalPosFactor) / 100));
						break;
					default:
						m.InnerHsv =
							HSV.FromRGB(InnerColor[Rand() % colorcntInside].GetColorAt((intervalPosFactor) / 100));
						break;
				}

				m.OuterHsvBrightness =
					RandomBrightness
						? RandDouble() * (1.0 - .20)
						: 1; //Adds a random brightness to each Snowflake making it look more realistic
				m.BuildUp = false;
				m.Direction = Rand(0, 2) == 0;

				if (frame == 0)
				{
					switch (FadeType)
					{
						case FadeType.None:
						case FadeType.Out:
							m.FadeType = FadeType;
							break;
						case FadeType.In:
							m.FadeType = FadeType.In;
							break;
						case FadeType.InOut:
							m.FadeType = Rand(0, 2) == 0 ? FadeType.In : FadeType.Out;
							break;
					}
				}

				_snowFlakes.Add(m);
			}

			if (FadeType == FadeType.InOut) fadeSpeed *= 2;
			if (FadeType != FadeType.None && frame != 0) UpdateSnowFlakes(fadeSpeed, colorcntOutSide, colorcntInside, intervalPosFactor);

			if (SnowBuildUp)
			{
				//Renders the Snow on the ground based off the current height.
				Color col = OutSideColor[0].GetColorAt((intervalPosFactor) / 100);
				if (level < 1)
				{
					HSV hsv = HSV.FromRGB(col);
					hsv.V *= level;
					col = hsv.ToRGB();
				}

				for (int x = 0; x < BufferWi; x++)
				{
					for (int y = 0; y < initialBuildUp; y++)
					{
						//The ground color will be use the first outside color of the snowflakes.
						frameBuffer.SetPixel(x, y, col);
					}
				}
			}

			// render all SnowFlakes
			foreach (SnowFlakeClass snowFlakes in _snowFlakes)
			{
				int xOffsetAdj = 0;
				int yOffsetAdj = 0;
				if (SnowFlakeMovement >= SnowFlakeMovement.Wobble)
				{
					xOffsetAdj = (int)(snowFlakes.WobbleX * snowFlakes.Wobble);
					yOffsetAdj = (int)(snowFlakes.WobbleY * snowFlakes.Wobble);
				}

				snowFlakes.DeltaX += snowFlakes.DeltaXOrig + snowFlakes.XSpeed;
				snowFlakes.DeltaY += snowFlakes.DeltaYOrig + snowFlakes.YSpeed;
				
				int snowflakeOverShoot = snowFlakes.SnowflakeWidth * 2;

				for (int c = 0; c < 1; c++)
				{
					int colorX;
					int colorY;
					if (!snowFlakes.BuildUp)
						//Skips the location processing part to not waste time as the Snowflake is no longer moving and sitting on the bottom.
					{
						//Sets the new position the SnowFlake is moving to
						colorX = xOffsetAdj + snowFlakes.X + (int) snowFlakes.DeltaX;
						colorY = yOffsetAdj + snowFlakes.Y + (int) snowFlakes.DeltaY;

						if (SnowflakeEffect != SnowflakeEffect.Explode)
						{
							if (SnowFlakeMovement >= SnowFlakeMovement.Wobble)
							{
								if (colorX < 0) colorX = BufferWi + colorX;
								if (colorY < 0) colorY = BufferHt + colorY;
								if (colorX > BufferWi) colorX = colorX - BufferWi;
								if (colorY > BufferHt) colorY = colorY - BufferHt;
							}

							if (SnowBuildUp) //Will detect snowflake hits up to 3 pixels wide
							{
								//If BuildUp is checked then check to see if a flake lands on another flake that's already at the bottom.
								foreach (
									SnowFlakeClass snowFlake in
										_snowFlakes.Where(
											snowFlake => snowFlake.BuildUp && (colorX >= snowFlake.BuildUpX - 1 && colorX <= snowFlake.BuildUpX + 1) &&
														 colorY <= snowFlake.BuildUpY))
								{
									snowFlakes.BuildUp = true;
									snowFlakes.BuildUpX = colorX;
									snowFlakes.InnerHsv = HSV.FromRGB(InnerColor[0].GetColorAt((intervalPosFactor) / 100));
									snowFlakes.OuterHsv = HSV.FromRGB(OutSideColor[0].GetColorAt((intervalPosFactor) / 100));
									snowFlakes.BuildUpY = snowFlakes.SnowflakeWidth < 3 ? snowFlake.BuildUpY + 1 : snowFlake.BuildUpY + 2;

									_increaseFlakeCount++; //Ensures a new Snowflake is added on the next frame to replace this one as its now resting on the bottom of the grid.
									break;
								}
							}
						}

						if (ColorType == SnowflakeColorType.RainBow && !snowFlakes.BuildUp)
							//No user colors are used for Rainbow effect. Color selection for user will be hidden.
						{
							snowFlakes.OuterHsv.H = (float) (Rand()%1000)/1000.0f;
							snowFlakes.OuterHsv.S = 1.0f;
							snowFlakes.OuterHsv.V = 1.0f;
						}

						if (colorX >= BufferWi + snowflakeOverShoot || colorY >= BufferHt + snowflakeOverShoot || colorX <= -snowflakeOverShoot || colorY <= initialBuildUp - snowflakeOverShoot)
						{
							//Flags SnowFlakes that have reached the end of the grid as expired unless Buildup is checked and then record the Snowflake
							//position to be used in future frames. Allows new Snowflakes to be created.
							if (SnowBuildUp && colorY <= initialBuildUp && !snowFlakes.BuildUp && SnowflakeEffect != SnowflakeEffect.Explode)
							{
								snowFlakes.BuildUp = true;
								snowFlakes.BuildUpX = colorX;
								snowFlakes.BuildUpY = initialBuildUp + 1;
								snowFlakes.InnerHsv = HSV.FromRGB(InnerColor[0].GetColorAt((intervalPosFactor) / 100));
								snowFlakes.OuterHsv = HSV.FromRGB(OutSideColor[0].GetColorAt((intervalPosFactor) / 100));

								_increaseFlakeCount++; //Ensures a new Snowflake is added on the next frame to replace this one as its now resting on the bottom of the grid.  
							}
							else
							{
								if (!snowFlakes.BuildUp)
									snowFlakes.Expired = true;
							}
						}
					}
					else
					{
						//Sets the color location of the snowflake if building up at the bottom.
						colorX = snowFlakes.BuildUpX;
						colorY = snowFlakes.BuildUpY;
					}

					//Added the color and then adjusts brightness based on effect time position, random Brightness and over all brightness level.
					HSV hsvInner = snowFlakes.InnerHsv;
					HSV hsvOuter = snowFlakes.OuterHsv;
					hsvInner.V *= snowFlakes.OuterHsvBrightness > -0.5 && snowFlakes.OuterHsvBrightness < -0.2 &&
					              snowFlakes.FadeType == FadeType.In
						? 0
						: level;
					Color innerColor = hsvInner.ToRGB();

					switch (snowFlakes.FadeType)
					{
						case FadeType.In:
						case FadeType.None:
							hsvOuter.V *= (snowFlakes.OuterHsvBrightness) * level;
							break;
						case FadeType.Out:
							hsvOuter.V *= (1 - snowFlakes.OuterHsvBrightness) * level;
							break;
					}
					
					Color outerColor = hsvOuter.ToRGB();

					if (initialBuildUp < BufferHt && colorY >= initialBuildUp - snowflakeOverShoot)
					{
						if (snowFlakes.BuildUp)
						{
							//Renders a flat Snowflake on the ground with a width based of size of the flake.
							for (int y = 0; y <= colorY - initialBuildUp; y++)
							{
								for (int x = -y - snowFlakes.SnowflakeWidth; x <= y + snowFlakes.SnowflakeWidth; x++)
								{
									frameBuffer.SetPixel(colorX + x, colorY - y, outerColor);
								}
							}
						}
						else
						{
							//Renders the falling Snowflake
							switch (snowFlakes.Type)
							{
								case SnowflakeType.Three:
									// 3 nodes
									if (snowFlakes.Direction)
									{
										frameBuffer.SetPixel(colorX - 1, colorY, outerColor);
										frameBuffer.SetPixel(colorX + 1, colorY, outerColor);
									}
									else
									{
										frameBuffer.SetPixel(colorX, colorY - 1, outerColor);
										frameBuffer.SetPixel(colorX, colorY + 1, outerColor);
									}
									break;
								case SnowflakeType.Five:
									// 5 nodes
									frameBuffer.SetPixel(colorX - 1, colorY, outerColor);
									frameBuffer.SetPixel(colorX + 1, colorY, outerColor);
									frameBuffer.SetPixel(colorX, colorY - 1, outerColor);
									frameBuffer.SetPixel(colorX, colorY + 1, outerColor);
									break;
								case SnowflakeType.Nine:
									// 9 nodes
									for (int j = 1; j <= 2; j++)
									{
										frameBuffer.SetPixel(colorX - j, colorY, outerColor);
										frameBuffer.SetPixel(colorX + j, colorY, outerColor);
										frameBuffer.SetPixel(colorX, colorY - j, outerColor);
										frameBuffer.SetPixel(colorX, colorY + j, outerColor);
									}
									break;
								case SnowflakeType.Thirteen:
									// 13 nodes
									frameBuffer.SetPixel(colorX - 1, colorY, outerColor);
									frameBuffer.SetPixel(colorX + 1, colorY, outerColor);
									frameBuffer.SetPixel(colorX, colorY - 1, outerColor);
									frameBuffer.SetPixel(colorX, colorY + 1, outerColor);

									frameBuffer.SetPixel(colorX - 1, colorY + 2, outerColor);
									frameBuffer.SetPixel(colorX + 1, colorY + 2, outerColor);
									frameBuffer.SetPixel(colorX - 1, colorY - 2, outerColor);
									frameBuffer.SetPixel(colorX + 1, colorY - 2, outerColor);
									frameBuffer.SetPixel(colorX + 2, colorY - 1, outerColor);
									frameBuffer.SetPixel(colorX + 2, colorY + 1, outerColor);
									frameBuffer.SetPixel(colorX - 2, colorY - 1, outerColor);
									frameBuffer.SetPixel(colorX - 2, colorY + 1, outerColor);
									break;
								case SnowflakeType.ThirteenV2:
									// 13 nodes
									for (int j = 1; j <= 3; j++)
									{
										frameBuffer.SetPixel(colorX - j, colorY, outerColor);
										frameBuffer.SetPixel(colorX + j, colorY, outerColor);
										frameBuffer.SetPixel(colorX, colorY - j, outerColor);
										frameBuffer.SetPixel(colorX, colorY + j, outerColor);
									}
									break;
								case SnowflakeType.Seventeen:
									// 17 nodes
									for (int j = 1; j <= 3; j++)
									{
										frameBuffer.SetPixel(colorX - j, colorY, outerColor);
										frameBuffer.SetPixel(colorX + j, colorY, outerColor);
										frameBuffer.SetPixel(colorX, colorY - j, outerColor);
										frameBuffer.SetPixel(colorX, colorY + j, outerColor);
									}
									frameBuffer.SetPixel(colorX - 1, colorY - 1, outerColor);
									frameBuffer.SetPixel(colorX + 1, colorY + 1, outerColor);
									frameBuffer.SetPixel(colorX - 1, colorY + 1, outerColor);
									frameBuffer.SetPixel(colorX + 1, colorY - 1, outerColor);
									break;
								case SnowflakeType.TwentyNine:
									// 29 nodes
									for (int j = -1; j < 2; j++)
									{
										for (int jj = -2; jj < 3; jj++)
										{
											frameBuffer.SetPixel(colorX + j, colorY + jj, outerColor);
										}
									}

									frameBuffer.SetPixel(colorX - 2, colorY + 1, outerColor);
									frameBuffer.SetPixel(colorX - 2, colorY, outerColor);
									frameBuffer.SetPixel(colorX - 2, colorY - 1, outerColor);
									frameBuffer.SetPixel(colorX + 2, colorY + 1, outerColor);
									frameBuffer.SetPixel(colorX + 2, colorY, outerColor);
									frameBuffer.SetPixel(colorX + 2, colorY - 1, outerColor);

									frameBuffer.SetPixel(colorX - 3, colorY, outerColor);
									frameBuffer.SetPixel(colorX - 4, colorY, outerColor);
									frameBuffer.SetPixel(colorX + 3, colorY, outerColor);
									frameBuffer.SetPixel(colorX + 4, colorY, outerColor);

									frameBuffer.SetPixel(colorX, colorY - 3, outerColor);
									frameBuffer.SetPixel(colorX, colorY - 4, outerColor);
									frameBuffer.SetPixel(colorX, colorY + 3, outerColor);
									frameBuffer.SetPixel(colorX, colorY + 4, outerColor);
									break;
								case SnowflakeType.FortyFive:
									// 45 nodes
									int ii = 4;
									for (int j = -4; j < 5; j++)
									{
										frameBuffer.SetPixel(colorX + j, colorY + ii, outerColor);
										ii--;
									}
									for (int j = -4; j < 5; j++)
									{
										frameBuffer.SetPixel(colorX + j, colorY + j, outerColor);
									}
									frameBuffer.SetPixel(colorX - 2, colorY + 3, outerColor);
									frameBuffer.SetPixel(colorX - 3, colorY + 2, outerColor);
									frameBuffer.SetPixel(colorX - 3, colorY - 2, outerColor);
									frameBuffer.SetPixel(colorX - 2, colorY - 3, outerColor);
									frameBuffer.SetPixel(colorX + 2, colorY + 3, outerColor);
									frameBuffer.SetPixel(colorX + 2, colorY - 3, outerColor);
									frameBuffer.SetPixel(colorX + 3, colorY + 2, outerColor);
									frameBuffer.SetPixel(colorX + 3, colorY - 2, outerColor);
									for (int j = -5; j < 6; j++)
									{
										frameBuffer.SetPixel(colorX, colorY + j, outerColor);
									}
									for (int j = -5; j < 6; j++)
									{
										frameBuffer.SetPixel(colorX + j, colorY, outerColor);
									}
									break;
							}
							frameBuffer.SetPixel(colorX, colorY, innerColor); //Inner point of the Flake
						}
					}

					snowFlakes.XSpeed *= xSpeedRatio;
					snowFlakes.YSpeed *= ySpeedRatio;
					snowFlakes.Wobble *= wobbleRatio;

					switch (SnowFlakeMovement)
					{
						case SnowFlakeMovement.Speed when snowFlakes.Expired:
						case SnowFlakeMovement.Wobble when snowFlakes.Expired:
						case SnowFlakeMovement.Wobble2 when snowFlakes.Expired:
						case SnowFlakeMovement.Wrap when snowFlakes.Expired:
						case SnowFlakeMovement.None when SnowflakeEffect > (SnowflakeEffect)2:
						case SnowFlakeMovement.Bounce when SnowflakeEffect > (SnowflakeEffect)2:
							{
							if (colorX < 0)
							{
								snowFlakes.DeltaX = 0;
								snowFlakes.X = BufferWi;
							}
							if (colorY < 0)
							{
								snowFlakes.DeltaY = 0;
								snowFlakes.Y = BufferHt;
							}
							if (colorX > BufferWi)
							{
								snowFlakes.DeltaX = 0;
								snowFlakes.X = 0;
							}
							if (colorY > BufferHt)
							{
								snowFlakes.DeltaY = 0;
								snowFlakes.Y = 0;
							}
							snowFlakes.Expired = false;
							break;
						}
						case SnowFlakeMovement.Bounce:
						{
							if (colorX < 0)
							{
								snowFlakes.X = 0;
								snowFlakes.DeltaX = 0;
								snowFlakes.DeltaXOrig = -snowFlakes.DeltaXOrig;
								snowFlakes.XSpeed = -snowFlakes.XSpeed;
							}

							if (colorY < 0)
							{
								snowFlakes.Y = 0;
								snowFlakes.DeltaY = 0;
								snowFlakes.DeltaYOrig = -snowFlakes.DeltaYOrig;
								snowFlakes.YSpeed = -snowFlakes.YSpeed;
							}

							if (colorX >= BufferWi)
							{
								snowFlakes.X = BufferWi;
								snowFlakes.DeltaX = 0;
								snowFlakes.DeltaXOrig = -snowFlakes.DeltaXOrig;
								snowFlakes.XSpeed = -snowFlakes.XSpeed;
							}
							if (colorY >= BufferHt)
							{
								snowFlakes.Y = BufferHt;
								snowFlakes.DeltaY = 0;
								snowFlakes.DeltaYOrig = -snowFlakes.DeltaYOrig;
								snowFlakes.YSpeed = -snowFlakes.YSpeed;
							}
							
							snowFlakes.Expired = false;
							break;
						}
					}
				}
			}
			
			if (SnowFlakeMovement == SnowFlakeMovement.None)
			{
				// Deletes SnowFlakes that have expired when reaching the edge of the grid, allowing new Snowflakes to be created.
				int snowFlakeNum = 0;
				while (snowFlakeNum < _snowFlakes.Count)
				{
					if (_snowFlakes[snowFlakeNum].Expired)
					{
						_snowFlakes.RemoveAt(snowFlakeNum);
					}
					else
					{
						snowFlakeNum++;
					}
				}
			}
		}

		private void UpdateSnowFlakes(double fadeSpeed, int colorcntOutSide, int colorcntInside, double intervalPosFactor)
		{
			int i = 0;
			foreach (SnowFlakeClass snowFlake in _snowFlakes)
			{
				if (snowFlake.OuterHsvBrightness + fadeSpeed >= 1)
				{
					if (FadeType == FadeType.InOut) snowFlake.FadeType = snowFlake.FadeType == FadeType.In ? FadeType.Out : FadeType.In;
					snowFlake.OuterHsvBrightness = -0.7;
					if (CycleColor && snowFlake.FadeType == FadeType.In)
					{
						switch (ColorType)
						{
							case SnowflakeColorType.Range
								: //Random two colors are selected from the list for each SnowFlake and then the color range between them are used.
								snowFlake.OuterHsv = SetRangeColor(
									HSV.FromRGB(OutSideColor[Rand() % colorcntOutSide]
										.GetColorAt((intervalPosFactor) / 100)),
									HSV.FromRGB(OutSideColor[Rand() % colorcntOutSide]
										.GetColorAt((intervalPosFactor) / 100)));
								snowFlake.InnerHsv = SetRangeColor(
									HSV.FromRGB(InnerColor[Rand() % colorcntInside]
										.GetColorAt((intervalPosFactor) / 100)),
									HSV.FromRGB(InnerColor[Rand() % colorcntInside]
										.GetColorAt((intervalPosFactor) / 100)));
								break;
							case SnowflakeColorType.Palette: //All user colors are used
								snowFlake.OuterHsv = HSV.FromRGB(OutSideColor[Rand() % colorcntOutSide]
									.GetColorAt((intervalPosFactor) / 100));
								snowFlake.InnerHsv =
									HSV.FromRGB(InnerColor[Rand() % colorcntInside]
										.GetColorAt((intervalPosFactor) / 100));
								break;
							case SnowflakeColorType.Alternate:
								snowFlake.OuterHsv = HSV.FromRGB(OutSideColor[i % colorcntOutSide]
									.GetColorAt((intervalPosFactor) / 100));
								snowFlake.InnerHsv =
									HSV.FromRGB(InnerColor[i % colorcntInside].GetColorAt((intervalPosFactor) / 100));
								break;
							default:
								snowFlake.InnerHsv =
									HSV.FromRGB(InnerColor[Rand() % colorcntInside]
										.GetColorAt((intervalPosFactor) / 100));
								break;
						}
					}

					i++;
				}
				else
				{
					snowFlake.OuterHsvBrightness += fadeSpeed;
				}
			}
		}

		// SnowFlakes
		public class SnowFlakeClass
		{
			public int X;
			public int Y;
			public double DeltaX;
			public double DeltaY;
			public double DeltaXOrig;
			public double DeltaYOrig;
			public HSV OuterHsv = new HSV();
			public HSV InnerHsv = new HSV();
			public bool Expired = false;
			public SnowflakeType Type;
			public double OuterHsvBrightness;
			public bool BuildUp;
			public int BuildUpX;
			public int BuildUpY;
			public double XSpeed;
			public double YSpeed;
			public double WobbleX;
			public double WobbleY;
			public double Wobble;
			public int SnowflakeWidth;
			public FadeType FadeType;
			public bool Direction;
		}

		private double CalculateCount(double intervalPos)
		{
			var value = ScaleCurveToValue(FlakeCountCurve.GetValue(intervalPos), 100, 1);
			if (value < 1) value = 1;

			return value;
		}

		private double CalculateSpeedVariation(double intervalPos)
		{
			var value = ScaleCurveToValue(SpeedVariationCurve.GetValue(intervalPos), 60, 1) * FrameTime / 50d;
			if (value < 1) value = 1;

			return value;
		}

		private double CalculateCenterSpeed(double intervalPos)
		{
			var value = ScaleCurveToValue(CenterSpeedCurve.GetValue(intervalPos), 60, 1) * FrameTime / 50d;
			if (value < 1) value = 1;

			return value;
		}

		private double CalculateInitialBuildUp()
		{
			return ScaleCurveToValue(InitialBuildUp, BufferHt, 0);
		}

		private double CalculateBuildUpSpeed(double intervalPos)
		{
			return ScaleCurveToValue(BuildUpSpeedCurve.GetValue(intervalPos), 80, 1);
		}

		private double CalculateXCenterSpeed(double intervalPos)
		{
			return ScaleCurveToValue(XCenterSpeedCurve.GetValue(intervalPos), 5, -5);
		}

		private double CalculateXSpeedVariation(double intervalPos)
		{
			return ScaleCurveToValue(XSpeedVariationCurve.GetValue(intervalPos), 10, 0);
		}

		private double CalculateYCenterSpeed(double intervalPos)
		{
			return ScaleCurveToValue(YCenterSpeedCurve.GetValue(intervalPos), 5, -5);
		}

		private double CalculateYSpeedVariation(double intervalPos)
		{
			return ScaleCurveToValue(YSpeedVariationCurve.GetValue(intervalPos), 10, 0);
		}

		private double CalculateWobbleCenter(double intervalPos)
		{
			return Math.Round(ScaleCurveToValue(WobbleCurve.GetValue(intervalPos), _maxBufferSize, -_maxBufferSize));
		}

		private int CalculateWobbleVariation(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(WobbleVariationCurve.GetValue(intervalPos), _maxBufferSize, 0));
		}

		private double CalculateFadeSpeed(double intervalPos)
		{
			return ScaleCurveToValue(FadeSpeed.GetValue(intervalPos), 0.1, 0) * FrameTime / 50d;
		}

		// generates a random number between Color num1 and and Color num2.
		private float RandomRange(float num1, float num2)
		{
			double hi, lo;

			if (num1 < num2)
			{
				lo = num1;
				hi = num2;
			}
			else
			{
				lo = num2;
				hi = num1;
			}
			return (float)(RandDouble() * (hi - lo) + lo);
		}

		//Use for Range type
		public HSV SetRangeColor(HSV hsv1, HSV hsv2)
		{
			HSV newHsv = new HSV(RandomRange((float)hsv1.H, (float)hsv2.H),
								 RandomRange((float)hsv1.S, (float)hsv2.S),
								 1.0f);
			return newHsv;
		}

		private T RandomFlakeType<T>()
		{
			T randomEnum;
			bool exclude45PtFlakes; //This is done so the user can exclude 45 Point Flakes from the Random selection as they would be too big on a small Matrix.
			do
			{
				exclude45PtFlakes = false;
				Array values = Enum.GetValues(typeof(T));
				randomEnum = (T)values.GetValue(Rand(0, values.Length));
				if (!PointFlake45 && randomEnum.ToString() == "FortyFive")
					exclude45PtFlakes = true;
			} while (exclude45PtFlakes);
			
			return randomEnum;
		}
	}
}
