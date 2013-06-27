using System;
using Vixen.Sys.Output;

namespace Vixen.Factory
{
	internal class IntentOutputFactory : IOutputFactory
	{
		public Output CreateOutput(string name, int index)
		{
			return CreateOutput(Guid.NewGuid(), name, index);
		}

		public Output CreateOutput(Guid id, string name, int index)
		{
			return new IntentOutput(id, name, index);
		}
	}
}