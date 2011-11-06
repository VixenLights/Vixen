using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Module.Editor;
using Vixen.Execution;
using Vixen.Module.Timing;

namespace SampleEditor {
	public partial class TheEditor : Form, IEditorUserInterface {
		private ISequence _sequence;
		private ProgramContext _context;
		private ITiming _timingSource;

		public TheEditor() {
			InitializeComponent();
		}

		public IEditorModuleInstance OwnerModule { get; set; }

		public void Start() {
			Show();
		}

		public ISequence Sequence {
			get { return _sequence; }
			set { 
				_sequence = value;
				if(_sequence != null) {
					Text = _sequence.Name;
				}
				buttonPlay.Enabled = _sequence != null;
				buttonPause.Enabled = _sequence != null;
				buttonResume.Enabled = _sequence != null;
				buttonStop.Enabled = _sequence != null;
			}
		}

		public ISelection Selection {
			get { return null; }
		}

		public void Save(string filePath = null) {
			if(string.IsNullOrEmpty(filePath)) {
				_sequence.Save();
			} else {
				_sequence.Save(filePath);
			}
			MessageBox.Show("Saved " + _sequence.Name);
		}

		public bool IsModified {
			// We're always dirty, just because.
			get { return true; }
		}

		private void TheEditor_FormClosing(object sender, FormClosingEventArgs e) {
			if(_context != null) {
				_context.Stop();
			}
		}

		void _context_SequenceStarted(object sender, SequenceStartedEventArgs e) {
			_timingSource = e.TimingSource;
		}

		void _context_ProgramEnded(object sender, ProgramEventArgs e) {
			timer.Stop();
			_context.SequenceStarted -= _context_SequenceStarted;
			_context.ProgramEnded -= _context_ProgramEnded;
			Execution.ReleaseContext(_context);
			_context = null;
		}

		private void timer_Tick(object sender, EventArgs e) {
			if(_timingSource != null) {
				labelExecution.Text = _timingSource.Position.ToString(@"mm\:ss\.fff");
			}
		}

		private void buttonPlay_Click(object sender, EventArgs e) {
			_context = Execution.CreateContext(this.Sequence);

			// We're going to listen for SequenceStarted so that we can grab the
			// timing source for each sequence as it runs (there may be multiple sequences).
			_context.SequenceStarted += _context_SequenceStarted;
	
			// We're going to listen for ProgramEnded because that is raised whether
			// the user stops the sequence or it ends naturally on its own.
			// When it's done executing, we will stop our UI update timer and release the context.
			_context.ProgramEnded += _context_ProgramEnded;
			
			// Run a timer so that we can periodically get the execution position
			// and update a label.
			timer.Enabled = _context.Play();
		}

		private void buttonPause_Click(object sender, EventArgs e) {
			if(_context != null) {
				_context.Pause();
			}
		}

		private void buttonResume_Click(object sender, EventArgs e) {
			if(_context != null) {
				_context.Play();
			}
		}

		private void buttonStop_Click(object sender, EventArgs e) {
			if(_context != null) {
				_context.Stop();
			}
		}
	}
}
