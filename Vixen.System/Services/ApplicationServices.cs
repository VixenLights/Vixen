using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Module.EffectEditor;
using Vixen.Rule;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Services {
    public class ApplicationServices {
        static internal IApplication ClientApplication;

		static public IModuleDescriptor[] GetModuleDescriptors(string typeOfModule) {
			return Modules.GetDescriptors(typeOfModule);
		}
		
		static public IModuleDescriptor[] GetModuleDescriptors<T>()
			where T : class, IModuleInstance {
			return Modules.GetDescriptors<T>();
		}

		static public IModuleDescriptor GetModuleDescriptor(Guid moduleTypeId) {
			return Modules.GetDescriptorById(moduleTypeId);
		}

		static public T GetModuleDescriptor<T>(Guid moduleTypeId)
			where T : class, IModuleDescriptor {
			return Modules.GetDescriptorById(moduleTypeId) as T;
		}

		/// <summary>
		/// Gets a dictionary of the available modules based on the descriptors of installed modules.
		/// </summary>
		/// <returns></returns>
		static public Dictionary<Guid, string> GetAvailableModules<T>()
			where T : class, IModuleInstance {
			return Modules.GetDescriptors<T>().ToDictionary(x => x.TypeId, x => x.TypeName);
		}

		static public string[] GetTypesOfModules() {
			return Modules.GetImplementations().Select(x => x.TypeOfModule).ToArray();
		}

		static public void UnloadModule(Guid moduleTypeId) {
			IModuleDescriptor descriptor = Modules.GetDescriptorById(moduleTypeId);
			if(descriptor != null) {
				Modules.UnloadModule(descriptor);
			}
		}

		static public void ReloadModule(Guid moduleTypeId) {
			IModuleDescriptor descriptor = Modules.GetDescriptorById(moduleTypeId);
			string moduleFilePath = descriptor.Assembly.Location;
			Modules.UnloadModule(descriptor);
			Modules.LoadModule(moduleFilePath, new[] { moduleTypeId });
		}

		/// <summary>
		/// Gets an instance of a module.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		static public T Get<T>(Guid id)
			where T : class, IModuleInstance {
			// Must go through the module type manager, instead of using
			// Modules.GetById, so that the type manager can affect the instance.
			// Modules.ModuleManagement can be called when the name of the module
			// type is known, which it is not here.
			IModuleManagement moduleManager = Modules.GetManager<T>();
			if(moduleManager != null) {
				return moduleManager.Get(id) as T;
			}
			return null;
		}

		/// <summary>
		/// Gets an instance of each module of the type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		static public T[] GetAll<T>()
			where T : class, IModuleInstance {
			// Must go through the module type manager so that it can affect the instance.
			IModuleManagement moduleManager = Modules.GetManager<T>();
			if(moduleManager != null) {
				return moduleManager.GetAll().Cast<T>().ToArray();
			}
			return null;
		}

		static public IEnumerable<IEffectEditorControl> GetEffectEditorControls(Guid effectId) {
			// Need the module-specific manager.
			EffectEditorModuleManagement manager = Modules.GetManager<IEffectEditorModuleInstance, EffectEditorModuleManagement>();
			return manager.GetEffectEditors(effectId);
		}

		static public INamingGenerator[] GetAllNamingGenerators() {
			return typeof(INamingGenerator).FindConcreteImplementationsWithin(Assembly.GetExecutingAssembly()).Select(Activator.CreateInstance).Cast<INamingGenerator>().ToArray();
		}

		static public bool AreAllEffectRequiredPropertiesPresent(IEffectModuleInstance effectModule) {
			// This only happens if the effect is created outside of us.
			// In that case, it's not really a module instance and we can't check for required properties.
			// They're on their own as far as the outcome.
			if(effectModule.Descriptor != null) {
				EffectModuleDescriptorBase effectDescriptor = Modules.GetDescriptorById<EffectModuleDescriptorBase>(effectModule.Descriptor.TypeId);
				return effectModule.TargetNodes.All(x => x.Properties.Select(y => y.Descriptor.TypeId).Intersect(effectDescriptor.PropertyDependencies).Count() == effectDescriptor.PropertyDependencies.Length);
			}
			return true;
		}
	}
}
