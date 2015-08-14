using System.Drawing;

namespace Common.Controls.Theme
{
	//add new themes colors here
	public class ThemeLoadColors
	{
		public static Color[] _vixenThemeColors = new Color[19];

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
			_vixenThemeColors[11] = Color.FromArgb(40, 40, 40); //Button Back Color Hover
			_vixenThemeColors[12] = Color.FromArgb(90, 90, 90); //Numeric Back Color
			_vixenThemeColors[13] = Color.FromArgb(68, 68, 68); //ComboBox Highlight Color
			_vixenThemeColors[14] = Color.FromArgb(90, 90, 90); //Time Line Element Background
			_vixenThemeColors[15] = Color.FromArgb(0, 0, 0); //TimeLine Grid color
			_vixenThemeColors[16] = Color.FromArgb(68, 68, 68); //TimeLine Effects Background
			_vixenThemeColors[17] = Color.FromArgb(221, 221, 221); //TimeLine Text Forecolor
			_vixenThemeColors[18] = Color.FromArgb(68, 68, 68); //TimeLine Label Background
		}

		public static void WindowsTheme()
		{
			//This is the old Windows Theme as best as I can
			_vixenThemeColors[0] = Color.FromArgb(240, 240, 240); //BackGroundColor
			_vixenThemeColors[3] = Color.FromArgb(10, 10, 10); //Foreground Color
			_vixenThemeColors[2] = Color.FromArgb(119, 119, 119); //ForeColorDisabled
			_vixenThemeColors[1] = SystemColors.Control; //Menu Selected Highlight Color
			_vixenThemeColors[4] = SystemColors.Window; //TextBox BackGround Color
			_vixenThemeColors[5] = Color.FromArgb(205, 205, 205); //Button Border
			_vixenThemeColors[6] = Color.FromArgb(205, 205, 205); //GroupBox Border Color
			_vixenThemeColors[7] = SystemColors.Window; //ComboBox Back Color
			_vixenThemeColors[8] = SystemColors.Window; //ListBox Back Color
			_vixenThemeColors[9] = SystemColors.Window; //Menu Back Color
			_vixenThemeColors[10] = Color.FromArgb(230, 230, 230); //Button Back Color
			_vixenThemeColors[11] = Color.FromArgb(233, 243, 252); //Button Back Color Hover
			_vixenThemeColors[12] = SystemColors.Window; //Numeric Back Color
			_vixenThemeColors[13] = SystemColors.Control; //ComboBox Highlight Color
			_vixenThemeColors[14] = Color.FromArgb(200, 200, 200); //Time Line Element Background
			_vixenThemeColors[15] = Color.FromArgb(10, 10, 10); //TimeLine Grid color
			_vixenThemeColors[16] = Color.FromArgb(60, 60, 60); //TimeLine Effects Background
			_vixenThemeColors[17] = Color.FromArgb(10, 10, 10); //TimeLine Text Forecolor
			_vixenThemeColors[18] = Color.FromArgb(227, 227, 227); //TimeLine Label Background
		}

		public static void ChristmasTheme()
		{
			//This is a Christmas Theme
			_vixenThemeColors[0] = Color.FromArgb(222, 255, 223); //BackGroundColor
			_vixenThemeColors[3] = Color.FromArgb(0, 0, 0); //Foreground Color
			_vixenThemeColors[2] = Color.FromArgb(126, 126, 126); //ForeColorDisabled
			_vixenThemeColors[1] = Color.FromArgb(255, 121, 121); //Menu Selected Highlight Color
			_vixenThemeColors[4] = Color.FromArgb(231, 252, 231); //TextBox BackGround Color
			_vixenThemeColors[5] = Color.FromArgb(40, 40, 40); //Button Border
			_vixenThemeColors[6] = Color.FromArgb(136, 136, 136); //GroupBox Border Color
			_vixenThemeColors[7] = Color.FromArgb(231, 252, 231); //ComboBox Back Color
			_vixenThemeColors[8] = Color.FromArgb(231, 252, 231); //ListBox Back Color
			_vixenThemeColors[9] = Color.FromArgb(255, 220, 220); //Menu Back Color
			_vixenThemeColors[10] = Color.FromArgb(255, 121, 121); //Button Back Color
			_vixenThemeColors[11] = Color.FromArgb(255, 185, 185); //Button Back Color Hover
			_vixenThemeColors[12] = Color.FromArgb(231, 252, 231); //Numeric Back Color
			_vixenThemeColors[13] = Color.FromArgb(255, 121, 121); //ComboBox Highlight Color
			_vixenThemeColors[14] = Color.FromArgb(231, 252, 231); //Time Line Element Background
			_vixenThemeColors[15] = Color.FromArgb(0, 0, 0); //TimeLine Grid color
			_vixenThemeColors[16] = Color.FromArgb(255, 220, 220); //TimeLine Effects Background
			_vixenThemeColors[17] = Color.FromArgb(0, 0, 0); //TimeLine Text Forecolor
			_vixenThemeColors[18] = Color.FromArgb(255, 220, 220); //TimeLine Label Background
		}

		public static void HalloweenTheme()
		{
			//This is a Halloween Theme
			_vixenThemeColors[0] = Color.FromArgb(255, 228, 190); //BackGroundColor
			_vixenThemeColors[3] = Color.FromArgb(0, 0, 0); //Foreground Color
			_vixenThemeColors[2] = Color.FromArgb(119, 119, 119); //ForeColorDisabled
			_vixenThemeColors[1] = Color.FromArgb(232, 113, 113); //Menu Selected Highlight Color
			_vixenThemeColors[4] = Color.FromArgb(243, 243, 234); //TextBox BackGround Color
			_vixenThemeColors[5] = Color.FromArgb(40, 40, 40); //Button Border
			_vixenThemeColors[6] = Color.FromArgb(136, 136, 136); //GroupBox Border Color
			_vixenThemeColors[7] = Color.FromArgb(243, 243, 234); //ComboBox Back Color
			_vixenThemeColors[8] = Color.FromArgb(243, 243, 234); //ListBox Back Color
			_vixenThemeColors[9] = Color.FromArgb(255, 228, 190); //Menu Back Color
			_vixenThemeColors[10] = Color.FromArgb(232, 113, 113); //Button Back Color
			_vixenThemeColors[11] = Color.FromArgb(215, 147, 147); //Button Back Color Hover
			_vixenThemeColors[12] = Color.FromArgb(243, 243, 234); //Numeric Back Color
			_vixenThemeColors[13] = Color.FromArgb(255, 127, 127); //ComboBox Highlight Color
			_vixenThemeColors[14] = Color.FromArgb(60, 60, 60); //Time Line Element Background
			_vixenThemeColors[15] = Color.FromArgb(0, 0, 0); //TimeLine Grid color
			_vixenThemeColors[16] = Color.FromArgb(255, 228, 190); //TimeLine Effects Background
			_vixenThemeColors[17] = Color.FromArgb(0, 0, 0); //TimeLine Text Forecolor
			_vixenThemeColors[18] = Color.FromArgb(255, 228, 190); //TimeLine Label Background
		}

		public static void InitialLoadTheme()
		{
			//Used when Vixen is opened and will transfer stored colors to the ThemeColorTable for use throughout
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
			ThemeColorTable._timeLinePanel1BackColor = _vixenThemeColors[14];
			ThemeColorTable._timeLineGridColor = _vixenThemeColors[15];
			ThemeColorTable._timeLineEffectsBackColor = _vixenThemeColors[16];
			ThemeColorTable._timeLineForeColor = _vixenThemeColors[17];
			ThemeColorTable._timeLineLabelBackColor = _vixenThemeColors[18];
		}
	}
}
