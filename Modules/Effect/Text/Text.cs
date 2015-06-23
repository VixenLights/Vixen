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
			UpdateSpeedAttribute();
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

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as TextData;
				UpdateSpeedAttribute();
				IsDirty = true;
			}
		}

		private void UpdateSpeedAttribute()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"Speed", !FitToTime}
			};
			SetBrowsable(propertyStates);
			TypeDescriptor.Refresh(this);
		}

		protected override void RenderEffect(int frame)
		{
			using (var bitmap = new Bitmap(BufferWi, BufferHt))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					var line = new[] {Line1, Line2, Line3, Line4};
					int i = 0;
					string msg = "";
					var stringMeasures = new[]
					{
						Convert.ToInt32(graphics.MeasureString(Line1, Font).Width),
						Convert.ToInt32(graphics.MeasureString(Line2, Font).Width),
						Convert.ToInt32(graphics.MeasureString(Line3, Font).Width),
						Convert.ToInt32(graphics.MeasureString(Line4, Font).Width)
					};
					int maxtextsize = stringMeasures.Max();
					int maxIndex = stringMeasures.ToList().IndexOf(maxtextsize);
					int numberLines;
					SizeF textsize = graphics.MeasureString(line[maxIndex], Font);
					if (Line4 != "")
					{
						numberLines = 4;
					}
					else
					{
						if (Line3 != "")
						{
							numberLines = 3;
						}
						else
						{
							numberLines = Line2 != "" ? 2 : 1;
						}
					}
					do
					{

						//using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, BufferWi, BufferHt), Color.Black, Color.Black, LinearGradientMode.Horizontal))//(Colors[i % Colors.Count()].GetColorAt(0)))
						//{
						
							switch (i)
							{
								case 0:
									msg = line[i];
									break;
								case 1:
									msg = "\n" + line[i];
									break;
								case 2:
									msg = "\n\n" + line[i];
									break;
								case 3:
									msg = "\n\n\n" + line[i];
									break;
							}
							int maxwidth = Convert.ToInt32(textsize.Width);
							int maxht = Convert.ToInt32(textsize.Height*numberLines);
							int xlimit = (BufferWi + maxwidth)*8 + 1;
							int ylimit = (BufferHt + maxht)*8 + 1;
							int offsetLeft = (Position*maxwidth/100) - maxwidth/2;
							int offsetTop = maxht/2 - (Position*maxht/100);
							double percent = GetEffectTimeIntervalPosition(frame);
							Point point;
							ColorGradient cg = Colors[i % Colors.Count()];
							switch (Direction)
							{
								case TextDirection.Left:
									// left
									int leftX;
									if (FitToTime)
									{
										leftX = BufferWi - (int) (percent*(textsize.Width + BufferWi));
									}
									else
									{
										leftX = BufferWi - (Speed*frame)%xlimit/8;
									}
									point =
										new Point(Convert.ToInt32(CenterStop?Math.Max(leftX, (BufferWi - (int)textsize.Width) / 2): leftX), offsetTop);
									DrawText(msg,graphics,point,cg);
									break;
								case TextDirection.Right:
									// right
									int rightX;
									if (FitToTime)
									{
										rightX =   -maxtextsize + (int)(percent*(maxtextsize+BufferWi));
									}
									else
									{
										rightX = (Speed * frame) % xlimit / 8 - BufferWi;
									}
									point =
										new Point(Convert.ToInt32(CenterStop? Math.Min(rightX, (BufferWi - (int)textsize.Width) / 2): rightX), offsetTop);
									DrawText(msg,graphics,point,cg);
									break;
								case TextDirection.Up:
									// up

									int upY;
									if (FitToTime)
									{
										upY = BufferHt - (int)( ((textsize.Height * numberLines) + BufferHt) * percent);
									}
									else
									{
										upY = BufferHt - (Speed * frame) % ylimit / 8;
									}

									point = new Point(offsetLeft,
										Convert.ToInt32(CenterStop? Math.Max(upY, (BufferHt - (int)(textsize.Height * numberLines)) / 2)
											: upY));
									DrawText(msg,graphics,point,cg);
									break;
								case TextDirection.Down:
									// down
									int downY;
									if (FitToTime)
									{
										downY = -(int)(textsize.Height * numberLines) + (int)(((textsize.Height * numberLines) + BufferHt) * percent);
									}
									else
									{
										downY = (Speed * frame) % ylimit / 8 - BufferHt;
									}
									point = new Point(offsetLeft,
										Convert.ToInt32(CenterStop
											? Math.Min(downY, (BufferHt - (int)(textsize.Height * numberLines)) / 2)
											: downY));
									DrawText(msg,graphics,point,cg);
									break;
								default:
									// no movement - centered
									point = new Point(0, offsetTop);
									DrawText(msg,graphics,point,cg);
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
						//}
						i++;
					} while (i < numberLines);
				}

			}
		}

		private void DrawText(string text, Graphics g, Point p, ColorGradient cg)
		{
			switch (GradientMode)
			{
				case GradientMode.AcrossElement:
					DrawTextAcrossGradient(text, g, p, cg, LinearGradientMode.Horizontal);
					break;
				case GradientMode.AcrossText:
					DrawTextWithGradient(text, g, p, cg, LinearGradientMode.Horizontal);
					break;
				case GradientMode.VerticalAcrossElement:
					DrawTextAcrossGradient(text, g, p, cg, LinearGradientMode.Vertical);
					break;
				case GradientMode.VerticalAcrossText:
					DrawTextWithGradient(text, g, p, cg, LinearGradientMode.Vertical);
					break;
				case GradientMode.DiagonalAcrossElement:
					DrawTextAcrossGradient(text, g, p, cg, LinearGradientMode.ForwardDiagonal);
					break;
				case GradientMode.DiagonalAcrossText:
					DrawTextWithGradient(text, g, p, cg, LinearGradientMode.ForwardDiagonal);
					break;
				case GradientMode.BackwardDiagonalAcrossElement:
					DrawTextAcrossGradient(text, g, p, cg, LinearGradientMode.BackwardDiagonal);
					break;
				case GradientMode.BackwardDiagonalAcrossText:
					DrawTextWithGradient(text, g, p, cg, LinearGradientMode.BackwardDiagonal);
					break;
			}
		}


		private void DrawTextWithGradient(string text, Graphics g, Point p, ColorGradient cg, LinearGradientMode mode)
		{
			var size = g.MeasureString(text, Font);
			var brush = new LinearGradientBrush(new Rectangle(p.X, p.Y, (int) size.Width, (int) size.Height), Color.Black,
				Color.Black, mode) {InterpolationColors = cg.GetColorBlend()};
			DrawTextWithBrush(text, brush, g, p, mode);
			brush.Dispose();
		}

		private void DrawTextAcrossGradient(string text, Graphics g, Point p, ColorGradient cg, LinearGradientMode mode)
		{
			var brush = new LinearGradientBrush(new Rectangle(0, p.Y, BufferWi, (int) g.MeasureString(text, Font).Height),
				Color.Black,
				Color.Black, mode) {InterpolationColors = cg.GetColorBlend()};
			DrawTextWithBrush(text, brush, g, p, mode);
			brush.Dispose();
		}

		private void DrawTextWithBrush(string text, Brush brush, Graphics g, Point p, LinearGradientMode mode)
		{
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
			g.DrawString(text, Font, brush, p);	
		}
	}
}
