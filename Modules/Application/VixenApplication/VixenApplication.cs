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
using Vixen.Module.Sequence;

namespace VixenApplication
{
	public partial class VixenApplication : Form, IApplication
	{
		private Guid _guid = new Guid("7b903272-73d0-416c-94b1-6932758b1963");
		private bool stopping;

		public VixenApplication()
		{
			stopping = false;
			InitializeComponent();
			AppCommands = new AppCommand(this);
			Execution.ExecutionStateChanged += executionStateChangedHandler;
			VixenSystem.Start(this, true);
		}

		private void VixenApp_FormClosing(object sender, FormClosingEventArgs e)
		{
			stopping = true;
			VixenSystem.Stop();
		}

		private void VixenApplication_Load(object sender, EventArgs e)
		{
			initializeEditorTypes();
			openFileDialog.InitialDirectory = Vixen.Sys.Sequence.DefaultDirectory;
		}

		#region IApplication implemetation

		public AppCommand AppCommands { get; private set; }

		public Guid ApplicationId
		{
			get { return _guid; }
		}

		private IEditorUserInterface _activeEditor = null;
		public IEditorUserInterface ActiveEditor
		{
			get
			{
				// Don't want to clear our reference on Deactivate because
				// it may be deactivated due to the client getting focus.
				if (_activeEditor.IsDisposed)
				{
					_activeEditor = null;
				}
				return _activeEditor;
			}
		}

		private List<IEditorUserInterface> _openEditors = new List<IEditorUserInterface>();
		public IEditorUserInterface[] AllEditors
		{
			get { return _openEditors.ToArray(); }
		}
		#endregion

		#region Sequence Editor Type population & management
		private void initializeEditorTypes()
		{
			ToolStripMenuItem item;
			foreach(KeyValuePair<Guid, string> typeId_FileTypeName in ApplicationServices.GetAvailableModules<ISequenceModuleInstance>()) {
				item = new ToolStripMenuItem(typeId_FileTypeName.Value);
				ISequenceModuleDescriptor descriptor = ApplicationServices.GetModuleDescriptor(typeId_FileTypeName.Key) as ISequenceModuleDescriptor;
				item.Tag = descriptor.FileExtension;
				item.Click += (sender, e) => {
					ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
					string fileType = (string)menuItem.Tag;
					IEditorUserInterface editor = ApplicationServices.CreateEditor(fileType);
					if(editor != null) {
						_OpenEditor(editor);
					}
				};
				contextMenuStripNewSequence.Items.Add(item);
			}
		}

		private void _OpenEditor(IEditorUserInterface editorUI)
		{
			_openEditors.Add(editorUI);

			editorUI.Closing += (sender, e) => {
				if(!_CloseEditor(sender as IEditorUserInterface)) {
					e.Cancel = true;
				}
			};

			editorUI.Activated += (sender, e) => {
				_activeEditor = sender as IEditorUserInterface;
			};

			editorUI.Start();
		}

		private bool _CloseEditor(IEditorUserInterface editor)
		{
			if (editor.IsModified) {
				DialogResult result = MessageBox.Show("Save changes to the sequence?", "Save Changes?", MessageBoxButtons.YesNoCancel);
				if (result == System.Windows.Forms.DialogResult.Cancel)
					return false;

				if (result == System.Windows.Forms.DialogResult.Yes)
					editor.Save();
			}

			if (_openEditors.Contains(editor))
			{
				_openEditors.Remove(editor);
				Form editorForm = editor as Form;
				editor.Dispose();
			}
			return true;
		}
		#endregion

		private void buttonNewSequence_Click(object sender, EventArgs e)
		{
			contextMenuStripNewSequence.Show(buttonNewSequence, new Point(0,buttonNewSequence.Height));
		}

		private void buttonOpenSequence_Click(object sender, EventArgs e)
		{
			// configure the open file dialog with a filter for currently available sequence types
			string filter = "";
			string allTypes = "";
			IEnumerable<ISequenceModuleDescriptor> sequenceDescriptors = ApplicationServices.GetModuleDescriptors<ISequenceModuleInstance>().Cast<ISequenceModuleDescriptor>();
			foreach (ISequenceModuleDescriptor descriptor in sequenceDescriptors) {
				filter += descriptor.TypeName + " (*" + descriptor.FileExtension + ")|*" + descriptor.FileExtension + "|";
				allTypes += "*" + descriptor.FileExtension + ";";
			}
			filter += "All files (*.*)|*.*";
			filter = "All Sequence Types (" + allTypes + ")|" + allTypes + "|" + filter;

			openFileDialog.Filter = filter;

			// if the user hit 'ok' on the dialog, try opening the selected file(s) in an approriate editor
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				Cursor = Cursors.WaitCursor;
				foreach (string file in openFileDialog.FileNames) {
					try {
						IEditorUserInterface editor = ApplicationServices.CreateEditor(file);

						if (editor == null) {
							VixenSystem.Logging.Error("Can't find an appropriate editor to open file " + file);
							continue;
						}

						_OpenEditor(editor);
					} catch (Exception ex) {
						VixenSystem.Logging.Error("Error trying to open file '" + file + "': ", ex);
					}
				}
				Cursor = Cursors.Default;
			}
		}

		private void buttonSetupChannels_Click(object sender, EventArgs e)
		{
			ConfigChannels form = new ConfigChannels();
			DialogResult result = form.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK) {
				VixenSystem.SaveSystemConfig();
			} else {
				VixenSystem.ReloadSystemConfig();
			}
		}

		private void buttonSetupOutputControllers_Click(object sender, EventArgs e)
		{
			ConfigControllers form = new ConfigControllers();
			DialogResult result = form.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK) {
				VixenSystem.SaveSystemConfig();
			} else {
				VixenSystem.ReloadSystemConfig();
			}
		}

		private void startToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Execution.OpenExecution();
		}

		private void stopToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Execution.CloseExecution();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void executionStateChangedHandler(Execution.ExecutionState state)
		{
			if (stopping)
				return;

			if (InvokeRequired)
				Invoke(new MethodInvoker(updateExecutionState));
			else
				updateExecutionState();
		}

		// we can't get passed in a state to display, since it may be called out-of-order if we're invoking across threads, etc.
		// so instead, just take this as a notification to update with the current state of the execution engine.
		private void updateExecutionState()
		{
			switch (Vixen.Sys.Execution.State) {
				case Execution.ExecutionState.Started:
					toolStripStatusLabelExecutionState.Text = "Execution: Started";
					toolStripStatusLabelExecutionLight.BackColor = Color.ForestGreen;
					break;

				case Execution.ExecutionState.Starting:
					toolStripStatusLabelExecutionState.Text = "Execution: Starting";
					toolStripStatusLabelExecutionLight.BackColor = Color.DodgerBlue;
					break;

				case Execution.ExecutionState.Stopped:
					toolStripStatusLabelExecutionState.Text = "Execution: Stopped";
					toolStripStatusLabelExecutionLight.BackColor = Color.Firebrick;
					break;

				case Execution.ExecutionState.Stopping:
					toolStripStatusLabelExecutionState.Text = "Execution: Stopping";
					toolStripStatusLabelExecutionLight.BackColor = Color.Gold;
					break;
			}
		}
	}
}
