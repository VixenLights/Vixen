using System.Collections.Generic;
using Vixen.Commands;

namespace Vixen.Sys {
	internal class CommandStateSourceCollection<K> : IStateSourceCollection<K, Command> {
		private Dictionary<K, CommandStateSource> _commandStates;

		public CommandStateSourceCollection() {
			_commandStates = new Dictionary<K, CommandStateSource>();
		}

		public void SetValue(K key, Command value) {
			CommandStateSource commandState;
			if(!_commandStates.TryGetValue(key, out commandState)) {
				commandState = new CommandStateSource();
				_commandStates[key] = commandState;
			}
			commandState.State = value;
		}

		public IStateSource<Command> GetState(K key) {
			CommandStateSource commandState;
			_commandStates.TryGetValue(key, out commandState);
			return commandState;
		}
	}
}
