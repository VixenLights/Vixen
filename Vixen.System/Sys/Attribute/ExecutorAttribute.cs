using System;

namespace Vixen.Sys.Attribute {
	[AttributeUsage(AttributeTargets.Class)]
	class ExecutorAttribute : System.Attribute {
		// Because generic attributes are not allowed...
		public ExecutorAttribute(Type executorType) {
			ExecutorType = executorType;
		}

		public Type ExecutorType { get; private set; }
	}
}
