using System;
using Vixen.Module.MixingFilter;
using Vixen.Sys;

namespace Vixen.Services
{
	public class LayerMixingFilterService
	{
		private static LayerMixingFilterService _instance;

		private LayerMixingFilterService()
		{
		}

		public static LayerMixingFilterService Instance
		{
			get { return _instance ?? (_instance = new LayerMixingFilterService()); }
		}

		public ILayerMixingFilterInstance GetInstance(Guid id)
		{
			LayerMixingFilterModuleManagement moduleManagement =
				Modules.GetManager<ILayerMixingFilterInstance, LayerMixingFilterModuleManagement>();
			ILayerMixingFilterInstance module = moduleManagement.Get(id);
			
			return module;
		}
	}
}
