using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module.Sequence;

namespace Vixen.Module.Input {
	public interface IInput {
		bool Enabled { get; set; }
		IInputInput[] InputInputs { get; }
		void UpdateState();
	}
}
