using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;
using Vixen.Module.Timing;

namespace Vixen.Execution {
	class NonExpiringChannelEnumerator : IEnumerator<CommandNode[]> {
		private IEnumerable<CommandNode> _source;
		private IEnumerator<CommandNode> _enumerator;
		private CommandNode[] _currentState;

		public NonExpiringChannelEnumerator(IEnumerable<CommandNode> data, ITiming timingSource) {
			_source = data;
			Reset();
		}

		public CommandNode[] Current {
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
			if(_enumerator.MoveNext()) {
				_currentState[0] = _enumerator.Current;
				return true;
			}
			return false;
		}

		public void Reset() {
			_currentState = new CommandNode[1];
			_enumerator = _source.GetEnumerator();
		}
	}
}
