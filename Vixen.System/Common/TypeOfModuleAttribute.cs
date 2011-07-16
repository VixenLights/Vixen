using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Common {
    // Default of:
    // AllowMultiple - false
    // Inherited - true
    [AttributeUsage(AttributeTargets.Class)]
    class TypeOfModuleAttribute : Attribute {
		public TypeOfModuleAttribute(string moduleTypeName) {
			Name = moduleTypeName;
		}

		public string Name { get; private set; }
    }
}
