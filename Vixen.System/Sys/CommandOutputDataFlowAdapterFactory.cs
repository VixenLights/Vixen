using System;
using System.Collections.Generic;
using Vixen.Data.Flow;
using Vixen.Sys.Output;

namespace Vixen.Sys {
	class CommandOutputDataFlowAdapterFactory {
		private Dictionary<Guid, CommandOutputDataFlowAdapter> _instances;

		public CommandOutputDataFlowAdapterFactory() {
			_instances = new Dictionary<Guid, CommandOutputDataFlowAdapter>();
		}

		public IDataFlowComponent GetAdapter(CommandOutput output) {
			CommandOutputDataFlowAdapter adapter;
			if(!_instances.TryGetValue(output.Id, out adapter)) {
				adapter = new CommandOutputDataFlowAdapter(output);
				_instances[output.Id] = adapter;
			}
			return adapter;
		}
	}
}
