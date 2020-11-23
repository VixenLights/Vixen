using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Common.Controls.Theme
{
	public sealed class ThemeGroupBoxRenderer
	{
		
		#region Draw GroupBox borders and Text

		public static void GroupBoxesDrawBorder(object sender, PaintEventArgs e, Font f)
		{
			GroupBox groupBox = sender as GroupBox;
			if (groupBox == null) return;
			var g = e.Graphics;
			
			Color textColor = groupBox.Enabled ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;

			Brush borderBrush = new SolidBrush(ThemeColorTable.GroupBoxBorderColor);
			Pen borderPen = new Pen(borderBrush);
			SizeF strSize = TextRenderer.MeasureText(groupBox.Text, groupBox.Font);
			Rectangle rect = new Rectangle(groupBox.ClientRectangle.X,
										   groupBox.ClientRectangle.Y + (int)(strSize.Height / 2),
										   groupBox.ClientRectangle.Width - 2,
										   groupBox.ClientRectangle.Height - (int)(strSize.Height / 2) - 1);

			// Clear text and border
			g.Clear(groupBox.BackColor);

			TextRenderer.DrawText(g,groupBox.Text,groupBox.Font,new Point(groupBox.Padding.Left,0), textColor);
			
			// Drawing Border
			//Left
			g.DrawLine(borderPen, rect.Location, new Point(rect.X, rect.Y + rect.Height));
			//Right
			g.DrawLine(borderPen, new Point(rect.X + rect.Width, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height));
			//Bottom
			g.DrawLine(borderPen, new Point(rect.X, rect.Y + rect.Height), new Point(rect.X + rect.Width, rect.Y + rect.Height));
			//Top1
			g.DrawLine(borderPen, new Point(rect.X, rect.Y), new Point(rect.X + groupBox.Padding.Left, rect.Y));
			//Top2
			g.DrawLine(borderPen, new Point(rect.X + groupBox.Padding.Left + (int)(strSize.Width), rect.Y), new Point(rect.X + rect.Width, rect.Y));

			borderBrush.Dispose();
			borderPen.Dispose();
		}

		#endregion
	}
}
