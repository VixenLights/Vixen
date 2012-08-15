using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vixen.Data.Flow;
using Vixen.Sys.Instrumentation;

namespace Vixen.Sys.Managers {
	public class ChannelManager : IEnumerable<Channel> {
		private ChannelUpdateTimeValue _channelUpdateTimeValue;
		private Stopwatch _stopwatch;
		private ChannelDataFlowAdapterFactory _dataFlowAdapters;

		// a mapping of channel  GUIDs to channel instances. Used for quick reverse mapping at runtime.
		private Dictionary<Guid, Channel> _instances;

		// a mapping of channels back to their containing channel nodes. Used in a few special cases, particularly for runtime, so we can
		// quickly and easily find the node that a particular channel references (eg. if we're previewing the rendered data on a virtual display,
		// or anything else where we need to actually 'reverse' the rendering process).
		private Dictionary<Channel, ChannelNode> _channelToChannelNode;

		public ChannelManager() {
			_instances = new Dictionary<Guid,Channel>();
			_channelToChannelNode = new Dictionary<Channel, ChannelNode>();
			_SetupInstrumentation();
			_dataFlowAdapters = new ChannelDataFlowAdapterFactory();
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
			if(channel != null) {
				if(_instances.ContainsKey(channel.Id))
					VixenSystem.Logging.Error("ChannelManager: Adding a channel, but it's already in the instance map!");

				lock(_instances) {
					_instances[channel.Id] = channel;
				}

				_AddDataFlowParticipant(channel);
			}
		}

		public void AddChannels(IEnumerable<Channel> channels) {
			foreach(Channel channel in channels) {
				AddChannel(channel);
			}
		}

		public void RemoveChannel(Channel channel) {
			lock(_instances) {
				_instances.Remove(channel.Id);
			}

			_RemoveDataFlowParticipant(channel);

			// Remove any nodes that reference the channel.
			VixenSystem.Nodes.RemoveChannelLeaf(channel);
		}

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

		public void Update() {
			lock(_instances) {
				_stopwatch.Restart();

				_instances.Values.AsParallel().ForAll(x => x.Update());

				_channelUpdateTimeValue.Set(_stopwatch.ElapsedMilliseconds);
			}
		}

		private void _AddDataFlowParticipant(Channel channel) {
			VixenSystem.DataFlow.AddComponent(_dataFlowAdapters.GetAdapter(channel));
		}

		private void _RemoveDataFlowParticipant(Channel channel) {
			VixenSystem.DataFlow.RemoveComponent(_dataFlowAdapters.GetAdapter(channel));
		}

		public IDataFlowComponent GetDataFlowComponentForChannel(Channel channel) {
			return _dataFlowAdapters.GetAdapter(channel);
		}

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

		private void _SetupInstrumentation() {
			_channelUpdateTimeValue = new ChannelUpdateTimeValue();
			VixenSystem.Instrumentation.AddValue(_channelUpdateTimeValue);
			_stopwatch = Stopwatch.StartNew();
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
	}
}
