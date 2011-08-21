using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Transform {
	public class CommandsAffected : Dictionary<string, CommandParameterReference> {
		public CommandsAffected() {
		}

		public CommandsAffected(Dictionary<string, CommandParameterReference> dictionary)
			: base(dictionary) {
		}
	}
}
