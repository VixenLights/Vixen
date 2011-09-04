using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.ModuleTemplate;

namespace Vixen.Sys {
	class Modules {
		// Module type id : IModuleInstance implementor
		static private Dictionary<Guid, Type> _activators = new Dictionary<Guid, Type>();
		// Module type id : module descriptor
		static private Dictionary<Guid, IModuleDescriptor> _moduleDescriptors = new Dictionary<Guid, IModuleDescriptor>();
		static private Dictionary<ModuleImplementation, List<IModuleDescriptor>> _moduleImplementationDescriptors = new Dictionary<ModuleImplementation, List<IModuleDescriptor>>();
		static public dynamic ModuleManagement { get; private set; }
		static public dynamic ModuleRepository { get; private set; }

		static public readonly string Directory;

		static Modules() {
			Directory = Path.Combine(Paths.BinaryRootPath, "Modules");

			_moduleImplementationDescriptors = Assembly.GetExecutingAssembly().GetAttributedTypes(typeof(TypeOfModuleAttribute)).ToDictionary(
					x => Activator.CreateInstance(x) as ModuleImplementation,
					x => new List<IModuleDescriptor>());

			ModuleImplementation[] moduleImplementations = _moduleImplementationDescriptors.Keys.ToArray();
			ModuleManagement = new ModuleManagement(moduleImplementations);
			ModuleRepository = new ModuleRepository(moduleImplementations);
		}

		static public void LoadModules() {
			IEnumerable<IModuleDescriptor> descriptors;
			// For each type of module discovered, get any descriptors.
			// Get the results into a static collection or this will be called every time
			// "descriptors" is enumerated.
			descriptors = _moduleImplementationDescriptors.Keys.SelectMany(_LoadModuleDescriptors).ToArray();
			// Remove any that have duplicate type ids.
			descriptors = _RemoveDuplicateTypes(descriptors);
			// Check the dependencies of all modules.
			descriptors = _ResolveAgainstDependencies(descriptors);
			// Reconcile the lists of descriptors by type against what will actually be loaded.
			foreach(ModuleImplementation moduleImplementation in _moduleImplementationDescriptors.Keys) {
				List<IModuleDescriptor> moduleTypeDescriptors = _moduleImplementationDescriptors[moduleImplementation];
				// It appears that Except will return the *distinct* set.  Since we're wanting
				// to remove instances with duplicate identities, Except will not work for us.
				foreach(IModuleDescriptor removedDescriptor in moduleTypeDescriptors.Where(x => !descriptors.Contains(x)).ToArray()) {
					moduleTypeDescriptors.Remove(removedDescriptor);
				}
			}
			// The remaining modules are okay to load.
			_LoadModules(descriptors);
		}

		//static public void UnloadModule(Guid moduleTypeId, string moduleType) {
		//    // Remove it from all lookups at this level.
		//    _activators.Remove(moduleTypeId);
		//    _moduleDescriptors.Remove(moduleTypeId);
		//    _moduleImplementationDescriptors.Remove
		//    // Get the module type implementation and remove the module from the repository.
		//    VixenSystem.Internal.GetModuleType(moduleType).Repository.Remove(moduleTypeId);
		//}

		static private void _LoadModules(IEnumerable<IModuleDescriptor> descriptors) {
			foreach(IModuleDescriptor descriptor in descriptors) {
				// Add module data path, if any.
				Paths.AddDataPaths(descriptor.GetType(), typeof(ModuleDataPathAttribute));
				// Add descriptor to the descriptor dictionary.
				_moduleDescriptors[descriptor.TypeId] = descriptor;
				// Get a reference to the module implementation and add it to the activator dictionary.
				_activators[descriptor.TypeId] = descriptor.ModuleClass;
			}
		}

		static private IEnumerable<IModuleDescriptor> _RemoveDuplicateTypes(IEnumerable<IModuleDescriptor> descriptors) {
			IEnumerable<IGrouping<Guid, IModuleDescriptor>> duplicateTypes = descriptors.GroupBy(x => x.TypeId).Where(x => x.Count() > 1);
			if(duplicateTypes.Count() > 0) {
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("The following modules were not loaded because they have duplicate type ids:");
				foreach(var duplicateType in duplicateTypes) {
					sb.AppendLine("Type Id: " + duplicateType.Key);
					foreach(IModuleDescriptor descriptor in duplicateType) {
						sb.AppendLine("  - " + descriptor.TypeName);
					}
				}
				VixenSystem.Logging.Error(sb.ToString());
			}

			return descriptors.Except(duplicateTypes.SelectMany(x => x));
		}

		static private IEnumerable<IModuleDescriptor> _ResolveAgainstDependencies(IEnumerable<IModuleDescriptor> descriptors) {
			List<IModuleDescriptor> allDescriptors = descriptors.ToList();

			// Check the dependencies of all modules.
			// Keep checking until there are no missing dependencies.
			while(true) {
				// Need a static collection to enumerate.
				IModuleDescriptor[] remainingDescriptors = allDescriptors.ToArray();
				// Remove any descriptors that have missing dependencies.
				foreach(IModuleDescriptor descriptor in remainingDescriptors) {
					// All dependencies must be present in the collection of descriptors
					// for the module to be eligible for loading.
					if(descriptor.Dependencies != null && descriptor.Dependencies.Intersect(allDescriptors.Select(x => x.TypeId)).Count() != descriptor.Dependencies.Length) {
						allDescriptors.Remove(descriptor);
						VixenSystem.Logging.Error("Could not load module \"" + descriptor.TypeName + "\" because it has dependencies that are missing.");
					}
				}
				// If nothing was removed, we are done.
				if(allDescriptors.Count == remainingDescriptors.Length) break;
			};

			return allDescriptors;
		}

		static private string _GetModuleTypeDirectory(ModuleImplementation moduleImplementation) {
			string moduleTypeName = (moduleImplementation.GetType().GetCustomAttributes(typeof(TypeOfModuleAttribute), false).First() as TypeOfModuleAttribute).Name;
			string moduleTypeDirectory = Path.Combine(Directory, moduleTypeName);
			return moduleTypeDirectory;
		}

		static private IEnumerable<IModuleDescriptor> _LoadModuleDescriptors(ModuleImplementation moduleImplementation) {
			string moduleTypeDirectory = _GetModuleTypeDirectory(moduleImplementation);
			List<IModuleDescriptor> descriptors = new List<IModuleDescriptor>();

			// For all files in the module type's directory...
			foreach(string filePath in System.IO.Directory.GetFiles(moduleTypeDirectory, "*.dll")) {
				try {
					// Try to get descriptors.
					descriptors.AddRange(_LoadModuleDescriptors(filePath));
				} catch(BadImageFormatException) {
					VixenSystem.Logging.Warning("File " + filePath + " was not loaded due to BadImageFormatException.");
				}
			}

			// Add the module types to the module implementation's descriptor list.
			List<IModuleDescriptor> implementationDescriptors = _moduleImplementationDescriptors[moduleImplementation];
			implementationDescriptors.AddRange(descriptors);

			return descriptors;
		}

		static private IEnumerable<IModuleDescriptor> _LoadModuleDescriptors(string filePath) {
			List<IModuleDescriptor> descriptors = new List<IModuleDescriptor>();

			if(File.Exists(filePath)) {
				Assembly assembly = null;
				assembly = Assembly.LoadFrom(filePath);

				IModuleDescriptor moduleDescriptor = null;

				// Look for concrete module descriptors.
				IEnumerable<Type> moduleDescriptorTypes = typeof(IModuleDescriptor).FindImplementationsWithin(assembly).Where(x => !x.IsAbstract);
				foreach(Type moduleDescriptorType in moduleDescriptorTypes) {
					// Get the module descriptor.
					moduleDescriptor = Activator.CreateInstance(moduleDescriptorType) as IModuleDescriptor;

					// Make sure its module's type is an IModuleInstance
					if(moduleDescriptor.ModuleClass.ImplementsInterface(typeof(IModuleInstance))) {
						// Set the name of the file it was borne from.
						moduleDescriptor.FileName = Path.GetFileName(filePath);
						// Set the assembly it was borne from.
						moduleDescriptor.Assembly = assembly;
						descriptors.Add(moduleDescriptor);
					} else {
						VixenSystem.Logging.Error("Tried to load module " + moduleDescriptor.TypeName + " from " + Path.GetFileName(filePath) + ", but the ModuleClass does not implement IModuleInstance.");
					}
				}
			}

			return descriptors;
		}

		/// <summary>
		/// Creates a new untyped instance of the module.  This should only be called by repositories.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		static public IModuleInstance GetById(Guid moduleTypeId) {
			Type instanceType;
			IModuleInstance instance = null;
			if(_activators.TryGetValue(moduleTypeId, out instanceType)) {
				instance = Activator.CreateInstance(instanceType) as IModuleInstance;
				instance.InstanceId = Guid.NewGuid();
				instance.Descriptor = GetDescriptorById(moduleTypeId);

				// See if there are any templates to apply to the instance.
				ModuleTemplateModuleManagement manager = Modules.GetManager<IModuleTemplateModuleInstance, ModuleTemplateModuleManagement>();
				manager.ProjectTemplateInto(instance);
			}
			return instance;
		}

		/// <summary>
		/// Determines if the module type represented by the type id is of the provided type.
		/// </summary>
		/// <param name="moduleTypeId"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		static internal bool IsOfType(Guid moduleTypeId, Type type) {
			// Example of type: IMediaModuleInstance
			Type instanceType;
			if(_activators.TryGetValue(moduleTypeId, out instanceType)) {
				if(type.IsInterface) {
					return instanceType.ImplementsInterface(type);
				} else {
					return instanceType.IsSubclassOf(type);
				}
			}
			return false;
		}

		static public IModuleDescriptor GetDescriptorById(Guid moduleTypeId) {
			IModuleDescriptor descriptor = null;
			_moduleDescriptors.TryGetValue(moduleTypeId, out descriptor);
			return descriptor;
		}

		static public T GetDescriptorById<T>(Guid moduleTypeId)
			where T : class, IModuleDescriptor {
			return GetDescriptorById(moduleTypeId) as T;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T">Module instance type as specified by its ModuleImplementation.</typeparam>
		/// <returns></returns>
		static public IModuleDescriptor[] GetDescriptors<T>()
			where T : class, IModuleInstance {
			ModuleImplementation moduleImplementation = GetImplementation<T>();
			if(moduleImplementation != null) {
				List<IModuleDescriptor> descriptors;
				if(_moduleImplementationDescriptors.TryGetValue(moduleImplementation, out descriptors)) {
					return descriptors.ToArray();
				}
			}
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T">Module instance type as specified by its ModuleImplementation.</typeparam>
		/// <typeparam name="U">IModuleDescriptor implemenatation to cast the results to.</typeparam>
		/// <returns></returns>
		static public U[] GetDescriptors<T, U>()
			where T : class, IModuleInstance
			where U : class, IModuleDescriptor {
			return GetDescriptors<T>().Cast<U>().ToArray();
		}

		static public IModuleDescriptor[] GetDescriptors(string typeOfModule) {
			ModuleImplementation moduleImplementation = _moduleImplementationDescriptors.Keys.FirstOrDefault(x => x.TypeOfModule == typeOfModule);
			if(moduleImplementation != null) {
				return _moduleImplementationDescriptors[moduleImplementation].ToArray();
			}
			return null;
		}

		static public bool IsValidId(Guid id) {
			return _activators.ContainsKey(id);
		}

		static internal void PopulateRepositories() {
			foreach(ModuleImplementation moduleImplementation in _moduleImplementationDescriptors.Keys) {
				foreach(IModuleDescriptor moduleDescriptor in _moduleImplementationDescriptors[moduleImplementation]) {
					try {
						moduleImplementation.Repository.Add(moduleDescriptor.TypeId);
					} catch(Exception ex) {
						VixenSystem.Logging.Error("Error occurred while adding module " + moduleDescriptor.FileName, ex);
					}
				}
			}
		}

		static internal void ClearRepositories() {
			foreach(ModuleImplementation moduleImplementation in _moduleImplementationDescriptors.Keys) {
				foreach(IModuleDescriptor moduleDescriptor in _moduleImplementationDescriptors[moduleImplementation]) {
					try {
						moduleImplementation.Repository.Remove(moduleDescriptor.TypeId);
					} catch(Exception ex) {
						VixenSystem.Logging.Error("Error occurred while removing module " + moduleDescriptor.FileName, ex);
					}
				}
			}
		}

		static public U GetManager<T, U>()
			where T : class, IModuleInstance
			where U : class, IModuleManagement<T> {
			ModuleImplementation moduleImplementation = GetImplementation<T>();
			if(moduleImplementation != null) {
				return moduleImplementation.Management as U;
			}
			return null;
		}

		static public IModuleManagement GetManager<T>()
			where T : class, IModuleInstance {
			ModuleImplementation moduleImplementation = GetImplementation<T>();
			if(moduleImplementation != null) {
				return moduleImplementation.Management;
			}
			return null;
		}

		static public U GetRepository<T, U>()
			where T : class, IModuleInstance
			where U : class, IModuleRepository<T> {
			ModuleImplementation moduleImplementation = GetImplementation<T>();
			if(moduleImplementation != null) {
				return moduleImplementation.Repository as U;
			}
			return null;
		}

		static public IModuleRepository GetRepository<T>()
			where T : class, IModuleInstance {
			ModuleImplementation moduleImplementation = GetImplementation<T>();
			if(moduleImplementation != null) {
				return moduleImplementation.Repository;
			}
			return null;
		}

		static public ModuleImplementation[] GetImplementations() {
			return _moduleImplementationDescriptors.Keys.ToArray();
		}

		static public ModuleImplementation GetImplementation<T>()
			where T : class, IModuleInstance {
			return _moduleImplementationDescriptors.Keys.FirstOrDefault(x => x.ModuleInstanceType == typeof(T));
		}

	}
}