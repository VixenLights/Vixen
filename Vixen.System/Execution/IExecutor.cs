using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution
{
	interface IExecutor : IDisposable
	{
		event EventHandler<SequenceStartedEventArgs> SequenceStarted;
		event EventHandler<SequenceEventArgs> SequenceEnded;
		event EventHandler<ExecutorMessageEventArgs> Message;
		event EventHandler<ExecutorMessageEventArgs> Error;

		ISequence Sequence { get; set; }
		bool IsPlaying { get; }
		void Play(TimeSpan startTime, TimeSpan endTime);
		void Pause();
		void Resume();
		void Stop();

		IEnumerable<EffectNode> GetSequenceData();
		ITiming GetSequenceTiming();
	}
}
