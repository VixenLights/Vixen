using Vixen.Execution;
using Vixen.IO;
using Vixen.Module.SequenceType;
using Vixen.Sys;
using Vixen.Module;

namespace VixenModules.Sequence.Vixen2x {
	public class Vixen2xSequenceTypeModule : SequenceTypeModuleInstanceBase {
		public override ISequence CreateSequence() {
			return new Vixen2xSequence();
		}

		public override IContentMigrator CreateMigrator() {
			return new SequenceMigrator();
		}

		public override ISequenceExecutor CreateExecutor() {
			return new Executor();
		}
	}
}
