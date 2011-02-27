using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Input {
	public interface IInputInput {
		Guid Id { get; set; }
		string Name { get; set; }
		double Value { get; set; }
		event EventHandler ValueChanged;
		Command GetCommand();
	}
}
