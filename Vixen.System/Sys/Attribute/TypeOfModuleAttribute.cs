using System;

namespace Vixen.Sys.Attribute
{
	// Default of:
	// AllowMultiple - false
	// Inherited - true
	[AttributeUsage(AttributeTargets.Class)]
	internal class TypeOfModuleAttribute : System.Attribute
	{
		public TypeOfModuleAttribute(string moduleTypeName)
		{
			Name = moduleTypeName;
		}

		public string Name { get; private set; }
	}
}