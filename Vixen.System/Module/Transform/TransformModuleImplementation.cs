using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Transform {
	[TypeOfModule("Transform")]
	class TransformModuleImplementation : ModuleImplementation<ITransformModuleInstance> {
		public TransformModuleImplementation()
			: base(new TransformModuleManagement(), new TransformModuleRepository()) {
		}
	}
}
