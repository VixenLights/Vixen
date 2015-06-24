using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.Effect.Pixel;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Text
{
	public class Text : PixelEffectBase
	{
		private TextData _data;

		public Text()
		{
			_data = new TextData();
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
		[ProviderCategory(@"Config", 1)]
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
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
		[ProviderCategory(@"Config", 1)]
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
		[ProviderCategory(@"Config", 1)]
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"FitTime")]
		[ProviderDescription(@"FitTime")]
		[PropertyOrder(4)]
		public bool FitToTime
		{
			get { return _data.FitToTime; }
			set
			{
				_data.FitToTime = value;
				IsDirty = true;
				UpdateSpeedAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
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
		[ProviderDisplayName(@"TextLine1")]
		[ProviderDescription(@"TextLine")]
		[PropertyOrder(1)]
		public string Line1
		{
			get { return _data.Line1; }
			set
			{
				_data.Line1 = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Text", 2)]
		[ProviderDisplayName(@"TextLine2")]
		[ProviderDescription(@"TextLine")]
		[PropertyOrder(2)]
		public string Line2
		{
			get { return _data.Line2; }
			set
			{
				_data.Line2 = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Text", 2)]
		[ProviderDisplayName(@"TextLine3")]
		[ProviderDescription(@"TextLine")]
		[PropertyOrder(3)]
		public string Line3
		{
			get { return _data.Line3; }
			set
			{
				_data.Line3 = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Text", 2)]
		[ProviderDisplayName(@"TextLine4")]
		[ProviderDescription(@"TextLine")]
		[PropertyOrder(4)]
		public string Line4
		{
			get { return _data.Line4; }
			set
			{
				_data.Line4 = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Text", 2)]
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

		#endregion

		#region Color properties

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
		[ProviderDisplayName(@"Color")]
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

		private void UpdateAllAttributes()
		{
			UpdateSpeedAttribute(false);
			UpdatePositionXAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		private void UpdateSpeedAttribute(bool refresh=true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"Speed", !FitToTime}
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

		protected override void RenderEffect(int frame)
		{
			using (var bitmap = new Bitmap(BufferWi, BufferHt))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					
					var line = new[] { Line1, Line2, Line3, Line4 };
					var textLines = new List<String>();
					
					int numberLines=0;
					
					SizeF textsize = new SizeF(0,0);
					
					foreach (string t in line)
					{
						if(!String.IsNullOrEmpty(t))
						{
							numberLines++;
							var size = graphics.MeasureString(t, Font);
							if (size.Width > textsize.Width)
							{
								textsize = size;
							}
							textLines.Add(t);
							continue;
						}
						break;
					}
					int maxTextSize = Convert.ToInt32(textsize.Width*.95);
					int maxht = Convert.ToInt32(textsize.Height * numberLines);
					int xlimit = (BufferWi + maxTextSize) * 8 + 1;
					int ylimit = (BufferHt + maxht) * 8 + 1;
					int offsetLeft = (((BufferWi - maxTextSize) / 2) * 2 + Position) / 2;
					int offsetTop = (((BufferHt - maxht)/2)*2 + Position) / 2;
					double intervalPosition = GetEffectTimeIntervalPosition(frame);

					int i = 0;
					
					Point point;
					
					switch (Direction)
					{
						case TextDirection.Left:
							// left
							int leftX;
							if (FitToTime)
							{
								leftX = BufferWi - (int)(intervalPosition * (textsize.Width + BufferWi));
							}
							else
							{
								leftX = BufferWi - (Speed * frame) % xlimit / 8;
							}
							
							point =
								new Point(Convert.ToInt32(CenterStop ? Math.Max(leftX, (BufferWi - (int)textsize.Width) / 2) : leftX), offsetTop);

							DrawText(textLines, graphics, point);
							
							break;
						case TextDirection.Right:
							// right
							int rightX;
							if (FitToTime)
							{
								rightX = -maxTextSize + (int)(intervalPosition * (maxTextSize + BufferWi));
							}
							else
							{
								rightX = (Speed * frame) % xlimit / 8 - BufferWi;
							}
							point =
								new Point(Convert.ToInt32(CenterStop ? Math.Min(rightX, (BufferWi - (int)textsize.Width) / 2) : rightX), offsetTop);
							DrawText(textLines, graphics, point);
							break;
						case TextDirection.Up:
							// up

							int upY;
							if (FitToTime)
							{
								upY = BufferHt - (int)(((textsize.Height * numberLines) + BufferHt) * intervalPosition);
							}
							else
							{
								upY = BufferHt - (Speed * frame) % ylimit / 8;
							}

							point = new Point(offsetLeft,
								Convert.ToInt32(CenterStop ? Math.Max(upY, (BufferHt - (int)(textsize.Height * numberLines)) / 2): upY));
							DrawText(textLines, graphics, point);
							break;
						case TextDirection.Down:
							// down
							int downY;
							if (FitToTime)
							{
								downY = -(int)(textsize.Height * numberLines) + (int)(((textsize.Height * numberLines) + BufferHt) * intervalPosition);
							}
							else
							{
								downY = (Speed * frame) % ylimit / 8 - BufferHt;
							}
							point = new Point(offsetLeft,
								Convert.ToInt32(CenterStop
									? Math.Min(downY, (BufferHt - (int)(textsize.Height * numberLines)) / 2)
									: downY));
							DrawText(textLines, graphics, point);
							break;
						default:
							// no movement - centered
							point = new Point(((BufferWi-maxTextSize)/2)+PositionX, offsetTop);
							DrawText(textLines, graphics, point);
							break;
					}

					// copy to buffer
					for (int x = 0; x < BufferWi; x++)
					{
						for (int y = 0; y < BufferHt; y++)
						{
							Color color = bitmap.GetPixel(x, BufferHt - y - 1);
							SetPixel(x, y, color);
						}
					}
						
						
				}

			}
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
				var size = g.MeasureString(text, Font);
				ColorGradient cg = Colors[i % Colors.Count()];
				var brush = new LinearGradientBrush(new Rectangle(p.X, p.Y, (int)size.Width, (int)size.Height), Color.Black,
					Color.Black, mode) { InterpolationColors = cg.GetColorBlend() };
				DrawTextWithBrush(text, brush, g, p, mode);
				brush.Dispose();
				p.Y += (int)size.Height;
				i++;
			}
			
		}

		private void DrawTextAcrossGradient(IEnumerable<String> textLines, Graphics g, Point p, LinearGradientMode mode)
		{
			int i = 0;
			foreach (var text in textLines)
			{
				var size = g.MeasureString(text, Font);
				ColorGradient cg = Colors[i % Colors.Count()];
				var brush = new LinearGradientBrush(new Rectangle(0, p.Y, BufferWi, (int)g.MeasureString(text, Font).Height),
					Color.Black,
					Color.Black, mode) { InterpolationColors = cg.GetColorBlend() };
				DrawTextWithBrush(text, brush, g, p, mode);
				brush.Dispose();
				p.Y += (int)size.Height;
				i++;
			}
		}

		private void DrawTextWithBrush(string text, Brush brush, Graphics g, Point p, LinearGradientMode mode)
		{
			g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
			g.DrawString(text, Font, brush, p);
		}
	}
}
