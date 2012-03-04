using System;

namespace Vixen.Sys.Attribute {
    /// <summary>
    /// Decorate properties and fields that return data directories that
    /// need to be present.
    /// The property or field must be static; public or private visibility is allowed.
	/// The directory will be a child of the Vixen data directory.
	/// This is for system code only.  Not exposed for modules.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    class DataPathAttribute : System.Attribute {
    }
}
