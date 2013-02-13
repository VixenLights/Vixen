using System;
using System.Collections.Generic;
using Vixen.Data.Flow;

namespace Vixen.Sys {
	class ElementDataFlowAdapterFactory {
		private Dictionary<Guid, ElementDataFlowAdapter> _instances;

		public ElementDataFlowAdapterFactory() {
			_instances = new Dictionary<Guid, ElementDataFlowAdapter>();
		}

		public IDataFlowComponent GetAdapter(Element element) {
			ElementDataFlowAdapter adapter;
			if(!_instances.TryGetValue(element.Id, out adapter)) {
				adapter = new ElementDataFlowAdapter(element);
				_instances[element.Id] = adapter;
			}
			return adapter;
		}
	}
}
