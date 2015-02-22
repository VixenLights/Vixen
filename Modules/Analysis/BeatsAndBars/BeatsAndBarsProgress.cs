using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VixenModules.Analysis.BeatsAndBars
{
	public partial class BeatsAndBarsProgress : Form
	{
		public BeatsAndBarsProgress()
		{
			InitializeComponent();
			progressBar1.Value = 0;
		}


		public void UpdateProgress(int value)
		{
			progressBar1.Value = value;
			percentLabel.Text = value.ToString() + "%";
			progressBar1.Refresh();
			percentLabel.Refresh();
			generateLabel.Refresh();
		}

	}
}
