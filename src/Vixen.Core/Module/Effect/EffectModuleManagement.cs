using Vixen.Sys;

namespace Vixen.Module.Effect
{
	internal class EffectModuleManagement : GenericModuleManagement<IEffectModuleInstance>
	{
		public IEffectModuleInstance Get(string effectName)
		{
			// Need the type-specific repository.
			EffectModuleRepository repository = Modules.GetRepository<IEffectModuleInstance, EffectModuleRepository>();
			IEffectModuleInstance instance = repository.Get(effectName);
			return instance;
		}
	}
}