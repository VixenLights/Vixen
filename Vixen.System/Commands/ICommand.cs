using Vixen.Sys;

namespace Vixen.Commands
{
	public interface ICommand : IDispatchable
	{
		object CommandValue { get;  }
	}
}