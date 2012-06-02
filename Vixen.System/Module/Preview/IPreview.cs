using Vixen.Commands;
using Vixen.Sys;
using Vixen.Sys.Output;

// Controllers implement the idea of outputs because they model a hardware controller that has hardware outputs.
// Previews have no such real-life model and therefore don't implement outputs or chaining.

namespace Vixen.Module.Preview {
    public interface IPreview : IOutputModule {
		void UpdateState(ChannelCommands channelCommands);
		IDataPolicy DataPolicy { get; }
	}
}
