using System;
using Vixen.Execution;

namespace VixenModules.App.SimpleSchedule {
	interface IScheduledItemStateObject {
		DateTime Start { get; }
		DateTime End { get; }

		bool ItemIsValid { get; }

		void RequestContext();
		IContext Context { get; set; }
		void ReleaseContext();
	}
}
