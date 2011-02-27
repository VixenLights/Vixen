using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Common {
	[AttributeUsage(AttributeTargets.Class)]
	class ExecutorAttribute : Attribute {
		// Because generic attributes are not allowed...
		public ExecutorAttribute(Type executorType) {
			ExecutorType = executorType;
		}

		public Type ExecutorType { get; private set; }
	}
}
