using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Module.PostFilter {
	public interface IPostFilter : ISetup {
		ICommand Affect(ICommand command);
	}
}
