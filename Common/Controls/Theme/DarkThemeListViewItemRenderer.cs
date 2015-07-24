using System.Drawing;
using System.Windows.Forms;

namespace Common.Controls.Theme
{
	public sealed class DarkThemeListViewItemRenderer
	{
		public static void DrawItem(object sender, DrawListViewItemEventArgs e)
		{

			if ((e.State & ListViewItemStates.Selected) != 0)
			{
				e.Graphics.FillRectangle(new SolidBrush(DarkThemeColorTable.BackgroundColor), e.Bounds);
				e.DrawFocusRectangle();
			}
			else
			{
				e.DrawBackground();
			}
				
			e.DrawText();
		}
	}
}
