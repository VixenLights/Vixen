using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.App.VirtualEffect
{
	public partial class VirtualEffectNameDialog : Form
	{
		public VirtualEffectNameDialog()
		{
			InitializeComponent();
		}

		private void textBoxName_TextChanged(object sender, EventArgs e)
		{
			if (textBoxName.Text == "") {
				buttonSave.Enabled = false;
			}
			else {
				buttonSave.Enabled = true;
			}
		}

		public String effectName
		{
			get { return textBoxName.Text; }
		}
	}
}