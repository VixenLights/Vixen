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

		#region Movement properties

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[PropertyOrder(1)]
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
		[ProviderCategory(@"Movement", 1)]
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
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Position")]
		[ProviderDescription(@"Position")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-100, 100, 1)]
		[PropertyOrder(2)]
		public int Position
		{
			get { return _data.Position; }
			set
			{
				_data.Position = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"PositionX")]
		[ProviderDescription(@"Position")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-100, 100, 1)]
		[PropertyOrder(2)]
		public int PositionX
		{
			get { return _data.PositionX; }
			set
			{
				_data.PositionX = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
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
		[ProviderCategory(@"Text", 2)]
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
		[ProviderCategory(@"Text", 2)]
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
		[ProviderCategory(@"Text", 2)]
		[ProviderDisplayName(@"Scale Text")]
		[ProviderDescription(@"Scale Text")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(3)]
		public int ScaleText
		{
			get { return _data.ScaleText; }
			set
			{
				_data.ScaleText = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Text", 2)]
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
		[ProviderCategory(@"Text", 2)]
		[ProviderDisplayName(@"TextMode")]
		[ProviderDescription(@"TextMode")]
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
		[MergableProperty(false)]
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

		[Value]
		[ProviderCategory(@"Color", 3)]
		[ProviderDisplayName(@"UseBaseColor")]
		[ProviderDescription(@"UseBaseColor")]
		[TypeConverter(typeof(BooleanStringTypeConverter))]
		[BoolDescription("Yes", "No")]
		[PropertyEditor("SelectionEditor")]
		[Browsable(true)]
		[PropertyOrder(2)]
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
		[PropertyOrder(3)]
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

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		protected override void TargetPositioningChanged()
		{
			UpdateScaleTextAttribute();
		}

		private void UpdateAllAttributes()
		{
			UpdateScaleTextAttribute(false);
			UpdateBaseColorAttribute(false);
			UpdatePositionXAttribute(false);
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
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

		private void UpdateScaleTextAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"ScaleText", TargetPositioning == TargetPositioningType.Locations}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdatePositionXAttribute(bool refresh=true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"PositionX", Direction==TextDirection.None}
			};
			SetBrowsable(propertyStates);

			if(refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private int _maxTextSize;

		protected override void SetupRender()
		{
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				// Adjust the font size for Location support, default will ensure when swicthing between string and location that the Font will be the same visual size.
				// Font is further adjusted using the scale text slider.
				double newFontSize = ((StringCount - ((StringCount - Font.Size) / 100) * (100 - ScaleText))) * ((double)BufferHt / StringCount);
				_font = new Font(Font.FontFamily.Name,  (int)newFontSize, Font.Style);
				return;
			}
			_font = Font;
		}

		protected override void CleanUpRender()
		{
			_font = null;
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			using (var bitmap = new Bitmap(BufferWi, BufferHt))
			{
				InitialRender(frame, bitmap);

				// copy to frameBuffer
				for (int x = 0; x < BufferWi; x++)
				{
					for (int y = 0; y < BufferHt; y++)
					{
						CalculatePixel(x, y, bitmap, frame, frameBuffer);
					}
				}
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
			for (int frame = 0; frame < numFrames; frame++)
			{
				double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame)*100)/100;
				using (var bitmap = new Bitmap(BufferWi, BufferHt))
				{
					InitialRender(frame, bitmap);
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
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				var text = TextMode == TextMode.Normal
					? TextLines.Where(x => !String.IsNullOrEmpty(x)).ToList()
					: SplitTextIntoCharacters(TextLines);
				int numberLines = text.Count();

				SizeF textsize = new SizeF(0, 0);

				foreach (string t in text)
				{
					if (!String.IsNullOrEmpty(t))
					{
						var size = graphics.MeasureString(t, _font);
						if (size.Width > textsize.Width)
						{
							textsize = size;
						}
					}
				}
				_maxTextSize = Convert.ToInt32(textsize.Width*.95);
				int maxht = Convert.ToInt32(textsize.Height*numberLines);
				int offsetLeft = (((BufferWi - _maxTextSize)/2)*2 + Position)/2;
				int offsetTop = (((BufferHt - maxht)/2)*2 + Position)/2;
				double intervalPosition = (GetEffectTimeIntervalPosition(frame)*Speed)%1;
				Point point;

				switch (Direction)
				{
					case TextDirection.Left:
						// left
						int leftX = BufferWi - (int) (intervalPosition*(textsize.Width + BufferWi));

						point =
							new Point(Convert.ToInt32(CenterStop ? Math.Max(leftX, (BufferWi - (int) textsize.Width)/2) : leftX), offsetTop);

						DrawText(text, graphics, point);

						break;
					case TextDirection.Right:
						// right
						int rightX = -_maxTextSize + (int) (intervalPosition*(_maxTextSize + BufferWi));

						point =
							new Point(Convert.ToInt32(CenterStop ? Math.Min(rightX, (BufferWi - (int) textsize.Width)/2) : rightX),
								offsetTop);
						DrawText(text, graphics, point);
						break;
					case TextDirection.Up:
						// up
						int upY = BufferHt - (int) (((textsize.Height*numberLines) + BufferHt)*intervalPosition);

						point = new Point(offsetLeft,
							Convert.ToInt32(CenterStop ? Math.Max(upY, (BufferHt - (int) (textsize.Height*numberLines))/2) : upY));
						DrawText(text, graphics, point);
						break;
					case TextDirection.Down:
						// down
						int downY = -(int) (textsize.Height*numberLines) +
						            (int) (((textsize.Height*numberLines) + BufferHt)*intervalPosition);

						point = new Point(offsetLeft,
							Convert.ToInt32(CenterStop
								? Math.Min(downY, (BufferHt - (int) (textsize.Height*numberLines))/2)
								: downY));
						DrawText(text, graphics, point);
						break;
					default:
						// no movement - centered
						point = new Point(((BufferWi - _maxTextSize)/2) + PositionX, offsetTop);
						DrawText(text, graphics, point);
						break;
				}
			}
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

			if (!EmptyColor.Equals(color))
			{
				var hsv = HSV.FromRGB(color);
				hsv.V = hsv.V*level;

				frameBuffer.SetPixel(xCoord, yCoord, hsv);
			}
			else if (TargetPositioning == TargetPositioningType.Locations)
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

				var size = g.MeasureString(text, _font);
				var offset = _maxTextSize - (int)size.Width;
				var offsetPoint = new Point(p.X + offset / 2, p.Y);
				var brushPointX = p.X;
				if (CenterText && TextMode == TextMode.Rotated)
				{
					brushPointX =p.X-offset/2;
				}
				else if (CenterText)
				{
					brushPointX = offsetPoint.X;
				}
				ColorGradient cg = Colors[i % Colors.Count()];
				var brush = new LinearGradientBrush(new Rectangle(brushPointX, p.Y, TextMode==TextMode.Rotated?_maxTextSize:(int)size.Width, (int)size.Height), Color.Black,
					Color.Black, mode) { InterpolationColors = cg.GetColorBlend() };
				
				DrawTextWithBrush(text, brush, g, CenterText?offsetPoint:p);
				brush.Dispose();
				p.Y += (int)size.Height;
				if (TextMode == TextMode.Normal)
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
				var size = g.MeasureString(text, _font);
				var offset = _maxTextSize - (int)size.Width;
				var offsetPoint = new Point(p.X + offset / 2, p.Y);

				ColorGradient cg = Colors[i % Colors.Count()];
				var brush = new LinearGradientBrush(new Rectangle(0, 0, BufferWi, BufferHt),
					Color.Black,
					Color.Black, mode) { InterpolationColors = cg.GetColorBlend() };
				DrawTextWithBrush(text, brush, g, CenterText ? offsetPoint : p);
				brush.Dispose();

				p.Y += (int)size.Height;
				if (TextMode == TextMode.Normal)
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
			g.DrawString(text, _font, brush, p);
		}
	}
}
