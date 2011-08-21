using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _595 {
	partial class SetupDialog : Form {
		private Data _data;

		public SetupDialog(Data data) {
			InitializeComponent();
			_data = data;
			_PortAddress = _data.Port;
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			if(_PortAddress == 0) {
				MessageBox.Show("The port address is 0.", "595 Setup", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				DialogResult = DialogResult.None;
			} else {
				_data.Port = _PortAddress;
			}
		}

		private ushort _PortAddress {
			get {
				ushort value;
				if(ushort.TryParse(textBoxPortAddress.Text, out value)) {
				}
				return 0;
			}
			set { textBoxPortAddress.Text = value.ToString(); }
		}
	}
}
