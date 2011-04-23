using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.Media {
	[ModuleType("Media")]
	class MediaModuleImplementation : ModuleImplementation<IMediaModuleInstance> {
		public MediaModuleImplementation()
			: base(new MediaModuleType(), new MediaModuleManagement(), new MediaModuleRepository()) {
		}
	}
}
