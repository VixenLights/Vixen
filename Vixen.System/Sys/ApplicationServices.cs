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
using Vixen.Sequence;
using Vixen.Module;
using Vixen.Module.Editor;
using Vixen.Module.FileTemplate;
using Vixen.Module.CommandEditor;
using Vixen.Module.CommandSpec;

//Provides access to otherwise inaccessible members.
namespace Vixen.Sys {
    /// <summary>
    /// For use by an external client, an IApplication instance.
    /// </summary>
    public class ApplicationServices {
        static internal IApplication ClientApplication;

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
			return Server.Internal.GetModuleTypes().Select(x => x.ModuleTypeName).ToArray();
		}

		//*** there needs to a loading counterpart to this as well
		//-> Have it load anything not currently referenced?
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
			return Server.Internal.GetModuleManager<T>().Get(id) as T;
		}

		/// <summary>
		/// This does not do what you think it does.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		static public T[] GetAll<T>()
			where T : class, IModuleInstance {
			// Must go through the module type manager so that it can affect the instance.
			return Server.Internal.GetModuleManager<T>().GetAll().Cast<T>().ToArray();
		}

		static public ICommandEditorControl GetCommandEditorControl(Guid commandSpecId) {
			// Need the module-specific manager.
			CommandEditorModuleManagement manager = Server.Internal.GetModuleManager<ICommandEditorModuleInstance, CommandEditorModuleManagement>();
			return manager.GetCommandEditor(commandSpecId);
		}

		//Maybe have an overload that takes a file type filter.
		static public string[] GetSequenceFileNames() {
			return Vixen.Sys.Sequence.GetAllFileNames();
		}

		static public Vixen.Module.Sequence.ISequenceModuleInstance CreateSequence(string fileExtension) {
			return Vixen.Sys.Sequence.Get(fileExtension);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">Must be a qualified file path.</param>
        /// <returns></returns>
        static public Vixen.Module.Sequence.ISequenceModuleInstance LoadSequence(string fileName) {
			return Vixen.Sys.Sequence.Load(fileName);
		}

        static public Program LoadProgram(string name) {
            return Program.Load(name);
        }

		// Module type id : Timing source name
        static public Dictionary<Guid, string> GetTimingSources() {
            return OutputController.GetTimingSources().ToDictionary(x => x, x => Modules.GetDescriptorById(x).TypeName);
        }

        static public OutputController[] GetConfiguredControllers(ISequence executable) {
            return OutputController.GetAll(executable.ModuleDataSet).ToArray();
        }

		static public IEditorModuleInstance GetEditor(Guid id) {
			EditorModuleManagement manager = Server.Internal.GetModuleManager<IEditorModuleInstance, EditorModuleManagement>();
			return manager.Get(id);
		}

		static public IEditorModuleInstance GetEditor(string fileName) {
			EditorModuleManagement manager = Server.Internal.GetModuleManager<IEditorModuleInstance, EditorModuleManagement>();
			return manager.Get(fileName);
		}

		/// <summary>
		/// Commits the template's data back to its backing store.
		/// </summary>
		/// <param name="template"></param>
		static public void CommitTemplate(IFileTemplate template) {
			FileTemplateModuleManagement manager = Server.Internal.GetModuleManager<IFileTemplateModuleInstance, FileTemplateModuleManagement>();
			manager.SaveTemplateData(template as IFileTemplateModuleInstance);
		}
    }
}
