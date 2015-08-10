using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;

namespace Common.Controls.Theme
{
	public partial class ThemeMainForm : Form
	{
		//Sets the file path for the VixenTheme.xml to the Roaming Vixen folder
		public static string _vixenThemePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen",
			"VixenTheme.xml");

		public ThemeMainForm()
		{
			InitializeComponent();
			//Renders the theme to the form
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			label16.ForeColor = ThemeColorTable.ForeColorDisabled; //used to display a disabled label
			
			Icon = Resources.Properties.Resources.Icon_Vixen3;
			textBox1.Text = "This is a Preview of the text";

			comboBoxThemes.SelectedIndex = comboBoxThemes.Items.Count - 1;
			comboBox1.SelectedIndex = 0;
		}

		private void ThemeControl_Load(object sender, EventArgs e)
		{
			loadTheme();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			SaveTheme();
			Close();
		}

		private void loadTheme()
		{
			//adds the theme colors to the Color Panels on the form for users to see.
			var i = 0;
			var _colorPanel = new[] { pictureBox0, pictureBox1, pictureBox2, pictureBox3, pictureBox4,
					pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11, pictureBox12, pictureBox13, pictureBox14, pictureBox15, pictureBox16, pictureBox17, pictureBox18 };

			foreach (var c in _colorPanel)
			{
				c.BackColor = ThemeLoadColors._vixenThemeColors[i];
				i++;
			}
		}


		private void SaveTheme()
		{
			var i = 0;
			var _colorPanel = new[] { pictureBox0, pictureBox1, pictureBox2, pictureBox3, pictureBox4,
					pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11, pictureBox12, pictureBox13, pictureBox14, pictureBox15, pictureBox16, pictureBox17, pictureBox18  };
			foreach (var c in _colorPanel)
			{
				ThemeLoadColors._vixenThemeColors[i] = c.BackColor;
				i++;
			}

			var xmlsettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};

			DataContractSerializer dataSer = new DataContractSerializer(typeof(Color[]));
			var dataWriter = XmlWriter.Create(_vixenThemePath, xmlsettings);
			dataSer.WriteObject(dataWriter, ThemeLoadColors._vixenThemeColors);
			dataWriter.Close();

			ThemeColorTable._backgroundColor = ThemeLoadColors._vixenThemeColors[0];
			ThemeColorTable._menuSelectedHighlightBackColor = ThemeLoadColors._vixenThemeColors[1];
			ThemeColorTable._foreColorDisabled = ThemeLoadColors._vixenThemeColors[2];
			ThemeColorTable._foreColor = ThemeLoadColors._vixenThemeColors[3];
			ThemeColorTable._textBoxBackColor = ThemeLoadColors._vixenThemeColors[4];
			ThemeColorTable._buttonBorderColor = ThemeLoadColors._vixenThemeColors[5];
			ThemeColorTable._groupBoxborderColor = ThemeLoadColors._vixenThemeColors[6];
			ThemeColorTable._comboBoxBackColor = ThemeLoadColors._vixenThemeColors[7];
			ThemeColorTable._listBoxBackColor = ThemeLoadColors._vixenThemeColors[8];
			ThemeColorTable._highlightColor = ThemeLoadColors._vixenThemeColors[9];
			ThemeColorTable._buttonBackColor = ThemeLoadColors._vixenThemeColors[10];
			ThemeColorTable._buttonBackColorHover = ThemeLoadColors._vixenThemeColors[11];
			ThemeColorTable._numericBackColor = ThemeLoadColors._vixenThemeColors[12];
			ThemeColorTable._comboBoxHighlightColor = ThemeLoadColors._vixenThemeColors[13];
			ThemeColorTable._timeLinePanel1BackColor = ThemeLoadColors._vixenThemeColors[14];
			ThemeColorTable._timeLineGridColor = ThemeLoadColors._vixenThemeColors[15];
			ThemeColorTable._timeLineEffectsBackColor = ThemeLoadColors._vixenThemeColors[16];
			ThemeColorTable._timeLineForeColor = ThemeLoadColors._vixenThemeColors[17];
			ThemeColorTable._timeLineLabelBackColor = ThemeLoadColors._vixenThemeColors[18];
		}

		private void comboBoxThemes_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (comboBoxThemes.SelectedItem.ToString())
			{
				case "Dark (Default)":
					ThemeLoadColors.DarkTheme();
					ThemeChanged();
					break;
				case "Windows":
					ThemeLoadColors.WindowsTheme();
					ThemeChanged();
					break;
				case "Christmas":
					ThemeLoadColors.ChristmasTheme();
					ThemeChanged();
					break;
				case "Halloween":
					ThemeLoadColors.HalloweenTheme();
					ThemeChanged();
					break;
			}
		}

		private void ThemeChanged()
		{
			loadTheme();
			SaveTheme();
			update();
			Refresh();
		}

		private void update()
		{
			var i = 0;
			var _colorPanel = new[]
				{
					pictureBox0, pictureBox1, pictureBox2, pictureBox3, pictureBox4,
					pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11, pictureBox12, pictureBox13, pictureBox14, pictureBox15, pictureBox16, pictureBox17, pictureBox18
				};
			foreach (var c in _colorPanel)
			{
				ThemeLoadColors._vixenThemeColors[i] = c.BackColor;

				i++;
			}
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			label16.ForeColor = ThemeColorTable.ForeColorDisabled;
			loadTheme();
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}

		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}

		private void button_Paint(object sender, PaintEventArgs e)
		{
			ThemeButtonRenderer.OnPaint(sender, e, null);
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			ThemeButtonRenderer.ButtonHover = true;
			var btn = sender as Button;
			btn.Invalidate();
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			ThemeButtonRenderer.ButtonHover = false;
			var btn = sender as Button;
			btn.Invalidate();
		}

		private void selectColor_Click(object sender, EventArgs e)
		{
			colorPicker(sender, e);
			SaveTheme();
			update();
			Refresh();
		}

		private void colorPicker(object sender, EventArgs e)
		{
			var currentColor = sender as PictureBox;
			using (ColorPicker cp = new ColorPicker())
			{
				cp.LockValue_V = false;
				cp.Color = XYZ.FromRGB(currentColor.BackColor);
				DialogResult result = cp.ShowDialog();
				if (result != DialogResult.OK) return;
				Color colorValue = cp.Color.ToRGB().ToArgb();

				PictureBox btn = sender as PictureBox;
				btn.BackColor = colorValue;
				comboBoxThemes.SelectedIndex = comboBoxThemes.Items.Count - 1;
			}
		}
	}
}
