using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace VixenModules.Effect.Nutcracker
{
	partial class NutcrackerEffects
	{
		public void RenderText(int position, string line1, string line2, string line3, string line4, Font font, int dir, int textRotation, bool centerStop)
		{
			using (var bitmap = new Bitmap(BufferWi, BufferHt))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					Color c = Palette.GetColor(0);
					using (Brush brush = new SolidBrush(c))
					{

						string msg = line1;

						if (line2.Length > 0)
						{
							msg += "\n" + line2;
						}
						if (line3.Length > 0)
						{
							msg += "\n" + line3;
						}
						if (line4.Length > 0)
						{
							msg += "\n" + line4;
						}

						SizeF textsize = graphics.MeasureString(msg, font);

						int maxwidth = Convert.ToInt32(textsize.Width);
						int maxht = Convert.ToInt32(textsize.Height);

						int extraLeft = IsGoingLeft(dir) ? BufferWi - (int)textsize.Width : 0;
						int extraRight = IsGoingRight(dir) ? BufferWi - (int)textsize.Width : 0;
						int extraDown = IsGoingDown(dir) ? BufferHt - (int)textsize.Height : 0;
						int extraUp = IsGoingUp(dir) ? BufferHt - (int)textsize.Height : 0;

						int xlimit = (BufferWi + maxwidth) * 8 + 1;
						int ylimit = (BufferHt + maxht) * 8 + 1;
						int offsetLeft = (position * maxwidth / 100) - maxwidth / 2;
						int offsetTop = maxht / 2 - (position * maxht / 100);

						graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

						Point point;
						switch (dir)
						{
							case 0:
								// left
								point = new Point(Convert.ToInt32(centerStop ? Math.Max(BufferWi - State / 8, extraLeft / 2) : BufferWi - State % xlimit / 8), offsetTop);
								graphics.DrawString(msg, font, brush, point);
								break;
							case 1:
								// right
								point = new Point(Convert.ToInt32(centerStop ? Math.Min(State / 8 - BufferWi, extraRight / 2) : State % xlimit / 8 - BufferWi), offsetTop);
								graphics.DrawString(msg, font, brush, point);
								break;
							case 2:
								// up
								point = new Point(offsetLeft, Convert.ToInt32(centerStop ? Math.Max((int)(BufferHt - State / 8), extraUp / 2) : BufferHt - State % ylimit / 8));
								graphics.DrawString(msg, font, brush, point);
								break;
							case 3:
								// down
								point = new Point(offsetLeft, Convert.ToInt32(centerStop ? Math.Min((int)(State / 8 - BufferHt), extraDown / 2) : State % ylimit / 8 -BufferHt));
								graphics.DrawString(msg, font, brush, point);
								break;
							default:
								// no movement - centered
								point = new Point(0, offsetTop);
								graphics.DrawString(msg, font, brush, point);
								break;
						}

						// copy to buffer
						for (int x = 0; x < BufferWi; x++)
						{
							for (int y = 0; y < BufferHt; y++)
							{
								c = bitmap.GetPixel(x, BufferHt - y - 1);
								SetPixel(x, y, c);
							}
						}
					}
				}
			}
		}

		private bool IsGoingLeft(int dir)
		{
			return dir == 0;
		}

		private bool IsGoingRight(int dir)
		{
			return dir == 1;
		}

		private bool IsGoingUp(int dir)
		{
			return dir == 2;
		}

		private bool IsGoingDown(int dir)
		{
			return dir == 3;
		}

	}
}
