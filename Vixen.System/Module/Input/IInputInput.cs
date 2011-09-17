using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace Vixen.Module.Input {
	public interface IInputInput {
		event EventHandler ValueChanged;
		Guid Id { get; set; }
		string Name { get; set; }
		double Value { get; set; }
		IEffectModuleInstance Effect { get; }
	}
}
