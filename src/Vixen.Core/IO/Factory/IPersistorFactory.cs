using Vixen.Sys;

namespace Vixen.IO.Factory
{
	internal interface IPersistorFactory
	{
		IObjectPersistor<SystemConfig> CreateSystemConfigPersistor();
		IObjectPersistor<ModuleStore> CreateModuleStorePersistor();
		IObjectPersistor<SystemContext> CreateSystemContextPersistor();
		IObjectPersistor<Program> CreateProgramPersistor();
		IObjectPersistor<ElementNodeTemplate> CreateElementNodeTemplatePersistor();
		IObjectPersistor CreateSequencePersistor();
		IObjectPersistor CreateSequenceCachePersister();
	}
}