using Vixen.IO;

namespace Vixen.Services {
	class FileService {
		static private FileService _instance;

		private FileService() { }

		public static FileService Instance {
			get { return _instance ?? (_instance = new FileService()); }
		}

		public VersionedFileSerializer CreateSequenceSerializer(string fileType) {
			IVersionedFileSerializer serializer = SerializerFactory.Instance.CreateSequenceSerializer(fileType);
			IMigrator migrator = MigratorFactory.Instance.CreateSequenceMigrator(fileType);
			return _CreateVersionedFileSerializer(migrator, serializer);
		}

		public VersionedFileSerializer CreateSystemConfigSerializer() {
			IVersionedFileSerializer serializer = SerializerFactory.Instance.CreateSystemConfigSerializer();
			IMigrator migrator = MigratorFactory.Instance.CreateSystemConfigMigrator();
			return _CreateVersionedFileSerializer(migrator, serializer);
		}

		public VersionedFileSerializer CreateModuleStoreSerializer() {
			IVersionedFileSerializer serializer = SerializerFactory.Instance.CreateModuleStoreSerializer();
			IMigrator migrator = MigratorFactory.Instance.CreateModuleStoreMigrator();
			return _CreateVersionedFileSerializer(migrator, serializer);
		}

		public VersionedFileSerializer CreateSystemContextSerializer() {
			IVersionedFileSerializer serializer = SerializerFactory.Instance.CreateSystemContextSerializer();
			IMigrator migrator = MigratorFactory.Instance.CreateSystemContextMigrator();
			return _CreateVersionedFileSerializer(migrator, serializer);
		}

		public VersionedFileSerializer CreateProgramSerializer() {
			IVersionedFileSerializer serializer = SerializerFactory.Instance.CreateProgramSerializer();
			IMigrator migrator = MigratorFactory.Instance.CreateProgramMigrator();
			return _CreateVersionedFileSerializer(migrator, serializer);
		}

		public VersionedFileSerializer CreateChannelNodeTemplateSerializer() {
			IVersionedFileSerializer serializer = SerializerFactory.Instance.CreateChannelNodeTemplateSerializer();
			IMigrator migrator = MigratorFactory.Instance.CreateChannelNodeTemplateMigrator();
			return _CreateVersionedFileSerializer(migrator, serializer);
		}

		public VersionedFileSerializer CreateOutputFilterTemplateSerializer() {
			IVersionedFileSerializer serializer = SerializerFactory.Instance.CreateOutputFilterTemplateSerializer();
			IMigrator migrator = MigratorFactory.Instance.CreateOutputFilterTemplateMigrator();
			return _CreateVersionedFileSerializer(migrator, serializer);
		}

		private static VersionedFileSerializer _CreateVersionedFileSerializer(IMigrator migrator, IVersionedFileSerializer serializer) {
			return new VersionedFileSerializer(serializer, migrator);
		}
	}
}
