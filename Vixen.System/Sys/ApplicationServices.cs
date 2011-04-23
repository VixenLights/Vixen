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

namespace Vixen.Sys {
	/// <summary>
	/// Provides controlled access to otherwise inaccessible members and some convenience methods.
	/// </summary>
    public class ApplicationServices {
        static internal IApplication ClientApplication = null;

		static public IModuleDescriptor[] GetModuleDescriptors(string moduleType) {
			return Modules.GetModuleDescriptors(moduleType);
		}

		static public IModuleDescriptor GetModuleDescriptor(Guid moduleTypeId) {
			return Modules.GetDescriptorById(moduleTypeId);
		}

		static public Dictionary<Guid, string> GetAvailableModules(string moduleType) {
			return Modules.GetModuleDescriptors(moduleType).ToDictionary(x => x.TypeId, x => x.TypeName);
		}

		static public string[] GetModuleTypes() {
			return VixenSystem.Internal.GetModuleTypes().Select(x => x.ModuleTypeName).ToArray();
		}

		static public void UnloadModule(Guid moduleTypeId, string moduleType) {
			Modules.UnloadModule(moduleTypeId, moduleType);
		}

		/// <summary>
		/// Gets an instance of a module.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		static public T Get<T>(Guid id)
			where T : class, IModuleInstance {
			// Must go through the module type manager so that it can affect the instance.
			return VixenSystem.Internal.GetModuleManager<T>().Get(id) as T;
		}

		/// <summary>
		/// This does not do what you think it does.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		static public T[] GetAll<T>()
			where T : class, IModuleInstance {
			// Must go through the module type manager so that it can affect the instance.
			return VixenSystem.Internal.GetModuleManager<T>().GetAll().Cast<T>().ToArray();
		}

		static public IEffectEditorControl GetEffectEditorControl(Guid effectId) {
			// Need the module-specific manager.
			EffectEditorModuleManagement manager = VixenSystem.Internal.GetModuleManager<IEffectEditorModuleInstance, EffectEditorModuleManagement>();
			return manager.GetEffectEditor(effectId);
		}

		//Maybe have an overload that takes a file type filter.
		static public string[] GetSequenceFileNames() {
			return Vixen.Sys.Sequence.GetAllFileNames();
		}

		static public ISequence CreateSequence(string fileExtension) {
			SequenceModuleManagement manager = Vixen.Sys.VixenSystem.Internal.GetModuleManager<ISequenceModuleInstance, SequenceModuleManagement>();
			return manager.Get(fileExtension) as ISequence;
		}

		static public OutputController[] GetControllers() {
			return OutputController.GetAll().ToArray();
		}

		static public IEditorModuleInstance GetEditor(Guid id) {
			EditorModuleManagement manager = VixenSystem.Internal.GetModuleManager<IEditorModuleInstance, EditorModuleManagement>();
			return manager.Get(id);
		}

		static public IEditorModuleInstance GetEditor(string fileName) {
			EditorModuleManagement manager = VixenSystem.Internal.GetModuleManager<IEditorModuleInstance, EditorModuleManagement>();
			return manager.Get(fileName);
		}

		/// <summary>
		/// Commits the template's data back to its backing store.
		/// </summary>
		/// <param name="template"></param>
		static public void CommitTemplate(IFileTemplate template) {
			FileTemplateModuleManagement manager = VixenSystem.Internal.GetModuleManager<IFileTemplateModuleInstance, FileTemplateModuleManagement>();
			manager.SaveTemplateData(template as IFileTemplateModuleInstance);
		}

		static public string[] GetScriptLanguages() {
			return Script.Registration.GetLanguages();
		}
    }
}
