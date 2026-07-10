using Vixen.Sys.Attribute;

namespace Vixen.Module.SequenceType
{
	[TypeOfModule("SequenceType")]
	internal class SequenceTypeModuleImplementation : ModuleImplementation<ISequenceTypeModuleInstance>
	{
		public SequenceTypeModuleImplementation()
			: base(new SequenceTypeModuleManagement(), new SequenceTypeModuleRepository())
		{
		}
	}
}