using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CommonElements;
using Script;
using ScriptSequence.Script;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Module.Script;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Editor;

namespace ScriptEditor {
	public partial class ScriptEditor : Form, IEditorUserInterface {
		private ScriptSequenceType _sequence;
		private ISequenceContext _context;
		private List<SourceFileTabPage> _sources = new List<SourceFileTabPage>();
		private IScriptModuleInstance _language;

		public ScriptEditor() {
			InitializeComponent();
		}

		private void ScriptEditor_Load(object sender, EventArgs e) {
			IScriptModuleDescriptor[] scriptModuleDescriptors = Vixen.Services.ApplicationServices.GetModuleDescriptors<IScriptModuleInstance>().Cast<IScriptModuleDescriptor>().ToArray();
			comboBoxLanguage.DisplayMember = "LanguageName";
			comboBoxLanguage.ValueMember = null;
			comboBoxLanguage.DataSource = scriptModuleDescriptors;
		}

		public ISequence Sequence {
			get { return _sequence; }
			set {
				if((_sequence = value as ScriptSequenceType) != null) {
					if(_sequence.SourceFiles.Any()) {
						// Display any source files.
						foreach(SourceFile sourceFile in _sequence.SourceFiles) {
							_AddFileTab(sourceFile);
						}
					} else {
						_AddInitialFile();
					}
				}
			}
		}

		private void _AddInitialFile() {
			if(_sequence == null || _Language == null) return;

			using(TextDialog textDialog = new TextDialog("Name for the new file:")) {
				if(textDialog.ShowDialog() == DialogResult.OK) {
					if(!string.IsNullOrWhiteSpace(textDialog.Response)) {
						_sequence.FilePath = textDialog.Response;
						_sequence.Language = _Language;
						// Add the initial file.
						SourceFile file = _sequence.CreateNewFile(textDialog.Response);
						_AddFileTab(file);
					} else {
						//*** change
						// They provided a bad name.
						// Prompt until they provide a good one or cancel, blah blah blah.
						MessageBox.Show("Name is required.");
					}
				}
			}
		}

		public ISelection Selection { get; private set; }

		public IModuleDescriptor Descriptor { get; set; }

		private void _AddFileTab(SourceFile sourceFile) {
			if(sourceFile == null) return;

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
					listBoxCompileErrors.DataSource = e.Message.Split(new [] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
					splitContainer.Panel2Collapsed = false;
				}
			}
		}

		void _context_SequenceEnded(object sender, SequenceEventArgs e) {
			_context.Error -= _context_Error;
			_context.Message -= _context_Message;
			_context.SequenceEnded -= _context_SequenceEnded;
			VixenSystem.Contexts.ReleaseContext(_context);
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

		private IScriptModuleInstance _Language {
			get {
				if(_language == null) {
					if(comboBoxLanguage.SelectedValue != null) {
						Guid scriptModuleTypeId = ((IModuleDescriptor)comboBoxLanguage.SelectedValue).TypeId;
						_language = Vixen.Services.ApplicationServices.Get<IScriptModuleInstance>(scriptModuleTypeId);
					}
				}
				return _language;
			}
		}



		public IEditorModuleInstance OwnerModule { get; set; }

		public void Start() {
			Show();
		}

		private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e) {
			if(_sequence == null || comboBoxLanguage.SelectedValue == null) return;

			//    If there are files, ask them to close all files first or offer to close them, then create initial file
			if(_sequence.SourceFiles.Any()) {
			}
				//temp
			else _AddInitialFile();

			//later
			//_AddInitialFile();
		}
	}
}
