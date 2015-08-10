using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Common.Resources;
using Button = System.Windows.Controls.Button;
using MouseEventHandler = System.Windows.Input.MouseEventHandler;
using Point = System.Drawing.Point;

namespace Common.Controls.Theme
{
	public sealed class ThemeButtonRenderer
	{
		public static bool ButtonHover;

		public static void OnPaint(object sender, PaintEventArgs e, Image icon)
		{
			// The following code is used to set a lighter color of the BackColor and BackColorHover of the button.
			float correctionFactor = 0.3f;
			float red = (255 - ThemeColorTable.ButtonBackColor.R) * correctionFactor + ThemeColorTable.ButtonBackColor.R;
			float green = (255 - ThemeColorTable.ButtonBackColor.G) * correctionFactor + ThemeColorTable.ButtonBackColor.G;
			float blue = (255 - ThemeColorTable.ButtonBackColor.B) * correctionFactor + ThemeColorTable.ButtonBackColor.B;
			Color buttonBackground = Color.FromArgb(ThemeColorTable.ButtonBackColor.A, (int)red, (int)green, (int)blue);
			red = (255 - ThemeColorTable.ButtonBackColorHover.R) * correctionFactor + ThemeColorTable.ButtonBackColorHover.R;
			green = (255 - ThemeColorTable.ButtonBackColorHover.G) * correctionFactor + ThemeColorTable.ButtonBackColorHover.G;
			blue = (255 - ThemeColorTable.ButtonBackColorHover.B) * correctionFactor + ThemeColorTable.ButtonBackColorHover.B;
			Color buttonBackgroundHover = Color.FromArgb(ThemeColorTable.ButtonBackColorHover.A, (int)red, (int)green, (int)blue);

			var btn = sender as System.Windows.Forms.Button;

			//Draws the Background
			Brush paintBrush;
			Graphics g = e.Graphics;
			if (ButtonHover)
			{
				paintBrush = new LinearGradientBrush(btn.ClientRectangle, buttonBackgroundHover, ThemeColorTable.ButtonBackColorHover,
					LinearGradientMode.Vertical);
			}
			else
			{
				paintBrush = new LinearGradientBrush(btn.ClientRectangle, buttonBackground, ThemeColorTable.ButtonBackColor,
					LinearGradientMode.Vertical);
			}
			g.FillRectangle(paintBrush, btn.ClientRectangle);

			//Draws an icon on the left side of the button if icon is passed from source, for example the help picture that is on the Help buttons.
			//and draw the Text
			var stringFormat = new StringFormat();
			stringFormat.LineAlignment = StringAlignment.Center;
			var _centerPoint = new PointF(btn.Width / 2, btn.Height / 2);
			if (icon != null)
			{
				stringFormat.Alignment = StringAlignment.Near;
				g.DrawImage(Tools.GetIcon(icon, 16), 0, _centerPoint.Y - 8);
			}
			else
			{
				stringFormat.Alignment = StringAlignment.Center;
			}
			g.DrawString(btn.Text, btn.Font,
				btn.Enabled ? (new SolidBrush(ThemeColorTable.ForeColor)) : (new SolidBrush(ThemeColorTable.ForeColorDisabled)),
				_centerPoint.X, _centerPoint.Y, stringFormat); //Sets forecolor of button depending of enabled status

			//Draws the Border
			Pen pen = new Pen(ThemeColorTable.ButtonBorderColor, 1);
			Point[] pts = border_Get(0, 0, btn.Width - 1, btn.Height - 1);
			e.Graphics.DrawLines(pen, pts);
			pen.Dispose();
		}

		private static Point[] border_Get(int nLeftEdge, int nTopEdge, int nWidth, int nHeight)
		{
			int X = nWidth;
			int Y = nHeight;
			Point[] points =
			{
				new Point(1, 0),
				new Point(X - 1, 0),
				new Point(X - 1, 1),
				new Point(X, 1),
				new Point(X, Y - 1),
				new Point(X - 1, Y - 1),
				new Point(X - 1, Y),
				new Point(1, Y),
				new Point(1, Y - 1),
				new Point(0, Y - 1),
				new Point(0, 1),
				new Point(1, 1)
			};
			for (int i = 0; i < points.Length; i++)
			{
				points[i].Offset(nLeftEdge, nTopEdge);
			}
			return points;
		}
	}
}
