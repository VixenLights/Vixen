using Vixen.Sys;

namespace Vixen.Module.Preview
{
	/// <summary>
	/// Core abstraction for the preview module.
	/// </summary>
	public interface IPreview
	{
		void UpdateState();

		void PlayerStarted();

		void PlayerEnded();
	}
}