using System;
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

		protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
		{
			Point startPoint;
			Point endPoint;
			if (e.Item.Height > e.Item.Width)
			{
				startPoint = new Point(3, 0);
				endPoint = new Point(3, e.Item.Height);
			}
			else
			{
				startPoint = new Point(3, 3);
				endPoint = new Point(e.Item.Width - 3, 3);
			}
			Graphics g = e.Graphics;
			Pen pen = new Pen(ThemeColorTable.BorderColor, 1);
			g.DrawLine(pen, startPoint, endPoint);
			pen.Dispose();
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
					int hor = (int) (2 * ScalingTools.GetScaleFactor());
					int ver = hor;

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
			using (Pen pen = new Pen(ThemeColorTable.ForeColor))
			{
				g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
			}
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
		
		protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
		{
			base.OnRenderItemImage(e);
			var menuItem = e.Item as ToolStripMenuItem;
			if (menuItem != null && menuItem.CheckOnClick && menuItem.Checked && e.ImageRectangle != Rectangle.Empty)
			{
				using (Pen p = new Pen(ThemeColorTable.HighlightColor, 1))
				{
					e.Graphics.DrawRectangle(p, 2, 0, e.ImageRectangle.Width + 5, e.ImageRectangle.Height + 5);
				}
			}
		}

		protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
		{
			Image image = Tools.GetIcon(Resources.Properties.Resources.check_mark, e.ImageRectangle.Height);
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
