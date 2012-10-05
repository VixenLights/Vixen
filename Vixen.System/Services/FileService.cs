using System;
using System.IO;
using Vixen.IO;
using Vixen.IO.Factory;
using Vixen.Sys;

namespace Vixen.Services {
	class FileService : IFileService {
		static private FileService _instance;

		private FileService() {
		}

		static public FileService Instance {
			get { return _instance ?? (_instance = new FileService()); }
		}

		 public SystemConfig LoadSystemConfigFile(string filePath) {
			IObjectLoader<SystemConfig> loader = LoaderFactory.Instance.CreateSystemConfigLoader();
			return loader.LoadFromFile(filePath);
		}

		 public void SaveSystemConfigFile(SystemConfig systemConfig) {
			string filePath = systemConfig.LoadedFilePath ?? SystemConfig.DefaultFilePath;
			SaveSystemConfigFile(systemConfig, filePath);
		}

		 public void SaveSystemConfigFile(SystemConfig systemConfig, string filePath) {
			IObjectPersistor<SystemConfig> persistor = PersistorFactory.Instance.CreateSystemConfigPersistor();
			persistor.SaveToFile(systemConfig, filePath);
		}

		 public ModuleStore LoadModuleStoreFile(string filePath) {
			IObjectLoader<ModuleStore> loader = LoaderFactory.Instance.CreateModuleStoreLoader();
			return loader.LoadFromFile(filePath);
		}

		 public void SaveModuleStoreFile(ModuleStore moduleStore) {
			SaveModuleStoreFile(moduleStore, ModuleStore.DefaultFilePath);
		}

		 public void SaveModuleStoreFile(ModuleStore moduleStore, string filePath) {
			IObjectPersistor<ModuleStore> persistor = PersistorFactory.Instance.CreateModuleStorePersistor();
			persistor.SaveToFile(moduleStore, filePath);
		}

		 public SystemContext LoadSystemContextFile(string filePath) {
			IObjectLoader<SystemContext> loader = LoaderFactory.Instance.CreateSystemContextLoader();
			return loader.LoadFromFile(filePath);
		}

		 public void SaveSystemContextFile(SystemContext systemContext, string filePath) {
			IObjectPersistor<SystemContext> persistor = PersistorFactory.Instance.CreateSystemContextPersistor();
			persistor.SaveToFile(systemContext, filePath);
		}

		 public Program LoadProgramFile(string filePath) {
			IObjectLoader<Program> loader = LoaderFactory.Instance.CreateProgramLoader();
			return loader.LoadFromFile(filePath);
		}

		 public void SaveProgramFile(Program program, string filePath) {
			filePath = _GetRootedPath(filePath, Program.ProgramDirectory);
			filePath = Path.ChangeExtension(filePath, Program.Extension);

			IObjectPersistor<Program> persistor = PersistorFactory.Instance.CreateProgramPersistor();
			persistor.SaveToFile(program, filePath);
		}

		 public ChannelNodeTemplate LoadChannelNodeTemplateFile(string filePath) {
			IObjectLoader<ChannelNodeTemplate> loader = LoaderFactory.Instance.CreateChannelNodeTemplateLoader();
			return loader.LoadFromFile(filePath);
		}

		 public void SaveChannelNodeTemplateFile(ChannelNodeTemplate channelNodeTemplate, string filePath) {
			filePath = _GetRootedPath(filePath, ChannelNodeTemplate.Directory);
			filePath = Path.ChangeExtension(filePath, ChannelNodeTemplate.Extension);

			IObjectPersistor<ChannelNodeTemplate> persistor = PersistorFactory.Instance.CreateChannelNodeTemplatePersistor();
			persistor.SaveToFile(channelNodeTemplate, filePath);
		}

		// Sequences are not system-level files that we maintain, they are essentially nothing more than module
		// data for the sequence-type module.
		// However, it differs in that the data store is a file unto itself and that it's a user file that the system
		// needs to be able to load and save.
		 public ISequence LoadSequenceFile(string filePath) {
			IObjectLoader loader = LoaderFactory.Instance.CreateSequenceLoader();
			return (ISequence)loader.LoadFromFile(filePath);
		}

		 public void SaveSequenceFile(ISequence sequence, string filePath) {
			filePath = _GetRootedPath(filePath, SequenceService.SequenceDirectory);

			IObjectPersistor persistor = PersistorFactory.Instance.CreateSequencePersistor();
			persistor.SaveToFile(sequence, filePath);
		}

		 public string GetFileType(string filePath) {
			return Path.GetExtension(filePath);
		}

		 private string _GetRootedPath(string filePath, string defaultDirectory) {
			 if(filePath == null) throw new ArgumentNullException("filePath");

			if(!Path.IsPathRooted(filePath)) {
				filePath = Path.Combine(defaultDirectory, Path.GetFileName(filePath));
			}

			return filePath;
		}
	}
}
