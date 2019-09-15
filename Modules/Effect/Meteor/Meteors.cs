using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
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

namespace VixenModules.Effect.Meteors
{
	public class Meteors : PixelEffectBase
	{
		private MeteorsData _data;
		private List<MeteorClass> _meteors;
		private double _gradientPosition = 0;
		private IPixelFrameBuffer _tempBuffer;
		private int _maxGroundHeight;
		private double _xSpeedAdjustment;
		private double _ySpeedAdjustment;
		private int _wobbleAdjustment;
		private int _maxBufferSize;

		public Meteors()
		{
			_data = new MeteorsData();
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
		[PropertyOrder(1)]
		public MeteorsEffect MeteorEffect
		{
			get { return _data.MeteorEffect; }
			set
			{
				_data.MeteorEffect = value;
				IsDirty = true;
				UpdateDirectionAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 360, 1)]
		[PropertyOrder(2)]
		public int Direction
		{
			get { return _data.Direction; }
			set
			{
				_data.Direction = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MinAngle")]
		[ProviderDescription(@"MinMeteorAngle")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 360, 1)]
		[PropertyOrder(3)]
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
		[ProviderDescription(@"MaxMeteorAngle")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 360, 1)]
		[PropertyOrder(4)]
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
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyOrder(5)]
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
		[PropertyOrder(6)]
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
		[ProviderDisplayName(@"Count")]
		[ProviderDescription(@"Count")]
		[PropertyOrder(7)]
		public Curve PixelCountCurve
		{
			get { return _data.PixelCountCurve; }
			set
			{
				_data.PixelCountCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"TailLength")]
		[ProviderDescription(@"TailLength")]
		[PropertyOrder(8)]
		public Curve LengthCurve
		{
			get { return _data.LengthCurve; }
			set
			{
				_data.LengthCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"StartPosition")]
		[ProviderDescription(@"StartPosition")]
		[PropertyOrder(9)]
		public MeteorStartPosition MeteorStartPosition
		{
			get { return _data.MeteorStartPosition; }
			set
			{
				_data.MeteorStartPosition = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Movement")]
		[ProviderDescription(@"MovementMode")]
		[PropertyOrder(10)]
		public MeteorMovement MeteorMovement
		{
			get { return _data.MeteorMovement; }
			set
			{
				_data.MeteorMovement = value;
				IsDirty = true;
				UpdateDirectionAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"EnableGroundLevel")]
		[ProviderDescription(@"EnableGroundLevel")]
		[PropertyOrder(11)]
		public bool EnableGroundLevel
		{
			get { return _data.EnableGroundLevel; }
			set
			{
				_data.EnableGroundLevel = value;
				IsDirty = true;
				UpdateGroundLevelAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"GroundLevel")]
		[ProviderDescription(@"GroundLevel")]
		[PropertyOrder(12)]
		public Curve GroundLevelCurve
		{
			get { return _data.GroundLevelCurve; }
			set
			{
				_data.GroundLevelCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MeteorPerString")]
		[ProviderDescription(@"MeteorPerString")]
		[PropertyOrder(13)]
		public bool MeteorPerString
		{
			get { return _data.MeteorPerString; }
			set
			{
				_data.MeteorPerString = value;
				IsDirty = true;
				UpdateDirectionAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"FlipDirection")]
		[ProviderDescription(@"FlipDirection")]
		[PropertyOrder(14)]
		public bool FlipDirection
		{
			get { return _data.FlipDirection; }
			set
			{
				_data.FlipDirection = value;
				IsDirty = true;
				UpdateDirectionAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"CountPerString")]
		[ProviderDescription(@"CountPerString")]
		[PropertyOrder(15)]
		public bool CountPerString
		{
			get { return _data.CountPerString; }
			set
			{
				_data.CountPerString = value;
				IsDirty = true;
				UpdateDirectionAttribute();
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
		public MeteorsColorType ColorType
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
		[ProviderCategory(@"Color", 3)]
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

		[Value]
		[ProviderCategory(@"Color", 3)]
		[ProviderDisplayName(@"GroundColor")]
		[ProviderDescription(@"GroundColor")]
		[PropertyOrder(2)]
		public ColorGradient GroundColor
		{
			get { return _data.GroundColor; }
			set
			{
				_data.GroundColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Level properties

		[Value]
		[ProviderCategory(@"Brightness", 4)]
		[ProviderDisplayName(@"RandomIntensity")]
		[ProviderDescription(@"RandomIntensity")]
		[PropertyOrder(0)]
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
			UpdateGroundLevelAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		//Used to hide Colors from user when Rainbow type is selected and unhides for the other types.
		private void UpdateColorAttribute(bool refresh = true)
		{
			bool meteorType = ColorType != MeteorsColorType.RainBow;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			propertyStates.Add("Colors", meteorType);
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
			if (MeteorEffect == MeteorsEffect.Explode)
			{
				direction = true;
			}
			if (MeteorEffect == MeteorsEffect.RandomDirection)
			{
				direction = true;
				variableDirection = true;
			}
			
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(11);
			propertyStates.Add("Direction", !direction && !MeteorPerString); 
			propertyStates.Add("MinDirection", variableDirection && !MeteorPerString);
			propertyStates.Add("MaxDirection", variableDirection && !MeteorPerString);
			propertyStates.Add("FlipDirection", MeteorPerString);
			propertyStates.Add("CountPerString", MeteorPerString);
			propertyStates.Add("WobbleCurve", MeteorMovement >= MeteorMovement.Wobble);
			propertyStates.Add("WobbleVariationCurve", MeteorMovement >= MeteorMovement.Wobble);
			propertyStates.Add("XCenterSpeedCurve", MeteorMovement == MeteorMovement.Speed);
			propertyStates.Add("YCenterSpeedCurve", MeteorMovement == MeteorMovement.Speed);
			propertyStates.Add("XSpeedVariationCurve", MeteorMovement == MeteorMovement.Speed);
			propertyStates.Add("YSpeedVariationCurve", MeteorMovement == MeteorMovement.Speed);
			propertyStates.Add("EnableGroundLevel", MeteorMovement == MeteorMovement.None);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}

			if (MeteorMovement != MeteorMovement.None) EnableGroundLevel = false;
		}

		private void UpdateGroundLevelAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2);
			propertyStates.Add("GroundLevelCurve", EnableGroundLevel);
			propertyStates.Add("GroundColor", EnableGroundLevel);
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
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/meteors/"; }
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as MeteorsData;
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
			_maxGroundHeight = 0;
			_xSpeedAdjustment = 1;
			_ySpeedAdjustment = 1;
			_wobbleAdjustment = 1;

			if (EnableGroundLevel)
			{
				_tempBuffer = new PixelFrameBuffer(BufferWi + 10, BufferHt + 10);
				for (int x = 0; x < BufferWi; x++)
				{
					for (int y = 0; y < CalculateGroundLevel(((double) 100/BufferWi)*x); y++)
					{
						_tempBuffer.SetPixel(x, y, GroundColor.GetColorAt(0));
						int temp = (int) CalculateGroundLevel(y);
						if (temp > _maxGroundHeight)
							_maxGroundHeight = temp;
					}
				}
			}

			_meteors = new List<MeteorClass>(32);
			_maxBufferSize = Math.Max(BufferHt / 2, BufferWi / 2);
		}

		protected override void CleanUpRender()
		{
			_meteors = null;
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			int colorcnt = Colors.Count;
			var intervalPosFactor = GetEffectTimeIntervalPosition(frame) * 100;
			double level = LevelCurve.GetValue(intervalPosFactor) / 100;
			var length = CalculateLength(intervalPosFactor);
			int tailLength = BufferHt < 10 ? length / 10 : BufferHt * length / 100;
			int minDirection = 1;
			int maxDirection = 360;
			int pixelCount = CalculatePixelCount(intervalPosFactor);
			double centerSpeed = CalculateCenterSpeed(intervalPosFactor);
			double spreadSpeed = CalculateSpeedVariation(intervalPosFactor);
			double minSpeed = centerSpeed - (spreadSpeed / 2);
			double maxSpeed = centerSpeed + (spreadSpeed / 2);
			if (minSpeed < 0) minSpeed = 0;
			if (maxSpeed > 200) maxSpeed = 200;
			if (tailLength < 1) tailLength = 1;

			double xSpeedRatio = 1;
			double ySpeedRatio = 1;
			double minXSpeed = 1;
			double maxXSpeed = 1;
			double minYSpeed = 1;
			double maxYSpeed = 1;
			int minWobble = 1;
			int maxWobble = 1;
			double wobbleRatio = 1;

			switch (MeteorMovement)
			{
				case MeteorMovement.Speed:
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
				case MeteorMovement.Wobble:
				case MeteorMovement.Wobble2:
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

			// create new meteors and maintain maximum number as per users selection.
			HSV hsv;
			int adjustedPixelCount;
			if (frame < pixelCount)
			{
				if (MeteorStartPosition == MeteorStartPosition.InitiallyRandom && frame > pixelCount)
				{
					adjustedPixelCount = 1;
				}
				else if (MeteorStartPosition == MeteorStartPosition.ZeroPosition)
				{
					adjustedPixelCount = pixelCount;
				}
				else
				{
					adjustedPixelCount = pixelCount < 10 || MeteorMovement >= MeteorMovement.Wobble ? pixelCount : pixelCount / 10;
				}
			}
			else
			{
				adjustedPixelCount = pixelCount;
			}

			int stringCount = 1;
			if (CountPerString && MeteorPerString)
			{
				stringCount = BufferWi;
			}

			for (int i = 0; i < adjustedPixelCount; i++)
			{
				for (int j = 0; j < stringCount; j++)
				{
					if (_meteors.Count >= pixelCount * stringCount) break;
					double position = (RandDouble() * ((maxSpeed) - minSpeed) + minSpeed) / 20;
					MeteorClass m = new MeteorClass();
					if (MeteorEffect == MeteorsEffect.RandomDirection)
					{
						minDirection = MinDirection;
						maxDirection = MaxDirection;
					}

					int direction;
					if (MeteorEffect == MeteorsEffect.None)
						direction =
							Direction; //Set Range for standard Meteor as we don't want to just have them going straight down or two directions like the original Meteor effect.
					else
					{
						//This is to generate random directions between the Min and Max values
						//However if Someone makes the MaxDirection lower then the Min Direction then
						//the new direction will be the inverse of the Min and Max effectively changing
						//the range from a downward motion to an upward motion, increasing the feature capability.
						if (maxDirection <= minDirection)
						{
							//used for the Upward movement of the Meteor (added feature)
							direction = Rand(1, 3) == 1 ? Rand(1, maxDirection) : Rand(minDirection, 360);
						}
						else
						{
							//used for the downward movement of the Meteor (standard way)
							direction = Rand(minDirection, maxDirection);
						}
					}

					//Moving
					if (!CountPerString) m.X = Rand() % BufferWi;
					m.Y = Rand() % BufferHt;
					if (MeteorPerString)
					{
						m.TailY = FlipDirection ? 1 : -1;

						if (CountPerString)
						{
							var meteorExists = false;
							foreach (var meteor in _meteors)
							{
								if (meteor.X == j)
								{
									meteorExists = true;
									break;
								}
							}

							if (meteorExists) continue;
							m.X = j;
						}

						m.DeltaY = m.TailY * position;
						m.Y = Rand() % BufferHt;
						m.WobbleX = -1 * ((double)Math.Abs(direction - 270) / 90);
						m.WobbleY = -1 * ((double)Math.Abs(direction - 360) / 90);
					}
					else if (direction >= 0 && direction <= 90)
					{
						m.TailX = ((double) direction / 90);
						m.DeltaX = m.TailX * position;
						m.TailY = ((double) Math.Abs(direction - 90) / 90);
						m.DeltaY = m.TailY * position;
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
						m.WobbleX = ((double)Math.Abs(direction - 90) / 90);
						m.WobbleY = -1 * ((double)Math.Abs(direction ) / 90);
					}
					else if (direction > 90 && direction <= 180)
					{
						m.TailX = ((double) Math.Abs(direction - 180) / 90);
						m.DeltaX = m.TailX * position;
						m.TailY = -1 * ((double) Math.Abs(direction - 90) / 90);
						m.DeltaY = m.TailY * position;
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
						m.WobbleX = -1 * ((double)Math.Abs(direction - 90) / 90);
						m.WobbleY = -1 * ((double)Math.Abs(direction - 180) / 90);
					}
					else if (direction > 180 && direction <= 270)
					{
						m.TailX = -1 * ((double) Math.Abs(direction - 180) / 90);
						m.DeltaX = m.TailX * position;
						m.TailY = -1 * ((double) Math.Abs(direction - 270) / 90);
						m.DeltaY = m.TailY * position;
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
						m.WobbleX = ((double)Math.Abs(direction - 90) / 90);
						m.WobbleY = -1 * ((double)Math.Abs(direction - 180) / 90);
					}
					else if (direction > 270 && direction <= 360)
					{
						m.TailX = -1 * ((double) Math.Abs(direction - 360) / 90);
						m.DeltaX = m.TailX * position;
						m.TailY = ((double) Math.Abs(270 - direction) / 90);
						m.DeltaY = m.TailY * position;
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
						m.WobbleX = -1 * ((double)Math.Abs(direction - 270) / 90);
						m.WobbleY = -1 * ((double)Math.Abs(direction - 360) / 90);
					}

					if (MeteorEffect == MeteorsEffect.Explode)
					{
						m.X = (BufferWi - 1) / 2;
						m.Y = (BufferHt - 1) / 2;
					}
					else
					{
						if (MeteorStartPosition == MeteorStartPosition.ZeroPosition)
						{
							if (MeteorPerString) m.Y = FlipDirection ? 0 : BufferHt;

						}
						else if (MeteorStartPosition == MeteorStartPosition.Random || frame < pixelCount)
						{
							if (!MeteorPerString) m.X = Rand() % BufferWi - 1;
							m.Y = BufferHt - 1 <= _maxGroundHeight ? 0 : Rand(_maxGroundHeight, BufferHt - 1);
						}
					}

					m.DeltaXOrig = m.DeltaX;
					m.DeltaYOrig = m.DeltaY;

					if (MeteorMovement == MeteorMovement.Speed)
					{
						m.XSpeed = (RandDouble() * ((maxXSpeed) - minXSpeed) + minXSpeed);
						m.YSpeed = (RandDouble() * ((maxYSpeed) - minYSpeed) + minYSpeed);
					}
					
					m.Wobble = Rand(minWobble, maxWobble);
					if (Rand(0, 2) == 1 && MeteorMovement == MeteorMovement.Wobble2) m.Wobble = -m.Wobble;

					switch (ColorType)
					{
						case MeteorsColorType.Range: //Random two colors are selected from the list for each meteor.
							m.Hsv =
								SetRangeColor(
									HSV.FromRGB(Colors[Rand() % colorcnt].GetColorAt((intervalPosFactor) / 100)),
									HSV.FromRGB(Colors[Rand() % colorcnt].GetColorAt((intervalPosFactor) / 100)));
							break;
						case MeteorsColorType.Palette: //All colors are used
							m.Hsv = HSV.FromRGB(Colors[Rand() % colorcnt].GetColorAt((intervalPosFactor) / 100));
							break;
						case MeteorsColorType.Gradient:
							m.Color = Rand() % colorcnt;
							_gradientPosition = 100 / (double) tailLength / 100;
							m.Hsv = HSV.FromRGB(Colors[m.Color].GetColorAt(0));
							break;
					}

					m.HsvBrightness = RandomBrightness ? RandDouble() * (1.0 - .20) + .20 : 1;
					_meteors.Add(m);

				}
			}

			if (EnableGroundLevel)
			{
				hsv = HSV.FromRGB(GroundColor.GetColorAt((intervalPosFactor) / 100));
				hsv.V *= LevelCurve.GetValue(intervalPosFactor) / 100;
				for (int x = 0; x < BufferWi; x++)
				{
					for (int y = 0; y < CalculateGroundLevel(((double) 100/BufferWi)*x); y++)
					{
						if (_tempBuffer.GetColorAt(x, y) != Color.Empty)
							frameBuffer.SetPixel(x, y, hsv);
					}
				}
			}

			// render meteors
			foreach (MeteorClass meteor in _meteors)
			{
				int xOffsetAdj = 0;
				int yOffsetAdj = 0;
				if (MeteorMovement >= MeteorMovement.Wobble)
				{
					xOffsetAdj = (int) (meteor.WobbleX * meteor.Wobble);
					yOffsetAdj = (int) (meteor.WobbleY * meteor.Wobble);
				}

				meteor.DeltaX += meteor.DeltaXOrig + meteor.XSpeed;
				meteor.DeltaY += meteor.DeltaYOrig + meteor.YSpeed;
				int colorX = xOffsetAdj + meteor.X + (int)meteor.DeltaX - BufferWi / 100;
				int colorY = yOffsetAdj + meteor.Y + (int)meteor.DeltaY + BufferHt / 100;

				if (MeteorMovement >= MeteorMovement.Wobble)
				{
					if (colorX < 0) colorX = BufferWi + colorX;
					if (colorY < 0)colorY = BufferHt + colorY;
					if (colorX > BufferWi) colorX = colorX - BufferWi;
					if (colorY > BufferHt) colorY = colorY - BufferHt;
				}

				for (int ph = 0; ph < tailLength; ph++)
				{
					switch (ColorType)
					{
						case MeteorsColorType.RainBow: //No user colors are used for Rainbow effect.
							meteor.Hsv.H = (float) (Rand()%1000)/1000.0f;
							meteor.Hsv.S = 1.0f;
							meteor.Hsv.V = 1.0f;
							break;
						case MeteorsColorType.Gradient:
							meteor.Hsv = HSV.FromRGB(Colors[meteor.Color].GetColorAt(_gradientPosition*ph));
							break;
					}
					hsv = meteor.Hsv;
					hsv.V *= meteor.HsvBrightness * (float) (1.0 - (double) ph/tailLength) * level;
					//var decPlaces = (int) (((decimal) (meteor.TailX*ph)%1)*100);
					var decPlaces = (int)(meteor.TailX * ph % 1d * 100);
					if (decPlaces <= 40 || decPlaces >= 60)
					{
						if (MeteorEffect == MeteorsEffect.Explode && ph > 0 && (colorX == (BufferWi / 2) + (int)(Math.Round(meteor.TailX * ph)) || colorX == (BufferWi / 2) - (int)(Math.Round(meteor.TailX * ph)) || colorY == (BufferHt / 2) + (int)(Math.Round(meteor.TailY * ph)) || colorY == (BufferHt / 2) - (int)(Math.Round(meteor.TailY * ph))))
						{
							break;
						}
						frameBuffer.SetPixel(colorX - (int)(Math.Round(meteor.TailX * ph)), colorY - (int)(Math.Round(meteor.TailY * ph)),
							hsv);
					}
				}

				if (colorX > 0 && colorX < BufferWi - 1 && colorY > 0 && colorY < BufferHt && EnableGroundLevel)
				{
					if (!_tempBuffer.GetColorAt(colorX, colorY).IsEmpty)
					{
						if (frame > 1)
						{
							_tempBuffer.SetPixel(colorX, colorY, Color.Empty);
							_tempBuffer.SetPixel(colorX, colorY - 1, Color.Empty);
							_tempBuffer.SetPixel(colorX - 1, colorY, Color.Empty);
							_tempBuffer.SetPixel(colorX + 1, colorY, Color.Empty);
							_tempBuffer.SetPixel(colorX, colorY + 1, Color.Empty);
							_tempBuffer.SetPixel(colorX + 1, colorY + 1, Color.Empty);
							_tempBuffer.SetPixel(colorX - 1, colorY + 1, Color.Empty);
							_tempBuffer.SetPixel(colorX + 1, colorY + 2, Color.Empty);
							_tempBuffer.SetPixel(colorX - 1, colorY + 2, Color.Empty);
							_tempBuffer.SetPixel(colorX + 1, colorY + 3, Color.Empty);
							_tempBuffer.SetPixel(colorX - 1, colorY + 3, Color.Empty);
							_tempBuffer.SetPixel(colorX + 2, colorY + 3, Color.Empty);
							_tempBuffer.SetPixel(colorX - 2, colorY + 3, Color.Empty);
							_tempBuffer.SetPixel(colorX + 2, colorY + 2, Color.Empty);
							_tempBuffer.SetPixel(colorX - 2, colorY + 2, Color.Empty);
							_tempBuffer.SetPixel(colorX, colorY + 2, Color.Empty);
							_tempBuffer.SetPixel(colorX, colorY + 3, Color.Empty);
							_tempBuffer.SetPixel(colorX - 2, colorY + 4, Color.Empty);
							_tempBuffer.SetPixel(colorX - 1, colorY + 4, Color.Empty);
							_tempBuffer.SetPixel(colorX, colorY + 4, Color.Empty);
							_tempBuffer.SetPixel(colorX + 1, colorY + 4, Color.Empty);
							_tempBuffer.SetPixel(colorX + 2, colorY + 4, Color.Empty);
						}
						meteor.Expired = true;
					}
				}

				if (colorX >= BufferWi + tailLength || colorY >= BufferHt + tailLength || colorX < 0 - tailLength ||
				    colorY < 0 - tailLength)
				{
					meteor.Expired = true; //flags Meteors that have reached the end of the grid as expiried.
					//	break;
				}

				meteor.XSpeed = meteor.XSpeed * xSpeedRatio;
				meteor.YSpeed = meteor.YSpeed * ySpeedRatio;
				meteor.Wobble = meteor.Wobble * wobbleRatio;

				switch (MeteorMovement)
				{
					case MeteorMovement.Speed when meteor.Expired:
					case MeteorMovement.Wobble when meteor.Expired:
					case MeteorMovement.Wobble2 when meteor.Expired:
					case MeteorMovement.Wrap when meteor.Expired:
					{
						if (colorX < 0)
						{
							meteor.DeltaX = 0;
							meteor.X = BufferWi;
						}
						if (colorY < 0)
						{
							meteor.DeltaY = 0;
							meteor.Y = BufferHt;
						}
						if (colorX > BufferWi)
						{
							meteor.DeltaX = 0;
							meteor.X = 0;
						}
						if (colorY > BufferHt)
						{
							meteor.DeltaY = 0;
							meteor.Y = 0;
						}
						meteor.Expired = false;
						break;
					}
					case MeteorMovement.Bounce when meteor.Expired:
					{
						if (colorX < 0)
						{
							meteor.X = 0;
							meteor.DeltaX = 0;
							meteor.DeltaXOrig = -meteor.DeltaXOrig;
							meteor.TailX = -meteor.TailX;
							meteor.XSpeed = -meteor.XSpeed;
							}

						if (colorY < 0)
						{
							meteor.Y = 0;
							meteor.DeltaY = 0;
							meteor.DeltaYOrig = -meteor.DeltaYOrig;
							meteor.TailY = -meteor.TailY;
							meteor.YSpeed = -meteor.YSpeed;
							}

						if (colorX > BufferWi)
						{
							meteor.X = BufferWi;
							meteor.DeltaX = 0;
							meteor.DeltaXOrig = -meteor.DeltaXOrig;
							meteor.TailX = -meteor.TailX;
							meteor.XSpeed = -meteor.XSpeed;
						}
						if (colorY > BufferHt)
						{
							meteor.Y = BufferHt;
							meteor.DeltaY = 0;
							meteor.DeltaYOrig = -meteor.DeltaYOrig;
							meteor.TailY = -meteor.TailY;
							meteor.YSpeed = -meteor.YSpeed;
							}

						
							meteor.Expired = false;
						break;
					}
				}
			}

			if (MeteorMovement == MeteorMovement.None)
			{
				// delete old meteors
				int meteorNum = 0;
				while (meteorNum < _meteors.Count)
				{

					if (_meteors[meteorNum].Expired)
					{
						_meteors.RemoveAt(meteorNum);
					}
					else
					{
						meteorNum++;
					}
				}
			}
		}

		private double CalculateSpeedVariation(double intervalPos)
		{
			return ScaleCurveToValue(SpeedVariationCurve.GetValue(intervalPos), 200, 0);
		}

		private double CalculateCenterSpeed(double intervalPos)
		{
			return ScaleCurveToValue(CenterSpeedCurve.GetValue(intervalPos), 200, 0);
		}

		private int CalculatePixelCount(double intervalPos)
		{
			var value = (int)ScaleCurveToValue(PixelCountCurve.GetValue(intervalPos), 200, 1);
			if (value < 1) value = 1;

			return value;
		}

		private int CalculateLength(double intervalPos)
		{
			var value = (int)ScaleCurveToValue(LengthCurve.GetValue(intervalPos), 100, 1);
			if (value < 1) value = 1;

			return value;
		}

		private double CalculateGroundLevel(double intervalPos)
		{
			int maxGroundHeight = MeteorEffect == MeteorsEffect.Explode ? 0 : 6;
			return ScaleCurveToValue(GroundLevelCurve.GetValue(intervalPos), BufferHt - maxGroundHeight, 0);
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

		// for Meteor effects
		public class MeteorClass
		{
			public int X;
			public int Y;
			public double DeltaX;
			public double DeltaY;
			public double DeltaXOrig;
			public double DeltaYOrig;
			public double TailX;
			public double TailY;
			public HSV Hsv = new HSV();
			public bool Expired = false;
			public int Color;
			public double HsvBrightness;
			public double XSpeed;
			public double YSpeed;
			public double WobbleX;
			public double WobbleY;
			public double Wobble;
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
	}
}
