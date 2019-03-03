using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Vixen.Module.Editor;
using Vixen.Module.SequenceType;
using Vixen.Services;
using Vixen.Sys;
using NLog;
using Common.Resources.Properties;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Application = System.Windows.Forms.Application;
using Point = System.Drawing.Point;
using SystemFonts = System.Drawing.SystemFonts;
using Timer = System.Windows.Forms.Timer;
using WPFApplication = System.Windows.Application;

namespace VixenApplication
{
	public partial class VixenApplication : BaseForm, IApplication
	{
		private static NLog.Logger Logging = LogManager.GetCurrentClassLogger();
		private const string ErrorMsg = "An application error occurred. Please contact the Vixen Dev Team " +
									"with the following information:\n\n";

		private const string LockFile = ".lock";

		private Guid _guid = new Guid("7b903272-73d0-416c-94b1-6932758b1963");
		private bool stopping;
		private bool _openExecution = true;
		private bool _disableControllers = false;
		private bool _devBuild = false;
		private bool _testBuild;
		private string _rootDataDirectory;
		private CpuUsage _cpuUsage;
		private bool _perfCountersAvailable;
		private int _currentBuildVersion;
		private string _currentReleaseVersion;


		private VixenApplicationData _applicationData;

		public VixenApplication()
		{
			InitializeComponent();

            //Begin WPF init
		    if (WPFApplication.Current == null)
		    {
		        // create the Application object
		        var app = new WPFApplication();
		    }

		    WPFApplication.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            //Load up the common WPF them file for our WPF application parts.
            ResourceDictionary dict = new ResourceDictionary
		    {
		        Source = new Uri("/WPFCommon;component/Theme/Theme.xaml", UriKind.Relative)
		    };

			WPFApplication.Current.Resources.MergedDictionaries.Add(dict);

            //End WPF init

            labelVersion.Font = new Font("Segoe UI", 14);
			//Get rid of the ugly grip that we dont want to show anyway. 
			//Workaround for a MS bug
			statusStrip.Padding = new Padding(statusStrip.Padding.Left,
			statusStrip.Padding.Top, statusStrip.Padding.Left, statusStrip.Padding.Bottom);
			statusStrip.Font = SystemFonts.StatusFont;

			Icon = Resources.Icon_Vixen3;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			statusStrip.BackColor = ThemeColorTable.BackgroundColor;
			statusStrip.ForeColor = ThemeColorTable.ForeColor;
			toolStripStatusLabel1.ForeColor = ThemeColorTable.ForeColor;
			toolStripStatusLabelExecutionLight.ForeColor = ThemeColorTable.ForeColor;
			toolStripStatusLabelExecutionState.ForeColor = ThemeColorTable.ForeColor;
			toolStripStatusLabel_memory.ForeColor = ThemeColorTable.ForeColor;
			contextMenuStripRecent.Renderer = new ThemeToolStripRenderer();

			string[] args = Environment.GetCommandLineArgs();
			foreach (string arg in args) {
				_ProcessArg(arg);
			}

			Logging.Info("Starting JIT Profiler");
			StartJITProfiler();
			Logging.Info("Completed JIT Profiler");

			if (_rootDataDirectory == null)
			{
				Logging.Info("Processing Profiles");
				ProcessProfiles();
				Logging.Info("Finished Processing Profiles");
			}

			_applicationData = new VixenApplicationData(_rootDataDirectory);

			_rootDataDirectory = _applicationData.DataFileDirectory;

			if (IsProfileLocked(_rootDataDirectory) || !CreateLockFile())
			{
				var form = new MessageBoxForm("Profile is already in use or unable to the lock the profile.","Error",MessageBoxButtons.OK, SystemIcons.Error);
				form.ShowDialog();
				form.Dispose(); 
				Environment.Exit(0);
			}
			
			stopping = false;
			toolStripStatusUpdates.Text = "";
			PopulateVersionStrings();

			AppCommands = new AppCommand(this);
			Execution.ExecutionStateChanged += executionStateChangedHandler;
			if(!VixenSystem.Start(this, _openExecution, _disableControllers, _applicationData.DataFileDirectory))
			{
				var messageBox = new MessageBoxForm("An error occured starting the system and the application will be halted.", "Error",MessageBoxButtons.OK, SystemIcons.Error);
				messageBox.ShowDialog();
				Application.Exit();
			}

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

			ToolStripMenuItem helpMenu = new ToolStripMenuItem("Help");
			menuStripMain.Items.Add(helpMenu);

			ToolStripMenuItem onlineMenu = new ToolStripMenuItem("Online Help");
			onlineMenu.Click += new System.EventHandler(this.OnlineHelpMenu_Click);
			helpMenu.DropDown.Items.Add(onlineMenu);

			ToolStripMenuItem vixenYouTubeChannelMenu = new ToolStripMenuItem("Vixen YouTube Channel");
			vixenYouTubeChannelMenu.Click += new System.EventHandler(this.VixenYouTubeChannelMenu_Click);
			helpMenu.DropDown.Items.Add(vixenYouTubeChannelMenu);

			ToolStripMenuItem updatesMenu = new ToolStripMenuItem("Check for Updates");
			updatesMenu.Click += new System.EventHandler(this.UpdatesMenu_Click);
			helpMenu.DropDown.Items.Add(updatesMenu);

			ToolStripMenuItem releaseNotesMenu = new ToolStripMenuItem("Release Notes");
			releaseNotesMenu.Click += new System.EventHandler(this.ReleaseNotesMenu_Click);
			helpMenu.DropDown.Items.Add(releaseNotesMenu);

			ToolStripMenuItem aboutMenu = new ToolStripMenuItem("About Vixen");
			aboutMenu.Click += new System.EventHandler(this.AboutMenu_Click);
			helpMenu.DropDown.Items.Add(aboutMenu);

			//Disables the Check for updates menu item as there is no need to have it enabled for Test Builds.
		//	if (labelDebugVersion.Text == "Test Build") updatesMenu.Enabled = false;

			toolStripItemClearSequences.Click += (mySender, myE) => ClearRecentSequencesList();
		}

		public string LockFilePath { get; set; }

		private static string _uniqueProcessId = null;

		public static string GetUniqueProcessId()
		{
			if (_uniqueProcessId == null)
			{
				var id = Process.GetCurrentProcess().Id;
				var machineName = Environment.MachineName;
				_uniqueProcessId = string.Format("{0}@{1}", id, machineName);
			}

			return _uniqueProcessId;
		}
		
		private bool CreateLockFile()
		{
			bool success = false;
			try
			{
				if (!Directory.Exists(_rootDataDirectory))
				{
					//Our startup folder is not present, so create it.
					Directory.CreateDirectory(_rootDataDirectory);
				}
				LockFilePath = Path.Combine(_rootDataDirectory, LockFile);
				if (!File.Exists(LockFilePath))
				{
					File.WriteAllText(LockFilePath, GetUniqueProcessId());
					//Set this back on the root app to use in case of system errors and we need a failsafe way to delete the lock
					Program.LockFilePath = LockFilePath;
					success = true;
				}

			}
			catch (Exception e)
			{
				Logging.Error(e, "An error occured creating the profile lock file.");
			}

			return success;
		}

		internal bool RemoveLockFile()
		{
			return RemoveLockFile(LockFilePath);
		}

		internal static bool RemoveLockFile(string lockFilePath)
		{
			bool success = false;
			try
			{
				if (File.Exists(lockFilePath))
				{
					File.Delete(lockFilePath);
					success = true;
				}
			}
			catch (Exception e)
			{
				Logging.Error(e, "An error occured removing the profile lock file.");
			}

			return success;
		}

		internal static bool IsProfileLocked(string path)
		{
			bool locked = false;
			try
			{
				if (Directory.Exists(path))
				{
					var lockFilePath = Path.Combine(path, LockFile);
					if (File.Exists(lockFilePath))
					{
						var lockProcessInfo = File.ReadAllText(lockFilePath).Split('@');
						var myProcessInfo = GetUniqueProcessId().Split('@');
						if (lockProcessInfo[1].Equals(myProcessInfo[1]))
						{
							//The machine name is the same so it was locked by this machine
							var lockProcessId = Convert.ToInt32(lockProcessInfo[0]);
							try
							{
								//See if the locking process is still running
								var process = Process.GetProcessById(lockProcessId);
								if (process.ProcessName.StartsWith("Vixen"))
								{
									//The process is running and it is Vixen so we ae locked.
									locked = true;
								}
								else
								{
									//Process is something other than Vixen so we can release the lock
									RemoveLockFile(lockFilePath);
								}

							}
							catch (Exception e)
							{
								//No process with that id so release the lock.
								RemoveLockFile(lockFilePath);
							}
						}
						else
						{
							//The machine name is not us, so we have to assume it is locked on some other device.
							locked = true;
						}
						
					}
				}
			}
			catch (Exception e)
			{
				Logging.Error(e, "An error occured checking the profile lock file.");
				locked = true;  //If we cannot determine if it is locked, then we can't assume it isn't.
			}

			return locked;
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

		private async void VixenApp_FormClosing(object sender, FormClosingEventArgs e)
		{
			// close all open editors
			foreach (IEditorUserInterface editor in _openEditors.ToArray()) {
				editor.CloseEditor();
			}

			while (VixenSystem.IsSaving())
			{
				Logging.Warn("Waiting for save to finish before closing.");
				Thread.Sleep(250);
			}

			stopping = true;
			await VixenSystem.Stop(false);

			_applicationData.SaveData();
			RemoveLockFile(LockFilePath);
			
			Application.Exit();
		}

		private void VixenApplication_Load(object sender, EventArgs e)
		{
			initializeEditorTypes();
			menuStripMain.Renderer = new ThemeToolStripRenderer();
			
			openFileDialog.InitialDirectory = SequenceService.SequenceDirectory;

			// Add menu items for the logs.
			string logDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Vixen 3", "Logs");

			var di = new System.IO.DirectoryInfo(logDirectory);

			foreach (string logName in di.GetFiles().Select(x => x.Name)) {
				logsToolStripMenuItem.DropDownItems.Add(logName, null,
				                                        (menuSender, menuArgs) => _ViewLog(((ToolStripMenuItem) menuSender).Text));
			//	logsToolStripMenuItem.DropDownItems.ForeColor = Color.FromArgb(90, 90, 90);
			}
			PopulateRecentSequencesList();
		}

		private async void VixenApplication_Shown(object sender, EventArgs e)
		{
			CheckForTestBuild();
			//Try to make sure at load we are on top.
			await Task.Delay(750);
			MakeTopMost();
		}

		private void MakeTopMost()
		{
			TopMost = true;
			TopMost = false;
		}

		private void PopulateVersionStrings()
		{
			System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(VixenSystem.AssemblyFileName);
			Version version = assembly.GetName().Version;

			_devBuild = version.Major == 0;

			if (_devBuild)
			{
				if (version.Build > 0)
				{
					labelVersion.Text = @"Development Build";
					labelDebugVersion.Text = $@"Build #{version.Build}" ;
					_currentBuildVersion = version.Build;
					labelDebugVersion.ForeColor = labelVersion.ForeColor = Color.Yellow;
					Logging.Info($"{labelVersion.Text} - {labelDebugVersion.Text}");
					CheckForDevBuildUpdates();
				}
				else
				{
					labelVersion.Text = @"Test Build";
					labelDebugVersion.Text = $@"Build #";
					labelDebugVersion.ForeColor = labelVersion.ForeColor = Color.Red;
					toolStripStatusUpdates.Text = String.Empty;
					Logging.Info($"{labelVersion.Text}");
					_testBuild = true;
				}

				
			}
			else
			{
				_currentReleaseVersion = $@"{version.Major}.{version.Minor}";
				if (version.Revision > 0)
				{
					_currentReleaseVersion += $@"u{version.Revision}";
				}
				labelVersion.Text = $@"Release {_currentReleaseVersion}";
				labelDebugVersion.Text = $@"Build #{version.Build}";
				_currentBuildVersion = version.Build;
				Logging.Info($"{labelVersion.Text} - {labelDebugVersion.Text}");

				CheckForReleaseUpdates();
			}

			labelDebugVersion.Visible = true;

			//Log the runtime versions 
			var runtimeVersion = FileVersionInfo.GetVersionInfo(typeof (int).Assembly.Location).ProductVersion;
			Logging.Info(".NET Runtime is: {0}", runtimeVersion);
		}

		private async void CheckForReleaseUpdates()
		{
			var version =await CheckLatestReleaseVersionAsync();
			if (!string.IsNullOrEmpty(version))
			{
				toolStripStatusUpdates.Text = $@" Release version {version} available.";
				Logging.Info(toolStripStatusUpdates.Text);
			}
		}

		private async void CheckForDevBuildUpdates()
		{
			var version = await CheckLatestBuildVersionAsync();
			if (!string.IsNullOrEmpty(version))
			{
				toolStripStatusUpdates.Text = $@" Development build {version} available.";
				Logging.Info(toolStripStatusUpdates.Text);
			}
		}

		public async Task<string> CheckLatestBuildVersionAsync()
		{
			try
			{
				if (await CheckForConnectionToWebsite())
				{
					using (HttpClient wc = new HttpClient())
					{
						wc.Timeout = TimeSpan.FromMilliseconds(5000);
						//Get Latest Build
						string getLatestDevelopementBuild =
							await wc.GetStringAsync(
								$"http://bugs.vixenlights.com/rest/api/latest/search?jql=Project='Vixen 3' AND fixVersion=DevBuild AND 'Fix Build Number'>{_currentBuildVersion} ORDER BY 'Fix Build Number' DESC&startAt=0&maxResults=1");
						//This will parse the latest development build number
						dynamic developementBuild = JObject.Parse(getLatestDevelopementBuild);
						if (developementBuild.issues.Count > 0)
						{
							if (developementBuild.issues[0].fields.customfield_10112 != null)
							{
								int latestDevelopementBuild = developementBuild.issues[0].fields.customfield_10112;
								//This does not return an array as the results are contained in a wrapper object for paging info
								//There results are in an array called issues, with in that is a set of fields that contain our custom field 
								if (latestDevelopementBuild > _currentBuildVersion)
								{
									return latestDevelopementBuild.ToString();
								}
							}
						}
						
					}
				}
			}
			catch (Exception e)
			{
				//Should only get here if there is no internet connection and e will stipulate that it can't get to the http://bugs.vixenlights.com website.
				Logging.Error("Checking for the latest Development Build failed - " + e);
			}
			return string.Empty;
		}

		public async Task<string> CheckLatestReleaseVersionAsync()
		{
			try
			{
				if (await CheckForConnectionToWebsite())
				{
					using (HttpClient wc = new HttpClient())
					{
						wc.Timeout = TimeSpan.FromMilliseconds(5000);
						//Get the Latest Release
						string getReleaseVersion =
							await wc.GetStringAsync("http://bugs.vixenlights.com/rest/api/latest/project/VIX/versions?orderBy=releaseDate");
						//Query returns an array of release version objects
						var releaseVersions = JArray.Parse(getReleaseVersion);
						//get the last one that has released == true as they are in asending order
						dynamic lastReleaseVersion = releaseVersions.Last(m => (bool)m.SelectToken("released"));
						//This is the name of the release
						string releaseVersion = lastReleaseVersion.name;

						if (releaseVersion != _currentReleaseVersion)
						{
							return releaseVersion;
						}
					}
				}
			}
			catch (Exception e)
			{
				//Should only get here if there is no internet connection and e will stipulate that it can't get to the http://bugs.vixenlights.com website.
				Logging.Error("Checking for the latest Release Version failed - " + e);
			}
			return "";
		}

		private void CheckForTestBuild()
		{
			if (_devBuild && !_testBuild) //Don't annoy developers 
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Exclamation;
				var messageBox = new MessageBoxForm("Please be aware that this is a development version. Some parts of the software may not work, and data loss is possible! Please backup your data before using this version of the software.", "Development/Test Software", false, false);
				messageBox.ShowDialog();
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

			// if we don't have any profiles yet, fall through so the "Default" profile will be created
			int profileCount = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "ProfileCount", 0);
			if (profileCount == 0)
			{
				return;
			}

			// now that we know we have profiles, get the rest of the settings
			string loadAction = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "LoadAction", "LoadSelected");
			int profileToLoad = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "ProfileToLoad", -1);

			// try to load the selected profile
			if (loadAction != "Ask" && profileToLoad > -1 && profileToLoad < profileCount)
			{
				string directory = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + profileToLoad + "/DataFolder", string.Empty);
				var isLocked = IsProfileLocked(directory);
				if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory) && !isLocked)
				{
					_rootDataDirectory = directory;
					string profileName = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + profileToLoad + "/Name", string.Empty);
					UpdateTitleWithProfileName(profileName);
				}
				else
				{
					string name = profile.GetSetting(XMLProfileSettings.SettingType.Profiles, "Profile" + profileToLoad + "/Name", string.Empty);
					ShowLoadProfileErrorMessage(name, isLocked);
				}
			}

			// if _rootDataDirectory is still empty at this point either we're configured to always ask or loading the selected profile failed
			// keep asking until we get a good profile directory
			while (string.IsNullOrEmpty(_rootDataDirectory))
			{
				SelectProfile selectProfile = new SelectProfile();
				DialogResult result = selectProfile.ShowDialog();

				if (result == DialogResult.OK)
				{
					string directory = selectProfile.DataFolder;
					var isLocked = IsProfileLocked(directory);
					if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory) && !isLocked)
					{
						_rootDataDirectory = directory;
						UpdateTitleWithProfileName(selectProfile.ProfileName);
						break;
					}
					ShowLoadProfileErrorMessage(selectProfile.ProfileName, isLocked);
				}
				else if (result == DialogResult.Cancel)
				{
					var messageBox = new MessageBoxForm(Application.ProductName + " cannot continue without a vaild profile." + Environment.NewLine + Environment.NewLine +
						"Are you sure you want to exit " + Application.ProductName + "?",
						Application.ProductName,MessageBoxButtons.YesNo, SystemIcons.Warning);
					messageBox.ShowDialog();
					if (messageBox.DialogResult == DialogResult.OK)
					{
						Environment.Exit(0);
					}
				}
				else
				{
					// SelectProfile.ShowDialog() should only return DialogResult.OK or Cancel, how did we get here?
					throw new NotImplementedException("SelectProfile.ShowDialog() returned " + result.ToString());
				}
			}
			Logging.Info($"Profile root : {_rootDataDirectory}");
			SetLogFilePaths();
		}

		private static void ShowLoadProfileErrorMessage(string name, bool isLocked)
		{
			var message =
				String.Format(
					"Selected profile {0} {1}!\n\nSelect a different profile to load or use the Profile Editor to create a new profile.",
					name, isLocked ? "is locked by another instance" : "data directory does not exist");
			var messageBox = new MessageBoxForm(message, "Error", MessageBoxButtons.OK, SystemIcons.Error);
			messageBox.ShowDialog();
		}

		private void UpdateTitleWithProfileName(string profileName)
		{
			Text = string.Format("Vixen Administration - {0} Profile", profileName);
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
							//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
							MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
							var messageBox = new MessageBoxForm("Can't find an editor to open this file type. (\"" + fileType + "\")",
											"Error opening file", false, false);
							messageBox.ShowDialog();
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
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Save changes to the sequence?", "Save Changes?", true, true);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.Cancel)
					return false;

				if (messageBox.DialogResult == DialogResult.OK)
					editor.Save();
			}
			else if (editor.IsEditorStateModified)
			{
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
			Cursor.Current = Cursors.WaitCursor;
			try {
				IEditorUserInterface editor = EditorService.Instance.CreateEditor(filename);

				if (editor == null) {
					Logging.Error("Can't find an appropriate editor to open file " + filename);
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Can't find an editor to open this file type. (\"" + Path.GetFileName(filename) + "\")",
									"Error opening file", false, false);
					messageBox.ShowDialog();
				}
				else {
					_OpenEditor(editor);
				}
			}
			catch (Exception ex) {
				Logging.Error("Error trying to open file '" + filename + "': ", ex);
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Error trying to open file '" + filename + "'.", "Error opening file", false, false);
				messageBox.ShowDialog();
			}
		}

		private async void SetupPreviews()
		{
			using (ConfigPreviews form = new ConfigPreviews()) {
				DialogResult result = form.ShowDialog();
				if (result == DialogResult.OK) {
					await VixenSystem.SaveSystemAndModuleConfigAsync();
				}
				else {
					VixenSystem.ReloadSystemConfig();
				}
			}
		}

		private async void SetupDisplay()
		{
			using (DisplaySetup form = new DisplaySetup()) {
				DialogResult dr = form.ShowDialog();

				if (dr == DialogResult.OK) {
					await VixenSystem.SaveSystemAndModuleConfigAsync();
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

		private async void optionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var dlg = new OptionsDialog();
			var res = dlg.ShowDialog();
			// so far the dialog box does it all, no real need for this check...
			if (res == DialogResult.OK)
			{
				await VixenSystem.SaveSystemConfigAsync();
			}
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

		private const int _maxRecentSequences = 20;

		private void listViewRecentSequences_DoubleClick(object sender, EventArgs e)
		{
			if (listViewRecentSequences.SelectedItems.Count <= 0)
				return;

			string file = listViewRecentSequences.SelectedItems[0].Tag as string;

			if (File.Exists(file)) {
				OpenSequenceFromFile(file);
			}
			else {
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Can't find selected sequence.", "Error", false, false);
				messageBox.ShowDialog();
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
			ColumnAutoSize();
		}

		public void ColumnAutoSize()
		{
			listViewRecentSequences.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			ListView.ColumnHeaderCollection cc = listViewRecentSequences.Columns;
			for (int i = 0; i < cc.Count; i++)
			{
				cc[i].Width = listViewRecentSequences.Width - (int)(listViewRecentSequences.Width *.18d);
			}
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
		private PerformanceCounter _committedRamCounter;
		private PerformanceCounter _reservedRamCounter;
		

		private void InitStats()
		{
			_thisProc = Process.GetCurrentProcess();
			_cpuUsage = new CpuUsage();

			//try
			//{
			//	if (PerformanceCounterCategory.Exists(".NET CLR Memory"))
			//	{
			//		_committedRamCounter = new PerformanceCounter(".NET CLR Memory", "# Total committed Bytes", _thisProc.ProcessName);
			//		_reservedRamCounter = new PerformanceCounter(".NET CLR Memory", "# Total reserved Bytes", _thisProc.ProcessName);
			//		_perfCountersAvailable = true;
			//	}
			//}
			//catch (Exception ex)
			//{
			//	Logging.Error("Cannot access performance counters. Refresh the counter list with lodctr /R");
			//}

			_statsTimer = new Timer();
			_statsTimer.Interval = StatsUpdateInterval;
			_statsTimer.Tick += statsTimer_Tick;
			statsTimer_Tick(null, EventArgs.Empty); // Fake the first update.
			_statsTimer.Start();
		}

		private void statsTimer_Tick(object sender, EventArgs e)
		{
			//long memUsage;
			//long reservedMemUsage;

			//if (_perfCountersAvailable)
			//{
			//	memUsage = Convert.ToInt32(_committedRamCounter.NextValue()/1024/1024);
			//	reservedMemUsage = Convert.ToInt32(_reservedRamCounter.NextValue()/1024/1024);
			//}
			//else
			//{
			//	_thisProc.Refresh();
			//	memUsage = _thisProc.PrivateMemorySize64 / 1024 / 1024;
			//	reservedMemUsage = _thisProc.VirtualMemorySize64 / 1024 / 1024;
			//}

			
			

			toolStripStatusLabel_memory.Text = String.Format("CPU: {0}%",_cpuUsage.GetUsage());
		}

		#endregion

		private void profilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DataProfileForm f = new DataProfileForm();
			if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				// Do something...
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Information; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("You must re-start Vixen for the changes to take effect.", "Profiles Changed", false, false);
				messageBox.ShowDialog();
			}
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

		private void OnlineHelpMenu_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Sequencer);
		}

		private void VixenYouTubeChannelMenu_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.YouTubeChannel);
		}

		private async void UpdatesMenu_Click(object sender, EventArgs e)
		{
			string currentVersion;
			string latestVersion;
			string currentVersionType;

			Cursor = Cursors.WaitCursor;

			if (! await CheckForConnectionToWebsite())
			{
				var messageBox = new MessageBoxForm("Unable to reach http://bugs.vixenlights.com. Please check your internet connection and verify you can reach the site and try again.", "Error", MessageBoxButtons.OK, SystemIcons.Error);
				messageBox.ShowDialog();
				Cursor = Cursors.Arrow;
				return;
			}

			if (_devBuild)
			{
				currentVersion = _currentBuildVersion.ToString();
				latestVersion = await CheckLatestBuildVersionAsync();
				currentVersionType = "DevBuild";
			}
			else
			{
				currentVersion = _currentReleaseVersion;
				latestVersion = await CheckLatestReleaseVersionAsync();
				currentVersionType = "Release";
			}

			var checkForUpdates = new CheckForUpdates(currentVersion, latestVersion, currentVersionType);
			checkForUpdates.ShowDialog();
			checkForUpdates.Dispose();
			Cursor = Cursors.Default;
		}

		public static async Task<bool> CheckForConnectionToWebsite()
		{
			try
			{
				using (var client = new HttpClient())
				{
					client.Timeout = TimeSpan.FromMilliseconds(5000);
					using (await client.GetAsync("http://bugs.vixenlights.com"))
					{
						return true;
					}
				}
			}
			catch
			{
				return false;
			}
		}

		private void ReleaseNotesMenu_Click(object sender, EventArgs e)
		{
			var releaseNotes = new ReleaseNotes();
			releaseNotes.ShowDialog();
			releaseNotes.Dispose(); 
		}

		private void AboutMenu_Click(object sender, EventArgs e)
		{
			string currentVersion = _devBuild ? _currentBuildVersion.ToString() : _currentReleaseVersion;
			var aboutVixen = new AboutVixen(currentVersion, _devBuild);
			aboutVixen.ShowDialog();
			aboutVixen.Dispose();
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}

		private void VixenApplication_Paint(object sender, PaintEventArgs e)
		{
			//draws divider lines
			Pen borderColor = new Pen(ThemeColorTable.GroupBoxBorderColor, 1);
			if (ActiveForm != null)
			{
				int extraSpace1 = (int) (30*ScalingTools.GetScaleFactor());
				int extraSpace2 = (int) (40 * ScalingTools.GetScaleFactor());
				e.Graphics.DrawLine(borderColor, 0, pictureBox1.Size.Height + extraSpace1, ActiveForm.Width, pictureBox1.Size.Height + extraSpace1);
				e.Graphics.DrawLine(borderColor, 0, Height - (statusStrip.Height + extraSpace2), Width, Height - (statusStrip.Height + extraSpace2));
			}
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;
		}

		private void ClearRecentSequencesList()
		{
			_applicationData.RecentSequences.Clear();
			listViewRecentSequences.Items.Clear();
		}
	}
}
