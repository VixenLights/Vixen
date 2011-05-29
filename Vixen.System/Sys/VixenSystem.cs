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
		static private Dictionary<string, ModuleImplementation> _moduleTypes = new Dictionary<string, ModuleImplementation>();
		static private Logging _logging;

		private enum RunState { Stopped, Starting, Started, Stopping };
		static private RunState _state = RunState.Stopped;

		static private void _InitializeLogging() {
			_logging = new Logging();
			_logging.AddLog(new ErrorLog());
			_logging.AddLog(new WarningLog());
			_logging.AddLog(new InfoLog());
			_logging.AddLog(new DebugLog());
		}

		static private void _DiscoverModuleTypes() {
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

        static public void Start(IApplication clientApplication) {
			if(_state == RunState.Stopped) {
				try {
					_state = RunState.Starting;
					ApplicationServices.ClientApplication = clientApplication;

					_InitializeLogging();

					// Find all types of modules implemented.
					_DiscoverModuleTypes();

					ModuleImplementation[] moduleImplementations = Internal.GetModuleTypes().ToArray();
					// Do this after module types are discovered because the dynamics
					// will need to know the module types loaded.
					VixenSystem.ModuleManagement = new ModuleManagement(moduleImplementations);
					VixenSystem.ModuleRepository = new ModuleRepository(moduleImplementations);
					VixenSystem.ModuleLoadNotifier = new ModuleLoadNotifier(moduleImplementations);

					// Build branches for each module type.
					foreach(ModuleImplementation moduleImplementation in moduleImplementations) {
						Helper.EnsureDirectory(Path.Combine(Modules.Directory, moduleImplementation.ModuleTypeName));
					}

					var moduleTypes =
						from moduleType in Internal.GetModuleTypes()
						let typeName = (moduleType.GetType().GetCustomAttributes(typeof(ModuleTypeAttribute), false).First() as ModuleTypeAttribute).Name
						select new ModuleType(typeName, moduleType);
					var applicationModules = moduleTypes.Where(x => x.TypeName.StartsWith("App"));
					var otherModules = moduleTypes.Where(x => !x.TypeName.StartsWith("App"));

					// Order is important here:
					// 1. Load non-app modules, which may be referenced.
					// 2. Load user data, which references the modules (deserializing module data).
					//    and let the modules know they've been loaded, which may reference the
					//    user data (their module data).
					// 3. Load pre-loaded objects, which may reference data-initialized modules.
					// 4. Load app modules, which aren't likely referenced, but are likely to
					//    reference others.

					// Load non-app modules.
					_LoadModules(otherModules);

					// Load user data.
					VixenSystem.UserData = new UserData();

					// Load system-level non-module object instances after loading modules
					// because some instances, such as hardware controllers, make use
					// of modules.
					_LoadObjects();

					// With everything else loaded and ready, load the app modules.
					_LoadModules(applicationModules);

					// Do this only after all module types are loaded because it will
					// have to reference the data model types to deserialize them.
					VixenSystem.UserData.LoadModuleData();

					// With module data loaded, notify the modules that they've been loaded.
					_NotifyModulesLoaded(otherModules);
					_NotifyModulesLoaded(applicationModules);

					Vixen.Sys.Execution.OpenExecution();
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
				_UnloadModules();
				VixenSystem.UserData.Save();
				_state = RunState.Stopped;
			}
        }

		static private void _LoadModules(IEnumerable<ModuleType> moduleTypes) {
			foreach(ModuleType moduleType in moduleTypes) {
				foreach(string filePath in Directory.GetFiles(Path.Combine(Modules.Directory, moduleType.TypeName), "*.dll")) {
					try {
						// Load the modules of the type.
						Modules.LoadModules(filePath, moduleType.TypeName);
					} catch(BadImageFormatException) { }
				}
			}
		}

		static private void _UnloadModules() {
			foreach(ModuleImplementation moduleImplementation in Internal.GetModuleTypes()) {
				foreach(IModuleDescriptor moduleDescriptor in Modules.GetModuleDescriptors(moduleImplementation.ModuleTypeName)) {
					moduleImplementation.ModuleTypeLoader.ModuleUnloading(moduleDescriptor);
					//*** actually unload the module
					//-> Or is that done elsewhere?
				}
			}
		}

		static private void _NotifyModulesLoaded(IEnumerable<ModuleType> moduleTypes) {
			foreach(ModuleType moduleType in moduleTypes) {
				// Notify the types that they've been loaded.
				foreach(IModuleDescriptor moduleDescriptor in Modules.GetModuleDescriptors(moduleType.Implementation.ModuleTypeName)) {
					moduleType.Implementation.ModuleTypeLoader.ModuleLoaded(moduleDescriptor);
				}
			}
		}

		static public ModuleDataSet ModuleData {
			get { return VixenSystem.UserData.ModuleData; }
		}

		static public string AssemblyFileName {
			get { return Assembly.GetExecutingAssembly().Location; }
		}

		static public dynamic ModuleManagement { get; private set; }
		static internal dynamic ModuleRepository { get; private set; }
		static internal dynamic ModuleLoadNotifier { get; private set; }

		static internal UserData UserData { get; private set; }

		static public dynamic Logging {
			get { return _logging; }
		}

		class ModuleType {
			public ModuleType(string typeName, ModuleImplementation implementation) {
				TypeName = typeName;
				Implementation = implementation;
			}

			public string TypeName;
			public ModuleImplementation Implementation;
		}

		#region Vixen.Sys.VixenSystem.Internal
		static internal class Internal {
			static public IEnumerable<ModuleImplementation> GetModuleTypes() {
				return _moduleTypes.Values;
			}

			static public ModuleImplementation GetModuleType(string moduleType) {
				ModuleImplementation implementation = null;
				_moduleTypes.TryGetValue(moduleType, out implementation);
				return implementation;
			}

			static public U GetModuleManager<T, U>()
				where T : class, IModuleInstance
				where U : class, IModuleManagement<T> {
				return _moduleTypes.Values.FirstOrDefault(x => x.ModuleInstanceType == typeof(T)).Management as U;
			}

			static public IModuleManagement GetModuleManager<T>()
				where T : class, IModuleInstance {
				return _moduleTypes.Values.FirstOrDefault(x => x.ModuleInstanceType == typeof(T)).Management;
			}

			static public U GetModuleRepository<T, U>()
				where T : class, IModuleInstance
				where U : class, IModuleRepository<T> {
				return _moduleTypes.Values.FirstOrDefault(x => x.ModuleInstanceType == typeof(T)).Repository as U;
			}

			static public IModuleRepository GetModuleRepository<T>()
				where T : class, IModuleInstance {
				return _moduleTypes.Values.FirstOrDefault(x => x.ModuleInstanceType == typeof(T)).Repository;
			}
		}
		#endregion
	}
}
