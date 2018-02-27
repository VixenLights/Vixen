using System;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.LayerMixingFilter.LumaKey
{
	public partial class LumaKeySetup : BaseForm
	{
		public LumaKeySetup(bool excludeZeroValues) //can i pass the whole data object here instead of the individual members?
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			ExcludeZeroValuesValues = excludeZeroValues;
			chkExcludeZero.Checked = excludeZeroValues;
		    LowerLimit = 0;
		    UpperLimit = 100;
		}

		public bool ExcludeZeroValuesValues { get; private set; }

        public int LowerLimit { get; private set; }
	    public int UpperLimit { get; private set; }

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
            if (trkUpperLimit.Value > LowerLimit)
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

	    private void numLowerLimit_TextChanged(object sender, EventArgs e)
	    {
            //TEST validation 
	        if (Convert.ToInt32(numLowerLimit.Text) < UpperLimit)
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

        private void numUpperLimit_TextChanged(object sender, EventArgs e)
        {
            //TEST validation 
            if(Convert.ToInt32(numUpperLimit.Text) > LowerLimit)
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
    }
}
