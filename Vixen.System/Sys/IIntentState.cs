using System;
using Vixen.Data.Value;

namespace Vixen.Sys
{
	public interface IIntentState : IDispatchable
	{
		IIntent Intent { get; }
		TimeSpan RelativeTime { get; }
		IIntentState Clone();
		object GetValue();
	}

	public interface IIntentState<out T> : IIntentState
		where T : IIntentDataType
	{
		IIntent<T> Intent { get; }
		T GetValue();
	}
}