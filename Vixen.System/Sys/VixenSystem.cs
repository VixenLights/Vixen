using System;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Xml.Linq;
using Vixen.Module;
using Vixen.IO;
using Vixen.IO.Xml;
using Vixen.Instrumentation;
//using Vixen.Module.PostFilter;
using Vixen.Module.PostFilter;
using Vixen.Sys.Managers;
using Vixen.Sys.Output;

namespace Vixen.Sys {
    public class VixenSystem {
		private const string ELEMENT_DATA_DIRECTORY = "DataDirectory";
		private const string ATTRIBUTE_IS_CONTEXT = "isContext";

		static private Logging _logging;

		public enum RunState { Stopped, Starting, Started, Stopping };
		static private RunState _state = RunState.Stopped;

        static public void Start(IApplication clientApplication, bool openExecution = true, bool disableControllers = false) {
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

					if(disableControllers) {
						SystemConfig.DisabledControllers = Controllers;
					}
					if(openExecution) {
						Execution.OpenExecution();
					}

					//****
					_AddPostFiltersToControllers();
					//****

					_state = RunState.Started;
					Logging.Info("Vixen System successfully started.");
				} catch(Exception ex) {
					// The client is expected to have subscribed to the logging event
					// so that it knows that an exception occurred during loading.
					Logging.Error("Error during system startup; the system has been stopped");
					Logging.Debug(ex);
					Stop();
				}
			}
        }

		private static void _AddPostFiltersToControllers() {
			Guid grayscaleFilterId = new Guid("{DAC271B0-0743-45ef-B4E0-D5957AF7F019}");
			Guid colorFilterId = new Guid("{B3C06A83-CE75-4e78-853D-B95B4E69CEAC}");

			foreach(OutputController outputController in Controllers) {
				for(int outputIndex = 0; outputIndex < outputController.OutputCount; outputIndex++) {
					IPostFilterModuleInstance postFilter = Modules.ModuleManagement.GetPostFilter(colorFilterId);
					if(postFilter != null) {
						outputController.AddPostFilter(outputIndex, postFilter);
					}
				}
			}
		}

        static public void Stop() {
			if(_state == RunState.Starting || _state == RunState.Started) {
				_state = RunState.Stopping;
				Logging.Info("Vixen System stopping...");
				ApplicationServices.ClientApplication = null;
				// Need to get the disabled controllers before stopping them all.
				SystemConfig.DisabledControllers = Controllers.Where(x => !x.IsRunning);
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
				SystemConfig.Controllers = Controllers;
				SystemConfig.OtherOutputDevices = OtherOutputDevices;
				SystemConfig.Channels = Channels;
				SystemConfig.Nodes = Nodes.GetRootNodes();
				SystemConfig.ChannelPatching = ChannelPatching;
				SystemConfig.ControllerLinking = ControllerLinking;
				SystemConfig.Save();
			}
		}

		static public void LoadSystemConfig()
		{
			Channels = new ChannelManager();
			Nodes = new NodeManager();
			Controllers = new ControllerManager();
			OtherOutputDevices = new OutputDeviceManager();
			Contexts = new ContextManager();
			ChannelPatching = new ChannelOutputPatchManager();
			ControllerLinking = new ControllerLinker();

			// Load system data in order of dependency.
			// The system data generally resides in the data branch, but it
			// may not be in the case of an alternate context.
			string systemDataPath = _GetSystemDataPath();
			IReader reader = new XmlModuleStoreReader();
			ModuleStore = (ModuleStore)reader.Read(Path.Combine(systemDataPath, ModuleStore.FileName));
			reader = new XmlSystemConfigReader();
			SystemConfig = (SystemConfig)reader.Read(Path.Combine(systemDataPath, SystemConfig.FileName));

			if(SystemConfig == null)
				SystemConfig = new SystemConfig();

			if(ModuleStore == null)
				ModuleStore = new ModuleStore();

			Channels.AddChannels(SystemConfig.Channels);
			Nodes.AddNodes(SystemConfig.Nodes);
			Controllers.AddRange(SystemConfig.Controllers);
			OtherOutputDevices.AddRange(SystemConfig.OtherOutputDevices);
			ChannelPatching.AddPatches(SystemConfig.ChannelPatching);
			ControllerLinking.AddRange(SystemConfig.ControllerLinking);

			//Channel channel = Channels.First();
			//ChannelNode node = Nodes.FirstOrDefault(x => x.Channel == channel);
			//ChannelNode node2 = Nodes.FirstOrDefault(x => x.Channel.Id == channel.Id);
			//ChannelNode ref1 = node;
			//ChannelNode ref2 = node2;

			//****
			//_AddChannelPostFilter();
			//****
		}

		//static private void _AddChannelPostFilter() {
		//    // Add the Color post-filter to the channels of the first 5 nodes of the 1024 channel batch.
		//    Channel[] firstFiveChannels = Nodes.Skip(64).Take(5).Select(x => x.Channel).ToArray();
		//    for(int i = 0; i < 5; i++) {
		//        IPostFilterModuleInstance postFilter = Modules.ModuleManagement.GetPostFilter(new Guid("{B3C06A83-CE75-4e78-853D-B95B4E69CEAC}"));
		//        if(postFilter != null) {
		//            firstFiveChannels[i].AddPostFilter(postFilter);
		//        }
		//    }
		//}

    	static public void ReloadSystemConfig()
		{
			bool wasRunning = Execution.IsOpen;
			Execution.CloseExecution();

			// purge all existing channels, nodes, and controllers (to try and clean up a bit).
			// might not actually matter, since we're going to make new Managers for them all
			// in a tick, but better safe than sorry.
			foreach (Channel c in Channels.ToArray())
				Channels.RemoveChannel(c);
			foreach (ChannelNode cn in Nodes.ToArray())
				Nodes.RemoveNode(cn, null, true);
			foreach (OutputController oc in Controllers.ToArray())
				Controllers.Remove(oc);
			foreach (IOutputDevice outputDevice in OtherOutputDevices.ToArray())
				OtherOutputDevices.Remove(outputDevice);

			LoadSystemConfig();

			if (wasRunning)
				Execution.OpenExecution();
		}


		static public RunState State {
			get { return _state; }
		}

		static public string AssemblyFileName {
			get { return Assembly.GetExecutingAssembly().Location; }
		}

		static public dynamic Logging {
			get { return _logging; }
		}

		static public Logging Logs {
			get { return _logging; }
		}

		static public ChannelManager Channels { get; private set; }
		static public NodeManager Nodes { get; private set; }
		static public ControllerManager Controllers { get; private set; }
		static public OutputDeviceManager OtherOutputDevices { get; private set; }
		static public ContextManager Contexts { get; private set; }
    	static public IInstrumentation Instrumentation { get; private set; }
		static public ChannelOutputPatchManager ChannelPatching { get; private set; }
		static public ControllerLinker ControllerLinking { get; private set; }

    	static internal ModuleStore ModuleStore { get; private set; }
		static internal SystemConfig SystemConfig { get; private set; }

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
			XElement element = Helper.LoadXml(filePath);
			if(element != null) {
				// Are we operating within a context?
				if(element.Attribute(ATTRIBUTE_IS_CONTEXT) != null) {
					// We're going to use the context's user data file and not the
					// one in the data branch.
					return Path.GetDirectoryName(filePath);
				}
			}

			// Use the default path in the data branch.
			return SystemConfig.Directory;
		}

		static private string _GetUserDataPath() {
			// Look for a user data file in the binary directory.
			XElement element = Helper.LoadXml(SystemConfig.DefaultFilePath);
			if(element != null) {
				// Does it specify an alternate data path?
				XElement dataDirectory = element.Element(ELEMENT_DATA_DIRECTORY);
				if(dataDirectory != null) {
					if(Directory.Exists(dataDirectory.Value)) {
						// We have an alternate path and it does exist.
						return dataDirectory.Value;
					}
				}
			}

			// Use the default path.
			return Paths.DataRootPath;
		}
	}
}
