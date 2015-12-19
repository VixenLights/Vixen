using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.Data.Flow
{
	public class IntentsDataFlowData : Dispatchable<IntentsDataFlowData>, IDataFlowData<List<IIntentState>>
	{
		public IntentsDataFlowData(List<IIntentState> intentStates)
		{
			Value = intentStates;
		}

		public List<IIntentState> Value { get; set; }

		object IDataFlowData.Value
		{
			get { return Value; }
		}
	}
}