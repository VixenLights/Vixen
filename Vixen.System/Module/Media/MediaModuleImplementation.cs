using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Media {
	[TypeOfModule("Media")]
	class MediaModuleImplementation : ModuleImplementation<IMediaModuleInstance> {
		public MediaModuleImplementation()
			: base(new MediaModuleManagement(), new MediaModuleRepository()) {
		}
	}
}
