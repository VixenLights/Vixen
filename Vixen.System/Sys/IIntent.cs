using System;
using System.Collections.Generic;
using Vixen.Data.Value;
using Vixen.Sys.LayerMixing;

namespace Vixen.Sys
{
	public interface IIntent : IDispatchable
	{
		TimeSpan TimeSpan { get; }
		IIntentState CreateIntentState(TimeSpan intentRelativeTime, ILayer layer);
		void FractureAt(TimeSpan intentRelativeTime);
		void FractureAt(IEnumerable<TimeSpan> intentRelativeTimes);
		void FractureAt(ITimeNode intentRelativeTime);
		IIntent[] DivideAt(TimeSpan intentRelativeTime);
		void ApplyFilter(ISequenceFilterNode sequenceFilterNode, TimeSpan contextAbsoluteIntentStartTime);
		object GetStateAt(TimeSpan intentRelativeTime);
	}

	public interface IIntent<out T> : IIntent
		where T : IIntentDataType
	{
		new T GetStateAt(TimeSpan intentRelativeTime);
	}
}