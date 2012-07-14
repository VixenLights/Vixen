using System;
using System.Linq;
using System.Windows.Forms;
using Vixen.Sys;

namespace VixenModules.App.InstrumentationPanel
{
	public partial class InstrumentationForm : Form {

		public InstrumentationForm() {
			InitializeComponent();
		}

		private void InstrumentationForm_Load(object sender, EventArgs e) {
			timer.Start();
		}

		private void timer_Tick(object sender, EventArgs e) {
			string[] lines = VixenSystem.Instrumentation.Values.Select(x => x.Name + ": " + x.FormattedValue).ToArray();
			textBox1.Lines = lines;
		}
	}
}
