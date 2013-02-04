using Vixen.Sys;

namespace Vixen.Services {
	interface IFileService {
		SystemConfig LoadSystemConfigFile(string filePath);
		void SaveSystemConfigFile(SystemConfig systemConfig);
		void SaveSystemConfigFile(SystemConfig systemConfig, string filePath);
		ModuleStore LoadModuleStoreFile(string filePath);
		void SaveModuleStoreFile(ModuleStore moduleStore);
		void SaveModuleStoreFile(ModuleStore moduleStore, string filePath);
		SystemContext LoadSystemContextFile(string filePath);
		void SaveSystemContextFile(SystemContext systemContext, string filePath);
		Program LoadProgramFile(string filePath);
		void SaveProgramFile(Program program, string filePath);
		ElementNodeTemplate LoadElementNodeTemplateFile(string filePath);
		void SaveElementNodeTemplateFile(ElementNodeTemplate elementNodeTemplate, string filePath);
		ISequence LoadSequenceFile(string filePath);
		void SaveSequenceFile(ISequence sequence, string filePath);
	}
}
