using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module {
    /// <summary>
    /// The descriptor for the module within the assembly.
    /// </summary>
    public interface IModuleDescriptor {
        Guid TypeId { get; }
		/// <summary>
		/// Type of the module class that this descriptor describes.  In other
		/// words, the module class that is associated with this descriptor.
		/// </summary>
        Type ModuleClass { get; }
		/// <summary>
		/// Type of the module data class associated with instances of
		/// this module type.
		/// </summary>
		Type ModuleDataClass { get; }
        string Author { get; }
        string TypeName { get; }
        string Description { get; }
        string Version { get; }
		string FileName { get; set; } // Set by the application.
		string ModuleTypeName { get; set; } // Set when the module is loaded; matches ModuleTypeAttribute constructor parameter.
    }
}
