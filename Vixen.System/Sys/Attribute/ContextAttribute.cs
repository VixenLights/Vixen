using System;
using Vixen.Execution;

namespace Vixen.Sys.Attribute
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class ContextAttribute : System.Attribute
	{
		public ContextAttribute(ContextTargetType targetType, ContextCaching caching)
		{
			TargetType = targetType;
			Caching = caching;
		}

		public ContextTargetType TargetType { get; private set; }

		public ContextCaching Caching { get; private set; }
	}
}