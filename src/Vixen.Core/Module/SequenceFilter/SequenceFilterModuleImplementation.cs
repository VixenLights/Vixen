using Vixen.Sys.Attribute;

namespace Vixen.Module.SequenceFilter
{
	[TypeOfModule("SequenceFilter")]
	internal class SequenceFilterModuleImplementation : ModuleImplementation<ISequenceFilterModuleInstance>
	{
		public SequenceFilterModuleImplementation()
			: base(new SequenceFilterModuleManagement(), new SequenceFilterModuleRepository())
		{
		}
	}
}