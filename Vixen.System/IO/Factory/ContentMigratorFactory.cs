using System;
using Vixen.IO.Xml.ElementNodeTemplate;
using Vixen.IO.Xml.ModuleStore;
using Vixen.IO.Xml.Program;
using Vixen.IO.Xml.SystemConfig;
using Vixen.IO.Xml.SystemContext;
using Vixen.Module.SequenceType;
using Vixen.Services;

namespace Vixen.IO.Factory
{
	// None of what form the data takes should be exposed outside of this i/o subsystem.  The factories
	// can expose the specific type, provided it's specifically for that type, but XElement
	// should never make it outside this subsystem, much less to the user.
	internal class ContentMigratorFactory : IContentMigratorFactory
	{
		private static ContentMigratorFactory _instance;

		private ContentMigratorFactory()
		{
		}

		public static ContentMigratorFactory Instance
		{
			get { return _instance ?? (_instance = new ContentMigratorFactory()); }
		}

		public IContentMigrator CreateSystemConfigContentMigrator()
		{
			return new SystemConfigXElementMigrator();
		}

		public IContentMigrator CreateModuleStoreContentMigrator()
		{
			return new ModuleStoreXElementMigrator();
		}

		public IContentMigrator CreateSystemContextContentMigrator()
		{
			return new SystemContextXElementMigrator();
		}

		public IContentMigrator CreateProgramContentMigrator()
		{
			return new ProgramXElementMigrator();
		}

		public IContentMigrator CreateElementNodeTemplateContentMigrator()
		{
			return new ElementNodeTemplateXElementMigrator();
		}

		public IContentMigrator CreateSequenceContentMigrator(string filePath)
		{
			ISequenceTypeModuleInstance sequenceTypeModule = SequenceTypeService.Instance.CreateSequenceFactory(filePath);
			if (sequenceTypeModule == null) {
				return null;
			}
			return sequenceTypeModule.CreateMigrator();
		}
	}
}