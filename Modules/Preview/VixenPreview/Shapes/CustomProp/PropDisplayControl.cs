using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{

	public class PropDisplayControl : Panel
	{
		private int columns = 16;
		private int rows = 12;
		private Mine[,] mines;

		public PropDisplayControl()
		{
			this.DoubleBuffered = true;
			this.ResizeRedraw = true;

			// initialize mine field:
			mines = new Mine[columns, rows];
			for (int y = 0; y < rows; ++y)
			{
				for (int x = 0; x < columns; ++x)
				{
					mines[x, y] = new Mine();
				}
			}
		}

		// adjust each column and row to fit entire client area:
		protected override void OnResize(EventArgs e)
		{
			int top = 0;
			for (int y = 0; y < rows; ++y)
			{
				int left = 0;
				int height = (this.ClientSize.Height - top) / (rows - y);
				for (int x = 0; x < columns; ++x)
				{
					int width = (this.ClientSize.Width - left) / (columns - x);
					mines[x, y].Bounds = new Rectangle(left, top, width, height);
					left += width;
				}
				top += height;
			}
			base.OnResize(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			for (int y = 0; y < rows; ++y)
			{
				for (int x = 0; x < columns; ++x)
				{
					if (mines[x, y].IsRevealed)
					{
						e.Graphics.FillRectangle(Brushes.DarkGray, mines[x, y].Bounds);
					}
					else
					{
						ControlPaint.DrawButton(e.Graphics, mines[x, y].Bounds, ButtonState.Normal);
					}
				}
			}
			base.OnPaint(e);
		}

		// determine which button the user pressed:
		protected override void OnMouseDown(MouseEventArgs e)
		{
			for (int y = 0; y < rows; ++y)
			{
				for (int x = 0; x < columns; ++x)
				{
					if (mines[x, y].Bounds.Contains(e.Location))
					{
						mines[x, y].IsRevealed = true;
						this.Invalidate();
						MessageBox.Show(
						  string.Format("You pressed on button ({0}, {1})",
						  x.ToString(), y.ToString())
						);
					}
				}
			}
			base.OnMouseDown(e);
		}

	}


	public class Mine
	{
		public Rectangle Bounds { get; set; }
		public bool IsBomb { get; set; }
		public bool IsRevealed { get; set; }
	}
}
