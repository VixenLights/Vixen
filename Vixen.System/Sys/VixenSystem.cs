using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Sequence;
using Vixen.Execution;
using Vixen.Hardware;
using Vixen.IO;
using Vixen.IO.Xml;

namespace Vixen.Sys {
    public class VixenSystem {
		private const string USER_DATA_FILE = "UserData.xml";
		private const string ELEMENT_DATA_DIRECTORY = "DataDirectory";

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
					string userDataFilePath = _GetUserDataFilePath();
					IReader reader = new XmlUserDataReader();
					UserData = (UserData)reader.Read(userDataFilePath);

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

		static private string _GetUserDataFilePath() {
			// Look for a user data file in the binary directory.
			string filePath = Path.Combine(Paths.BinaryRootPath, USER_DATA_FILE);
			XElement element = Helper.LoadXml(filePath);
			if(element != null) {
				XElement dataDirectory = element.Element(ELEMENT_DATA_DIRECTORY);
				if(dataDirectory != null) {
					if(Directory.Exists(dataDirectory.Value)) {
						// We have an alternate path and it does exist.
						Paths.DataRootPath = dataDirectory.Value;
					}
				}
			}

			// Setting DataRootPath is the way to get the data branch built, but we
			// don't want to do that until we have certainly determined the location
			// of their data branch.  Otherwise we would create multiple branches and
			// they will have an empty branch that is likely to raise questions.
			// SO...we may or may not have already set it above, so we are going to
			// set it to ensure the data branch exists.
			Paths.DataRootPath = Paths.DataRootPath;

			// Reset the expected location of the user data file.
			return Path.Combine(Paths.DataRootPath, USER_DATA_FILE);
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

		static public IModuleDataSet ModuleData {
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
