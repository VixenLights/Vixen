using System;
using Vixen.Sys.Output;

namespace Vixen.Factory {
	class CommandOutputFactory : IOutputFactory {
		public Output CreateOutput(string name) {
			return CreateOutput(Guid.NewGuid(), name);
		}

		public Output CreateOutput(Guid id, string name) {
			return new CommandOutput(id, name);
		}
	}
}
