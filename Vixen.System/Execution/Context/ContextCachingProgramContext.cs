using Vixen.Sys.Attribute;

namespace Vixen.Execution.Context
{
	[Context(ContextTargetType.Program, ContextCaching.ContextLevelCaching)]
	public class ContextCachingProgramContext : CachingProgramContext
	{
		public ContextCachingProgramContext()
			: base(CachingLevel.Context)
		{
		}
	}
}