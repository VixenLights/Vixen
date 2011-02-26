using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Hardware {
	public interface IControllerHardwareModule : IHardwareModule {
		void Initialize(int startTime);
		/// <summary>
		/// Allows the module author to specify whether or not the controller will initially
		/// be a singleton.  The user can override this behavior.
		/// </summary>
		bool DefaultAsSingleton { get; }
	}
}
