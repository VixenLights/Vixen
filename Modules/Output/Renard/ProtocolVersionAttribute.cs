using System;

namespace VixenModules.Output.Renard
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class ProtocolVersionAttribute : Attribute
	{
		public ProtocolVersionAttribute(int version)
		{
			Version = version;
		}

		public int Version { get; private set; }
	}
}