using System.Drawing;
using System.Windows.Forms;
using Button = System.Windows.Forms.Button;
using CheckBox = System.Windows.Forms.CheckBox;
using ComboBox = System.Windows.Forms.ComboBox;
using Control = System.Windows.Forms.Control;
using GroupBox = System.Windows.Forms.GroupBox;
using Label = System.Windows.Forms.Label;
using ListBox = System.Windows.Forms.ListBox;
using ListView = System.Windows.Forms.ListView;
using Panel = System.Windows.Forms.Panel;
using RadioButton = System.Windows.Forms.RadioButton;
using TextBox = System.Windows.Forms.TextBox;
using TreeView = System.Windows.Forms.TreeView;

namespace Common.Controls.Theme
{
	public sealed class ThemeUpdateControls
	{
		//The following will move through each control and adjust each control properties as required.
		public static void UpdateControls(Control control)
		{
			foreach (Control c in control.Controls)
			{
				if (c is GroupBox | c is Panel | c is Label | c is ToolStripEx | c is ToolStrip | c is RadioButton | c is CheckBox | c is TreeView | c.ToString().Contains("PropertyGrid"))
				{
					c.ForeColor = ThemeColorTable.ForeColor;
					c.BackColor = ThemeColorTable.BackgroundColor;
				}
				if (c is Button)
				{
					Button btn = c as Button;
					btn.FlatStyle = FlatStyle.Flat;
					btn.FlatAppearance.BorderColor = ThemeColorTable.ButtonBorderColor;
					if (c.Width > 40)
					{
						btn.BackgroundImage = ThemeColorTable.newBackGroundImage ?? Resources.Properties.Resources.HeadingBackgroundImage;
						btn.BackgroundImageLayout = ImageLayout.Stretch;
						// The following code is used to set the backcolor of the button. The reason this is done is because when a button is
						// disabled it uses a darker shade of the backcolor to determine the disabled Text Color (Microsoft's fault)
						// As such this is the easiest way to get a disabled color to be the same as the Forecolor that is darker but not too dark. Other way whould be to repaint.
						// This also allows all button EnabledChanged events that were put in to be removed as they are no longer required.
						float correctionFactor = 0.8f; //This factor is used to increases the shade of the color for the diabled text color.
						float red = (255 - ThemeColorTable.ForeColor.R) * correctionFactor + ThemeColorTable.ForeColor.R;
						float green = (255 - ThemeColorTable.ForeColor.G) * correctionFactor + ThemeColorTable.ForeColor.G;
						float blue = (255 - ThemeColorTable.ForeColor.B) * correctionFactor + ThemeColorTable.ForeColor.B;
						Color buttonForeColorDisabled = Color.FromArgb(ThemeColorTable.ForeColor.A, (int)red, (int)green, (int)blue);
						btn.BackColor = buttonForeColorDisabled;
					}
					else
					{
						btn.FlatAppearance.BorderSize = 0;
					}
					c.ForeColor = ThemeColorTable.ForeColor;
				}
				if (c is TextBox & !c.ToString().Contains("UpDown"))
				{
					TextBox btn = c as TextBox;
					btn.ForeColor = ThemeColorTable.ForeColor;
					btn.BackColor = ThemeColorTable.TextBoxBackgroundColor;
					btn.BorderStyle = BorderStyle.FixedSingle;
				}
				if (c is MaskedTextBox & !c.ToString().Contains("UpDown"))
				{
					MaskedTextBox btn = c as MaskedTextBox;
					btn.ForeColor = ThemeColorTable.ForeColor;
					btn.BackColor = ThemeColorTable.TextBoxBackgroundColor;
					btn.BorderStyle = BorderStyle.FixedSingle;
				}
				if (c is ComboBox)
				{
					ComboBox btn = c as ComboBox;
					btn.BackColor = ThemeColorTable.ComboBoxBackColor;
					btn.ForeColor = ThemeColorTable.ForeColor;
					btn.FlatStyle = FlatStyle.Flat;
				}
				if (c.ToString().Contains("DateTimePicker"))
				{
					DateTimePicker btn = c as DateTimePicker;
					btn.CalendarTitleBackColor = ThemeColorTable.TextBoxBackgroundColor;
					btn.CalendarTitleForeColor = ThemeColorTable.ForeColor;
				}
				if (c is ListBox | c is ListView | c is MultiSelectTreeview)
				{
					c.BackColor = ThemeColorTable.ListBoxBackColor;
					c.ForeColor = ThemeColorTable.ForeColor;
				}
				if (c.ToString().Contains("NumericUpDown"))
				{
					c.BackColor = ThemeColorTable.NumericBackColor;
					c.ForeColor = ThemeColorTable.ForeColor;
				}
				if (c.Controls.Count > 0)
				{
					UpdateControls(c);
				}
			}

		}
	}
}
