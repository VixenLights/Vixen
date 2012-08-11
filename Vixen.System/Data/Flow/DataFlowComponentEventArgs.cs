using System;

namespace Vixen.Data.Flow {
	public class DataFlowComponentEventArgs : EventArgs {
		public DataFlowComponentEventArgs(IDataFlowComponent component) {
			Component = component;
		}

		public IDataFlowComponent Component { get; private set; }
	}
}
