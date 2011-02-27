using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module.Sequence;

namespace Vixen.Module.Input {
	public interface IInput {
		//*** The input module needs to have a public member that exposes
		//    the Enabled property that needs to be in its data object.
		bool Enabled { get; set; }
		IInputInput[] InputInputs { get; }
		void UpdateState();
	}
}
