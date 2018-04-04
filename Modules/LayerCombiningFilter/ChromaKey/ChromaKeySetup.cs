using System;
using System.Windows.Forms;
using System.Drawing;
using Common.Controls;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.LayerMixingFilter.ChromaKey
{
	public partial class ChromaKeySetup : BaseForm
	{
		private int _lowerLimit { get; set; }
		private int _upperLimit { get; set; }

		public ChromaKeySetup(ChromaKeyData data)
	    {
	        InitializeComponent();
	        ForeColor = ThemeColorTable.ForeColor;
	        BackColor = ThemeColorTable.BackgroundColor;
	        ThemeUpdateControls.UpdateControls(this);
		    _lowerLimit = (int)(data.LowerLimit * 100);
		    _upperLimit = (int)(data.UpperLimit * 100);
			UpdateLimitControls();
	        colorPanel1.Color = data.KeyColor;
	        HueTolerance = data.HueTolerance;
	        trkHueTolerance.Value = Convert.ToInt32(data.HueTolerance);
	        SaturationTolerance = data.SaturationTolerance;
	        trkSaturationTolerance.Value = Convert.ToInt32(data.SaturationTolerance*100);
	    }

        public double LowerLimit { get { return _lowerLimit / 100d; } }
	    public double UpperLimit { get { return _upperLimit / 100d; } }
        public Color KeyColor { get; private set; }
        public double KeySaturation {get { return Math.Round(HSV.FromRGB(KeyColor).S, 2); } }
		public float KeyHue { get { return KeyColor.GetHue(); } }
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
		    ValidateLower(trkLowerLimit.Value);
        }

        private void trkUpperLimit_Scroll(object sender, EventArgs e)
        {
	        ValidateUpper(trkUpperLimit.Value);       
        }

		private void numLowerLimit_LostFocus(object sender, EventArgs e)
		{
			ValidateLower(numLowerLimit.IntValue);
		}

		private void numUpperLimit_LostFocus(object sender, EventArgs e)
		{
			ValidateUpper(numUpperLimit.IntValue);
		}

		private void numUpperLimit_TextChanged(object sender, EventArgs e)
		{
			trkUpperLimit.Value = numUpperLimit.IntValue;
		}

		private void numLowerLimit_TextChanged(object sender, EventArgs e)
		{
			trkLowerLimit.Value = numLowerLimit.IntValue;
		}

		private void ValidateLower(int v)
		{
			_lowerLimit = v <= _upperLimit ? v : _upperLimit;
			UpdateLimitControls();
		}

		private void ValidateUpper(int v)
		{
			_upperLimit = v >= _lowerLimit ? v : _lowerLimit;
			UpdateLimitControls();
		}

		private void UpdateLimitControls()
		{
			toolTip.SetToolTip(trkLowerLimit, trkLowerLimit.Value.ToString());
			toolTip.SetToolTip(trkUpperLimit, trkUpperLimit.Value.ToString());
			trkLowerLimit.Value = _lowerLimit;
			trkUpperLimit.Value = _upperLimit;
			numLowerLimit.Text = _lowerLimit.ToString();
			numUpperLimit.Text = _upperLimit.ToString();
		}

		private void colorPanel1_ColorChanged(object sender, EventArgs e)
	    {
	        KeyColor = colorPanel1.Color;
	        toolTip.SetToolTip(colorPanel1, "Click to Select the Key Color");
	    }

	    private void trkHueTolerance_Scroll(object sender, EventArgs e)
	    {
	        HueTolerance = trkHueTolerance.Value; //add in scaling to 180?
	        toolTip.SetToolTip(trkHueTolerance, trkHueTolerance.Value.ToString());
	    }
	    
	    private void trkSaturationTolerance_Scroll(object sender, EventArgs e)
	    {
	        SaturationTolerance = trkHueTolerance.Value / 100;
	        toolTip.SetToolTip(trkSaturationTolerance, trkSaturationTolerance.Value.ToString());
	    }
    }
}