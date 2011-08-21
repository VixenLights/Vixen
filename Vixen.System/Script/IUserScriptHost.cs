using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Script {
	interface IUserScriptHost {
		event EventHandler<ExecutorMessageEventArgs> Error;
		event EventHandler Ended;
		ScriptSequence Sequence { get; set; }
		void Start();
		void Stop();
	}
}
