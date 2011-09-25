using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	class ChannelSortedList : SortedList<TimeSpan, Command>, IChannelDataStore {
		public void Add(Command command) {
			Add(command.StartTime, command);
		}

		public new IEnumerator<Command> GetEnumerator() {
			return new ChannelSortedListEnumerator(this);
		}

		class ChannelSortedListEnumerator : IEnumerator<Command> {
			private ChannelSortedList _list;
			private Command _current;
			private int _index;

			public ChannelSortedListEnumerator(ChannelSortedList list) {
				_list = list;
				Reset();
			}

			public Command Current {
				get { return _current; }
			}

			public void Dispose() {
			}

			object IEnumerator.Current {
				get { return Current; }
			}

			public bool MoveNext() {
				if(_index < _list.Count) {
					_current = _list.Values[_index];
					_index++;
					return true;
				}
				_current = null;
				return false;
			}

			public void Reset() {
				_index = 0;
			}
		}
	}
}
