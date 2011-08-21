using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Timing;

namespace Vixen.Execution {
	abstract class TimedChannelEnumerator<T> : IEnumerator<T>
		where T : class, ITimed, new() {
		private long _startTime;
		private long _endTime;
		private IEnumerable<T> _source;
		private ITiming _timingSource;
		private T _newFrame;
		private IEnumerator<T> _enumerator;
		private long _position;
		private T _emptyInstance = new T();
		private T _currentFrame;

		// enumerator.Current = 1-frame buffer

		public TimedChannelEnumerator(IEnumerable<T> source, ITiming timingSource, long startTime, long endTime) {
			_source = source;
			_timingSource = timingSource;
			_startTime = startTime;
			_endTime = endTime;
			Reset();
		}

		public T Current {
			get { return _currentFrame; }
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
			_position = _timingSource.Position;
			_newFrame = _currentFrame;

			if(_position >= _currentFrame.EndTime) {
				// Current frame has expired.

				// Assume a cleared state.
				_newFrame = _emptyInstance;
				// Burn frames until buffered frame is not expired or there is no data.
				while((_enumerator.Current == null || _enumerator.Current.EndTime <= _position) && _enumerator.MoveNext()) { }
				if(_enumerator.Current != null && _enumerator.Current.StartTime <= _position) {
					// We have data and its time is valid.
					_newFrame = _enumerator.Current;
				}
			}

			// If the state is to change, assign the new frame.
			// The frame references will either be _emptyInstance or data, so a reference
			// comparison is acceptable.
			if(_currentFrame != _newFrame) {
				// Do not allow a state change if the source is being written to.
				// This is to prevent an update from happening in the middle of
				// what is supposed to be an atomic write operation across multiple
				// channels.
				lock(_source) {
					_currentFrame = _newFrame;
				}
				// State has changed.
				return true;
			}

			// Otherwise there is no state change.
			return false;
		}

		public void Reset() {
			_currentFrame = _emptyInstance;
			_enumerator = _source.GetEnumerator();
			// Get to the first event >= the start time.
			while(_enumerator.MoveNext() && _enumerator.Current.StartTime < _startTime) { }
		}
	}

}
