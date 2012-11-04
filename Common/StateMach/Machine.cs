using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.StateMach {
	public class Machine<T>
		where T : class, IEquatable<T> {
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

			SetState(obj, startState);
		}

		public void RemoveStatedObject(T obj) {
			if(obj == null) throw new ArgumentNullException("obj");

			_objectStates.Remove(obj);
		}

		public void ReplaceStatedObject(T oldObj, T newObj) {
			if(oldObj == null) throw new ArgumentNullException("oldObj");
			if(newObj == null) throw new ArgumentNullException("newObj");

			if(_objectStates.ContainsKey(oldObj)) {
				lock(_objectStates) {
					_objectStates[newObj] = _objectStates[oldObj];
					_objectStates.Remove(oldObj);
				}
			}
		}

		public IEnumerable<T> GetStatedObjects() {
			return _objectStates.Keys;
		}

		public void SetState(T obj, IState<T> state) {
			if(obj == null) throw new ArgumentNullException("obj");
			if(state == null) throw new ArgumentNullException("state");

			_EnterState(obj, state);
		}

		public void Update() {
			foreach(T obj in _objectStates.Keys.ToArray()) {
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
