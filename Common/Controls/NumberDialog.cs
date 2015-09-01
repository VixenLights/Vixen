using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Theme;

namespace Common.Controls
{
	public partial class NumberDialog : Form
	{
		public NumberDialog(string title, string prompt, int value, int minimum = 0, int maximum = int.MaxValue)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			
			numericUpDownChooser.Minimum = minimum;
			numericUpDownChooser.Maximum = maximum;
            numericUpDownChooser.Value = value;
			Text = title;
			labelPrompt.Text = prompt;
		}

		public int Value
		{
			get { return decimal.ToInt32(numericUpDownChooser.Value); }
		}

		private void numericUpDownChooser_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) {
				DialogResult = DialogResult.OK;
			} else if (e.KeyCode == Keys.Escape) {
				DialogResult = DialogResult.Cancel;
			}
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImage;

		}
	}
}