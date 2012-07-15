using System;
using Vixen.Sys;

namespace Vixen.Execution.Context {
	public interface ISequenceContext : IContext {
		event EventHandler<SequenceStartedEventArgs> SequenceStarted;
		event EventHandler<SequenceEventArgs> SequenceEnded;
		event EventHandler<ExecutorMessageEventArgs> Message;
		event EventHandler<ExecutorMessageEventArgs> Error;

		ISequence Sequence { get; set; }
		void Play(TimeSpan startTime, TimeSpan endTime);
	}
}
