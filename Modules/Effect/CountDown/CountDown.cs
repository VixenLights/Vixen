using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
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
				if (!Colors.CheckLibraryReference())
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
		[ProviderDisplayName(@"CountDownTime")]
		[ProviderDescription(@"CountDownTime")]
		[PropertyOrder(3)]
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
		[PropertyOrder(4)]
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
		[PropertyOrder(5)]
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
		public ColorGradient Colors
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
			UpdateCountDownTypeAttributes(false);
			UpdateStringOrientationAttributes();
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
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			using (var bitmap = new Bitmap(BufferWi, BufferHt))
			{
				InitialRender(frame, bitmap);
				_level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
				// copy to frameBuffer
				for (int x = 0; x < BufferWi; x++)
				{
					for (int y = 0; y < BufferHt; y++)
					{
						CalculatePixel(x, y, bitmap, frameBuffer);
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
				_level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
				using (var bitmap = new Bitmap(BufferWi, BufferHt))
				{
					InitialRender(frame, bitmap);
					foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
					{
						foreach (var elementLocation in elementLocations)
						{
							CalculatePixel(elementLocation.X, elementLocation.Y, bitmap, frameBuffer);
						}
					}
				}
			}
		}
		
		private void InitialRender(int frame, Bitmap bitmap)
		{
			var intervalPos = GetEffectTimeIntervalPosition(frame);
			var intervalPosFactor = intervalPos * 100;
			var textAngle = CalculateAngle(intervalPosFactor);

			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				SetFadePositionLevel(frame);
				
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
			_text = GetCountDownTime(frame);

			var totalFrames = GetNumberFrames();
			int startFrame = 0;
			if (CountDownType == CountDownType.Effect) startFrame = (int)(((decimal)totalFrames / 20 - Math.Floor((decimal)totalFrames / 20)) * 20);
				
			switch (CountDownFade)
			{
				case CountDownFade.In:
					_fade = (double) 1 / 20 * ((frame - startFrame) % 20);
					if (_fade < 0) _fade += 1;
					break;
				case CountDownFade.Out:
					_fade = 1 - (double) 1 / 20 * ((frame - startFrame - 1) % 20);
					if (_fade > 1) _fade -= 1;
						break;
				case CountDownFade.InOut:
					_fade = (frame - startFrame) % 20;
					if (_fade < 0) _fade = -_fade;
					if (_fade < 10)
					{
						_fade = (double) 1 / 20 * _fade;
						if (_fade < 0) _fade += 1;
					}
					else
					{
						_fade = 1 - (double)1 / 20 * _fade;
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
						(int) Math.Ceiling((Convert.ToInt16(CountDownTime) * 1000 - (double) (frame * 50)) / 1000);
					if (CountDownFade == CountDownFade.In) countDownNumber -= 1;
					break;
				case CountDownType.CountUp:
					countDownNumber =
						(int)Math.Floor((Convert.ToInt16(CountDownTime) * 1000 + (double)(frame * 50)) / 1000);
					if (CountDownFade == CountDownFade.In) countDownNumber += 1;
					break;
				default:
					countDownNumber = (int) Math.Ceiling(GetRemainingTime(frame) / 1000);
					break;
			}

			if (countDownNumber > 60 && TimeFormat == TimeFormat.Minutes)
			{
				TimeSpan time = TimeSpan.FromSeconds(countDownNumber);
				return time.ToString(@"m\:ss");
			}
			return countDownNumber.ToString();
		}

		private void CalculatePixel(int x, int y, Bitmap bitmap, IPixelFrameBuffer frameBuffer)
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
			ColorGradient cg = _level < 1 || CountDownFade != CountDownFade.None ? GetNewGolorGradient() : new ColorGradient(Colors);
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
			ColorGradient cg = _level < 1 || CountDownFade != CountDownFade.None ? GetNewGolorGradient() : new ColorGradient(Colors);
			var brush = new LinearGradientBrush(new Rectangle(0, 0, BufferWi, BufferHt),
					Color.Black,
					Color.Black, mode)
				{InterpolationColors = cg.GetColorBlend()};
			DrawTextWithBrush(text, brush, g, offsetPoint);
			brush.Dispose();
			p.Y += (int) size.Height;
		}

		private ColorGradient GetNewGolorGradient()
		{
			ColorGradient cg = new ColorGradient(Colors);
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
								TimeSpan time = TimeSpan.FromSeconds(countTime);
								displayTime = time.ToString(@"m\:ss");
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
								TimeSpan time = TimeSpan.FromSeconds(countTime);
								displayTime = time.ToString(@"m\:ss");
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
					bool isInt = ((double)totalFrames / 20) == (int)(totalFrames / 20);
					int startTick = (int)((double)clipRectangle.Width / totalFrames * (int)((totalFrames / 20 - Math.Floor(totalFrames / 20)) * 20));
					countTime = (int)Math.Ceiling(totalFrames * .050);
					if (!isInt) countTime--;
					secondTicks = (int)((double)clipRectangle.Width / (int)TimeSpan.Ticks * 10000000);
					for ( int i = 0; i < clipRectangle.Width; i++)
					{
						if (i % secondTicks == 0)
						{
							if (countTime > 60 && TimeFormat == TimeFormat.Minutes)
							{
								displayTime = TimeSpan.FromSeconds(countTime).ToString(@"m\:ss");
								if (i == 0 && startTick > 2) displayTime1 = TimeSpan.FromSeconds(countTime + 1).ToString(@"m\:ss");
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

		private void DrawText(Graphics g, Rectangle clipRectangle, string displayedText, LinearGradientMode mode, int startX)
		{
			Font adjustedFont = Vixen.Common.Graphics.GetAdjustedFont(g, displayedText, clipRectangle, Font.Name, 48);
			SizeF adjustedSizeNew = g.MeasureString(displayedText, adjustedFont);
			var brush = new LinearGradientBrush(
					new Rectangle(0, 0, (int)adjustedSizeNew.Width, (int)adjustedSizeNew.Height),
					Color.Black,
					Color.Black, mode)
			{ InterpolationColors = Colors.GetColorBlend() };
			g.DrawString(displayedText, adjustedFont, brush, clipRectangle.X + startX, 2);
		}

		public override bool ForceGenerateVisualRepresentation { get { return true; } }
	}
}
