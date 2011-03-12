using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Vixen.Common;
using System.IO;
using Vixen.Module;

namespace Vixen.Sys {
    public class Server : IDisposable {
        static private Server _instance = null;

		private Dictionary<string, ModuleImplementation> _moduleTypes = new Dictionary<string, ModuleImplementation>();
		private Logging _logging;

        private Server(IApplication clientApplication) {
			ApplicationServices.ClientApplication = clientApplication;

			_logging = new Logging();
			_logging.AddLog(new ErrorLog());
			_logging.AddLog(new WarningLog());
			_logging.AddLog(new InfoLog());
			_logging.AddLog(new DebugLog());

			// Find all types of modules implemented.
			_DiscoverModuleTypes();

			// ** Everything else was moved out of this constructor because the rest of the
			// ** initialization will be making use of the variable that references this
			// ** instance being created, so it needs to be set.
		}

		private void _DiscoverModuleTypes() {
			string moduleTypeName;
			foreach(Type type in Assembly.GetExecutingAssembly().GetAttributedTypes(typeof(ModuleTypeAttribute))) {
				moduleTypeName = (type.GetCustomAttributes(typeof(ModuleTypeAttribute), false).First() as ModuleTypeAttribute).Name;
				_moduleTypes[moduleTypeName] = Activator.CreateInstance(type) as ModuleImplementation;
			}
		}

        static private void _LoadObjects() {
            Type attributeType = typeof(PreloadAttribute);

            // Iterate types in the system assembly.
            foreach(Type type in Assembly.GetExecutingAssembly().GetTypes()) {
                // Iterate static members of the type, public and private.
                // They must be static because we won't have an instance to pull values from.
                foreach(MethodInfo mi in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)) {
                    // Does the type have a ObjectPreload attribute?
                    if(mi.GetCustomAttributes(attributeType, false).Length != 0) {
                        // Invoke the method.
                        mi.Invoke(null, null);
                    }
                }
            }
        }

        ~Server() {
            Dispose();
        }

        public void Dispose() {
			_NotifyModulesUnloading();
            GC.SuppressFinalize(this);
        }

        static public void Start(IApplication clientApplication) {
            if(_instance == null) {
				try {
					_instance = new Server(clientApplication);

					ModuleImplementation[] moduleImplementations = Internal.GetModuleTypes().ToArray();
					// Do this after module types are discovered because the dynamics
					// will need to know the module types loaded.
					Server.ModuleManagement = new ModuleManagement(moduleImplementations);
					Server.ModuleRepository = new ModuleRepository(moduleImplementations);
					Server.ModuleLoadNotifier = new ModuleLoadNotifier(moduleImplementations);

					// Build branches for each module type.
					foreach(ModuleImplementation moduleImplementation in moduleImplementations) {
						Helper.EnsureDirectory(Path.Combine(Modules.Directory, moduleImplementation.ModuleTypeName));
					}

					// Order is important here:
					// 1. Load modules.
					// 2. Load user data, which references the modules (deserializing module data).
					// 3. Let the modules know they've been loaded, which may reference the user data (their module data).
					// 4. Load pre-loaded object, which may reference data-initialized modules.

					// Load all modules, regardless of type.
					_LoadModules();

					// Load user data.
					Server.UserData = new UserData();

					// Module managers can now be notified of each loaded instance.
					_NotifyModulesLoaded();

					// Load system-level non-module object instances after loading modules
					// because some instances, such as hardware controllers, make use
					// of modules.
					_LoadObjects();
				} catch(Exception ex) {
					// The client is expected to have subscribed to the logging event
					// so that it knows that an exception occurred during loading.
					Logging.Error(ex);
				}
            }
        }

        static public void Stop() {
            if(_instance != null) {
                _instance.Dispose();
                _instance = null;
				Server.UserData.Save();
			}
        }

		static private void _LoadModules() {
			var moduleTypes = 
				from moduleType in Internal.GetModuleTypes()
				let typeName = (moduleType.GetType().GetCustomAttributes(typeof(ModuleTypeAttribute), false).First() as ModuleTypeAttribute).Name
				select new { Name = typeName, Implementation = moduleType };
			var applicationModules = moduleTypes.Where(x => x.Name.StartsWith("App"));
			var otherModules = moduleTypes.Except(applicationModules);
			// Load module types other than application modules first.  Application modules
			// are likely to reference other module types while other module types are not
			// likely to reference them.
			foreach(var moduleImplementation in otherModules) {
				foreach(string filePath in Directory.GetFiles(Path.Combine(Modules.Directory, moduleImplementation.Name), "*.dll")) {
					try {
						Modules.LoadModules(filePath, moduleImplementation.Name);
					} catch(BadImageFormatException) { }
				}
			}
			foreach(var moduleImplementation in applicationModules) {
				foreach(string filePath in Directory.GetFiles(Path.Combine(Modules.Directory, moduleImplementation.Name), "*.dll")) {
					try {
						Modules.LoadModules(filePath, moduleImplementation.Name);
					} catch(BadImageFormatException) { }
				}
			}
		}

		static private void _NotifyModulesLoaded() {
			foreach(ModuleImplementation moduleImplementation in Internal.GetModuleTypes()) {
				foreach(IModuleDescriptor moduleDescriptor in Modules.GetModuleDescriptors(moduleImplementation.ModuleTypeName)) {
					moduleImplementation.LoadNotifier.ModuleLoaded(moduleDescriptor);
				}
			}
		}

		static private void _NotifyModulesUnloading() {
			foreach(ModuleImplementation moduleImplementation in Internal.GetModuleTypes()) {
				foreach(IModuleDescriptor moduleDescriptor in Modules.GetModuleDescriptors(moduleImplementation.ModuleTypeName)) {
					moduleImplementation.LoadNotifier.ModuleUnloading(moduleDescriptor);
				}
			}
		}

		static public ModuleDataSet ModuleData {
			get { return Server.UserData.ModuleData; }
		}

		static public string AssemblyFileName {
			get { return Assembly.GetExecutingAssembly().Location; }
		}

		static public dynamic ModuleManagement { get; private set; }
		static internal dynamic ModuleRepository { get; private set; }
		static internal dynamic ModuleLoadNotifier { get; private set; }

		static private UserData UserData { get; set; }

		static public dynamic Logging {
			get { return _instance._logging; }
		}

		static internal class Internal {
			static public IEnumerable<ModuleImplementation> GetModuleTypes() {
				return _instance._moduleTypes.Values;
			}

			static public ModuleImplementation GetModuleType(string moduleType) {
				ModuleImplementation implementation = null;
				_instance._moduleTypes.TryGetValue(moduleType, out implementation);
				return implementation;
			}

			static public U GetModuleManager<T, U>()
				where T : class, IModuleInstance
				where U : class, IModuleManagement<T> {
				return _instance._moduleTypes.Values.FirstOrDefault(x => x.ModuleInstanceType == typeof(T)).Management as U;
			}

			static public IModuleManagement GetModuleManager<T>()
				where T : class, IModuleInstance {
					return _instance._moduleTypes.Values.FirstOrDefault(x => x.ModuleInstanceType == typeof(T)).Management;
			}

			static public U GetModuleRepository<T, U>()
				where T : class, IModuleInstance
				where U : class, IModuleRepository<T> {
				return _instance._moduleTypes.Values.FirstOrDefault(x => x.ModuleInstanceType == typeof(T)).Repository as U;
			}

			static public IModuleRepository GetModuleRepository<T>()
				where T : class, IModuleInstance {
				return _instance._moduleTypes.Values.FirstOrDefault(x => x.ModuleInstanceType == typeof(T)).Repository;
			}
		}
	}
}
