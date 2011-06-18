using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Vixen.Module;
using Vixen.Common;
using System.IO;

namespace Vixen.Sys {
	class Modules {
		// Module type id : IModuleInstance implementor
		static private Dictionary<Guid, Type> _activators = new Dictionary<Guid, Type>();
		// Module type id : containing assembly
		static private Dictionary<Guid, Assembly> _moduleAssemblies = new Dictionary<Guid, Assembly>();
		// Module type id : module descriptor
		static private Dictionary<Guid, IModuleDescriptor> _moduleDescriptors = new Dictionary<Guid, IModuleDescriptor>();

		static public readonly string Directory;

		static Modules() {
			Directory = Path.Combine(Paths.BinaryRootPath, "Modules");
		}

		static public void LoadModules(string filePath, string moduleTypeName) {
			// If moduleTypeName is null or empty, then there is no defined module type for the
			// path that the file came from.
			if(string.IsNullOrEmpty(moduleTypeName)) return;
			if(!File.Exists(filePath)) return;

			//Assembly assembly = Assembly.LoadFile(filePath);
			Assembly assembly = null;
			assembly = Assembly.LoadFrom(filePath);

			IModuleDescriptor moduleDescriptor = null;

			// Look for concrete module descriptors.
			IEnumerable<Type> moduleDescriptorTypes = typeof(IModuleDescriptor).FindImplementationsWithin(assembly).Where(x => !x.IsAbstract);
			foreach(Type moduleDescriptorType in moduleDescriptorTypes) {
				// Get the module descriptor.
				moduleDescriptor = Activator.CreateInstance(moduleDescriptorType) as IModuleDescriptor;

				// Set the name of the file it was borne from.
				moduleDescriptor.FileName = Path.GetFileName(filePath);

				// Set the name of the module type that this is a part of (e.g. "Output")
				moduleDescriptor.ModuleTypeName = moduleTypeName;

				// Add module data path, if any.
				Paths.AddDataPaths(moduleDescriptorType, typeof(ModuleDataPathAttribute));

				// Ensure the module type being described implements IModuleInstance.
				if(moduleDescriptor.ModuleClass.ImplementsInterface(typeof(IModuleInstance))) {
					// Add descriptor to the descriptor dictionary.
					_moduleDescriptors[moduleDescriptor.TypeId] = moduleDescriptor;
					// Add descriptor to the assembly dictionary.
					_moduleAssemblies[moduleDescriptor.TypeId] = assembly;
					// Get a reference to the module implementation and add it to the activator dictionary.
					_activators[moduleDescriptor.TypeId] = moduleDescriptor.ModuleClass;
				}
			}
		}

		static public void UnloadModule(Guid moduleTypeId, string moduleType) {
			// Remove it from all lookups at this level.
			_activators.Remove(moduleTypeId);
			_moduleDescriptors.Remove(moduleTypeId);
			// Get the module type implementation and remove the module from the repository.
			VixenSystem.Internal.GetModuleType(moduleType).Repository.Remove(moduleTypeId);
		}

		/// <summary>
		/// Creates a new untyped instance of the module.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		static internal IModuleInstance GetById(Guid moduleTypeId) {
			Type instanceType;
			IModuleInstance instance = null;
			if(_activators.TryGetValue(moduleTypeId, out instanceType)) {
				instance = Activator.CreateInstance(instanceType) as IModuleInstance;
				instance.InstanceId = Guid.NewGuid();
				instance.TypeName = GetDescriptorById(moduleTypeId).TypeName;
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
		static public IModuleDescriptor[] GetModuleDescriptors<T>()
			where T : class, IModuleInstance {
			string moduleTypeName = VixenSystem.Internal.GetModuleTypes().First(x => x.ModuleInstanceType == typeof(T)).ModuleTypeName;
			return _moduleDescriptors.Values.Where(x => x.ModuleTypeName == moduleTypeName).ToArray();
		}

		static public IModuleDescriptor[] GetModuleDescriptors(string moduleTypeName) {
			return _moduleDescriptors.Values.Where(x => x.ModuleTypeName.Equals(moduleTypeName, StringComparison.OrdinalIgnoreCase)).ToArray();
		}

		static public bool IsValidId(Guid id) {
			return _activators.ContainsKey(id);
		}
	}
}
