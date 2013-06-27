using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Input
{
	public class InputValueChangedEventArgs : EventArgs
	{
		public InputValueChangedEventArgs(IInputModuleInstance inputModule, IInputInput input)
		{
			InputModule = inputModule;
			Input = input;
		}

		public IInputModuleInstance InputModule { get; private set; }
		public IInputInput Input { get; private set; }
	}
}