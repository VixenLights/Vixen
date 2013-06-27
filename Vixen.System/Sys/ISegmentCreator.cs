using System;

namespace Vixen.Sys
{
	internal interface ISegmentCreator<T>
	{
		IIntentSegment<T> CreateSegment(T startValue, T endValue, TimeSpan timeSpan);
	}
}