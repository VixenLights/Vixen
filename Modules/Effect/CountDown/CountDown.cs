using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Marks;
using Vixen.Module;
using Vixen.Sys.Attribute;
using Vixen.TypeConverters;
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
		private int _countDownNumber;
		private int _maxTextSize;
		private IEnumerable<IMark> _marks = null;
		private List<int> _countDownFrame = new List<int>();
		private int _countDownFrames;
		private int _countDownTimer;
		private string _text;

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
				if (Colors.Any(x => !x.CheckLibraryReference()))
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
		[DisplayName(@"CountDownSource")]
		[Description(@"CountDownSource")]
		[PropertyOrder(0)]
		public CountDownMode CountDownMode
		{
			get
			{
				return _data.CountDownMode;
			}
			set
			{
				if (_data.CountDownMode != value)
				{
					_data.CountDownMode = value;
					UpdateCountDownModeAttributes();
					IsDirty = true;
					OnPropertyChanged();
				}
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Mark Collection")]
		[ProviderDescription(@"Mark Collection that has the time position.")]
		[TypeConverter(typeof(IMarkCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(1)]
		public string MarkCollectionId
		{
			get
			{
				return MarkCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId)?.Name;
			}
			set
			{
				var newMarkCollection = MarkCollections.FirstOrDefault(x => x.Name.Equals(value));
				var id = newMarkCollection?.Id ?? Guid.Empty;
				if (!id.Equals(_data.MarkCollectionId))
				{
					var oldMarkCollection = MarkCollections.FirstOrDefault(x => x.Id.Equals(_data.MarkCollectionId));
					RemoveMarkCollectionListeners(oldMarkCollection);
					_data.MarkCollectionId = id;
					AddMarkCollectionListeners(newMarkCollection);
					IsDirty = true;
					OnPropertyChanged();
				}
			}
		}

		[Value]
		[ProviderCategory("Config", 1)]
		[ProviderDisplayName(@"CountDownType")]
		[ProviderDescription(@"CountDownType")]
		[PropertyOrder(2)]
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
					IsDirty = true;
					OnPropertyChanged();
				}
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"TimeFormat")]
		[ProviderDescription(@"TimeFormat")]
		[PropertyOrder(3)]
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
		[PropertyOrder(4)]
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
		[ProviderDisplayName(@"CountDownInterval")]
		[ProviderDescription(@"CountDownInterval")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 60, 1)]
		[PropertyOrder(5)]
		public int CountDownInterval
		{
			get { return _data.CountDownInterval; }
			set
			{
				_data.CountDownInterval = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"TimeVisibleLength")]
		[ProviderDescription(@"TimeVisibleLength")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 10000, 1)]
		[PropertyOrder(6)]
		public int TimeVisibleLength
		{
			get { return _data.TimeVisibleLength; }
			set
			{
				_data.TimeVisibleLength = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Font")]
		[ProviderDescription(@"Font")]
		[PropertyOrder(7)]
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
		[PropertyOrder(8)]
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

		#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 3)]
		[ProviderDisplayName(@"TextColors")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(0)]
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
			UpdateCountDownModeAttributes(false);
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
		}
		private void UpdateCountDownModeAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(4)
			{
				{"MarkCollectionId", CountDownMode != CountDownMode.None},

				{"TimeVisibleLength", CountDownMode != CountDownMode.None},

				{"CountDownFade", CountDownMode != CountDownMode.None},

				{"LevelCurve", CountDownMode == CountDownMode.None}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
			if (CountDownMode == CountDownMode.None) CountDownFade = CountDownFade.None;
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
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(4)
			{
				{"XOffsetCurve", !hideXOffsetCurve},

				{"YOffsetCurve", !hideYOffsetCurve},

				{"AngleCurve", Direction == CountDownDirection.Rotate},

				{"Speed", Direction != CountDownDirection.None}
			};
			SetBrowsable(propertyStates);

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		protected override void SetupRender()
		{
			// Gets initial countdown value depending on if countdown to end of effect or sequence.
			_countDownNumber = CountDownType == CountDownType.Sequence
				? (int)Math.Ceiling(GetRemainingSequenceTime(0) / 1000)
				: (int)Math.Ceiling(GetRemainingTime(0) / 1000);

			if (CountDownMode != CountDownMode.None)
			{
				_countDownFrame.Clear();
				SetupMarks();
				_countDownFrames = TimeVisibleLength / 50;
				_countDownTimer = 0;
				if (_marks != null)
				{
					foreach (var mark in _marks)
					{
						switch (CountDownFade)
						{
							case CountDownFade.In:
								_countDownFrame.Add((int)((mark.StartTime.TotalMilliseconds - StartTime.TotalMilliseconds - TimeVisibleLength) / 50));
								break;
							case CountDownFade.InOut:
								_countDownFrame.Add((int)((mark.StartTime.TotalMilliseconds - StartTime.TotalMilliseconds - (double)TimeVisibleLength / 2) / 50));
								break;
							default:
								_countDownFrame.Add((int)((mark.StartTime.TotalMilliseconds - StartTime.TotalMilliseconds) / 50));
								break;
						}
					}
				}
			}

			if (TargetPositioning == TargetPositioningType.Locations)
			{
				// Adjust the font size for Location support, default will ensure when swicthing between string and location that the Font will be the same visual size.
				// Font is further adjusted using the scale text slider.
				double newFontSize = ((StringCount - ((StringCount - Font.Size) / 100) * 100)) * ((double)BufferHt / StringCount);
				_font = new Font(Font.FontFamily.Name, (int)newFontSize, Font.Style);
				_newFontSize = _font.Size;
				return;
			}
			_font = Font;
			_newFontSize = Font.Size;
		}

		protected override void CleanUpRender()
		{
			_font = null;
			_newfont = null;
			_countDownFrame.Clear();
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			using (var bitmap = new Bitmap(BufferWi, BufferHt))
			{
				InitialRender(frame, bitmap);
				if (CountDownMode == CountDownMode.None || _countDownTimer > 0) // No point rendering if there is nothing to be displayed saves a significant amount of render time depending on countdown settings.
				{
					double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
					// copy to frameBuffer
					for (int x = 0; x < BufferWi; x++)
					{
						for (int y = 0; y < BufferHt; y++)
						{
							CalculatePixel(x, y, bitmap, level, frameBuffer);
						}
					}
				}
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
			for (int frame = 0; frame < numFrames; frame++)
			{
				frameBuffer.CurrentFrame = frame;
				double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
				using (var bitmap = new Bitmap(BufferWi, BufferHt))
				{
					InitialRender(frame, bitmap);
					if (CountDownMode == CountDownMode.None || _countDownTimer > 0) // No point rendering if there is nothing to be displayed saves a significant amount of render time depending on countdown settings.
					{
						foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
						{
							foreach (var elementLocation in elementLocations)
							{
								CalculatePixel(elementLocation.X, elementLocation.Y, bitmap, level, frameBuffer);
							}
						}
					}
				}
			}
		}
		
		private void InitialRender(int frame, Bitmap bitmap)
		{
			if (_countDownFrame.Any(countDownFrame => frame == countDownFrame)) _countDownTimer = _countDownFrames;
			
			// Only do this if we are required to show the count down on this frame.
			if (CountDownMode == CountDownMode.None || _countDownTimer > 0)
			{
				var intervalPos = GetEffectTimeIntervalPosition(frame);
				var intervalPosFactor = intervalPos * 100;
				var textAngle = CalculateAngle(intervalPosFactor);

				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					if (CountDownMode == CountDownMode.None || _countDownTimer == _countDownFrames) _text = CountDownTime(frame);

					SizeF textsize = new SizeF(0, 0);

					//Adjust Font Size based on the Font scaling factor
					_newFontSize = _font.SizeInPoints * (CalculateFontScale(intervalPosFactor) / 100);
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

					//Rotate the text based off the angle setting
					if (Direction == CountDownDirection.Rotate)
					{
						//move rotation point to center of image
						graphics.TranslateTransform((float) (bitmap.Width / 2 + xOffset), (float) (bitmap.Height / 2 + (yOffset / 2)));
						//rotate
						graphics.SmoothingMode = SmoothingMode.HighQuality;
						graphics.RotateTransform(textAngle);
						//move image back
						graphics.TranslateTransform(-(float) (bitmap.Width / 2 + xOffset), -(float) (bitmap.Height / 2 + (yOffset / 2)));
					}

					switch (Direction)
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
					int offsetLeft = (((BufferWi - _maxTextSize) / 2) * 2 + xOffset) / 2;
					int offsetTop = (((BufferHt - maxht) / 2) * 2 + yOffset) / 2;
					double intervalPosition = (GetEffectTimeIntervalPosition(frame) * Speed) % 1;
					Point point;

					switch (Direction)
					{
						case CountDownDirection.Left:
							// left
							int leftX = BufferWi - (int) (intervalPosition * (textsize.Width + BufferWi));
							point =
								new Point(Convert.ToInt32(CenterStop ? Math.Max(leftX, (BufferWi - (int) textsize.Width) / 2) : leftX),
									offsetTop);
							break;
						case CountDownDirection.Right:
							// right
							int rightX = -_maxTextSize + (int) (intervalPosition * (_maxTextSize + BufferWi));
							point =
								new Point(Convert.ToInt32(CenterStop ? Math.Min(rightX, (BufferWi - (int) textsize.Width) / 2) : rightX),
									offsetTop);
							break;
						case CountDownDirection.Up:
							// up
							int upY = BufferHt - (int) ((textsize.Height + BufferHt) * intervalPosition);
							point = new Point(offsetLeft,
								Convert.ToInt32(CenterStop ? Math.Max(upY, (BufferHt - (int) textsize.Height) / 2) : upY));
							break;
						case CountDownDirection.Down:
							// down
							int downY = -(int) textsize.Height +
							            (int) ((textsize.Height + BufferHt) * intervalPosition);
							point = new Point(offsetLeft,
								Convert.ToInt32(CenterStop
									? Math.Min(downY, (BufferHt - (int) textsize.Height) / 2)
									: downY));
							break;
						default:
							// no movement - centered
							point = new Point((BufferWi - _maxTextSize) / 2 + xOffset, offsetTop);
							break;
					}
					DrawText(_text, graphics, point);
				}
				_countDownTimer--;
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

		private string CountDownTime(int frame)
		{
			// Adjusts countdown value depending on if countdown to end of effect or sequence.
			int countDownNumber = CountDownType == CountDownType.Sequence
				? (int) Math.Ceiling(GetRemainingSequenceTime(frame) / 1000)
				: (int) Math.Ceiling(GetRemainingTime(frame) / 1000);

			if (countDownNumber % CountDownInterval > 0 && countDownNumber > 10)
			{
				if (_countDownNumber > 60 && TimeFormat == TimeFormat.Minutes)
				{
					TimeSpan time = TimeSpan.FromSeconds(_countDownNumber);
					return time.ToString(@"m\:ss");
				}
				return _countDownNumber.ToString();
			}
			_countDownNumber = countDownNumber;

			if (_countDownNumber > 60 && TimeFormat == TimeFormat.Minutes)
			{
				TimeSpan time = TimeSpan.FromSeconds(_countDownNumber);
				return time.ToString(@"m\:ss");
			}
			return _countDownNumber.ToString();
		}

		private void CalculatePixel(int x, int y, Bitmap bitmap, double level, IPixelFrameBuffer frameBuffer)
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
			Color color = bitmap.GetPixel(x, BufferHt - y - 1);

			if (!_emptyColor.Equals(color))
			{
				var hsv = HSV.FromRGB(color);
				switch (CountDownFade)
				{
					case CountDownFade.Out:
						hsv.V = hsv.V * ((double)1 / _countDownFrames * _countDownTimer);
						break;
					case CountDownFade.In:
						hsv.V = hsv.V * (1 - (double)1 / _countDownFrames * _countDownTimer);
						break;
					case CountDownFade.InOut:
						hsv.V = _countDownFrames / (_countDownTimer + 1) < 2
							? hsv.V * ((1 - (double) 1 / _countDownFrames * (_countDownTimer + 1)) * 2)
							: hsv.V * ((double) 1 / _countDownFrames * (_countDownTimer + 1) * 2);
						break;
					default:
						hsv.V = hsv.V * level;
						break;
				}

				frameBuffer.SetPixel(xCoord, yCoord, hsv);
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
			ColorGradient cg = Colors[0 % Colors.Count()];
			var brush = new LinearGradientBrush(new Rectangle(brushPointX, p.Y, _maxTextSize, (int) size.Height), Color.Black,
					Color.Black, mode)
				{InterpolationColors = cg.GetColorBlend()};
			DrawTextWithBrush(text, brush, g, offsetPoint);
			brush.Dispose();
			p.Y += (int) size.Height;
		}

		private void DrawTextAcrossGradient(String text, Graphics g, Point p, LinearGradientMode mode)
		{
			var size = g.MeasureString(text, _newfont);
			var offset = _maxTextSize - (int) size.Width;
			var offsetPoint = new Point(p.X + offset / 2, p.Y);
			ColorGradient cg = Colors[0 % Colors.Count()];
			var brush = new LinearGradientBrush(new Rectangle(0, 0, BufferWi, BufferHt),
					Color.Black,
					Color.Black, mode)
				{InterpolationColors = cg.GetColorBlend()};
			DrawTextWithBrush(text, brush, g, offsetPoint);
			brush.Dispose();
			p.Y += (int) size.Height;
		}

		private void DrawTextWithBrush(string text, Brush brush, Graphics g, Point p)
		{
			g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
			g.DrawString(text, _newfont, brush, p);
		}

		private void SetupMarks()
		{
			IMarkCollection mc = MarkCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
			_marks = mc?.MarksInclusiveOfTime(StartTime, StartTime + TimeSpan);
		}

		/// <inheritdoc />
		protected override void MarkCollectionsChanged()
		{
			if (CountDownMode != CountDownMode.None)
			{
				var markCollection = MarkCollections.FirstOrDefault(x => x.Name.Equals(MarkCollectionId));
				InitializeMarkCollectionListeners(markCollection);
			}
		}

		/// <inheritdoc />
		protected override void MarkCollectionsRemoved(IList<IMarkCollection> addedCollections)
		{
			var mc = addedCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
			if (mc != null)
			{
				//Our collection is gone!!!!
				RemoveMarkCollectionListeners(mc);
				MarkCollectionId = String.Empty;
			}
		}
	}
}
