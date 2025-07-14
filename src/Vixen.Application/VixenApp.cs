#nullable disable

using Catel.IoC;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Newtonsoft.Json.Linq;
using NLog;
using System.Diagnostics;
using System.Runtime;
using System.Windows;
using Orchestra;
using Vixen.Module.Editor;
using Vixen.Module.SequenceType;
using Vixen.Services;
using Vixen.Sys;
using VixenApplication.Setup;
using Application = System.Windows.Forms.Application;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using Point = System.Drawing.Point;
using SystemFonts = System.Drawing.SystemFonts;
using Timer = System.Windows.Forms.Timer;
using WPFApplication = System.Windows.Application;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Windows.Forms.Integration;
using Common.WPFCommon.Services;
using System.Windows.Media;
using Vixen.Sys.Output;
using VixenApplication.SetupDisplay.Views;

namespace VixenApplication
{
	public partial class VixenApp : BaseForm, IApplication
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private const string LockFile = ".lock";

		private Guid _guid = new Guid("7b903272-73d0-416c-94b1-6932758b1963");
		private bool _stopping;
		private bool _openExecution = true;
		private bool _disableControllers = false;
		private bool _devBuild = false;
		private bool _testBuild;
		private string _rootDataDirectory;
		private CpuUsage _cpuUsage;
		private int _currentBuildVersion;
		private string _currentReleaseVersion;
		private bool _closing;
		private readonly IProgress<Tuple<int, string>> _startupProgress;

		private readonly VixenApplicationData _applicationData;
		private string _releaseVersion = String.Empty;
		private string _buildVersion = String.Empty;

		public VixenApp()
		{
			InitializeComponent();

			VixenSystem.UIThread = Thread.CurrentThread;
			VixenSystem.UIContext = SynchronizationContext.Current;
			_startupProgress = new Progress<Tuple<int, string>>(UpdateProgress);

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

			// This resource dictionary is required by the Orc Wizard library
			ResourceDictionary dictOrc = new ResourceDictionary
			{
				Source = new Uri("/Orc.Wizard;component/themes/generic.xaml", UriKind.Relative)
			};

			WPFApplication.Current.Resources.MergedDictionaries.Add(dictOrc);
			WPFApplication.Current.Resources.MergedDictionaries.Add(dict);

			// Applies Orc Theme; This call makes the InfoBarMessageControl header bar readable in a dark theme
			WPFApplication.Current.ApplyTheme();

			//End WPF init

			listViewRecentSequences.Items.Clear();
			//labelVersion.Font = new Font("Segoe UI", 14);
			//Get rid of the ugly grip that we dont want to show anyway. 
			//Workaround for a MS bug
			statusStrip.Padding = new Padding(statusStrip.Padding.Left,
			statusStrip.Padding.Top, statusStrip.Padding.Left, statusStrip.Padding.Bottom);
			statusStrip.Font = SystemFonts.StatusFont;

			ThemeUpdateControls.UpdateControls(this);
			statusStrip.BackColor = ThemeColorTable.BackgroundColor;
			statusStrip.ForeColor = ThemeColorTable.ForeColor;
			toolStripStatusLabel1.ForeColor = ThemeColorTable.ForeColor;
			toolStripStatusLabelExecutionLight.ForeColor = ThemeColorTable.ForeColor;
			toolStripStatusLabelExecutionState.ForeColor = ThemeColorTable.ForeColor;
			toolStripStatusLabel_memory.ForeColor = ThemeColorTable.ForeColor;
			contextMenuStripRecent.Renderer = new ThemeToolStripRenderer();
			progressBar.TextColor = ThemeColorTable.ForeColor;
			progressBar.ProgressColor = Color.DarkGreen;
			labelVixen.Font = new Font(labelVixen.Font, System.Drawing.FontStyle.Bold);
			AutoResizeText(labelVixen);
			Font releaseFont = AutoResizeText(labelRelease);
			Font buildFont = AutoResizeText(labelBuild);
			if (releaseFont.Size < buildFont.Size)
				labelBuild.Font = releaseFont;
			else if (releaseFont.Size > buildFont.Size)
				labelRelease.Font = buildFont;

			string[] args = Environment.GetCommandLineArgs();
			foreach (string arg in args)
			{
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
			AppCommands = new AppCommand(this);
			_applicationData = new VixenApplicationData(_rootDataDirectory);

			_rootDataDirectory = _applicationData.DataFileDirectory;

			if (IsProfileLocked(_rootDataDirectory) || !CreateLockFile())
			{
				var form = new MessageBoxForm("Profile is already in use or unable to the lock the profile.", "Error", MessageBoxButtons.OK, SystemIcons.Error);
				form.ShowDialog(this);
				form.Dispose();
				Environment.Exit(0);
			}

			_stopping = false;
			toolStripStatusUpdates.Text = "";
			PopulateVersionStrings();

			Execution.ExecutionStateChanged += executionStateChangedHandler;
			updateExecutionState();

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

			//Disables the Check for updates menu item as there is no need to have it enabled for Test Builds.
			//	if (labelDebugVersion.Text == "Test Build") updatesMenu.Enabled = false;

			toolStripItemClearSequences.Click += (mySender, myE) => ClearRecentSequencesList();

			// Save off the sequences group box offset from the bottom of dialog
			_sequenceGroupBoxOffsetFromBottom = ClientSize.Height - groupBoxSequences.Bottom;
		}

		/// <summary>
		/// This field stores off how far the sequences group box is from the bottom of the dialog.
		/// This field helps with resizing the dialog.
		/// </summary>
		private int _sequenceGroupBoxOffsetFromBottom;

		private void CreateHelpMenu()
		{
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
		}

		public string LockFilePath { get; set; }

		private static string _uniqueProcessId = null;

		private void UpdateProgress(Tuple<int, string> status)
		{
			progressBar.Value = status.Item1;
			progressBar.CustomText = status.Item2;
		}

		public static string GetUniqueProcessId()
		{
			if (_uniqueProcessId == null)
			{
				var id = Process.GetCurrentProcess().Id;
				var machineName = Environment.MachineName;
				_uniqueProcessId = string.Format("{0}@{1}", id, machineName);
				Logging.Info($"Process info: {_uniqueProcessId}");
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
				Logging.Error(e, "An error occurred creating the profile lock file.");
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
				Logging.Error(e, "An error occurred removing the profile lock file.");
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
						if (myProcessInfo.Length < 2 || lockProcessInfo.Length < 2)
						{
							//We have no real way of knowing, so have to assume it is locked.
							locked = true;
						}
						else if (lockProcessInfo[1].Equals(myProcessInfo[1]))
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
							catch (Exception)
							{
								//Ignore the normal exception when it is not found. We will just remove the lock.
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
				Logging.Error(e, "An error occurred checking the profile lock file.");
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
				Logging.Warn(e, "JIT Profiling Disabled");
			}
		}

		private async void VixenApp_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_openEditors.Any(x => x.IsModified))
			{
				MessageBoxForm mbf = new MessageBoxForm("You have open editor(s) with unsaved changes, are you sure you want to close Vixen?\n\n If you choose yes, you will be prompted to save those editors as they are closed.",
					"Close Vixen?", MessageBoxButtons.YesNo, SystemIcons.Warning);
				var result = mbf.ShowDialog(this);
				if (result == DialogResult.No)
				{
					e.Cancel = true;
					return;
				}
			}

			_closing = true;
			// close all open editors
			foreach (IEditorUserInterface editor in _openEditors.ToArray())
			{
				editor.CloseEditor();
			}

			while (VixenSystem.IsSaving())
			{
				Logging.Warn("Waiting for save to finish before closing.");
				Thread.Sleep(250);
			}

			_stopping = true;
			await VixenSystem.Stop(false);

			_applicationData.SaveData();
			RemoveLockFile(LockFilePath);

			Application.Exit();
		}

		private async void VixenApplication_Load(object sender, EventArgs e)
		{
			EnableButtons(false);
			Cursor = Cursors.WaitCursor;
			_startupProgress?.Report(Tuple.Create(10, "Starting up"));


			if (!await VixenSystem.Start(this, _disableControllers, _rootDataDirectory, _openExecution, _startupProgress))
			{
				var messageBox = new MessageBoxForm("An error occurred starting the system and the application will be halted.", "Error", MessageBoxButtons.OK, SystemIcons.Error);
				messageBox.ShowDialog(this);
				Application.Exit();
			}

			_startupProgress.Report(Tuple.Create(80, "Initializing editors"));
			InitStats();

			RegisterIOC();
			initializeEditorTypes();
			menuStripMain.Renderer = new ThemeToolStripRenderer();

			openFileDialog.InitialDirectory = SequenceService.SequenceDirectory;

			// Add menu items for the logs.
			string logDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Vixen 3", "Logs");

			var di = new System.IO.DirectoryInfo(logDirectory);

			_startupProgress.Report(Tuple.Create(90, "Finalizing"));

			CreateHelpMenu();

			foreach (string logName in di.GetFiles().Select(x => x.Name))
			{
				logsToolStripMenuItem.DropDownItems.Add(logName, null,
														(menuSender, menuArgs) => _ViewLog(((ToolStripMenuItem)menuSender).Text));
			}

			_startupProgress.Report(Tuple.Create(100, "Ready"));

			progressBar.Visible = false;
			PopulateRecentSequencesList();
			EnableButtons();
			MakeTopMost();
			Cursor = Cursors.Default;
		}

		private void EnableButtons(bool enable = true)
		{
			buttonNewSequence.Enabled = buttonOpenSequence.Enabled =
				buttonSetupDisplay.Enabled = buttonSetupOutputPreviews.Enabled = enable;
			menuStripMain.Enabled = enable;
		}

		private void RegisterIOC()
		{
			var serviceLocator = ServiceLocator.Default;
			serviceLocator.RegisterType<IDownloadService, DownloadService>(); 
			serviceLocator.RegisterType<IMessageBoxService, MessageBoxService>(); 
		}

		private void VixenApplication_Shown(object sender, EventArgs e)
		{
			CheckForTestBuild();
			MakeTopMost();
		}

		private async void MakeTopMost()
		{
			await Task.Run(async delegate
			{
				await Task.Delay(1000);
				Invoke(new MethodInvoker(SetTopMost));
			});

		}

		private void SetTopMost()
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
					_releaseVersion = @"Development Build";
					_buildVersion = $@"Build #{version.Build}";
					_currentBuildVersion = version.Build;
					labelBuild.ForeColor = labelRelease.ForeColor = Color.Yellow;
					Logging.Info($"{_releaseVersion} - {_buildVersion}");
					CheckForDevBuildUpdates();
				}
				else
				{
					_releaseVersion = @"Test Build";
					_buildVersion = $@"Build #";
					labelBuild.ForeColor = labelRelease.ForeColor = Color.Red;
					toolStripStatusUpdates.Text = String.Empty;
					Logging.Info($"{_releaseVersion}");
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
				_releaseVersion = $@"Release {_currentReleaseVersion}";
				_buildVersion = $@"Build #{version.Build}";
				_currentBuildVersion = version.Build;
				Logging.Info($"{_releaseVersion} - {_buildVersion}");

				CheckForReleaseUpdates();
			}

			labelRelease.Text = _releaseVersion;
			labelBuild.Text = _buildVersion;

			//Log the runtime versions 
			var runtimeVersion = FileVersionInfo.GetVersionInfo(typeof(int).Assembly.Location).ProductVersion;
			Logging.Info(".NET Runtime is: {0}", runtimeVersion);
		}

		private async void CheckForReleaseUpdates()
		{
			var version = await CheckLatestReleaseVersionAsync();
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
				messageBox.ShowDialog(this);
			}
		}

		private void _ProcessArg(string arg)
		{
			string[] argParts = arg.Split('=');
			switch (argParts[0])
			{
				case "no_controllers":
					_disableControllers = true;
					break;
				case "no_execution":
					_openExecution = false;
					break;
				case "data_dir":
					if (argParts.Length > 1)
					{
						_rootDataDirectory = argParts[1];
					}
					else
					{
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
					var messageBox = new MessageBoxForm(Application.ProductName + " cannot continue without a valid profile." + Environment.NewLine + Environment.NewLine +
						"Are you sure you want to exit " + Application.ProductName + "?",
						Application.ProductName, MessageBoxButtons.YesNo, SystemIcons.Warning);
					messageBox.ShowDialog(this);
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
			messageBox.StartPosition = FormStartPosition.CenterScreen;
			messageBox.ShowDialog();
		}

		private void UpdateTitleWithProfileName(string profileName)
		{
			VixenSystem.ProfileName = profileName;
			Text = string.Format("Vixen Administration - {0} Profile", profileName);
		}

		/// <summary>
		/// Sets the log file paths to the appropriate profile log directory
		/// </summary>
		private void SetLogFilePaths()
		{
			//string logDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Vixen 3");
			string logDirectory = _rootDataDirectory;
			if (System.IO.Directory.Exists(logDirectory))
			{
				NLog.Config.LoggingConfiguration config = NLog.LogManager.Configuration;
				config.AllTargets.ToList().ForEach(t =>
				{
					var target = t as NLog.Targets.FileTarget;
					if (target != null)
					{

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
			foreach (
				KeyValuePair<Guid, string> typeId_FileTypeName in
					ApplicationServices.GetAvailableModules<ISequenceTypeModuleInstance>())
			{
				item = new ToolStripMenuItem(typeId_FileTypeName.Value);
				ISequenceTypeModuleDescriptor descriptor =
					ApplicationServices.GetModuleDescriptor(typeId_FileTypeName.Key) as ISequenceTypeModuleDescriptor;

				if (descriptor.CanCreateNew)
				{
					item.Tag = descriptor.FileExtension;
					item.Click += (sender, e) =>
					{
						Cursor = Cursors.WaitCursor;
						ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
						string fileType = (string)menuItem.Tag;
						IEditorUserInterface editor = EditorService.Instance.CreateEditor(fileType);
						if (editor == null)
						{
							Logging.Error("Can't find an appropriate editor to open file of type " + fileType);
							//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
							MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
							var messageBox = new MessageBoxForm("Can't find an editor to open this file type. (\"" + fileType + "\")",
											"Error opening file", false, false);
							messageBox.ShowDialog(this);
						}
						else
						{
							Cursor = Cursors.WaitCursor;
							_OpenEditor(editor);
						}

						Cursor = Cursors.Default;
					};
					contextMenuStripNewSequence.Items.Add(item);
				}
			}
		}

		private void _OpenEditor(IEditorUserInterface editorUI)
		{
			_openEditors.Add(editorUI);
			editorUI.Closing += editorUI_Closing;
			editorUI.Activated += editorUI_Activated;

			editorUI.StartEditor();
		}

		void editorUI_Activated(object sender, EventArgs e)
		{
			_activeEditor?.EditorLostActivation();
			_activeEditor = sender as IEditorUserInterface;
			_activeEditor.EditorGotActivation();
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
			if (editor.IsModified)
			{
				var messageBox = new MessageBoxForm($"Save changes to the sequence {editor.Sequence?.Name}?", "Save Changes?", _closing ? MessageBoxButtons.YesNo : MessageBoxButtons.YesNoCancel, SystemIcons.Question);
				messageBox.StartPosition = FormStartPosition.CenterScreen;
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

			if (_openEditors.Contains(editor))
			{
				_openEditors.Remove(editor);
			}

			_activeEditor = null;

			AddSequenceToRecentList(editor.Sequence.FilePath);
			editor.Activated -= editorUI_Activated;
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
			foreach (ISequenceTypeModuleDescriptor descriptor in sequenceDescriptors)
			{
				filter += descriptor.TypeName + " (*" + descriptor.FileExtension + ")|*" + descriptor.FileExtension + "|";
				allTypes += "*" + descriptor.FileExtension + ";";
			}
			filter += "All files (*.*)|*.*";
			filter = "All Sequence Types (" + allTypes + ")|" + allTypes + "|" + filter;

			openFileDialog.Filter = filter;

			// if the user hit 'ok' on the dialog, try opening the selected file(s) in an approriate editor
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				foreach (string file in openFileDialog.FileNames)
				{
					OpenSequenceFromFile(file);
				}
			}
		}

		private void OpenSequenceFromFile(string filename)
		{
			Cursor = Cursors.WaitCursor;
			try
			{
				IEditorUserInterface editor = EditorService.Instance.CreateEditor(filename);

				if (editor == null)
				{
					Logging.Error("Can't find an appropriate editor to open file " + filename);
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Can't find an editor to open this file type. (\"" + Path.GetFileName(filename) + "\")",
									"Error opening file", false, false);
					messageBox.ShowDialog(this);
				}
				else
				{
					_OpenEditor(editor);
					Cursor = Cursors.Default;
				}
			}
			catch (Exception ex)
			{
				Logging.Error(ex, "Error trying to open file '" + filename + "': ");
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Error trying to open file '" + filename + "'.", "Error opening file", false, false);
				messageBox.ShowDialog(this);
			}
		}

		private async void SetupPreviews()
		{
			using (ConfigPreviews form = new ConfigPreviews())
			{
				DialogResult result = form.ShowDialog();
				if (result == DialogResult.OK)
				{
					Cursor = Cursors.WaitCursor;
					EnableButtons(false);
					progressBar.Visible = true;
					UpdateProgress(Tuple.Create(0, "Saving System Configuration"));
					await VixenSystem.SaveSystemConfigAsync();
					UpdateProgress(Tuple.Create(50, "Saving Module Configuration"));
					await VixenSystem.SaveModuleConfigAsync();
					progressBar.Visible = false;
					EnableButtons();
					Cursor = Cursors.Default;
				}
				else
				{
					Cursor = Cursors.WaitCursor;
					EnableButtons(false);
					progressBar.Visible = true;
					UpdateProgress(Tuple.Create(0, "Reloading Configuration"));
					VixenSystem.ReloadSystemConfig();
					progressBar.Visible = false;
					EnableButtons();
					Cursor = Cursors.Default;
					MakeTopMost();
				}
			}
		}

        private async Task SetupDisplayNew()
        {
	        List<OutputPreview> previewsToRestart = new();
	        foreach (OutputPreview oc in VixenSystem.Previews)
	        {
		        if (oc.IsRunning)
		        {
					oc.Stop();
					previewsToRestart.Add(oc);
		        }
	        }
			
	        var form = new SetupDisplayWindow();
            ElementHost.EnableModelessKeyboardInterop(form);
            form.ShowDialog();

			foreach (OutputPreview oc in previewsToRestart)
			{
				oc.Start();
			}

			//Give a little delay and then ensure we are on top.
			await Task.Delay(500);
			TopMost = true;
			TopMost = false;
		}

		private async void SetupDisplay()
		{
			using (Setup.DisplaySetup form = new Setup.DisplaySetup())
			{
				DialogResult dr = form.ShowDialog();

				if (dr == DialogResult.OK)
				{
					Cursor = Cursors.WaitCursor;
					EnableButtons(false);
					progressBar.Visible = true;
					UpdateProgress(Tuple.Create(0, "Saving System Configuration"));
					await VixenSystem.SaveSystemConfigAsync();
					UpdateProgress(Tuple.Create(50, "Saving Module Configuration"));
					await VixenSystem.SaveModuleConfigAsync();
					progressBar.Visible = false;
					EnableButtons();
					Cursor = Cursors.Default;
				}
				else
				{
					Cursor = Cursors.WaitCursor;
					EnableButtons(false);
					progressBar.Visible = true;
					UpdateProgress(Tuple.Create(0, "Reloading Configuration"));
					VixenSystem.ReloadSystemConfig();
					progressBar.Visible = false;
					EnableButtons();
					Cursor = Cursors.Default;
					MakeTopMost();
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
			if (_stopping)
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

			if (Execution.IsOpen)
			{
				toolStripStatusLabelExecutionLight.BackColor = Color.ForestGreen;
			}
			else if (Execution.IsClosed)
			{
				toolStripStatusLabelExecutionLight.BackColor = Color.Firebrick;
			}
			else if (Execution.IsInTest)
			{
				toolStripStatusLabelExecutionLight.BackColor = Color.DodgerBlue;
			}
			else
			{
				toolStripStatusLabelExecutionLight.BackColor = Color.Gold;
			}

			startToolStripMenuItem.Enabled = !Execution.IsOpen;
			stopToolStripMenuItem.Enabled = !Execution.IsClosed;
		}

		private void _ViewLog(string logName)
		{
			string logDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Vixen 3", "Logs");

			using (Process process = new Process())
			{
				process.StartInfo = new ProcessStartInfo("notepad.exe", Path.Combine(logDirectory, logName));
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

			if (File.Exists(file))
			{
				OpenSequenceFromFile(file);
			}
			else
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Can't find selected sequence.", "Error", false, false);
				messageBox.ShowDialog(this);
			}
		}

		private void AddSequenceToRecentList(string filename)
		{
			// remove the item from the list if it exists, then insert it in the front
			foreach (string filepath in _applicationData.RecentSequences.ToArray())
			{
				if (filepath == filename)
				{
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

			foreach (string filepath in _applicationData.RecentSequences)
			{
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
				cc[i].Width = listViewRecentSequences.Width - (int)(listViewRecentSequences.Width * .18d);
			}
		}

		#endregion

		private void viewInstalledModulesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (InstalledModules installedModules = new InstalledModules())
			{
				installedModules.ShowDialog();
			}
		}

		#region Stats

		private const int StatsUpdateInterval = 1000; // ms
		private Timer _statsTimer;


		private void InitStats()
		{
			_cpuUsage = new CpuUsage();

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




			toolStripStatusLabel_memory.Text = String.Format("CPU: {0}%", _cpuUsage.GetUsage());
		}

		#endregion

		private void profilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DataProfileForm f = new DataProfileForm();
			if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				// Do something...
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Information; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("You must re-start Vixen for the changes to take effect.", "Profiles Changed", false, false);
				messageBox.ShowDialog(this);
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
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
				SetupDisplayNew();
            }
            else
            {
				SetupDisplay();
			}
				
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

			if (!await CheckForConnectionToWebsite())
			{
				var messageBox = new MessageBoxForm("Unable to reach http://bugs.vixenlights.com. Please check your internet connection and verify you can reach the site and try again.", "Error", MessageBoxButtons.OK, SystemIcons.Error);
				messageBox.ShowDialogThreadSafe(this);
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

		private void ClearRecentSequencesList()
		{
			_applicationData.RecentSequences.Clear();
			listViewRecentSequences.Items.Clear();
		}

		/// <summary>
		/// Occurs when the Size property value changes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments.</param>
		private void VixenApplication_SizeChanged(object sender, EventArgs e)
		{
			// Save off the current group box current height
			int originalHeight = groupBoxSequences.Height;

			// Calculate the new height of the group box
			int newHeight = ClientSize.Height - groupBoxSequences.Top - _sequenceGroupBoxOffsetFromBottom;

			// Adjust the list view first otherwise the group box won't size properly
			listViewRecentSequences.Height += newHeight - originalHeight;

			// Resize the group box
			groupBoxSequences.Height = newHeight;

			// Resize the texts in the top pane, making sure the Release and Build fonts
			// are ultimately the same size.
			AutoResizeText(labelVixen);
			Font releaseFont = AutoResizeText(labelRelease);
			Font buildFont = AutoResizeText(labelBuild);
			if (releaseFont.Size < buildFont.Size)
				labelBuild.Font = releaseFont;
			else if (releaseFont.Size > buildFont.Size)
				labelRelease.Font = buildFont;

			// Refresh the dialog 
			Refresh();
			}

		/// <summary>
		/// Resize the text in a control so that it completely fills the available space
		/// </summary>
		/// <param name="control">Source control to resize text</param>
		/// <param name="maxFontSize">Optional: Maximum font size. Defaults to 200</param>
		/// <returns>Font - the resulting font size of the text</returns>
		private Font AutoResizeText(Control control, int maxFontSize = 200)
		{
			Graphics graphics = control.CreateGraphics();
			Font returnFont = control.Font;

			// Loop through all potential font sizes, looking for one that just fits           
			for (int AdjustedSize = maxFontSize; AdjustedSize >= 2; AdjustedSize--)
			{
				Font TestFont = new Font(control.Font.Name, AdjustedSize, control.Font.Style);

				// Get size of the string
				SizeF AdjustedSizeNew = graphics.MeasureString(control.Text, TestFont);

				// Test to see if the new string will fit within the space. We'll use
				// 94% to leave a little border
				if (0.94 * control.Width > AdjustedSizeNew.Width && 0.94 * control.Height > AdjustedSizeNew.Height)
				{
					// Found a font that will fit.
					control.Font = TestFont;
					returnFont = TestFont;
					break;
				}
			}

			graphics.Dispose();
			return returnFont;
		}
	}
}
