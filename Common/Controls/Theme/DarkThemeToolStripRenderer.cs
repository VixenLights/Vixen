using System.Drawing;
using System.Windows.Forms;

namespace Common.Controls.Theme
{
	public class DarkThemeToolStripRenderer : ToolStripProfessionalRenderer
	{
		public DarkThemeToolStripRenderer() : base(new DarkThemeColorTable()) { }

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			e.TextColor = DarkThemeColorTable.ForeColor;
			base.OnRenderItemText(e);
		}

		protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
		{
			base.OnRenderButtonBackground(e);
			ToolStripButton item = e.Item as ToolStripButton;
			Graphics g = e.Graphics;
			Rectangle bounds = new Rectangle(Point.Empty, item.Size);

			if (item.CheckState == CheckState.Checked)
			{
				using (Pen p = new Pen(DarkThemeColorTable.SelectedHighlightColor, 1))
				{
					g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
				}
			}
		}

		protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
		{
			base.OnRenderItemImage(e);
			Graphics g = e.Graphics;
			Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
		}

		protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
		{
			base.OnRenderItemBackground(e);
			Graphics g = e.Graphics;
			Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
			if (e.Item.Selected == true)
			{
				using (Pen p = new Pen(DarkThemeColorTable.SelectedHighlightColor, 1))
				{
					g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
				}
			}
		}

		protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
		{
			base.OnRenderItemCheck(e);
			Rectangle bounds = new Rectangle(e.ImageRectangle.Left - 2, 1, e.ImageRectangle.Width + 4, e.Item.Height - 2);
			Graphics g = e.Graphics;
			using (Pen p = new Pen(DarkThemeColorTable.SelectedHighlightColor, 1))
			{
				g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
			}

		}

	
	}
}
