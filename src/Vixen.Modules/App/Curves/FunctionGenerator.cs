using System.Diagnostics;
using Common.Controls.Theme;
using Common.Resources.Properties;

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

		private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var psi = new ProcessStartInfo()
			{
				FileName = "http://www.vixenlights.com/vixen-3-documentation/basic-concepts-of-vixen-3/curve-editor/",
				UseShellExecute = true
			};
			Process.Start(psi);
		}
	}
}
