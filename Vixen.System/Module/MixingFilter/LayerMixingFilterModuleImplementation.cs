using Vixen.Sys.Attribute;

namespace Vixen.Module.MixingFilter
{
	[TypeOfModule("LayerCombiningFilter")]
	internal class LayerMixingFilterModuleImplementation: ModuleImplementation<ILayerMixingFilterInstance>
	{
		public LayerMixingFilterModuleImplementation() 
			: base(new LayerMixingFilterModuleManagement(), new LayerMixingFilterModuleRepository())
		{
		}
	}
}
