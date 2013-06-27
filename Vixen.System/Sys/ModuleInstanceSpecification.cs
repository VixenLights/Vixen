using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys
{
	public class ModuleInstanceSpecification<T> : Dictionary<T, HashSet<Tuple<Guid, Guid>>>
	{
		public void Add(T key, Guid moduleTypeId, Guid moduleInstanceId)
		{
			Tuple<Guid, Guid> instanceSpec = new Tuple<Guid, Guid>(moduleTypeId, moduleInstanceId);
			HashSet<Tuple<Guid, Guid>> instanceSpecs;
			if (!TryGetValue(key, out instanceSpecs)) {
				instanceSpecs = new HashSet<Tuple<Guid, Guid>>();
				Add(key, instanceSpecs);
			}
			instanceSpecs.Add(instanceSpec);
		}
	}
}