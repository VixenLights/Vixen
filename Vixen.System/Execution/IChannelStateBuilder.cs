using System;

namespace Vixen.Execution {
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Type of object contributing to a channel's state.</typeparam>
	/// <typeparam name="U">Type of object representing the channel's state.</typeparam>
	interface IChannelStateBuilder<T, U> {
		void Clear();
		void AddChannelState(Guid channelId, T state);
		U GetChannelState(Guid channelId);
	}
}
