using System;
using Vixen.Execution;

namespace VixenModules.App.SimpleSchedule {
	interface IScheduledItemStateObject : IEquatable<IScheduledItemStateObject> {
		Guid Id { get; }

		DateTime Start { get; }
		DateTime End { get; }

		bool ItemIsValid { get; }

		void RequestContext();
		IContext Context { get; set; }
		void ReleaseContext();
	}
}
