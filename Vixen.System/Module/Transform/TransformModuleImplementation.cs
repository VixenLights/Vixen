using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.Transform {
	[ModuleType("Transform")]
	class TransformModuleImplementation : ModuleImplementation<ITransformModuleInstance> {
		public TransformModuleImplementation()
			: base(new TransformModuleType(), new TransformModuleManagement(), new TransformModuleRepository()) {
		}
	}
}
