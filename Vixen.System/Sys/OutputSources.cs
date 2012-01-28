using System.Collections.Generic;
using Vixen.Commands;

namespace Vixen.Sys {
	public class OutputSources : IEnumerable<IStateSource<Command>> {
		public OutputSources(int outputIndex) {
			OutputIndex = outputIndex;
			Sources = new List<IStateSource<Command>>();
		}

		public int OutputIndex { get; private set; }
		public List<IStateSource<Command>> Sources { get; private set; }

		public IEnumerator<IStateSource<Command>> GetEnumerator() {
			return Sources.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
