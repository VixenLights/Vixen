using System;
using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.Execution
{
	public interface IContext : IExecutionControl, IRuns, IDisposable
	{
		event EventHandler ContextStarted;
		event EventHandler ContextEnded;

		IExecutor Executor { set; }

		Guid Id { get; }
		string Name { get; }
		TimeSpan GetTimeSnapshot();
		IEnumerable<Guid> UpdateElementStates(TimeSpan currentTime);
		IStateSource<IIntentStates> GetState(Guid key);
	}
}