using System;
using System.Collections.Generic;

namespace Common.StateMach {
	public interface IState<in T>
		where T : class {
		string Name { get; }
		IEnumerable<ITransition<T>> Transitions { get; }
		void Entering(T obj);
		void Leaving(T obj);
	}
}
