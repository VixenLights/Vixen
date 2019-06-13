using System;
using Vixen.Data.Value;
using Vixen.Sys.LayerMixing;

namespace Vixen.Sys
{
	public interface IIntent : IDispatchable
	{
		TimeSpan TimeSpan { get; }
		IIntentState CreateIntentState(TimeSpan intentRelativeTime, ILayer layer);
		object GetStateAt(TimeSpan intentRelativeTime);
	}

	public interface IIntent<out T> : IIntent
		where T : IIntentDataType
	{
		new T GetStateAt(TimeSpan intentRelativeTime);
	}
}