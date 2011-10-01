using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Script;
using Vixen.Module;
using Vixen.Module.Editor;
using Vixen.Execution;

namespace TestEditor {
	public partial class CSharp_ScriptEditor : Form, IEditorUserInterface {
		private ScriptSequence _sequence;
		private ProgramContext _context = null;
		private List<SourceFileTabPage> _sources = new List<SourceFileTabPage>();

		public CSharp_ScriptEditor() {
			InitializeComponent();
			EditorValues = new EditorValues();
		}

		public ISequence Sequence {
			get { return _sequence as ISequence; }
			set {
				if((_sequence = value as ScriptSequence) != null) {
					// Display any source files.
					_sequence.SourceFiles.ForEach(_AddFile);
				}
			}
		}

		public ISelection Selection { get; private set; }

		public IModuleDescriptor Descriptor { get; set; }

		public void NewSequence() {
			this.Sequence = ApplicationServices.CreateSequence(".csp");
			using(CommonElements.TextDialog textDialog = new CommonElements.TextDialog("Name for the new script sequence:")) {
				if(textDialog.ShowDialog() == DialogResult.OK) {
					if(!string.IsNullOrWhiteSpace(textDialog.Response)) {
						_sequence.FilePath = textDialog.Response;
						// Add the initial file.
						SourceFile file = _sequence.CreateNewFile("NewFile");
						_AddFile(file);
					} else {
						// They provided a bad name.
						MessageBox.Show("Name is required.");
					}
				} else {
					// They canceled.
					this.Sequence = null;
				}
			}
		}

		private void _AddFile(SourceFile sourceFile) {
			TabPage tabPage = new TabPage(sourceFile.Name);
			SourceFileTabPage sourceControl = new SourceFileTabPage(sourceFile);
			_sources.Add(sourceControl);
			tabPage.Controls.Add(sourceControl);
			sourceControl.Dock = DockStyle.Fill;
			tabControl.TabPages.Add(tabPage);
		}

		public void Save(string filePath = null) {
			_CommitChanges();
			if(string.IsNullOrWhiteSpace(filePath)) {
				_sequence.Save();
			} else {
				_sequence.Save(filePath);
			}
		}

		public EditorValues EditorValues { get; private set; }

		public bool IsModified {
			get { return _sources.Any(x => x.IsModified); }
		}

		public Guid InstanceId { get; set; }

		public IModuleDataModel ModuleData { get; set; }

		private void button1_Click(object sender, EventArgs e) {
			_Stop();
			_Execute(TimeSpan.Zero, Sequence.Length);
		}

		private void buttonCompile_Click(object sender, EventArgs e) {
			// Specify an end time < start time to compile only.
			// This is the only way we have to signal a "compile only" condition
			// to the script executor.
			// If more special cases pop up, something can be added to the execution
			// context and the base executor for communicating things beyond execution.
			_Execute(TimeSpan.FromMilliseconds(1), TimeSpan.Zero);
		}

		private void _CommitChanges() {
			// Get the source back into the source objects.
			foreach(SourceFileTabPage source in _sources) {
				source.Commit();
			}
		}

		private void _Execute(TimeSpan startTime, TimeSpan endTime) {
			_CommitChanges();

			Cursor = Cursors.WaitCursor;
			try {
				listBoxRuntimeErrors.Items.Clear();

				_context = Execution.CreateContext(this.Sequence);
				_context.ProgramEnded += new EventHandler(_context_ProgramEnded);
				_context.Message += new EventHandler<ExecutorMessageEventArgs>(_context_Message);
				_context.Error += new EventHandler<ExecutorMessageEventArgs>(_context_Error);
				_context.Play(startTime, endTime);
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			} finally {
				Cursor = Cursors.Default;
			}
		}

		void _context_Error(object sender, ExecutorMessageEventArgs e) {
			if(InvokeRequired) {
				BeginInvoke((MethodInvoker)(() => _context_Error(sender, e)));
			} else {
				listBoxRuntimeErrors.Items.Add(e.Message);
				tabControlErrors.SelectedTab = tabPageRuntimeErrors;
				splitContainer.Panel2Collapsed = false;
			}
		}

		void _context_Message(object sender, ExecutorMessageEventArgs e) {
			if(InvokeRequired) {
				BeginInvoke((MethodInvoker)(() => _context_Message(sender, e)));
			} else {
				if(string.IsNullOrEmpty(e.Message)) {
					splitContainer.Panel2Collapsed = true;
				} else {
					listBoxCompileErrors.DataSource = e.Message.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
					splitContainer.Panel2Collapsed = false;
				}
			}
		}

		void _context_ProgramEnded(object sender, EventArgs e) {
			_context.Error -= _context_Error;
			_context.Message -= _context_Message;
			_context.ProgramEnded -= _context_ProgramEnded;
			Execution.ReleaseContext(_context);
			_context = null;
		}

		private void buttonStop_Click(object sender, EventArgs e) {
			_Stop();
		}

		private void _Stop() {
			if(_context != null) {
				_context.Stop();
			}
		}



		public IEditorModuleInstance OwnerModule { get; set; }

		public void Start() {
			Show();
		}
	}
}
