using System;
using Vixen.Module.SequenceType;
using Vixen.Services;

namespace Vixen.IO {
	class MigratorFactory : IMigratorFactory {
		static private MigratorFactory _instance;
		static private IMigratorFactory _factory;

		private MigratorFactory() { }

		public static MigratorFactory Instance {
			get { return _instance ?? (_instance = new MigratorFactory()); }
		}

		internal static IMigratorFactory Factory {
			get {
				if(_factory == null) throw new Exception("Migrator factory has not been set.");
				return _factory;
			}
			set { _factory = value; }
		}

		public IMigrator CreateSequenceMigrator(string fileType) {
			ISequenceTypeModuleInstance sequenceFactory = SequenceTypeService.Instance.CreateSequenceFactory(fileType);
			if(sequenceFactory != null) {
				return sequenceFactory.CreateMigrator();
			}
			return null;
		}

		public IMigrator CreateSystemConfigMigrator() {
			return Factory.CreateSystemConfigMigrator();
		}

		public IMigrator CreateModuleStoreMigrator() {
			return Factory.CreateModuleStoreMigrator();
		}

		public IMigrator CreateSystemContextMigrator() {
			return Factory.CreateSystemContextMigrator();
		}

		public IMigrator CreateProgramMigrator() {
			return Factory.CreateProgramMigrator();
		}

		public IMigrator CreateChannelNodeTemplateMigrator() {
			return Factory.CreateChannelNodeTemplateMigrator();
		}

		public IMigrator CreateOutputFilterTemplateMigrator() {
			return Factory.CreateOutputFilterTemplateMigrator();
		}
	}
}
