using System;
using Vixen.Sys.Output;

namespace Vixen.Factory {
	interface IOutputFactory {
		Output CreateOutput(string name);
		Output CreateOutput(Guid id, string name);
	}
}
