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
			commandState.Value = value;
		}

		public IStateSource<Command> GetValue(K key) {
			CommandStateSource commandState;
			_commandStates.TryGetValue(key, out commandState);
			return commandState;
		}

		//private class CommandState : IStateSource<Command> {
		//    public Command Value { get; set; }
		//}
	}
}
