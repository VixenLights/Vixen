using System;
using System.Collections.Generic;
using Vixen.Module.Timing;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace Vixen.Execution
{
	public interface IExecutor : IExecutionControl, IRuns
	{
		event EventHandler<SequenceStartedEventArgs> SequenceStarted;
		event EventHandler<SequenceStartedEventArgs> SequenceReStarted;
		event EventHandler<SequenceEventArgs> SequenceEnded;
		event EventHandler<ExecutorMessageEventArgs> Message;
		event EventHandler<ExecutorMessageEventArgs> Error;

		string Name { get; }
		ITiming TimingSource { get; }
		IEnumerable<ISequenceFilterNode> SequenceFilters { get; }
		SequenceLayers SequenceLayers { get; }
	}
}