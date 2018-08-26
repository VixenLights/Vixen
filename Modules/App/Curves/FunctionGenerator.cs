using System;
using System.Windows.Forms;
using Common.Controls.Theme;

namespace VixenModules.App.Curves
{
	public partial class FunctionGenerator : Form
	{
		public FunctionGenerator(string function)
		{
			InitializeComponent();
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
	}
}
