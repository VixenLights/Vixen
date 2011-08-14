using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using System.IO;
using System.Xml.Linq;
using Vixen.Common;

namespace Vixen.Module.ModuleTemplate {
	class ModuleTemplateModuleManagement : IModuleManagement<IModuleTemplateModuleInstance> {
		//private const string DIRECTORY_NAME = "Template";
		//private const string FILE_NAME_ROOT = "TemplateData";

		//[DataPath]
		//private static string _directory = System.IO.Path.Combine(Paths.DataRootPath, DIRECTORY_NAME);

		public IModuleTemplateModuleInstance Get(Guid id) {
			// Template modules are singletons.
			// The repository assumes to instantiate the template's data.
			return Modules.ModuleRepository.GetModuleTemplate(id);
		}

		object IModuleManagement.Get(Guid id) {
			return Get(id);
		}

		public IModuleTemplateModuleInstance[] GetAll() {
			return Modules.ModuleRepository.GetAllModuleTemplate();
		}

		object[] IModuleManagement.GetAll() {
			return GetAll();
		}

		public IModuleTemplateModuleInstance Clone(IModuleTemplateModuleInstance instance) {
			// These are singletons.
			return null;
		}

		object IModuleManagement.Clone(object instance) {
			return Clone(instance as IModuleTemplateModuleInstance);
		}

		//public void ProjectTemplateInto(string fileType, object target) {
		//    // Get all file template module descriptors.
		//    IEnumerable<IModuleTemplateModuleDescriptor> fileTemplateDescriptors = Modules.GetModuleDescriptors<IModuleTemplateModuleInstance, IModuleTemplateModuleDescriptor>();
		//    // Find the one for the file type.
		//    IModuleTemplateModuleDescriptor descriptor = fileTemplateDescriptors.FirstOrDefault(x => x.FileType.Equals(fileType, StringComparison.OrdinalIgnoreCase));
		//    if(descriptor != null) {
		//        // Get an instance of the module.
		//        IModuleTemplateModuleInstance instance = Get(descriptor.TypeId);
		//        // Project the template into the target instance.
		//        instance.Project(target);
		//    }
		//}
		public void ProjectTemplateInto(IModuleInstance target) {
			// Get all template module descriptors.
			IEnumerable<IModuleTemplateModuleDescriptor> templateDescriptors = Modules.GetModuleDescriptors<IModuleTemplateModuleInstance, IModuleTemplateModuleDescriptor>();
			// Find the one for the module type.
			// (i.e. Has the module type as a dependency.)
			IModuleTemplateModuleDescriptor descriptor = templateDescriptors.FirstOrDefault(x => x.Dependencies.Contains(target.Descriptor.TypeId));
			if(descriptor != null) {
				// Get an instance of the template module.
				IModuleTemplateModuleInstance instance = Get(descriptor.TypeId);
				// Project the template into the target instance.
				instance.Project(target);
			}
		}

		//public void LoadTemplateData(IModuleTemplateModuleInstance instance) {
		//    // Does a file exist?
		//    string fileName = _GetTemplateDataFileName(instance);
		//    if(!File.Exists(fileName)) {
		//        // Create a file.
		//        ModuleDataSet.CreateEmptyDataSetFile(fileName);
		//    }
		//    // Load the template data from the file.
		//    ModuleDataSet dataSet = new ModuleDataSet();
		//    dataSet.Deserialize(Helper.LoadXml(fileName).ToString());
		//    dataSet.GetModuleTypeData(instance);
		//}

		//public void SaveTemplateData(IModuleTemplateModuleInstance instance) {
		//    string fileName = _GetTemplateDataFileName(instance);
		//    string xmlData = instance.ModuleData.ModuleDataSet.Serialize();
		//    File.WriteAllText(fileName, xmlData);
		//}

		//private string _GetTemplateDataFileName(IModuleTemplateModuleInstance instance) {
		//    return Path.Combine(_directory, Path.ChangeExtension(FILE_NAME_ROOT, Modules.GetDescriptorById<IModuleTemplateModuleDescriptor>(instance.Descriptor.TypeId).FileType));
		//}
	}
}
