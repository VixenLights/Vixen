using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenTestbed {
	partial class Form1 {
		private void buttonControllers_Click(object sender, EventArgs e) {
			using(ControllersForm controllersForm = new ControllersForm()) {
				controllersForm.ShowDialog();
			}
		}

		private void buttonChannels_Click(object sender, EventArgs e) {
			using(ChannelsForm channelsForm = new ChannelsForm()) {
				channelsForm.ShowDialog();
			}
		}

		private void buttonPatching_Click(object sender, EventArgs e) {
			using(PatchingForm patchingForm = new PatchingForm()) {
				patchingForm.ShowDialog();
			}
		}
	}
}
