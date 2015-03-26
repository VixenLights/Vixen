using Vixen.Sys.Attribute;

namespace Vixen.Module.Analysis
{
	[TypeOfModule("Analysis")]
	internal class AnalysisModuleImplementation : ModuleImplementation<IAnalysisModuleInstance>
	{
		public AnalysisModuleImplementation()
			: base(new AnalysisModuleManagement(), new AnalysisModuleRepository())
		{
		}
	}
}