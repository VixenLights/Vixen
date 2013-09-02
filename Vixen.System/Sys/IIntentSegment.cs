using System;

namespace Vixen.Sys
{
	public interface IIntentSegment : IDispatchable
	{
		TimeSpan TimeSpan { get; }
		object StartValue { get; set; }
		object EndValue { get; set; }
	}

	public interface IIntentSegment<T> : IIntentSegment
	{
		new T StartValue { get; set; }
		new T EndValue { get; set; }
		T GetStateAt(TimeSpan segmentRelativeTime);
	}
}