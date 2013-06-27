using System.Collections.Generic;
using Vixen.Module.OutputFilter;

namespace Vixen.Sys
{
	public class OutputFilterCollection : List<IOutputFilterModuleInstance>
	{
		public OutputFilterCollection()
		{
		}

		public OutputFilterCollection(IEnumerable<IOutputFilterModuleInstance> items)
			: base(items)
		{
		}
	}
}