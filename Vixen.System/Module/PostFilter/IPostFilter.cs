using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;

namespace Vixen.Module.PostFilter {
	public interface IPostFilter {
		Command Affect(Command command);
	}
}
