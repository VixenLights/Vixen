using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Input {
	public class InputValueChangedEventArgs : EventArgs {
		public InputValueChangedEventArgs(IInputInput input) {
			Input = input;
		}

		public IInputInput Input { get; private set; }
	}
}
