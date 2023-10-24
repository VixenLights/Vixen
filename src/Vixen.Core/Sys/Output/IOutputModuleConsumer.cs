using Vixen.Module;

namespace Vixen.Sys.Output
{
	internal interface IOutputModuleConsumer<out T> : IModuleConsumer<T>, IHardware, IHasSetup
		where T : class, IOutputModule
	{
		int UpdateInterval { get; }
		IOutputDeviceUpdateSignaler UpdateSignaler { get; }

		/// <summary>
		/// Controller modules should specify true if they want to set their own output names
		/// </summary>
		bool SupportsNamedOutputs { get; }

		/// <summary>
		/// Controller modules can override this to add their own naming to the outputs
		/// </summary>
		void NameOutputs();

	}
}