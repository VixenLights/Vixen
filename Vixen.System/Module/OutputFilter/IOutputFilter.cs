using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Module.OutputFilter {
	public interface IOutputFilter : IHasSetup {
		ICommand Affect(ICommand command);
	}
}
