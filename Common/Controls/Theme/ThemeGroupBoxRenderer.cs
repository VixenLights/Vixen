using System.Drawing;
using System.Windows.Forms;

namespace Common.Controls.Theme
{
	public sealed class ThemeGroupBoxRenderer
	{
		#region Draw GroupBox borders and Text

		public static void GroupBoxesDrawBorder(object sender, PaintEventArgs e, Font font)
		{
			GroupBox groupBox = sender as GroupBox;
			if (groupBox == null) return;

			//used to draw the borders and text for the groupboxes to change the default box color.
			//get the text size in groupbox
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
