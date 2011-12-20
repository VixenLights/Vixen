using System;

namespace Vixen.Module.Input {
	public interface IInput {
		event EventHandler<InputValueChangedEventArgs> InputValueChanged;
		IInputInput[] Inputs { get; }
		string DeviceName { get; }
	}
}
