using Common.Controls.Scaling;

namespace Common.Controls.Theme
{
	public sealed class ThemePropertyGridRenderer
	{
		//Used on the Preview property grid
		public static void PropertyGridRender(PropertyGrid propertyGrid)
		{
			propertyGrid.AutoScaleMode = AutoScaleMode.Font;
			propertyGrid.ViewBackColor = ThemeColorTable.BackgroundColor;
			propertyGrid.CommandsBackColor = ThemeColorTable.BackgroundColor;
			propertyGrid.BackColor = ThemeColorTable.BackgroundColor;
			propertyGrid.HelpBackColor = ThemeColorTable.BackgroundColor;
			propertyGrid.LineColor = ThemeColorTable.TextBoxBackgroundColor;
			propertyGrid.CategoryForeColor = ThemeColorTable.ForeColor;
			propertyGrid.CommandsForeColor = ThemeColorTable.ForeColor;
			propertyGrid.HelpForeColor = ThemeColorTable.ForeColor;
			if (ScalingTools.GetScaleFactor() >= 2)
			{
				propertyGrid.LargeButtons = true;
			}
		}
	}
}
