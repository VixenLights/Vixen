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
		public static Font StandardFont = SystemFonts.MessageBoxFont;

		//used to provide color to various controls.
		//will move through each control and sub controls and adjust each control properties as required.
		public static void UpdateControls(Control control, List<Control> excludes = null)
		{
			control.Font = StandardFont;
			control.ForeColor = ThemeColorTable.ForeColor;
			control.BackColor = ThemeColorTable.BackgroundColor;
			if (control is Form form)
			{
				form.Icon = Resources.Properties.Resources.Icon_Vixen3;
			}
			foreach (Control c in control.Controls)
			{
				if (excludes != null && excludes.Contains(c)) continue;
				c.Font = StandardFont;
				if (c is Label l)
				{
					if (c.Tag?.ToString()?.Contains("KEEP_FORECOLOR") == false)
						l.ForeColor = ThemeColorTable.ForeColor;
					l.BackColor = ThemeColorTable.BackgroundColor;
					if (l is LinkLabel ll)
					{
						ll.LinkColor = ThemeColorTable.LinkColor;
					}
					continue;
				}
				if (c is GroupBox | c is Panel | c is ToolStripEx |c is ToolStrip |  c is RadioButton | c is CheckBox | c is TreeView | c.ToString().Contains("PropertyGrid"))
				{
					c.ForeColor = ThemeColorTable.ForeColor;
					c.BackColor = ThemeColorTable.BackgroundColor;

					if (c.Controls.Count > 0)
					{
						UpdateControls(c, excludes);
					}
					continue;
				}
				if (c is Button)
				{
					Button btn = c as Button;
					btn.FlatStyle = FlatStyle.Flat;
					btn.FlatAppearance.BorderSize = 0;

					// If this is a button with no image  or  button with text with or without an image
					if ((btn.BackgroundImage == null && btn.Image == null) || btn.Text?.Length > 0)
					{
						btn.BackgroundImageLayout = ImageLayout.Stretch;
						btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImage;
						btn.BackColor = Color.Transparent;
						btn.ForeColor = ThemeColorTable.ForeColor;

						// Install mouse standard events.
						btn.MouseLeave += new System.EventHandler(buttonBackground_MouseLeave);
						btn.MouseHover += new System.EventHandler(buttonBackground_MouseHover);
					}
					else
					{
						btn.FlatAppearance.MouseOverBackColor = ThemeColorTable.ButtonBackColorHover;
						btn.FlatAppearance.MouseDownBackColor = ThemeColorTable.ButtonDownBackColor;
					}
					continue;
				}
				if (c is TextBox & !c.ToString().Contains("UpDown"))
				{
					TextBox btn = c as TextBox;
					btn.ForeColor = ThemeColorTable.ForeColor;
					btn.BackColor = ThemeColorTable.TextBoxBackgroundColor;
					if (btn.ReadOnly)
					{
						btn.BackColor = ThemeColorTable.BackgroundColor;
					}
					if (btn.BorderStyle != BorderStyle.None)
					{
						btn.BorderStyle = BorderStyle.FixedSingle;
					}
					continue;
				}
	
				if (c is RichTextBox)
				{
					RichTextBox txt = c as RichTextBox;
					txt.ForeColor = ThemeColorTable.ForeColor;
					txt.BackColor = ThemeColorTable.TextBoxBackgroundColor;
					if (txt.BorderStyle != BorderStyle.None)
					{
						txt.BorderStyle = BorderStyle.FixedSingle;
					}
					continue;
				}
				if (c is MaskedTextBox & !c.ToString().Contains("UpDown"))
				{
					MaskedTextBox btn = c as MaskedTextBox;
					btn.ForeColor = ThemeColorTable.ForeColor;
					btn.BackColor = ThemeColorTable.TextBoxBackgroundColor;
					btn.BorderStyle = BorderStyle.FixedSingle;
					continue;
				}
				if (c is ComboBox)
				{
					ComboBox btn = c as ComboBox;
					btn.BackColor = ThemeColorTable.ComboBoxBackColor;
					btn.ForeColor = ThemeColorTable.ForeColor;
					btn.FlatStyle = FlatStyle.Flat;
					continue;
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

		private static void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = sender as Button;
			btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImageHover;
		}

		private static void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = sender as Button;
			btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImage;
		}
	}
}
