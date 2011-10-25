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
	}
}
