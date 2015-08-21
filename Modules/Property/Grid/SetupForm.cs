using System;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Property.Grid
{
	internal partial class SetupForm : Form
	{
		private int _productRequired;

		public SetupForm(int width, int height, int productRequired)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			_Width = width;
			_Height = height;
			_ProductRequired = productRequired;
		}

		public int SelectedWidth { get; private set; }

		public int SelectedHeight { get; private set; }

		private int _Width
		{
			get { return (int) nudWidth.Value; }
			set { nudWidth.Value = value; }
		}

		private int _Height
		{
			get { return (int) nudHeight.Value; }
			set { nudHeight.Value = value; }
		}

		private int _ProductRequired
		{
			get { return _productRequired; }
			set
			{
				_productRequired = value;
				labelProduct.Text = "The product of these needs to be " + value;
			}
		}

		private void _CheckProduct()
		{
			bool productIsCorrect = _Width*_Height == _ProductRequired;
			labelProduct.Visible = !productIsCorrect;
			buttonOK.Enabled = productIsCorrect;
		}

		private void nudWidth_ValueChanged(object sender, EventArgs e)
		{
			_CheckProduct();
		}

		private void nudHeight_ValueChanged(object sender, EventArgs e)
		{
			_CheckProduct();
		}

		private void nudWidth_Leave(object sender, EventArgs e)
		{
			_CheckProduct();
		}

		private void nudHeight_Leave(object sender, EventArgs e)
		{
			_CheckProduct();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			SelectedWidth = _Width;
			SelectedHeight = _Height;
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;

		}
	}
}