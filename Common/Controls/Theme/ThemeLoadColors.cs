using System.Drawing;

namespace Common.Controls.Theme
{
	public class ThemeLoadColors
	{
		public static Color[] _vixenThemeColors = new Color[15];

		public static void DarkTheme()
		{
			//This is the new default Dark Theme
			_vixenThemeColors[0] = Color.FromArgb(68, 68, 68); //BackGroundColor
			_vixenThemeColors[3] = Color.FromArgb(221, 221, 221); //Foreground Color
			_vixenThemeColors[2] = Color.FromArgb(119, 119, 119); //ForeColorDisabled
			_vixenThemeColors[1] = Color.FromArgb(68, 68, 68); //Menu Selected Highlight Color
			_vixenThemeColors[4] = Color.FromArgb(90, 90, 90); //TextBox BackGround Color
			_vixenThemeColors[5] = Color.FromArgb(40, 40, 40); //Button Border
			_vixenThemeColors[6] = Color.FromArgb(136, 136, 136); //GroupBox Border Color
			_vixenThemeColors[7] = Color.FromArgb(90, 90, 90); //ComboBox Back Color
			_vixenThemeColors[8] = Color.FromArgb(90, 90, 90); //ListBox Back Color
			_vixenThemeColors[9] = Color.FromArgb(90, 90, 90); //Menu Back Color
			_vixenThemeColors[10] = Color.FromArgb(20, 20, 20); //Button Back Color
			_vixenThemeColors[11] = Color.FromArgb(90, 90, 90); //Button Back Color Hover
			_vixenThemeColors[12] = Color.FromArgb(90, 90, 90); //Numeric Back Color
			_vixenThemeColors[13] = Color.FromArgb(68, 68, 68); //ComboBox Highlight Color

			_vixenThemeColors[14] = Color.Black; //This is used as a test to see if the Dark Button background is used.
			ThemeColorTable.newBackGroundImage = null; //Button Back Color
			ThemeColorTable.newBackGroundImageHover = null; //Button Back Color hover
		}

		public static void WindowsTheme()
		{
			//This is the old Windows Theme
			_vixenThemeColors[0] = SystemColors.Control; //BackGroundColor
			_vixenThemeColors[3] = SystemColors.ControlText; //Foreground Color
			_vixenThemeColors[2] = Color.FromArgb(119, 119, 119); //ForeColorDisabled
			_vixenThemeColors[1] = SystemColors.Control; //Menu Selected Highlight Color
			_vixenThemeColors[4] = SystemColors.Control; //TextBox BackGround Color
			_vixenThemeColors[5] = Color.FromArgb(205, 205, 205); //Button Border
			_vixenThemeColors[6] = Color.FromArgb(205, 205, 205); //GroupBox Border Color
			_vixenThemeColors[7] = SystemColors.Window; //ComboBox Back Color
			_vixenThemeColors[8] = SystemColors.Window; //ListBox Back Color
			_vixenThemeColors[9] = SystemColors.Window; //Menu Back Color
			_vixenThemeColors[10] = Color.FromArgb(235, 235, 235); //Button Back Color
			_vixenThemeColors[11] = Color.FromArgb(233, 243, 252); //Button Back Color Hover
			_vixenThemeColors[12] = SystemColors.Window; //Numeric Back Color
			_vixenThemeColors[13] = SystemColors.Control; //ComboBox Highlight Color

			_vixenThemeColors[14] = Color.White; //This is used as a test to see if the Dark Button background is used.
		}

		public static void LightTheme()
		{
			//This is a new custom theme
			_vixenThemeColors[0] = Color.FromArgb(68, 68, 68); //BackGroundColor
			_vixenThemeColors[3] = Color.FromArgb(221, 221, 221); //Foreground Color
			_vixenThemeColors[2] = Color.FromArgb(68, 68, 68); //ForeColorDisabled
			_vixenThemeColors[1] = Color.FromArgb(68, 68, 68); //Menu Selected Highlight Color
			_vixenThemeColors[4] = Color.FromArgb(80, 80, 80); //TextBox BackGround Color
			_vixenThemeColors[5] = Color.FromArgb(40, 40, 40); //Button Border
			_vixenThemeColors[6] = Color.FromArgb(136, 136, 136); //GroupBox Border Color
			_vixenThemeColors[7] = Color.FromArgb(90, 90, 90); //ComboBox Back Color
			_vixenThemeColors[8] = Color.FromArgb(68, 68, 68); //ListBox Back Color
			_vixenThemeColors[9] = Color.FromArgb(90, 90, 90); //Menu Back Color
			_vixenThemeColors[10] = Color.FromArgb(20, 20, 20); //Button Back Color
			_vixenThemeColors[11] = Color.FromArgb(80, 80, 80); //Button Back Color Hover
			_vixenThemeColors[12] = Color.FromArgb(80, 80, 80); //Numeric Back Color
			_vixenThemeColors[13] = Color.FromArgb(68, 68, 68); //ComboBox Highlight Color

			_vixenThemeColors[14] = Color.White; //This is used as a test to see if the Dark Button background is used.
		}

		public static void InitialLoadTheme()
		{
			//Used when Vixen is opened and will transfer stored colres to the ThemeColorTable for use throughout
			//all Vixen Forms when loaded.
			ThemeColorTable._backgroundColor = _vixenThemeColors[0];
			ThemeColorTable._menuSelectedHighlightBackColor = _vixenThemeColors[1];
			ThemeColorTable._foreColorDisabled = _vixenThemeColors[2];
			ThemeColorTable._foreColor = _vixenThemeColors[3];
			ThemeColorTable._textBoxBackColor = _vixenThemeColors[4];
			ThemeColorTable._buttonBorderColor = _vixenThemeColors[5];
			ThemeColorTable._groupBoxborderColor = _vixenThemeColors[6];
			ThemeColorTable._comboBoxBackColor = _vixenThemeColors[7];
			ThemeColorTable._listBoxBackColor = _vixenThemeColors[8];
			ThemeColorTable._highlightColor = _vixenThemeColors[9];
			ThemeColorTable._buttonBackColor = _vixenThemeColors[10];
			ThemeColorTable._buttonBackColorHover = _vixenThemeColors[11];
			ThemeColorTable._numericBackColor = _vixenThemeColors[12];
			ThemeColorTable._comboBoxHighlightColor = _vixenThemeColors[13];
			if (_vixenThemeColors[14] == Color.White) //This is used as a test to see if the Dark Button background is used.
			{
				ThemeMainForm.ColorButtonChange(); //Will re-render the Buttons if the standard Background Image is not used because a user has changed the Button background color of the theme.
			}
		}
	}
}
