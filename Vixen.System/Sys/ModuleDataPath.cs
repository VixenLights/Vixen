using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	/// <summary>
	/// The directory will be a child of ModuleData.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class ModuleDataPathAttribute : Attribute {
	}
}
