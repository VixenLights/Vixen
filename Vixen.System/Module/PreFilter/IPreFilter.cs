using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;

namespace Vixen.Module.PreFilter {
	public interface IPreFilter {
		Command Affect(Command command);
	}
}
