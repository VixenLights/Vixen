using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestOutput
{
	public partial class RenardSetup : Form
	{
		public RenardSetup()
		{
			InitializeComponent();
		}

		RenardRenderStyle _style;
		public RenardRenderStyle RenderStyle
		{
			get { return _style; }
			set
			{
				_style = value;
				if (_style == RenardRenderStyle.Monochrome)
					radioButtonMonochrome.Checked = true;
				else if (_style == RenardRenderStyle.RGBMultiChannel)
					radioButtonMultiRGB.Checked = true;
				else if (_style == RenardRenderStyle.RGBSingleChannel)
					radioButtonSingleRGB.Checked = true;
			}	
		}

		private void radioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonMonochrome.Checked)
				RenderStyle = RenardRenderStyle.Monochrome;
			if (radioButtonMultiRGB.Checked)
				RenderStyle = RenardRenderStyle.RGBMultiChannel;
			if (radioButtonSingleRGB.Checked)
				RenderStyle = RenardRenderStyle.RGBSingleChannel;
		}
	}
}
