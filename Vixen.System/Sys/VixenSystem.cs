using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Vixen.Common;
using System.IO;
using Vixen.Module;
using Vixen.Module.Sequence;
using Vixen.Execution;
using Vixen.Hardware;

namespace Vixen.Sys {
    public class VixenSystem {
		static private Logging _logging;

		private enum RunState { Stopped, Starting, Started, Stopping };
		static private RunState _state = RunState.Stopped;

        static public void Start(IApplication clientApplication, bool openExecution = true) {
			if(_state == RunState.Stopped) {
				try {
					_state = RunState.Starting;
					ApplicationServices.ClientApplication = clientApplication;

					_InitializeLogging();

					ModuleImplementation[] moduleImplementations = Modules.GetImplementations();

					// Build branches for each module type.
					foreach(ModuleImplementation moduleImplementation in moduleImplementations) {
						Helper.EnsureDirectory(Path.Combine(Modules.Directory, moduleImplementation.TypeOfModule));
					}

					// Load all modules.
					Modules.LoadModules();

					// Load user data
					UserData = new UserData();
					UserData.LoadModuleData();

					// Add modules to repositories.
					Modules.PopulateRepositories();

					// Load the controllers before opening execution.
					// Creating the fixtures will create channels which will create patches
					// which will create sources for outputs that need to exist first.
					OutputController.ReloadAll();
					
					if(openExecution) {
						Vixen.Sys.Execution.OpenExecution();
					}

					_state = RunState.Started;
				} catch(Exception ex) {
					// The client is expected to have subscribed to the logging event
					// so that it knows that an exception occurred during loading.
					Logging.Error("System loading", ex);
					_state = RunState.Stopped;
				}
			}
        }

        static public void Stop() {
			if(_state == RunState.Started) {
				_state = RunState.Stopping;
				ApplicationServices.ClientApplication = null;
				Vixen.Sys.Execution.CloseExecution();
				Modules.ClearRepositories();
				VixenSystem.UserData.Save();
				_state = RunState.Stopped;
			}
        }

		static public ModuleDataSet ModuleData {
			get { return VixenSystem.UserData.ModuleData; }
		}

		static public string AssemblyFileName {
			get { return Assembly.GetExecutingAssembly().Location; }
		}

		static internal UserData UserData { get; private set; }

		static public dynamic Logging {
			get { return _logging; }
		}

		static private void _InitializeLogging() {
			_logging = new Logging();
			_logging.AddLog(new ErrorLog());
			_logging.AddLog(new WarningLog());
			_logging.AddLog(new InfoLog());
			_logging.AddLog(new DebugLog());
		}
	}
}
