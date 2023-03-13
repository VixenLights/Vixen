using Vixen.Sys.Attribute;

namespace Vixen.Module.Effect
{
	[TypeOfModule("Effect")]
	internal class EffectModuleImplementation : ModuleImplementation<IEffectModuleInstance>
	{
		public EffectModuleImplementation()
			: base(new EffectModuleManagement(), new EffectModuleRepository())
		{
		}
	}
}