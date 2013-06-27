using System;

namespace Common.StateMach
{
	public interface ITransition<in T>
		where T : class
	{
		Predicate<T> Condition { get; }
		IState<T> TargetState { get; }
	}
}