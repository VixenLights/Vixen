namespace Vixen.Sys.Output
{
	/// <summary>
	/// Core abstraction for an output-device module.
	/// </summary>
	public interface IOutputter : IHasSetup, IHardware
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