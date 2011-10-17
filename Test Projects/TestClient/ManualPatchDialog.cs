using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;

namespace TestClient {
    public partial class ManualPatchDialog : Form {
        public ManualPatchDialog(OutputController[] controllers) {
            InitializeComponent();

            comboBoxController.DisplayMember = "Name";
            comboBoxController.ValueMember = "Id";
            comboBoxController.DataSource = controllers;

			comboBoxChannel.DisplayMember = "Name";
			comboBoxChannel.ValueMember = "Id";
			comboBoxChannel.DataSource = VixenSystem.Channels.ToArray();
        }

        private void comboBoxController_SelectedIndexChanged(object sender, EventArgs e) {
            OutputController controller = comboBoxController.SelectedItem as OutputController;
            comboBoxOutput.DataSource = Enumerable.Range(0, controller.OutputCount).ToArray();
        }

        private void comboBoxFixture_SelectedIndexChanged(object sender, EventArgs e) {
			//comboBoxChannel.DisplayMember = "Name";
			//comboBoxChannel.ValueMember = "Id";
			//comboBoxChannel.DataSource = (comboBoxFixture.SelectedItem as Fixture).Channels.ToArray();
        }

        private void buttonPatch_Click(object sender, EventArgs e) {
            if(comboBoxController.SelectedItem != null &&
                comboBoxOutput.SelectedIndex != -1 &&
                comboBoxChannel.SelectedItem != null) {
                OutputController controller = comboBoxController.SelectedItem as OutputController;
				Channel channel = comboBoxChannel.SelectedItem as Channel;
                channel.Patch.Add(controller.Id, comboBoxOutput.SelectedIndex);
            } else {
                MessageBox.Show("Fix selections");
            }
        }
    }
}
