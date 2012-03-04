using System.Collections.Generic;
using Vixen.Commands;

namespace Vixen.Sys {
	public interface IDataPolicy {
		ICommand GenerateCommand(IEnumerable<IIntentState> intentStates); 
	}
}
