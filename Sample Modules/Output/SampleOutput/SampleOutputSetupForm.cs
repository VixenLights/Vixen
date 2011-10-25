using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SampleOutput {
	public partial class SampleOutputSetupForm : Form {
		private SampleOutputData _data;

		public SampleOutputSetupForm(SampleOutputData data) {
			InitializeComponent();
			_data = data;
		}

		private void buttonClearRunCount_Click(object sender, EventArgs e) {
			_data.RunCount = 0;
			_UpdateLabels();
		}

		private void SampleOutputSetupForm_Load(object sender, EventArgs e) {
			_UpdateLabels();
		}

		private void _UpdateLabels() {
			labelRunCount.Text = _data.RunCount.ToString() + " times";
			if(_data.LastStartDate != DateTime.MinValue) {
				labelLastStarted.Text = _data.LastStartDate.ToString();
			}
		}
	}
}
