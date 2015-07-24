using System.Drawing;
using System.Windows.Forms;

namespace Common.Controls.Theme
{
	public sealed class DarkThemeGroupBoxRenderer
	{
		#region Draw lines and GroupBox borders

		public static void GroupBoxesDrawBorder(object sender, PaintEventArgs e, Font font)
		{
			GroupBox groupBox = sender as GroupBox;
			if (groupBox == null) return;

			//used to draw the borders and text for the groupboxes to change the default box color.
			//get the text size in groupbox
			Size tSize = TextRenderer.MeasureText(groupBox.Text, font);

			e.Graphics.Clear(DarkThemeColorTable.BackgroundColor);
			//draw the border
			Rectangle borderRect = e.ClipRectangle;
			borderRect.Y = (borderRect.Y + (tSize.Height / 2));
			borderRect.Height = (borderRect.Height - (tSize.Height / 2));
			ControlPaint.DrawBorder(e.Graphics, borderRect, DarkThemeColorTable.BorderColor, ButtonBorderStyle.Solid);

			//draw the text
			Rectangle textRect = e.ClipRectangle;
			textRect.X = (textRect.X + 6);
			textRect.Width = tSize.Width + 10;
			textRect.Height = tSize.Height;
			e.Graphics.FillRectangle(new SolidBrush(DarkThemeColorTable.BackgroundColor), textRect);
			e.Graphics.DrawString(groupBox.Text, font, new SolidBrush(DarkThemeColorTable.ForeColor), textRect);
		}
		#endregion
	}
}
