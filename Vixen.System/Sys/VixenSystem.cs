using System;
using System.Linq;
using System.Reflection;
using System.IO;
using Vixen.Module;
using Vixen.Instrumentation;
using Vixen.Services;
using Vixen.Sys.Managers;
using Vixen.Sys.Output;

namespace Vixen.Sys {
    public class VixenSystem {
		static private Logging _logging;

		public enum RunState { Stopped, Starting, Started, Stopping };
		static private RunState _state = RunState.Stopped;

    	static public void Start(IApplication clientApplication, bool openExecution = true, bool disableDevices = false) {
			if(_state == RunState.Stopped) {
				try {
					_state = RunState.Starting;
					ApplicationServices.ClientApplication = clientApplication;

					// A user data file in the binary branch will give any alternate
					// data branch to use.
					Paths.DataRootPath = _GetUserDataPath();

					_InitializeLogging();
					Logging.Info("Vixen System starting up...");

					Instrumentation = new Instrumentation.Instrumentation();

					ModuleImplementation[] moduleImplementations = Modules.GetImplementations();

					// Build branches for each module type.
					foreach(ModuleImplementation moduleImplementation in moduleImplementations) {
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

					// Load all module descriptors.
					Modules.LoadAllModules();

					LoadSystemConfig();

					// Add modules to repositories.
					Modules.PopulateRepositories();

					if(disableDevices) {
						SystemConfig.DisabledDevices = OutputDeviceManagement.Devices;
					}
					if(openExecution) {
						Execution.OpenExecution();
					}

					_state = RunState.Started;
					Logging.Info("Vixen System successfully started.");
				} catch(Exception ex) {
					// The client is expected to have subscribed to the logging event
					// so that it knows that an exception occurred during loading.
					Logging.Error("Error during system startup; the system has been stopped", ex);
					Stop();
				}
			}
        }

        static public void Stop() {
			if(_state == RunState.Starting || _state == RunState.Started) {
				_state = RunState.Stopping;
				Logging.Info("Vixen System stopping...");
				ApplicationServices.ClientApplication = null;
				// Need to get the disabled devices before stopping them all.
				SystemConfig.DisabledDevices = OutputDeviceManagement.Devices.Where(x => (x != null) && !x.IsRunning);
				Execution.CloseExecution();
				Modules.ClearRepositories();
				if(ModuleStore != null) {
					ModuleStore.Save();
				}
				SaveSystemConfig();
				_state = RunState.Stopped;
				Logging.Info("Vixen System successfully stopped.");
			}
        }

		static public void SaveSystemConfig()
		{
			if (SystemConfig != null) {
				// 'copy' the current details (nodes/channels/controllers) from the executing state
				// to the SystemConfig, so they're there for writing when we save
				SystemConfig.OutputControllers = OutputControllers;
				SystemConfig.SmartOutputControllers = SmartOutputControllers;
				SystemConfig.Previews = Previews;
				SystemConfig.Channels = Channels;
				SystemConfig.Nodes = Nodes.GetRootNodes();
				SystemConfig.ControllerLinking = ControllerLinking;
				SystemConfig.Filters = Filters;
				SystemConfig.DataFlow = DataFlow;
				SystemConfig.Save();
			}
		}

		static public void LoadSystemConfig()
		{
			DataFlow = new DataFlowManager();
			Channels = new ChannelManager();
			Nodes = new NodeManager();
			OutputControllers = new OutputControllerManager(
				new ControllerLinkingManagement<OutputController>(),
				new OutputDeviceCollection<OutputController>(),
				new OutputDeviceExecution<OutputController>());
			SmartOutputControllers = new SmartOutputControllerManager(
				new ControllerLinkingManagement<SmartOutputController>(),
				new OutputDeviceCollection<SmartOutputController>(),
				new OutputDeviceExecution<SmartOutputController>());
			Previews = new PreviewManager(
				new OutputDeviceCollection<OutputPreview>(),
				new OutputDeviceExecution<OutputPreview>());
			Contexts = new ContextManager();
			Filters = new FilterManager(DataFlow);
			ControllerLinking = new ControllerLinker();
			ControllerManagement = new ControllerFacade();
			ControllerManagement.AddParticipant(OutputControllers);
			ControllerManagement.AddParticipant(SmartOutputControllers);
			OutputDeviceManagement = new OutputDeviceFacade();
			OutputDeviceManagement.AddParticipant(OutputControllers);
			OutputDeviceManagement.AddParticipant(SmartOutputControllers);
			OutputDeviceManagement.AddParticipant(Previews);

			// Load system data in order of dependency.
			// The system data generally resides in the data branch, but it
			// may not be in the case of an alternate context.
			string systemDataPath = _GetSystemDataPath();
			// Load module data before system config.
			// System config creates objects that use modules that have data in the store.
			ModuleStore = _LoadModuleStore(systemDataPath) ?? new ModuleStore();
			SystemConfig = _LoadSystemConfig(systemDataPath) ?? new SystemConfig();

			Channels.AddChannels(SystemConfig.Channels);
			Nodes.AddNodes(SystemConfig.Nodes);
			OutputControllers.AddRange(SystemConfig.OutputControllers.Cast<OutputController>());
			SmartOutputControllers.AddRange(SystemConfig.SmartOutputControllers.Cast<SmartOutputController>());
			Previews.AddRange(SystemConfig.Previews.Cast<OutputPreview>());
			ControllerLinking.AddRange(SystemConfig.ControllerLinking);
			Filters.AddRange(SystemConfig.Filters);
			
			DataFlow.Initialize(SystemConfig.DataFlow);
		}

    	static public void ReloadSystemConfig()
		{
			bool wasRunning = Execution.IsOpen;
			Execution.CloseExecution();

			// purge all existing channels, nodes, and controllers (to try and clean up a bit).
			// might not actually matter, since we're going to make new Managers for them all
			// in a tick, but better safe than sorry.
			foreach (ChannelNode cn in Nodes.ToArray())
				Nodes.RemoveNode(cn, null, true);
			foreach (OutputController oc in OutputControllers.ToArray())
				OutputControllers.Remove(oc);
    		foreach(SmartOutputController smartOutputController in SmartOutputControllers.ToArray()) {
				SmartOutputControllers.Remove(smartOutputController);
    		}
			foreach (OutputPreview outputPreview in Previews.ToArray())
				Previews.Remove(outputPreview);

			LoadSystemConfig();

			if (wasRunning)
				Execution.OpenExecution();
		}


		static public RunState State {
			get { return _state; }
		}

		static public string AssemblyFileName {
			get { return Assembly.GetExecutingAssembly().GetFilePath(); }
		}

		static public dynamic Logging {
			get { return _logging; }
		}

		static public Logging Logs {
			get { return _logging; }
		}

    	public static bool AllowFilterEvaluation {
    		get { return SystemConfig.AllowFilterEvaluation; }
    		set { SystemConfig.AllowFilterEvaluation = value; }
    	}

    	static public ChannelManager Channels { get; private set; }
		static public NodeManager Nodes { get; private set; }
		static public OutputControllerManager OutputControllers { get; private set; }
		static public SmartOutputControllerManager SmartOutputControllers { get; private set; }
    	static public PreviewManager Previews { get; private set; }
		static public ContextManager Contexts { get; private set; }
		static public FilterManager Filters { get; private set; }
    	static public IInstrumentation Instrumentation { get; private set; }
		static public ControllerLinker ControllerLinking { get; private set; }
		static public DataFlowManager DataFlow { get; private set; }
		static public ControllerFacade ControllerManagement { get; private set; }
		static public OutputDeviceFacade OutputDeviceManagement { get; private set; }

    	public static Guid Identity {
    		get { return SystemConfig.Identity; }
    	}

    	static internal ModuleStore ModuleStore { get; private set; }
		static internal SystemConfig SystemConfig { get; private set; }

		private static ModuleStore _LoadModuleStore(string systemDataPath) {
			string moduleStoreFilePath = Path.Combine(systemDataPath, ModuleStore.FileName);
			return FileService.Instance.LoadModuleStoreFile(moduleStoreFilePath);
		}

		private static SystemConfig _LoadSystemConfig(string systemDataPath) {
			string systemConfigFilePath = Path.Combine(systemDataPath, SystemConfig.FileName);
			return FileService.Instance.LoadSystemConfigFile(systemConfigFilePath);
		}

		static private void _InitializeLogging() {
			_logging = new Logging();
			_logging.AddLog(new ErrorLog());
			_logging.AddLog(new WarningLog());
			_logging.AddLog(new InfoLog());
			_logging.AddLog(new DebugLog());
		}

		static private string _GetSystemDataPath() {
			// Look for a user data file in the binary directory.
			string filePath = Path.Combine(Paths.BinaryRootPath, SystemConfig.FileName);
			if(_OperatingWithinContext(filePath)) {
				// We're going to use the context's user data file and not the
				// one in the data branch.
				return Path.GetDirectoryName(filePath);
			}

			// Use the default path in the data branch.
			return SystemConfig.Directory;
		}

		static private bool _OperatingWithinContext(string systemConfigFilePath) {
			SystemConfig systemConfig = FileService.Instance.LoadSystemConfigFile(systemConfigFilePath);
			return systemConfig != null && systemConfig.IsContext;
		}

    	static private string _GetUserDataPath() {
			// Look for a user data file in the binary directory.
			string filePath = Path.Combine(Paths.BinaryRootPath, SystemConfig.FileName);
			SystemConfig systemConfig = FileService.Instance.LoadSystemConfigFile(filePath);
			if(systemConfig != null && Directory.Exists(systemConfig.AlternateDataPath)) return systemConfig.AlternateDataPath;
			
			// Use the default data path.
			return Paths.DefaultDataRootPath;
		}
	}
}
