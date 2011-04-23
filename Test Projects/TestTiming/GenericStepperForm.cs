using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Common;
using Vixen.Module.Timing;

namespace TestTiming {
	public partial class GenericStepperForm : Form {
		public GenericStepperForm() {
			InitializeComponent();
		}

		public long Position { get; set; }

		private void buttonNext_Click(object sender, EventArgs e) {
			Position += (long)numericUpDownStep.Value;
			labelCurrentStep.Text = Position.ToString();
		}
	}
}
