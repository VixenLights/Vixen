namespace Vixen.Sys.Output
{
	/// <summary>
	/// Core abstraction for the in-memory smart controller device.
	/// </summary>
	public interface ISmartControllerDevice : IOutputDevice, IHasOutputs<IntentOutput>
	{
	}
}