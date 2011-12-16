using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys {
	class ControllerSources {
		public ControllerSources(OutputController controller) {
			ControllerId = controller.Id;
			OutputSources = Enumerable.Range(0, controller.OutputCount).ToDictionary(x => x, x => new OutputSources(x));
		}

		public Guid ControllerId { get; private set; }
		public Dictionary<int, OutputSources> OutputSources { get; private set; }
	}
}
