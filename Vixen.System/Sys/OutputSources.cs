using System.Collections.Generic;

namespace Vixen.Sys {
	public class OutputSources : IEnumerable<IOutputStateSource> {
		public OutputSources(int outputIndex) {
			OutputIndex = outputIndex;
			Sources = new List<IOutputStateSource>();
		}

		public int OutputIndex { get; private set; }
		public List<IOutputStateSource> Sources { get; private set; }

		public IEnumerator<IOutputStateSource> GetEnumerator() {
			return Sources.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
