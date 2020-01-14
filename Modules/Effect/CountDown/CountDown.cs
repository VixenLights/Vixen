using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.Scaling;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.CountDown
{
	public class CountDown : PixelEffectBase
	{
		private CountDownData _data;
		private static Color _emptyColor = Color.FromArgb(0, 0, 0, 0);
		private Font _font;
		private Font _newfont;
		private float _newFontSize;
		private int _maxTextSize;
		private string _text;
		private double _directionPosition;
		private double _fade;
		private double _level;
		private int _countDownNumberIteration;
		private int _previousCountDownNumber;
		private CountDownDirection _direction;
		private int _sizeAdjust;
		private double _fps;

		public CountDown()
		{
			_data = new CountDownData();
			EnableTargetPositioning(true, true);
			UpdateAllAttributes();
		}

		public override bool IsDirty
		{
			get
			{
				if (GradientColors.Any(x => !x.CheckLibraryReference()))
				{
					base.IsDirty = true;
				}

				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
		}

		#region String Setup properties

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

		#region Text properties
		
		[Value]
		[ProviderCategory("Config", 1)]
		[ProviderDisplayName(@"CountDownType")]
		[ProviderDescription(@"CountDownType")]
		[PropertyOrder(0)]
		public CountDownType CountDownType
		{
			get
			{
				return _data.CountDownType;
			}
			set
			{
				if (_data.CountDownType != value)
				{
					_data.CountDownType = value;
					UpdateCountDownTypeAttributes();
					IsDirty = true;
					OnPropertyChanged();
				}
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"TimeFormat")]
		[ProviderDescription(@"TimeFormat")]
		[PropertyOrder(1)]
		public TimeFormat TimeFormat
		{
			get { return _data.TimeFormat; }
			set
			{
				_data.TimeFormat = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"CountDownFade")]
		[ProviderDescription(@"CountDownFade")]
		[PropertyOrder(2)]
		public CountDownFade CountDownFade
		{
			get { return _data.CountDownFade; }
			set
			{
				_data.CountDownFade = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"CountDownSizeMode")]
		[ProviderDescription(@"CountDownSizeMode")]
		[PropertyOrder(3)]
		public SizeMode SizeMode
		{
			get { return _data.SizeMode; }
			set
			{
				_data.SizeMode = value;
				UpdateSizeModeAttributes();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"CountDownTime")]
		[ProviderDescription(@"CountDownTime")]
		[PropertyOrder(4)]
		public string CountDownTime
		{
			get { return _data.CountDownTime; }
			set
			{
				int test;
				bool isNum = int.TryParse(value, out test);
				_data.CountDownTime = !isNum ? "0" : value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Font")]
		[ProviderDescription(@"Font")]
		[PropertyOrder(5)]
		public Font Font
		{
			get { return _data.Font.FontValue; }
			set
			{
				_data.Font.FontValue = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"FontScale")]
		[ProviderDescription(@"FontScale")]
		[PropertyOrder(6)]
		public Curve FontScaleCurve
		{
			get { return _data.FontScaleCurve; }
			set
			{
				_data.FontScaleCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"FilmSpinner")]
		[ProviderDescription(@"FilmSpinner")]
		[PropertyOrder(7)]
		public bool Spinner
		{
			get { return _data.Spinner; }
			set
			{
				_data.Spinner = value;
				UpdateFilmSpinnerAttributes();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Movement properties

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[PropertyOrder(1)]
		public CountDownDirection Direction
		{
			get { return _data.Direction; }
			set
			{
				_data.Direction = value;
				IsDirty = true;
				UpdatePositionXAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"Iterations")]
		[ProviderDescription(@"Iterations")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(6)]
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
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"Vertical Offset")]
		[ProviderDescription(@"Vertical Offset")]
		[PropertyOrder(2)]
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
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"Horizontal Offset")]
		[ProviderDescription(@"Horizontal Offset")]
		[PropertyOrder(2)]
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
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"Angle")]
		[ProviderDescription(@"Angle")]
		[PropertyOrder(2)]
		public Curve AngleCurve
		{
			get { return _data.AngleCurve; }
			set
			{
				_data.AngleCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"CenterStop")]
		[ProviderDescription(@"CenterStop")]
		[PropertyOrder(3)]
		public bool CenterStop
		{
			get { return _data.CenterStop; }
			set
			{
				_data.CenterStop = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"PerIteration")]
		[ProviderDescription(@"PerIteration")]
		[PropertyOrder(4)]
		public bool PerIteration
		{
			get { return _data.PerIteration; }
			set
			{
				_data.PerIteration = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 3)]
		[ProviderDisplayName(@"TextColors")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(0)]
		public List<ColorGradient> GradientColors
		{
			get { return _data.GradientColors; }
			set
			{
				_data.GradientColors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		
		[Value]
		[ProviderCategory(@"Color", 3)]
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
		[ProviderCategory(@"Color", 3)]
		[ProviderDisplayName(@"FilmSpinnerColors")]
		[ProviderDescription(@"FilmSpinnerColors")]
		[PropertyOrder(2)]
		public List<ColorGradient> SpinnerColors
		{
			get { return _data.SpinnerColors; }
			set
			{
				_data.SpinnerColors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 3)]
		[ProviderDisplayName(@"FilmSpinnerGradientMode")]
		[ProviderDescription(@"FilmSpinnerGradientMode")]
		[PropertyOrder(3)]
		public SpinnerGradientMode SpinnerGradientMode
		{
			get { return _data.SpinnerGradientMode; }
			set
			{
				_data.SpinnerGradientMode = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Brightness

		[Value]
		[ProviderCategory(@"Brightness", 4)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"TextBrightness")]
		[PropertyOrder(0)]
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
				_data = value as CountDownData;
				UpdateAllAttributes();
				IsDirty = true;
			}
		}

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/countdown/"; }
		}

		#endregion

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private void UpdateAllAttributes()
		{
			UpdatePositionXAttribute(false);
			UpdateCountDownTypeAttributes(false);
			UpdateSizeModeAttributes(false);
			UpdateStringOrientationAttributes();
			UpdateFilmSpinnerAttributes(false);
			TypeDescriptor.Refresh(this);
		}

		private void UpdateCountDownTypeAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"CountDownTime", CountDownType != CountDownType.Effect }
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}
		private void UpdateSizeModeAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"FontScaleCurve", SizeMode == SizeMode.None }
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateFilmSpinnerAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{"SpinnerColors", Spinner },
				{"SpinnerGradientMode", Spinner }
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
					TypeDescriptor.Refresh(this);
			}
		}

		private void UpdatePositionXAttribute(bool refresh = true)
		{
			bool hideXOffsetCurve = false, hideYOffsetCurve = false;
			switch (Direction)
			{
				case CountDownDirection.Left:
				case CountDownDirection.Right:
					hideXOffsetCurve = true;
					break;
				case CountDownDirection.Up:
				case CountDownDirection.Down:
					hideYOffsetCurve = true;
					break;
			}
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(6)
			{
				{"XOffsetCurve", !hideXOffsetCurve},

				{"YOffsetCurve", !hideYOffsetCurve},

				{"AngleCurve", Direction == CountDownDirection.Rotate},

				{"Speed", Direction != CountDownDirection.None && Direction != CountDownDirection.Rotate},

				{"CenterStop",Direction != CountDownDirection.None &&  Direction != CountDownDirection.Rotate},

				{"PerIteration", Direction != CountDownDirection.None && Direction != CountDownDirection.Rotate}

			};
			SetBrowsable(propertyStates);

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		protected override void SetupRender()
		{
			_fps = 1000d / FrameTime;
			_countDownNumberIteration = -1;
			_direction = Direction;
			_sizeAdjust = 0;
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				// Adjust the font size for Location support, default will ensure when swicthing between string and location that the Font will be the same visual size.
				// Font is further adjusted using the scale text slider.
				double newFontSize = ((StringCount - ((StringCount - Font.Size) / 100) * 100)) * ((double)BufferHt / StringCount);
				_font = new Font(Font.FontFamily.Name, (int)newFontSize, Font.Style);
				_newFontSize = _font.Size;
				return;
			}
			double scaleFactor = ScalingTools.GetScaleFactor();
			_font = new Font(Font.FontFamily, Font.Size / (float)scaleFactor, Font.Style);
			_newFontSize = _font.Size;
		}

		protected override void CleanUpRender()
		{
			_font = null;
			_newfont = null;
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			var bufferHt = BufferHt;
			var bufferWi = BufferWi;
			using (var bitmap = new Bitmap(bufferWi, bufferHt))
			{
				InitialRender(frame, bitmap, bufferHt, bufferWi);
				_level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
				FastPixel.FastPixel fp = new FastPixel.FastPixel(bitmap);
				fp.Lock();
				// copy to frameBuffer
				for (int x = 0; x < bufferWi; x++)
				{
					for (int y = 0; y < bufferHt; y++)
					{
						CalculatePixel(x, y, ref bufferHt, fp, frameBuffer);
					}
				}
				fp.Unlock(false);
				fp.Dispose();
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
			var bufferHt = BufferHt;
			var bufferWi = BufferWi;
			for (int frame = 0; frame < numFrames; frame++)
			{
				frameBuffer.CurrentFrame = frame;
				_level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
				using (var bitmap = new Bitmap(bufferWi, bufferHt))
				{
					InitialRender(frame, bitmap, bufferHt, bufferWi);
					FastPixel.FastPixel fp = new FastPixel.FastPixel(bitmap);
					fp.Lock();
					foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
					{
						foreach (var elementLocation in elementLocations)
						{
							CalculatePixel(elementLocation.X, elementLocation.Y, ref bufferHt, fp, frameBuffer);
						}
					}
					fp.Unlock(false);
					fp.Dispose();
				}
			}
		}
		
		private void InitialRender(int frame, Bitmap bitmap, int bufferHt, int bufferWi)
		{
			var intervalPos = GetEffectTimeIntervalPosition(frame);
			var intervalPosFactor = intervalPos * 100;
			var textAngle = CalculateAngle(intervalPosFactor);

			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				SetFadePositionLevel(frame);
				
				SizeF textsize = new SizeF(0, 0);

				//Adjust Font Size based on the Size mode.
				_sizeAdjust++;
				switch (SizeMode)
				{
					case SizeMode.Grow:
						_newFontSize = _font.SizeInPoints / (float)_fps * _sizeAdjust; 
						break;
					case SizeMode.Shrink:
						_newFontSize = _font.SizeInPoints / (float)_fps * (21 - _sizeAdjust);
						break;
					default:
						_newFontSize = _font.SizeInPoints * (CalculateFontScale(intervalPosFactor) / 100);
						break;

				}
				_newfont = new Font(Font.FontFamily.Name, _newFontSize, Font.Style);

				if (!String.IsNullOrEmpty(_text))
				{
					var size = graphics.MeasureString(_text, _newfont);
					if (size.Width > textsize.Width)
					{
						textsize = size;
					}
				}

				_maxTextSize = Convert.ToInt32(textsize.Width * .95);
				int maxht = Convert.ToInt32(textsize.Height);

				int xOffset = CalculateXOffset(intervalPosFactor);
				int yOffset = CalculateYOffset(intervalPosFactor, maxht);
					 
				switch (_direction)
				{
					case CountDownDirection.Left:
					case CountDownDirection.Right:
						xOffset = 0;
						break;
					case CountDownDirection.Up:
					case CountDownDirection.Down:
						yOffset = 0;
						break;
				}
				int offsetLeft = (((bufferWi - _maxTextSize) / 2) * 2 + xOffset) / 2;
				int offsetTop = (((bufferHt - maxht) / 2) * 2 + yOffset) / 2;
				Point point;

				switch (_direction)
				{
					case CountDownDirection.Left:
						// left
						int leftX = bufferWi - (int) (_directionPosition * (textsize.Width + bufferWi));
						point =
							new Point(Convert.ToInt32(CenterStop ? Math.Max(leftX, (bufferWi - (int) textsize.Width) / 2) : leftX),
								offsetTop);
						break;
					case CountDownDirection.Right:
						// right
						int rightX = -_maxTextSize + (int) (_directionPosition * (_maxTextSize + bufferWi));
						point =
							new Point(Convert.ToInt32(CenterStop ? Math.Min(rightX, (bufferWi - (int) textsize.Width) / 2) : rightX),
								offsetTop);
						break;
					case CountDownDirection.Up:
						// up
						int upY = bufferHt - (int) ((textsize.Height + bufferHt) * _directionPosition);
						point = new Point(offsetLeft,
							Convert.ToInt32(CenterStop ? Math.Max(upY, (bufferHt - (int) textsize.Height) / 2) : upY));
						break;
					case CountDownDirection.Down:
						// down
						int downY = -(int) textsize.Height +
						            (int) ((textsize.Height + bufferHt) * _directionPosition);
						point = new Point(offsetLeft,
							Convert.ToInt32(CenterStop
								? Math.Min(downY, (bufferHt - (int) textsize.Height) / 2)
								: downY));
						break;
					default:
						// no movement - centered
						point = new Point((bufferWi - _maxTextSize) / 2 + xOffset, offsetTop);
						break;
				}

				Point centerOfText = new Point(point.X + (int)textsize.Width / 2, point.Y + (int)textsize.Height / 2);

				int movementXOffset = 0;
				int movementYOffset = 0;
				int filmSpinnerRadialLineAngleInDegrees = 0;

				if (Spinner)
				{									
					movementXOffset = centerOfText.X - (bufferWi / 2);
					movementYOffset = centerOfText.Y - (bufferHt / 2);
					
					filmSpinnerRadialLineAngleInDegrees = GetFilmSpinnerRadialLineAngle(frame);

					// Drawing the spinner swipe needs to be done before the transformation below otherwise the 
					// swipe gradient will not fit the matrix and gets repeated
					DrawFilmSpinnerSwipe(graphics, bufferWi, bufferHt, movementXOffset, movementYOffset, frame, textAngle, filmSpinnerRadialLineAngleInDegrees);
				}

				//Rotate the text based off the angle setting
				if (_direction == CountDownDirection.Rotate)
				{
					//move rotation point to center of image
					graphics.TranslateTransform((float) (bitmap.Width / 2 + xOffset), (float) (bitmap.Height / 2 + (yOffset / 2)));
					//rotate
					graphics.SmoothingMode = SmoothingMode.None;
					graphics.RotateTransform(textAngle);
					//move image back
					graphics.TranslateTransform(-(float) (bitmap.Width / 2 + xOffset), -(float) (bitmap.Height / 2 + (yOffset / 2)));
				}

				if (Spinner)
				{
					int crossHashWidth = GetCrossHashWidth(bufferHt, bufferWi);
					int left = (bufferWi - crossHashWidth) / 2 + movementXOffset;
					int top = (bufferHt - crossHashWidth) / 2 + movementYOffset;

					int spinnerRadialLineLength = (int)Math.Ceiling(Math.Sqrt(bufferWi * bufferWi + bufferHt * bufferHt));

					Rectangle crossHashRect = new Rectangle(left, top, crossHashWidth, crossHashWidth);                    
					DrawFilmSpinner(graphics, crossHashRect, frame, spinnerRadialLineLength, filmSpinnerRadialLineAngleInDegrees);                            
				}
				DrawText(_text, graphics, point);
			}
		}
		  
		private void SetFadePositionLevel(int frame)
		{
			var totalFrames = GetNumberFrames();
			int startFrame = 0;
			if (CountDownType == CountDownType.Effect) startFrame = (int)((totalFrames / _fps - Math.Floor(totalFrames / _fps)) * _fps);

			if (PerIteration)
			{
				_directionPosition = CountDownType != CountDownType.Effect
					? (double) 1 / totalFrames * (TimeSpan.TotalSeconds) * frame % 1
					: 1 / _fps * ((frame - startFrame - 1) % _fps);
			}
			else
			{
				_directionPosition = (GetEffectTimeIntervalPosition(frame) * Speed) % 1;
			}
			
			_text = GetCountDownTime(frame);

				
			switch (CountDownFade)
			{
				case CountDownFade.In:
					_fade = 1 / _fps * ((frame - startFrame) % _fps);
					if (_fade < 0) _fade += 1;
					break;
				case CountDownFade.Out:
					_fade = 1 - 1 / _fps * ((frame - startFrame - 1) % _fps);
					if (_fade > 1) _fade -= 1;
						break;
				case CountDownFade.InOut:
					_fade = (frame - startFrame) % _fps;
					if (_fade < 0) _fade = -_fade;
					if (_fade < 10)
					{
						_fade = 1 / _fps * _fade;
						if (_fade < 0) _fade += 1;
					}
					else
					{
						_fade = 1 - 1 / _fps * _fade;
						if (_fade > 1) _fade -= 1;
					}
					break;
				case CountDownFade.None:
					_fade = 1;
					break;
			}
		}

		private int CalculateXOffset(double intervalPos)
		{
			return (int)ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), BufferWi, -BufferWi);
		}

		private int CalculateYOffset(double intervalPos, int maxht)
		{
			return (int)ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), BufferHt + maxht/2, -BufferHt - maxht/2);
		}

		private int CalculateAngle(double intervalPos)
		{
			return (int)ScaleCurveToValue(AngleCurve.GetValue(intervalPos), 360, 0);
		}

		private float CalculateFontScale(double intervalPos)
		{
			return (float)ScaleCurveToValue(FontScaleCurve.GetValue(intervalPos), 100, 1);
		}

		private string GetCountDownTime(int frame)
		{
			// Adjusts countdown value depending on if countdown to end of effect or user defined.
			int countDownNumber;
			switch (CountDownType)
			{
				case CountDownType.CountDown:
					countDownNumber =
						(int) Math.Ceiling((Convert.ToInt16(CountDownTime) * 1000 - (double) (frame * FrameTime)) / 1000);
					if (CountDownFade == CountDownFade.In) countDownNumber -= 1;
					break;
				case CountDownType.CountUp:
					countDownNumber =
						(int)Math.Floor((Convert.ToInt16(CountDownTime) * 1000 + (double)(frame * FrameTime)) / 1000);
					if (CountDownFade == CountDownFade.In) countDownNumber += 1;
					break;
				default:
					countDownNumber = (int) Math.Ceiling(GetRemainingTime(frame) / 1000);
					break;
			}
			if (_previousCountDownNumber != countDownNumber)
			{
				_previousCountDownNumber = countDownNumber;
				_countDownNumberIteration++;
				if (Direction == CountDownDirection.Random)
				{
					_direction = (CountDownDirection)Rand(0, 6);
				}
				_sizeAdjust = 0;
			}
			
			if (countDownNumber > 60 && TimeFormat == TimeFormat.Minutes)
			{
				return ConvertMinutesToString(countDownNumber);
			}
			return countDownNumber.ToString();
		}

		private void CalculatePixel(int x, int y, ref int bufferHt, FastPixel.FastPixel bitmap, IPixelFrameBuffer frameBuffer)
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
			Color color = bitmap.GetPixel(x, bufferHt - y - 1);

			if (!_emptyColor.Equals(color))
			{
				frameBuffer.SetPixel(xCoord, yCoord, color);
			}
			else if (TargetPositioning == TargetPositioningType.Locations)
			{
				frameBuffer.SetPixel(xCoord, yCoord, Color.Transparent);
			}
		}

		private void DrawText(String text, Graphics g, Point p)
		{
			switch (GradientMode)
			{
				case GradientMode.AcrossElement:
					DrawTextAcrossGradient(text, g, p, LinearGradientMode.Horizontal);
					break;
				case GradientMode.AcrossText:
					DrawTextWithGradient(text, g, p, LinearGradientMode.Horizontal);
					break;
				case GradientMode.VerticalAcrossElement:
					DrawTextAcrossGradient(text, g, p, LinearGradientMode.Vertical);
					break;
				case GradientMode.VerticalAcrossText:
					DrawTextWithGradient(text, g, p, LinearGradientMode.Vertical);
					break;
				case GradientMode.DiagonalAcrossElement:
					DrawTextAcrossGradient(text, g, p, LinearGradientMode.ForwardDiagonal);
					break;
				case GradientMode.DiagonalAcrossText:
					DrawTextWithGradient(text, g, p, LinearGradientMode.ForwardDiagonal);
					break;
				case GradientMode.BackwardDiagonalAcrossElement:
					DrawTextAcrossGradient(text, g, p, LinearGradientMode.BackwardDiagonal);
					break;
				case GradientMode.BackwardDiagonalAcrossText:
					DrawTextWithGradient(text, g, p, LinearGradientMode.BackwardDiagonal);
					break;
			}
		}
		
		private void DrawTextWithGradient(String text, Graphics g, Point p, LinearGradientMode mode)
		{
			var size = g.MeasureString(text, _newfont);
			var offset = _maxTextSize - (int) size.Width;
			var offsetPoint = new Point(p.X + offset / 2, p.Y);
			var brushPointX = offsetPoint.X;

			Rectangle rect = new Rectangle(brushPointX, p.Y, _maxTextSize, (int)size.Height);
			Brush brush = CreateGradientBrushForRectangle(mode, GradientColors, rect);
				
			DrawTextWithBrush(text, brush, g, offsetPoint);
			brush.Dispose();
			p.Y += (int) size.Height;
		}

		private Brush CreateGradientBrushForRectangle(LinearGradientMode mode, List<ColorGradient> gradientColors, Rectangle brushRect)
		{
			ColorGradient cg = _level < 1 || CountDownFade != CountDownFade.None ? GetNewGolorGradient(gradientColors) : new ColorGradient(gradientColors[_countDownNumberIteration % gradientColors.Count()]);

			var brush = new LinearGradientBrush(            
				brushRect,
				Color.Black,
				Color.Black, 
				mode)
				{ InterpolationColors = cg.GetColorBlend() };

			return brush;
		}
	
		private void DrawTextAcrossGradient(String text, Graphics g, Point p, LinearGradientMode mode)
		{
			var size = g.MeasureString(text, _newfont);
			var offset = _maxTextSize - (int) size.Width;
			var offsetPoint = new Point(p.X + offset / 2, p.Y);

			Rectangle rect = new Rectangle(0, 0, BufferWi, BufferHt);
			Brush brush = CreateGradientBrushForRectangle(mode, GradientColors, rect);

			DrawTextWithBrush(text, brush, g, offsetPoint);            
			brush.Dispose();

			p.Y += (int) size.Height;
		}

		private ColorGradient GetNewGolorGradient(List<ColorGradient> gradientColors)
		{
			ColorGradient cg = new ColorGradient(gradientColors[_countDownNumberIteration % gradientColors.Count()]);
			foreach (var color in cg.Colors)
			{
				HSV hsv = HSV.FromRGB(color.Color.ToRGB());
				if (CountDownFade != CountDownFade.None) hsv.V *= _fade;
				hsv.V *= _level;
				color.Color = XYZ.FromRGB(hsv.ToRGB());
			}
			return cg;
		}

		private void DrawTextWithBrush(string text, Brush brush, Graphics g, Point p)
		{
			g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
			g.DrawString(text, _newfont, brush, p);
		}

		public override void GenerateVisualRepresentation(Graphics g, Rectangle clipRectangle)
		{
			LinearGradientMode mode = LinearGradientMode.Horizontal;
			switch (GradientMode)
			{
				case GradientMode.VerticalAcrossElement:
				case GradientMode.VerticalAcrossText:
					mode = LinearGradientMode.Vertical;
					break;
				case GradientMode.DiagonalAcrossElement:
				case GradientMode.DiagonalAcrossText:
					mode = LinearGradientMode.ForwardDiagonal;
					break;
				case GradientMode.BackwardDiagonalAcrossElement:
				case GradientMode.BackwardDiagonalAcrossText:
					mode = LinearGradientMode.BackwardDiagonal;
					break;
			}
			int secondTicks;
			string displayTime;
			int countTime = Convert.ToInt32(CountDownTime);
			_countDownNumberIteration = 0;
			switch (CountDownType)
			{
				case CountDownType.CountDown:
					if (CountDownFade == CountDownFade.In) countTime--;
					secondTicks = (int)((double)clipRectangle.Width / (int)TimeSpan.Ticks * 10000000);// TimeSpan.TicksPerMinute / 60 / 100000;
					for (int i = 0; i < clipRectangle.Width; i++)
					{
						if (i% secondTicks == 0)
						{
							if (countTime > 60 && TimeFormat == TimeFormat.Minutes)
							{
								displayTime = ConvertMinutesToString(countTime);
							}
							else
							{
								displayTime = countTime.ToString();
							}																					
							DrawText(g, clipRectangle, displayTime, mode, i);
							countTime -= 1;
						}
					}
					break;
				case CountDownType.CountUp:
					if (CountDownFade == CountDownFade.In) countTime++;
					secondTicks = (int)((double)clipRectangle.Width / (int)TimeSpan.Ticks * 10000000);
					for (int i = 0; i < clipRectangle.Width; i++)
					{
						if (i % secondTicks == 0)
						{
							if (countTime > 60 && TimeFormat == TimeFormat.Minutes)
							{
								displayTime = ConvertMinutesToString(countTime);
							}
							else
							{
								displayTime = countTime.ToString();
							}
							DrawText(g, clipRectangle, displayTime, mode, i);
							countTime += 1;
						}
					}
					break;
				default:
					double totalFrames = GetNumberFrames();
					string displayTime1 = "";
					bool isInt = Math.Abs((totalFrames / _fps) % 1) <= Double.Epsilon * 100;
					int startTick = (int)((double)clipRectangle.Width / totalFrames * (int)((totalFrames / _fps - Math.Floor(totalFrames / _fps)) * _fps));
					countTime = (int)Math.Ceiling(totalFrames / _fps);
					if (!isInt) countTime--;
					secondTicks = (int)((double)clipRectangle.Width / (int)TimeSpan.Ticks * 10000000);
					for ( int i = 0; i < clipRectangle.Width; i++)
					{
						if (i % secondTicks == 0)
						{
							if (countTime > 60 && TimeFormat == TimeFormat.Minutes)
							{
								displayTime = ConvertMinutesToString(countTime);
								if (i == 0 && startTick > 2) displayTime1 = ConvertMinutesToString(countTime + 1);
							}
							else
							{
								displayTime = countTime.ToString();
								if (i == 0 && startTick > 2) displayTime1 = (countTime + 1).ToString();
							}
							if (i == 0 && startTick > 2) DrawText(g, clipRectangle, displayTime1, mode, i);
							DrawText(g, clipRectangle, displayTime, mode, i + startTick);
							countTime -= 1;
						}
					}
					break;
			}
		}

		private string ConvertMinutesToString(long countTime)
		{
			if (countTime < 3600)
			{
				return TimeSpan.FromSeconds(countTime).ToString(@"m\:ss");
			}
			return TimeSpan.FromSeconds(countTime).ToString(@"h\:mm\:ss");
		}

		private void DrawText(Graphics g, Rectangle clipRectangle, string displayedText, LinearGradientMode mode, int startX)
		{
			Font adjustedFont = Vixen.Common.Graphics.GetAdjustedFont(g, displayedText, clipRectangle, Font.Name, 48, Font);
			SizeF adjustedSizeNew = g.MeasureString(displayedText, adjustedFont);
			var brush = new LinearGradientBrush(
					new Rectangle(clipRectangle.X + startX, 2, (int)adjustedSizeNew.Width, (int)adjustedSizeNew.Height),
					Color.Black,
					Color.Black, mode)
			{ InterpolationColors = GradientColors[_countDownNumberIteration % GradientColors.Count()].GetColorBlend() }; 
			g.DrawString(displayedText, adjustedFont, brush, clipRectangle.X + startX, 2);
			_countDownNumberIteration++;

			if (Spinner)
			{
				int squareWidth = Math.Min((int)adjustedSizeNew.Width, (int)adjustedSizeNew.Height);
				Rectangle rect = new Rectangle(clipRectangle.X + startX, 2 + ((int)adjustedSizeNew.Height - squareWidth) / 2, squareWidth, squareWidth);

				using (Brush crossHashBrush = CreateCrossHashBrush(g, rect))
				{
					using (Pen pen = new Pen(crossHashBrush, 1))
					{
						DrawFilmSpinnerCrossHash(g, rect, pen);
					}
				}
			}			
		}

		public override bool ForceGenerateVisualRepresentation { get { return true; } }

		#region Film Spinner Draw Methods

		private void DrawFilmSpinner(
			Graphics graphics,
			Rectangle crossHashRect,
			int frame,
			int spinnerRadialLineLength,
			int radialLineAngleInDegrees)
		{
			Rectangle brushRectangle = new Rectangle(crossHashRect.Left - 1, crossHashRect.Top - 1, crossHashRect.Width + 2, crossHashRect.Height + 2);

			using (Brush brush = CreateCrossHashBrush(graphics, brushRectangle))
			{
				using (Pen pen = new Pen(brush, 1))
				{
					DrawFilmSpinnerCrossHash(graphics, crossHashRect, pen);
															
					DrawFilmSpinnerRadialLine(
						graphics,
						pen,
						radialLineAngleInDegrees + 90,  // Adding 90 degrees to get 0 back to the X-axis
						crossHashRect.Left + crossHashRect.Width / 2,
						crossHashRect.Top + crossHashRect.Height / 2,
						spinnerRadialLineLength);
				}
			}
		}

		private void DrawFilmSpinnerCrossHash(Graphics graphics, Rectangle rect, Pen pen)
		{
			// Draw circle
			graphics.DrawEllipse(pen, rect);

			// Draw vertical line            
			graphics.DrawLine(pen, rect.Left + rect.Width / 2, rect.Top, rect.Left + rect.Width / 2, rect.Bottom);

			// Draw horizontal line            
			graphics.DrawLine(pen, rect.Left, rect.Top + rect.Height / 2, rect.Left + rect.Width, rect.Top + rect.Height / 2);
		}

		private void DrawFilmSpinnerRadialLine(Graphics graphics, Pen pen, int angleInDegrees, int lineStartX, int lineStartY, int lineLength)
		{
			double angleInRadians = angleInDegrees * (Math.PI / 180.0);
			double ptX = lineStartX + lineLength * Math.Cos(angleInRadians);
			double ptY = lineStartY - 1 * lineLength * Math.Sin(angleInRadians);

			graphics.DrawLine(pen, lineStartX, lineStartY, (int)ptX, (int)ptY);
		}
		private void DrawFilmSpinnerSwipe(
			Graphics graphics, 
			int bufferWi, 
			int bufferHt, 
			int movementXOffset, 
			int movementYOffset, 
			int frame, 
			int textRotationAngle,
			int radialLineAngleInDegrees)
		{
			// Calculate the length of the diagonal of the matrix 
			double swipeRadius = (int)Math.Sqrt(bufferWi * bufferWi + bufferHt * bufferHt);
			int swipeRadiusInt = (int)Math.Ceiling(swipeRadius);

			// Get the max width of the spinner cross hash
			int maxCrossHashWidth = GetMaxCrossHashWidth(bufferHt, bufferWi);

			// Swipe circle needs to be big enough so that it covers the matrix even when
			// the cross hash is slightly off the matrix
			int swipeCircleSize = 2 * swipeRadiusInt + maxCrossHashWidth;

			// Center the swipe rectangle over the matrix by adjusting the X and Y of the rectangle
			int negativeX = (int)Math.Ceiling((bufferWi - swipeCircleSize) / 2.0);
			int negativeY = (int)Math.Ceiling((bufferHt - swipeCircleSize) / 2.0);

			Rectangle swipeRect = new Rectangle(
				negativeX + movementXOffset,
				negativeY + movementYOffset,
				swipeCircleSize,
				swipeCircleSize);

			using (Brush swipeBrush = CreateSpinnerSwipeBrush(new Rectangle(0, 0, bufferWi, BufferHt)))
			{
				int rotateAngle = 0;
				if (_direction == CountDownDirection.Rotate)
				{
					rotateAngle = textRotationAngle;
				}

				// Offset by -90 because zero needs to be along the Y axis
				int startAngle = -90 + rotateAngle;
				int endAngle = 360 - radialLineAngleInDegrees;

				graphics.FillPie(swipeBrush, swipeRect, startAngle, endAngle);
			}
		}

		#endregion

		#region Film Spinner Size Methods

		private int GetMaxCrossHashWidth(int bufferHt, int bufferWi)
		{
			// 2 Pixel margin at top and bottom of matrix
			return Math.Min(bufferHt, bufferWi) - 4;
		}

		private int GetCrossHashWidth(int bufferHt, int bufferWi)
		{
			int maxCrossHashWidth = GetMaxCrossHashWidth(bufferHt, bufferWi);
					
			// Attempt to create 20 Pixels differece between minimum and maximum cross hash size			
			int minCrossHashWidth = maxCrossHashWidth - 20;

			if (minCrossHashWidth < 0)
			{
				minCrossHashWidth = 1;
			}

			int crossHashWidth = 0;

			switch (SizeMode)
			{
				case SizeMode.Grow:
					crossHashWidth = minCrossHashWidth + _sizeAdjust;
					break;
				case SizeMode.Shrink:
					crossHashWidth = maxCrossHashWidth - _sizeAdjust; 
					break;
				case SizeMode.None:
				default:
					crossHashWidth = maxCrossHashWidth;
					break;
			}
			
			return crossHashWidth;
		}

		private int GetFilmSpinnerRadialLineAngle(int frame)
		{
			const int MillisecondsPerSecond = 1000;
			double timeRemainingInMilliSeconds = Math.Ceiling(GetRemainingTime(frame) % MillisecondsPerSecond);
			int angleInDegrees = (int)Math.Ceiling((decimal)timeRemainingInMilliSeconds / MillisecondsPerSecond * 360);

			return angleInDegrees;
		}

		#endregion

		#region Film Spinner Brush Methods

		private Brush CreateCrossHashBrush(Graphics graphics, Rectangle rect)
		{
			Brush brush;

			switch (GradientMode)
			{
				case GradientMode.AcrossElement:
				case GradientMode.AcrossText:
					brush = CreateGradientBrushForRectangle(LinearGradientMode.Horizontal, GradientColors, rect);
					break;
				case GradientMode.VerticalAcrossElement:
				case GradientMode.VerticalAcrossText:
					brush = CreateGradientBrushForRectangle(LinearGradientMode.Vertical, GradientColors, rect);
					break;
				case GradientMode.DiagonalAcrossElement:
				case GradientMode.DiagonalAcrossText:
					brush = CreateGradientBrushForRectangle(LinearGradientMode.ForwardDiagonal, GradientColors, rect);
					break;
				case GradientMode.BackwardDiagonalAcrossElement:
				case GradientMode.BackwardDiagonalAcrossText:
				default:
					brush = CreateGradientBrushForRectangle(LinearGradientMode.BackwardDiagonal, GradientColors, rect);
					break;
			}

			return brush;
		}
		private Brush CreateSpinnerSwipeBrush(Rectangle swipeRect)
		{
			Brush brush;

			switch (SpinnerGradientMode)
			{
				case SpinnerGradientMode.Horizontal:
					brush = CreateGradientBrushForRectangle(LinearGradientMode.Horizontal, SpinnerColors, swipeRect); 
					break;
				case SpinnerGradientMode.Vertical:
					brush = CreateGradientBrushForRectangle(LinearGradientMode.Vertical, SpinnerColors, swipeRect);
					break;
				case SpinnerGradientMode.Diagonal:
					brush = CreateGradientBrushForRectangle(LinearGradientMode.ForwardDiagonal, SpinnerColors, swipeRect);
					break;
				case SpinnerGradientMode.ReverseDiagonal:
				default:
					brush = CreateGradientBrushForRectangle(LinearGradientMode.BackwardDiagonal, SpinnerColors, swipeRect);
					break;
			}

			return brush;
		}

		#endregion		
	 }
}
