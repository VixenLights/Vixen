using System;

namespace Vixen.Module
{
	internal interface IModuleConsumer<out T>
		where T : class, IModuleInstance
	{
		Guid ModuleId { get; }
		Guid ModuleInstanceId { get; }
		T Module { get; }
	}
}