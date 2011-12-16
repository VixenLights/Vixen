using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	public interface IOutputSourceCollection {
		OutputSources GetOutputSources(Guid controllerId, int outputIndex);
		IEnumerable<Guid> Controllers { get; }
		IEnumerable<OutputSources> GetControllerSources(Guid controllerId);
	}
}
