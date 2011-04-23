using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Execution;

namespace Vixen.Module.Timing {
	public interface ITiming : IExecutionControl {
		long Position { get; set; }
	}
}
