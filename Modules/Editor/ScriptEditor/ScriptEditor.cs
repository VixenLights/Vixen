using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Common.ScriptSequence.Script;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Editor;
using VixenModules.SequenceType.Script;

namespace VixenModules.Editor.ScriptEditor {
	public partial class ScriptEditor : Form, IEditorUserInterface {
		private ScriptSequenceType _sequence;
		private ISequenceContext _context;
		private List<SourceFileTabPage> _sources = new List<SourceFileTabPage>();

		public ScriptEditor() {
			InitializeComponent();
		}

		public ISequence Sequence { get; set; }

		public ISelection Selection { get; private set; }

		public IModuleDescriptor Descriptor { get; set; }

		public void Save(string filePath = null) {
			_CommitChanges();

			if(string.IsNullOrWhiteSpace(filePath)) {
				_sequence.Save();
			} else {
				_sequence.Save(filePath);
			}
		}

		public bool IsModified {
			get { return _sources.Any(x => x.IsModified); }
		}

		public Guid InstanceId { get; set; }

		public IModuleDataModel ModuleData { get; set; }

		public IEditorModuleInstance OwnerModule { get; set; }

		public void Start() {
			if(_LoadFromSequence(Sequence)) {
				Show();
			} else {
				Close();
			}
		}

		private bool _LoadFromSequence(ISequence sequence) {
			if((_sequence = sequence as ScriptSequenceType) != null) {
				if(_sequence.SourceFiles.Any()) {
					// Display any source files.
					foreach(SourceFile sourceFile in _sequence.SourceFiles) {
						_AddFileTab(sourceFile);
					}
				} else {
					if(!_AddInitialFile()) return false;
				}
			}

			return true;
		}

		private bool _AddInitialFile() {
			if(_sequence == null) return false;

			using(ProjectConfigForm projectConfigForm = new ProjectConfigForm()) {
				if(projectConfigForm.ShowDialog() == DialogResult.OK) {
					_sequence.FilePath = projectConfigForm.SelectedProjectName + _sequence.FileExtension;
					_sequence.Language = projectConfigForm.SelectedLanguage;
					// Add the initial file.
					SourceFile file = _sequence.CreateNewFile(projectConfigForm.SelectedFileName);
					_AddFileTab(file);
					return true;
				}
				return false;
			}
		}

		private void _AddFileTab(SourceFile sourceFile) {
			if(sourceFile == null) return;

			TabPage tabPage = new TabPage(sourceFile.Name);
			SourceFileTabPage sourceControl = new SourceFileTabPage(sourceFile);
			sourceControl.SelectionChanged += SourceSelectionChanged;
			_sources.Add(sourceControl);
			tabPage.Controls.Add(sourceControl);
			sourceControl.Dock = DockStyle.Fill;
			tabControl.TabPages.Add(tabPage);
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

				_context = VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.NoCaching), Sequence);
				if(_context == null) {
					MessageBox.Show(this, "Unable to play this sequence.  See error log for details.", "Script Editor", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					return;
				}
				_context.SequenceEnded += _context_SequenceEnded;
				_context.Message += _context_Message;
				_context.Error += _context_Error;
				_context.Play(startTime, endTime);
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			} finally {
				Cursor = Cursors.Default;
			}
		}

		private void _Stop() {
			if(_context != null) {
				_context.Stop();
			}
		}

		private void _context_Error(object sender, ExecutorMessageEventArgs e) {
			if(InvokeRequired) {
				BeginInvoke((MethodInvoker)(() => _context_Error(sender, e)));
			} else {
				listBoxRuntimeErrors.Items.Add(e.Message);
				tabControlErrors.SelectedTab = tabPageRuntimeErrors;
				splitContainer.Panel2Collapsed = false;
			}
		}

		private void _context_Message(object sender, ExecutorMessageEventArgs e) {
			if(InvokeRequired) {
				BeginInvoke((MethodInvoker)(() => _context_Message(sender, e)));
			} else {
				if(string.IsNullOrEmpty(e.Message)) {
					listBoxCompileErrors.DataSource = new[] { DateTime.Now.ToShortTimeString() + "  No errors." };
				} else {
					listBoxCompileErrors.DataSource = e.Message.Split(new [] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				}
			}
		}

		private void _context_SequenceEnded(object sender, SequenceEventArgs e) {
			_context.Error -= _context_Error;
			_context.Message -= _context_Message;
			_context.SequenceEnded -= _context_SequenceEnded;
			VixenSystem.Contexts.ReleaseContext(_context);
			_context = null;
		}

		private void buttonStop_Click(object sender, EventArgs e) {
			_Stop();
		}

		private void SourceSelectionChanged(object sender, EventArgs e) {
			SourceFileTabPage sourceControl = (SourceFileTabPage)sender;
			labelCaretLocation.Text = string.Format("{0}, {1}", sourceControl.CaretLocation.X + 1, sourceControl.CaretLocation.Y + 1);
		}

		private void buttonRun_Click(object sender, EventArgs e) {
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

		private void buttonSave_Click(object sender, EventArgs e) {
			try {
				Save();
				MessageBox.Show("Project saved.");
			} catch(Exception ex) {
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
