using Vixen.Sys;

namespace Vixen.IO.Factory {
	interface IPersistorFactory {
		IObjectPersistor<SystemConfig> CreateSystemConfigPersistor();
		IObjectPersistor<ModuleStore> CreateModuleStorePersistor();
		IObjectPersistor<SystemContext> CreateSystemContextPersistor();
		IObjectPersistor<Program> CreateProgramPersistor();
		IObjectPersistor<ChannelNodeTemplate> CreateChannelNodeTemplatePersistor();
		IObjectPersistor CreateSequencePersistor();
	}
}
