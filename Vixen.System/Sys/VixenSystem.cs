using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Instrumentation;
using Vixen.Services;
using Vixen.Sys.Managers;
using Vixen.Sys.Output;

namespace Vixen.Sys
{
	public class VixenSystem
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public enum RunState
		{
			Stopped,
			Starting,
			Started,
			Stopping
		};

		private static RunState _state = RunState.Stopped;
		private static bool _systemConfigSaving;
		private static bool _moduleConfigSaving;

		internal static bool MigrationOccured { get; set; }

		public static Thread UIThread { get; set; }
		public static SynchronizationContext UIContext { get; set; }

		public static bool IsSaving()
		{
			return _systemConfigSaving || _moduleConfigSaving;
		}

		public static async Task<bool> Start(IApplication clientApplication, bool disableDevices, string dataDirectory, bool openExecution = true, IProgress<Tuple<int, string>> progress=null)
		{
			if (_state == RunState.Stopped) {
				try {
					Logging.Info("Vixen System starting up...");

					_state = RunState.Starting;
					
					ApplicationServices.ClientApplication = clientApplication;

					Instrumentation = new Instrumentation.Instrumentation();

					await LoadConfiguration(disableDevices, dataDirectory, progress);

					if (openExecution) {
						Execution.OpenExecution();
					}

					_state = RunState.Started;
					Logging.Info("Vixen System successfully started.");
				}
				catch (Exception ex) {
					// The client is expected to have subscribed to the logging event
					// so that it knows that an exception occurred during loading.
					Logging.Error(ex,"Error during system startup!");
					return false;
				}
			}

			return true;
		}

		public static async Task LoadConfiguration(bool disableDevices = false, string dataRootDirectory = null, IProgress<Tuple<int, string>> progress = null)
		{

			await Task.Factory.StartNew(async () =>
			{
				progress?.Report(Tuple.Create(10, "Preparing modules"));
				// A user data file in the binary branch will give any alternate
				// data branch to use.
				Paths.DataRootPath = dataRootDirectory ?? _GetUserDataPath();
					
				ModuleImplementation[] moduleImplementations = Modules.GetImplementations();

				// Build branches for each module type.
				foreach (ModuleImplementation moduleImplementation in moduleImplementations) {
					Helper.EnsureDirectory(Path.Combine(Modules.Directory, moduleImplementation.TypeOfModule));
				}
				// There is going to be a "Common" directory for non-module DLLs.
				// This will give them a place to be other than the module directories.
				// If they're in a module directory, the system will try to load them and
				// it will result in an unnecessary log notification for the user.
				// All other binary directories (module directories) have something driving
				// their presence, but this doesn't.  So it's going to be a blatantly
				// ugly statement for now.
				Helper.EnsureDirectory(Path.Combine(Paths.BinaryRootPath, "Common"));

				progress?.Report(Tuple.Create(20, "Loading modules"));

				// Load all module descriptors.
				Modules.LoadAllModules();

				progress?.Report(Tuple.Create(30, "Loading system config"));
				LoadSystemConfig(progress);

				DefaultUpdateTimeSpan = TimeSpan.FromMilliseconds(SystemConfig.DefaultUpdateInterval);

				// Add modules to repositories.
				Modules.PopulateRepositories();

				if (disableDevices) {
					SystemConfig.DisabledDevices = OutputDeviceManagement.Devices;
				}

				if (MigrationOccured)
				{
					progress?.Report(Tuple.Create(80, "Saving migrated config"));
					//save the configs to ensure they do not get lost. We no longer automatically save on close.
					await SaveSystemAndModuleConfigAsync();
					MigrationOccured = false;
				}

				progress?.Report(Tuple.Create(70, "Config loaded"));
			});
			
		}

		public static async Task Stop(bool save = true)
		{
			if (_state == RunState.Starting || _state == RunState.Started) {
				_state = RunState.Stopping;
				Logging.Info("Vixen System stopping...");
				ApplicationServices.ClientApplication = null;
				// Need to get the disabled devices before stopping them all.
				SaveDisabledDevices();
				Execution.CloseExecution();
				Modules.ClearRepositories();
				if (save)
				{
					await SaveSystemAndModuleConfigAsync();
				}
				_state = RunState.Stopped;
				Logging.Info("Vixen System successfully stopped.");
			}

		}

		public static void SaveDisabledDevices()
		{
			if (SystemConfig != null) {
				SystemConfig.DisabledDevices = OutputDeviceManagement.Devices.Where(x => x != null).Where(x => !x.IsRunning);
			}
		}

		public static async Task<bool> SaveSystemAndModuleConfigAsync()
		{
			List<Task> taskList = new List<Task>();
			taskList.Add(SaveSystemConfigAsync());
			taskList.Add(SaveModuleConfigAsync());
			await Task.WhenAll(taskList.ToArray());
			return true;
		}

		public static async Task<bool> SaveSystemConfigAsync()
		{
			if (SystemConfig != null)
			{
				Logging.Info("Saving System Config.");
				if (_systemConfigSaving)
				{
					Logging.Error("System config is already being saved. Skipping duplicate request.");
					return false;
				}
				_systemConfigSaving = true;
				// 'copy' the current details (nodes/elements/controllers) from the executing state
				// to the SystemConfig, so they're there for writing when we save

				// we may not want to always save the disabled devices to the config (ie. if the system is stopped at the
				// moment) since the disabled devices are inferred from the running status of active devices
				if (_state == RunState.Started)
				{
					SaveDisabledDevices();
				}

				SystemConfig.OutputControllers = OutputControllers;
				//SystemConfig.SmartOutputControllers = SmartOutputControllers;
				SystemConfig.Previews = Previews;
				SystemConfig.Elements = Elements;
				SystemConfig.Nodes = Nodes.GetRootNodes();
				SystemConfig.Filters = Filters;
				SystemConfig.DataFlow = DataFlow;
			
				return await Task.Factory.StartNew(() =>
				{
					SystemConfig.Save();
					_systemConfigSaving = false;
					Logging.Info("System Config saved.");
					return true;
				});
				
			}

			return false;
		}

		public static async Task<bool> SaveModuleConfigAsync()
		{
			
			if (ModuleStore != null)
			{
				if (_moduleConfigSaving)
				{
					Logging.Error("Module config is already being saved. Skipping duplicate request.");
					return false;
				}

				_moduleConfigSaving = true;
				Logging.Info("Saving Module Config.");
				return await Task.Factory.StartNew(() =>
				{
					ModuleStore.Save();
					_moduleConfigSaving = false;
					Logging.Info("Module Config saved.");
					return true;
				});

			}
			return false;

		}

		public static void LoadSystemConfig(IProgress<Tuple<int, string>> progress= null)
		{
			while (_systemConfigSaving || _moduleConfigSaving)
			{
				Logging.Info("Reload Requested while save in progress. Waiting 5 ms.");
				Thread.Sleep(5);
			}
			Execution.initInstrumentation();
			DataFlow = new DataFlowManager();
			Elements = new ElementManager();
			Nodes = new NodeManager();
			OutputControllers = new OutputControllerManager(
				new OutputDeviceCollection<OutputController>(),
				new OutputDeviceExecution<OutputController>());
			//SmartOutputControllers = new SmartOutputControllerManager(
			//	new ControllerLinkingManagement<SmartOutputController>(),
			//	new OutputDeviceCollection<SmartOutputController>(),
			//	new OutputDeviceExecution<SmartOutputController>());
			Previews = new PreviewManager(
				new OutputDeviceCollection<OutputPreview>(),
				new OutputDeviceExecution<OutputPreview>());
			Contexts = new ContextManager();
			Filters = new FilterManager(DataFlow);
			
			ControllerManagement = new ControllerFacade();
			ControllerManagement.AddParticipant(OutputControllers);
			//ControllerManagement.AddParticipant(SmartOutputControllers);
			OutputDeviceManagement = new OutputDeviceFacade();
			OutputDeviceManagement.AddParticipant(OutputControllers);
			//OutputDeviceManagement.AddParticipant(SmartOutputControllers);
			OutputDeviceManagement.AddParticipant(Previews);

			// Load system data in order of dependency.
			// The system data generally resides in the data branch, but it
			// may not be in the case of an alternate context.
			string systemDataPath = _GetSystemDataPath();
			// Load module data before system config.
			// System config creates objects that use modules that have data in the store.
			ModuleStore = _LoadModuleStore(systemDataPath) ?? new ModuleStore();
			progress?.Report(Tuple.Create(50, "Module config loaded"));
			SystemConfig = _LoadSystemConfig(systemDataPath) ?? new SystemConfig();
			progress?.Report(Tuple.Create(70, "System config loaded"));

			Elements.AddElements(SystemConfig.Elements);
			Nodes.AddNodes(SystemConfig.Nodes);
			OutputControllers.AddRange(SystemConfig.OutputControllers.Cast<OutputController>());
			//SmartOutputControllers.AddRange(SystemConfig.SmartOutputControllers.Cast<SmartOutputController>());
			Previews.AddRange(SystemConfig.Previews.Cast<OutputPreview>());
			Filters.AddRange(SystemConfig.Filters);

			DataFlow.Initialize(SystemConfig.DataFlow);
		}

		public static void ReloadSystemConfig()
		{
			bool wasRunning = Execution.IsOpen;
			Execution.CloseExecution();
			
			LoadSystemConfig();

			if (wasRunning)
				Execution.OpenExecution();
		}


		public static RunState State
		{
			get { return _state; }
		}

		public static string AssemblyFileName
		{
			get { return Assembly.GetExecutingAssembly().GetFilePath(); }
		}

	

		public static bool AllowFilterEvaluation
		{
			get { return SystemConfig.AllowFilterEvaluation; }
			set { SystemConfig.AllowFilterEvaluation = value; }
		}

		public static int DefaultUpdateInterval
		{
			get { 
				// turns out we might need the default before we have SystemConfig set up...
				return SystemConfig == null
					? SystemConfig.DEFAULT_UPDATE_INTERVAL
					: SystemConfig.DefaultUpdateInterval; 
			}
			set
			{
				SystemConfig.DefaultUpdateInterval = value;
				DefaultUpdateTimeSpan = TimeSpan.FromMilliseconds(value);
			}
		}

		public static TimeSpan DefaultUpdateTimeSpan { get; private set; }

		public static ElementManager Elements { get; private set; }
		public static NodeManager Nodes { get; private set; }
		public static OutputControllerManager OutputControllers { get; private set; }
		//public static SmartOutputControllerManager SmartOutputControllers { get; private set; }
		public static PreviewManager Previews { get; private set; }
		public static ContextManager Contexts { get; private set; }
		public static FilterManager Filters { get; private set; }
		public static IInstrumentation Instrumentation { get; private set; }
		public static DataFlowManager DataFlow { get; private set; }
		public static ControllerFacade ControllerManagement { get; private set; }
		public static OutputDeviceFacade OutputDeviceManagement { get; private set; }
		public static bool VersionBeyondWindowsXP {
			get {
				System.OperatingSystem osInfo = System.Environment.OSVersion;
				if (osInfo.Platform == PlatformID.Win32NT) {
					//If OsVersion is > XP then allow D2D
					if (osInfo.Version.Major > 5) return true;
				}

				//Otherwise force GDI Preview rendering
				return false;
			}
		}
		public static Guid Identity
		{
			get { return SystemConfig.Identity; }
		}

		internal static ModuleStore ModuleStore { get; private set; }
		internal static SystemConfig SystemConfig { get; private set; }

		private static ModuleStore _LoadModuleStore(string systemDataPath)
		{
			string moduleStoreFilePath = Path.Combine(systemDataPath, ModuleStore.FileName);
			return FileService.Instance.LoadModuleStoreFile(moduleStoreFilePath);
		}

		private static SystemConfig _LoadSystemConfig(string systemDataPath)
		{
			string systemConfigFilePath = Path.Combine(systemDataPath, SystemConfig.FileName);
			return FileService.Instance.LoadSystemConfigFile(systemConfigFilePath);
		}

	
		private static string _GetSystemDataPath()
		{
			// Look for a user data file in the binary directory.
			string filePath = Path.Combine(Paths.BinaryRootPath, SystemConfig.FileName);
			if (_OperatingWithinContext(filePath)) {
				// We're going to use the context's user data file and not the
				// one in the data branch.
				return Path.GetDirectoryName(filePath);
			}

			// Use the default path in the data branch.
			return SystemConfig.Directory;
		}

		private static bool _OperatingWithinContext(string systemConfigFilePath)
		{
			SystemConfig systemConfig = FileService.Instance.LoadSystemConfigFile(systemConfigFilePath);
			return systemConfig != null && systemConfig.IsContext;
		}

		private static string _GetUserDataPath()
		{
			string dataPath = System.Configuration.ConfigurationManager.AppSettings["DataPath"];
			return dataPath ?? Paths.DefaultDataRootPath;
		}

		public static string ProfileName { get; set; }
	}
}