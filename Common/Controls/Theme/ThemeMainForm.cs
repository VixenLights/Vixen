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
			ColorButtonChange();
			if (ThemeLoadColors._vixenThemeColors[14] == Color.Black) //This is used as a test to see if the Dark Button background is used.
			{
				ThemeColorTable.newBackGroundImage = null; //Button Back Color
				ThemeColorTable.newBackGroundImageHover = null; //Button Back Color hover
			}
			//Renders the theme to the form
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			label16.ForeColor = ThemeColorTable.ForeColorDisabled; //used to display a disabled label
			
			Icon = Resources.Properties.Resources.Icon_Vixen3;
			textBox1.Text = "This is a Preview of the text";

			comboBoxThemes.SelectedIndex = 3;
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
					pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11, pictureBox12, pictureBox13, pictureBox14 };

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
					pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11, pictureBox12, pictureBox13, pictureBox14  };
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
					ColorButtonChange();
					ThemeChanged();
					break;
				case "Light":
					ThemeLoadColors.LightTheme();
					ColorButtonChange();
					ThemeChanged();
					break;
			}
			ThemeLoadColors._vixenThemeColors[14] = Color.White; //This is used as a test to see if the Dark Button background is used.
		}

		private void ThemeChanged()
		{
			loadTheme();
			SaveTheme();
			update();
			Refresh();
		}

		private void colorChange(object sender, EventArgs e)
		{
			comboBoxThemes.SelectedIndex = 3;
			ThemeLoadColors._vixenThemeColors[14] = Color.White; //This is used as a test to see if the Dark Button background is used.
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
					pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11, pictureBox12, pictureBox13, pictureBox14
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

		//Used to redraw the Background image for button a differnt color.
		#region Redraw Background image for Button

		private void colorButtonChange(object sender, EventArgs e)
		{
			comboBoxThemes.SelectedIndex = 3;
			pictureBox14.BackColor = Color.White; //Button Default Background
			update();
			ColorButtonChange();
			SaveTheme();
			ThemeUpdateControls.UpdateControls(this);
			label16.ForeColor = ThemeColorTable.ForeColorDisabled;
		}

		public static void ColorButtonChange()
		{
			Color color = ThemeLoadColors._vixenThemeColors[10]; //Your desired colour

			byte r = color.R; //For Red colour
			byte g = color.G; //For Red colour
			byte b = color.B; //For Red colour

			Bitmap bmp = new Bitmap(Resources.Properties.Resources.HeadingBackgroundImage);
			for (int x = 0; x < bmp.Width; x++)
			{
				for (int y = 0; y < bmp.Height; y++)
				{
					Color gotColor = bmp.GetPixel(x, y);
					gotColor = Color.FromArgb(r, g, b);
					bmp.SetPixel(x, y, gotColor);
				}
			}
			ThemeColorTable.newBackGroundImage = bmp;

			Color color1 = ThemeLoadColors._vixenThemeColors[11]; //Your desired colour

			byte r1 = color1.R; //For Red colour
			byte g1 = color1.G; //For Red colour
			byte b1 = color1.B; //For Red colour

			Bitmap bmp1 = new Bitmap(Resources.Properties.Resources.HeadingBackgroundImage);
			for (int x = 0; x < bmp1.Width; x++)
			{
				for (int y = 0; y < bmp1.Height; y++)
				{
					Color gotColor1 = bmp1.GetPixel(x, y);
					gotColor1 = Color.FromArgb(r1, g1, b1);
					bmp1.SetPixel(x, y, gotColor1);
				}
			}
			ThemeColorTable.newBackGroundImageHover = bmp1;
		}
		#endregion

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}

		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = ThemeColorTable.newBackGroundImageHover ?? Resources.Properties.Resources.HeadingBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = ThemeColorTable.newBackGroundImage ?? Resources.Properties.Resources.HeadingBackgroundImage;
		}

		private void selectColor_Click(object sender, EventArgs e)
		{
			colorPicker(sender, e);
			ThemeLoadColors._vixenThemeColors[14] = Color.White; //This is used as a test to see if the Dark Button background is used.
			SaveTheme();
			update();
			Refresh();
		}

		private void selectButtonColor_Click(object sender, EventArgs e)
		{
			colorPicker(sender, e);
			pictureBox14.BackColor = Color.White; //Button Default Background
			update();
			ColorButtonChange();
			SaveTheme();
			ThemeUpdateControls.UpdateControls(this);
			label16.ForeColor = ThemeColorTable.ForeColorDisabled;
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
				comboBoxThemes.SelectedIndex = 3;
			}
		}

		private PushButtonState state = PushButtonState.Normal;
		private void button2_Paint(object sender, PaintEventArgs e)
		{
			base.OnPaint(e);
			var btn = sender as Button;
			StringFormat _sf = new StringFormat();
			_sf.Alignment = StringAlignment.Center;
			_sf.LineAlignment = StringAlignment.Center;
			Graphics g = e.Graphics;
			Brush _paintBrush = new LinearGradientBrush(btn.ClientRectangle, Color.FromArgb(60,60,60), Color.FromArgb(15,15,15),
					LinearGradientMode.Vertical);
			g.FillRectangle(_paintBrush, btn.ClientRectangle);
			PointF _centerPoint = new PointF(btn.Width / 2, btn.Height / 2);
			g.DrawString(btn.Text, btn.Font, (new SolidBrush(ThemeColorTable.ForeColor)), _centerPoint.X, _centerPoint.Y, _sf);

			paint_Border(btn, e);
		}

		private void paint_Border(Button btn, PaintEventArgs e)
		{
			if (e == null)
				return;
			if (e.Graphics == null)
				return;
			Pen pen = new Pen(ThemeColorTable.ButtonBorderColor, 1);
			Point[] pts = border_Get(0, 0, btn.Width - 1, btn.Height - 1);
			e.Graphics.DrawLines(pen, pts);
			pen.Dispose();
		}

		private Point[] border_Get(int nLeftEdge, int nTopEdge, int nWidth, int nHeight)
		{
			int X = nWidth;
			int Y = nHeight;
			Point[] points =
			{
				new Point(1, 0),
				new Point(X - 1, 0),
				new Point(X - 1, 1),
				new Point(X, 1),
				new Point(X, Y - 1),
				new Point(X - 1, Y - 1),
				new Point(X - 1, Y),
				new Point(1, Y),
				new Point(1, Y - 1),
				new Point(0, Y - 1),
				new Point(0, 1),
				new Point(1, 1)
			};
			for (int i = 0; i < points.Length; i++)
			{
				points[i].Offset(nLeftEdge, nTopEdge);
			}
			return points;
		}

		private void button2_Click(object sender, EventArgs e)
		{

		}
	}
}
