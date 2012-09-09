using System;

namespace Vixen.Module {
	interface IModuleConsumer<out T>
		where T : class, IModuleInstance {
		Guid ModuleId { get; }
		T Module { get; }
	}
}
