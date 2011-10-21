using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Commands;

namespace Vixen.Execution {
	public class ChannelManager : IEnumerable<Channel> {
		private Dictionary<Channel, SystemChannelEnumerator> _channels = new Dictionary<Channel, SystemChannelEnumerator>();

		public ChannelManager() {
		}

		public ChannelManager(IEnumerable<Channel> channels)
			: this() {
			AddChannels(channels);
		}

		public Channel AddChannel(string channelName) {
			channelName = _Uniquify(channelName);
			Channel channel = new Channel(channelName);
			AddChannel(channel);
			return channel;
		}

		public void AddChannel(Channel channel) {
			// Create an enumerator.
			_CreateChannelEnumerators(channel);
		}

		public void AddChannels(IEnumerable<Channel> channels) {
			foreach(Channel channel in channels) {
				AddChannel(channel);
			}
		}

		public void RemoveChannel(Channel channel) {
			SystemChannelEnumerator enumerator;
			if(_channels.TryGetValue(channel, out enumerator)) {
				lock(_channels) {
					// Kill enumerator.
					enumerator.Dispose();
					// Remove from channel dictionary.
					_channels.Remove(channel);
					// Can't have any nodes reference this channel.
					VixenSystem.Nodes.RemoveChannelLeaf(channel);
				}
			}
		}

		public void OpenChannels() {
			_CreateChannelEnumerators(_channels.Keys);
		}

		public void CloseChannels() {
			_ResetChannelEnumerators();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="channel"></param>
		/// <returns>Channel's new state, null if no state change.</returns>
		public Command UpdateChannelState(Channel channel) {
			IEnumerator<CommandNode[]> enumerator;

			lock(_channels) {
				// we potentially might be trying to update a channel one last time just after it's been
				// deleted (since the _UpdateChannelStates in Execution iterates over a copy of these
				// channels). If so, just ignore it if we can't find this channel. This is probably the
				// best performing solution that doesn't need locking around big chunks of code.
				if (!_channels.ContainsKey(channel))
					return null;
				enumerator = _channels[channel];
				// Will return true if state has changed.
				// State changes when data qualifies for execution.
				if(enumerator.MoveNext()) {
					Command channelState = Command.Combine(enumerator.Current.Select(x => x.Command));
					channel.Patch.Write(channelState);
					return channelState;
				}
				return null;
			}
		}

		private void _CreateChannelEnumerators(params Channel[] channels) {
			_CreateChannelEnumerators(channels as IEnumerable<Channel>);
		}

		private void _CreateChannelEnumerators(IEnumerable<Channel> channels) {
			lock(_channels) {
				foreach(Channel channel in channels) {
					if(!_channels.ContainsKey(channel) || _channels[channel] == null) {
						_channels[channel] = new SystemChannelEnumerator(channel, Vixen.Sys.Execution.SystemTime);
					}
				}
			}
		}

		private void _ResetChannelEnumerators() {
			lock(_channels) {
				foreach(Channel channel in _channels.Keys.ToArray()) {
					_channels[channel].Dispose();
					_channels[channel] = null;
				}
			}
		}

		private string _Uniquify(string name) {
			if(_channels.Keys.Any(x => x.Name == name)) {
				string originalName = name;
				bool unique;
				int counter = 2;
				do {
					name = originalName + "-" + counter++;
					unique = !_channels.Keys.Any(x => x.Name == name);
				} while(!unique);
			}
			return name;
		}

		public IEnumerator<Channel> GetEnumerator() {
			// Enumerate against a copy of the collection.
			return _channels.Keys.ToList().GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
