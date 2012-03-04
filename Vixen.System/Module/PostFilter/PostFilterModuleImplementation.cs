using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys.Attribute;

namespace Vixen.Module.PostFilter {
	[TypeOfModule("PostFilter")]
	class PostFilterModuleImplementation : ModuleImplementation<IPostFilterModuleInstance> {
		public PostFilterModuleImplementation()
			: base(new PostFilterModuleManagement(), new PostFilterModuleRepository()) {
		}
	}
}
