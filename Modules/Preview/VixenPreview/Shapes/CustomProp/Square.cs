using Common.Controls.Theme;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	public class Square
	{

		private Button _label;
		private Prop _prop;

		public Square() { }
		public Square(Prop prop, int x, int y, int id = 0)
		{
			_prop = prop;
			X = x;
			Y = y;
			_label = new Button();
			_label.Paint += button_Paint;
			int w = _prop.Panel.Width / _prop.Width;
			int h = _prop.Panel.Height / _prop.Height;
			ChannelID = id;


			_label.Width = w + 1;
			_label.Height = h + 1;
			_label.Left = w * X;
			_label.Top = h * Y;
			_label.MouseDown += new MouseEventHandler(ButtonClick);
			setButtonData();
			_prop.Panel.Controls.Add(Button);
		}

		public Button Button
		{
			get { return (this._label); }
		}

		private void setButtonData()
		{
			Button.Text = ChannelID > 0 ? ChannelID.ToString() : string.Empty;
			Button.ForeColor = ChannelID == 0 ? ThemeColorTable.ButtonTextColor : ThemeColorTable.ButtonBackColor;
			Button.BackColor = ChannelID == 0 ? ThemeColorTable.ButtonBackColor : Color.White;
			Button.Font = new System.Drawing.Font("Arial Black", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));

		}


		private void button_Paint(object sender, PaintEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(Button.Text)) return;
			float fontSize = NewFontSize(e.Graphics, Button.Bounds.Size, Button.Font, Button.Text);
			Font f = new Font("Arial", fontSize, FontStyle.Bold);
			Button.Font = f;
		}
		public static float NewFontSize(Graphics graphics, Size size, Font font, string str)
		{
			SizeF stringSize = graphics.MeasureString(str, font);
			float wRatio = (size.Width - 5) / stringSize.Width;			//The subtraction of the width/height values will account for the border 
			float hRatio = (size.Height - 5) / stringSize.Height;		//that was otherwise being ignored and the text size was wrong
			float ratio = Math.Min(hRatio, wRatio);
			return font.Size * ratio;
		}

		private void ButtonClick(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.Left:
					ChannelID = _prop.SelectedChannelId;
					//Button.Text = _prop.SelectedChannelId > 0 ? _prop.SelectedChannelId.ToString() : string.Empty;
					setButtonData();
					break;
				case MouseButtons.Right:
					//Button.Text = string.Empty;
					ChannelID = 0;
					setButtonData();
					break;

				default:
					break;
			}

		}


		public int ChannelID { get; set; }





		public int X
		{
			get;
			set;
		}

		public int Y
		{
			get;
			set;
		}


		public void RemoveEvents()
		{
			if (_label != null)
				_label.MouseDown -= new MouseEventHandler(ButtonClick);
		}


	}
}
