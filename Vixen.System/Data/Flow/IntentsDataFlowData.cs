using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.Data.Flow {
	public class IntentsDataFlowData : Dispatchable<IntentsDataFlowData>, IDataFlowData<IEnumerable<IIntentState>>  {
		public IntentsDataFlowData(IEnumerable<IIntentState> intentStates) {
			Value = intentStates;
		}

		public IEnumerable<IIntentState> Value { get; private set; }

		object IDataFlowData.Value {
			get { return Value; }
		}
	}
}
