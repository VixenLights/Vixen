using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Module.Preview {
	/// <summary>
	/// Core abstraction for the preview module.
	/// </summary>
	public interface IPreview {
		void UpdateState(ChannelCommands channelCommands);
		IDataPolicy DataPolicy { get; }
	}
}
