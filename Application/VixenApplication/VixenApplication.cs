using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Runtime;
using Vixen.Module.Editor;
using Vixen.Module.SequenceType;
using Vixen.Services;
using Vixen.Sys;
using NLog;
using Common.Resources.Properties;
using Common.Controls;

namespace VixenApplication
{
	public partial class VixenApplication : Form, IApplication
	{
		private static NLog.Logger Logging = LogManager.GetCurrentClassLogger();

		private Guid _guid = new Guid("7b903272-73d0-416c-94b1-6932758b1963");
		private bool stopping;
		private bool _openExecution = true;
		private bool _disableControllers = false;
		private bool _devBuild = false;
		private string _rootDataDirectory;
		private CpuUsage _cpuUsage;

		private VixenApplicationData _applicationData;

		public VixenApplication()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;

			string[] args = Environment.GetCommandLineArgs();
			foreach (string arg in args) {
				_ProcessArg(arg);
			}

			StartJITProfiler();

			if (_rootDataDirectory == null)
				ProcessProfiles();

			_applicationData = new VixenApplicationData(_rootDataDirectory);

			stopping = false;
			PopulateVersionStrings();
			AppCommands = new AppCommand(this);
			Execution.ExecutionStateChanged += executionStateChangedHandler;
			VixenSystem.Start(this, _openExecution, _disableControllers, _applicationData.DataFileDirectory);

			InitStats();

			// other modules look for and create it this way...
			AppCommand toolsMenu = AppCommands.Find("Tools");
			if (toolsMenu == null)
			{
				toolsMenu = new AppCommand("Tools", "Tools");
				AppCommands.Add(toolsMenu);
		}
			var myMenu = new AppCommand("Options", "Options...");
			myMenu.Click += optionsToolStripMenuItem_Click;
			toolsMenu.Add(myMenu);

		}

		private void StartJITProfiler()
		{
			try
			{
				string perfDataPath = 
					System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "Vixen");
				if (!System.IO.Directory.Exists(perfDataPath))
					System.IO.Directory.CreateDirectory(perfDataPath);

				ProfileOptimization.SetProfileRoot(perfDataPath);
				ProfileOptimization.StartProfile("~perfData.tmp");
			}
			catch (Exception e)
			{
				Logging.Warn("JIT Profiling Disabled", e);
			}
		}

		private void VixenApp_FormClosing(object sender, FormClosingEventArgs e)
		{
			// close all open editors
			foreach (IEditorUserInterface editor in _openEditors.ToArray()) {
				editor.CloseEditor();
			}

			stopping = true;
			VixenSystem.Stop();

			_applicationData.SaveData();
			Application.Exit();
		}

		private void VixenApplication_Load(object sender, EventArgs e)
		{
			initializeEditorTypes();
			openFileDialog.InitialDirectory = SequenceService.SequenceDirectory;

			// Add menu items for the logs.
			string logDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Vixen 3", "Logs");

			var di = new System.IO.DirectoryInfo(logDirectory);

			foreach (string logName in di.GetFiles().Select(x => x.Name)) {
				logsToolStripMenuItem.DropDownItems.Add(logName, null,
				                                        (menuSender, menuArgs) => _ViewLog(((ToolStripMenuItem) menuSender).Text));
			}

			PopulateRecentSequencesList();
		}

		private void VixenApplication_Shown(object sender, EventArgs e)
		{
			CheckForTestBuild();
		}

		private void PopulateVersionStrings()
		{
			System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(VixenSystem.AssemblyFileName);
			Version version = assembly.GetName().Version;

			_devBuild = version.Major == 0;

			if (_devBuild) {
				labelVersion.Text = "DevBuild";
			} else {
				labelVersion.Text = string.Format("{0}.{1}", version.Major, version.Minor);
				if (version.Revision > 0) {
					labelVersion.Text += string.Format("u{0}", version.Revision);
				}
			}

			labelDebugVersion.Text = string.Format("Build #{0}", version.Build);
			labelDebugVersion.Visible = true;
		}

		private void CheckForTestBuild()
		{
			if (_devBuild) {
				MessageBox.Show(
					"Please be aware that this is a development version. Some parts of the software may not work, " +
					"and data loss is possible! Please backup your data before using this version of the software.",
					"Development/Test Software", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void _ProcessArg(string arg)
		{
			string[] argParts = arg.Split('=');
			switch (argParts[0]) {
				case "no_controllers":
					_disableControllers = true;
					break;
				case "no_execution":
					_openExecution = false;
					break;
				case "data_dir":
					if (argParts.Length > 1) {
						_rootDataDirectory = argParts[1];
					}
					else {
						_rootDataDirectory = null;
					}
					break;
			}
		}

		private void ProcessProfiles()
		{
			XMLProfileSettings profile = new XMLProfileSettings();
			string loadAction = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "LoadAction", "LoadSelected");
			int profileCount = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "ProfileCount", 0);
			int profileToLoad = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "ProfileToLoad", -1);
			// why ask if there is 0 or 1 profile?
			if (loadAction == "Ask" && profileCount > 1) {
				SelectProfile f = new SelectProfile();
				if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					_rootDataDirectory = f.DataFolder;
				}
			}
			else 
				// So we're to use the "selected" one...
			{
				// If we don't have any profiles, get outta here
				if (profileCount == 0 || profileToLoad < 0) {
					return;
				}
				else {
					if (profileToLoad < profileCount) {
						_rootDataDirectory = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + profileToLoad.ToString() + "/DataFolder", string.Empty);
						if (_rootDataDirectory != string.Empty) {
							if (!System.IO.Directory.Exists(_rootDataDirectory))
								System.IO.Directory.CreateDirectory(_rootDataDirectory);
						}
						else {
							_rootDataDirectory = null;
						}
					}
				}
			}
			SetLogFilePaths();
		}

		/// <summary>
		/// Sets the log file paths to the appropriate profile log directory
		/// </summary>
		private void SetLogFilePaths() {
			//string logDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Vixen 3");
			string logDirectory = _rootDataDirectory;
			if (System.IO.Directory.Exists(logDirectory)) {
				NLog.Config.LoggingConfiguration config = NLog.LogManager.Configuration;
				config.AllTargets.ToList().ForEach(t => {
					var target = t as NLog.Targets.FileTarget;
					if (target != null) {

						var strFileName = target.FileName.ToString().Replace("[VIXENPROFILEDIR]", logDirectory).Replace('/', '\\').Replace("'", "");
						var strArchiveFileName = target.ArchiveFileName.ToString().Replace("[VIXENPROFILEDIR]", logDirectory).Replace('/', '\\').Replace("'", "");

						target.FileName = strFileName;
						target.ArchiveFileName = strArchiveFileName;
					}
				});

				NLog.LogManager.Configuration = config;
			}
			//config.AllTargets.ToList().ForEach(t => {
			//	var target = t as NLog.Targets.FileTarget;
			//	if (target != null) {

			//		var strFileName = target.FileName.ToString().Replace("[VIXENPROFILEDIR]", _rootDataDirectory).Replace('/', '\\').Replace("'", "");
			//		var strArchiveFileName = target.ArchiveFileName.ToString().Replace("[VIXENPROFILEDIR]", _rootDataDirectory).Replace('/', '\\').Replace("'", "");

			//		target.FileName = strFileName;
			//		target.ArchiveFileName = strArchiveFileName;

			//	}

			//});

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
				if (_activeEditor.IsDisposed) {
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
			foreach (
				KeyValuePair<Guid, string> typeId_FileTypeName in
					ApplicationServices.GetAvailableModules<ISequenceTypeModuleInstance>()) {
				item = new ToolStripMenuItem(typeId_FileTypeName.Value);
				ISequenceTypeModuleDescriptor descriptor =
					ApplicationServices.GetModuleDescriptor(typeId_FileTypeName.Key) as ISequenceTypeModuleDescriptor;

				if (descriptor.CanCreateNew) {
					item.Tag = descriptor.FileExtension;
					item.Click += (sender, e) => {
						ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
						string fileType = (string)menuItem.Tag;
						IEditorUserInterface editor = EditorService.Instance.CreateEditor(fileType);
						if (editor == null) {
							Logging.Error("Can't find an appropriate editor to open file of type " + fileType);
							MessageBox.Show("Can't find an editor to open this file type. (\"" + fileType + "\")",
											"Error opening file", MessageBoxButtons.OK);
						}
						else {
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
			editorUI.Closing +=editorUI_Closing;
			editorUI.Activated +=editorUI_Activated;

			editorUI.StartEditor();
		}

		void editorUI_Activated(object sender, EventArgs e)
			                    	{
			_activeEditor = sender as IEditorUserInterface; 
			                    		}

		void editorUI_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			IEditorUserInterface editor = (sender as IEditorUserInterface);
			if (!_CloseEditor(editor))
			{
				e.Cancel = true;
		}
			else
			{
				editor.EditorClosing();
			}
		}

		private bool _CloseEditor(IEditorUserInterface editor)
		{
			if (editor.IsModified) {
				DialogResult result = MessageBox.Show("Save changes to the sequence?", "Save Changes?",
				                                      MessageBoxButtons.YesNoCancel);
				if (result == System.Windows.Forms.DialogResult.Cancel)
					return false;

				if (result == System.Windows.Forms.DialogResult.Yes)
					editor.Save();
			}

			if (_openEditors.Contains(editor)) {
				_openEditors.Remove(editor);
			}

			_activeEditor= null;
			
			AddSequenceToRecentList(editor.Sequence.FilePath);
			editor.Activated-= editorUI_Activated;
			editor.Closing -= editorUI_Closing;
			//editor.Dispose();
			//editor = null;
			return true;
		}

		#endregion

		private void buttonNewSequence_Click(object sender, EventArgs e)
		{
			//If there is only one editor available, then don't show the context menu, just start it
			if (contextMenuStripNewSequence.Items.Count == 1)
			{
				contextMenuStripNewSequence.Items[0].PerformClick();
			}
			else
			{
				contextMenuStripNewSequence.Show(buttonNewSequence, new Point(0, buttonNewSequence.Height));
			}
			
		}

		private void buttonOpenSequence_Click(object sender, EventArgs e)
		{
			// configure the open file dialog with a filter for currently available sequence types
			string filter = "";
			string allTypes = "";
			IEnumerable<ISequenceTypeModuleDescriptor> sequenceDescriptors =
				ApplicationServices.GetModuleDescriptors<ISequenceTypeModuleInstance>().Cast<ISequenceTypeModuleDescriptor>();
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
					Logging.Error("Can't find an appropriate editor to open file " + filename);
					MessageBox.Show("Can't find an editor to open this file type. (\"" + Path.GetFileName(filename) + "\")",
					                "Error opening file", MessageBoxButtons.OK);
				}
				else {
					_OpenEditor(editor);
				}
			}
			catch (Exception ex) {
				Logging.Error("Error trying to open file '" + filename + "': ", ex);
				MessageBox.Show("Error trying to open file '" + filename + "'.", "Error opening file", MessageBoxButtons.OK);
			}
		}

		private void SetupElements()
		{
			using (ConfigElements form = new ConfigElements()) {
				DialogResult result = form.ShowDialog();
				if (result == DialogResult.OK) {
					VixenSystem.SaveSystemConfig();
				}
				else {
					VixenSystem.ReloadSystemConfig();
				}
			}
		}

		private void SetupControllers()
		{
			using (ConfigControllers form = new ConfigControllers()) {
				DialogResult result = form.ShowDialog();
				if (result == DialogResult.OK) {
					VixenSystem.SaveSystemConfig();
				}
				else {
					VixenSystem.ReloadSystemConfig();
				}
			}
		}

		private void SetupFiltersAndPatching()
		{
			using (ConfigFiltersAndPatching form = new ConfigFiltersAndPatching(_applicationData)) {
				DialogResult result = form.ShowDialog();
				if (result == DialogResult.OK) {
					VixenSystem.SaveSystemConfig();
				}
				else {
					VixenSystem.ReloadSystemConfig();
				}
			}
		}

		private void SetupPreviews()
		{
			using (ConfigPreviews form = new ConfigPreviews()) {
				DialogResult result = form.ShowDialog();
				if (result == DialogResult.OK) {
					VixenSystem.SaveSystemConfig();
				}
				else {
					VixenSystem.ReloadSystemConfig();
				}
			}
		}

		private void SetupDisplay()
		{
			using (DisplaySetup form = new DisplaySetup()) {
				DialogResult dr = form.ShowDialog();

				if (dr == DialogResult.OK) {
					VixenSystem.SaveSystemConfig();
				}
				else {
					VixenSystem.ReloadSystemConfig();
				}
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

		private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var dlg = new OptionsDialog();
			var res = dlg.ShowDialog();
			// so far the dialog box does it all, no real need for this check...
			if( res != DialogResult.OK)
				return;
		}

		// we can't get passed in a state to display, since it may be called out-of-order if we're invoking across threads, etc.
		// so instead, just take this as a notification to update with the current state of the execution engine.
		private void updateExecutionState()
		{
			toolStripStatusLabelExecutionState.Text = "Execution: " + Vixen.Sys.Execution.State;

			if (Execution.IsOpen) {
				toolStripStatusLabelExecutionLight.BackColor = Color.ForestGreen;
			}
			else if (Execution.IsClosed) {
				toolStripStatusLabelExecutionLight.BackColor = Color.Firebrick;
			}
			else if (Execution.IsInTest) {
				toolStripStatusLabelExecutionLight.BackColor = Color.DodgerBlue;
			}
			else {
				toolStripStatusLabelExecutionLight.BackColor = Color.Gold;
			}

			startToolStripMenuItem.Enabled = !Execution.IsOpen;
			stopToolStripMenuItem.Enabled = !Execution.IsClosed;
		}

		private void _ViewLog(string logName)
		{
			string logDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Vixen 3", "Logs");

			using (Process process = new Process()) {
				process.StartInfo = new ProcessStartInfo("notepad.exe", Path.Combine(logDirectory,logName));
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
			}
			else {
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
				_applicationData.RecentSequences.RemoveRange(_maxRecentSequences,
				                                             _applicationData.RecentSequences.Count - _maxRecentSequences);

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

		private void viewInstalledModulesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (InstalledModules installedModules = new InstalledModules()) {
				installedModules.ShowDialog();
			}
		}

		#region Stats

		private const int StatsUpdateInterval = 1000; // ms
		private Timer _statsTimer;
		private Process _thisProc;

		private void InitStats()
		{
			_thisProc = Process.GetCurrentProcess();
			_cpuUsage = new CpuUsage();

			_statsTimer = new Timer();
			_statsTimer.Interval = StatsUpdateInterval;
			_statsTimer.Tick += statsTimer_Tick;
			statsTimer_Tick(null, EventArgs.Empty); // Fake the first update.
			_statsTimer.Start();
		}

		private void statsTimer_Tick(object sender, EventArgs e)
		{
			long memUsage = _thisProc.PrivateMemorySize64/1024/1024;
			long sharedMem = _thisProc.VirtualMemorySize64/1024/1024;

			toolStripStatusLabel_memory.Text = String.Format("Mem: {0}/{2} MB   CPU: {1}%",
			                                                 memUsage, _cpuUsage.GetUsage(), sharedMem);
		}

		#endregion

		private void profilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DataProfileForm f = new DataProfileForm();
			if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				// Do something...
				MessageBox.Show("You must re-start Vixen for the changes to take effect.", "Profiles Changed", MessageBoxButtons.OK);
			}
		}

		private void setupElementsGroupsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetupElements();
		}

		private void setupControllersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetupControllers();
		}

		private void setupFiltersPatchingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetupFiltersAndPatching();
		}

		private void setupDisplayToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetupDisplay();
		}

		private void setupPreviewsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetupPreviews();
		}

		private void buttonSetupOutputPreviews_Click(object sender, EventArgs e)
		{
			SetupPreviews();
		}

		private void buttonSetupDisplay_Click(object sender, EventArgs e)
		{
			SetupDisplay();
		}
	}
}