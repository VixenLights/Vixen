using System.Collections.Generic;
using Vixen.Script;

namespace Vixen.Sys {
	public class SourceFileCollection {
		public SourceFileCollection() {
			Files = new List<SourceFile>();
		}

		public string Directory { get; set; }
		public List<SourceFile> Files { get; private set; }
	}
}
