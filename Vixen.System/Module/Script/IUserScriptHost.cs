using System;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Module.Script {
	public interface IUserScriptHost {
		event EventHandler<ExecutorMessageEventArgs> Error;
		event EventHandler Ended;

		ISequence Sequence { get; set; }
		ITiming TimingSource { get; set; }
		
		void Start();
		void Stop();
	}
}
