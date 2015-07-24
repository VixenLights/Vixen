using System.Drawing;
using System.Windows.Forms;

namespace Common.Controls.Theme
{
	public class DarkThemeColorTable : ProfessionalColorTable
	{
		private static readonly Color _borderColor = Color.FromArgb(136, 136, 136);
		private static readonly Color _selectedHighlightColor = _borderColor;
		private static readonly Color _backgroundColor = Color.FromArgb(68, 68, 68);
		private static readonly Color _highlightColor = Color.FromArgb(90, 90, 90);
		private static readonly Color _foreColor = Color.FromArgb(221, 221, 221);
		private static readonly Color _foreColorDisabled = Color.FromArgb(119, 119, 119);

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
			get { return _highlightColor; }
		}
		public override Color MenuItemBorder
		{
			get { return _borderColor; }
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
			get { return _selectedHighlightColor; }
		}

		public override Color ButtonSelectedGradientBegin
		{
			get { return _highlightColor; }
		}

		public override Color ButtonSelectedGradientEnd
		{
			get { return _highlightColor; }
		} 

		public static Color BorderColor
		{
			get { return _borderColor; }
		}

		public static Color SelectedHighlightColor
		{
			get { return _selectedHighlightColor; }
		}

		public static Color BackgroundColor
		{
			get { return _backgroundColor; }
		}

		public static Color HighlightColor
		{
			get { return _highlightColor; }
		}

		public static Color ForeColor
		{
			get { return _foreColor; }
		}

		public static Color ForeColorDisabled
		{
			get { return _foreColorDisabled; }
		}
	}
}