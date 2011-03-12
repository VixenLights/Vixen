using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Module.Sequence;

namespace Vixen.Script {
	interface IUserScriptHost {
		event EventHandler<ExecutorMessageEventArgs> Error;
		event EventHandler Ended;
		ScriptSequenceBase Sequence { get; set; }
		void Start();
		void Stop();
	}
}
