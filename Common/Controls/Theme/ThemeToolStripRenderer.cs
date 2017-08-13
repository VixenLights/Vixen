using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml.Serialization;
using Common.Controls.Scaling;
using Common.Resources;
using Common.Resources.Properties;

namespace Common.Controls.Theme
{
	//used to render all tool/status/menu strips
	public class ThemeToolStripRenderer : ToolStripProfessionalRenderer
	{
		public ThemeToolStripRenderer() : base(new ThemeColorTable()) { }

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			e.TextColor = ThemeColorTable.ForeColor;
			base.OnRenderItemText(e);
		}
		protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
		{
			ToolStripItem toolStripItem = e.Item;
			if (toolStripItem is ToolStripDropDownItem)
			{
				Graphics g = e.Graphics;
				Rectangle dropDownRect = e.ArrowRectangle;
				using (Brush brush = new SolidBrush(toolStripItem.Enabled ? ThemeColorTable.ForeColor : SystemColors.ControlDark))
				{
					Point middle = new Point(dropDownRect.Left + dropDownRect.Width / 2, dropDownRect.Top + dropDownRect.Height / 2);
					Point[] arrow;
					int hor=2 , ver = 2;

					switch (e.Direction)
					{
						case ArrowDirection.Up:

							arrow = new Point[] {
								new Point(middle.X - hor, middle.Y + 1),
								new Point(middle.X + hor + 1, middle.Y + 1),
								new Point(middle.X, middle.Y - ver)};

							break;
						case ArrowDirection.Left:
							arrow = new Point[] {
								new Point(middle.X + hor, middle.Y - 2 * ver),
								new Point(middle.X + hor, middle.Y + 2 * ver),
								new Point(middle.X - hor, middle.Y)};

							break;
						case ArrowDirection.Right:
							arrow = new Point[] {
								new Point(middle.X - hor, middle.Y - 2 * ver),
								new Point(middle.X - hor, middle.Y + 2 * ver),
								new Point(middle.X + hor, middle.Y)};

							break;
						default:
							arrow = new Point[] {
								new Point(middle.X - hor, middle.Y - 1),
								new Point(middle.X + hor + 1, middle.Y - 1),
								new Point(middle.X, middle.Y + ver) };
							break;
					}
					g.FillPolygon(brush, arrow);
				}
			}
			else
			{
				base.OnRenderArrow(e);
			}
		}

		private List<Point> GetArrow(ArrowDirection direction, Rectangle r)
		{
			List<Point> points = new List<Point>();
			
			switch (direction)
			{
				case ArrowDirection.Down:
					points.Add(new Point(r.Left - 2, r.Height / 2 - 3));
					points.Add(new Point(r.Right + 2, r.Height / 2 - 3));
					points.Add(new Point(r.Left + (r.Width / 2), r.Height / 2 + 3));
					break;
				case ArrowDirection.Right:
					points.Add(new Point(r.Left - 2, r.Height / 2 - 3));
					points.Add(new Point(r.Right + 2, r.Height / 2 - 3));
					points.Add(new Point(r.Left + (r.Width / 2),
						r.Height / 2 + 3));
					break;
			}

			return points;
		}

		protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
		{
			var item = e.Item;
			Graphics g = e.Graphics;
			Rectangle bounds = new Rectangle(Point.Empty, item.Size);


			if (e.Item.Selected)
			{
				RenderSelectedButtonFill(bounds, g);
			}
			else
			{
				using (Brush b = new SolidBrush(item.BackColor))
				{
					g.FillRectangle(b, bounds);
				}
			}
		}

		protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
		{
			var item = e.Item;
			Graphics g = e.Graphics;
			Rectangle bounds = new Rectangle(Point.Empty, item.Size);

			
			if (e.Item.Selected)
			{
				RenderSelectedButtonFill(bounds, g);
			}
			else
			{
				Color fillColor = ThemeColorTable.BackgroundColor;
				using (Brush b = new SolidBrush(fillColor))
				{
					g.FillRectangle(b, bounds);
				}
			}

		}

		protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
		{
			//base.OnRenderButtonBackground(e);
			ToolStripButton item = e.Item as ToolStripButton;
			Graphics g = e.Graphics;
			Rectangle bounds = new Rectangle(Point.Empty, item.Size);

			if (item.CheckState == CheckState.Checked)
			{
				RenderCheckedButtonFill(bounds, item, g);
			}
			else
			{
				if (item.Selected)
				{
					RenderSelectedButtonFill(bounds, g);
				}
				else
				{
					Color fillColor = ThemeColorTable.BackgroundColor;
					using (Brush b = new SolidBrush(fillColor))
					{
						g.FillRectangle(b, bounds);
					}
				}
				
			}

		}

		private void RenderSelectedButtonFill(Rectangle bounds, Graphics g)
		{
			GraphicsPath path = new GraphicsPath();
			path.AddRectangle(new RectangleF(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1));
			using (PathGradientBrush pthGrBrush = new PathGradientBrush(path))
			{
				// Set the color at the center of the path.
				pthGrBrush.CenterColor = ColorTable.ButtonSelectedGradientBegin;

				Color[] colors = { ColorTable.ButtonSelectedGradientEnd };
				pthGrBrush.SurroundColors = colors;

				g.FillRectangle(pthGrBrush, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
			}
		}

		private void RenderCheckedButtonFill(Rectangle bounds, ToolStripButton item, Graphics g)
		{
			if (item.Selected)
			{
				RenderSelectedButtonFill(bounds, g);
			}
			
			// Set the color of border when item is selected.
			Pen pen = new Pen(ThemeColorTable.ForeColor);
			g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
			
		}

		protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
		{
			base.OnRenderItemBackground(e);
			Graphics g = e.Graphics;
			Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
			if (e.Item.Selected)
			{
				using (Pen p = new Pen(ThemeColorTable.HighlightColor, 1))
				{
					g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
				}
			}
		}

		protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
		{
			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
			Image image = Tools.GetIcon(Resources.Properties.Resources.check_mark, iconSize);
			Rectangle imageRect = e.ImageRectangle;


			if (imageRect != Rectangle.Empty && image != null)
			{
				if (!e.Item.Enabled)
				{
					image = CreateDisabledImage(image);
				}

				e.Graphics.DrawImage(image, imageRect, new Rectangle(Point.Empty, imageRect.Size), GraphicsUnit.Pixel);
			}

		}
	}
}
