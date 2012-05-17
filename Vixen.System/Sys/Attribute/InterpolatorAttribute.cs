using System;

namespace Vixen.Sys.Attribute {
	[AttributeUsage(AttributeTargets.Class)]
	class InterpolatorAttribute : System.Attribute {
		public InterpolatorAttribute(Type targetType) {
			TargetType = targetType;
		}

		public Type TargetType { get; private set; }
	}
}
