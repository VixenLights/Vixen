using System.Collections.Generic;
using Vixen.Commands;

namespace Vixen.Sys
{
	public interface ICombinator : IDispatchable
	{
		ICommand Combine(List<ICommand> commands);
	}
}