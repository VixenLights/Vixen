namespace Vixen.Sys.Output {
	/// <summary>
	/// Core abstraction for an output-device module.
	/// </summary>
	public interface IOutputter : IHasSetup, IHardware {
		int OutputCount { get; set; }
		int UpdateInterval { get; }
		IOutputDeviceUpdateSignaler UpdateSignaler { get; }
	}
}
