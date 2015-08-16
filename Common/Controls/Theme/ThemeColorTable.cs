using System.Drawing;
using System.Windows.Forms;

namespace Common.Controls.Theme
{
	public class ThemeColorTable : ProfessionalColorTable
	{
		//Form
		public static Color _backgroundColor = Color.FromArgb(68, 68, 68);

		//Buttons
		public static Color _buttonBorderColor = Color.FromArgb(40, 40, 40);
		public static Color _buttonBackColor = Color.FromArgb(20, 20, 20);
		public static Color _buttonBackColorHover = Color.FromArgb(40, 40, 40);
		public static Color _buttonTextColor;

		//Text
		public static Color _foreColorDisabled = Color.FromArgb(119, 119, 119);
		public static Color _foreColor = Color.FromArgb(221, 221, 221);

		//TextBoxes
		public static Color _textBoxBackColor = Color.FromArgb(90, 90, 90);

		//ComboBoxes
		public static Color _comboBoxBackColor = Color.FromArgb(90, 90, 90);
		public static Color _comboBoxHighlightColor = Color.FromArgb(68, 68, 68);

		//GroupBox
		public static Color _groupBoxborderColor = Color.FromArgb(136, 136, 136);

		//ListBoxes
		public static Color _listBoxBackColor = Color.FromArgb(90, 90, 90);

		//NumericBoxes
		public static Color _numericBackColor = Color.FromArgb(90, 90, 90);

		//MenuStrips
		public static Color _menuSelectedHighlightBackColor = Color.FromArgb(68, 68, 68);
		public static Color _highlightColor = Color.FromArgb(90, 90, 90);

		//Sequence TimeLine
		public static Color _timeLinePanel1BackColor = Color.FromArgb(40, 40, 40);
		public static Color _timeLineGridColor = Color.FromArgb(0, 0, 0);
		public static Color _timeLineEffectsBackColor = Color.FromArgb(68, 68, 68);
		public static Color _timeLineForeColor = Color.FromArgb(221, 221, 221);
		public static Color _timeLineLabelBackColor = Color.FromArgb(68, 68, 68);

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

		//Timeline Element Panel1 Background
		public static Color TimeLinePanel1BackColor
		{
			get { return _timeLinePanel1BackColor; }
		}

		public static Color TimeLineGridColor
		{
			get { return _timeLineGridColor; }
		}

		public static Color TimeLineEffectsBackColor
		{
			get { return _timeLineEffectsBackColor; }
		}

		public static Color TimeLineForeColor
		{
			get { return _timeLineForeColor; }
		}

		public static Color TimeLineLabelBackColor
		{
			get { return _timeLineLabelBackColor; }
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