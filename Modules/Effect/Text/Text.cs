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
using Vixen.Marks;
using Vixen.Module;
using Vixen.Sys.Attribute;
using Vixen.TypeConverters;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Text
{
	public class Text : PixelEffectBase
	{
		private TextData _data;
		private static Color EmptyColor = Color.FromArgb(0, 0, 0, 0);
		private Font _font;
		private Font _newfont;
		private float _newFontSize;
		private List<string> _text;
		private List<TextClass> _textClass;
		private IEnumerable<IMark> _marks = null;
		private double _directionPosition;
		private double _fade;
		private double _level;
		private int _wordIteration;


		public Text()
		{
			_data = new TextData();
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

		#region Config properties

		[Value]
		[ProviderCategory("Config", 1)]
		[ProviderDisplayName(@"TextTrigger")]
		[ProviderDescription(@"TextTrigger")]
		[PropertyOrder(0)]
		public TextSource TextSource
		{
			get
			{
				return _data.TextSource;
			}
			set
			{
				if (_data.TextSource != value)
				{
					_data.TextSource = value;
					UpdateTextModeAttributes();
					IsDirty = true;
					OnPropertyChanged();
				}
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Mark Collection")]
		[ProviderDescription(@"Mark Collection that has the time position to display each word.")]
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
		[ProviderDisplayName(@"TextDuration")]
		[ProviderDescription(@"TextDuration")]
		[PropertyOrder(2)]
		public TextDuration TextDuration
		{
			get { return _data.TextDuration; }
			set
			{
				_data.TextDuration = value;
				UpdateTextModeAttributes();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"TextFade")]
		[ProviderDescription(@"TextFade")]
		[PropertyOrder(3)]
		public TextFade TextFade
		{
			get { return _data.TextFade; }
			set
			{
				_data.TextFade = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[PropertyOrder(4)]
		public TextDirection Direction
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Iterations")]
		[ProviderDescription(@"Iterations")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(5)]
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
		[ProviderDisplayName(@"Time Visible Length (ms)")]
		[ProviderDescription(@"Shows each word for selected period of time up to the next word.")]
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
		[ProviderDisplayName(@"RepeatText")]
		[ProviderDescription(@"RepeatText")]
		[PropertyOrder(7)]
		public bool RepeatText
		{
			get { return _data.RepeatText; }
			set
			{
				_data.RepeatText = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"DirectionPerWord")]
		[ProviderDescription(@"DirectionPerWord")]
		[PropertyOrder(8)]
		public bool DirectionPerWord
		{
			get { return _data.DirectionPerWord; }
			set
			{
				_data.DirectionPerWord = value;
				UpdateTextModeAttributes();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Movement properties

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"Vertical Offset")]
		[ProviderDescription(@"Vertical Offset")]
		//[NumberRange(-100, 100, 1)]
		[PropertyOrder(0)]
		public Curve YOffsetCurve
		{
			get { return _data.YOffSetCurve; }
			set
			{
				_data.YOffSetCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"Horizontal Offset")]
		[ProviderDescription(@"Horizontal Offset")]
		//[NumberRange(-100, 100, 1)]
		[PropertyOrder(1)]
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

		#region Text properties

		[Value]
		[ProviderCategory(@"Text", 3)]
		[ProviderDisplayName(@"TextLines")]
		[ProviderDescription(@"TextLines")]
		[PropertyOrder(1)]
		public List<string> TextLines
		{
			get { return _data.Text; }
			set
			{
				_data.Text = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Text", 3)]
		[ProviderDisplayName(@"Font")]
		[ProviderDescription(@"Font")]
		[PropertyOrder(2)]
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
		[ProviderCategory(@"Text", 3)]
		[ProviderDisplayName(@"FontScale")]
		[ProviderDescription(@"FontScale")]
		[PropertyOrder(3)]
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
		[ProviderCategory(@"Text", 3)]
		[ProviderDisplayName(@"CenterText")]
		[ProviderDescription(@"CenterText")]
		[PropertyOrder(4)]
		public bool CenterText
		{
			get { return _data.CenterText; }
			set
			{
				_data.CenterText = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Text", 3)]
		[ProviderDisplayName(@"Text Layout")]
		[ProviderDescription(@"Text Layout")]
		[PropertyOrder(5)]
		public TextMode TextMode
		{
			get { return _data.TextMode; }
			set
			{
				_data.TextMode = value;
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
		[ProviderDisplayName(@"GradientMode")]
		[ProviderDescription(@"GradientMode")]
		[PropertyOrder(2)]
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
		[ProviderDisplayName(@"UseBaseColor")]
		[ProviderDescription(@"UseBaseColor")]
		[TypeConverter(typeof(BooleanStringTypeConverter))]
		[BoolDescription("Yes", "No")]
		[PropertyEditor("SelectionEditor")]
		[Browsable(true)]
		[PropertyOrder(3)]
		public override bool UseBaseColor
		{
			get { return _data.UseBaseColor; }
			set
			{
				_data.UseBaseColor = value;
				IsDirty = true;
				UpdateBaseColorAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 3)]
		[ProviderDisplayName(@"BaseColor")]
		[ProviderDescription(@"BaseColor")]
		[PropertyOrder(4)]
		public override Color BaseColor
		{
			get { return _data.BaseColor; }
			set
			{
				_data.BaseColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

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

		[Value]
		[ProviderCategory(@"Brightness", 4)]
		[ProviderDisplayName(@"BaseBrightness")]
		[ProviderDescription(@"BaseBrightness")]
		[PropertyOrder(1)]
		public override Curve BaseLevelCurve
		{
			get { return _data.BaseLevelCurve; }
			set
			{
				_data.BaseLevelCurve = value;
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
				_data = value as TextData;
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
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/text/"; }
		}

		#endregion

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private void UpdateAllAttributes()
		{
			UpdateBaseColorAttribute(false);
			UpdatePositionXAttribute(false);
			UpdateTextModeAttributes(false);
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
		}
		private void UpdateTextModeAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(10)
			{
				{"MarkCollectionId", TextSource != TextSource.None},

				{"DirectionPerWord", TextSource != TextSource.None && Direction < TextDirection.Rotate},

				{"TimeVisibleLength", TextSource != TextSource.None && TextDuration == TextDuration.UserDefined},

				{"TextDuration", TextSource != TextSource.None},

				{"RepeatText", TextSource == TextSource.MarkCollection},

				{"TextFade", TextSource != TextSource.None},

				{"Speed", (TextSource == TextSource.None || !DirectionPerWord) && Direction < TextDirection.Rotate},

				{"TextMode", TextSource == TextSource.None},

				{"TextLines", TextSource != TextSource.MarkCollectionLabels},

				{"CycleColor", TextSource != TextSource.None}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateBaseColorAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{"BaseColor", UseBaseColor},

				{"BaseLevelCurve", UseBaseColor}
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
				case TextDirection.Left:
				case TextDirection.Right:
					hideXOffsetCurve = true;
					break;
				case TextDirection.Up:
				case TextDirection.Down:
					hideYOffsetCurve = true;
					break;
			}
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3)
			{
				{"XOffsetCurve", !hideXOffsetCurve},

				{"YOffsetCurve", !hideYOffsetCurve},
				
				{"AngleCurve", Direction == TextDirection.Rotate},

				{"DirectionPerWord", TextSource != TextSource.None && Direction < TextDirection.Rotate},

				{"Speed", Direction < TextDirection.Rotate},

				{"CenterStop", Direction < TextDirection.Rotate}
			};
			SetBrowsable(propertyStates);

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private int _maxTextSize;

		protected override void SetupRender()
		{
			_textClass = new List<TextClass>();
			_text = TextMode == TextMode.Normal || TextSource == TextSource.MarkCollection
				? TextLines.Where(x => !String.IsNullOrEmpty(x)).ToList()
				: SplitTextIntoCharacters(TextLines);

			if (TextSource != TextSource.None) SetupMarks();

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
			_text.Clear();
			_textClass = null;
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			var bufferHt = BufferHt;
			var bufferWi = BufferWi;
			using (var bitmap = new Bitmap(bufferWi, bufferHt))
			{
				InitialRender(frame, bitmap, bufferHt, bufferWi);
				if (_text.Count == 0 && !UseBaseColor) return;
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
				_level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame)*100)/100;
				using (var bitmap = new Bitmap(BufferWi, BufferHt))
				{
					InitialRender(frame, bitmap, bufferHt, bufferWi);
					FastPixel.FastPixel fp = new FastPixel.FastPixel(bitmap);
					fp.Lock();
					if (_text.Count == 0 && !UseBaseColor) continue;
					foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
					{
						foreach (var elementLocation in elementLocations)
						{
							CalculateLocationPixel(elementLocation.X, elementLocation.Y, ref bufferHt, fp, frameBuffer);
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
				// Sets Fade level and text position offsets.
				SetFadePositionLevel(frame);

				if (_text.Count == 0) return; // No point going any further as there is no text to be display for this frame.

				int numberLines = _text.Count();
				SizeF textsize = new SizeF(0, 0);

				//Adjust Font Size based on the Font scaling factor
				_newFontSize = _font.SizeInPoints * (CalculateFontScale(intervalPosFactor) / 100);
				_newfont = new Font(Font.FontFamily.Name, _newFontSize, Font.Style);

				foreach (string t in _text)
				{
					if (!String.IsNullOrEmpty(t))
					{
						var size = graphics.MeasureString(t, _newfont);
						if (size.Width > textsize.Width)
						{
							textsize = size;
						}
					}
				}

				_maxTextSize = Convert.ToInt32(textsize.Width * .95);
				int maxht = Convert.ToInt32(textsize.Height * numberLines);

				int xOffset = CalculateXOffset(intervalPosFactor);
				int yOffset = CalculateYOffset(intervalPosFactor);

				//Rotate the text based off the angle setting
				if (Direction == TextDirection.Rotate)
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
					case TextDirection.Left:
					case TextDirection.Right:
						xOffset = 0;
						break;
					case TextDirection.Up:
					case TextDirection.Down:
						yOffset = 0;
						break;
				}

				int offsetLeft = (((bufferWi - _maxTextSize) / 2) * 2 + xOffset) / 2;
				int offsetTop = (((bufferHt - maxht) / 2) * 2 + yOffset) / 2;
				Point point;

				switch (Direction)
				{
					case TextDirection.Left:
						// left
						int leftX = bufferWi - (int) (_directionPosition * (textsize.Width + bufferWi));
						point =
							new Point(Convert.ToInt32(CenterStop ? Math.Max(leftX, (bufferWi - (int) textsize.Width) / 2) : leftX),
								offsetTop);
						break;
					case TextDirection.Right:
						// right
						int rightX = -_maxTextSize + (int) (_directionPosition * (_maxTextSize + bufferWi));
						point =
							new Point(Convert.ToInt32(CenterStop ? Math.Min(rightX, (bufferWi - (int) textsize.Width) / 2) : rightX),
								offsetTop);
						break;
					case TextDirection.Up:
						// up
						int upY = bufferHt - (int) (((textsize.Height * numberLines) + bufferHt) * _directionPosition);
						point = new Point(offsetLeft,
							Convert.ToInt32(CenterStop ? Math.Max(upY, (bufferHt - (int) (textsize.Height * numberLines)) / 2) : upY));
						break;
					case TextDirection.Down:
						// down
						int downY = -(int) (textsize.Height * numberLines) +
						            (int) (((textsize.Height * numberLines) + bufferHt) * _directionPosition);
						point = new Point(offsetLeft,
							Convert.ToInt32(CenterStop
								? Math.Min(downY, (bufferHt - (int) (textsize.Height * numberLines)) / 2)
								: downY));
						break;
					default:
						// no movement - centered
						point = new Point((bufferWi - _maxTextSize) / 2 + xOffset, offsetTop);
						break;
				}

				DrawText(_text, graphics, point);

			}
		}

		private void SetFadePositionLevel(int frame)
		{
			_directionPosition = (GetEffectTimeIntervalPosition(frame) * Speed) % 1;
			_fade = 1;
			if (TextSource != TextSource.None)
			{
				bool clearText = true;
				for (var i = 0; i < _textClass.Count; i++)
				{
					TextClass text = _textClass[i];
					if (frame >= text.StartFrame && frame < text.EndFrame)
					{
						_wordIteration = i;
						_text = new List<string>(text.Text);
						switch (TextFade)
						{
							case TextFade.In:
								_fade = TextDuration != TextDuration.MarkDuration
									? (double) (frame - text.StartFrame) / (text.Frame - text.StartFrame)
									: (double) (frame - text.Frame) / (text.EndFrame - text.Frame);
								break;
							case TextFade.Out:
								_fade = 1 - (double)(frame - text.Frame) / (text.EndFrame - text.Frame);
								break;
							case TextFade.InOut:
								if (TextDuration != TextDuration.MarkDuration)
								{
									_fade = frame < text.Frame
										? (double)(frame - text.StartFrame) / (text.Frame - text.StartFrame)
										: 1 - (double)(frame - text.Frame) / (text.EndFrame - text.Frame);
								}
								else
								{
									_fade = frame < text.Frame + (text.EndFrame - text.Frame) / 2
										? (double)(frame - text.Frame) / (text.EndFrame - text.Frame)
										: 1 - (double)(frame - text.Frame) / (text.EndFrame - text.Frame);
								}
								break;
							case TextFade.None:
								_fade = 1;
								break;
						}
						if (DirectionPerWord) _directionPosition = (double)(frame - text.StartFrame) / (text.EndFrame - text.StartFrame);
						clearText = false;
						break;
					}
				}
				if (clearText) _text.Clear();
			}
		}

		private int CalculateXOffset(double intervalPos)
		{
			return (int)ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), 100, -100);
		}

		private int CalculateYOffset(double intervalPos)
		{
			return (int)ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), -100, 100);
		}

		private int CalculateAngle(double intervalPos)
		{
			return (int)ScaleCurveToValue(AngleCurve.GetValue(intervalPos), 360, 0);
		}

		private float CalculateFontScale(double intervalPos)
		{
			return (float)ScaleCurveToValue(FontScaleCurve.GetValue(intervalPos), 100, 1);
		}

		private void CalculatePixel(int x, int y, ref int bufferHt, FastPixel.FastPixel bitmap, IPixelFrameBuffer frameBuffer)
		{
			Color color = bitmap.GetPixel(x, bufferHt - y - 1);

			if (!EmptyColor.Equals(color))
			{
				frameBuffer.SetPixel(x, y, color);
			}
		}

		private void CalculateLocationPixel(int x, int y, ref int bufferHt, FastPixel.FastPixel bitmap, IPixelFrameBuffer frameBuffer)
		{
			int yCoord = y;
			int xCoord = x;
			
			//Flip me over so and offset my coordinates I can act like the string version
			y = Math.Abs((BufferHtOffset - y) + (bufferHt - 1 + BufferHtOffset));
			y = y - BufferHtOffset;
			x = x - BufferWiOffset;
			
			Color color = bitmap.GetPixel(x, bufferHt - y - 1);

			if (!EmptyColor.Equals(color))
			{
				frameBuffer.SetPixel(xCoord, yCoord, color);
			}
			else
			{
				//Set me to my base color or transparent
				frameBuffer.SetPixel(xCoord, yCoord, UseBaseColor ? BaseColor : Color.Transparent);
			}
		}

		private List<string> SplitTextIntoCharacters(List<string> textLines)
		{
			List<string> splitText = new List<string>();
			foreach (var textLine in textLines)
			{
				splitText.AddRange(textLine.ToCharArray().Select(c => c.ToString()));
				splitText.Add(Environment.NewLine);
			}
			splitText.RemoveAt(splitText.Count-1);
			return splitText;
		}

		private void DrawText(IEnumerable<String> textLines, Graphics g, Point p)
		{
			switch (GradientMode)
			{
				case GradientMode.AcrossElement:
					DrawTextAcrossGradient(textLines, g, p, LinearGradientMode.Horizontal);
					break;
				case GradientMode.AcrossText:
					DrawTextWithGradient(textLines, g, p, LinearGradientMode.Horizontal);
					break;
				case GradientMode.VerticalAcrossElement:
					DrawTextAcrossGradient(textLines, g, p, LinearGradientMode.Vertical);
					break;
				case GradientMode.VerticalAcrossText:
					DrawTextWithGradient(textLines, g, p, LinearGradientMode.Vertical);
					break;
				case GradientMode.DiagonalAcrossElement:
					DrawTextAcrossGradient(textLines, g, p, LinearGradientMode.ForwardDiagonal);
					break;
				case GradientMode.DiagonalAcrossText:
					DrawTextWithGradient(textLines, g, p, LinearGradientMode.ForwardDiagonal);
					break;
				case GradientMode.BackwardDiagonalAcrossElement:
					DrawTextAcrossGradient(textLines, g, p, LinearGradientMode.BackwardDiagonal);
					break;
				case GradientMode.BackwardDiagonalAcrossText:
					DrawTextWithGradient(textLines, g, p, LinearGradientMode.BackwardDiagonal);
					break;
			}
		}

		private void DrawTextWithGradient(IEnumerable<String> textLines, Graphics g, Point p, LinearGradientMode mode)
		{
			int i = 0;
			foreach (var text in textLines)
			{
				var size = g.MeasureString(text, _newfont);
				var offset = _maxTextSize - (int)size.Width;
				var offsetPoint = new Point(p.X + offset / 2, p.Y);
				var brushPointX = p.X;
				if (CenterText && TextMode == TextMode.Rotated && TextSource == TextSource.None)
				{
					brushPointX =p.X-offset/2;
				}
				else if (CenterText)
				{
					brushPointX = offsetPoint.X;
				}

				ColorGradient cg = TextSource != TextSource.None && CycleColor ? Colors[_wordIteration % Colors.Count()] : Colors[i % Colors.Count()];

				if (_level < 1 || TextFade != TextFade.None) cg = GetNewGolorGradient(cg);

				var brush = new LinearGradientBrush(new Rectangle(brushPointX, p.Y, TextMode==TextMode.Rotated && TextSource == TextSource.None ? _maxTextSize:(int)size.Width, (int)size.Height), Color.Black,
					Color.Black, mode) { InterpolationColors = cg.GetColorBlend() };
				
				DrawTextWithBrush(text, brush, g, CenterText?offsetPoint:p);
				brush.Dispose();
				p.Y += (int)size.Height;
				if (TextMode == TextMode.Normal || TextSource == TextSource.MarkCollection)
				{
					i++;
				}
				else
				{
					if (text == Environment.NewLine)
					{
						i++;
					}	
				}
			}
		}

		private void DrawTextAcrossGradient(IEnumerable<String> textLines, Graphics g, Point p, LinearGradientMode mode)
		{
			int i = 0;
			foreach (var text in textLines)
			{
				var size = g.MeasureString(text, _newfont);
				var offset = _maxTextSize - (int)size.Width;
				var offsetPoint = new Point(p.X + offset / 2, p.Y);
				
				ColorGradient cg = TextSource != TextSource.None && CycleColor ? Colors[_wordIteration % Colors.Count()] : Colors[i % Colors.Count()];
				
				if (_level < 1 || TextFade != TextFade.None) cg = GetNewGolorGradient(cg);
				var brush = new LinearGradientBrush(new Rectangle(0, 0, BufferWi, BufferHt),
					Color.Black,
					Color.Black, mode) { InterpolationColors = cg.GetColorBlend() };
				DrawTextWithBrush(text, brush, g, CenterText ? offsetPoint : p);
				brush.Dispose();

				p.Y += (int)size.Height;
				if (TextMode == TextMode.Normal || TextSource == TextSource.MarkCollection)
				{
					i++;
				}
				else
				{
					if (text == Environment.NewLine)
					{
						i++;
					}
				}
			}
		}

		private void DrawTextWithBrush(string text, Brush brush, Graphics g, Point p)
		{
			g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
			g.DrawString(text, _newfont, brush, p);
		}
		
		private ColorGradient GetNewGolorGradient(ColorGradient cg)
		{
			cg = new ColorGradient(cg);
			foreach (var color in cg.Colors)
			{
				HSV hsv = HSV.FromRGB(color.Color.ToRGB());
				if (TextFade != TextFade.None) hsv.V *= _fade;
				hsv.V *= _level;
				color.Color = XYZ.FromRGB(hsv.ToRGB());
			}
			return cg;
		}

		// Text Class
		public class TextClass
		{
			public List<string> Text = new List<string>();
			public int StartFrame;
			public int Frame;
			public int EndFrame;
		}

		private void SetupMarks()
		{
			IMarkCollection mc = MarkCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
			_marks = mc?.MarksInclusiveOfTime(StartTime, StartTime + TimeSpan);

			// Populate TextClass with start frame, mark frame and endframe
			if ((_text.Count > 0 || TextSource == TextSource.MarkCollectionLabels) && _marks != null)
			{
				var i = 0;
				var currentMark = 0; 
				foreach (var mark in _marks)
				{
					TextClass t = new TextClass();
					if (TextSource == TextSource.MarkCollectionLabels)
					{
						t.Text.Add(mark.Text);
					}
					else
					{
						foreach (string t1 in _text)
						{
							if (t1.Split(' ').Length > i) t.Text.Add(t1.Split(' ')[i]);
						}
					}

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
						if (TextDuration != TextDuration.MarkDuration)
						{
							_textClass[_textClass.Count - 1].EndFrame = t.StartFrame;
						}
					}

					if (_marks.Count() == currentMark + 1 && TextDuration != TextDuration.MarkDuration) t.EndFrame = GetNumberFrames();

					i++;
					if (t.Text.Count == 0 && RepeatText)
					{
						t.Text = _textClass[0].Text;
						i = 1;
					}
					_textClass.Add(t);
					currentMark++;
				}
			}

			// Adjusts start and end frames for each mark depending on fade settings.
			for (var i = 0; i < _textClass.Count; i++)
			{
				TextClass text = _textClass[i];
				if (text.Text.Count == 0) text.Text.Add(" ");
				else if (text.Text[0] == "") text.Text[0] = " ";
				switch (TextFade)
				{
					case TextFade.In:
						switch (TextDuration)
						{
							case TextDuration.AutoFit:
								if (i != 0) text.StartFrame = _textClass[i - 1].Frame;
								text.EndFrame = text.Frame;
								break;
							case TextDuration.MarkDuration:
								text.StartFrame = text.Frame;
								break;
							case TextDuration.UserDefined:
								//Gets max and min frame to compare with users Visual time setting and pick the smallest.
								int minFrameOffset = i != 0
									? text.Frame - _textClass[i - 1].Frame
									: text.Frame;
								text.EndFrame = text.Frame;
								text.StartFrame = (int)(text.Frame - Math.Min((double)TimeVisibleLength / 50, minFrameOffset));
								break;
						}
						break;
					case TextFade.None:
					case TextFade.Out:
						switch (TextDuration)
						{
							case TextDuration.AutoFit:
								if (i != _textClass.Count - 1) text.EndFrame = _textClass[i + 1].Frame;
								text.StartFrame = text.Frame;
								break;
							case TextDuration.MarkDuration:
								text.StartFrame = text.Frame;
								break;
							case TextDuration.UserDefined:
								//Gets max and min frame to compare with users Visual time setting and pick the smallest.
								int maxFrameOffset = i != _textClass.Count - 1
									? _textClass[i + 1].Frame - text.Frame
									: GetNumberFrames() - text.Frame;
								text.StartFrame = text.Frame;
								text.EndFrame = (int) (text.Frame + Math.Min((double)TimeVisibleLength / 50, maxFrameOffset));
								break;
						}
						break;
					case TextFade.InOut:
						switch (TextDuration)
						{
							case TextDuration.MarkDuration:
								text.StartFrame = text.Frame;
								break;
							case TextDuration.UserDefined:
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
			if (TextSource != TextSource.None)
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
			switch (TextSource)
			{
				case TextSource.MarkCollectionLabels:
					if (_marks != null && _marks.Any())
					{
						_wordIteration = -1;
						foreach (var mark in _marks)
						{
							_wordIteration = TextSource != TextSource.None && CycleColor ? _wordIteration + 1 : 0;
							if (mark.Text == "") continue;
							var startX = (int) ((mark.StartTime.Ticks - StartTime.Ticks) / (double) TimeSpan.Ticks * clipRectangle.Width);
							DrawText(g, clipRectangle, mark.Text, mode, startX);
						}
					}
					break;
				case TextSource.MarkCollection:
					if (_marks != null && TextLines[0] != "" && _marks.Any())
					{
						string[] text = TextLines[0].Split();
						if (TextLines[0] == "") return;
						int i = 0;
						_wordIteration = 0;
						foreach (var mark in _marks)
						{
							var startX = (int)((mark.StartTime.Ticks - StartTime.Ticks) / (double)TimeSpan.Ticks * clipRectangle.Width);
							DrawText(g, clipRectangle, text[i], mode, startX);
							i++;
							if (TextSource != TextSource.None && CycleColor) _wordIteration++;
							if (RepeatText)
							{
								if (i % text.Length == 0) i = 0;
							}
							else if (text.Length <= i) return;
						}
					}
					break;
				default:
					if (TextLines[0] == "") return;
					DrawText(g, clipRectangle, TextLines[0], mode, 2);
					break;
			}
		}

		private void DrawText(Graphics g, Rectangle clipRectangle, string displayedText, LinearGradientMode mode, int startX)
		{
			Font adjustedFont = Vixen.Common.Graphics.GetAdjustedFont(g, displayedText, clipRectangle, Font.Name, 48, Font);
			SizeF adjustedSizeNew = g.MeasureString(displayedText, adjustedFont);
			int adjustedStartPosition = TextSource == TextSource.None ? startX : clipRectangle.X + startX;
			var brush = new LinearGradientBrush(
					new Rectangle(adjustedStartPosition, 2, (int)adjustedSizeNew.Width, (int)adjustedSizeNew.Height),
					Color.Black,
					Color.Black, mode)
				{ InterpolationColors = Colors[_wordIteration % Colors.Count].GetColorBlend() };
			g.DrawString(displayedText, adjustedFont, brush, adjustedStartPosition, 2);
		}

		public override bool ForceGenerateVisualRepresentation { get { return true; } }
	}
}
