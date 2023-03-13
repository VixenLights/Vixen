using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;


namespace VixenModules.SequenceType.Vixen2x
{
	public partial class CoversionProgressForm : BaseForm
	{
		public string StatusLineLabel
		{
			set { lblStatusLine.Text = value; }
		}


		public CoversionProgressForm()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
		}

		public void UpdateProgressBar(int value)
		{
			pbImport.Value = value;
		}

		public void SetupProgressBar(int min, int max)
		{
			pbImport.Minimum = min;
			pbImport.Maximum = max;
			pbImport.Value = 0;
		}
	}
}