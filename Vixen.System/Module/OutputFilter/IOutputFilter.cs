using Vixen.Sys;

namespace Vixen.Module.OutputFilter {
	public interface IOutputFilter : IHasSetup {
		IIntentState Affect(IIntentState intentValue); 
	}
}
