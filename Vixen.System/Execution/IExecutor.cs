using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sequence;
using Vixen.Module.Sequence;

namespace Vixen.Execution
{
	interface IExecutor : IDisposable
	{
		event EventHandler<SequenceStartedEventArgs> SequenceStarted;
		event EventHandler SequenceEnded;
		event EventHandler<ExecutorMessageEventArgs> Message;
		event EventHandler<ExecutorMessageEventArgs> Error;

		ISequenceModuleInstance Sequence { get; set; }
		void Play(int startTime, int endTime);
		void Pause();
		void Resume();
		void Stop();
	}
}
