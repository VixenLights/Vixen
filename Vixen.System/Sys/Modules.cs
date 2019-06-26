using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Vixen.Module;
using Vixen.Services;
using Vixen.Sys.Attribute;

namespace Vixen.Sys
{
	internal class Modules
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		// Module type id : IModuleInstance implementor
		private static Dictionary<Guid, Type> _activators = new Dictionary<Guid, Type>();
		// Module type id : module descriptor
		private static Dictionary<Guid, IModuleDescriptor> _moduleDescriptors = new Dictionary<Guid, IModuleDescriptor>();
		// ModuleImplementation : descriptors for modules of that type
		private static Dictionary<ModuleImplementation, HashSet<IModuleDescriptor>> _moduleImplementationDescriptors =
			new Dictionary<ModuleImplementation, HashSet<IModuleDescriptor>>();

		public static dynamic ModuleManagement { get; private set; }
		public static dynamic ModuleRepository { get; private set; }

		public static readonly string Directory;

		static Modules()
		{
			Directory = Path.Combine(Paths.BinaryRootPath, "Modules");

			_moduleImplementationDescriptors = Assembly.GetExecutingAssembly().GetAttributedTypes(typeof (TypeOfModuleAttribute))
				.ToDictionary(
					x => Activator.CreateInstance(x) as ModuleImplementation,
					x => new HashSet<IModuleDescriptor>());

			ModuleImplementation[] moduleImplementations = _moduleImplementationDescriptors.Keys.ToArray();
			ModuleManagement = new ModuleManagement(moduleImplementations);
			ModuleRepository = new ModuleRepository(moduleImplementations);
		}

		public static void LoadAllModules()
		{
			// All module types must be loaded simultaneously for dependencies to be resolved.
			IEnumerable<string> allModuleFiles =
				_moduleImplementationDescriptors.Keys.Select(_GetModuleTypeDirectory).SelectMany(
					x => System.IO.Directory.GetFiles(x, "*.dll"));
			_LoadModulesFromFile(allModuleFiles);
		}

		public static void LoadModule(string filePath, IEnumerable<Guid> typesToLoad = null)
		{
			_LoadModulesFromFile(new[] {filePath}, typesToLoad);
		}

		private static void _LoadModulesFromFile(IEnumerable<string> filePaths, IEnumerable<Guid> typesToLoad = null)
		{
			IEnumerable<IModuleDescriptor> descriptors;

			// For each type of module discovered, get any descriptors.
			// Get the results into a static collection or this will be called every time
			// "descriptors" is enumerated.
			descriptors = _LoadModuleDescriptors(filePaths.ToArray());
			// Restrict to only the types specified, if any.
			if (typesToLoad != null) {
				descriptors = descriptors.Where(x => typesToLoad.Contains(x.TypeId));
			}
			// Remove any that have duplicate type ids.
			descriptors = _RemoveDuplicateTypes(descriptors);
			// Check the dependencies of all modules.
			descriptors = _ResolveAgainstDependencies(descriptors);
			// Reconcile the lists of descriptors earlier loaded against what will actually be loaded
			// and remove anything that won't be loaded.
			foreach (ModuleImplementation moduleImplementation in _moduleImplementationDescriptors.Keys) {
				HashSet<IModuleDescriptor> moduleTypeDescriptors = _moduleImplementationDescriptors[moduleImplementation];
				// It appears that Except will return the *distinct* set.  Since we're wanting
				// to remove instances with duplicate identities, Except will not work for us.
				foreach (IModuleDescriptor removedDescriptor in moduleTypeDescriptors.Where(x => !descriptors.Contains(x)).ToArray()
					) {
					moduleTypeDescriptors.Remove(removedDescriptor);
				}
			}

			// The remaining modules are okay to load.
			_LoadModules(descriptors);
		}

		private static void _LoadModules(IEnumerable<IModuleDescriptor> descriptors)
		{
			foreach (IModuleDescriptor descriptor in descriptors) {
				// Add module data path, if any.
				Paths.AddDataPaths(descriptor.GetType(), typeof (ModuleDataPathAttribute));
				// Add descriptor to the descriptor dictionary.
				_moduleDescriptors[descriptor.TypeId] = descriptor;
				// Get a reference to the module implementation and add it to the activator dictionary.
				_activators[descriptor.TypeId] = descriptor.ModuleClass;
			}
		}

		public static void UnloadModule(IModuleDescriptor descriptor)
		{
			//*** what about the dependencies upon this module?
			//-> And if there are lists that reference a dependency, they won't know that
			//   it's been unloaded
			_activators.Remove(descriptor.TypeId);
			_moduleDescriptors.Remove(descriptor.TypeId);
			// We don't know the ModuleImplementation that the descriptor belongs to.
			foreach (HashSet<IModuleDescriptor> moduleImplementationDescriptors in _moduleImplementationDescriptors.Values) {
				moduleImplementationDescriptors.Remove(descriptor);
			}
		}

		private static IEnumerable<IModuleDescriptor> _RemoveDuplicateTypes(IEnumerable<IModuleDescriptor> loadingDescriptors)
		{
			// Check the loading descriptors against themselves and the loaded descriptors.
			// If there are duplicates, remove them from the loading descriptors, not the ones
			// already loaded.
			IEnumerable<IModuleDescriptor> allDescriptors = loadingDescriptors.Concat(_moduleDescriptors.Values);
			IEnumerable<IGrouping<Guid, IModuleDescriptor>> duplicateTypes =
				allDescriptors.GroupBy(x => x.TypeId).Where(x => x.Count() > 1);
			if (duplicateTypes.Any()) {
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("The following modules were not loaded because they have duplicate type ids:");
				foreach (var duplicateType in duplicateTypes) {
					sb.AppendFormat("Type Id: {0}\n", duplicateType.Key);
					foreach (IModuleDescriptor descriptor in duplicateType) {
						sb.AppendFormat("  - {0} Loaded From {1} \n", descriptor.TypeName, descriptor.Assembly.GetFilePath());
					}
				}
				Logging.Error(sb.ToString());
			}

			return loadingDescriptors.Except(duplicateTypes.SelectMany(x => x));
		}

		private static IEnumerable<IModuleDescriptor> _ResolveAgainstDependencies(
			IEnumerable<IModuleDescriptor> loadingDescriptors)
		{
			List<IModuleDescriptor> allDescriptors = loadingDescriptors.Concat(_moduleDescriptors.Values).ToList();

			// Check the dependencies of all modules.
			// Keep checking until there are no missing dependencies.
			while (true) {
				// Need a static collection to enumerate, but only of the loading descriptors.
				IModuleDescriptor[] remainingDescriptors = loadingDescriptors.ToArray();
				// Remove any descriptors that have missing dependencies.
				foreach (IModuleDescriptor descriptor in remainingDescriptors) {
					// All dependencies must be present in the collection of descriptors
					// for the module to be eligible for loading.
					if (descriptor.Dependencies != null && descriptor.Dependencies.Any() &&
					    descriptor.Dependencies.Intersect(allDescriptors.Select(x => x.TypeId)).Count() !=
					    descriptor.Dependencies.Length) {
						allDescriptors.Remove(descriptor);
						Logging.Error("Could not load module \"" + descriptor.TypeName +
												  "\" because it has dependencies that are missing.");
					}
				}
				// If nothing was removed, we are done.
				if (loadingDescriptors.Count() == remainingDescriptors.Length) break;
			}
			;

			return allDescriptors;
		}

		private static string _GetModuleTypeDirectory(ModuleImplementation moduleImplementation)
		{
			string moduleTypeName =
				(moduleImplementation.GetType().GetCustomAttributes(typeof (TypeOfModuleAttribute), false).First() as
				 TypeOfModuleAttribute).Name;
			string moduleTypeDirectory = Path.Combine(Directory, moduleTypeName);
			moduleImplementation.Path = moduleTypeDirectory;
			return moduleTypeDirectory;
		}

		private static IEnumerable<IModuleDescriptor> _LoadModuleDescriptors(IEnumerable<string> filePaths)
		{
			List<IModuleDescriptor> descriptors = new List<IModuleDescriptor>();

			foreach (string filePath in filePaths) {
				try {
					// Determine what kind of module it is so its descriptor can be added
					// to that implementation's descriptor list.
					ModuleImplementation moduleImplementation = _FindImplementation(filePath);
					if (moduleImplementation == null) {
						Logging.Error(string.Format("File {0} was not loaded because its module type could not be determined.", filePath));
						continue;
					}
					// Try to get descriptors.
					IEnumerable<IModuleDescriptor> descriptorsFound = _LoadModuleDescriptors(filePath);
					descriptors.AddRange(descriptorsFound);
					_moduleImplementationDescriptors[moduleImplementation].AddRange(descriptorsFound);
				}
				catch (BadImageFormatException) {
				}
				catch (ReflectionTypeLoadException ex) {
					foreach (Exception loaderException in ex.LoaderExceptions) {
						Logging.Error(loaderException, "Loader exception:" + Environment.NewLine + loaderException.Message);
					}
				}
				catch (Exception ex) {
					Logging.Error(ex, "Error loading modules from " + filePath);
				}
			}

			return descriptors;
		}

		private static ModuleImplementation _FindImplementation(string filePath)
		{
			string directory = Path.GetDirectoryName(filePath);
			return
				_moduleImplementationDescriptors.Keys.FirstOrDefault(
					x => x.Path.Equals(directory, StringComparison.OrdinalIgnoreCase));
		}

		private static IEnumerable<IModuleDescriptor> _LoadModuleDescriptors(string filePath)
		{
			List<IModuleDescriptor> descriptors = new List<IModuleDescriptor>();

			if (File.Exists(filePath)) {
				Assembly assembly = null;
				try {
					assembly = Assembly.LoadFrom(filePath);
				}
				catch (NotSupportedException se) {
					Logging.Error("Could not load module assembly " + filePath +
					                          ".  See http://vanderbiest.org/blog/2010/08/11/system-notsupportedexception-when-referencing-to-an-assembly/", se);
					return new IModuleDescriptor[0];
				}
				catch (BadImageFormatException) {
					return new IModuleDescriptor[0];
				}
				catch (Exception ex) {
					Logging.Error(ex, $"Could not load module assembly {filePath}.");
					return new IModuleDescriptor[0];
				}

				// Look for concrete module descriptors.
				IEnumerable<Type> moduleDescriptorTypes =
					typeof (IModuleDescriptor).FindImplementationsWithin(assembly).Where(x => !x.IsAbstract);
				foreach (Type moduleDescriptorType in moduleDescriptorTypes) {
					try {
						// Get the module descriptor.
						object o = Activator.CreateInstance(moduleDescriptorType);
						IModuleDescriptor moduleDescriptor = o as IModuleDescriptor;

						if (moduleDescriptor != null) {
							if (moduleDescriptor.ModuleClass != null) {
								// Make sure its module's type is an IModuleInstance
								if (moduleDescriptor.ModuleClass.ImplementsInterface(typeof (IModuleInstance))) {
									// Set the name of the file it was borne from.
									moduleDescriptor.FileName = Path.GetFileName(filePath);
									// Set the assembly it was borne from.
									moduleDescriptor.Assembly = assembly;
									descriptors.Add(moduleDescriptor);
								}
								else {
									Logging.Error(string.Format("Tried to load module {0}  from {1}, but the ModuleClass does not implement IModuleInstance.", moduleDescriptor.TypeName, Path.GetFileName(filePath)));
								}
							}
							else {
								Logging.Error(string.Format("Tried to load module {0} from {1}, but the ModuleClass is null.", moduleDescriptor.TypeName, Path.GetFileName(filePath)));
 							}
						}
						else {
							 Logging.Error(string.Format("Tried to load module of type {0} from {1}, but the descriptor does not implement IModuleDescriptor.", moduleDescriptorType.Name, Path.GetFileName(filePath)));
						}
					}
					catch (Exception ex) {
						Logging.Error(ex,
							$"Error loading module descriptor {moduleDescriptorType.Name} from {Path.GetFileName(filePath)}.");
					}
				}
			}

			return descriptors;
		}

		/// <summary>
		/// Creates a new untyped instance of the module.  This should only be called by repositories.
		/// </summary>
		public static IModuleInstance GetById(Guid moduleTypeId)
		{
			Type instanceType;
			IModuleInstance instance = null;
			if (_activators.TryGetValue(moduleTypeId, out instanceType)) {
				instance = (IModuleInstance) Activator.CreateInstance(instanceType);
				instance.Descriptor = GetDescriptorById(moduleTypeId);

				try {
					instance.StaticModuleData = GetModuleStaticData(instance);
				}
				catch (Exception ex) {
					Logging.Error(ex, "Error when assigning module static data.");
				}

				try {
					instance.ModuleData = _GetModuleData(instance);
				}
				catch (Exception ex) {
					Logging.Error(ex, "Error when assigning module data.");
				}

				// See if there are any templates to apply to the instance.
				ModuleTemplateService.Instance.ProjectTemplateInto(instance);
			}
			else {
				Logging.Warn("Attempt to instantiate non-existent module " + moduleTypeId);
			}
			return instance;
		}

		/// <summary>
		/// Determines if the module type represented by the type id is of the provided type.
		/// </summary>
		/// <param name="moduleTypeId"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static bool IsOfType(Guid moduleTypeId, Type type)
		{
			// Example of type: IMediaModuleInstance
			Type instanceType;
			if (_activators.TryGetValue(moduleTypeId, out instanceType)) {
				if (type.IsInterface) {
					return instanceType.ImplementsInterface(type);
				}
				else {
					return instanceType.IsSubclassOf(type);
				}
			}
			return false;
		}

		public static IModuleDescriptor GetDescriptorById(Guid moduleTypeId)
		{
			IModuleDescriptor descriptor = null;
			_moduleDescriptors.TryGetValue(moduleTypeId, out descriptor);
			return descriptor;
		}

		public static T GetDescriptorById<T>(Guid moduleTypeId)
			where T : class, IModuleDescriptor
		{
			return GetDescriptorById(moduleTypeId) as T;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T">Module instance type as specified by its ModuleImplementation.</typeparam>
		/// <returns></returns>
		public static IModuleDescriptor[] GetDescriptors<T>()
			where T : class, IModuleInstance
		{
			ModuleImplementation moduleImplementation = GetImplementation<T>();
			if (moduleImplementation != null) {
				HashSet<IModuleDescriptor> descriptors;
				if (_moduleImplementationDescriptors.TryGetValue(moduleImplementation, out descriptors)) {
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
		public static U[] GetDescriptors<T, U>()
			where T : class, IModuleInstance
			where U : class, IModuleDescriptor
		{
			return GetDescriptors<T>().Cast<U>().ToArray();
		}

		public static IModuleDescriptor[] GetDescriptors(string typeOfModule)
		{
			ModuleImplementation moduleImplementation =
				_moduleImplementationDescriptors.Keys.FirstOrDefault(x => x.TypeOfModule == typeOfModule);
			if (moduleImplementation != null) {
				return _moduleImplementationDescriptors[moduleImplementation].ToArray();
			}
			return null;
		}

		public static bool IsValidId(Guid id)
		{
			return _activators.ContainsKey(id);
		}

		internal static void PopulateRepositories()
		{
			foreach (ModuleImplementation moduleImplementation in _moduleImplementationDescriptors.Keys) {
				foreach (IModuleDescriptor moduleDescriptor in _moduleImplementationDescriptors[moduleImplementation]) {
					try {
						moduleImplementation.Repository.Add(moduleDescriptor.TypeId);
					}
					catch (Exception ex) {
						Logging.Error(ex, "Error occurred while adding module " + moduleDescriptor.FileName);
					}
				}
			}
		}

		internal static void ClearRepositories()
		{
			foreach (ModuleImplementation moduleImplementation in _moduleImplementationDescriptors.Keys) {
				foreach (IModuleDescriptor moduleDescriptor in _moduleImplementationDescriptors[moduleImplementation]) {
					try {
						moduleImplementation.Repository.Remove(moduleDescriptor.TypeId);
					}
					catch (Exception ex) {
						Logging.Error(ex, "Error occurred while removing module " + moduleDescriptor.FileName);
					}
				}
			}
		}

		public static U GetManager<T, U>()
			where T : class, IModuleInstance
			where U : class, IModuleManagement<T>
		{
			ModuleImplementation moduleImplementation = GetImplementation<T>();
			if (moduleImplementation != null) {
				return moduleImplementation.Management as U;
			}
			return null;
		}

		public static IModuleManagement GetManager<T>()
			where T : class, IModuleInstance
		{
			ModuleImplementation moduleImplementation = GetImplementation<T>();
			if (moduleImplementation != null) {
				return moduleImplementation.Management;
			}
			return null;
		}

		public static U GetRepository<T, U>()
			where T : class, IModuleInstance
			where U : class, IModuleRepository<T>
		{
			ModuleImplementation moduleImplementation = GetImplementation<T>();
			if (moduleImplementation != null) {
				return moduleImplementation.Repository as U;
			}
			return null;
		}

		public static IModuleRepository GetRepository<T>()
			where T : class, IModuleInstance
		{
			ModuleImplementation moduleImplementation = GetImplementation<T>();
			if (moduleImplementation != null) {
				return moduleImplementation.Repository;
			}
			return null;
		}

		public static ModuleImplementation[] GetImplementations()
		{
			return _moduleImplementationDescriptors.Keys.ToArray();
		}

		public static ModuleImplementation GetImplementation<T>()
			where T : class, IModuleInstance
		{
			return _moduleImplementationDescriptors.Keys.FirstOrDefault(x => x.ModuleInstanceType == typeof (T));
		}

		/// <summary>
		/// Creates a default data object for the module without adding it to any dataset.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		private static IModuleDataModel _GetModuleData(IModuleInstance instance)
		{
			// Remember, the data is orphaned initially!
			IModuleDataModel dataModel = null;

			if (instance.Descriptor.ModuleDataClass != null) {
				dataModel = ModuleLocalDataSet.CreateModuleDataInstance(instance);
				if (dataModel == null) {
					Logging.Error(string.Format("Module \"{0}\" in {1} has a reference to type {2} for its module data class, but it's not an implementation of {3}.", instance.Descriptor.TypeName, instance.Descriptor.FileName, instance.Descriptor.ModuleDataClass.Name, typeof(IModuleDataModel).Name));
				}
			}

			return dataModel;
		}

		public static IModuleDataModel GetModuleStaticData(IModuleInstance instance)
		{
			// All instances of a given module type will share a single instance of that type's
			// static data.  A change in one is reflected in all.
			return VixenSystem.ModuleStore.TypeData.GetTypeData(instance);
		}

		public static IModuleDataModel GetModuleStaticData(Guid id)
		{
			// All instances of a given module type will share a single instance of that type's
			// static data.  A change in one is reflected in all.
			return VixenSystem.ModuleStore.TypeData.GetTypeData(id);
		}
	}
}