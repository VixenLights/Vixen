using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Data.Flow;

namespace Vixen.Sys.Dispatch {
	abstract public class DataFlowDataDispatch : IAnyDataFlowDataHandler {
		virtual public void Handle(IDataFlowData<ICommand> obj) { }

		virtual public void Handle(IDataFlowData<IEnumerable<ICommand>> obj) { }

		virtual public void Handle(IDataFlowData<IEnumerable<IIntentState>> obj) { }
	}
}
