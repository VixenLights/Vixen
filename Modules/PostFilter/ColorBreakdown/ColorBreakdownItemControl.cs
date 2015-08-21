using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Data.Value;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	public partial class ColorBreakdownItemControl : UserControl
	{
		public ColorBreakdownItemControl()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			panelColor.BackColor = Color.White;
			textBoxName.Text = "New Color";
		}

		public ColorBreakdownItemControl(ColorBreakdownItem breakdownItem)
		{
			InitializeComponent();
			panelColor.BackColor = breakdownItem.Color;
			textBoxName.Text = breakdownItem.Name;
		}

		public ColorBreakdownItemControl(Color color, string name)
		{
			InitializeComponent();
			panelColor.BackColor = color;
			textBoxName.Text = name;
		}

		public ColorBreakdownItem ColorBreakdownItem
		{
			get
			{
				ColorBreakdownItem result = new ColorBreakdownItem();
				result.Name = textBoxName.Text;
				result.Color = panelColor.BackColor;
				return result;
			}
		}


		public event EventHandler DeleteRequested;

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (DeleteRequested != null)
				DeleteRequested(this, EventArgs.Empty);
		}

		private void panelColor_Click(object sender, EventArgs e)
		{
			using (ColorPicker cp = new ColorPicker()) {
				cp.Color = XYZ.FromRGB(panelColor.BackColor);
				DialogResult result = cp.ShowDialog();
				if (result == DialogResult.OK) {
					panelColor.BackColor = cp.Color.ToRGB().ToArgb();
				}
			}
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