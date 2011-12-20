using System;

namespace Vixen.Module.Input {
	public interface IInputEffectMap {
		Guid InputModuleId { get; set; }
		string InputId { get; set; }
		Guid EffectModuleId { get; set; }
		object[] EffectParameterValues { get; set; }
		int InputValueParameterIndex { get; set; }
	}
}
