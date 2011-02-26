using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using System.Threading;

namespace Vixen.Hardware {
	/// <summary>
	/// Stages a multi-output update to be latched to the hardware atomically.
	/// </summary>
	class OutputControllerBuffer {
		private OutputController _controller;
		private bool _modified;
		private CommandData[] _state;

		public OutputControllerBuffer(OutputController controller) {
			_controller = controller;
			_state = new CommandData[controller.OutputCount];
		}

		public void BeginTransaction() {
			// Lock the out buffer so nothing else can affect the controller state.
			Monitor.Enter(_state);
			_modified = false;
		}

		public void Write(int outputIndex, CommandData command) {
			// Write to the in buffer.
			if(outputIndex >= 0 && outputIndex < _controller.OutputCount) {
				_controller.ApplyTransforms(command, outputIndex);
				//*** need to merge the states, not overwrite blindly
				_state[outputIndex] = command;
				_modified = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>True if the state was modified.</returns>
		public bool Commit() {
			// Unlock the buffer.
			Monitor.Exit(_state);
			return _modified;
		}

		public CommandData[] GetOutputStates() {
			// Don't give the modules a reference to the buffer's array.
			return _state.ToArray();
		}

	}
}
