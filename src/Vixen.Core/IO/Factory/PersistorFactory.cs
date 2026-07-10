using Vixen.IO.Persistor;
using Vixen.Sys;

namespace Vixen.IO.Factory
{
	internal class PersistorFactory : IPersistorFactory
	{
		private static PersistorFactory _instance;

		private PersistorFactory()
		{
		}

		public static PersistorFactory Instance
		{
			get { return _instance ?? (_instance = new PersistorFactory()); }
		}

		public IObjectPersistor<SystemConfig> CreateSystemConfigPersistor()
		{
			return new SystemConfigPersistor();
		}

		public IObjectPersistor<ModuleStore> CreateModuleStorePersistor()
		{
			return new ModuleStorePersistor();
		}

		public IObjectPersistor<SystemContext> CreateSystemContextPersistor()
		{
			return new SystemContextPersistor();
		}

		public IObjectPersistor<Program> CreateProgramPersistor()
		{
			return new ProgramPersistor();
		}

		public IObjectPersistor<ElementNodeTemplate> CreateElementNodeTemplatePersistor()
		{
			return new ElementNodeTemplatePersistor();
		}

		public IObjectPersistor CreateSequencePersistor()
		{
			return new SequencePersistor();
		}

		public IObjectPersistor CreateSequenceCachePersister()
		{
			return new SequenceCachePersister();
		}
	}
}