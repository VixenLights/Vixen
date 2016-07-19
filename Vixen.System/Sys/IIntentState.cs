using System;
using Vixen.Data.Value;
using Vixen.Sys.LayerMixing;

namespace Vixen.Sys
{
	public interface IIntentState : IDispatchable
	{
		IIntent Intent { get; }
		TimeSpan RelativeTime { get; }
		IIntentState Clone();
		object GetValue();
		ILayer Layer { get; }
	}

	public interface IIntentState<out T> : IIntentState
		where T : IIntentDataType
	{
		new IIntent<T> Intent { get; }
		new T GetValue();
	}
}