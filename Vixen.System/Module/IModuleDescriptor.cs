using System;
using System.Reflection;

namespace Vixen.Module {
    /// <summary>
    /// The descriptor for the module within the assembly.
    /// </summary>
    public interface IModuleDescriptor {
		string TypeName { get; }
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
		/// <summary>
		/// Type of the module static data class associated with all instances of
		/// this module type.
		/// </summary>
		Type ModuleStaticDataClass { get; }
		string Author { get; }
        string Description { get; }
        string Version { get; }
		/// <summary>
		/// The file that the module was borne from.  Set by the system.
		/// </summary>
		string FileName { get; set; }
		/// <summary>
		/// The file that the module was borne from.  Set by the system.
		/// </summary>
		Assembly Assembly { get; set; }
		/// <summary>
		/// TypeIds of modules that this module is dependent upon.
		/// </summary>
		Guid[] Dependencies { get; }
	}
}
