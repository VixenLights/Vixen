using System;
using System.Windows.Forms;

namespace Common.Controls.Theme
{
	public sealed class ThemePropertyGridRenderer
	{
		//Used on the Preview property grid
		public static void PropertyGridRender(Object sender)
		{
			PropertyGrid propertyGrid = sender as PropertyGrid;
			if (propertyGrid == null) return;
			propertyGrid.ViewBackColor = ThemeColorTable.BackgroundColor;
			propertyGrid.CommandsBackColor = ThemeColorTable.BackgroundColor;
			propertyGrid.BackColor = ThemeColorTable.BackgroundColor;
			propertyGrid.HelpBackColor = ThemeColorTable.BackgroundColor;
			propertyGrid.LineColor = ThemeColorTable.TextBoxBackgroundColor;
			propertyGrid.CategoryForeColor = ThemeColorTable.ForeColor;
			propertyGrid.CommandsForeColor = ThemeColorTable.ForeColor;
			propertyGrid.HelpForeColor = ThemeColorTable.ForeColor;
		}
	}
}
