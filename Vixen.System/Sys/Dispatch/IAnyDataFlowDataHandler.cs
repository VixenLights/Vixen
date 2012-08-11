using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Data.Flow;

namespace Vixen.Sys.Dispatch {
	interface IAnyDataFlowDataHandler : IHandler<IDataFlowData<ICommand>>, IHandler<IDataFlowData<IEnumerable<ICommand>>>, IHandler<IDataFlowData<IEnumerable<IIntentState>>> {
	}
}
