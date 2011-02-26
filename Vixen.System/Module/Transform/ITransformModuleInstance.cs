using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Transform {
	public interface ITransformModuleInstance : ITransform, IModuleInstance {
		/// <summary>
		/// This is useful for configuring a single instance of transform.
		/// </summary>
		void Setup();
	}
}
