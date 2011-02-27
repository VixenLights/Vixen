using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Common {
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class ModuleDataPathAttribute : Attribute {
	}
}
