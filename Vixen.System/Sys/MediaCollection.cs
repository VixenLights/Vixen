using System.Collections.Generic;
using Vixen.Module.Media;

namespace Vixen.Sys
{
	public class MediaCollection : List<IMediaModuleInstance>
	{
		public MediaCollection()
		{
		}

		public MediaCollection(IEnumerable<IMediaModuleInstance> modules)
			: base(modules)
		{
		}
	}
}