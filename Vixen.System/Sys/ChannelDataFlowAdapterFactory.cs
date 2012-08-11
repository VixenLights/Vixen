using System;
using System.Collections.Generic;
using Vixen.Data.Flow;

namespace Vixen.Sys {
	class ChannelDataFlowAdapterFactory {
		private Dictionary<Guid, ChannelDataFlowAdapter> _instances;

		public ChannelDataFlowAdapterFactory() {
			_instances = new Dictionary<Guid, ChannelDataFlowAdapter>();
		}

		public IDataFlowComponent GetAdapter(Channel channel) {
			ChannelDataFlowAdapter adapter;
			if(!_instances.TryGetValue(channel.Id, out adapter)) {
				adapter = new ChannelDataFlowAdapter(channel);
				_instances[channel.Id] = adapter;
			}
			return adapter;
		}
	}
}
