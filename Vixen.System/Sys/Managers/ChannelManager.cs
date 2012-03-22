using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vixen.Sys.Instrumentation;
using Vixen.Sys.SourceCollection;

namespace Vixen.Sys.Managers {
	public class ChannelManager : IEnumerable<Channel> {
		//private Dictionary<Channel, IEnumerator<CommandNode[]>> _channelEnumerators;
		private ChannelUpdateTimeValue _channelUpdateTimeValue;
		private Stopwatch _stopwatch;

		// a mapping of channel  GUIDs to channel instances. Used for quick reverse mapping at runtime.
		private Dictionary<Guid, Channel> _instances;

		// a mapping of channels back to their containing channel nodes. Used in a few special cases, particularly for runtime, so we can
		// quickly and easily find the node that a particular channel references (eg. if we're previewing the rendered data on a virtual display,
		// or anything else where we need to actually 'reverse' the rendering process).
		private Dictionary<Channel, ChannelNode> _channelToChannelNode;
		//private Type _enumeratorChannelsOpenedWith;

		public ChannelManager() {
			_instances = new Dictionary<Guid,Channel>();
			_channelToChannelNode = new Dictionary<Channel, ChannelNode>();
			//_channelEnumerators = new Dictionary<Channel, IEnumerator<CommandNode[]>>();
			_SetupInstrumentation();
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
			//_CreateChannelEnumerators(channel);
			if (_instances.ContainsKey(channel.Id))
				VixenSystem.Logging.Error("ChannelManager: Adding a channel, but it's already in the instance map!");

			lock(_instances) {
				_instances[channel.Id] = channel;
			}
		}

		public void AddChannels(IEnumerable<Channel> channels) {
			foreach(Channel channel in channels) {
				AddChannel(channel);
			}
		}

		public void RemoveChannel(Channel channel) {
			//IEnumerator<CommandNode[]> enumerator;
			//if(_channelEnumerators.TryGetValue(channel, out enumerator)) {
			//    lock(_channelEnumerators) {
			//        // Kill enumerator if it hasn't been already.
			//        if (enumerator != null)
			//            enumerator.Dispose();
			//        // Remove from channel dictionary.
			//        _channelEnumerators.Remove(channel);
			//    }
			//}
			lock(_instances) {
				_instances.Remove(channel.Id);
			}
			// Remove any nodes that reference the channel.
			VixenSystem.Nodes.RemoveChannelLeaf(channel);
		}

		//internal void OpenChannels<T>()
		//    where T : IEnumerator<CommandNode[]> {
		//    //_enumeratorChannelsOpenedWith = typeof(T);
		//    //_CreateChannelEnumerators(_instances.Values);
		//}

		//public void CloseChannels() {
		//    //_ResetChannelEnumerators();
		//}

		public Channel GetChannel(Guid id) {
			if (_instances.ContainsKey(id)) {
				return _instances[id];
			}
			return null;
		}

		public bool SetChannelNodeForChannel(Channel channel, ChannelNode node)
		{
			if (channel == null)
				return false;

			bool rv = _channelToChannelNode.ContainsKey(channel);

			_channelToChannelNode[channel] = node;
			return rv;
		}

		public ChannelNode GetChannelNodeForChannel(Channel channel)
		{
			if (_channelToChannelNode.ContainsKey(channel))
				return _channelToChannelNode[channel];

			return null;
		}

		internal IOutputSourceCollection GetSources() {
			return new ChannelPatch();
		}

		///// <summary>
		///// 
		///// </summary>
		///// <param name="channel"></param>
		///// <returns>Channel's new state, null if no state change.</returns>
		//public Command UpdateChannelState(Channel channel, out bool updatedState) {
		//    IEnumerator<CommandNode[]> enumerator;
		//    updatedState = false;

		//    lock(_channelEnumerators) {
		//        // we potentially might be trying to update a channel one last time just after it's been
		//        // deleted (since the _UpdateChannelStates in Execution iterates over a copy of these
		//        // channels). If so, just ignore it if we can't find this channel. This is probably the
		//        // best performing solution that doesn't need locking around big chunks of code.
		//        if (!_channelEnumerators.ContainsKey(channel))
		//            return null;
		//        enumerator = _channelEnumerators[channel];
		//        // Will return true if state has changed.
		//        // State changes when data qualifies for execution.
		//        if(enumerator.MoveNext()) {
		//            Command channelState = Command.Combine(enumerator.Current.Select(x => x.Command));
		//            channel.Patch.Write(channelState);
		//            updatedState = true;
		//            return channelState;
		//        }
		//        return null;
		//    }
		//}

		public void Update() {
			lock(_instances) {
				_stopwatch.Restart();

				_instances.Values.AsParallel().ForAll(x => x.Update());

				_channelUpdateTimeValue.Set(_stopwatch.ElapsedMilliseconds);
			}
		}

		//private void _CreateChannelEnumerators(params Channel[] channels) {
		//    _CreateChannelEnumerators(channels as IEnumerable<Channel>);
		//}

		//private void _CreateChannelEnumerators(IEnumerable<Channel> channels) {
		//    if(_enumeratorChannelsOpenedWith == null) return;

		//    lock(_channelEnumerators) {
		//        foreach(Channel channel in channels.ToArray()) {
		//            if(!_channelEnumerators.ContainsKey(channel) || _channelEnumerators[channel] == null) {
		//                IEnumerator<CommandNode[]> enumerator = Activator.CreateInstance(_enumeratorChannelsOpenedWith, channel, Vixen.Sys.Execution.SystemTime) as IEnumerator<CommandNode[]>;
		//                _channelEnumerators[channel] = enumerator;
		//            }
		//        }
		//    }
		//}

		//private void _ResetChannelEnumerators() {
		//    lock(_channelEnumerators) {
		//        foreach(Channel channel in _channelEnumerators.Keys.ToArray()) {
		//            _channelEnumerators[channel].Dispose();
		//            _channelEnumerators[channel] = null;
		//        }
		//    }
		//}

		private string _Uniquify(string name) {
			if(_instances.Values.Any(x => x.Name == name)) {
				string originalName = name;
				bool unique;
				int counter = 2;
				do {
					name = originalName + "-" + counter++;
					unique = !_instances.Values.Any(x => x.Name == name);
				} while(!unique);
			}
			return name;
		}

		public IEnumerator<Channel> GetEnumerator()
		{
			lock(_instances) {
				Channel[] channels = _instances.Values.ToArray();
				return ((IEnumerable<Channel>)channels).GetEnumerator();
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private void _SetupInstrumentation() {
			_channelUpdateTimeValue = new ChannelUpdateTimeValue();
			VixenSystem.Instrumentation.AddValue(_channelUpdateTimeValue);
			_stopwatch = Stopwatch.StartNew();
		}
	}
}
