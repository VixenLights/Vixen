using System;
using System.Windows.Forms;
using Common.Controls.Theme;

namespace Common.Controls.ConfigureElements
{
	public partial class AddPixelGrid : Form
	{
		public AddPixelGrid()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		public string GridName
		{
			get { return textGridName.Text; }
		}

		public int PixelsPerString
		{
			get { return Convert.ToInt32(LightsPerString.Value); }
		}

		public int StringCount
		{
			get { return Convert.ToInt32(Strings.Value); }
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
	}
}