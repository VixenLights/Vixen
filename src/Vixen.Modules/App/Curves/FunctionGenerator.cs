﻿using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.App.Curves
{
	public partial class FunctionGenerator : Form
	{
		public FunctionGenerator(string function)
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			ThemeUpdateControls.UpdateControls(this);
			txtFunction.Text = function;
		}

		public FunctionGenerator():this(String.Empty)
		{
			
		}

		public string Function { get; private set; }

		private void btnGenerate_Click(object sender, EventArgs e)
		{
			Function = txtFunction.Text;
			DialogResult = DialogResult.OK;
		}

		private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.vixenlights.com/vixen-3-documentation/basic-concepts-of-vixen-3/curve-editor/");
		}
	}
}
