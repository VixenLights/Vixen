using System;

namespace Vixen.Module {
	interface IModuleConsumer {
		Guid ModuleId { get; }
		IModuleInstance Module { get; }
	}
}
