using Vixen.Sys;

namespace Vixen.Data.Flow
{
	public class IntentDataFlowData : Dispatchable<IntentDataFlowData>, IDataFlowData<IIntentState>
	{
		public IntentDataFlowData(IIntentState intentState)
		{
			Value = intentState;
		}

		object IDataFlowData.Value
		{
			get { return Value; }
		}

		public IIntentState Value { get; set; }
	}
}
