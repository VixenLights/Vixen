using System.Collections.Generic;

namespace Vixen.Sys {
	interface IHasOutputSources {
		void AddSource(IOutputStateSource source);
		void AddSources(IEnumerable<IOutputStateSource> sources);
		void RemoveSource(IOutputStateSource source);
		void ClearSources();
	}
}
