namespace Vixen.Module.Controller
{
	/// <summary>
	/// Interface for sending raw byte channel data to a controller.
	/// </summary>
	public interface ISimpleController
	{
		/// <summary>
		/// Updates the output state of the controller with the specified byte array.
		/// </summary>
		/// <param name="outputStates">Output state byte array to send to the controller hardware</param>
		void UpdateState(byte[] outputStates);
	}
}
