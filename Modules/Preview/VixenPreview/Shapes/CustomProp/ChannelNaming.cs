using Common.Controls.Theme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	public partial class ChannelNaming : Form
	{
		public ChannelNaming()
		{
			InitializeComponent();
		//	ThemeUpdateControls.UpdateControls(this);
		}

		public string Value { get; set; }


		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			this.Value = textBox1.Text;
		}

		private void ChannelNaming_Load(object sender, EventArgs e)
		{
			this.textBox1.Text = Value;
		}
	}
}
