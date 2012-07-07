using Vixen.IO.Xml.Migrator;

namespace Vixen.IO.Factory {
	class XmlMigratorFactory : IMigratorFactory {
		public IMigrator CreateSystemConfigMigrator() {
			return new XmlSystemConfigMigrator();
		}

		public IMigrator CreateModuleStoreMigrator() {
			return new XmlModuleStoreMigrator();
		}

		public IMigrator CreateSystemContextMigrator() {
			return new XmlSystemContextMigrator();
		}

		public IMigrator CreateProgramMigrator() {
			return new XmlProgramMigrator();
		}

		public IMigrator CreateChannelNodeTemplateMigrator() {
			return new XmlChannelNodeTemplateMigrator();
		}

		public IMigrator CreateOutputFilterTemplateMigrator() {
			return new XmlOutputFilterTemplateMigrator();
		}
	}
}
