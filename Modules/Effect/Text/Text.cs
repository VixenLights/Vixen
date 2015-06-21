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
		[ProviderCategory(@"Config", 0)]
		[ProviderDisplayName(@"Orientation")]
		[ProviderDescription(@"Orientation")]
		[PropertyOrder(0)]
		public StringOrientation Orientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				StringOrientation = value;
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
		[PropertyOrder(2)]
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
		[NumberRange(1, 20, 1)]
		[PropertyOrder(3)]
		public int Position
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
		[ProviderDisplayName(@"TextLine1")]
		[ProviderDescription(@"TextLine")]
		[PropertyOrder(4)]
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"TextLine2")]
		[ProviderDescription(@"TextLine")]
		[PropertyOrder(5)]
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"TextLine3")]
		[ProviderDescription(@"TextLine")]
		[PropertyOrder(6)]
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"TextLine4")]
		[ProviderDescription(@"TextLine")]
		[PropertyOrder(7)]
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Font")]
		[ProviderDescription(@"Font")]
		[PropertyOrder(8)]
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
		[ProviderDisplayName(@"CenterStop")]
		[ProviderDescription(@"CenterStop")]
		[PropertyOrder(8)]
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Color")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(9)]
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
				IsDirty = true;
			}
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
					var maxlinesize = new[]
					{
						Convert.ToInt32(graphics.MeasureString(Line1, Font).Width),
						Convert.ToInt32(graphics.MeasureString(Line2, Font).Width),
						Convert.ToInt32(graphics.MeasureString(Line3, Font).Width),
						Convert.ToInt32(graphics.MeasureString(Line4, Font).Width)
					};
					int maxtextsize = maxlinesize.Max();
					int maxIndex = maxlinesize.ToList().IndexOf(maxtextsize);
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
						using (Brush brush = new SolidBrush(Colors[i % Colors.Count()].GetColorAt(0)))
						{
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
							int extraLeft = Direction == TextDirection.Left ? BufferWi - (int) textsize.Width : 0;
							int extraRight = Direction == TextDirection.Right ? BufferWi - (int) textsize.Width : 0;
							int extraDown = Direction == TextDirection.Down ? BufferHt - (int) (textsize.Height*numberLines) : 0;
							int extraUp = Direction == TextDirection.Up ? BufferHt - (int) (textsize.Height*numberLines) : 0;
							int xlimit = (BufferWi + maxwidth)*8 + 1;
							int ylimit = (BufferHt + maxht)*8 + 1;
							int offsetLeft = (Position*maxwidth/100) - maxwidth/2;
							int offsetTop = maxht/2 - (Position*maxht/100);
							graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
							Point point;
							switch (Direction)
							{
								case TextDirection.Left:
									// left
									point =
										new Point(
											Convert.ToInt32(CenterStop
												? Math.Max(BufferWi - Speed/8, extraLeft/2)
												: BufferWi - Speed%xlimit/8), offsetTop);
									graphics.DrawString(msg, Font, brush, point);
									break;
								case TextDirection.Right:
									// right
									point =
										new Point(
											Convert.ToInt32(CenterStop
												? Math.Min(Speed/8 - BufferWi, extraRight/2)
												: Speed%xlimit/8 - BufferWi), offsetTop);
									graphics.DrawString(msg, Font, brush, point);
									break;
								case TextDirection.Up:
									// up
									point = new Point(offsetLeft,
										Convert.ToInt32(CenterStop
											? Math.Max((int) (BufferHt - Speed/8), extraUp/2)
											: BufferHt - Speed%ylimit/8));
									graphics.DrawString(msg, Font, brush, point);
									break;
								case TextDirection.Down:
									// down
									point = new Point(offsetLeft,
										Convert.ToInt32(CenterStop
											? Math.Min((Speed/8 - BufferHt), extraDown/2)
											: Speed%ylimit/8 - BufferHt));
									graphics.DrawString(msg, Font, brush, point);
									break;
								default:
									// no movement - centered
									point = new Point(0, offsetTop);
									graphics.DrawString(msg, Font, brush, point);
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
						i++;
					} while (i < 4);
				}

			}
		}
	}
}
