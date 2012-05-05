using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Module.PostFilter {
	public interface IPostFilter : IHasSetup {
		ICommand Affect(ICommand command);
	}
}
