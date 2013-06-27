using System;
using Vixen.Execution;

namespace VixenModules.App.SimpleSchedule
{
	internal interface IScheduledItemStateObject : IEquatable<IScheduledItemStateObject>
	{
		Guid Id { get; }

		DateTime Start { get; }
		DateTime End { get; }

		void RequestContext();
		IContext Context { get; set; }
		void ReleaseContext();
	}
}