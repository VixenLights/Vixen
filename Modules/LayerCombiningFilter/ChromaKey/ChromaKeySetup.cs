using System;
using System.Windows.Forms;
using System.Drawing;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.LayerMixingFilter.ChromaKey
{
	public partial class ChromaKeySetup : BaseForm
	{
		public ChromaKeySetup(bool excludeZeroValues,int lowerLimit, int upperLimit, Color keyColor)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			ExcludeZeroValuesValues = excludeZeroValues;
			chkExcludeZero.Checked = excludeZeroValues;
		    LowerLimit = lowerLimit;
		    UpperLimit = upperLimit;
		    UpdateLimitControls();
		    colorPanel1.Color = keyColor;
		}

		public bool ExcludeZeroValuesValues { get; private set; }

        public int LowerLimit { get; private set; }
	    public int UpperLimit { get; private set; }
        public Color KeyColor { get; private set; }

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

	    private void trkLowerLimit_Scroll(object sender, EventArgs e)
	    {
	        if (trkLowerLimit.Value < UpperLimit)
	        {
	            LowerLimit = trkLowerLimit.Value;
            }
	        else
	        {
	            LowerLimit = UpperLimit - 1;
	            trkLowerLimit.Value = LowerLimit;
	        }
	        numLowerLimit.Text = Convert.ToString(LowerLimit);
        }

        private void trkUpperLimit_Scroll(object sender, EventArgs e)
        {
            if (trkUpperLimit.Value > LowerLimit )
            {
                UpperLimit = trkUpperLimit.Value;
            }
            else
            {
                UpperLimit = LowerLimit + 1;
                trkUpperLimit.Value = UpperLimit;
            }
            numUpperLimit.Text = Convert.ToString(UpperLimit);          
        }

	    private void numLowerLimit_LostFocus(object sender, EventArgs e)
	    {
	        if ( (numLowerLimit.IntValue < UpperLimit) && numLowerLimit.IntValue >= 0)
	        {
	            LowerLimit = numLowerLimit.IntValue;
	        }
	        else
	        {
	            LowerLimit = UpperLimit - 1;
	            numLowerLimit.Text = Convert.ToString(LowerLimit);
	        }
	        trkLowerLimit.Value = LowerLimit;
	    }

	    private void numUpperLimit_LostFocus(object sender, EventArgs e)
	    {
	        if (numUpperLimit.IntValue > LowerLimit && numUpperLimit.IntValue <= 100 )
	        {
	            UpperLimit = numUpperLimit.IntValue;
	        }
	        else
	        {
	            UpperLimit = LowerLimit + 1;
	            numUpperLimit.Text = Convert.ToString(UpperLimit);
	        }
	        trkUpperLimit.Value = UpperLimit;
	    }

	    private void UpdateLimitControls()
	    {
	        trkLowerLimit.Value = LowerLimit;
	        trkUpperLimit.Value = UpperLimit;
	        numLowerLimit.Text = Convert.ToString(LowerLimit);
	        numUpperLimit.Text = Convert.ToString(UpperLimit);
        }

        private void colorPanel1_Load(object sender, EventArgs e)
        {

        }

	    private void colorPanel1_ColorChanged(object sender, EventArgs e)
	    {
	        KeyColor = colorPanel1.Color;
	    }
    }
}