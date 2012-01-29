using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution {
	// T - single instance in
	// U - collection of T out
	abstract class MultiStateTimedEnumerator<T, U> : IEnumerator<U>
		where T : class, IDataNode
		where U : class, IEnumerable<T> {
		private TimeSpan _startTime;
		//private TimeSpan _endTime;
		private IEnumerable<T> _source;
		private ITiming _timingSource;
		private IEnumerator<T> _enumerator;
		private TimeSpan _position;
		private U _currentState;
		private HashSet<T> _states = new HashSet<T>();

		// enumerator.Current = 1-frame buffer

		//public MultiStateTimedEnumerator(IEnumerable<T> source, ITiming timingSource, TimeSpan startTime, TimeSpan endTime) {
		protected MultiStateTimedEnumerator(IEnumerable<T> source, ITiming timingSource, TimeSpan startTime) {
			_source = source;
			_timingSource = timingSource;
			_startTime = startTime;
			//_endTime = endTime;
			Reset();
		}

		public U Current {
			get { return _currentState; }
		}

		public void Dispose() {
			if(_enumerator != null) {
				_enumerator.Dispose();
			}
		}

		object System.Collections.IEnumerator.Current {
			get { return Current; }
		}

		public bool MoveNext() {
			bool dirty = false;

			_position = _timingSource.Position;

			// Check for any expired frames in the current state.
			foreach(T instance in _states.ToArray()) {
				if(_IsExpired(instance)) {
					// Remove expired frame.
					_states.Remove(instance);
					// Mark as dirty.
					dirty = true;
				}
			}

			// Burn frames until buffered frame is not expired or there is no data.
			bool expired = false;
			while((_enumerator.Current == null || (expired = _IsExpired(_enumerator.Current)))) {
				if(expired) {
					_ItemExpired(_enumerator.Current);
					expired = false;
				}
				if(!_enumerator.MoveNext())
					break;
			}

			// If we have data and its time is valid...
			while (_enumerator.Current != null && _enumerator.Current.StartTime <= _position && !_IsExpired(_enumerator.Current)) {
				_ItemQualified(_enumerator.Current);
				// Add the new frame to the state, and automatically move on to the next.
				dirty = _states.Add(_enumerator.Current);
				_enumerator.MoveNext();
			}

			// If we are dirty, the state has changed.
			if(dirty) {
				lock(_source) {
					_currentState = _states.ToArray() as U;
				}
				return true;
			}

			// State has not changed.
			return false;
		}

		public void Reset() {
			_currentState = new T[0] as U;
			_enumerator = _source.GetEnumerator();
			// Get to the first event >= the start time.
			while(_enumerator.MoveNext() && _enumerator.Current.StartTime < _startTime) { }
		}

		private bool _IsExpired(T instance) {
			return _position >= instance.EndTime;
		}

		virtual protected void _ItemQualified(T value) { }
		virtual protected void _ItemExpired(T value) { }
	}
}
