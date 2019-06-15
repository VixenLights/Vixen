using System.Collections.Generic;
using System.Linq;
using Vixen.Data.Flow;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Policy
{
	internal class SmartControllerDataPolicy : DataFlowDataDispatch
	{
		public IIntent[] OutputCurrentState;

		public override void Handle(IntentsDataFlowData obj)
		{
			//IntentChangeCollection intentChanges = null;
			//IIntent[] newState = obj.Value.Select(x => x.Intent).ToArray();

			//if (_OutputHasStateToCompare) {
			//	if (_OutputStateDiffersFrom(newState)) {
			//		IEnumerable<IIntent> addedIntents = newState.Except(OutputCurrentState);
			//		IEnumerable<IIntent> removedIntents = OutputCurrentState.Except(newState);
			//		intentChanges = new IntentChangeCollection(addedIntents, removedIntents);
			//	}
			//}
			//else {
			//	intentChanges = new IntentChangeCollection(newState, null);
			//}

			//OutputCurrentState = newState.ToArray();
			//Result = intentChanges;
		}

		public override void Handle(CommandDataFlowData obj)
		{
			OutputCurrentState = null;
			Result = null;
		}

		public override void Handle(CommandsDataFlowData obj)
		{
			OutputCurrentState = null;
			Result = null;
		}

		public IntentChangeCollection Result { get; private set; }

		private bool _OutputHasStateToCompare
		{
			get { return OutputCurrentState != null; }
		}

		private bool _OutputStateDiffersFrom(IEnumerable<IIntent> state)
		{
			//*** test the effectiveness of this
			return !OutputCurrentState.SequenceEqual(state);
		}
	}
}