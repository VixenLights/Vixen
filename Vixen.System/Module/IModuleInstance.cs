using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module {
    public interface IModuleInstance : IDisposable {
		/// <summary>
		/// System-supplied id of the instance of the module.
		/// </summary>
        Guid InstanceId { get; set; }
		/// <summary>
		/// Module's data model object, set by the system.
		/// </summary>
		IModuleDataModel ModuleData { get; set; }
		/// <summary>
		/// System-supplied reference to the descriptor for this instance.
		/// </summary>
		IModuleDescriptor Descriptor { get; set; }
	}
}
