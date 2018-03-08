using System;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.LayerMixingFilter.LumaKey
{
	public partial class LumaKeySetup : BaseForm
	{
		public LumaKeySetup(double lowerLimit, double upperLimit)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
		    LowerLimit = lowerLimit;
		    UpperLimit = upperLimit;
		    UpdateLimitControls();
		}

        public double LowerLimit { get; private set; }
	    public double UpperLimit { get; private set; }

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
	        if (trkLowerLimit.Value < UpperLimit * 100)
	        {
	            LowerLimit = trkLowerLimit.Value / 100d;
            }
	        else
	        {
	            LowerLimit = UpperLimit - .01;
	            trkLowerLimit.Value = (int)(LowerLimit*100);
	        }
	        numLowerLimit.Text = (100 * LowerLimit).ToString();
        }

        private void trkUpperLimit_Scroll(object sender, EventArgs e)
        {
            if (trkUpperLimit.Value > LowerLimit * 100)
            {
                UpperLimit = trkUpperLimit.Value / 100d;
            }
            else
            {
                UpperLimit = LowerLimit + .01;
                trkUpperLimit.Value = (int)(UpperLimit*100);
            }
            numUpperLimit.Text = (100 * UpperLimit).ToString();          
        }

	    private void numLowerLimit_LostFocus(object sender, EventArgs e)
	    {
	        if ( (numLowerLimit.IntValue < UpperLimit*100) && numLowerLimit.IntValue >= 0)
	        {
	            LowerLimit = numLowerLimit.IntValue / 100d;
	        }
	        else
	        {
	            LowerLimit = UpperLimit - .01;
	            numLowerLimit.Text = (LowerLimit * 100).ToString();
	        }
	        trkLowerLimit.Value = (int)(LowerLimit * 100);
	    }

	    private void numUpperLimit_LostFocus(object sender, EventArgs e)
	    {
	        if (numUpperLimit.IntValue > LowerLimit*100 && numUpperLimit.IntValue <= 100 )
	        {
	            UpperLimit = numUpperLimit.IntValue / 100d;
	        }
	        else
	        {
	            UpperLimit = LowerLimit + .01;
	            numUpperLimit.Text = (UpperLimit * 100).ToString();
	        }
	        trkUpperLimit.Value = (int)(UpperLimit * 100);
	    }

	    private void UpdateLimitControls()
	    {
	        var lowerLimit = LowerLimit * 100;
	        var upperLimit = UpperLimit * 100;
	        
	        trkLowerLimit.Value = (int)lowerLimit;
	        trkUpperLimit.Value = (int)upperLimit;
	        numLowerLimit.Text = lowerLimit.ToString();
	        numUpperLimit.Text = upperLimit.ToString();
        }
    }
}