using System.Drawing;
using System.Windows.Forms;

namespace Common.Controls.Theme
{
	public sealed class ThemeGroupBoxRenderer
	{
		#region Draw GroupBox borders and Text

		public static void GroupBoxesDrawBorder(object sender, PaintEventArgs e, Font font)
		{
			//used to draw the borders and text for the groupboxes to change the default box color.
			GroupBox groupBox = sender as GroupBox;
			if (groupBox == null) return;

			//get the text size in groupbox and clears groupbox and adds new background color
			Size tSize = TextRenderer.MeasureText(groupBox.Text, font);
			e.Graphics.Clear(ThemeColorTable.BackgroundColor);

			//draw the border
			Rectangle borderRect = e.ClipRectangle;
			borderRect.Y = (borderRect.Y + (tSize.Height / 2));
			borderRect.Height = (borderRect.Height - (tSize.Height / 2));
			ControlPaint.DrawBorder(e.Graphics, borderRect, ThemeColorTable.GroupBoxBorderColor, ButtonBorderStyle.Solid);

			//draw the text
			Rectangle textRect = e.ClipRectangle;
			textRect.X = (textRect.X + 6);
			textRect.Width = tSize.Width + 12;
			textRect.Height = tSize.Height;
			e.Graphics.FillRectangle(new SolidBrush(ThemeColorTable.BackgroundColor), textRect);
			e.Graphics.DrawString(groupBox.Text, font, new SolidBrush(ThemeColorTable.ForeColor), textRect);
		}
		#endregion
	}
}
