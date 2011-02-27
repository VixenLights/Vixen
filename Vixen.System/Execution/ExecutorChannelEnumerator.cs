using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;

namespace Vixen.Execution {
	class ExecutorChannelEnumerator : IEnumerator<CommandData> {
		private int _startTime;
		private int _endTime;
		private OutputChannel _channel;
		private ITimingSource _timingSource;
		//Don't like doing this, don't like comparing frames to determine state change.
		private CommandData _newFrame;
		// The default instance of a CommandData is considered invalid and invalid
		// commands are ignored.
		private CommandData _currentFrame = default(CommandData);
		private IEnumerator<CommandData> _enumerator;
		private int _position;

		// enumerator.Current = 1-frame buffer

		public ExecutorChannelEnumerator(OutputChannel channel, ITimingSource timingSource, int startTime, int endTime) {
			_channel = channel;
			_timingSource = timingSource;
			_startTime = startTime;
			_endTime = endTime;
			Reset();
		}

		public CommandData Current {
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
			if(_position >= _endTime) return false;

			if(_position >= _currentFrame.StartTime && _position < _currentFrame.EndTime) {
				// Current frame is still valid, no state change.
				return false;
			} else {
				// Current frame has expired.
				// Assume a cleared state.
				_newFrame = CommandData.Empty;

				// Burn through frames until one is not expired or we run out of data.
				while(!_enumerator.Current.IsRequired && _enumerator.Current.EndTime <= _position) {
					if(!_enumerator.MoveNext()) {
						break;
					}
				}

				// Either we've run out of data or the buffered frame is not expired.
				if(_enumerator.Current.IsValid && _enumerator.Current.StartTime <= _position) {
					// The buffered frame qualifies.
					_newFrame = _enumerator.Current;
					_enumerator.MoveNext();
				} else if(_enumerator.Current.StartTime > _position) {
					// The buffered frame is future.
					// Leave it buffered for a check in the next loop.
					// Let the return be a cleared state.
				}
				// Else we are out of data.
				// Let the return be a cleared state.

				if(_newFrame != _currentFrame) {
					// State change.
					_currentFrame = _newFrame;
					return true;
				}

				// State will not change.
				return false;
			}
		}

		public void Reset() {
			_enumerator = _channel.GetEnumerator();
			// Get to the first event >= the start time.
			while(_enumerator.MoveNext() && _enumerator.Current.StartTime < _startTime) { }
		}
	}
}
