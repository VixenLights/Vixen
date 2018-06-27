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

		private string _text;
		private readonly List<TextClass> _textClass = new List<TextClass>();
		private IEnumerable<IMark> _marks = null;
		private double _directionPosition;
		private double _fade;

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
		[ProviderDisplayName(@"CountDownSource")]
		[ProviderDescription(@"CountDownSource")]
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"CountDownDuration")]
		[ProviderDescription(@"CountDownDuration")]
		[PropertyOrder(2)]
		public CountDownDuration CountDownDuration
		{
			get { return _data.CountDownDuration; }
			set
			{
				_data.CountDownDuration = value;
				UpdateCountDownModeAttributes();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory("Config", 1)]
		[ProviderDisplayName(@"CountDownType")]
		[ProviderDescription(@"CountDownType")]
		[PropertyOrder(3)]
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
		[PropertyOrder(4)]
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
		[PropertyOrder(5)]
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
		[ProviderDisplayName(@"DirectionPerWord")]
		[ProviderDescription(@"DirectionPerWord")]
		[PropertyOrder(6)]
		public bool DirectionPerWord
		{
			get { return _data.DirectionPerWord; }
			set
			{
				_data.DirectionPerWord = value;
				UpdateCountDownModeAttributes();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MarkTimeFreeze")]
		[ProviderDescription(@"MarkTimeFreeze")]
		[PropertyOrder(7)]
		public bool MarkTimeFreeze
		{
			get { return _data.MarkTimeFreeze; }
			set
			{
				_data.MarkTimeFreeze = value;
				UpdateCountDownModeAttributes();
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
		[PropertyOrder(8)]
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
		[PropertyOrder(9)]
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
		[PropertyOrder(10)]
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
		[PropertyOrder(11)]
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

				{"DirectionPerWord", CountDownMode != CountDownMode.None && Direction < CountDownDirection.Rotate},

				{"TimeVisibleLength", CountDownMode != CountDownMode.None && CountDownDuration == CountDownDuration.UserDefined},

				{"CountDownFade", CountDownMode != CountDownMode.None},

				{"LevelCurve", CountDownMode == CountDownMode.None},

				{"CountDownDuration", CountDownMode != CountDownMode.None},

				{"Speed", CountDownMode == CountDownMode.None || !DirectionPerWord},

				{"MarkTimeFreeze", CountDownMode != CountDownMode.None}
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

				{"Speed", Direction != CountDownDirection.None},

				{"DirectionPerWord", CountDownMode != CountDownMode.None && Direction < CountDownDirection.Rotate},

			};
			SetBrowsable(propertyStates);

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		protected override void SetupRender()
		{
			if (CountDownMode != CountDownMode.None) SetupMarks();

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
			_textClass.Clear();
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			using (var bitmap = new Bitmap(BufferWi, BufferHt))
			{
				InitialRender(frame, bitmap);
				if (_text == "") return; // No point going any further as there is no text to be display for this frame.

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
					if (_text == "") continue; // No point going any further as there is no text to be display for this frame.

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
		
		private void InitialRender(int frame, Bitmap bitmap)
		{
			// Only do this if we are required to show the count down on this frame.
			
				var intervalPos = GetEffectTimeIntervalPosition(frame);
				var intervalPosFactor = intervalPos * 100;
				var textAngle = CalculateAngle(intervalPosFactor);

			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				SetFadePositionLevel(frame);

				if (_text == "") return; // No point going any further as there is no text to be display for this frame.

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
				Point point;

				switch (Direction)
				{
					case CountDownDirection.Left:
						// left
						int leftX = BufferWi - (int) (_directionPosition * (textsize.Width + BufferWi));
						point =
							new Point(Convert.ToInt32(CenterStop ? Math.Max(leftX, (BufferWi - (int) textsize.Width) / 2) : leftX),
								offsetTop);
						break;
					case CountDownDirection.Right:
						// right
						int rightX = -_maxTextSize + (int) (_directionPosition * (_maxTextSize + BufferWi));
						point =
							new Point(Convert.ToInt32(CenterStop ? Math.Min(rightX, (BufferWi - (int) textsize.Width) / 2) : rightX),
								offsetTop);
						break;
					case CountDownDirection.Up:
						// up
						int upY = BufferHt - (int) ((textsize.Height + BufferHt) * _directionPosition);
						point = new Point(offsetLeft,
							Convert.ToInt32(CenterStop ? Math.Max(upY, (BufferHt - (int) textsize.Height) / 2) : upY));
						break;
					case CountDownDirection.Down:
						// down
						int downY = -(int) textsize.Height +
						            (int) ((textsize.Height + BufferHt) * _directionPosition);
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
		}

		private void SetFadePositionLevel(int frame)
		{
			_directionPosition = (GetEffectTimeIntervalPosition(frame) * Speed) % 1;
			_fade = 1;
			if (CountDownMode != CountDownMode.None)
			{
				bool clearText = true;
				for (var i = 0; i < _textClass.Count; i++)
				{
					TextClass text = _textClass[i];
					if (frame >= text.StartFrame && frame < text.EndFrame)
					{
						_text = GetCountDownTime(MarkTimeFreeze ? text.Frame : frame);

						switch (CountDownFade)
						{
							case CountDownFade.In:
								_fade = (double) (frame - text.StartFrame) / (text.Frame - text.StartFrame);
								break;
							case CountDownFade.Out:
								_fade = 1 - (double)(frame - text.Frame) / (text.EndFrame - text.Frame);
								break;
							case CountDownFade.InOut:
									_fade = frame < text.Frame
										? (double)(frame - text.StartFrame) / (text.Frame - text.StartFrame)
										: 1 - (double)(frame - text.Frame) / (text.EndFrame - text.Frame);
								break;
							case CountDownFade.None:
								_fade = 1;
								break;
						}
						if (DirectionPerWord) _directionPosition = (double)(frame - text.StartFrame) / (text.EndFrame - text.StartFrame);
						clearText = false;
						break;
					}
				}
				if (clearText) _text = "";
			}
			else
			{
				_text = GetCountDownTime(frame);
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
						hsv.V = hsv.V * _fade;
						break;
					case CountDownFade.In:
						hsv.V = hsv.V * _fade;
						break;
					case CountDownFade.InOut:
						hsv.V = _fade;
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

		// Text Class
		public class TextClass
		{
			public string Text;
			public int StartFrame;
			public int Frame;
			public int EndFrame;
		}

		private void SetupMarks()
		{
			IMarkCollection mc = MarkCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
			_marks = mc?.MarksInclusiveOfTime(StartTime, StartTime + TimeSpan);

			// Populate TextClass with start frame, mark frame and endframe
			if (CountDownMode == CountDownMode.MarkCollection && _marks != null)
			{
				var i = 0;
				var currentMark = 0;
				foreach (var mark in _marks)
				{
					TextClass t = new TextClass();

					double markTime = mark.StartTime.TotalMilliseconds - StartTime.TotalMilliseconds;
					t.Frame = (int)markTime / 50;

					t.EndFrame = (int)(mark.EndTime.TotalMilliseconds - StartTime.TotalMilliseconds) / 50;
					if (_textClass.Count == 0)
					{
						t.StartFrame = 0;
					}
					else
					{
						t.StartFrame = ((t.Frame - _textClass[_textClass.Count - 1].Frame) / 2) + _textClass[_textClass.Count - 1].Frame;
						_textClass[_textClass.Count - 1].EndFrame = t.StartFrame;
					}

					if (_marks.Count() == currentMark + 1) t.EndFrame = GetNumberFrames();

					i++;
					_textClass.Add(t);
					currentMark++;
				}
			}

			// Adjusts start and end frames for each mark depending on fade settings.
			for (var i = 0; i < _textClass.Count; i++)
			{
				TextClass text = _textClass[i];
				switch (CountDownFade)
				{
					case CountDownFade.In:
						switch (CountDownDuration)
						{
							case CountDownDuration.AutoFit:
								if (i != 0) text.StartFrame = _textClass[i - 1].Frame;
								text.EndFrame = text.Frame;
								break;
							case CountDownDuration.UserDefined:
								//Gets max and min frame to compare with users Visual time setting and pick the smallest.
								int minFrameOffset = i != 0
									? text.Frame - _textClass[i - 1].Frame
									: text.Frame;
								text.EndFrame = text.Frame;
								text.StartFrame = (int)(text.Frame - Math.Min((double)TimeVisibleLength / 50, minFrameOffset));
								break;
						}
						break;
					case CountDownFade.None:
					case CountDownFade.Out:
						switch (CountDownDuration)
						{
							case CountDownDuration.AutoFit:
								if (i != _textClass.Count - 1) text.EndFrame = _textClass[i + 1].Frame;
								text.StartFrame = text.Frame;
								break;
							case CountDownDuration.UserDefined:
								//Gets max and min frame to compare with users Visual time setting and pick the smallest.
								int maxFrameOffset = i != _textClass.Count - 1
									? _textClass[i + 1].Frame - text.Frame
									: GetNumberFrames() - text.Frame;
								text.StartFrame = text.Frame;
								text.EndFrame = (int)(text.Frame + Math.Min((double)TimeVisibleLength / 50, maxFrameOffset));
								break;
						}
						break;
					case CountDownFade.InOut:
						switch (CountDownDuration)
						{
							case CountDownDuration.UserDefined:
								//Gets max and min frame to compare with users Visual time setting and pick the smallest.
								int maxFrameOffset = i != _textClass.Count - 1
									? _textClass[i + 1].Frame - text.Frame
									: GetNumberFrames() - text.Frame;
								int minFrameOffset = i != 0
									? text.Frame - _textClass[i - 1].Frame
									: text.Frame;
								text.EndFrame = (int)(text.Frame + Math.Min((double)TimeVisibleLength / 100, maxFrameOffset));
								text.StartFrame = (int)(text.Frame - Math.Min((double)TimeVisibleLength / 100, minFrameOffset));
								break;
						}
						break;
				}
			}
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
