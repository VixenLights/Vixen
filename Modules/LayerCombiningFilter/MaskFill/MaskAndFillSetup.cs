using System;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.LayerMixingFilter.MaskFill
{
	public partial class MaskAndFillSetup : BaseForm
	{
		public MaskAndFillSetup(bool excludeZeroValues)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			ExcludeZeroValuesValues = excludeZeroValues;
			chkExcludeZero.Checked = excludeZeroValues;
		}

		public bool ExcludeZeroValuesValues { get; private set; }

		private void chkExcludeZero_CheckedChanged(object sender, EventArgs e)
		{
			ExcludeZeroValuesValues = chkExcludeZero.Checked;
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
