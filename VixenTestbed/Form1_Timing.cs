using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Module.Timing;

namespace VixenTestbed {
	partial class Form1 {
		private ITimingModuleInstance _timingModule;

		private ITimingModuleInstance _SelectedTimingModule {
			get { return moduleListTiming.GetSelectedModule<ITimingModuleInstance>(); }
		}

		private void moduleListTiming_SelectedModuleChanged(object sender, EventArgs e) {
			groupBoxTimingExecution.Enabled = _SelectedTimingModule != null;
		}

		private void buttonPlayTiming_Click(object sender, EventArgs e) {
			try {
				if(_timingModule == null) {
					_timingModule = _SelectedTimingModule;
					_timingModule.Start();
					timingTimer.Enabled = true;
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonPauseTiming_Click(object sender, EventArgs e) {
			try {
				if(_timingModule != null) {
					_timingModule.Pause();
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonResumeTiming_Click(object sender, EventArgs e) {
			try {
				if(_timingModule != null) {
					_timingModule.Resume();
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonStopTiming_Click(object sender, EventArgs e) {
			try {
				if(_timingModule != null) {
					timingTimer.Enabled = false;
					_timingModule.Stop();
					_timingModule = null;
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void timingTimer_Tick(object sender, EventArgs e) {
			if(_timingModule != null) {
				labelTimingCurrentPosition.Text = _timingModule.Position.ToString();
			}
		}
	}
}
