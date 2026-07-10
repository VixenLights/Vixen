using Vixen.Data.Flow;
using Vixen.Sys;

namespace Vixen.Module.OutputFilter
{
	public interface IOutputFilter : IHasSetup, IDataFlowComponent
	{
		//IIntentState Affect(IIntentState intentValue);
		//IDataFlowData Affect(IDataFlowData data);
		void Update(IDataFlowData data);
	}
}