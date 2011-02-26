using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module.Sequence;

namespace Vixen.Module.RuntimeBehavior {
	public interface IRuntimeBehavior {
		//void Initialize(ISequenceModuleInstance sequence, ITimingSource timingSource);
		void Startup(ISequenceModuleInstance sequence, ITimingSource timingSource);
		void Shutdown();
		IEnumerable<CommandNode> GenerateCommandNodes(InsertDataParameters parameters);
		bool Enabled { get; set; }
		Tuple<string, Action>[] BehaviorActions { get; }
	}
}
