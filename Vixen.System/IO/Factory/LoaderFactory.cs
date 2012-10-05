using Vixen.IO.Loader;
using Vixen.Sys;

namespace Vixen.IO.Factory {
	class LoaderFactory : ILoaderFactory {
		static private LoaderFactory _instance;

		private LoaderFactory() {
		}

		public static LoaderFactory Instance {
			get { return _instance ?? (_instance = new LoaderFactory()); }
		}

		public IObjectLoader<SystemConfig> CreateSystemConfigLoader() {
			return new SystemConfigLoader();
		}

		public IObjectLoader<ModuleStore> CreateModuleStoreLoader() {
			return new ModuleStoreLoader();
		}

		public IObjectLoader<SystemContext> CreateSystemContextLoader() {
			return new SystemContextLoader();
		}

		public IObjectLoader<Program> CreateProgramLoader() {
			return new ProgramLoader();
		}

		public IObjectLoader<ChannelNodeTemplate> CreateChannelNodeTemplateLoader() {
			return new ChannelNodeTemplateLoader();
		}

		public IObjectLoader CreateSequenceLoader() {
			return new SequenceLoader();
		}
	}
}
