using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Module.Media;

namespace VixenTestbed {
	partial class Form1 {
		private IMediaModuleInstance _SelectedMediaModule {
			get { return moduleListMedia.GetSelectedModule<IMediaModuleInstance>(); }
		}

		private void buttonSetupMedia_Click(object sender, EventArgs e) {
			try {
				_SelectedMediaModule.Setup();
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonLoadMediaFile_Click(object sender, EventArgs e) {
			try {
				IMediaModuleDescriptor descriptor = _SelectedMediaModule.Descriptor as IMediaModuleDescriptor;
				openFileDialog.Filter = descriptor.TypeName + " files |" + string.Join(";", descriptor.FileExtensions.Select(x => "*" + x).ToArray());
				if(openFileDialog.ShowDialog() == DialogResult.OK) {
					_SelectedMediaModule.MediaFilePath = openFileDialog.FileName;
					_SelectedMediaModule.LoadMedia(TimeSpan.Zero);
				}
				string mediaFilePath = _SelectedMediaModule.MediaFilePath;
				labelLoadedMedia.Text = string.IsNullOrWhiteSpace(mediaFilePath) ? "Nothing loaded" : System.IO.Path.GetFileName(mediaFilePath);
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonPlayMedia_Click(object sender, EventArgs e) {
			try {
				_SelectedMediaModule.Start();
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonPauseMedia_Click(object sender, EventArgs e) {
			try {
				_SelectedMediaModule.Pause();
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonResumeMedia_Click(object sender, EventArgs e) {
			try {
				_SelectedMediaModule.Resume();
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonStopMedia_Click(object sender, EventArgs e) {
			try {
				_SelectedMediaModule.Stop();
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void moduleListMedia_SelectedModuleChanged(object sender, EventArgs e) {
			buttonLoadMediaFile.Enabled =
			buttonSetupMedia.Enabled =
			groupBoxMediaExecution.Enabled =
				_SelectedMediaModule != null;
		}
	}
}
