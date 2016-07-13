using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Vixen.Module.Analysis;
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
		//used to provide color to various controls.
		//will move through each control and sub controls and adjust each control properties as required.
		public static void UpdateControls(Control control, List<Control> excludes = null)
		{
			control.Font = SystemFonts.MessageBoxFont;
			foreach (Control c in control.Controls)
			{
				if (excludes != null && excludes.Contains(c)) continue;
				c.Font = SystemFonts.MessageBoxFont;
				if (c is GroupBox | c is Panel | c is Label | c is ToolStripEx | c is ToolStrip | c is RadioButton | c is CheckBox | c is TreeView | c.ToString().Contains("PropertyGrid"))
				{
					c.ForeColor = ThemeColorTable.ForeColor;
					c.BackColor = ThemeColorTable.BackgroundColor;
				}
				if (c is Button)
				{
					Button btn = c as Button;
					btn.FlatStyle = FlatStyle.Flat;
					btn.FlatAppearance.BorderSize = 0;
					if (btn.BackgroundImage==null && btn.Image==null)
					{
						btn.BackgroundImageLayout = ImageLayout.Stretch;
						btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImage;
						btn.BackColor = Color.Transparent;
						btn.ForeColor = ThemeColorTable.ForeColor;
					}
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
					UpdateControls(c, excludes);
				}
			}
		}

		public static void UpdateButton(Button btn)
		{
			btn.FlatStyle = FlatStyle.Flat;
			btn.FlatAppearance.BorderSize = 0;
			if (btn.BackgroundImage == null && btn.Image == null)
			{
				btn.BackgroundImageLayout = ImageLayout.Stretch;
				btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImage;
				btn.BackColor = Color.Transparent;
				btn.ForeColor = ThemeColorTable.ForeColor;
			}
			
		}
	}
}
