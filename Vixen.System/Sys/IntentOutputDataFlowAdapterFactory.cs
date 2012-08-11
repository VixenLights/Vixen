using System;
using System.Collections.Generic;
using Vixen.Data.Flow;
using Vixen.Sys.Output;

namespace Vixen.Sys {
	class IntentOutputDataFlowAdapterFactory {
		private Dictionary<Guid, IntentOutputDataFlowAdapter> _instances;

		public IntentOutputDataFlowAdapterFactory() {
			_instances = new Dictionary<Guid, IntentOutputDataFlowAdapter>();
		}

		public IDataFlowComponent GetAdapter(IntentOutput output) {
			IntentOutputDataFlowAdapter adapter;
			if(!_instances.TryGetValue(output.Id, out adapter)) {
				adapter = new IntentOutputDataFlowAdapter(output);
				_instances[output.Id] = adapter;
			}
			return adapter;
		}
	}
}
