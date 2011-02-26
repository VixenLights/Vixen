using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module {
    public interface IModuleInstance : IDisposable {
        Guid TypeId { get; }
        Guid InstanceId { get; set; }
		/// <summary>
		/// Module's data model object, set by the system.
		/// </summary>
		IModuleDataModel ModuleData { get; set; }
		string TypeName { get; set; }
	}
}
