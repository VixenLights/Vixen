using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using System.IO;
using System.Xml.Linq;
using Vixen.Common;

namespace Vixen.Module.FileTemplate {
	class FileTemplateModuleManagement : IModuleManagement<IFileTemplateModuleInstance> {
		private const string DIRECTORY_NAME = "Template";
		private const string FILE_NAME_ROOT = "TemplateData";

		[DataPath]
		private static string _directory = System.IO.Path.Combine(Paths.DataRootPath, DIRECTORY_NAME);

		public IFileTemplateModuleInstance Get(Guid id) {
			// Template modules are singletons.
			// The repository assumes to instantiate the template's data.
			return Server.ModuleRepository.GetFileTemplate(id);
		}

		object IModuleManagement.Get(Guid id) {
			return Get(id);
		}

		public IFileTemplateModuleInstance[] GetAll() {
			return Server.ModuleRepository.GetAllFileTemplate();
		}

		object[] IModuleManagement.GetAll() {
			return GetAll();
		}

		public IFileTemplateModuleInstance Clone(IFileTemplateModuleInstance instance) {
			// These are singletons.
			return null;
		}

		object IModuleManagement.Clone(object instance) {
			return Clone(instance as IFileTemplateModuleInstance);
		}

		public void ProjectTemplateInto(string fileType, object target) {
			// Get the descriptor based on the file type.
			IFileTemplateModuleDescriptor descriptor = Modules.GetModuleDescriptors("FileTemplate").Cast<IFileTemplateModuleDescriptor>().FirstOrDefault(x => x.FileType.Equals(fileType, StringComparison.OrdinalIgnoreCase));
			if(descriptor != null) {
				// Get an instance of the module.
				IFileTemplateModuleInstance instance = Get(descriptor.TypeId);
				// Project the template into the target instance.
				instance.Project(target);
			}
		}

		public void LoadTemplateData(IFileTemplateModuleInstance instance) {
			// Does a file exist?
			string fileName = _GetTemplateDataFileName(instance);
			if(!File.Exists(fileName)) {
				// Create a file.
				ModuleDataSet.CreateEmptyDataSetFile(fileName);
			}
			// Load the template data from the file.
			ModuleDataSet dataSet = new ModuleDataSet();
			dataSet.Deserialize(Helper.LoadXml(fileName).ToString());
			dataSet.GetModuleTypeData(instance);
		}

		public void SaveTemplateData(IFileTemplateModuleInstance instance) {
			string fileName = _GetTemplateDataFileName(instance);
			string xmlData = instance.ModuleData.ModuleDataSet.Serialize();
			File.WriteAllText(fileName, xmlData);
		}

		private string _GetTemplateDataFileName(IFileTemplateModuleInstance instance) {
			return Path.Combine(_directory, Path.ChangeExtension(FILE_NAME_ROOT, Modules.GetDescriptorById<IFileTemplateModuleDescriptor>(instance.TypeId).FileType));
		}

	}
}
