using System;

namespace Vixen.Sys.Attribute {
	/// <summary>
	/// Allows module descriptors to specify a directory the system will create under Module Data Files.
	/// This must be applied to a property or field that is not read-only.
	/// The absolute path created will be written back to the member.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class ModuleDataPathAttribute : System.Attribute {
	}
}
