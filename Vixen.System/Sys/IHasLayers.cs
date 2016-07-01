using System.Collections.Generic;
using Vixen.Module.MixingFilter;
using Vixen.Sys.LayerMixing;

namespace Vixen.Sys
{
	public interface IHasLayers
	{
		IEnumerable<ILayer> GetAllLayers();

		SequenceLayers GetSequenceLayerManager();
	}
}
