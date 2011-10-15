using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Media;

namespace VixenTestbed {
	partial class Form1 {
		private IMediaModuleInstance _SelectedMediaModule {
			get { return moduleListMedia.GetSelectedModule<IMediaModuleInstance>(); }
		}

		private void buttonSetupMedia_Click(object sender, EventArgs e) {
			_SelectedMediaModule.Setup();
		}

		private void buttonLoadMediaFile_Click(object sender, EventArgs e) {
			_SelectedMediaModule.LoadMedia(TimeSpan.Zero);
		}

		private void buttonPlayMedia_Click(object sender, EventArgs e) {
			_SelectedMediaModule.Start();
		}

		private void buttonPauseMedia_Click(object sender, EventArgs e) {
			_SelectedMediaModule.Pause();
		}

		private void buttonResumeMedia_Click(object sender, EventArgs e) {
			_SelectedMediaModule.Resume();
		}

		private void buttonStopMedia_Click(object sender, EventArgs e) {
			_SelectedMediaModule.Stop();
		}

		private void moduleListMedia_SelectedModuleChanged(object sender, EventArgs e) {
			buttonLoadMediaFile.Enabled =
			buttonSetupMedia.Enabled =
			groupBoxMediaExecution.Enabled =
				_SelectedMediaModule != null;
		}
	}
}
