using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandStandard;
using Vixen.Common;
using Vixen.IO;
using System.IO;
using System.Xml;
using Vixen.Hardware;
using Vixen.Script;
using Vixen.Module;
using Vixen.Module.Editor;
using Vixen.Module.FileTemplate;
using Vixen.Module.EffectEditor;
using Vixen.Module.Effect;
using Vixen.Module.Timing;
using Vixen.Module.Media;
using Vixen.Module.Sequence;
using Vixen.Module.Script;

namespace Vixen.Sys {
	/// <summary>
	/// Provides controlled access to otherwise inaccessible members and some convenience methods.
	/// </summary>
    public class ApplicationServices {
        static internal IApplication ClientApplication = null;

		static public IModuleDescriptor[] GetModuleDescriptors(string typeOfModule) {
			return Modules.GetModuleDescriptors(typeOfModule);
		}
		
		static public IModuleDescriptor[] GetModuleDescriptors<T>()
			where T : class, IModuleInstance {
			return Modules.GetModuleDescriptors<T>();
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
		/// <param name="moduleType"></param>
		/// <returns></returns>
		static public Dictionary<Guid, string> GetAvailableModules<T>()
			where T : class, IModuleInstance {
			return Modules.GetModuleDescriptors<T>().ToDictionary(x => x.TypeId, x => x.TypeName);
		}

		static public string[] GetTypesOfModules() {
			return Modules.GetImplementations().Select(x => x.TypeOfModule).ToArray();
		}

		//*** Bring back when the user is allowed to load a module during runtime
		//static public void UnloadModule(Guid moduleTypeId, string moduleType) {
		//    Modules.UnloadModule(moduleTypeId, moduleType);
		//}

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
			IModuleManagement moduleManager = Modules.GetModuleManager<T>();
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
			IModuleManagement moduleManager = Modules.GetModuleManager<T>();
			if(moduleManager != null) {
				return moduleManager.GetAll().Cast<T>().ToArray();
			}
			return null;
		}

		static public IEffectEditorControl GetEffectEditorControl(Guid effectId) {
			// Need the module-specific manager.
			EffectEditorModuleManagement manager = Modules.GetModuleManager<IEffectEditorModuleInstance, EffectEditorModuleManagement>();
			return manager.GetEffectEditor(effectId);
		}

		//Maybe have an overload that takes a file type filter.
		static public string[] GetSequenceFileNames() {
			return Vixen.Sys.Sequence.GetAllFileNames();
		}

		static public ISequence CreateSequence(string fileExtension) {
			SequenceModuleManagement manager = Modules.GetModuleManager<ISequenceModuleInstance, SequenceModuleManagement>();
			return manager.Get(fileExtension) as ISequence;
		}

		static public OutputController[] GetControllers() {
			return OutputController.GetAll().ToArray();
		}

		// Only exists to be alongside GetEditor(string) instead of making them look in
		// multiple locations.
		static public IEditorModuleInstance GetEditor(Guid id) {
			return Modules.ModuleManagement.GetEditor(id);
		}

		static public IEditorModuleInstance GetEditor(string fileName) {
			EditorModuleManagement manager = Modules.GetModuleManager<IEditorModuleInstance, EditorModuleManagement>();
			if(manager != null) {
				return manager.Get(fileName);
			}
			return null;
		}

		/// <summary>
		/// Commits the template's data back to its backing store.
		/// </summary>
		/// <param name="template"></param>
		static public void CommitTemplate(IFileTemplate template) {
			FileTemplateModuleManagement manager = Modules.GetModuleManager<IFileTemplateModuleInstance, FileTemplateModuleManagement>();
			manager.SaveTemplateData(template as IFileTemplateModuleInstance);
		}

		static public string[] GetScriptLanguages() {
			ScriptModuleManagement manager = Modules.GetModuleManager<IScriptModuleInstance, ScriptModuleManagement>();
			return manager.GetLanguages();
		}

	}
}
