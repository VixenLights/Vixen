using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Module.PostFilter {
	public interface IPostFilter : ISetup {
		Command Affect(Command command);
	}
}
