using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.StateMach {
	public class Machine<T>
		where T : class {
		private HashSet<IState<T>> _states;
		private Dictionary<T, IState<T>> _objectStates;

		public Machine() {
			_states = new HashSet<IState<T>>();
			_objectStates = new Dictionary<T, IState<T>>();
		}

		public void AddState(IState<T> state) {
			if(state == null) throw new ArgumentNullException("state");

			_states.Add(state);
		}

		public void AddStatedObject(T obj, IState<T> startState) {
			if(obj == null) throw new ArgumentNullException("obj");
			if(startState == null) throw new ArgumentNullException("startState");

			_EnterState(obj, startState);
		}

		public void Update() {
			foreach(T obj in _objectStates.Keys) {
				ITransition<T> transitionToTake = _FindTransitionWithTrueCondition(obj);
				_TransitionStates(obj, _objectStates[obj], transitionToTake);
			}
		}

		private ITransition<T> _FindTransitionWithTrueCondition(T obj) {
			return _objectStates[obj].Transitions.FirstOrDefault(x => x.Condition(obj));
		}

		private void _TransitionStates(T obj, IState<T> currentState, ITransition<T> transition) {
			if(transition == null) return;

			_LeaveState(obj, currentState);
			_EnterState(obj, transition.TargetState);
		}

		private void _LeaveState(T obj, IState<T> state) {
			lock(_objectStates) {
				state.Leaving(obj);
				_objectStates.Remove(obj);
			}
		}

		private void _EnterState(T obj, IState<T> state) {
			lock(_objectStates) {
				_objectStates[obj] = state;
				state.Entering(obj);
			}
		}
	}
}
