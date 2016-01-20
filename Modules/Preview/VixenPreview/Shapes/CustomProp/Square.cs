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

		private Button _button;
		private Prop _prop;

		public Square() { }
		public Square(Prop prop, int x, int y, int id = 0)
		{
			_prop = prop;
			X = x;
			Y = y;
			_button = new Button();

			int w = _prop.Panel.Width / _prop.Width;
			int h = _prop.Panel.Height / _prop.Height;
			ChannelID = id;


			_button.Width = w + 1;
			_button.Height = h + 1;
			_button.Left = w * X;
			_button.Top = h * Y;
			_button.Font = new System.Drawing.Font("Arial Black", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			_button.MouseDown += new MouseEventHandler(ButtonClick);
			setButtonData();
			_prop.Panel.Controls.Add(Button);
		}

		public Button Button
		{
			get { return (this._button); }
		}

		private void setButtonData()
		{
			Button.Text = ChannelID > 0 ? ChannelID.ToString() : string.Empty;
			Button.ForeColor = ChannelID == 0 ? ThemeColorTable.ButtonTextColor : ThemeColorTable.ButtonBackColor;
			Button.BackColor = ChannelID == 0 ? ThemeColorTable.ButtonBackColor : Color.White;
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

		public void Open()
		{

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
			if (_button != null)
				_button.MouseDown -= new MouseEventHandler(ButtonClick);
		}


	}
}
