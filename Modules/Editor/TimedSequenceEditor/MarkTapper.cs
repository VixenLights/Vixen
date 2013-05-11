using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Execution;
using Vixen.Module.Timing;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class MarkTapper : Form
	{
		private IExecutionControl _executionControl;
		private ITiming _timingSource;
		private bool _playing;

		public MarkTapper(IExecutionControl executionControl, ITiming timingSource)
		{
			InitializeComponent();
			_executionControl = executionControl;
			_timingSource = timingSource;
			Results = new List<TimeSpan>();

			if (_executionControl == null || _timingSource == null) {
				groupBoxControls.Enabled = false;
			}

			_playing = false;
		}

		public List<TimeSpan> Results { get; set; }

		private void buttonPlay_Click(object sender, EventArgs e)
		{
			_executionControl.Start();
			_playing = true;
		}

		private void buttonPause_Click(object sender, EventArgs e)
		{
			_executionControl.Pause();
			_playing = false;
		}

		private void buttonResume_Click(object sender, EventArgs e)
		{
			_executionControl.Resume();
			_playing = true;
		}

		private void buttonStop_Click(object sender, EventArgs e)
		{
			_executionControl.Stop();
			_playing = false;
		}

		private void MarkTapper_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space) {
				_triggerResult();
				e.Handled = true;
			}
		}

		private void _triggerResult()
		{
			// round the tapped time to the nearest millisecond
			Results.Add(TimeSpan.FromMilliseconds(Math.Round(_timingSource.Position.TotalMilliseconds)));
			panelTap.BackColor = Color.SkyBlue;
			timerTap.Enabled = true;
		}

		private void timerTap_Tick(object sender, EventArgs e)
		{
			panelTap.BackColor = SystemColors.Control;
			timerTap.Enabled = false;
		}

		private void MarkTapper_FormClosing(object sender, FormClosingEventArgs e)
		{
			_executionControl.Stop();
		}

        private void panelTap_MouseDown(object sender, MouseEventArgs e)
        {
            if (_playing)
                _triggerResult();
        }
	}
}
