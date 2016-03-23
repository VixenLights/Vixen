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
		byte Layer { get; }
	}

	public interface IIntentState<out T> : IIntentState
		where T : IIntentDataType
	{
		new IIntent<T> Intent { get; }
		new T GetValue();
	}
}