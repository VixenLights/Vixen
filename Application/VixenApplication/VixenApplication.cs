using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Vixen.Module.Editor;
using Vixen.Module.SequenceType;
using Vixen.Services;
using Vixen.Sys;

namespace VixenApplication
{
	public partial class VixenApplication : Form, IApplication
	{
		private Guid _guid = new Guid("7b903272-73d0-416c-94b1-6932758b1963");
		private bool stopping;
		private bool _openExecution = true;
		private bool _disableControllers = false;
		private CpuUsage _cpuUsage;

		private VixenApplicationData _applicationData;

		public VixenApplication()
		{
			string[] args = Environment.GetCommandLineArgs();
			foreach(string arg in args) {
				_ProcessArg(arg);
			}

			stopping = false;
			InitializeComponent();
			labelVersion.Text = "[" + _GetVersionString(VixenSystem.AssemblyFileName) + "]";
			AppCommands = new AppCommand(this);
			Execution.ExecutionStateChanged += executionStateChangedHandler;
			VixenSystem.Start(this, _openExecution, _disableControllers);

			InitStats();

			_applicationData = new VixenApplicationData();
		}


		private void VixenApp_FormClosing(object sender, FormClosingEventArgs e)
		{
			stopping = true;
			VixenSystem.Stop();

			_applicationData.SaveData();
		}

		private void VixenApplication_Load(object sender, EventArgs e)
		{
			initializeEditorTypes();
			openFileDialog.InitialDirectory = SequenceService.SequenceDirectory;

			// Add menu items for the logs.
			foreach(string logName in VixenSystem.Logs.LogNames) {
				logsToolStripMenuItem.DropDownItems.Add(logName, null, (menuSender, menuArgs) => _ViewLog(((ToolStripMenuItem)menuSender).Text));
			}

			PopulateRecentSequencesList();
		}

		private string _GetVersionString(string assemblyFileName) {
			System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(assemblyFileName);
			Version version = assembly.GetName().Version;
			return version.Major + "." + version.Minor + "." + version.Build;
		}

		private void _ProcessArg(string arg) {
			switch(arg) {
				case "no_controllers":
					_disableControllers = true;
					break;
				case "no_execution":
					_openExecution = false;
					break;
			}
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
			foreach(KeyValuePair<Guid, string> typeId_FileTypeName in ApplicationServices.GetAvailableModules<ISequenceTypeModuleInstance>()) {
				item = new ToolStripMenuItem(typeId_FileTypeName.Value);
				ISequenceTypeModuleDescriptor descriptor = ApplicationServices.GetModuleDescriptor(typeId_FileTypeName.Key) as ISequenceTypeModuleDescriptor;

				if (descriptor.CanCreateNew) {
					item.Tag = descriptor.FileExtension;
					item.Click += (sender, e) => {
						ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
						string fileType = (string)menuItem.Tag;
						IEditorUserInterface editor = EditorService.Instance.CreateEditor(fileType);
						if (editor != null) {
							_OpenEditor(editor);
						}
					};
					contextMenuStripNewSequence.Items.Add(item);
				}
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

			AddSequenceToRecentList(editor.Sequence.FilePath);

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
			IEnumerable<ISequenceTypeModuleDescriptor> sequenceDescriptors = ApplicationServices.GetModuleDescriptors<ISequenceTypeModuleInstance>().Cast<ISequenceTypeModuleDescriptor>();
			foreach (ISequenceTypeModuleDescriptor descriptor in sequenceDescriptors) {
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
					OpenSequenceFromFile(file);
				}
				Cursor = Cursors.Default;
			}
		}

		private void OpenSequenceFromFile(string filename)
		{
			try {
				IEditorUserInterface editor = EditorService.Instance.CreateEditor(filename);

				if (editor == null) {
					VixenSystem.Logging.Error("Can't find an appropriate editor to open file " + filename);
				}

				_OpenEditor(editor);
			} catch (Exception ex) {
				VixenSystem.Logging.Error("Error trying to open file '" + filename + "': ", ex);
			}
		}

		private void buttonSetupChannels_Click(object sender, EventArgs e)
		{
			ConfigChannels form = new ConfigChannels();
			DialogResult result = form.ShowDialog();
			if (result == DialogResult.OK) {
				VixenSystem.SaveSystemConfig();
			} else {
				VixenSystem.ReloadSystemConfig();
			}
		}

		private void buttonSetupOutputControllers_Click(object sender, EventArgs e)
		{
			ConfigControllers form = new ConfigControllers();
			DialogResult result = form.ShowDialog();
			if (result == DialogResult.OK) {
				VixenSystem.SaveSystemConfig();
			} else {
				VixenSystem.ReloadSystemConfig();
			}
		}

		private void buttonSetupFiltersAndPatching_Click(object sender, EventArgs e)
		{
			ConfigFiltersAndPatching form = new ConfigFiltersAndPatching(_applicationData);
			DialogResult result = form.ShowDialog();
			if (result == DialogResult.OK) {
				VixenSystem.SaveSystemConfig();
			} else {
				VixenSystem.ReloadSystemConfig();
			}
		}

		private void buttonSetupOutputPreviews_Click(object sender, EventArgs e)
		{
			ConfigPreviews form = new ConfigPreviews();
			DialogResult result = form.ShowDialog();
			if(result == DialogResult.OK) {
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

		private void executionStateChangedHandler(object sender, EventArgs e)
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
			toolStripStatusLabelExecutionState.Text = "Execution: " + Vixen.Sys.Execution.State;

			if(Execution.IsOpen) {
				toolStripStatusLabelExecutionLight.BackColor = Color.ForestGreen;
			} else if(Execution.IsClosed) {
				toolStripStatusLabelExecutionLight.BackColor = Color.Firebrick;
			} else if(Execution.IsInTest) {
				toolStripStatusLabelExecutionLight.BackColor = Color.DodgerBlue;
			} else {
				toolStripStatusLabelExecutionLight.BackColor = Color.Gold;
			}

			startToolStripMenuItem.Enabled = !Execution.IsOpen;
			stopToolStripMenuItem.Enabled = !Execution.IsClosed;
		}

		private void _ViewLog(string logName) {
			string tempFilePath = Path.Combine(Path.GetTempPath(), logName);
			IEnumerable<string> logContents = VixenSystem.Logs.RetrieveLogContents(logName);
			File.WriteAllLines(tempFilePath, logContents);
			using(Process process = new Process()) {
				process.StartInfo = new ProcessStartInfo("notepad.exe", tempFilePath);
				process.Start();
			}
		}

		#region Recent Sequences list

		private const int _maxRecentSequences = 10;

		private void listViewRecentSequences_DoubleClick(object sender, EventArgs e)
		{
			if (listViewRecentSequences.SelectedItems.Count <= 0)
				return;

			string file = listViewRecentSequences.SelectedItems[0].Tag as string;

			if (File.Exists(file)) {
				OpenSequenceFromFile(file);
			} else {
				MessageBox.Show("Can't find selected sequence.");
			}
		}

		private void AddSequenceToRecentList(string filename)
		{
			// remove the item from the list if it exists, then insert it in the front
			foreach (string filepath in _applicationData.RecentSequences.ToArray()) {
				if (filepath == filename) {
					_applicationData.RecentSequences.Remove(filepath);
				}
			}

			_applicationData.RecentSequences.Insert(0, filename);

			if (_applicationData.RecentSequences.Count > _maxRecentSequences)
				_applicationData.RecentSequences.RemoveRange(_maxRecentSequences, _applicationData.RecentSequences.Count - _maxRecentSequences);

			_applicationData.SaveData();
			PopulateRecentSequencesList();
		}

		private void PopulateRecentSequencesList()
		{
			listViewRecentSequences.BeginUpdate();
			listViewRecentSequences.Items.Clear();

			foreach (string filepath in _applicationData.RecentSequences) {
				if (!File.Exists(filepath))
					continue;

				ListViewItem item = new ListViewItem(Path.GetFileName(filepath));
				item.Tag = filepath;
				listViewRecentSequences.Items.Add(item);
			}

			listViewRecentSequences.EndUpdate();
		}

		#endregion

		private void viewInstalledModulesToolStripMenuItem_Click(object sender, EventArgs e) {
			using(InstalledModules installedModules = new InstalledModules()) {
				installedModules.ShowDialog();
			}
		}

		private void channelGroupTestToolStripMenuItem_Click(object sender, EventArgs e) {
			//using(TestForm testForm = new TestForm()) {
			//    testForm.ShowDialog();
			//}
		}

		#region Stats
		private const int StatsUpdateInterval = 1000;	// ms
		private Timer _statsTimer;
		private Process _thisProc;

		private void InitStats()
		{
			_thisProc = Process.GetCurrentProcess();
			_cpuUsage = new CpuUsage();

			_statsTimer = new Timer();
			_statsTimer.Interval = StatsUpdateInterval;
			_statsTimer.Tick += statsTimer_Tick;
			statsTimer_Tick(null, EventArgs.Empty);		// Fake the first update.
			_statsTimer.Start();
		}

		private void statsTimer_Tick(object sender, EventArgs e)
		{
			long memUsage = _thisProc.PrivateMemorySize64 / 1024 / 1024;

			toolStripStatusLabel_memory.Text = String.Format("Mem: {0} MB   CPU: {1}%",
				memUsage, _cpuUsage.GetUsage());
		}

		#endregion

	}
}
