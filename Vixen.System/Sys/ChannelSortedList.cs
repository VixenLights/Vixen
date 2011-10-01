using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;

namespace Vixen.Sys {
	class ChannelSortedList : SortedList<TimeSpan, CommandNode>, IChannelDataStore {
		public void Add(CommandNode command) {
			Add(command.StartTime, command);
		}

		public new IEnumerator<CommandNode> GetEnumerator() {
			return new ChannelSortedListEnumerator(this);
		}

		class ChannelSortedListEnumerator : IEnumerator<CommandNode> {
			private ChannelSortedList _list;
			private CommandNode _current;

			public ChannelSortedListEnumerator(ChannelSortedList list) {
				_list = list;
				Reset();
			}

			public CommandNode Current {
				get { return _current; }
			}

			public void Dispose() {
			}

			object IEnumerator.Current {
				get { return Current; }
			}

			public bool MoveNext() {
				if(_list.Count > 0) {
					_current = _list.Values[0];
					_list.RemoveAt(0);
					return true;
				}
				_current = null;
				return false;
			}

			public void Reset() {
			}
		}
	}
}
