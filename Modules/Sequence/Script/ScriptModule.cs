using Common.ScriptSequence;
using Vixen.Execution;
using Vixen.IO;
using Vixen.Module.SequenceType;
using Vixen.Sys;

namespace VixenModules.SequenceType.Script {
	public class ScriptModule : SequenceTypeModuleInstanceBase {
		public override ISequence CreateSequence() {
			return new ScriptSequenceType();
		}

		public override IContentMigrator CreateMigrator() {
			return new ScriptSequenceMigrator();
		}

		public override ISequenceExecutor CreateExecutor() {
			return new ScriptExecutor();
		}

		public override Vixen.Module.IModuleDataModel ModuleData {
			get {
				return base.ModuleData;
			}
			set {
				base.ModuleData = value;
				((ScriptData)value).SourceFileDirectory = ScriptDescriptor.ScriptSourceDirectory;
			}
		}
	}
}
