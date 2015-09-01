using System.Drawing;
using System.Windows.Forms;

namespace Common.Controls.Theme
{
	public sealed class ThemeComboBoxRenderer
	{
		public static void DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0)
				return;

			ComboBox combo = sender as ComboBox;
			combo.BackColor = ThemeColorTable.ComboBoxBackColor;
			if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
			{
				e.Graphics.FillRectangle(new SolidBrush(ThemeColorTable.ComboBoxHighlightColor),
					e.Bounds);
				e.DrawFocusRectangle();
			}
			else
			{
				e.DrawBackground();
			}
			e.Graphics.DrawString(combo.Items[e.Index].ToString(), e.Font, new SolidBrush(ThemeColorTable.ForeColor), new Point(e.Bounds.X, e.Bounds.Y));
		}
	}
}