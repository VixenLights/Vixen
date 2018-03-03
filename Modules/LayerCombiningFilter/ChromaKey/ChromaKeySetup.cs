using System;
using System.Windows.Forms;
using System.Drawing;
using System.Security.AccessControl;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.LayerMixingFilter.ChromaKey
{
	public partial class ChromaKeySetup : BaseForm
	{
	    public ChromaKeySetup(ChromaKeyData data)
	    {
	        InitializeComponent();
	        ForeColor = ThemeColorTable.ForeColor;
	        BackColor = ThemeColorTable.BackgroundColor;
	        ThemeUpdateControls.UpdateControls(this);
	        LowerLimit = data.LowerLimit;
	        UpperLimit = data.UpperLimit;
	        UpdateLimitControls();
	        colorPanel1.Color = data.KeyColor;
	        HueTolerance = data.HueTolerance;
	        trkHueTolerance.Value = Convert.ToInt32(data.HueTolerance);
	        SaturationTolerance = data.SaturationTolerance;
	        trkSaturationTolerance.Value = Convert.ToInt32(data.SaturationTolerance*100);
	    }

        public int LowerLimit { get; private set; }
	    public int UpperLimit { get; private set; }
        public Color KeyColor { get; private set; }
        public float HueTolerance { get; private set; }
        public float SaturationTolerance { get; private set; }

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
	        toolTip.SetToolTip(trkLowerLimit, trkLowerLimit.Value.ToString());
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
            toolTip.SetToolTip(trkUpperLimit, trkUpperLimit.Value.ToString());
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

	    private void colorPanel1_ColorChanged(object sender, EventArgs e)
	    {
	        KeyColor = colorPanel1.Color;
	        toolTip.SetToolTip(colorPanel1, "Click to Select the Key Color");
	    }

	    private void trkHueTolerance_Scroll(object sender, EventArgs e)
	    {
	        HueTolerance = trkHueTolerance.Value; //add in scaling to 180
	        toolTip.SetToolTip(trkHueTolerance, trkHueTolerance.Value.ToString());
	    }
	    
	    private void trkSaturationTolerance_Scroll(object sender, EventArgs e)
	    {
	        SaturationTolerance = trkHueTolerance.Value / 100;
	        toolTip.SetToolTip(trkSaturationTolerance, trkSaturationTolerance.Value.ToString());
	    }
    }
}