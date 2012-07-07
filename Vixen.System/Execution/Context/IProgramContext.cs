using System;
using Vixen.Execution.DataSource;
using Vixen.Sys;

namespace Vixen.Execution.Context {
	public interface IProgramContext : IContext {
		event EventHandler<SequenceStartedEventArgs> SequenceStarted;
		event EventHandler<SequenceEventArgs> SequenceEnded;
		event EventHandler<ProgramEventArgs> ProgramStarted;
		event EventHandler<ProgramEventArgs> ProgramEnded;
		event EventHandler<ExecutorMessageEventArgs> Message;
		event EventHandler<ExecutorMessageEventArgs> Error;

		IProgram Program { get; set; }
		IMutableDataSource ContextDataSource { get; }
		int Queue(ISequence sequence);
	}
}
