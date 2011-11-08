using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;

namespace Vixen.Sys {
	class ChannelSortedList : SortedList<TimeSpan, Queue<CommandNode>>, IChannelDataStore {
		private object _sync = new object();

		public void Add(CommandNode command) {
			lock(_sync) {
				if (ContainsKey(command.StartTime)) {
					this[command.StartTime].Enqueue(command);
				} else {
					this[command.StartTime] = new Queue<CommandNode>(command.AsEnumerable());
				}
			}
		}

		public new IEnumerator<CommandNode> GetEnumerator() {
			return new ChannelSortedListEnumerator(this, _sync);
		}

		#region ChannelSortedListEnumerator class
		class ChannelSortedListEnumerator : IEnumerator<CommandNode> {
			private ChannelSortedList _list;
			private CommandNode _current;
			private object _sync;

			public ChannelSortedListEnumerator(ChannelSortedList list, object sync) {
				_sync = sync;
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
					lock(_sync) {
						_current = _list.Values[0].Dequeue();
						if (_list.Values[0].Count <= 0)
							_list.RemoveAt(0);
					}
					return true;
				}
				_current = null;
				return false;
			}

			public void Reset() {
			}
		}
		#endregion
	}
}
