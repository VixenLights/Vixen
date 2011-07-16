using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;

namespace TestProperty {
	public partial class RGBSetup : Form {
		private RGBData _data;

		public RGBSetup(RGBData data, OutputChannel[] channels) {
			InitializeComponent();
			_data = data;
			comboBoxRed.DisplayMember = "Name";
			comboBoxRed.ValueMember = "Id";
			comboBoxRed.DataSource = channels.ToArray();
			if(data.RedChannelId != Guid.Empty) comboBoxRed.SelectedValue = data.RedChannelId;
			comboBoxGreen.DisplayMember = "Name";
			comboBoxGreen.ValueMember = "Id";
			comboBoxGreen.DataSource = channels.ToArray();
			if(data.GreenChannelId != Guid.Empty) comboBoxGreen.SelectedValue = data.GreenChannelId;
			comboBoxBlue.DisplayMember = "Name";
			comboBoxBlue.ValueMember = "Id";
			comboBoxBlue.DataSource = channels.ToArray();
			if(data.BlueChannelId != Guid.Empty) comboBoxBlue.SelectedValue = data.BlueChannelId;
		}

		private bool _Validate() {
			if(comboBoxRed.SelectedItem == null ||
				comboBoxGreen.SelectedItem == null ||
				comboBoxBlue.SelectedItem == null) {
					MessageBox.Show("All channels must be assigned.", "RGB", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					return false;
			}

			return true;
		}

		private void buttonOk_Click(object sender, EventArgs e) {
			if(_Validate()) {
				_data.RedChannelId = (comboBoxRed.SelectedItem as OutputChannel).Id;
				_data.GreenChannelId = (comboBoxGreen.SelectedItem as OutputChannel).Id;
				_data.BlueChannelId = (comboBoxBlue.SelectedItem as OutputChannel).Id;
			}
		}
	}
}
