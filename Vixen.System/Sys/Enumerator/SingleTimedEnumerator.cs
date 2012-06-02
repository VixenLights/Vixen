using System;
using System.Collections.Generic;
using Vixen.Module.Timing;

namespace Vixen.Sys.Enumerator {
	abstract class SingleTimedEnumerator<T> : IEnumerator<T>
		where T : class, IDataNode, new() {
		private TimeSpan _startTime;
		//private TimeSpan _endTime;
		private IEnumerable<T> _source;
		private ITiming _timingSource;
		private T _newFrame;
		private IEnumerator<T> _enumerator;
		private TimeSpan _position;
		private T _emptyInstance = new T();
		private T _currentFrame;
		private SingleTimedEnumeratorProgressType _responseType;
		private bool _skipMissedItems;

		// enumerator.Current = 1-frame buffer

		//protected SingleTimedEnumerator(IEnumerable<T> source, ITiming timingSource, TimeSpan startTime, TimeSpan endTime,
		//    SingleTimedEnumeratorProgressType progressResponseType, bool skipMissedItems)
		protected SingleTimedEnumerator(IEnumerable<T> source, ITiming timingSource, TimeSpan startTime,
			SingleTimedEnumeratorProgressType progressResponseType, bool skipMissedItems)
		{
			_source = source;
			_timingSource = timingSource;
			_startTime = startTime;
			//_endTime = endTime;
			_responseType = progressResponseType;
			_skipMissedItems = skipMissedItems;
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

			// check if we need to progress to the next item: always true if we're triggering on leading edge, and
			// true if held for duration and the current frame has expired
			if (_responseType == SingleTimedEnumeratorProgressType.LeadingEdge ||
				_responseType == SingleTimedEnumeratorProgressType.HeldForDuration && _position >= _currentFrame.EndTime) {

				// move to the next item in the data list while any of the following:
				// 1) the current one is null
				// 2) we're skipping missed items and the item is expired, or our current frame is expired
				// 3) we're edge triggering, and we've already output the current frame; immediately go on to the next
				while ((_enumerator.Current == null) ||
					   ((_enumerator.Current == _currentFrame || _skipMissedItems) && _enumerator.Current.EndTime <= _position) ||
					   (_responseType == SingleTimedEnumeratorProgressType.LeadingEdge && _enumerator.Current == _currentFrame)
					  )
					if (!_enumerator.MoveNext())
						break;

				// if the next element in the list is current, then populate ourselves with it. Otherwise, fall back to <nothing>.
				if (_enumerator.Current != null && _enumerator.Current.StartTime <= _position) {
					_newFrame = _enumerator.Current;
				} else {
					_newFrame = _emptyInstance;
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

	/// <summary>
	/// the options for the way the single timed item enumerator will respond.
	/// </summary>
	enum SingleTimedEnumeratorProgressType
	{
		/// <summary>
		/// The leading edge of a Timed item will satisfy the progress conditions. The next item
		/// returned will be the next ordered Timed item, regardless of if the first one has ended.
		/// </summary>
		LeadingEdge,

		/// <summary>
		/// The Timed item will 'block' for its duration. This means any other timed items that
		/// start during the first will be ignored, unless they are still active at the end of the first.
		/// </summary>
		HeldForDuration
	}
}
