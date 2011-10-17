using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vixen.Sys;
using Vixen.IO;
using Vixen.IO.Xml;
using Vixen.Script;
using Vixen.Module;
using Vixen.Module.Editor;
using Vixen.Module.ModuleTemplate;
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
		/// <param name="moduleType"></param>
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

		//Maybe have an overload that takes a file type filter.
		static public string[] GetSequenceFileNames() {
			return Vixen.Sys.Sequence.GetAllFileNames();
		}

		static public ISequence CreateSequence(string sequenceFileType) {
			SequenceModuleManagement manager = Modules.GetManager<ISequenceModuleInstance, SequenceModuleManagement>();
			return manager.Get(sequenceFileType) as ISequence;
		}

		static public OutputController[] GetControllers() {
			return VixenSystem.Controllers.ToArray();
		}

		static public IEditorUserInterface GetEditor(Guid id) {
			EditorModuleManagement manager = Modules.GetManager<IEditorModuleInstance, EditorModuleManagement>();
			if(manager != null) {
				return manager.Get(id);
			}
			return null;
		}

		static public IEditorUserInterface GetEditor(string fileName) {
			EditorModuleManagement manager = Modules.GetManager<IEditorModuleInstance, EditorModuleManagement>();
			if(manager != null) {
				return manager.Get(fileName);
			}
			return null;
		}

		static public string[] GetScriptLanguages() {
			ScriptModuleManagement manager = Modules.GetManager<IScriptModuleInstance, ScriptModuleManagement>();
			return manager.GetLanguages();
		}

		static public void PackageSystemContext(string targetFilePath) {
			SystemContext context = SystemContext.PackageSystemContext(targetFilePath);
			context.Save(targetFilePath);
		}

		static public string UnpackageSystemContext(string contextFilePath) {
			SystemContext context = SystemContext.UnpackageSystemContext(contextFilePath);
			return context.Explode(contextFilePath);
		}
	}
}
