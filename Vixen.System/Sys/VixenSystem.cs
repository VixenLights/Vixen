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
		private const string ATTRIBUTE_IS_CONTEXT = "isContext";

		static private Logging _logging;

		public enum RunState { Stopped, Starting, Started, Stopping };
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

					// The user data file generally resides in the data branch, but it
					// may not be in the case of an alternate context.
					string userDataFilePath = _GetUserDataFilePath();
					// A user data file in the binary branch will give any alternate
					// data branch to use.
					Paths.DataRootPath = _GetUserDataPath();
					// Load user data.
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
				} catch(ReflectionTypeLoadException ex) {
					Logging.Debug("Loader exceptions:" + Environment.NewLine + ex.LoaderExceptions.Select(x => x.Message + Environment.NewLine) + Environment.NewLine + "The system has been stopped.");
					Stop();
				} catch(Exception ex) {
					// The client is expected to have subscribed to the logging event
					// so that it knows that an exception occurred during loading.
					Logging.Debug("Error during system startup; the system has been stopped", ex);
					Stop();
				}
			}
        }

        static public void Stop() {
			if(_state == RunState.Starting || _state == RunState.Started) {
				_state = RunState.Stopping;
				ApplicationServices.ClientApplication = null;
				Vixen.Sys.Execution.CloseExecution();
				Modules.ClearRepositories();
				if(UserData != null) {
					UserData.Save();
				}
				_state = RunState.Stopped;
			}
        }

		static public RunState State {
			get { return _state; }
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

		static private string _GetUserDataFilePath() {
			// Look for a user data file in the binary directory.
			string filePath = Path.Combine(Paths.BinaryRootPath, USER_DATA_FILE);
			XElement element = Helper.LoadXml(filePath);
			if(element != null) {
				// Are we operating within a context?
				if(element.Attribute(ATTRIBUTE_IS_CONTEXT) != null) {
					// We're going to use the context's user data file and not the
					// one in the data branch.
					return filePath;
				}
			}

			// Use the default path in the data branch.
			return Path.Combine(Paths.DataRootPath, USER_DATA_FILE);
		}

		static private string _GetUserDataPath() {
			// Look for a user data file in the binary directory.
			string filePath = Path.Combine(Paths.BinaryRootPath, USER_DATA_FILE);
			XElement element = Helper.LoadXml(filePath);
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
