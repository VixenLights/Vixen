using Vixen.Sys.Attribute;

namespace Vixen.Execution.Context {
	[Context(ContextTargetType.Program, ContextCaching.SequenceLevelCaching)]
	public class SequenceCachingProgramContext : CachingProgramContext {
		public SequenceCachingProgramContext()
			: base(CachingLevel.Sequence) {
		}
	}
}
