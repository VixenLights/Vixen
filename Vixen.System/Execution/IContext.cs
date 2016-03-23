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
		bool UpdateElementStates(TimeSpan currentTime);
		IIntentStates GetState(Guid key);
	}
}