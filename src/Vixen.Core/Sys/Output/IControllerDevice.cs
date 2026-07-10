namespace Vixen.Sys.Output
{
	/// <summary>
	/// Core abstraction for the in-memory controller device.
	/// </summary>
	public interface IControllerDevice : IOutputDevice, IHasOutputs<CommandOutput>
	{
	}
}