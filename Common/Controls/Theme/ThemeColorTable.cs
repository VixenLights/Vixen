using System.Drawing;
using System.Windows.Forms;

namespace Common.Controls.Theme
{
	public class ThemeColorTable : ProfessionalColorTable
	{


		public static Bitmap newBackGroundImage = null;
		public static Bitmap newBackGroundImageHover = null;

		//Form
		public static Color _backgroundColor;

		//Buttons
		public static Color _buttonBorderColor;
		public static Color _buttonBackColor;
		public static Color _buttonBackColorHover;
		public static Color _buttonTextColor;

		//Text
		public static Color _foreColorDisabled;
		public static Color _foreColor;

		//TextBoxes
		public static Color _textBoxBackColor;

		//ComboBoxes
		public static Color _comboBoxBackColor;
		public static Color _comboBoxHighlightColor;

		//GroupBox
		public static Color _groupBoxborderColor;

		//ListBoxes
		public static Color _listBoxBackColor;

		//NumericBoxes
		public static Color _numericBackColor;

		//MenuStrips
		public static Color _menuSelectedHighlightBackColor;
		public static Color _highlightColor;

		//Forms
		public static Color BackgroundColor
		{
			get { return _backgroundColor; }
		}
		
		//Buttons
		public static Color ButtonBorderColor
		{
			get { return _buttonBorderColor; }
		}

		public static Color ButtonBackColor
		{
			get { return _buttonBackColor; }
		}

		public static Color ButtonBackColorHover
		{
			get { return _buttonBackColorHover; }
		}

		public static Color ButtonTextColor
		{
			get { return _buttonTextColor; }
		}

		//Text
		public static Color ForeColorDisabled
		{
			get { return _foreColorDisabled; }
		}

		public static Color ForeColor
		{
			get { return _foreColor; }
		}

		//TextBoxes
		public static Color TextBoxBackgroundColor
		{
			get { return _textBoxBackColor; }
		}

		//ComboBoxes
		public static Color ComboBoxBackColor
		{
			get { return _comboBoxBackColor; }
		}

		public static Color ComboBoxHighlightColor
		{
			get { return _comboBoxHighlightColor; }
		}

		public static Color ComboBoxborderColor
		{
			get { return _groupBoxborderColor; }
		}

		//GroupBoxes
		public static Color GroupBoxBorderColor
		{
			get { return _groupBoxborderColor; }
		}


		//ListBoxes
		public static Color ListBoxBackColor
		{
			get { return _listBoxBackColor; }
		}

		//NumericBoxes
		public static Color NumericBackColor
		{
			get { return _numericBackColor; }
		}

		//MenuStrips

		public override Color MenuItemSelected
		{
			get { return _backgroundColor; }
		}
		public override Color MenuItemSelectedGradientBegin
		{
			get { return _highlightColor; }
		}
		public override Color MenuItemSelectedGradientEnd
		{
			get { return _backgroundColor; }
		}
		public override Color MenuItemBorder
		{
			get { return _groupBoxborderColor; }
		}

		public override Color MenuStripGradientBegin
		{
			get { return _backgroundColor; }
		}

		public override Color MenuStripGradientEnd
		{
			get { return _backgroundColor; }
		}

		public override Color ToolStripGradientBegin
		{
			get { return _backgroundColor; }
		}

		public override Color ToolStripGradientEnd
		{
			get { return _backgroundColor; }
		}

		public override Color ToolStripPanelGradientBegin
		{
			get { return _backgroundColor; }
		}

		public override Color ToolStripPanelGradientEnd
		{
			get { return _backgroundColor; }
		}

		public override Color ToolStripContentPanelGradientBegin
		{
			get { return _backgroundColor; }
		}

		public override Color ToolStripContentPanelGradientEnd
		{
			get { return _backgroundColor; }
		}

		public override Color SeparatorDark 
		{
			get { return _backgroundColor; }
		}

		public override Color SeparatorLight
		{
			get { return _groupBoxborderColor; }
		}

		public override Color StatusStripGradientBegin
		{
			get { return _backgroundColor; }
		}

		public override Color StatusStripGradientEnd
		{
			get { return _backgroundColor; }
		}

		public override Color ToolStripBorder
		{
			get { return _groupBoxborderColor; }
		}

		public override Color ImageMarginGradientBegin
		{
			get { return _highlightColor; }
		}

		public override Color ImageMarginGradientEnd
		{
			get { return _highlightColor; }
		}

		public override Color ImageMarginGradientMiddle
		{
			get { return _highlightColor; }
		}

		public override Color MenuItemPressedGradientBegin
		{
			get { return _highlightColor; }
		}

		public override Color MenuItemPressedGradientMiddle
		{
			get { return _highlightColor; }
		}

		public override Color MenuItemPressedGradientEnd
		{
			get { return _highlightColor; }
		}
		public override Color ToolStripDropDownBackground
		{
			get { return _highlightColor; }
		}

		public override Color CheckBackground
		{
			get { return _highlightColor; }
		}

		public override Color ButtonSelectedHighlight
		{
			get { return _highlightColor; }
		}

		public override Color CheckSelectedBackground
		{
			get { return _backgroundColor; }
		}

		public override Color ButtonPressedHighlight
		{
			get { return _highlightColor; }
		}

		public override Color CheckPressedBackground
		{
			get { return _backgroundColor; }
		}

		public override Color ButtonCheckedHighlight
		{

			get { return _highlightColor; }
		}

		public override Color ButtonSelectedBorder
		{
			get { return _menuSelectedHighlightBackColor; }
		}

		public override Color ButtonSelectedGradientBegin
		{
			get { return _highlightColor; }
		}

		public override Color ButtonSelectedGradientEnd
		{
			get { return _highlightColor; }
		}

		public override Color GripLight
		{
			get { return _groupBoxborderColor; }
		}

		public override Color GripDark
		{
			get { return _backgroundColor; }
		}

		public static Color SelectedHighlightColor
		{
			get { return _menuSelectedHighlightBackColor; }
		}

		public static Color HighlightColor
		{
			get { return _highlightColor; }
		}
	}
}