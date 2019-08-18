using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Module.EffectEditor;
using Vixen.Rule;
using Vixen.Sys;

namespace Vixen.Services
{
	public class ApplicationServices
	{
		internal static IApplication ClientApplication;

		public static IModuleDescriptor[] GetModuleDescriptors(string typeOfModule)
		{
			return Modules.GetDescriptors(typeOfModule);
		}

		public static IModuleDescriptor[] GetModuleDescriptors<T>()
			where T : class, IModuleInstance
		{
			return Modules.GetDescriptors<T>();
		}

		public static IModuleDescriptor GetModuleDescriptor(Guid moduleTypeId)
		{
			return Modules.GetDescriptorById(moduleTypeId);
		}

		public static T GetModuleDescriptor<T>(Guid moduleTypeId)
			where T : class, IModuleDescriptor
		{
			return Modules.GetDescriptorById(moduleTypeId) as T;
		}

		/// <summary>
		/// Gets a dictionary of the available modules based on the descriptors of installed modules.
		/// </summary>
		/// <returns></returns>
		public static Dictionary<Guid, string> GetAvailableModules<T>()
			where T : class, IModuleInstance
		{
			return Modules.GetDescriptors<T>().ToDictionary(x => x.TypeId, x => x.TypeName);
		}

		public static string[] GetTypesOfModules()
		{
			return Modules.GetImplementations().Select(x => x.TypeOfModule).ToArray();
		}

		public static void UnloadModule(Guid moduleTypeId)
		{
			IModuleDescriptor descriptor = Modules.GetDescriptorById(moduleTypeId);
			if (descriptor != null) {
				Modules.UnloadModule(descriptor);
			}
		}

		public static void ReloadModule(Guid moduleTypeId)
		{
			IModuleDescriptor descriptor = Modules.GetDescriptorById(moduleTypeId);
			string moduleFilePath = descriptor.Assembly.Location;
			Modules.UnloadModule(descriptor);
			Modules.LoadModule(moduleFilePath, new[] {moduleTypeId});
		}

		/// <summary>
		/// Gets an instance of a module.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T Get<T>(Guid id)
			where T : class, IModuleInstance
		{
			// Must go through the module type manager, instead of using
			// Modules.GetById, so that the type manager can affect the instance.
			// Modules.ModuleManagement can be called when the name of the module
			// type is known, which it is not here.
			IModuleManagement moduleManager = Modules.GetManager<T>();
			if (moduleManager != null) {
				return moduleManager.Get(id) as T;
			}
			return null;
		}

		/// <summary>
		/// Gets an instance of each module of the type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T[] GetAll<T>()
			where T : class, IModuleInstance
		{
			// Must go through the module type manager so that it can affect the instance.
			IModuleManagement moduleManager = Modules.GetManager<T>();
			if (moduleManager != null) {
				return moduleManager.GetAll().Cast<T>().ToArray();
			}
			return null;
		}

		public static IEnumerable<IEffectEditorControl> GetEffectEditorControls(Guid effectId)
		{
			// Need the module-specific manager.
			EffectEditorModuleManagement manager =
				Modules.GetManager<IEffectEditorModuleInstance, EffectEditorModuleManagement>();
			return manager.GetEffectEditors(effectId);
		}

		public static INamingGenerator[] GetAllNamingGenerators()
		{
			return
				typeof(INamingGenerator).FindConcreteImplementationsWithin(Assembly.GetExecutingAssembly()).Select(
					Activator.CreateInstance).Cast<INamingGenerator>().ToArray();
		}

		public static INamingTemplate[] GetAllNamingTemplates()
		{
			return
				typeof(INamingTemplate).FindConcreteImplementationsWithin(Assembly.GetExecutingAssembly()).Select(
					Activator.CreateInstance).Cast<INamingTemplate>().ToArray();
		}

		public static IElementTemplate[] GetAllElementTemplates()
		{

			return AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(p => (typeof (IElementTemplate)).IsAssignableFrom(p) && !p.IsAbstract)
				.Select(Activator.CreateInstance)
				.Cast<IElementTemplate>()
				.ToArray();
		}

		public static IElementTemplate GetElementTemplate(string name)
		{
			return GetAllElementTemplates().FirstOrDefault(t => t.TemplateName.Equals(name));
		}

		public static IElementSetupHelper[] GetAllElementSetupHelpers()
		{

            return AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(p => (typeof(IElementSetupHelper)).IsAssignableFrom(p) && !p.IsAbstract)
				.Select(Activator.CreateInstance)
				.Cast<IElementSetupHelper>()
				.ToArray();
		}

		public static bool AreAllEffectRequiredPropertiesPresent(IEffectModuleInstance effectModule)
		{
			// This only happens if the effect is created outside of us.
			// In that case, it's not really a module instance and we can't check for required properties.
			// They're on their own as far as the outcome.
			if (effectModule.Descriptor != null) {
				EffectModuleDescriptorBase effectDescriptor =
					Modules.GetDescriptorById<EffectModuleDescriptorBase>(effectModule.Descriptor.TypeId);
				return
					effectModule.TargetNodes.All(
						x =>
						x.Properties.Select(y => y.Descriptor.TypeId).Intersect(effectDescriptor.PropertyDependencies).Count() ==
						effectDescriptor.PropertyDependencies.Length);
			}
			return true;
		}

		public static Program LoadProgram(string filePath)
		{
			return FileService.Instance.LoadProgramFile(filePath);
		}

		public static IModuleDataModel GetModuleStaticData(IModuleInstance instance)
		{
			return Modules.GetModuleStaticData(instance);
		}

		public static IModuleDataModel GetModuleStaticData(Guid id)
		{
			return Modules.GetModuleStaticData(id);
		}

	}
}