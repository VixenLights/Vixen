using System.Collections.Generic;
using Vixen.Module.PostFilter;

namespace Vixen.Sys {
	public class PostFilterCollection : List<IPostFilterModuleInstance> {
		public PostFilterCollection() {
		}

		public PostFilterCollection(IEnumerable<IPostFilterModuleInstance> items)
			: base(items) {
		}
	}
}
