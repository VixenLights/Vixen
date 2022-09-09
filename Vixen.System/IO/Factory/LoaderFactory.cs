﻿using Vixen.IO.Loader;
using Vixen.Sys;

namespace Vixen.IO.Factory
{
	internal class LoaderFactory : ILoaderFactory
	{
		private static LoaderFactory _instance;

		private LoaderFactory()
		{
		}

		public static LoaderFactory Instance
		{
			get { return _instance ?? (_instance = new LoaderFactory()); }
		}

		public IObjectLoader<SystemConfig> CreateSystemConfigLoader()
		{
			return new SystemConfigLoader();
		}

		public IObjectLoader<ModuleStore> CreateModuleStoreLoader()
		{
			return new ModuleStoreLoader();
		}

		public IObjectLoader<SystemContext> CreateSystemContextLoader()
		{
			return new SystemContextLoader();
		}

		public IObjectLoader<Program> CreateProgramLoader()
		{
			return new ProgramLoader();
		}

		public IObjectLoader<ElementNodeTemplate> CreateElementNodeTemplateLoader()
		{
			return new ElementNodeTemplateLoader();
		}

		public IObjectLoader CreateSequenceLoader()
		{
			return new SequenceLoader();
		}

		public IObjectLoader CreateSequenceCacheLoader()
		{
			return new SequenceCacheLoader();
		}

		public IObjectLoader<T> CreateFixtureSpecificationLoader<T>() where T : class, IDataModel, new()
		{
			return new FixtureSpecificationLoader<T>();	
		}
	}
}