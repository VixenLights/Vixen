using Vixen.Data.Flow;

namespace Vixen.Sys.Dispatch {
	abstract public class DataFlowDataDispatch : IAnyDataFlowDataHandler {
		virtual public void Handle(CommandDataFlowData obj) { }

		virtual public void Handle(CommandsDataFlowData obj) { }

		virtual public void Handle(IntentsDataFlowData obj) { }
	}
}
