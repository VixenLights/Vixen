using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Instrumentation;

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
