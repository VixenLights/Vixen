namespace Vixen.IO {
	interface IMigratorFactory {
		IMigrator CreateSystemConfigMigrator();
		IMigrator CreateModuleStoreMigrator();
		IMigrator CreateSystemContextMigrator();
		IMigrator CreateProgramMigrator();
		IMigrator CreateChannelNodeTemplateMigrator();
		IMigrator CreateOutputFilterTemplateMigrator();
	}
}
